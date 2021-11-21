using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionBuilder
{    
    public class ExpressionHelper
    {
        /// <summary>
        /// Создает экземпляр ParameterExpression.
        /// </summary>
        /// <typeparam name="T">Тип параметра.</typeparam>
        /// <param name="name">Имя параметра.</param>
        /// <returns>Параметр ParameterExpression.</returns>
        public static ParameterExpression CreateParameter<T>(string name) =>
            Expression.Parameter(typeof(T), name);

        /// <summary>
        /// Создает экземпляр ConstantExpression.
        /// </summary>
        /// <typeparam name="T">Тип константы.</typeparam>
        /// <param name="value">Значение константы.</param>
        /// <returns>Константа ConstantExpression.</returns>
        public static ConstantExpression CreateConstant<T>(T value) =>
            Expression.Constant(value, typeof(T));

        /// <summary>
        /// Создает сеттер для поля или свойства заданного класса.
        /// </summary>
        /// <typeparam name="O">Тип класса, содержащего изменяемое поле или свойство.</typeparam>
        /// <typeparam name="P">Тип изменяемого поля или свойства.</typeparam>
        /// <param name="propertyOrFieldName">Имя поля или свойства.</param>
        /// <returns>Сеттер.</returns>
        public static Action<O, P> CreateSetter<O, P>(string propertyOrFieldName)
        {
            var item = Expression.Parameter(typeof(O), "item");
            var value = Expression.Parameter(typeof(P), "value");
            var propertyOrField = Expression.PropertyOrField(item, propertyOrFieldName);
            var assign = Expression.Assign(propertyOrField, value);

            var expr = Expression.Block(assign, Expression.Empty());

            return (Action<O, P>)Expression.Lambda(expr, item, value).Compile();
        }

        public static Delegate CreateSetter(string propertyOrFieldName, Type typeOfObject, Type typeOfField)
        {
            var item = Expression.Parameter(typeOfObject, "item");
            var value = Expression.Parameter(typeOfField, "value");
            var propertyOrField = Expression.PropertyOrField(item, propertyOrFieldName);
            var assign = Expression.Assign(propertyOrField, value);

            var expr = Expression.Block(assign, Expression.Empty());

            return Expression.Lambda(expr, item, value).Compile();
        }

        /// <summary>
        /// Создает инкриментор для поля или свойства заданного класса.
        /// </summary>
        /// <typeparam name="O">Тип класса, содержащего изменяемое поле или свойство.</typeparam>
        /// <typeparam name="P">Тип изменяемого поля или свойства.</typeparam>
        /// <param name="propertyOrFieldName">Имя поля или свойства.</param>
        /// <returns>Инкриментор.</returns>
        public static Action<O, P> CreateInrementor<O, P>(string propertyOrFieldName)
        {
            var item = Expression.Parameter(typeof(O), "item");
            var value = Expression.Parameter(typeof(P), "value");
            var propertyOrField = Expression.PropertyOrField(item, propertyOrFieldName);
            var addAssign = Expression.AddAssign(propertyOrField, value);

            var expr = Expression.Block(addAssign, Expression.Empty());

            return (Action<O, P>)Expression.Lambda(expr, item, value).Compile();
        }

        /// <summary>
        /// Создает из функции одной переменной заданного класса делегат, который принимает и возвращает expression.
        /// </summary>
        /// <param name="containingClass">Класс, содержащий метод.</param>
        /// <param name="methodName">Имя метода</param>
        /// <returns>Полученный из метода делегат.</returns>
        public static Func<Expression, Expression> CreateExpressionFromUnaryFunc(Type containingClass, string methodName)
        {
            return (Expression arg1) => Expression.Call(
                containingClass.GetMethod(methodName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                , arg1);
        }

        /// <summary>
        /// Создает из функции двух переменных заданного класса делегат, который принимает и возвращает expression.
        /// </summary>
        /// <param name="containingClass">Класс, содержащий метод.</param>
        /// <param name="methodName">Имя метода</param>
        /// <returns>Полученный из метода делегат.</returns>
        public static Func<Expression, Expression, Expression> CreateExpressionFromBinaryFunc(Type containingClass, string methodName)
        {
            return (Expression arg1, Expression arg2) => Expression.Call(
                containingClass.GetMethod(methodName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                , arg1, arg2);
        }
    }
}
