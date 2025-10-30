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
    var authorizeAttribute = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();

    // اگر اکشن نیاز به لاگین نداشته باشه
    if (authorizeAttribute == null)
    {
        await _next(context);
        return;
    }

    // 1. احراز هویت
    if (!context.User.Identity?.IsAuthenticated ?? true)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("دسترسی غیرمجاز: توکن نامعتبر.");
        return;
    }

    // 2. استخراج userId
    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!int.TryParse(userIdClaim, out int userId))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("توکن نامعتبر: شناسه کاربر یافت نشد.");
        return;
    }

    context.Items["UserId"] = userId;

    // 3. استخراج پرمیشن مورد نیاز از Attribute سفارشی
    var requirePermissionAttribute = endpoint?.Metadata.GetMetadata<RequirePermissionAttribute>();
    if (requirePermissionAttribute != null)
    {
        bool hasPermission = await permissionService.HasPermissionAsync(userId, requirePermissionAttribute.Permission);
        if (!hasPermission)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync($"دسترسی ممنوع: شما اجازه '{requirePermissionAttribute.Permission}' را ندارید.");
            return;
        }
    }

    await _next(context);
}
    }

    // Extension برای ثبت Middleware
    public static class TokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }
}
