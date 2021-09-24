using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CourseWork3
{
    class MyActionBuilder
    {
        /// <summary>
        /// Словарь поддерживаемых бинарных операторов.
        /// </summary>
        static public readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> operators
            = new Dictionary<string, Func<Expression, Expression, BinaryExpression>>()
            { 
                ["+"] = Expression.Add,
                ["-"] = Expression.Subtract,
                ["*"] = Expression.Multiply,
                ["/"] = Expression.Divide,
                ["^"] = Expression.Power,
            };

        /// <summary>
        /// Словарь поддерживаемых функций одной переменной.
        /// </summary>
        static public readonly Dictionary<string, Func<Expression, Expression>> functions
            = new Dictionary<string, Func<Expression, Expression>>()
            {
                ["sqrt"] = GetExpressionFromMethod("Sqrt", typeof(Math)),
                ["sqr"] = GetExpressionFromMethod("Sqr", typeof(MyActionBuilder)),
                ["sin"] = GetExpressionFromMethod("Sin", typeof(Math)),
                ["cos"] = GetExpressionFromMethod("Cos", typeof(Math)),
                ["tg"] = GetExpressionFromMethod("Tan", typeof(Math)),
                ["abs"] = GetExpressionFromMethod("Abs", typeof(MyActionBuilder)),
            };

        /// <summary>
        /// Словарь поддерживаемых математический констант.
        /// </summary>
        static public readonly Dictionary<string, Expression> Constants
            = new Dictionary<string, Expression>()
            {
                ["pi"] = Expression.Constant(Math.PI, typeof(double)),
                ["e"] = Expression.Constant(Math.E, typeof(double)),
            };

        static public Expression CreateParameter(string name)
        {
            return Expression.Parameter(typeof(double), name);
        }

        /// <summary>
        /// Метод создает из метода одной переменной заданного класса делегат, который принимает и возвращает Expression.
        /// </summary>
        /// <param name="method">Имя метода.</param>
        /// <param name="containingClass">Класс, содержащий метод. Вариант использования: typeof(Math) </param>
        /// <returns>Полученный из метода делегат.</returns>
        static private Func<Expression, Expression> GetExpressionFromMethod(string method, Type containingClass)
        {
            return (Expression value) =>  Expression.Call(
                containingClass.GetMethod(method,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                , value);
        }

        static private double Abs(double value) => Math.Abs(value);
        static private double Sqr(double value) => value * value;
    }
}
