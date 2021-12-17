using CourseWork3.Game;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

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
            [Keywords.SpawnCount] = typeof(int),
            [Keywords.RotationSpeed] = typeof(float),
            [Keywords.Hitbox] = typeof(float),

            [Keywords.Color] = typeof(System.Drawing.Color),

            [Keywords.Sprite] = typeof(Sprite),
            [Keywords.Projectile] = typeof(Pattern<Projectile>),
            [Keywords.Generator] = typeof(Pattern<Generator>),

            [Keywords.Life] = typeof(int),

        };

        private ExpressionBuilder.MathFExpressionBuilder MathExpressionBuilder = new ExpressionBuilder.MathFExpressionBuilder();

        private static ExpressionBuilder.NestedExpressionParameter[] nestedParametersForProjectile;

        private static string[] paramsName = { Keywords.ProjParamName };

        static Parser()
        {
            // Настройка информации о параметре, извлекаемом из класса-снаряда для мат. вычислений
            var externalParam = System.Linq.Expressions.Expression.Parameter(typeof(Projectile));
            var internalParam = System.Linq.Expressions.Expression.PropertyOrField(externalParam, nameof(Projectile.GenTime));
            var internalParamName = Keywords.ProjParamName;
            var param = new ExpressionBuilder.NestedExpressionParameter(externalParam, internalParam, internalParamName);
            nestedParametersForProjectile = new ExpressionBuilder.NestedExpressionParameter[] { param };
            //
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
                    else if (tokens[pointer] == Keywords.Enemy) ParseEnemy(tokens, ref pointer);
                    else if (tokens[pointer] == Keywords.Level) ParseLevel(tokens, ref pointer);
                    else throw new NotImplementedException();
                pointer++;
            }
        }

        #region Parse patterns
        private void ParseSprite(string[] tokens, ref int pointer)
        {
            string path = null;
            Vector2 size = Vector2.One;
            pointer++;
            string name = ParseString(tokens, ref pointer);
            pointer++;
            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if ((tokens[pointer] != Keywords.EOL))
                    if (tokens[pointer] == Keywords.Path)
                    { pointer++; path = GameMain.PathOfPatternFolder + @"\" + ParseString(tokens, ref pointer); }
                    else if (tokens[pointer] == Keywords.SizeRelativeToHitbox) 
                    { pointer++; size = ParseVector2(tokens, ref pointer); }
                    else throw new NotImplementedException();
                pointer++;
            }
            GameMain.SpriteCollection.Add(name, new Sprite(path, size));
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
                            else if (paramType == typeof(Sprite)) param = GameMain.SpriteCollection[ParseString(tokens, ref pointer)];
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
            List<ICommand<Generator>> commands = new List<ICommand<Generator>>();
            Action<Generator, object> action;
            object param;
            pointer++;
            string patternName = ParseString(tokens, ref pointer);
            pointer++;

            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if ((tokens[pointer] != Keywords.EOL))
                    if (tokens[pointer] == Keywords.RepeatStart) repeatIndex = commands.Count;
                    else
                    {
                        param = null;
                        action = (Keywords.IsPropMethod(tokens[pointer])) ?
                            Generator.ActionsForParser[tokens[pointer++] + tokens[pointer]] :
                            action = Generator.ActionsForParser[tokens[pointer]];

                        if (TypeOfCommandParam.TryGetValue(tokens[pointer++], out Type paramType))
                            if (paramType == typeof(float)) param = ParseFloatFromMathExpression(tokens, ref pointer)();
                            else if (paramType == typeof(int)) param = (int)ParseFloatFromMathExpression(tokens, ref pointer)();
                            else if (paramType == typeof(Pattern<Projectile>)) param = GameMain.ProjeciltePatternCollection[ParseString(tokens, ref pointer)];

                        commands.Add(new PropertyChangerCommand<Generator>(action, param));
                    }
                pointer++;
            }
            pointer++;
            GameMain.GeneratorPatternCollection.TryAdd(patternName,
                new Pattern<Generator>(commands.ToArray(), repeatIndex));
        }

        private void ParseEnemy(string[] tokens, ref int pointer)
        {
            int? repeatIndex = null;
            Action<Enemy, object> action;
            object param;
            var commands = new List<ICommand<Enemy>>();
            pointer++;
            string patternName = ParseString(tokens, ref pointer);
            pointer++;
            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if ((tokens[pointer] != Keywords.EOL))
                    if (tokens[pointer] == Keywords.RepeatStart) repeatIndex = commands.Count;
                    else
                    {
                        param = null;
                        action = (Keywords.IsPropMethod(tokens[pointer])) ?
                            Enemy.ActionsForParser[tokens[pointer++] + tokens[pointer]] :
                            action = Enemy.ActionsForParser[tokens[pointer]];

                        if (TypeOfCommandParam.TryGetValue(tokens[pointer++], out Type paramType))
                            if (paramType == typeof(float)) param = ParseFloatFromMathExpression(tokens, ref pointer)();
                            else if (paramType == typeof(int)) param = (int)ParseFloatFromMathExpression(tokens, ref pointer)();
                            else if (paramType == typeof(Pattern<Generator>)) param = GameMain.GeneratorPatternCollection[ParseString(tokens, ref pointer)];
                            else if (paramType == typeof(Sprite)) param = GameMain.SpriteCollection[ParseString(tokens, ref pointer)];

                        commands.Add(new PropertyChangerCommand<Enemy>(action, param));
                        // обработка команд врага
                    }
                pointer++;
            }
            pointer++;
            GameMain.EnemyPatternCollection.TryAdd(patternName,
                new Pattern<Enemy>(commands.ToArray(), repeatIndex));
        }

        private void ParseLevel(string[] tokens, ref int pointer)
        {
            var iterators = new Dictionary<string, float>();
            pointer++;
            var levelCommands = new List<ILevelCommand>();
            ParseLevel(tokens, ref pointer, ref iterators, ref levelCommands);
            GameMain.World.Pattern = new LevelPattern(levelCommands.ToArray());
        }

        private void ParseLevel(string[] tokens, ref int pointer, ref Dictionary<string, float> iterators, ref List<ILevelCommand> levelCommands)
        {
            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if (tokens[pointer] != Keywords.EOL)
                    if (tokens[pointer] == Keywords.For) ParseForLoop(tokens, ref pointer, ref iterators, ref levelCommands);
                    else if (tokens[pointer] == Keywords.Spawn)
                    {
                        pointer++;
                        var enemyPattern = GameMain.EnemyPatternCollection[ParseString(tokens, ref pointer)];
                        pointer++;
                        ParseParameterSeparatorIfExist(tokens, ref pointer);
                        var x = ParseForLoopMathExpression(tokens, ref pointer, iterators);
                        pointer++;
                        var y = ParseForLoopMathExpression(tokens, ref pointer, iterators);
                        levelCommands.Add(new SpawnCommand(enemyPattern, new OpenTK.Vector2(x, y)));
                    }
                    else if (tokens[pointer] == Keywords.Pause)
                    {
                        pointer++;
                        levelCommands.Add(new PauseCommand(ParseFloatFromMathExpression(tokens, ref pointer)()));
                    }
                pointer++;
            }
        }

        private void ParseForLoop(string[] tokens, ref int pointer, ref Dictionary<string, float> iterators, ref List<ILevelCommand> levelCommands)
        {
            string nameofIterator = tokens[++pointer];
            bool IsFirstAppearanceOfIterator = iterators.TryAdd(nameofIterator, 0);

            pointer++;
            float from = ParseFloatFromMathExpression(tokens, ref pointer)();
            pointer++;
            float to = ParseFloatFromMathExpression(tokens, ref pointer)();
            float incrementor = (tokens[pointer] == Keywords.ParameterSeparator) ?
                ParseFloatFromMathExpression(tokens, ref pointer)() : 1;

            int i;
            int firstCommandPointer = ++pointer;
            if (to >= from)
                for (iterators[nameofIterator] = from; iterators[nameofIterator] <= to; iterators[nameofIterator] += incrementor)
                {
                    pointer = firstCommandPointer;
                    ParseLevel(tokens, ref pointer, ref iterators, ref levelCommands);
                }
            else for (iterators[nameofIterator] = from; iterators[nameofIterator] >= to; iterators[nameofIterator] -= incrementor)
                {
                    pointer = firstCommandPointer;
                    ParseLevel(tokens, ref pointer, ref iterators, ref levelCommands);
                }

            if (IsFirstAppearanceOfIterator) iterators.Remove(nameofIterator);
        }

        #endregion

        #region Parse parameters
        private bool ParseParameterSeparatorIfExist(string[] tokens, ref int pointer)
        {
            if (tokens[pointer] == Keywords.ParameterSeparator)
            {
                pointer++;
                return true;
            }
            return false;
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

        private float ParseForLoopMathExpression(string[] tokens, ref int pointer, Dictionary<string, float> iterators)
        {
            List<string> mathExpressionTokens = new List<string>();
            while (tokens[pointer] != Keywords.EOL && tokens[pointer] != Keywords.ParameterSeparator)
            {
                mathExpressionTokens.Add(tokens[pointer++]);
            }
            return (float)(MathExpressionBuilder.CompileTokens(mathExpressionTokens.ToArray(),
                iterators.Keys.ToArray()).DynamicInvoke(iterators.Values.Select(x => (object?)x).ToArray()));
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
        #endregion
    }
}
