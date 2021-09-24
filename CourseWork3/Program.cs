﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CourseWork3
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<ParameterExpression> expParams = new List<ParameterExpression>();
            //MyActionBuilder actionBuilder = new MyActionBuilder();
            //Expression test = Expression.Constant(-2.0, typeof(double));
            //Console.WriteLine(Expression.Lambda(test, expParams).Compile().DynamicInvoke());

            var tokens = MyActionBuilder.SplitToTokens("3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3");
            //foreach (var token in tokens) Console.WriteLine(token);
            MyActionBuilder actionBuilder = new MyActionBuilder();
            var RPN = actionBuilder.ConvertToRPN(tokens);

            Console.WriteLine(string.Join(" ", RPN));

            Console.Read();
        }


    }
}
