using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyFrameWork.AppTool
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var combined = Expression.Invoke(expr1, parameter);
            var second = Expression.Invoke(expr2, parameter);
            var body = Expression.AndAlso(combined, second);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
