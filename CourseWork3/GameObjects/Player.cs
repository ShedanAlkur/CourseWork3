using CourseWork3.GameObjects;
using CourseWork3.GraphicsOpenGL;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CourseWork3.Game
{
    class Player : GameObject
    {
        const byte Depth = 1;
        const byte SupportDepth = 20;
        public const float DefaultHitboxSize = 12;
        const float timeOfInvincibility = 3;

        Sprite playerSprite;
        Texture2D supportBallTexture;
        Vector2 supportBallSize;

        const float velocityScalar = 200f;
        const float halfVelocityScalar = 0.5f * velocityScalar;
        public float GrazeHitBoxSize;
        public float HalfGrazeHitBoxSize;

        float currentTimeOfInvincibility;
        public bool IsInvincible;

        Generator mainGenerator;
        Generator supportGenerator1;
        Generator supportGenerator2;
        Vector2 rightSupportStartPos;
        Vector2 rightSupportOffset;
        float timeOfSupportMovement;
        float currentTimeOfSupportMovement;

        public Player(Vector2 position) : base(position)
        {
            HitBoxSize = DefaultHitboxSize;
            playerSprite = GameMain.SpriteCollection["_player"];
            supportBallTexture = GameMain.TextureCollection["_supportBall"];
            supportBallSize = new Vector2(20);

            GrazeHitBoxSize = 2f * HitBoxSize;
            HalfGrazeHitBoxSize = GrazeHitBoxSize / 2f;

            rightSupportStartPos = new Vector2(45, -10);
            rightSupportOffset = new Vector2(-20, 30);
            timeOfSupportMovement = 1;

            mainGenerator = new Generator(GameMain.GeneratorPatternCollection["_playermain"], this);
            supportGenerator1 = new Generator(GameMain.GeneratorPatternCollection["_playersup"], this, false);
            supportGenerator2 = new Generator(GameMain.GeneratorPatternCollection["_playersup"], this, false);
            supportGenerator1.Position = Position + rightSupportStartPos * new Vector2(-1, 1);
            supportGenerator2.Position = Position + rightSupportStartPos;
            GameMain.World.Add(mainGenerator);
            GameMain.World.Add(supportGenerator1);
            GameMain.World.Add(supportGenerator2);
        }

        public override void Update(float elapsedTime)
        {
            if (IsInvincible)
            {
                currentTimeOfInvincibility += elapsedTime;
                if (currentTimeOfInvincibility > timeOfInvincibility)
                {
                    IsInvincible = false;
                    currentTimeOfInvincibility = 0;
                }
            }

            Vector2 currentOffset = rightSupportStartPos + rightSupportOffset * (currentTimeOfSupportMovement / timeOfSupportMovement);
            supportGenerator1.Position = Position + currentOffset * new Vector2(-1, 1);
            supportGenerator2.Position = Position + currentOffset;

            if (GameMain.Input.KeyDown(Key.Z))
            {
                mainGenerator.CurrentPauseTime = 
                supportGenerator1.CurrentPauseTime =
                supportGenerator2.CurrentPauseTime = 0;
            }
            else
            {
                mainGenerator.CurrentPauseTime =
                supportGenerator1.CurrentPauseTime =
                supportGenerator2.CurrentPauseTime = float.PositiveInfinity;

            }

            float temp;
            if (GameMain.Input.KeyDown(Key.ShiftLeft))
            {
                temp = halfVelocityScalar;
                if (currentTimeOfSupportMovement >= timeOfSupportMovement)
                    currentTimeOfSupportMovement = timeOfSupportMovement;
                else currentTimeOfSupportMovement += elapsedTime;
            }
            else
            {
                temp = velocityScalar;
                if (currentTimeOfSupportMovement <= 0)
                    currentTimeOfSupportMovement = 0;
                else currentTimeOfSupportMovement -= elapsedTime;
            }
            Vector2 velocity = Vector2.Zero;
            if (GameMain.Input.KeyDown(Key.Left))
                velocity.X = -temp;
            else if (GameMain.Input.KeyDown(Key.Right))
                velocity.X = temp;

            if (GameMain.Input.KeyDown(Key.Down))
                velocity.Y = -temp;
            else if (GameMain.Input.KeyDown(Key.Up))
                velocity.Y = temp;

            this.Velocity = velocity;
            base.Update(elapsedTime);

            if (Position.X + HalfHitBoxSize > World.BottomRightPoint.X) Position.X = World.BottomRightPoint.X - HalfHitBoxSize;
            else if (Position.X - HalfHitBoxSize < World.TopLeftPoint.X) Position.X = World.TopLeftPoint.X + HalfHitBoxSize;
            if (Position.Y + HalfHitBoxSize > World.TopLeftPoint.Y) Position.Y = World.TopLeftPoint.Y - HalfHitBoxSize;
            else if (Position.Y - HalfHitBoxSize < World.BottomRightPoint.Y) Position.Y = World.BottomRightPoint.Y + HalfHitBoxSize;

            if (GameMain.Input.KeyPress(Key.X))
            {
                if (GameMain.Stats.BombCount > 0)
                {
                    GameMain.World.Add(new Bomb(Position));
                    GameMain.Stats.BombCount--;
                }
            }

            if (GameMain.Input.KeyPress(Key.D))
                GameMain.World.Add(new Item(Position + new Vector2(0, 150)));
        }

        public override void Draw()
        {
            if (GameMain.DrawHitboxes) GameMain.Graphics.Draw(GameMain.SpriteCollection["_collision"].Texture, Position, HitBoxSize * Vector2.One, 0, Depth);

            if (IsInvincible)
            {
                var value = (int)((MathF.Sin(currentTimeOfInvincibility / timeOfInvincibility * MathHelper.TwoPi * 6) + 1) / 2f * 255);
                GameMain.Graphics.Draw(playerSprite.Texture, Position, HitBoxSize * playerSprite.SizeRelativeToHitbox, 0,
                    Color.FromArgb(value, value, value), Depth);
            }
            else GameMain.Graphics.Draw(playerSprite.Texture, Position, HitBoxSize * playerSprite.SizeRelativeToHitbox, 0, Depth);

            GameMain.Graphics.Draw(supportBallTexture, supportGenerator1.Position, supportBallSize, -GameMain.Time.TotalElapsedSeconds, SupportDepth);
            GameMain.Graphics.Draw(supportBallTexture, supportGenerator2.Position, supportBallSize, GameMain.Time.TotalElapsedSeconds, SupportDepth);
        }

        public override void OnCollision(GameObject gameObject)
        {
            base.OnCollision(gameObject);
            switch (gameObject)
            {
                case Projectile proj:
                    if (proj.IsEnemyProjectile && !IsInvincible && SqrCollisionCheck(gameObject) &&
                        RoundCollisionCheck(gameObject)) Die(); break;
                case Enemy _:
                    if (!IsInvincible && SqrCollisionCheck(gameObject) && RoundCollisionCheck(gameObject)) Die(); break;
                default: return;
            }
        }

        private void Die()
        {
            if (--GameMain.Stats.LifeCount < 1)
            {
                //Console.WriteLine("Конец игры");
                return;
            }

            IsInvincible = true;
            currentTimeOfInvincibility = 0;

            Position = World.DefaultPlayerPosition;
        }
    }

}
