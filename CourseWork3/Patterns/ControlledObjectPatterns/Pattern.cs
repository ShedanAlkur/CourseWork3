using CourseWork3.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class Pattern<T> : ICommand<T> where T : ControlledObject<T>
    {
        private ICommand<T>[] commands;

        private int? repeatIndex;

        public Pattern(ICommand<T>[] commands, int? repeatIndex = null)
        {
            this.commands = commands;
            this.repeatIndex = repeatIndex;
        }

        public void Invoke(T gameObject)
        {
            gameObject.IsSelectedRuntimeCommand = false;
            // Применяем очередность команд, пока не дойдем до команды выполнения runtime
            do
            { 
                // Если индекс текущей команды превысил количество команд, то выходим или возвращаемся к точке повтора
                if (gameObject.CurrentIndex >= commands.Length)
                {

                    if (repeatIndex == null) return;
                    else gameObject.CurrentIndex = (int)repeatIndex;
                }
                // Выполняем текущую команду
                commands[gameObject.CurrentIndex++].Invoke(gameObject);
            }
            while (!gameObject.IsSelectedRuntimeCommand);

        }
    }
}
