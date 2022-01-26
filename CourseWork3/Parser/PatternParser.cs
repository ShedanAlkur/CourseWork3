using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Parser
{
    abstract class PatternParser
    {
        private Dictionary<string, float> variables;
        private Stack<ForLoopInfo> ongoingLoops;

        public abstract void Parse(string[] tokens, int pointer);

        public void ParseForLoop(string[] tokens, int pointer) { throw new NotImplementedException(); }
        public string ParseString(string[] tokens, int pointer) { throw new NotImplementedException(); }
        public Delegate ParseMathExpression(string[] tokens, int pointer) { throw new NotImplementedException(); }
        public OpenTK.Vector2 ParseVector2(string[] tokens, int pointer) { throw new NotImplementedException(); }
    }
}
