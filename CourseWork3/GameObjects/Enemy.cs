using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    class Enemy : ControlledObject<Enemy>
    {
        private int hp;

        public Enemy(Pattern<Enemy> pattern) : base(pattern)
        {
        }
    }
}
