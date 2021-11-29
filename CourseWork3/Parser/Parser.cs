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
            string[] tokens = Lexer.SplitToTokensFromFile(path);
            int pointer = 0;
        }

        private void ParseProjectile(string[] tokens, ref int pointer)
        {

        }
        private void ParseGenerator(string[] tokens, ref int pointer)
        {

        }
    }
}
