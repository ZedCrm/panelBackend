using API.Attributes;
using App.Contracts.Object.Base.auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenValidationMiddleware> _logger;

        public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            var endpoint = context.GetEndpoint();

            // اگر endpoint وجود نداشت یا نیازی به authorize نداشت
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var authorizeAttribute = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();

            // اگر اکشن نیازی به لاگین نداشته باشد
            if (authorizeAttribute == null && !RequiresPermission(endpoint))
            {
                await _next(context);
                return;
            }

            // بررسی احراز هویت
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("دسترسی غیرمجاز: توکن نامعتبر.");
                return;
            }

            // استخراج userId
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("توکن نامعتبر: شناسه کاربر یافت نشد.");
                return;
            }

            context.Items["UserId"] = userId;

            // ================================================
            // روش اول: بررسی RequirePermissionAttribute (اگر روی اکشن یا کنترلر باشد)
            // ================================================
            var requirePermissionAttr = endpoint?.Metadata.GetMetadata<RequirePermissionAttribute>();
            if (requirePermissionAttr != null)
            {
                bool hasPermission = await permissionService.HasPermissionAsync(userId, requirePermissionAttr.Permission);
                if (!hasPermission)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync($"دسترسی ممنوع: شما اجازه '{requirePermissionAttr.Permission}' را ندارید.");
                    return;
                }
            }

            // ================================================
            // روش دوم: بررسی خودکار بر اساس نام کنترلر و اکشن (برای GenericControllerها)
            // ================================================
            var routeData = context.GetRouteData();
            var controllerName = routeData?.Values["controller"]?.ToString();
            var actionName = routeData?.Values["action"]?.ToString();

            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
            {
                string? requiredPermission = GetPermissionFromAction(controllerName, actionName);

                if (!string.IsNullOrEmpty(requiredPermission))
                {
                    bool hasPermission = await permissionService.HasPermissionAsync(userId, requiredPermission);
                    if (!hasPermission)
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync($"شما دسترسی لازم را ندارید: {requiredPermission}");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private static bool RequiresPermission(Endpoint? endpoint)
        {
            if (endpoint == null) return false;
            return endpoint.Metadata.GetMetadata<RequirePermissionAttribute>() != null;
        }

        private static string? GetPermissionFromAction(string controllerName, string actionName)
        {
            // حذف کلمه "Controller" از نام کنترلر
            string cleanController = controllerName.Replace("Controller", "");

            return actionName switch
            {
                "GetAll" or "GetById" => $"{cleanController}.View",
                "Create" => $"{cleanController}.Create",
                "Update" => $"{cleanController}.Update",
                "Delete" => $"{cleanController}.Delete",
                _ => null
            };
        }
    }

    public static class TokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }
}