using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class Item : GameObject
    {
        const float START_VELOCITY = 400;
        const float END_VELOCITY = 200;
        const float ACCELERATION = 300;

        Vector2 size = new Vector2(30);
        public Item(Vector2 position) : base(position)
        {
            Velocity = new Vector2(0, START_VELOCITY);
            AccelerationAngle = MathHelper.Pi;
            AccelerationScalar = ACCELERATION;
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            if (Velocity.Y < -END_VELOCITY)
            {
                Velocity = new Vector2(0, -END_VELOCITY);
                AccelerationScalar = 0;
            }
        }

        public override void Draw()
        {
            GameMain.Graphics.Draw(GameMain.TextureCollection["item"], Position, size, 0, 0);
        }
    }
}
