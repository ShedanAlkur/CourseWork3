using ExpressionBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TestProject
{
    [TestClass]
    public class UnitTestMathFExpressionBuilder
    {
        [TestMethod]
        public void TestBuildExpression()
        {
            var parser = new MathFExpressionBuilder();

            string input = "sqrt(2 + 3) * round (9.2 ^ 14) + sin(3*pi/2)*e";
            float expectedResult = MathF.Sqrt(2 + 3) * MathF.Round(MathF.Pow(9.2f, 14)) + MathF.Sin(3 * MathF.PI / 2) * MathF.E;
            float result = (float)parser.CompileString(input).DynamicInvoke();

            Assert.AreEqual(expectedResult, result);

            Assert.AreEqual(0f, parser.CompileString("clamp (-1; 0; 1)").DynamicInvoke());
            Assert.AreEqual(-7f, parser.CompileString("min (12; -7)").DynamicInvoke());
            Assert.AreEqual(MathF.Log(10, MathF.E), parser.CompileString("log (10; e)").DynamicInvoke());
        }

        [TestMethod]
        public void TestParameters()
        {
            var parser = new MathFExpressionBuilder();
            parser.AllowAutomaticAddingParameters = true;

            string input = "var1 + var2 * var3";
            float[] vars = { 12, 52, 26 };
            float expectedResult = vars[0] + vars[1] * vars[2];
            Delegate del = parser.CompileString(input, "var0");
            vars = new float[] { 1000000000 }.Concat(vars).ToArray<float>();
            float result = (float)(del.DynamicInvoke((object?[]?)vars.Cast<object?>().ToArray()));

            Assert.AreEqual(expectedResult, result);
        }

        class TestClass
        {
            public float Field;
        }

        [TestMethod]
        public void TestImplicitParameters()
        {
            var obj = new TestClass();
            obj.Field = 123.534f;

            var parser = new MathFExpressionBuilder();

            var externalParam = System.Linq.Expressions.Expression.Parameter(typeof(TestClass));
            var internalParam = System.Linq.Expressions.Expression.PropertyOrField(externalParam, nameof(TestClass.Field));
            string internalParamName = "field";
            var intParam = new InternalImplicitParameter(internalParam, internalParamName);
            var impParam = new ImplicitParameter(externalParam, intParam);

            string input = internalParamName;

            float expectedResult = obj.Field;
            Delegate del = parser.CompileTokens(
                parser.SplitToTokens(input), new ImplicitParameter[] { impParam });
            float result = (float)(del.DynamicInvoke(obj));

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestRandom()
        {
            var parser = new MathFExpressionBuilder();
            string input = "random";
            float result = (float)parser.CompileString(input).DynamicInvoke();

            Assert.IsTrue(result >= 0 && result <= 1);
        }


        [TestMethod]
        public void TestLogicParsing()
        {
            var parser = new MathFExpressionBuilder();

            Assert.AreEqual(true, parser.CompileString("true").DynamicInvoke());
            Assert.AreEqual(!(1 > 2) && (1 < 8), parser.CompileString("!(1 > 2) && (1 < 8)").DynamicInvoke());
            Assert.AreEqual(false || false && true, parser.CompileString("false || false && true").DynamicInvoke());
            Assert.AreEqual(1 > 2 || 1 >= 2 || 1 == 1 || 1 != 1 || 1 < 1 || 1 <= 1,
                parser.CompileString("1 > 2 || 1 >= 2 || 1 == 1 || 1 != 1 || 1 < 1 || 1 <= 1").DynamicInvoke());
            Assert.AreEqual((2 > 1) ? 69f : 4221f, parser.CompileString("if (2 > 1; 69; 4221)").DynamicInvoke());
        }


        [TestMethod]
        public void TestDelegate()
        {
            Delegate test = (Func<int, int,int, int>)((int var1, int var2, int var3) => var1 + var2 + var3);
            Console.WriteLine(test.DynamicInvoke(new int[]{ 1, 2, 3}.Cast<object?>().ToArray()));
        }
    }
}
