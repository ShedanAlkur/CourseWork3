using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class RuntimeCommand<T> : ICommand<T> where T : ControlledObject<T>
    {
        private float maxRunTime;

        public RuntimeCommand(float maxRunTime)
        {
            this.maxRunTime = maxRunTime;
        }

        public void Invoke(T gameObject)
        {
            gameObject.CurrentRuntime = 0;
            gameObject.MaxRuntime = this.maxRunTime;
            gameObject.IsSelectedRuntimeCommand = true;
        }
    }
}
