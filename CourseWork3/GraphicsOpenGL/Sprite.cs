using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.GraphicsOpenGL
{
    class Sprite
    {
        public Texture2D Texture { get; private set; }
        public Vector2 SizeRelativeToHitbox { get; private set; }

        public Sprite(Texture2D texture, Vector2 sizeRelativeToHitbox)
        {
            this.Texture = texture;
            this.SizeRelativeToHitbox = sizeRelativeToHitbox;
        }

        public Sprite(Texture2D texture)
        {
            this.Texture = texture;
            this.SizeRelativeToHitbox = Vector2.One;
        }

        public Sprite(string texturePath, Vector2 sizeRelativeToHitbox)
        {
            if (string.IsNullOrEmpty(texturePath)) throw new ArgumentNullException(nameof(texturePath));
            this.Texture = new Texture2D(texturePath);
            this.SizeRelativeToHitbox = sizeRelativeToHitbox;
        }

        public Sprite(string texturePath)
        {
            if (string.IsNullOrEmpty(texturePath)) throw new ArgumentNullException(nameof(texturePath));
            this.Texture = new Texture2D(texturePath);
            this.SizeRelativeToHitbox = Vector2.One;
        }
    }
}
