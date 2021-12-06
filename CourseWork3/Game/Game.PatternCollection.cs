using System;
using System.Collections.Generic;
using System.Text;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Patterns;

namespace CourseWork3.Game
{
    partial class GameMain
    {
        public class PatternCollection<T> where T : ControlledObject<T>
        {
            private Dictionary<string, Pattern<T>> patterns;
            public bool TryAdd(string name, Pattern<T> pattern) => patterns.TryAdd(name, pattern);
            public bool TryGet(string name, out Pattern<T> pattern) => patterns.TryGetValue(name, out pattern);
            public void Remove(string name) => patterns.Remove(name);
        }
    }
}
