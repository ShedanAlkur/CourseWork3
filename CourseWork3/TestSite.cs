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
    
    }
}
