// MyFrameWork/AppTool/QueryableExtensions.cs
using System.Linq.Expressions;
using System.Reflection;

namespace MyFrameWork.AppTool
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// مرتب‌سازی پویا روی هر نوع IQueryable
        /// </summary>
        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> query, 
            string? sortBy, 
            bool sortDirection = true)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = GetProperty(typeof(T), sortBy);
            
            if (property == null)
                return query;

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortDirection ? "OrderBy" : "OrderByDescending";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType);

            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }

        /// <summary>
        /// مرتب‌سازی با پشتیبانی از ThenBy
        /// </summary>
        public static IOrderedQueryable<T> ApplyOrderBy<T>(
            this IQueryable<T> query, 
            string sortBy, 
            bool sortDirection = true)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = GetProperty(typeof(T), sortBy);
            
            if (property == null)
                return (IOrderedQueryable<T>)query;

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortDirection ? "OrderBy" : "OrderByDescending";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType);

            return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }

        /// <summary>
        /// ThenBy برای مرتب‌سازی چند مرحله‌ای
        /// </summary>
        public static IOrderedQueryable<T> ThenBySorting<T>(
            this IOrderedQueryable<T> query, 
            string sortBy, 
            bool sortDirection = true)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = GetProperty(typeof(T), sortBy);
            
            if (property == null)
                return query;

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortDirection ? "ThenBy" : "ThenByDescending";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType);

            return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }

        private static PropertyInfo? GetProperty(Type type, string propertyName)
        {
            // پشتیبانی از خاصیت‌های تو در تو مثل "User.FullName"
            if (propertyName.Contains('.'))
            {
                var parts = propertyName.Split('.');
                var currentType = type;
                PropertyInfo? property = null;
                
                foreach (var part in parts)
                {
                    property = currentType.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property == null) return null;
                    currentType = property.PropertyType;
                }
                return property;
            }

            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}