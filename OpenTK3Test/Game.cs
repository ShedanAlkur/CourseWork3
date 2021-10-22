using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OpenTK3Test
{
    class Game
    {
        public GameWindow Window;

        public Game(GameWindow window)
        {
            this.Window = window;

            window.RenderFrame += Window_RenderFrame;
            window.UpdateFrame += Window_UpdateFrame;
        }


        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(0, 0);
            GL.Vertex2(1, 0);
            GL.Vertex2(0, 1);
            GL.End();

            Window.SwapBuffers();
        }
        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {

        }
    }
}
