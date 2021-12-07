using CourseWork3.Game;
using CourseWork3.Patterns;
using System;
using System.Collections.Generic;

namespace CourseWork3.Parser
{
    class Parser
    {
        private static readonly Dictionary<string, Type> TypeOfCommandParam = new Dictionary<string, Type>
        {
            [Keywords.VelocityToPoint] = typeof(OpenTK.Vector2),
            [Keywords.PointRotation] = typeof(OpenTK.Vector2),

            [Keywords.Runtime] = typeof(float),
            [Keywords.Pause] = typeof(float),
            [Keywords.PositionX] = typeof(float),
            [Keywords.PositionY] = typeof(float),
            [Keywords.VelocityScalar] = typeof(float),
            [Keywords.VelocityAngle] = typeof(float),
            [Keywords.AccelerationScalar] = typeof(float),
            [Keywords.AccelerationAngle] = typeof(float),
            [Keywords.Angle] = typeof(float),
            [Keywords.Sector] = typeof(float),
            [Keywords.SpawnDelay] = typeof(float),
            [Keywords.SpawnCount] = typeof(float),

            [Keywords.Color] = typeof(System.Drawing.Color),

            [Keywords.Sprite] = null,
            [Keywords.Projectile] = typeof(Pattern<Projectile>),
            [Keywords.Generator] = typeof(Pattern<Generator<EnemyProjectile>>),
        };

        private ExpressionBuilder.MathFExpressionBuilder MathExpressionBuilder = new ExpressionBuilder.MathFExpressionBuilder();

        private static ExpressionBuilder.NestedExpressionParameter[] nestedParametersForProjectile;

        private static string[] paramsName = { Keywords.ProjParamName };

