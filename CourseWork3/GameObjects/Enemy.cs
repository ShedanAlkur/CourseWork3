using CourseWork3.GraphicsOpenGL;
using CourseWork3.Parser;
using CourseWork3.Patterns;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseWork3.Game
{
    class Enemy : ControlledObject<Enemy>
    {
        const byte Depth = 20;
        public const float DefaultHitbox = 50;

        public static new Dictionary<string, Action<Enemy, object>> ActionsForParser;

        private int life;

        private List<Generator> ownedGenerators = new List<Generator>();


        private bool wasInWorld;

        Sprite sprite;

        static Enemy()
        {
            ActionsForParser = new Dictionary<string, Action<Enemy, object>>
            {
                [Keywords.Set + Keywords.Sprite] = (Enemy obj, object value) => obj.sprite = (Sprite)value,
                [Keywords.Set + Keywords.Generator] = (Enemy obj, object value) =>
                {
                    obj.RemoveOwnedGenerators();
                    obj.ownedGenerators.Add(new Generator((Pattern<Generator>)value, obj));
                    GameMain.World.Add(obj.ownedGenerators.Last());
                },
                [Keywords.Set + Keywords.Life] = (Enemy obj, object value) => obj.life = (int)value,

                [Keywords.Increase + Keywords.Generator] = (Enemy obj, object value) =>
                {
                    obj.ownedGenerators.Add(new Generator((Pattern<Generator>)value, obj));
                    GameMain.World.Add(obj.ownedGenerators.Last());
                },
                [Keywords.Increase + Keywords.Life] = (Enemy obj, object value) => obj.life += (int)value,

                [Keywords.Clear + Keywords.Sprite] = (Enemy obj, object value) => throw new NotImplementedException(),
                [Keywords.Clear + Keywords.Generator] = (Enemy obj, object value) => obj.RemoveOwnedGenerators(),
            };
            ControlledObject<Enemy>.ActionsForParser.ToList().ForEach(x =>
                ActionsForParser.Add(x.Key, x.Value));
        }

        public void RemoveOwnedGenerators()
        {
            foreach (var gen in ownedGenerators) gen.Terminated = true;
            ownedGenerators.Clear();
        }

        public Enemy(Pattern<Enemy> pattern, Vector2 position) : base(pattern, position)
        {
            wasInWorld = false;
            sprite = GameMain.SpriteCollection["_projectile"];
            HitBoxSize = 50;
        }

        public override void Draw()
        {
            if (GameMain.DrawHitboxes) GameMain.Graphics.Draw(GameMain.SpriteCollection["_collision"].Texture, Position, HitBoxSize * Vector2.One, 0, Depth);
            GameMain.Graphics.Draw(sprite.Texture, Position, HitBoxSize * sprite.SizeRelativeToHitbox, 0, Depth);
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
    }
}
