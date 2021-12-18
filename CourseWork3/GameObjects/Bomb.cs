using CourseWork3.Game;
using CourseWork3.GraphicsOpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.GameObjects
{
    class Bomb : GameObject
    {
        const byte Depth = 100;
        public const float DefaultHitboxSize = 160;
        const float maxLifeTime = 7f;

        float currentLifeTime;
        Sprite sprite;

        public Bomb(Vector2 position) : base(position)
        {
            HitBoxSize = DefaultHitboxSize;
            sprite = GameMain.SpriteCollection["_bomb"];
        }

        public override void Update(float elapsedTime)
        {
            currentLifeTime += elapsedTime;
            if (currentLifeTime > maxLifeTime) Terminated = true;
        }

        public override void Draw()
        {
            GameMain.Graphics.Draw(sprite.Texture, Position, HitBoxSize * sprite.SizeRelativeToHitbox, currentLifeTime, Depth);
            if (GameMain.DrawHitboxes) GameMain.Graphics.Draw(GameMain.SpriteCollection["_collision"].Texture, Position, HitBoxSize * Vector2.One, 0, Depth);
        }
    }
}
