using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.GraphicsOpenGL
{
    class Sprite
    {
        int columns;
        int rows;
        int singleTextureHeight;
        int singleTextureWidth;
        Texture2D texture;

        public Sprite(string texturePath, int columns = 1, int rows = 1)
        {
            return;
            if (string.IsNullOrEmpty(texturePath)) throw new ArgumentNullException(nameof(texturePath));
            this.texture = new Texture2D(texturePath);
            this.columns = (columns >= 1) ? columns : 1;
            this.rows = (rows >= 1) ? rows : 1;
            singleTextureHeight = texture.Height / rows;
            singleTextureHeight = texture.Width / columns;
        }
    }
}
