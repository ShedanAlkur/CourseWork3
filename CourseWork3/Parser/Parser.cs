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

        private static string[] paramsName = { "gen_time" }; 

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
                else if (tokens[pointer] == Keywords.Runtime)
                    commands.Add(new RuntimeCommand<Projectile>(float.Parse(tokens[++pointer])));
                else if (tokens[pointer] == Keywords.Pause)
                {
                    commands.Add(new PauseCommand<Projectile>(ParseFloatFromMathExpression(tokens, ref pointer)));
                }
                else if (tokens[pointer] == Keywords.Runtime)
                {
                    commands.Add(new RuntimeCommand<Projectile>(ParseFloatFromMathExpression(tokens, ref pointer)));
                }
                else if (Keywords.IsPropMethod(tokens[pointer]))
                {
                    string commandName = tokens[pointer] + tokens[++pointer];
                    Action<Projectile, object> action;
                    if (Keywords.isControlledObjectProperty(tokens[pointer]))
                        action = ControlledObject<Projectile>.ParserMethods[commandName];
                    else if (Keywords.isProjectileProperty(tokens[pointer]))
                        action = Projectile.ParserMethods[commandName];
                    else throw new NotImplementedException();

                    if (IsPredeterminedNumber(tokens, ref pointer))
                    {
                        commands.Add(new PropertyChangerCommand<Projectile>(action,
                            ParseFloatFromMathExpression(tokens, ref pointer)));
                    }
                    else
                    {
                        commands.Add(new BasedOnObjectPropertyChangerCommand<Projectile>(action,
                            ParseMathExpressionForProjectile(tokens, ref pointer)));
                    }
                }
                pointer++;
            }
        }
        private void ParseGenerator(string[] tokens, ref int pointer)
        {
            int? repeatIndex = null;
            int commandCount = 0;
            List<ICommand<Generator<Projectile>>> commands = new List<ICommand<Generator<Projectile>>>();

            while (tokens[pointer] != Keywords.End)
            {
                if (tokens[pointer] == Keywords.RepeatStart) repeatIndex = commandCount;
                else if (tokens[pointer] == Keywords.Runtime)
                    commands.Add(new RuntimeCommand<Generator<Projectile>>(float.Parse(tokens[++pointer])));
                else if (tokens[pointer] == Keywords.Pause)
                {
                    // Обработка команды delay
                }
                else if (tokens[pointer] == Keywords.Runtime)
                {
                    // Обработка команды Runtime
                }
                else if (Keywords.IsPropMethod(tokens[pointer]))
                {
                    string commandName = tokens[pointer] + tokens[++pointer];
                    Action<Generator<Projectile>, object> action;
                    if (Keywords.isControlledObjectProperty(tokens[pointer]))
                        action = ControlledObject<Generator<Projectile>>.ParserMethods[commandName];
                    else if (Keywords.isProjectileProperty(tokens[pointer]))
                        action = Generator<Projectile>.ParserMethods[commandName];
                    else throw new NotImplementedException();
                }
                pointer++;
            }
        }
        private void ParseLevel(string[] tokens, ref int pointer)
        {

        }

        private bool IsPredeterminedNumber(string[] tokens, ref int pointer)
        {
            int localPointer = pointer;
            List<string> mathExpressionTokens = new List<string>();
            while (tokens[localPointer] != Keywords.EOL && tokens[localPointer] != Keywords.ParameterSeparator)
            {
                mathExpressionTokens.Add(tokens[localPointer++]);
            }
            MathExpressionBuilder.CompileTokens(mathExpressionTokens.ToArray());
            if (MathExpressionBuilder.Parameters.Length == 0) return true;
            else return false;
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
        private Func<Projectile, float> ParseMathExpressionForProjectile(string[] tokens, ref int pointer)
        {
            throw new NotImplementedException();
        }

    }
}
