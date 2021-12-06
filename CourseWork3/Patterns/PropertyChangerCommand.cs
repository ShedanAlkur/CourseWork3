using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class PropertyChangerCommand<T> : ICommand<T> where T : ControlledObject<T>
    {
        Action<T, object> action;
        object value;

        public PropertyChangerCommand(Action<T, object> command, object value)
        {
            this.action = command;
            this.value = value;
        }

        public void Invoke(T gameObject)
        {
            action(gameObject, value);
        }
    }
}
