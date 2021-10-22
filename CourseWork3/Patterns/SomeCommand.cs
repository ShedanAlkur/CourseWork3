using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class SomeCommand<T> : ICommand<T> where T : ControlledObject<T>
    {
        string wow;
        public SomeCommand(string wow)
        {
            this.wow = wow;
        }

        public void Invoke(ControlledObject<T> gameObject)
        {
            Console.WriteLine("Выполнена команда " + wow);
        }
    }
}
