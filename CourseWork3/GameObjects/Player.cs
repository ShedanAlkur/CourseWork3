using CourseWork3.GraphicsOpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;

namespace CourseWork3.Game
{
    class Player : GameObject
    {
        PlayerLevel Level;

        Texture2D texture;
        Vector2 size;
        const float velocityScalar = 100f;

        public Player()
        {
            texture = GameMain.TextureCollection["projectile"];
            size = new Vector2(50, 100);
        }

        public override void Update(float elapsedTime)
        {
            Vector2 velocity = Vector2.Zero;
            if (GameMain.Input.KeyDown(Key.Left))
                velocity.X = -velocityScalar;
            else if (GameMain.Input.KeyDown(Key.Right))
                velocity.X = velocityScalar;

            if (GameMain.Input.KeyDown(Key.Down))
                velocity.Y = -velocityScalar;
            else if (GameMain.Input.KeyDown(Key.Up))
                velocity.Y = velocityScalar;

            this.Velocity = velocity;

            base.Update(elapsedTime);
        }

        public override void Draw()
        {
            GameMain.Graphics.Draw(texture, Position, size, 0, 1);
        }
    }

}
