using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class PropertyChangerCommand<T> : ICommand<T> where T : ControlledObject<T>
    {
        string command;
        object value;

        public void Invoke(ControlledObject<T> gameObject)
        {

            throw new NotImplementedException();
        }
    }
}
