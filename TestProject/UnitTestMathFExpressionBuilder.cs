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
        }

        [TestMethod]
        public void TestParameters()
        {
            var parser = new MathFExpressionBuilder();

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
        public void TestNestedParameters()
        {
            var obj = new TestClass();
            obj.Field = 123.534f;

            var parser = new MathFExpressionBuilder();


            var externalParam = System.Linq.Expressions.Expression.Parameter(typeof(TestClass));
            var internalParam = System.Linq.Expressions.Expression.PropertyOrField(externalParam, nameof(TestClass.Field));
            string internalParamName = "field";

            var nestedParam = new NestedExpressionParameter(externalParam, internalParam, internalParamName);
            string input = internalParamName;

            float expectedResult = obj.Field;
            Delegate del = parser.CompileTokens(
                parser.SplitToTokens(input), new NestedExpressionParameter[] { nestedParam });
            float result = (float)(del.DynamicInvoke(obj));

            Assert.AreEqual(expectedResult, result);
        }
    }
}
