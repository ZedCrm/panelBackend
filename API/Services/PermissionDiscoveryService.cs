// API/Services/PermissionDiscoveryService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Attributes;
using API.Controllers; // برای دسترسی به GenericController
using App.Contracts.Object.Base.auth;
using Microsoft.AspNetCore.Mvc;

namespace API.Services
{
    public class PermissionDiscoveryService : IPermissionDiscoveryService
    {
        public IEnumerable<string> GetAllPermissionNames()
        {
            var permissions = new HashSet<string>();
            var assembly = Assembly.GetExecutingAssembly();

            var controllers = assembly.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var controller in controllers)
            {
                // 1. مجوزهای صریح روی کلاس
                var classAttrs = controller.GetCustomAttributes<RequirePermissionAttribute>(true);
                foreach (var attr in classAttrs)
                    permissions.Add(attr.Permission);

                // 2. مجوزهای صریح روی متدها
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    var methodAttrs = method.GetCustomAttributes<RequirePermissionAttribute>(true);
                    foreach (var attr in methodAttrs)
                        permissions.Add(attr.Permission);
                }

                // 3. مجوزهای ضمنی برای کنترلرهایی که از GenericController ارث برده‌اند
                if (IsDerivedFromGenericController(controller))
                {
                    string controllerName = GetControllerBaseName(controller);

                    // مجوزهای استاندارد CRUD
                    permissions.Add($"{controllerName}.View");
                    permissions.Add($"{controllerName}.Create");
                    permissions.Add($"{controllerName}.Update");
                    permissions.Add($"{controllerName}.Delete");
                }
            }

            return permissions;
        }

        /// <summary>
        /// بررسی می‌کند آیا کنترلر از GenericController مشتق شده است.
        /// </summary>
        private bool IsDerivedFromGenericController(Type controllerType)
        {
            var baseType = controllerType.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                // GenericController دارای 4 نوع پارامتر است → باید 3 کاما داشته باشد
                if (baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == typeof(GenericController<,,,>))
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// نام پایه کنترلر (حذف پسوند "Controller") را برمی‌گرداند.
        /// </summary>
        private string GetControllerBaseName(Type controllerType)
        {
            string name = controllerType.Name;
            if (name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
                return name.Substring(0, name.Length - 10);
            return name;
        }
    }
}