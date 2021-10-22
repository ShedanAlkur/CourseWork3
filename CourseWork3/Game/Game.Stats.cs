using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Game
{
    partial class GameMain
    {
        public class GameStats
        {
            private static GameStats instance;

            public static GameStats Instance
            {
                get
                {
                    if (instance == null)
                        instance = new GameStats();
                    return instance;
                }
            }

            private GameStats()
            {

            }


        }
    }
}
