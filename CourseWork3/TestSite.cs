using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CourseWork3
{
    class TestSite
    {

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
    

        public static void ExpFieldTest()
        {
            var obj = new TestClass();
            Console.WriteLine($"obj.Field = {obj.Field}");

            var act = CreateSetter<TestClass, int>("Field");
            act(obj, 71);
            Console.WriteLine($"obj.Field = {obj.Field}");

            var act2 = CreateInrementor<TestClass, int>("Field");
            act2(obj, 69);

            Console.WriteLine($"obj.Field = {obj.Field}");

        }

        public static Action<O, P> CreateSetter<O, P>(string propertyOrFieldName)
        {
            var item = Expression.Parameter(typeof(O), "item");
            var value = Expression.Parameter(typeof(P), "value");
            var propertyOrField = Expression.PropertyOrField(item, propertyOrFieldName);
            var assign = Expression.Assign(propertyOrField, value);

            var expr = Expression.Block(assign, Expression.Empty());

            return (Action<O, P>)Expression.Lambda(expr, item, value).Compile();
        }

        public static Action<O, P> CreateInrementor<O, P>(string propertyOrFieldName)
        {
            var item = Expression.Parameter(typeof(O), "item");
            var value = Expression.Parameter(typeof(P), "value");
            var propertyOrField = Expression.PropertyOrField(item, propertyOrFieldName);

            var assign = Expression.AddAssign(propertyOrField, value);

            var expr = Expression.Block(assign, Expression.Empty());

            return (Action<O, P>)Expression.Lambda(expr, item, value).Compile();
        }


    }


    class TestClass
    {
        public int Field;
        public int Property { get; set; }                
    }
}
