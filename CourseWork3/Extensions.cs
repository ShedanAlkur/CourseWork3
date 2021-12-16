using OpenTK;
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

    public static class Vector2Ext
    {
        /// <summary>
        /// Создает радиус-вектор по заданному углу наклона.
        /// </summary>
        /// <param name="angle">Угол, измеряемый в радианах.</param>
        public static Vector2 ByAngle(float angle)
            => new Vector2(MathF.Cos(angle), MathF.Sin(angle));

        /// <summary>
        /// Определяет угол тангенса, соответствующего координатам входного вектора.
        /// </summary>
        /// <param name="vector">Вектор, между координатами которого будет находится угол.</param>
        /// <returns>Угол θ, измеряемый в радианах. Такой, что -π ≤ θ ≤ π и  tan(θ) = y / x</returns>
        public static float GetAngle(this Vector2 vector)
            => MathF.Atan2(vector.Y, vector.X);

        public static Vector2 FastNormilize(this Vector2 vector)
        {
            float lenght = vector.LengthFast;
            return new Vector2(vector.X / lenght, vector.Y / lenght);
        }

        public static float Sqr(this float value)
            => value * value;
    }
}
