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
            //if (GameMain.Input.KeyDown(Key.Left))
            //    Velocity.X = -velocityScalar;
            //else if (GameMain.Input.KeyDown(Key.Right))
            //    Velocity.X = velocityScalar;
            //else Velocity.X = 0;

            //if (GameMain.Input.KeyDown(Key.Down))
            //    Velocity.Y = -velocityScalar;
            //else if (GameMain.Input.KeyDown(Key.Up))
            //    Velocity.Y = velocityScalar;
            //else Velocity.Y = 0;

            base.Update(elapsedTime);
        }

        public override void Draw()
        {
            GameMain.Graphics.Draw(texture, Position, size, 0, 1);
        }
    }

}
