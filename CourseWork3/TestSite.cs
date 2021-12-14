using CourseWork3.Game;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Parser;
using CourseWork3.Patterns;
using ExpressionBuilder;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static CourseWork3.Game.GameMain;

namespace CourseWork3
{
    class TestClass
    {
        public float Field;
        public float Property { get; set; }                
    }
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
            var externalParam = Expression.Parameter(typeof(TestClass));
            var internalParam = Expression.PropertyOrField(externalParam, nameof(TestClass.Field));
            var internalParamName = "param";
            var param = new ExpressionBuilder.NestedExpressionParameter(externalParam, internalParam, internalParamName);

            var expBuilder = new ExpressionBuilder.MathFExpressionBuilder();
            var del = expBuilder.CompileTokens(expBuilder.SplitToTokens("param * 2"),
                new ExpressionBuilder.NestedExpressionParameter[] { param });
            var foo = (Func<TestClass, float>)del;

            var clas = new TestClass();
            Console.WriteLine(foo(clas));
            clas.Field = 1;
            Console.WriteLine(foo(clas));
            clas.Field = 69;
            Console.WriteLine(foo(clas));
        }

        public static void LexerTest()
        {
            Lexer.SplitToTokensFromFile(@"Content\fileForParser.txt");
        }

        public static void ControlledObjectSetterTest()
        {
            // на релизе вариант со словарями быстрее в 2 раз
            var sw = new Stopwatch();
            var countOfTests = 1000000;

            sw.Reset();
            sw.Start();
            var pattern = new Pattern<Projectile>(new ICommand<Projectile>[0]);
            //var proj = new Projectile(pattern);
            Action<Projectile, object> act1 = (Projectile obj, object value) => obj.Velocity = (Vector2)value;
            for (int i = 0; i < countOfTests; i++)
            {
                //act1(proj, Vector2.Zero);
            };
            sw.Stop();
            System.Console.WriteLine($"set-position time of {countOfTests} counts = {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"one set in {(float)sw.ElapsedMilliseconds / countOfTests} ms");

            sw.Reset();
            sw.Start();
            Action<Projectile, Vector2> act2 = ExpressionHelper.CreateSetter<Projectile, Vector2>("Velocity");
            for (int i = 0; i < countOfTests; i++)
            {
                //act2(proj, Vector2.Zero);
            }
            sw.Stop();
            System.Console.WriteLine($"set-velocity time of {countOfTests} counts = {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"one set in {(float)sw.ElapsedMilliseconds / countOfTests} ms");
        }

        public static void InfiniteInput()
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

        public static void MyActionBuilderTest()
        {
            var actionBuilder = new MathFExpressionBuilder();
            var tokens = actionBuilder.SplitToTokens("(10 * 3 - sin(pi / 2))^2 ");
            var RPN = actionBuilder.ConvertToRPN(tokens);
            Console.WriteLine(string.Join(" ", RPN));
            Console.WriteLine(actionBuilder.CompileString("(10 * 3 -  ((()))  SIN(pi / 2))^2 ").DynamicInvoke()); // 841

            Console.WriteLine(actionBuilder.CompileString("10 * B + A", new string[] { "A", "B", "C" }).DynamicInvoke(9, 6, 777)); // 69
            Console.WriteLine(actionBuilder.CompileString("10 * B + A").DynamicInvoke(9, 6)); // 96

            actionBuilder.CompileString("A + pi * c");
            Console.WriteLine(string.Join(" ", actionBuilder.Parameters)); // [a, c]
        }

        public static void RandomDelegateTest()
        {
            var actionBuilder = new MathFExpressionBuilder();
            Stopwatch sw = new Stopwatch();
            Random rnd = new Random();
            actionBuilder.CompileString("round (random * 100) * i");
            var del = actionBuilder.ResultDelegate;
            float numberOfTests = 100000000;

            float www(float i) => (float)Math.Round(rnd.NextDouble() * 100) * i;

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

            var delToFunc = (Func<float, float>)actionBuilder.ResultDelegate;
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

            sw.Restart();
            for (int i = 0; i < numberOfTests; i++)
                delToFunc(i);
            sw.Stop();
            Console.WriteLine($"Среднее время вызова delegate->func() - {sw.ElapsedTicks / numberOfTests} тиков");
        }

        internal static void ParserTest()
        {
            GameMain.SpriteCollection = new Dictionary<string, Sprite>();
            GameMain.ProjeciltePatternCollection = new Dictionary<string, Pattern<Projectile>>();
            GameMain.GeneratorPatternCollection = new Dictionary<string, Pattern<Generator>>();
            GameMain.EnemyPatternCollection = new Dictionary<string, Pattern<Enemy>>();
            var parser = new Parser.Parser();
            parser.ParseFile(@"Content\fileForParser.txt");
            
            throw new NotImplementedException();
        }
    }
   

}
