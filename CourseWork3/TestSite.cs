using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CourseWork3
{
    class TestSite
    {
        #region like-expression methods
        public static Expression Like(Expression lhs, Expression rhs)
        {
            MethodInfo? method = typeof(TestSite).GetMethod("Like123",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return Expression.Call(method, lhs, rhs);
        }

        public static Expression Like2(Expression lhs, Expression rhs)
        {
            // str1.Contains(str2)
            Expression op = Expression.Call(lhs, "Contains", Type.EmptyTypes, rhs);
            return op;
        }

        public static bool testLike(string a, string b)
        {
            return a.Contains(b); // just for illustration
        }
        public static bool Like123(string a, string b)
        {
            return a.Contains(b); // just for illustration
        }
        #endregion

        public static void ExpFieldTest()
        {
            var setter1 = ExpressionBuilder.ExpressionHelper.CreateSetter<TestClass, int>("Property");
            var setter2 = ExpressionBuilder.ExpressionHelper.CreateSetterByType("Property", typeof(TestClass), typeof(int));

            var test = new TestClass();
            Console.WriteLine($"{nameof(test.Property)} = {test.Property}");
            setter1(test, 69);
            Console.WriteLine($"{nameof(test.Property)} = {test.Property}");
            ((Action<TestClass, int>)setter2)(test, 70);
            Console.WriteLine($"{nameof(test.Property)} = {test.Property}");
        }


        public static void TestTest()
        {
            var item = Expression.Parameter(typeof(Projectile), "item");
            var field = Expression.PropertyOrField(item, nameof(Projectile.CurrentRuntime));
            var res = ("123",field);
            Console.WriteLine();
        }

    }


    class TestClass
    {
        public int Field;
        public int Property { get; set; }                
    }
}