        static Parser()
        {
            // Настройка информации о параметре, извлекаемом из класса-снаряда для мат. вычислений
            var externalParam = System.Linq.Expressions.Expression.Parameter(typeof(Projectile));
            var internalParam = System.Linq.Expressions.Expression.PropertyOrField(externalParam, nameof(Projectile.CurrentRuntime));
            var internalParamName = Keywords.ProjParamName;
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
            {
                if (tokens[pointer] != Keywords.EOL)
                    if (tokens[pointer] == Keywords.Sprite) ParseSprite(tokens, ref pointer);
                    else if (tokens[pointer] == Keywords.Projectile) ParseProjectile(tokens, ref pointer);
                    else if (tokens[pointer] == Keywords.Generator) ParseGenerator(tokens, ref pointer);
                    else if (tokens[pointer] == Keywords.Level) ParseLevel(tokens, ref pointer);
                    else throw new NotImplementedException();
                pointer++;
            }
        }
        private void ParseSprite(string[] tokens, ref int pointer)
        {
            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                pointer++;
            }
        }
        private void ParseProjectile(string[] tokens, ref int pointer)
        {
            int? repeatIndex = null;
            List<ICommand<Projectile>> commands = new List<ICommand<Projectile>>();
            Action<Projectile, object> action;
            object param;
            pointer++;
            string patternName = ParseString(tokens, ref pointer);
            pointer++;

            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if ((tokens[pointer] != Keywords.EOL))
                    if (tokens[pointer] == Keywords.RepeatStart) repeatIndex = commands.Count;
                    //else if (tokens[pointer] == Keywords.Runtime)
                    //{
                    //    pointer++;
                    //    commands.Add(new ProjRuntimeCommand(ParseProjFuncMathExpression(tokens, ref pointer)));
                    //}
                    else
                    {
                        param = null;
                        action = (Keywords.IsPropMethod(tokens[pointer])) ?
                            Projectile.ActionsForParser[tokens[pointer++] + tokens[pointer]] :
                            action = Projectile.ActionsForParser[tokens[pointer]];

                        bool flagOfFloatParam = false;

                        if (TypeOfCommandParam.TryGetValue(tokens[pointer++], out Type paramType))
                            if (paramType == typeof(float)) { param = ParseProjFuncMathExpression(tokens, ref pointer); flagOfFloatParam = true; }
                            else if (paramType == typeof(System.Drawing.Color)) param = System.Drawing.Color.FromName(ParseString(tokens, ref pointer));
                            else if (paramType == typeof(OpenTK.Vector2))
                                param = ParseVector2(tokens, ref pointer);
                        if (flagOfFloatParam)
                            commands.Add(new BasedOnProjectileChangerCommand(action, (Func<Projectile, float>)param));
                        else commands.Add(new PropertyChangerCommand<Projectile>(action, param));
                    }
                pointer++;
            }
            pointer++;
            GameMain.ProjeciltePatternCollection.TryAdd(patternName,
                new Pattern<Projectile>(commands.ToArray(), repeatIndex));
        }


        private void ParseGenerator(string[] tokens, ref int pointer)
        {
            int? repeatIndex = null;
            List<ICommand<Generator<EnemyProjectile>>> commands = new List<ICommand<Generator<EnemyProjectile>>>();
            Action<Generator<EnemyProjectile>, object> action;
            object param;
            pointer++;
            string patternName = ParseString(tokens, ref pointer);
            pointer++;

            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if ((tokens[pointer] != Keywords.EOL))
                    if (tokens[pointer] == Keywords.RepeatStart) repeatIndex = commands.Count;
                    //else if (tokens[pointer] == Keywords.Runtime)
                    //{
                    //    pointer++;
                    //    commands.Add(new RuntimeCommand<Generator<EnemyProjectile>>(ParseFloatFromMathExpression(tokens, ref pointer)()));
                    //}
                    else
                    {
                        param = null;
                        action = (Keywords.IsPropMethod(tokens[pointer])) ?
                            Generator<EnemyProjectile>.ActionsForParser[tokens[pointer++] + tokens[pointer]] :
                            action = Generator<EnemyProjectile>.ActionsForParser[tokens[pointer]];

                        if (TypeOfCommandParam.TryGetValue(tokens[pointer++], out Type paramType))
                            if (paramType == typeof(float)) param = ParseFloatFromMathExpression(tokens, ref pointer);
                            else if (paramType == typeof(Pattern<Projectile>)) param = GameMain.ProjeciltePatternCollection[ParseString(tokens, ref pointer)];

                        commands.Add(new PropertyChangerCommand<Generator<EnemyProjectile>>(action, param));
                    }
                pointer++;
            }
            pointer++;
            GameMain.GeneratorPatternCollection.TryAdd(patternName,
                new Pattern<Generator<EnemyProjectile>>(commands.ToArray(), repeatIndex));
        }
        private void ParseLevel(string[] tokens, ref int pointer)
        {
            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                pointer++;
            }
        }


        private Func<float> ParseFloatFromMathExpression(string[] tokens, ref int pointer)
        {
            List<string> mathExpressionTokens = new List<string>();
            while (tokens[pointer] != Keywords.EOL && tokens[pointer] != Keywords.ParameterSeparator)
            {
                mathExpressionTokens.Add(tokens[pointer++]);
            }
            return (Func<float>)MathExpressionBuilder.CompileTokens(mathExpressionTokens.ToArray());
        }
        private Func<Projectile, float> ParseProjFuncMathExpression(string[] tokens, ref int pointer)
        {
            List<string> mathExpressionTokens = new List<string>();
            while (tokens[pointer] != Keywords.EOL && tokens[pointer] != Keywords.ParameterSeparator)
            {
                mathExpressionTokens.Add(tokens[pointer++]);
            }
            return (Func<Projectile, float>)MathExpressionBuilder.CompileTokens(mathExpressionTokens.ToArray(), nestedParametersForProjectile);
        }
        private string ParseString(string[] tokens, ref int pointer)
        {
            if (tokens[pointer].StartsWith("\"") && tokens[pointer].EndsWith("\""))
                return tokens[pointer].Substring(1, tokens[pointer].Length - 2);
            else throw new NotImplementedException();
        }

        private object ParseColor(string[] tokens, ref int pointer) =>
            System.Drawing.Color.FromName(tokens[++pointer]);

        private OpenTK.Vector2 ParseVector2(string[] tokens, ref int pointer)
        {
            var x = ParseFloatFromMathExpression(tokens, ref pointer)();
            if (tokens[pointer] == Keywords.EOL) throw new NotImplementedException();
            pointer++;
            var y = ParseFloatFromMathExpression(tokens, ref pointer)();
            return new OpenTK.Vector2(x, y);
        }
    }
}
