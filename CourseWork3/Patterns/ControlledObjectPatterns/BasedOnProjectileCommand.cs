using CourseWork3.Game;
using CourseWork3.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class BasedOnProjectileCommand : ICommand<Projectile>
    {
        Action<Projectile, object> action;
        Func<Projectile, float> value;

        public BasedOnProjectileCommand(Action<Projectile, object> command, Func<Projectile, float> value)
        {
            this.action = command;
            this.value = value;
        }

        public void Invoke(Projectile gameObject)
        {
            action(gameObject, value(gameObject));
        }
    }
}
