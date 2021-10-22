using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class PlayerProjectile : Projectile
    {
        public PlayerProjectile(Pattern<Projectile> pattern) : base(pattern)
        {
        }
    }
}
