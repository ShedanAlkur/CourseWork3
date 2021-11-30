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
        private static string[] paramsName = { "gen_time" }; 

        public void ParseFile(string path)
        {
            var tokens = Lexer.SplitToTokensFromFile(path);
        }

        public void Parse(string[] tokens)
        {
            int pointer = 0;
            while (pointer < tokens.Length)
                if (tokens[pointer] == Keywords.ProjectileBlockBegin) ParseProjectile(tokens, ref pointer);
                else if (tokens[pointer] == Keywords.GeneratorBlockBegin) ParseGenerator(tokens, ref pointer);
                else if (tokens[pointer] == Keywords.EOF) return;
                else throw new NotImplementedException();
        }

        private void ParseProjectile(string[] tokens, ref int pointer)
        {
            int? repeatIndex = null;
            int commandCount = 0;
            List<ICommand<Projectile>> commands = new List<ICommand<Projectile>>();

            while (tokens[pointer] != Keywords.EndOfBlock)
                if (tokens[pointer] == Keywords.Repeat_start) repeatIndex = commandCount;
                else if (tokens[pointer] == Keywords.Runtime)
                {    
                    commands.Add(new RuntimeCommand<Projectile>(float.Parse(tokens[++pointer])));
                    pointer++;
                }    
                else if (tokens[pointer] == Keywords.Delay)
                {    
                    // Обработка команды delay
                    pointer++;
                }    
                else if (Keywords.IsPropMethod(tokens[pointer]))
                {
                    string commandName = tokens[pointer++];
                    commandName += tokens[pointer];
                    Action<Projectile, object> action;
                    if (Keywords.isControlledObjectProperty(tokens[pointer]))
                        action = ControlledObject<Projectile>.ParserMethods[commandName];
                    else if (Keywords.isProjectileProperty(tokens[pointer]))
                        action = Projectile.ParserMethods[commandName];

                }
        }

        private Func<float, float> ParseMathExpression(string[] tokens, ref int pointer)
        {
            List<string> mathExpressionTokens = new List<string>();
            while (tokens[pointer] != Keywords.EOL && tokens[pointer] != ",")
            {
                mathExpressionTokens.Add(tokens[pointer++]);
            }
            ExpressionBuilder.MathFExpressionBuilder exprBuilder = new ExpressionBuilder.MathFExpressionBuilder();
            return (Func<float, float>)exprBuilder.CompileTokens(mathExpressionTokens.ToArray(), Keywords.ProjParamName);
        }

        private void ParseGenerator(string[] tokens, ref int pointer)
        {

        }
    }
}
