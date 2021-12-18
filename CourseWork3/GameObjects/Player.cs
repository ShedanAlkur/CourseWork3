using CourseWork3.GameObjects;
using CourseWork3.GraphicsOpenGL;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class Player : GameObject
    {
        const byte Depth = 1;
        public const float DefaultHitboxSize = 12;

        PlayerLevel Level;

        Sprite sprite;
        const float velocityScalar = 100f;
        public float GrazeHitBoxSize;
        public float HalfGrazeHitBoxSize;


        public Player(Vector2 position) : base(position)
        {
            HitBoxSize = DefaultHitboxSize;
            sprite = GameMain.SpriteCollection["_player"];

            GrazeHitBoxSize = 2f * HitBoxSize;
            HalfGrazeHitBoxSize = GrazeHitBoxSize / 2f;
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

            if (GameMain.Input.KeyPress(Key.Z))
                GameMain.World.Add(new Item(Position + new Vector2(0, 150)));


            if (GameMain.Input.KeyPress(Key.X))
            {
                if (GameMain.Stats.BombCount > 0)
                {
                    GameMain.World.Add(new Bomb(Position));
                    GameMain.Stats.BombCount--;
                }
            }

            this.Velocity = velocity;

            base.Update(elapsedTime);

            if (Position.X + HalfHitBoxSize > World.BottomRightPoint.X) Position.X = World.BottomRightPoint.X - HalfHitBoxSize;
            else if (Position.X - HalfHitBoxSize < World.TopLeftPoint.X) Position.X = World.TopLeftPoint.X + HalfHitBoxSize;
            if (Position.Y + HalfHitBoxSize > World.TopLeftPoint.Y) Position.Y = World.TopLeftPoint.Y - HalfHitBoxSize;
            else if (Position.Y - HalfHitBoxSize < World.BottomRightPoint.Y) Position.Y = World.BottomRightPoint.Y + HalfHitBoxSize;
        }

        public override void Draw()
        {
            if (GameMain.DrawHitboxes) GameMain.Graphics.Draw(GameMain.SpriteCollection["_collision"].Texture, Position, HitBoxSize * Vector2.One, 0, Depth);
            GameMain.Graphics.Draw(sprite.Texture, Position, HitBoxSize * sprite.SizeRelativeToHitbox, 0, Depth);
        }

        public override void OnCollision(GameObject gameObject)
        {
            base.OnCollision(gameObject);
            switch (gameObject)
            {
                case Projectile proj: 
                    if (proj.IsEnemyProjectile && SqrCollisionCheck(gameObject) &&
                        RoundCollisionCheck(gameObject)) Die(); break;
                case Enemy _:
                    if (SqrCollisionCheck(gameObject) && RoundCollisionCheck(gameObject)) Die(); break;
                default: return;
            }
        }

        private void Die()
        {
            Position = World.DefaultPlayerPosition;
        }
    }

}
