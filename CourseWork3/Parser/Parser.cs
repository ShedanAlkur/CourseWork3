using CourseWork3.Game;
using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CourseWork3.Parser
{
    class Parser
    {
        private ExpressionBuilder.MathFExpressionBuilder MathExpressionBuilder = new ExpressionBuilder.MathFExpressionBuilder();

        private static ExpressionBuilder.NestedExpressionParameter[] nestedParametersForProjectile;

        private static string[] paramsName = { Keywords.ProjParamName }; 

        static Parser()
        {
            var externalParam = System.Linq.Expressions.Expression.Parameter(typeof(Projectile));
            var internalParam = System.Linq.Expressions.Expression.PropertyOrField(externalParam, nameof(Projectile.CurrentRuntime));
            var internalParamName = Keywords.Runtime;
            var param = new ExpressionBuilder.NestedExpressionParameter(externalParam, internalParam, internalParamName);
            nestedParametersForProjectile = new ExpressionBuilder.NestedExpressionParameter[] { param };
        }

        public void ParseFile(string path)
        {
            Parse(Lexer.SplitToTokensFromFile(path));
        }
        public void Parse(string[] tokens)
        {
            int pointer = 0;
            while (tokens[pointer] != Keywords.EOF)
                if (tokens[pointer] == Keywords.EOL) pointer++;
                else if (tokens[pointer] == Keywords.Sprite) ParseSprite(tokens, ref pointer);
                else if (tokens[pointer] == Keywords.Projectile) ParseProjectile(tokens, ref pointer);
                else if (tokens[pointer] == Keywords.Generator) ParseGenerator(tokens, ref pointer);
                else if (tokens[pointer] == Keywords.Level) ParseLevel(tokens, ref pointer);
                else throw new NotImplementedException();
        }
        private void ParseSprite(string[] tokens, ref int pointer)
        {

        }
        private void ParseProjectile(string[] tokens, ref int pointer)
        {
            int? repeatIndex = null;
            int commandCount = 0;
            List<ICommand<Projectile>> commands = new List<ICommand<Projectile>>();

            while (tokens[pointer] != Keywords.End)
            {
                if ((tokens[pointer] == Keywords.EOL)) pointer++;
                else if (tokens[pointer] == Keywords.RepeatStart) repeatIndex = commandCount;
                else if (Keywords.IsPropMethod(tokens[pointer]))
                }
                pointer++;
            }
        }
        private void ParseGenerator(string[] tokens, ref int pointer)
        {
            int? repeatIndex = null;
            int commandCount = 0;
            List<ICommand<Generator<Projectile>>> commands = new List<ICommand<Generator<Projectile>>>();


        }
        private void ParseLevel(string[] tokens, ref int pointer)
        {

        }


        private float ParseFloatFromMathExpression(string[] tokens, ref int pointer)
        {
            List<string> mathExpressionTokens = new List<string>();
            while (tokens[pointer] != Keywords.EOL && tokens[pointer] != Keywords.ParameterSeparator)
            {
                mathExpressionTokens.Add(tokens[pointer++]);
            }
            return ((Func<float>)MathExpressionBuilder.CompileTokens(mathExpressionTokens.ToArray()))();
        }
        private Func<Projectile, float> ParseProjFuncMathExpression(string[] tokens, ref int pointer)
        {
            throw new NotImplementedException();
        }
        private string ParseString(string[] tokens, ref int pointer)
    {

    }

    }
}
