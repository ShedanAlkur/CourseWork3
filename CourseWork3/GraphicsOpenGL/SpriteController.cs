using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.GraphicsOpenGL
{
    abstract class SpriteController : Game.IUpdateable
    {
        public abstract Sprite GetSprite();
        public abstract void Update(float elapsedTime);
    }
}
