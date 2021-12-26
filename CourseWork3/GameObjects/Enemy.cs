using CourseWork3.Game;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Parser;
using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseWork3.GameObjects
{
    class Enemy : ControlledObject<Enemy>
    {
        const byte Depth = 20;
        public const float DefaultHitboxSize = 40;

        public static new Dictionary<string, Action<Enemy, object>> ActionsForParser;

        private int life;

        private List<Generator> ownedGenerators = new List<Generator>();


        private bool wasInWorld;

        public Sprite Sprite;

        static Enemy()
        {
            ActionsForParser = new Dictionary<string, Action<Enemy, object>>
            {
                [Keywords.Set + Keywords.Sprite] = (Enemy obj, object value) => obj.Sprite = (Sprite)value,
                [Keywords.Set + Keywords.Generator] = (Enemy obj, object value) =>
                {
                    obj.RemoveOwnedGenerators();
                    obj.ownedGenerators.Add(new Generator((Pattern<Generator>)value, obj));
                    GameMain.World.Add(obj.ownedGenerators.Last());
                },
                [Keywords.Set + Keywords.Life] = (Enemy obj, object value) => obj.life = Convert.ToInt32(value),

                [Keywords.Increase + Keywords.Generator] = (Enemy obj, object value) =>
                {
                    obj.ownedGenerators.Add(new Generator((Pattern<Generator>)value, obj));
                    GameMain.World.Add(obj.ownedGenerators.Last());
                },
                [Keywords.Increase + Keywords.Life] = (Enemy obj, object value) => obj.life += (int)value,

                [Keywords.Clear + Keywords.Generator] = (Enemy obj, object value) => obj.RemoveOwnedGenerators(),

                [Keywords.MoveTo] = (Enemy obj, object value) => 
                {
                    (Vector2 point, float? time) = ((Vector2, float?))value;
                    obj.MoveTo(point, time);
                }
            };
            ControlledObject<Enemy>.ActionsForParser.ToList().ForEach(x =>
                ActionsForParser.Add(x.Key, x.Value));
        }

        public void RemoveOwnedGenerators()
        {
            foreach (var gen in ownedGenerators) gen.Terminated = true;
            ownedGenerators.Clear();
        }

        public void MoveTo(Vector2 point, float? time)
        {
            AccelerationScalar = 0;
            var diff = point - Position;
            VelocityAngle = diff.GetAngle();
            if (!(time is null))
                VelocityScalar = diff.LengthFast / (float)time;
        }

        public Enemy(Pattern<Enemy> pattern, Vector2 position) : base(pattern, position)
        {
            HitBoxSize = DefaultHitboxSize;
            wasInWorld = false;
            Sprite = GameMain.SpriteCollection["_projectile"];
        }

        public override void Draw()
        {
            if (GameMain.DrawHitboxes) GameMain.Graphics.Draw(GameMain.SpriteCollection["_collision"].Texture, Position, HitBoxSize * Vector2.One, 0, Depth);
            GameMain.Graphics.Draw(Sprite.Texture, Position, HitBoxSize * Sprite.SizeRelativeToHitbox, 0, Depth);
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            if (life <= 0)
            { 
                Terminated = true;
                return;
            }

            if (WorldCollisionCheck()) { if (!wasInWorld) wasInWorld = true; }
            else if (wasInWorld) Terminated = true;
        }

        public override void OnCollision(GameObject gameObject)
        {
            switch (gameObject)
            {
                case Projectile projectile: 
                    if (!projectile.IsEnemyProjectile && !projectile.Terminated &&
                        SqrCollisionCheck(projectile) && RoundCollisionCheck(projectile)) TakeDamage(1);
                    break;
                default: break;
            }
        }

        private void TakeDamage(int damage)
        {
            GameMain.Stats.CurrentRecord += 1;
            life -= damage;
            if (life <= 0)
            {
                GameMain.World.Add(new ItemBomb(Position));
            }

        }
    }
}
