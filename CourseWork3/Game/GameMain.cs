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
        public GameInput Input { get; private set; }
        public GraphicsOpenGL.Graphics Graphics { get; private set; }
        public World World { get; private set; }
        GameWindow window;

        Texture2D tex;
        FrameBuffer worldFrameBuffer;
        public GameMain(GameWindow window)
        {
            this.window = window;
            window.VSync = VSyncMode.On;

            window.Load += Window_Load;
            window.UpdateFrame += Window_UpdateFrame;
            window.RenderFrame += Window_RenderFrame;
            window.Resize += Window_Resize;
            window.FocusedChanged += Window_FocusedChanged;
            window.Closing += Window_Closing;

            Graphics = new GraphicsOpenGL.Graphics();
            //pause = true;
            //IsRunning = true;
        }



        private void Window_Load(object sender, EventArgs e)
        {
            Graphics.Init();
            Graphics.LoadQuadVBO();
            tex = new Texture2D(@"content\projectile.png");

            worldFrameBuffer = new FrameBuffer(window.Width / 2, window.Height / 2);

            //tex = new Texture2D(@"content1\projectile.png");
        }

        private void Window_UpdateFrame(object sender, EventArgs e)
        {
            //window.Title = "UPS: " + (1 / obj.Time).ToString("0000");
        }

        private void Window_RenderFrame(object sender, EventArgs e)
        {
            //window.Title += " FPS: " + (1 / obj.Time).ToString("0000");

            int width = window.Width;
            int height = window.Height;

            // Рисуем мини-сцену
            worldFrameBuffer.Bind();
            Graphics.ApplyProjection(width / 2, height / 2);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), 1, 0);
            Graphics.BeginDraw(Color.FromArgb(50, 50, 128));
            Graphics.Draw(tex, new Vector2(0, 0), new Vector2(50), 0, 2);

            // Рисуем большую сцену
            FrameBuffer.Unbind();
            Graphics.ApplyProjection(width, height);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), 1, 0);
            Graphics.BeginDraw(Color.FromArgb(50, 50, 50));
            Graphics.Draw(tex, new Vector2(0, 0), new Vector2(50), 0, 2);
            Graphics.Draw(worldFrameBuffer.Texture, new Vector2(width / 4, 0), new Vector2(width / 4, height / 4), 0, 0);

            window.SwapBuffers();
        }

        private void Window_Resize(object sender, EventArgs e)
        {
            worldFrameBuffer.Resize(window.Width / 2, window.Height / 2);
        }

        private void Window_FocusedChanged(object sender, EventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
