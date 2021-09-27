using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CourseWork3
{
    class Program
    {
        static void Main(string[] args)
        {
            RandomDelegateTest();


            Console.Read();
        }


        static void MyActionBuilderTest()
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
        }

        static void RandomDelegateTest()
        {
            MyActionBuilder actionBuilder = new MyActionBuilder();
            Stopwatch sw = new Stopwatch();
            Random rnd = new Random();
            actionBuilder.CompileString("round (random * 100)");
            var del = actionBuilder.ResultDelegate;
            double numberOfTests = 1000000;

             double www() => Math.Round(rnd.NextDouble() * 100);

            sw.Restart();
            for (int i = 0; i < numberOfTests; i++)
                www();
            sw.Stop();
            Console.WriteLine($"Среднее время вызова функции - {sw.ElapsedTicks / numberOfTests} тиков");

            sw.Restart();
            for (int i = 0; i < numberOfTests; i++)
                del.DynamicInvoke();
            sw.Stop();
            Console.WriteLine($"Среднее время вызова делегата - {sw.ElapsedTicks / numberOfTests} тиков");

            Console.WriteLine(del.DynamicInvoke());
        }

    }
}
