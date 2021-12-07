using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    interface ICommand<T> where T: ControlledObject<T>
    {
        abstract public void Invoke(T gameObject);
    }
}
