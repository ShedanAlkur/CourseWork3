using CourseWork3.GraphicsOpenGL;
using CourseWork3.Parser;
using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CourseWork3.Game
{
    class Projectile : ControlledObject<Projectile>
    {
        const byte Depth = 10;

        public static readonly Type Type = typeof(Projectile);

        private Color color;
        public Color Color { get => color; set => color = value; }

        public bool IsEnemyProjectile { get; private set; }
        bool isGrazed;
        bool wasInWorld;

        public float GenTime { get; private set; }

        Sprite sprite;

        public static new Dictionary<string, Action<Projectile, object>> ActionsForParser;

        static Projectile()
        {
            ActionsForParser = new Dictionary<string, Action<Projectile, object>>
            {
                [Keywords.Set + Keywords.Color] = (Projectile obj, object value) => obj.Color = (Color)value,
                [Keywords.RandomColor] = (Projectile obj, object value) =>
                obj.Color = Color.FromArgb(GameMain.random.Next(0, 256), GameMain.random.Next(0, 256), GameMain.random.Next(0, 256)),
            };

            ControlledObject<Projectile>.ActionsForParser.ToList().ForEach(x =>
                ActionsForParser.Add(x.Key, x.Value));
        }

        public Projectile(Pattern<Projectile> pattern, Vector2 position, float angle, float time, bool isEnemyProjectile)
            : base(pattern, position)
        {
            this.VelocityAngle = angle;
            this.GenTime = time;
            this.IsEnemyProjectile = isEnemyProjectile;

            var texture = GameMain.TextureCollection["projectile"];
            sprite = new Sprite(texture, Vector2.One);
            HitBoxSize = 50;
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            if (WorldCollisionCheck()) { if (!wasInWorld) wasInWorld = true; }
            else if (wasInWorld) Terminated = true;
        }

        public override void Draw()
        {
            //Console.WriteLine($"{Position}");
            GameMain.Graphics.Draw(sprite.Texture, Position, HitBoxSize * sprite.SizeRelativeToHitbox, VelocityAngle - MathHelper.PiOver2, color, Depth);
        }

        public override void OnCollision(GameObject gameObject)
        {
            switch (gameObject)
            {
                case Player player:
                    if (!isGrazed && GrazeColllisionCheck(player)) OnGraze(); break;
                default: return;
            }
        }

        private void OnGraze()
        {
            isGrazed = true;
        }

        public bool GrazeColllisionCheck(Player player)
        {
            return
                this.Position.Y - this.HalfHitBoxSize < player.Position.Y + player.HalfHitBoxSize &&
                this.Position.Y + this.HalfHitBoxSize > player.Position.Y - player.HalfHitBoxSize &&
                this.Position.X + this.HalfHitBoxSize > player.Position.X - player.HalfHitBoxSize &&
                this.Position.X - this.HalfHitBoxSize < player.Position.X + player.HalfHitBoxSize;
        }
    }
}
