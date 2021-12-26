using CourseWork3.Game;
using CourseWork3.GameObjects;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Patterns;
using ExpressionBuilder;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;

namespace CourseWork3.Parser
{
    class BetterParser
    {

        private static readonly Dictionary<string, Type> TypeOfCommandParam = new Dictionary<string, Type>()
        {
            [Keywords.VelocityToPoint] = typeof(Vector2),
            [Keywords.PointRotation] = typeof(Vector2),
            [Keywords.PointCounterRotation] = typeof(Vector2),

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
            [Keywords.RotationAcceleration] = typeof(float),
            [Keywords.Hitbox] = typeof(float),

            [Keywords.Color] = typeof(Color),

            [Keywords.Sprite] = typeof(Sprite),
            [Keywords.Projectile] = typeof(Pattern<Projectile>),
            [Keywords.Generator] = typeof(Pattern<Generator>),

            [Keywords.Life] = typeof(int),
            [Keywords.MoveTo] = typeof((Vector2, float?)),
        };

        private MathFExpressionBuilder MathExpressionBuilder = new MathFExpressionBuilder();
        private static ImplicitParameter ProjectileImplicitParameter;
        private static ImplicitParameter GeneratorImplicitParameter;
        private static ImplicitParameter EnemyImplicitParameter;

        private string[] tokens;
        private int pointer;
        private int lineCount;

        private string pathOfPatternFile;
        private string pathOfPatternFolder;

        private Dictionary<string, float> Variables = new Dictionary<string, float>();
        List<ILevelCommand> levelCommands;

        static BetterParser()
        {
            ParameterExpression externalParam;
            ImplicitParameter implicitParameter;

            // projetile implicit param
            externalParam = Expression.Parameter(typeof(Projectile));
            implicitParameter = new ImplicitParameter(externalParam, new InternalImplicitParameter[]
            {
                new InternalImplicitParameter(Expression.PropertyOrField(externalParam, nameof(Projectile.GenTime)), Keywords.ProjParamName),
            });
            ProjectileImplicitParameter = implicitParameter;

            // generator implicit param
            externalParam = Expression.Parameter(typeof(Generator));
            implicitParameter = new ImplicitParameter(externalParam, new InternalImplicitParameter[]
            {

            });
            GeneratorImplicitParameter = implicitParameter;

            // enemy implicit param
            externalParam = Expression.Parameter(typeof(Enemy));
            implicitParameter = new ImplicitParameter(externalParam, new InternalImplicitParameter[]
            {

            });
            EnemyImplicitParameter = implicitParameter;
        }

        public BetterParser()
        {

        }


        public void ParseFile(string pathOfPatternFile)
        {
            this.pathOfPatternFile = pathOfPatternFile;
            this.pathOfPatternFolder = System.IO.Path.GetDirectoryName(pathOfPatternFile);
            tokens = Lexer.SplitToTokensFromFile(pathOfPatternFile);
            pointer = 0;
            ParseTokens();
        }

        private void ParseTokens()
        {
            while (tokens[pointer] != Keywords.EOF)
            {
                if (tokens[pointer] == Keywords.EOL) { pointer++; lineCount++; }
                else if (tokens[pointer] == Keywords.Sprite) { ParseSprite(); }
                else if (tokens[pointer] == Keywords.Projectile) { ParseControlledObject(Projectile.ActionsForParser, 
                    ProjectileImplicitParameter, GameMain.ProjeciltePatternCollection); }
                else if (tokens[pointer] == Keywords.Generator) { ParseControlledObject(Generator.ActionsForParser,
                    GeneratorImplicitParameter, GameMain.GeneratorPatternCollection); }
                else if (tokens[pointer] == Keywords.Enemy) { ParseControlledObject(Enemy.ActionsForParser,
                    EnemyImplicitParameter, GameMain.EnemyPatternCollection); }
                else if (tokens[pointer] == Keywords.Level) { ParseLevelMain(); }
                else throw new InvalidCastException($"Встречен неожиданный {tokens[pointer]} для начала шаблона.");
            }
        }

