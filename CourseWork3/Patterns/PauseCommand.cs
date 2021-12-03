using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class PauseCommand<T> : ICommand<T> where T : ControlledObject<T>
    {
        float pauseTime;

        public PauseCommand(float pauseTime)
        {
            this.pauseTime = pauseTime;
        }

        public void Invoke(T gameObject)
        {
            throw new NotImplementedException();
        }
    }
}
