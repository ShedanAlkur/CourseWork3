using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CourseWork3
{
    class Program
    {
        static double sqr(double value) => value * value;

        static void Main(string[] args)
        {
            List<ParameterExpression> expParams = new List<ParameterExpression>();

            Expression test = MyActionBuilder.functions["sin"](
                MyActionBuilder.operators["/"](
                    MyActionBuilder.Constants["pi"], 
                    Expression.Constant(-2.0, typeof(double))));

            Console.WriteLine(Expression.Lambda(test, expParams).Compile().DynamicInvoke());
            Console.Read();
        }


    }
}
