using System;
using System.Collections.Generic;
using System.Text;
using CourseWork3.Patterns;

namespace CourseWork3.Game
{
    class PlayerLevel
    {
        public int RequiredPower;
        public Pattern<Generator<PlayerProjectile>> MainShots;
        public Pattern<Generator<PlayerProjectile>> SubShots;
    }
}
