// API/Services/PermissionDiscoveryService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Attributes;
using App.Contracts.Object.Base.auth;
using Microsoft.AspNetCore.Mvc;

namespace API.Services
{
    public class PermissionDiscoveryService : IPermissionDiscoveryService
    {
        public IEnumerable<string> GetAllPermissionNames()
        {
            var permissions = new HashSet<string>();
            var assembly = Assembly.GetExecutingAssembly(); // فقط اسمبلی API

            var controllers = assembly.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var controller in controllers)
            {
                // پرمیشن‌های روی کلاس
                var classAttrs = controller.GetCustomAttributes<RequirePermissionAttribute>(true);
                foreach (var attr in classAttrs)
                    permissions.Add(attr.Permission);

                // پرمیشن‌های روی متدها
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    var methodAttrs = method.GetCustomAttributes<RequirePermissionAttribute>(true);
                    foreach (var attr in methodAttrs)
                        permissions.Add(attr.Permission);
                }
            }

            return permissions;
        }
    }
}