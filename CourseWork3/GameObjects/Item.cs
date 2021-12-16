using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class Item : GameObject
    {
        const float StartVelocity = 550f;
        const float EndVelocity = 300f;
        const float Acceleration = 400f;

        Vector2 size = new Vector2(30);
        public Item(Vector2 position) : base(position)
        {
            Velocity = new Vector2(0, StartVelocity);
            AccelerationAngle = MathHelper.Pi;
            AccelerationScalar = Acceleration;
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            if (Velocity.Y <= -EndVelocity)
            {
                Velocity = new Vector2(0, -EndVelocity);
                AccelerationScalar = 0;
            }
        }

        public override void Draw()
        {
            GameMain.Graphics.Draw(GameMain.TextureCollection["item"], Position, size, 0, 0);
        }
    }
}
