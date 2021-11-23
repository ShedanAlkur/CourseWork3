using NUnit.Framework;
using System.Diagnostics;

namespace TestProject1
{
    public class Tests
    {
        Stopwatch sw;
        TestClass obj;
        System.Action<TestClass, int> actionSetter;
        System.Delegate delegateSetter;
        int countOfTests;

        [SetUp]
        public void Setup()
        {
            sw = new Stopwatch();
            obj = new TestClass();
            actionSetter = ExpressionBuilder.ExpressionHelper.CreateSetter<TestClass, int>("Property");
            delegateSetter = ExpressionBuilder.ExpressionHelper.CreateSetterByType("Property",
                typeof(TestClass), typeof(int));
            countOfTests = 1000000;
        }

        [Test]
        public void Test1()
        {
            sw.Reset();
            sw.Start();
            for (int i = 0; i < countOfTests; i++)
            {
                actionSetter(obj, i);
            }
            sw.Stop();
            System.Console.WriteLine($"{nameof(actionSetter)} time of {countOfTests} counts = {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"one set in {(float)sw.ElapsedMilliseconds / countOfTests}ms");

            sw.Reset();
            sw.Start();
            for (int i = 0; i < countOfTests; i++)
            {
                delegateSetter.DynamicInvoke(obj, i);
            }
            sw.Stop();
            System.Console.WriteLine($"{nameof(actionSetter)} time of {countOfTests} counts = {sw.ElapsedMilliseconds} ms");
            System.Console.WriteLine($"one set in {(float)sw.ElapsedMilliseconds / countOfTests}ms");

            Assert.Pass();
        }
    }

    class TestClass
    {
        public int Field;
        public int Property { get; set; }
    }
}