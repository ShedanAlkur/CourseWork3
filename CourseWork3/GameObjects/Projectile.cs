using CourseWork3.Parser;
using CourseWork3.Patterns;
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

        public static new Dictionary<string, Dictionary<string, Action<Projectile, object>>> ParserActionByTwoCommand;
        public static new Dictionary<string, Action<Projectile, object>> ParserActionByOneCommand;

        static Projectile()
        {
            ParserActionByTwoCommand = new Dictionary<string, Dictionary<string, Action<Projectile, object>>>
            {

                [Keywords.Set] = new Dictionary<string, Action<Projectile, object>>
                {
                    [Keywords.Color] = (Projectile obj, object value) => obj.Color = (Color)value,
                },
                [Keywords.Increase] = new Dictionary<string, Action<Projectile, object>>(),
            };
            ControlledObject<Projectile>.ParserActionByTwoCommand.ToList().ForEach(firstCommand =>
                firstCommand.Value.ToList().ForEach(secondCommand =>
                ParserActionByTwoCommand[firstCommand.Key].Add(secondCommand.Key, secondCommand.Value)));

            ParserActionByOneCommand = new Dictionary<string, Action<Projectile, object>>();
            ControlledObject<Projectile>.ParserActionByOneCommand.ToList().ForEach(command =>
                ParserActionByOneCommand.Add(command.Key, command.Value));
        }

        public Projectile(Pattern<Projectile> pattern) : base(pattern) { }

    }
}