        private void ParseSprite()
        {
            pointer++;
            string name = ParseString();
            string path = null;
            Vector2 size = Vector2.One;
            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if (tokens[pointer] == Keywords.EOL) { pointer++; lineCount++; }
                else if (tokens[pointer] == Keywords.Path) { pointer++; path = ParseString(); }
                else if (tokens[pointer] == Keywords.Size) { pointer++; size = ParseVector2(); }
                else throw new NotImplementedException($"Встречен неизвестный токен {tokens[pointer]} при создании шаблона спрайта.");
            }
            pointer++;
            if (path == null)
                throw new NullReferenceException($"В шаблоне спрайта не задан путь {Keywords.Path}.");
            GameMain.SpriteCollection.Add(name, new Sprite(pathOfPatternFolder + @"\"+ path, size));
        }

        private void ParseControlledObject<T>(Dictionary<string, Action<T, object>> dictOfActions, ImplicitParameter implicitParameter,
            Dictionary<string, Pattern<T>> outputCollection) where T : ControlledObject<T>
        {
            pointer++;
            int? repeatIndex = null;
            var commands = new List<ICommand<T>>();
            Action<T, object> action;
            string name = ParseString();
            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if (tokens[pointer] == Keywords.EOL) { pointer++; lineCount++; }
                else if (tokens[pointer] == Keywords.RepeatStart) { pointer++; repeatIndex = commands.Count; }
                else
                {
                    string commandName;
                    action = null;
                    commandName = null;
                    if (Keywords.IsPropMethod(tokens[pointer]))
                        commandName = tokens[pointer++] + tokens[pointer++];
                    else commandName = tokens[pointer++];
                    action = dictOfActions[commandName];

                    ParseParameter<T>(tokens[pointer - 1], new ImplicitParameter[]{ implicitParameter, ImplicitParameter.Create(Variables) }, 
                        out object param, out bool flagOfFuncParam);

                    if (flagOfFuncParam) 
                        commands.Add(new BasedOnControlledObjectCommand<T, float>(action, (Func<T, float>)param));
                    else commands.Add(new ControlledObjectCommand<T>(action, param));
                }
            }
            pointer++;
            outputCollection.Add(name, new Pattern<T>(commands.ToArray(), repeatIndex));
        }

        private void ParseParameter<T>(string paramOrCommandName, ImplicitParameter[] implicitParameters, out object param, out bool flagOfFuncParam) where T : ControlledObject<T>
        {
            param = null;
            flagOfFuncParam = false;

            if (TypeOfCommandParam.TryGetValue(paramOrCommandName, out Type paramType))
                if (paramType == typeof(float) || paramType == typeof(int))
                {
                    param = ParseControlledObjectMathExpression<T>(implicitParameters);
                    flagOfFuncParam = true;
                }
                else if (paramType == typeof(Color)) param = Color.FromName(ParseString());
                else if (paramType == typeof(Vector2)) param = ParseVector2();
                else if (paramType == typeof(Sprite)) param = GameMain.SpriteCollection[ParseString()];
                else if (paramType == typeof(Pattern<Projectile>)) param = GameMain.ProjeciltePatternCollection[ParseString()];
                else if (paramType == typeof(Pattern<Generator>)) param = GameMain.GeneratorPatternCollection[ParseString()];
                else if (paramType == typeof(Pattern<Enemy>)) param = GameMain.EnemyPatternCollection[ParseString()];
                else if (paramType == typeof((Vector2, float?)))
                {
                    Vector2 value1 = ParseVector2();
                    float? value2 = null;
                    if (tokens[pointer] == Keywords.ParameterSeparator)
                    {
                        pointer++;
                        value2 = ParseGeneralMathExpression()();
                    }
                    param = (value1, value2);
                }
        }

        private void ParseLevelMain()
        {
            pointer++;
            levelCommands = new List<ILevelCommand>();
            ParseLevel();
            GameMain.World.Pattern = new LevelPattern(levelCommands.ToArray());
        }

