using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class SetterCommand<T> : ICommand<T> where T : ControlledObject<T>
    {
        Action<T, object> action;
        object value;

        public SetterCommand(Action<T, object> action, object value)
        {
            this.action = action;
            this.value = value;
        }

        public void Invoke(T gameObject)
        {
            action(gameObject, value);
        }
    }

}
