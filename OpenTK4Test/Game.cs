using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace OpenTK4Test
{

    class Game
    {
        //public GameWindow Window;
        //public Game(GameWindow window)
        //{
        //    this.Window = window;

        //    window.RenderFrame += Window_RenderFrame;
        //}

        //private void Window_RenderFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
        //{
        //    GL.ClearColor(Color.CornflowerBlue);
        //    GL.Clear(ClearBufferMask.ColorBufferBit);

        //    GL.Begin(PrimitiveType.);
        //    GL.Vertex2(0, 0);
        //    GL.Vertex2(1, 0);
        //    GL.Vertex2(0, 1);
        //    GL.End();

        //    Window.SwapBuffers();
        //}

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
