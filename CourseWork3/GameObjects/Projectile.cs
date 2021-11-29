using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CourseWork3.Game
{
    class Projectile : ControlledObject<Projectile>
    {
        private Color color;
        public Color Color { get => color; set => color = value; }

        public static new Dictionary<string, Action<Projectile, object>> ParserMethods 
            = new Dictionary<string, Action<Projectile, object>>
        {

        };

        static Projectile()
        {
            ParserMethods = new Dictionary<string, Action<Projectile, object>>
            {
                [$"set-color".ToLower()] = (Projectile obj, object value) => obj.Color = (Color)value,
            };
        }

        public Projectile(Pattern<Projectile> pattern) : base(pattern) { }

    }
}
