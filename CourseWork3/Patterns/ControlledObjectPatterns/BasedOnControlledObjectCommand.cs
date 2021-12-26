using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class BasedOnControlledObjectCommand<T, V> : ICommand<T> where T : GameObjects.ControlledObject<T>
    {    
        Action<T, object> action;
        Func<T, V> value;
        public BasedOnControlledObjectCommand(Action<T, object> command, Func<T, V> value)
        {
            this.action = command;
            this.value = value;
        }

        public void Invoke(T gameObject)
        {
            action(gameObject, value(gameObject));
        }
    }
}
