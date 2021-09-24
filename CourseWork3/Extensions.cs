using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CourseWork3
{
    static class Extensions
    {
        static IEnumerable<T> WhereLike<T>(
        this IEnumerable<T> data,
        string propertyOrFieldName,
        string value)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.Call(
                typeof(Program).GetMethod("Like",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public),
                    Expression.PropertyOrField(param, propertyOrFieldName),
                    Expression.Constant(value, typeof(string)));
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return data.Where(lambda.Compile());
        }
        static bool Like(string a, string b)
        {
            return a.Contains(b); // just for illustration
        }
    }
}
