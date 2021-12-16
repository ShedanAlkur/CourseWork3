using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using CourseWork3.GraphicsOpenGL;
using System.IO;
using CourseWork3.Patterns;
using System.Collections.Generic;

namespace CourseWork3.Game
{
    static partial class GameMain
    {

        private static GameWindow window;
        public static GraphicsOpenGL.Graphics Graphics;

        public static Dictionary<string, Sprite> SpriteCollection;
        public static Dictionary<string, Texture2D> TextureCollection;
        public static Dictionary<string, Pattern<Projectile>> ProjeciltePatternCollection;
        public static Dictionary<string, Pattern<Generator>> GeneratorPatternCollection;
        public static Dictionary<string, Pattern<Enemy>> EnemyPatternCollection;

        public static World World;
        public static GameInput Input;
        public static GameCamera Camera;
        public static GameStats Stats;
        public static GameTime Time;

        public static Random random;

        public static Texture2D tex;
        static Vector2 projSize;

        static FrameBuffer worldFrameBuffer;
        static Vector2 worldSize;

        static int defaultWidth;
        static int defaultHeight;
        static float windowScale;
        static int windowXOffset;
        static int windowYOffset;
        static int defWorldXOffset;
        static int defWorldYOffset;

        static bool isLoaded;

        static string pathOfFileForParser;

        public static void Init(GameWindow window, string[] args)
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

            int value = 480;
            worldSize = new Vector2(value, (int)(1.2f * value));

            defWorldXOffset = (int)(25);
            defWorldYOffset = (int)((defaultHeight - worldSize.Y) / 2);
            if (args.Length > 0) pathOfFileForParser = args[0];

            window.Run(200, 60);
        }

        private static void Window_Load(object sender, EventArgs e)
        {
            random = new Random();

            Graphics = GraphicsOpenGL.Graphics.Instance;
            Graphics.Init();

            SpriteCollection = new Dictionary<string, Sprite>();
            ProjeciltePatternCollection = new Dictionary<string, Pattern<Projectile>>();
            GeneratorPatternCollection = new Dictionary<string, Pattern<Generator>>();
            EnemyPatternCollection = new Dictionary<string, Pattern<Enemy>>();
            TextureCollection = new Dictionary<string, Texture2D>();

            string currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            tex = new Texture2D(currentDirectory + @"\content\projectileDirected.png");
            TextureCollection.Add("projectile", tex);
            TextureCollection.Add("item", new Texture2D(currentDirectory + @"\content\Item.png"));

            worldFrameBuffer = new FrameBuffer(window.Width / 2, window.Height / 2);
            projSize = new Vector2(50, 50);

            World = World.Instance;

            var parser = new Parser.Parser();
            Console.WriteLine("путь:" + pathOfFileForParser);
            parser.ParseFile(pathOfFileForParser ?? currentDirectory +  @"\Content\fileForParser.txt");

            Input = new GameInput(window);
            Camera = new GameCamera(new Vector2(0, 0), GameCamera.MovementType.Linear, 1f, 0f);
            Stats = new GameStats();
            Time = new GameTime();

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

            #region отрисовка через framebuffer
            // Рисуем мини-сцену
            worldFrameBuffer.Bind();
            Graphics.ApplyViewport((int)(worldSize.X * windowScale), (int)(worldSize.Y * windowScale));
            Graphics.ApplyProjection((int)worldSize.X, (int)worldSize.Y);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), new Vector2(1, -1), 0);
            Graphics.BeginDraw(Color.FromArgb(50, 50, 128));
            World.Render();

            // Рисуем большую сцену
            FrameBuffer.Unbind();
            Graphics.ApplyViewport(windowXOffset, windowYOffset,
                (int)(defaultWidth * windowScale),
                (int)(defaultHeight * windowScale));
            Graphics.ApplyProjection(defaultWidth, defaultHeight);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), 1, 0);
            Graphics.BeginDraw(Color.FromArgb(40, 40, 40));
            Graphics.Draw(worldFrameBuffer.Texture, new Vector2(-150, 0),
                new Vector2(worldSize.X, worldSize.Y), 0, 0);
            Graphics.Draw(tex, new Vector2(0, 0), new Vector2(defaultHeight), Time.TotalElapsedSeconds, 100);
            #endregion

            #region отрисовка через viewport
            //// Рисуем большую сцену
            //Graphics.ApplyViewport(windowXOffset, windowYOffset,
            //    (int)(defaultWidth * windowScale),
            //    (int)(defaultHeight * windowScale));
            //Graphics.ApplyProjection(defaultWidth, defaultHeight);
            //Graphics.ApplyViewTransofrm(new Vector2(0, 0), 1, 0);
            //Graphics.BeginDraw(Color.FromArgb(40, 40, 40));
            //Graphics.Draw(tex, new Vector2(0, 0), new Vector2(defaultHeight), Time.TotalElapsedSeconds * 200, 100);

            //// Рисуем мини-сцену
            //Graphics.ApplyViewport(windowXOffset + (int)(defWorldXOffset * windowScale), (int)(windowYOffset + defWorldYOffset * windowScale),
            //    (int)(worldSize.X * windowScale), (int)(worldSize.Y * windowScale));
            ////GL.Clear(ClearBufferMask.ColorBufferBit);
            //Graphics.ApplyProjection((int)worldSize.X, (int)worldSize.Y);
            //Graphics.ApplyViewTransofrm(new Vector2(0, 0), new Vector2(1, 1), 0);
            //GL.Color3(Color.White);
            //GL.Rect(0, 0, worldSize.X, worldSize.Y);
            //Graphics.Draw(tex, new Vector2(0, 0), new Vector2(50), 0, 2);
            //Graphics.Draw(tex, Vector2.Zero, worldSize, 0, Color.GreenYellow, 99);
            //Graphics.Draw(tex, new Vector2(0, -50), new Vector2(projSize.X, projSize.Y), 0, Color.Blue, 1);
            //World.Render();
            #endregion

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
