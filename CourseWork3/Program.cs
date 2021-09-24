using System;
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
            MyActionBuilder actionBuilder = new MyActionBuilder();
            var tokens = actionBuilder.SplitToTokens("(10 * 3 - sin(pi / 2))^2 ");
            var RPN = actionBuilder.ConvertToRPN(tokens);
            Console.WriteLine(string.Join(" ", RPN));
            Console.WriteLine(actionBuilder.CompileString("(10 * 3 -  ((()))  SIN(pi / 2))^2 ").DynamicInvoke()); // 841

            Console.WriteLine(actionBuilder.CompileString("10 * B + A", new string[] { "A", "B", "C" }).DynamicInvoke(9, 6, 777)); // 69
            Console.WriteLine(actionBuilder.CompileString("10 * B + A").DynamicInvoke(9, 6)); // 96

            actionBuilder.CompileString("A + pi * c");
            Console.WriteLine(string.Join(" ", actionBuilder.Parameters)); // [a, c]


            Console.Read();
        }


    }
}
