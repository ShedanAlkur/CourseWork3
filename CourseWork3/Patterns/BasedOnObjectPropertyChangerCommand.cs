using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class BasedOnObjectPropertyChangerCommand<T> : ICommand<T> where T : ControlledObject<T>
    {
        Action<T, object> command;
        Func<T, float> value;

        public BasedOnObjectPropertyChangerCommand(Action<T, object> command, Func<T, float> value)
        {
            this.command = command;
            this.value = value;
        }

        public void Invoke(T gameObject)
        {
            command(gameObject, value(gameObject));
        }
    }
}
