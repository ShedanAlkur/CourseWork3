using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CourseWork3.Parser
{
    class Parser
    {
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

        }
        private void ParseGenerator(string[] tokens, ref int pointer)
        {

        }
    }
}
