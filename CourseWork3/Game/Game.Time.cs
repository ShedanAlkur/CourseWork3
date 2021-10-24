using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using CourseWork3.GraphicsOpenGL;
using System.IO;

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
