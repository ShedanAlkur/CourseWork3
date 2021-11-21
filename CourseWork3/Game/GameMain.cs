using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using CourseWork3.GraphicsOpenGL;
using System.IO;

namespace CourseWork3.Game
{
    static partial class GameMain
    {

        private static GameWindow window;
        public static GraphicsOpenGL.Graphics Graphics;
        public static GameTextureCollection TextureCollection;
        public static World World;
        public static GameInput Input;
        public static GameCamera Camera;
        public static GameStats Stats;
        public static GameTime Time;

        static Texture2D tex;
        static Vector2 projSize;

        static FrameBuffer worldFrameBuffer;
        static Vector2 worldSize;

        static int defaultWidth;
        static int defaultHeight;
        static float windowScale;
        static int windowXOffset;
        static int windowYOffset;

        static bool isLoaded;

        public static void Init(GameWindow window)
        { 
            GameMain.window = window;
            window.VSync = VSyncMode.On;

            window.Load += Window_Load;
            window.UpdateFrame += Window_UpdateFrame;
            window.RenderFrame += Window_RenderFrame;
            window.Resize += Window_Resize;
            window.FocusedChanged += Window_FocusedChanged;
            window.Closing += Window_Closing;

            windowScale = 1.0f;
            defaultWidth = window.Width;
            defaultHeight = window.Height;

            window.Run(200, 60);
        }

        private static void Window_Load(object sender, EventArgs e)
        {
            Graphics = GraphicsOpenGL.Graphics.Instance;
            Graphics.Init();

            TextureCollection = new GameTextureCollection();
            tex = new Texture2D(@"content\projectileDirected.png");
            TextureCollection.Add("projectile", tex);
            TextureCollection.Add("item", new Texture2D(@"content\Item.png"));

            int value = 480;
            worldSize = new Vector2(value, (int)(1.2f * value));
            worldFrameBuffer = new FrameBuffer(window.Width / 2, window.Height / 2);
            World = World.Instance;
            for (int i = -100; i <= 100; i+=20)
            {
            }
                World.Add(new Item(new Vector2(0, 0)));

            Input = new GameInput(window);
            Camera = new GameCamera(new Vector2(0, 0), GameCamera.MovementType.Linear, 1f, 0f);
            Stats = new GameStats();
            Time = new GameTime();

            projSize = new Vector2(50, 50);

            isLoaded = true;
        }

        private static void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (!isLoaded) return;
            Time.DeltaTimeOfUpdate = (float)e.Time;
            Input.Update();
            World.Update((float)e.Time);

            window.Title = "UPS: " + (1 / Time.DeltaTimeOfUpdate).ToString("0000") + " FPS: " + (1 / Time.DeltaTimeOfRender).ToString("0000");
        }

        private static void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            if (!isLoaded) return;
            Time.DeltaTimeOfRender = (float)e.Time;

            int width = window.Width;
            int height = window.Height;


            //// Рисуем мини-сцену
            //worldFrameBuffer.Bind();
            //Graphics.ApplyViewport((int)(worldSize.X * windowScale), (int)(worldSize.Y * windowScale));
            //Graphics.ApplyProjection((int)worldSize.X, (int)worldSize.Y);
            //Graphics.ApplyViewTransofrm(new Vector2(0, 0), new Vector2(1, -1), 0);
            //Graphics.BeginDraw(Color.FromArgb(50, 50, 128));
            //Graphics.Draw(tex, new Vector2(0, 0), new Vector2(50), 0, 2);
            //Graphics.Draw(tex, new Vector2(0, -50), new Vector2(projSize.X, projSize.Y), 0, Color.Blue, 1);
            //World.Render();

            //// Рисуем большую сцену
            //FrameBuffer.Unbind();
            //Graphics.ApplyViewport(windowXOffset, windowYOffset,
            //    (int)(defaultWidth * windowScale),
            //    (int)(defaultHeight * windowScale));
            //Graphics.ApplyProjection(defaultWidth, defaultHeight);
            //Graphics.ApplyViewTransofrm(new Vector2(0, 0), 1, 0);
            //Graphics.BeginDraw(Color.FromArgb(40, 40, 40));
            //Graphics.Draw(worldFrameBuffer.Texture, new Vector2(-150, 0),
            //    new Vector2(worldSize.X, worldSize.Y), 0, 0);
            //Graphics.Draw(tex, new Vector2(0, 0), new Vector2(defaultHeight), Time.TotalElapsedSeconds * 200, 100);


            Graphics.ApplyViewport(windowXOffset, windowYOffset,
                (int)(defaultWidth * windowScale),
                (int)(defaultHeight * windowScale));
            Graphics.ApplyProjection(defaultWidth, defaultHeight);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), 1, 0);
            Graphics.BeginDraw(Color.FromArgb(40, 40, 40));
            Graphics.Draw(tex, new Vector2(0, 0), new Vector2(defaultHeight), Time.TotalElapsedSeconds * 200, 100);

            Graphics.ApplyViewport(windowXOffset + 150, windowYOffset + 100, (int)(worldSize.X * windowScale), (int)(worldSize.Y * windowScale));
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            Graphics.ApplyProjection((int)worldSize.X, (int)worldSize.Y);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), new Vector2(1, 1), 0);
            Graphics.Draw(tex, new Vector2(0, 0), new Vector2(50), 0, 2);
            Graphics.Draw(tex, new Vector2(0, -50), new Vector2(projSize.X, projSize.Y), 0, Color.Blue, 1);
            World.Render();

            window.SwapBuffers();
        }

        private static void Window_Resize(object sender, EventArgs e)
        {
            
            windowScale = MathF.Min((float)window.Width / defaultWidth, (float)window.Height / defaultHeight);
            windowXOffset = (int)(window.Width - defaultWidth * windowScale) / 2;
            windowYOffset = (int)(window.Height - defaultHeight * windowScale) / 2;
            worldFrameBuffer.Resize((int)(worldSize.X * windowScale), (int)(worldSize.Y * windowScale));
        }

        private static void Window_FocusedChanged(object sender, EventArgs e)
        {

        }

        private static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
