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
        private Color color;
        public Color Color { get => color; set => color = value; }

        private bool isEnemyProjectile;

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

        public Projectile(Pattern<Projectile> pattern, Vector2 position, float angle, bool isEnemyProjectile) 
            : base(pattern, position) 
        {
            this.VelocityAngle = angle;
            this.isEnemyProjectile = isEnemyProjectile;
        }

    }
}
