using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class Projectile : ControlledObject<Projectile>
    {
        public Projectile(Pattern<Projectile> pattern) : base(pattern) { }
    }
}
