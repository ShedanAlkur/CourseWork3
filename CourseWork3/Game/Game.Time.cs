using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    partial class GameMain
    {
        public class GameTime
        {
            private float deltaTimeOfUpdate;
            public float DeltaTimeOfUpdate
            {
                get => deltaTimeOfUpdate;
                set
                {
                    deltaTimeOfUpdate = value;
                    TotalElapsedSeconds += value;
                }
            }
            public float DeltaTimeOfRender;
            public float TotalElapsedSeconds { get; private set; }

            public GameTime()
            {

            }
        }
    }
}