        private void ParseLevel()
        {
            while (tokens[pointer] != Keywords.EndOfPattern)
            {
                if (tokens[pointer] == Keywords.EOL) { pointer++; lineCount++; }
                else if (tokens[pointer] == Keywords.For) ParseForloop(ParseLevel);
                else if (tokens[pointer] == Keywords.Pause)
                {
                    pointer++;
                    levelCommands.Add(new PauseCommand(ParseGeneralMathExpression(new ImplicitParameter[] { ImplicitParameter.Create(Variables) })()));
                }
                else if (tokens[pointer] == Keywords.Spawn)
                {
                    pointer++;
                    var enemyPattern = GameMain.EnemyPatternCollection[ParseString()];
                    if (tokens[pointer] == Keywords.ParameterSeparator) pointer++;
                    var position = ParseVector2(new ImplicitParameter[] { ImplicitParameter.Create(Variables) });
                    levelCommands.Add(new SpawnCommand(enemyPattern, position));
                }
                else throw new NotImplementedException();
            }
            pointer++;
        }

        private void ParseForloop(Action parseMethod)
        {
            pointer++;
            string nameOfIterator = tokens[pointer];
            bool IsFirstAppearanceOfIterator = Variables.TryAdd(nameOfIterator, 0);

            pointer++;
            float from = ParseGeneralMathExpression()();
            if (tokens[pointer] != Keywords.ParameterSeparator) throw new NotImplementedException();
            pointer++;
            float to = ParseGeneralMathExpression()();
            float incrementor = 1;
            if (tokens[pointer] == Keywords.ParameterSeparator)
            {
                pointer++;
                incrementor = ParseGeneralMathExpression()();
                if (incrementor == 0) throw new NotImplementedException();
            }
            if ((from < to && incrementor < 0) || (from > to && incrementor > 0)) incrementor = -incrementor;

            int firstCommandPointer = pointer;
            if (to > from)
                for (Variables[nameOfIterator] = from; Variables[nameOfIterator] <= to; Variables[nameOfIterator] += incrementor)
                {
                    pointer = firstCommandPointer;
                    parseMethod();
                }
            else
                for (Variables[nameOfIterator] = from; Variables[nameOfIterator] >= to; Variables[nameOfIterator] += incrementor)
                {
                    pointer = firstCommandPointer;
                    parseMethod();
                }

            if (IsFirstAppearanceOfIterator) Variables.Remove(nameOfIterator);
            //while (tokens[pointer] == Keywords.EOL || tokens[pointer] == Keywords.EndOfPattern) pointer++;
        }

        private Func<float> ParseGeneralMathExpression(ImplicitParameter[] implicitParameters = null)
        {
            List<string> mathTokens = new List<string>();
            while (tokens[pointer] != Keywords.EOL && tokens[pointer] != Keywords.ParameterSeparator)
                mathTokens.Add(tokens[pointer++]);
            return (Func<float>)MathExpressionBuilder.CompileTokens(mathTokens.ToArray(), implicitParameters);
        }

        private Func<T, float> ParseControlledObjectMathExpression<T>(ImplicitParameter[] implicitParameters) where T : ControlledObject<T>
        {
            List<string> mathTokens = new List<string>();
            while (tokens[pointer] != Keywords.EOL && tokens[pointer] != Keywords.ParameterSeparator)
                mathTokens.Add(tokens[pointer++]);
            return (Func<T, float>)MathExpressionBuilder.CompileTokens(mathTokens.ToArray(), implicitParameters);
        }

        private Vector2 ParseVector2(ImplicitParameter[] implicitParameters = null)
        {
            var x = ParseGeneralMathExpression(implicitParameters)();
            if (tokens[pointer] == Keywords.ParameterSeparator) pointer++;
            if (tokens[pointer] == Keywords.EOL)
                throw new InvalidCastException("Не удалось преобразовать токены в Vector2, после первой координаты встречен разделитель аргументов");
            var y = ParseGeneralMathExpression(implicitParameters)();
            return new Vector2(x, y);
        }

        private string ParseString()
        {
            if (tokens[pointer].StartsWith("\"") && tokens[pointer].EndsWith("\""))
                return tokens[pointer].Substring(1, tokens[pointer++].Length - 2);
            else throw new InvalidCastException($"Встреченный токен {tokens[pointer]} не удалось преобразовать в string");
        }
    }
}
