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
        GameWindow window;
        public GameInput Input { get; private set; }
        public GraphicsOpenGL.Graphics Graphics { get; private set; }
        public World World { get; private set; }
        private GameCamera camera;

        Texture2D tex;
        Vector2 projSize;

        FrameBuffer worldFrameBuffer;
        Vector2 worldSize;

        int defaultWidth;
        int defaultHeight;
        float windowScaleParam;

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
            camera = new GameCamera(new Vector2(0, 0), GameCamera.MovementType.Linear, 1f, 0f);
            //pause = true;
            //IsRunning = true;

            windowScaleParam = 1.0f;
            defaultWidth = window.Width;
            defaultHeight = window.Height;

        }

        private void Window_Load(object sender, EventArgs e)
        {
            Graphics.Init();
            Graphics.LoadQuadVBO();
            tex = new Texture2D(@"content\projectile.png");
            projSize = new Vector2(50, 50);
            worldSize = new Vector2(480 / 2, 620 / 2);
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
            Graphics.ApplyProjection((int)(worldSize.X * windowScaleParam), (int)(worldSize.Y * windowScaleParam));
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), new Vector2(windowScaleParam, -windowScaleParam), 0);
            Graphics.BeginDraw(Color.FromArgb(50, 50, 128));
            Graphics.Draw(tex, new Vector2(0, 0), new Vector2(50), 0, 2);
            Graphics.Draw(tex, new Vector2(0, -50), new Vector2(projSize.X, projSize.Y), 0, Color.Blue, 0);

            // Рисуем большую сцену
            FrameBuffer.Unbind();
            Graphics.ApplyProjection(width, height);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), windowScaleParam, 0);

            Graphics.BeginDraw(Color.FromArgb(40, 40, 40));

            Graphics.Draw(tex, new Vector2(15, 0), new Vector2(projSize.X, projSize.Y), 0, Color.Red, 0);
            Graphics.Draw(tex, new Vector2(-15, 0), new Vector2(projSize.X, projSize.Y), 0, Color.Blue, 0);
            Graphics.Draw(tex, new Vector2(0, 25), new Vector2(projSize.X, projSize.Y), 0, Color.Green, 0);

            Graphics.Draw(worldFrameBuffer.Texture, new Vector2(-worldSize.X, 0),
                new Vector2(worldSize.X, worldSize.Y), 0, 0);

            window.SwapBuffers();
        }

        private void Window_Resize(object sender, EventArgs e)
        {
            windowScaleParam = MathF.Min((float)window.Width / defaultWidth, (float)window.Height / defaultHeight);
            worldFrameBuffer.Resize((int)(worldSize.X * windowScaleParam), (int)(worldSize.Y * windowScaleParam));
        }

        private void Window_FocusedChanged(object sender, EventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
