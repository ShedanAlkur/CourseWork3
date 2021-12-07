using System;
using System.Collections.Generic;
using System.Text;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Patterns;

namespace CourseWork3.Game
{
    partial class GameMain
    {
        public class GameCollection<T>
        {
            private Dictionary<string, T> patterns = new Dictionary<string, T>();
            public bool TryAdd(string name, T element) => patterns.TryAdd(name, element);
            public bool TryGet(string name, out T element) => patterns.TryGetValue(name, out element);
            public void Add(string name, T element) => patterns.Add(name, element);
            public void Remove(string name) => patterns.Remove(name);
            public T this[string name] => patterns[name];
        }
    }
}
