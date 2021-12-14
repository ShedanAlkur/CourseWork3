using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class LevelPattern
    {
        private ILevelCommand[] commands;

        public LevelPattern(ILevelCommand[] commands)
        {
            this.commands = commands ?? new ILevelCommand[0];
        }

        public void Invoke()
        {
            do
            {
                if (Game.GameMain.World.CurrentIndex >= commands.Length) return;
                commands[Game.GameMain.World.CurrentIndex].Invoke();
            }
            while (!(commands[Game.GameMain.World.CurrentIndex++] is PauseCommand));
        }
    }
}
