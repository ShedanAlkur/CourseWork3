using CourseWork3.Game;
using CourseWork3.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class BasedOnProjectileChangerCommand : ICommand<Projectile>
    {
        Action<Projectile, object> action;
        Func<Projectile, float> value;

        public BasedOnProjectileChangerCommand(Action<Projectile, object> command, Func<Projectile, float> value)
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
