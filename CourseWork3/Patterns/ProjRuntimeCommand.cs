using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class ProjRuntimeCommand : ICommand<Projectile>
    {
        private Func<Projectile, float> maxRunTime;

        public ProjRuntimeCommand(Func<Projectile, float> maxRunTime)
        {
            this.maxRunTime = maxRunTime;
        }

        public void Invoke(Projectile gameObject)
        {
            gameObject.CurrentRuntime = 0;
            gameObject.MaxRuntime = (float)maxRunTime(gameObject);
            gameObject.IsSelectedRuntimeCommand = true;
        }
    }
}
