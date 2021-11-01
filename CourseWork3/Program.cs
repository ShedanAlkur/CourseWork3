using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CourseWork3.Game;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Patterns;
using CourseWork3.Game;
using OpenTK;

namespace CourseWork3
{
    class Program
    {
        static void Main(string[] args)
        {
            using (GameWindow window = new GameWindow(800, 600))
            {
                GameMain.Init(window);
            }
        }

        static void InfiniteInput()
        {
            while (true)
            {
                MyActionBuilder actionBuilder = new MyActionBuilder();
                try
                {
                    Console.WriteLine(actionBuilder.CompileString(Console.ReadLine()).DynamicInvoke());
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
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
            actionBuilder.CompileString("round (random * 100) * i");
            var del = actionBuilder.ResultDelegate;
            double numberOfTests = 1000000;

             double www(double i) => Math.Round(rnd.NextDouble() * 100) * i;

            sw.Restart();
            for (int i = 0; i < numberOfTests; i++)
                www(i);
            sw.Stop();
            Console.WriteLine($"Среднее время вызова функции - {sw.ElapsedTicks / numberOfTests} тиков");

            sw.Restart();
            for (int i = 0; i < numberOfTests; i++)
                del.DynamicInvoke(i);
            sw.Stop();
            Console.WriteLine($"Среднее время вызова delegate.DynamicInvoke - {sw.ElapsedTicks / numberOfTests} тиков");

            Func<double, double> delToFunc = (Func<double, double>)actionBuilder.ResultDelegate;
            sw.Restart();
            for (int i = 0; i < numberOfTests; i++)
                delToFunc.DynamicInvoke(i);
            sw.Stop();
            Console.WriteLine($"Среднее время вызова delegate->func.DynamicInvoke - {sw.ElapsedTicks / numberOfTests} тиков");

            sw.Restart();
            for (int i = 0; i < numberOfTests; i++)
                delToFunc.Invoke(i);
            sw.Stop();
            Console.WriteLine($"Среднее время вызова delegate->func.Invoke - {sw.ElapsedTicks / numberOfTests} тиков");
        }

    }
}
