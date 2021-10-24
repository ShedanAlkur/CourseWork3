using System;
using System.Collections.Generic;
using System.Text;
using CourseWork3.GraphicsOpenGL;

namespace CourseWork3.Game
{
    partial class GameMain
    {
        public class GameTextureCollection
        {
            Dictionary<string, Texture2D> Textures;

            public GameTextureCollection()
            {
                Textures = new Dictionary<string, Texture2D>();
            }

            public void Add(string name, Texture2D texture)
            {
                Textures.Add(name, texture);
            }

            public void Remove(string name)
            {
                Textures.Remove(name);
            }

            public Texture2D this[string name]
            {
                get
                {
                    return Textures[name];
                }
            }
        }
    }
}
