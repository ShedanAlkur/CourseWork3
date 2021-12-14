using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    class PauseCommand : ILevelCommand
    {
        float pauseTime;

        public PauseCommand(float pauseTime)
        {
            this.pauseTime = pauseTime;
        }

        public void Invoke()
        {
            Game.GameMain.World.CurrentPausetime = 0;
            Game.GameMain.World.MaxPausetime = pauseTime;
        }

        public override string ToString()
        {
            return $"Pause on {pauseTime} second";
        }
    }
}
