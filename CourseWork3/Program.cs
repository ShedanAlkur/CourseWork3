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
using ExpressionBuilder;

namespace CourseWork3
{
    class Program
    {
        static void Main(string[] args)
        {
            ControlledObjectSetterTest();
            return;

            using (GameWindow window = new GameWindow(800, 600))
            {
                GameMain.Init(window);
            }
        }

        static void ControlledObjectSetterTest()
        {
            // на релизе вариант со словарями быстрее в 2 раз
            var sw = new Stopwatch();
            var countOfTests = 1000000;

            sw.Reset();
            sw.Start();
            var pattern = new Pattern<Projectile>(new ICommand<Projectile>[0]);
            var proj = new Projectile(pattern);
            Action<Projectile, object> act1 = (Projectile obj, object value) => obj.Velocity = (Vector2)value;
            for (int i = 0; i < countOfTests; i++)
            {
                act1(proj, Vector2.Zero);
            };
            sw.Stop();
            System.Console.WriteLine($"set-position time of {countOfTests} counts = {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"one set in {(float)sw.ElapsedMilliseconds / countOfTests} ms");

            sw.Reset();
            sw.Start();
            Action<Projectile, Vector2> act2 = ExpressionHelper.CreateSetter<Projectile, Vector2>("Velocity");
            for (int i = 0; i < countOfTests; i++)
            {
                act2(proj, Vector2.Zero);
            }
            sw.Stop();
            System.Console.WriteLine($"set-velocity time of {countOfTests} counts = {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"one set in {(float)sw.ElapsedMilliseconds / countOfTests} ms");
        }

        static void ControlledObjectConstructTest()
        {
            // на релизе вариант со словарями медленнее в 9 раз
            var sw = new Stopwatch();
            var countOfTests = 100000;

            sw.Reset();
            sw.Start();
            var temp = new Pattern<Projectile>(new ICommand<Projectile>[0]);
            for (int i = 0; i < countOfTests; i++)
            {
                new ControlledObject<Projectile>(temp);          
            };
            sw.Stop();
            System.Console.WriteLine($"{nameof(ControlledObject<Projectile>)} time of {countOfTests} counts = {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"one set in {(float)sw.ElapsedMilliseconds / countOfTests} ms");

            sw.Reset();
            sw.Start();
            for (int i = 0; i < countOfTests; i++)
            {
                new ControlledObject2<Projectile>(temp);
            }
            sw.Stop();
            System.Console.WriteLine($"{nameof(ControlledObject<Projectile>)} time of {countOfTests} counts = {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"one set in {(float)sw.ElapsedMilliseconds / countOfTests} ms");
        }

        static void InfiniteInput()
        {
            while (true)
            {
                var actionBuilder = new MathExpressionBuilder();
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
            var actionBuilder = new MathExpressionBuilder();
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
            var actionBuilder = new MathExpressionBuilder();
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
