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
        public static new Dictionary<string, Action<Enemy, object>> ActionsForParser;

        private int life;

        private List<Generator> ownedGenerators = new List<Generator>();

        Texture2D texture;
        Vector2 size;

        static Enemy()
        {
            ActionsForParser = new Dictionary<string, Action<Enemy, object>>
            {
                [Keywords.Set + Keywords.Sprite] = (Enemy obj, object value) => throw new NotImplementedException(),
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
            texture = GameMain.TextureCollection["projectile"];
            size = new Vector2(50, 50);
        }

        public override void Draw()
        {
            GameMain.Graphics.Draw(texture, Position, size, 0,System.Drawing.Color.Red, 2);
        }
    }
}
