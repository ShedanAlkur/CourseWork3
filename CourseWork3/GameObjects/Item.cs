using CourseWork3.GraphicsOpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CourseWork3.Game
{
    class Item : GameObject
    {
        const byte Depth = 15; 
        public const float DefaultHitboxSize = 100f;

        const float StartVelocity = 550f;
        const float EndVelocity = 300f;
        const float Acceleration = 400f;


        Sprite arrowSprite;
        Sprite itemSprite;

        protected virtual Color Color { get => Color.Gray; }

        public Item(Vector2 position) : base(position)
        {
            HitBoxSize = DefaultHitboxSize;
            arrowSprite = GameMain.SpriteCollection["_arrow"];
            itemSprite = GameMain.SpriteCollection["_item"];
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

            if (Position.Y + HitBoxSize < World.BottomRightPoint.Y) 
                Terminated = true;

        }

        public override void Draw()
        {
            if (Position.Y > World.TopLeftPoint.Y)
                GameMain.Graphics.Draw(arrowSprite.Texture, new Vector2(Position.X, World.TopLeftPoint.Y - arrowSprite.SizeRelativeToHitbox.Y * HitBoxSize), HitBoxSize * itemSprite.SizeRelativeToHitbox, 0, Color, Depth);
            
            if (GameMain.DrawHitboxes) GameMain.Graphics.Draw(GameMain.SpriteCollection["_collision"].Texture, Position, HitBoxSize * Vector2.One, 0, Depth);

            GameMain.Graphics.Draw(itemSprite.Texture, Position, HitBoxSize * itemSprite.SizeRelativeToHitbox, 0, Color, Depth);
        }

        public override void OnCollision(GameObject gameObject)
        {
            switch (gameObject)
            {
                case Player player: if (SqrCollisionCheck(player)) Use(); break;
                default: break;
            }
        }

        protected virtual void Use()
        {
            Terminated = true;
        }
    }
}
