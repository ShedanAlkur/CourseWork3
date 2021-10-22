using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class EnemyProjectile : Projectile
    {
        public EnemyProjectile(Pattern<Projectile> pattern) : base(pattern)
        {
        }
    }
}
