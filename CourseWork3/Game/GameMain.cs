using CourseWork3.GameObjects;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Patterns;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CourseWork3.Game
{
    /// <summary>
    /// Класс GameMain является основным классом программы. 
    /// Является статическим для доступа к игровым данных из любой части системы. 
    /// Этот класс выполняет хранение всех графических материалов и шаблонов, 
    /// загруженных в игру; контролирует циклы обновления и отрисовки игровых объектов.
    /// </summary>
    static partial class GameMain
    {

        private static GameWindow window;
        public static GraphicsOpenGL.Graphics Graphics;
        static float accumulatedTime;
        static float targetUpdatePeriod;

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

        private static TextRenderer statsRenderer;

        static FrameBuffer worldFrameBuffer;

        static int defaultWidth;
        static int defaultHeight;
        static float windowScale;
        static int windowXOffset;
        static int windowYOffset;
        static int defWorldXOffset;
        static int defWorldYOffset;

        static bool isLoaded;
        static bool isPaused;
        public static bool DrawHitboxes;

        public static string PathOfPatternFile;
        public static string PathOfPatternFolder;
        public static string PathOfExecuteFolder;

        public static void Init(GameWindow window, string pathOfPatternFile = null)
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

            defWorldXOffset = (int)(25);
            defWorldYOffset = (int)((defaultHeight - World.Size.Y) / 2);

            PathOfExecuteFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(pathOfPatternFile))
            {
                PathOfPatternFile = pathOfPatternFile;
                PathOfPatternFolder = Path.GetDirectoryName(PathOfPatternFile);
            }
            else PathOfPatternFile = PathOfExecuteFolder + @"\Content\fileForParser.txt";



            int updatesPerSecond = 200;
            int framesPerSecond = 60;
            targetUpdatePeriod = 1f / updatesPerSecond;
            window.Run(updatesPerSecond, framesPerSecond);
        }

        private static void LoadDefaultResources()
        {
            TextureCollection.Add("_item", new Texture2D(PathOfExecuteFolder + @"\content\Item.png"));
            SpriteCollection.Add("_item", new Sprite(TextureCollection["_item"], new Vector2(24) / Item.DefaultHitboxSize));

            TextureCollection.Add("_arrow", new Texture2D(PathOfExecuteFolder + @"\content\Arrow.png"));
            SpriteCollection.Add("_arrow", new Sprite(TextureCollection["_arrow"], new Vector2(24) / Item.DefaultHitboxSize));

            TextureCollection.Add("_player", new Texture2D(PathOfExecuteFolder + @"\content\Player.png"));
            SpriteCollection.Add("_player", new Sprite(TextureCollection["_player"], new Vector2(40, 60) / Player.DefaultHitboxSize));
            TextureCollection.Add("_supportBall", new Texture2D(PathOfExecuteFolder + @"\content\SupportBall.png"));

            TextureCollection.Add("_collision", new Texture2D(PathOfExecuteFolder + @"\content\Collision.png"));
            SpriteCollection.Add("_collision", new Sprite(TextureCollection["_collision"], Vector2.One));

            TextureCollection.Add("_projectile", new Texture2D(PathOfExecuteFolder + @"\content\Projectile.png"));
            SpriteCollection.Add("_projectile", new Sprite(TextureCollection["_projectile"], new Vector2(20) / Projectile.DefaultHitboxSize));

            TextureCollection.Add("_bomb", new Texture2D(PathOfExecuteFolder + @"\content\Bomb2.png"));
            SpriteCollection.Add("_bomb", new Sprite(TextureCollection["_bomb"], new Vector2(150) / GameObjects.Bomb.DefaultHitboxSize));

            var parser = new Parser.BetterParser();
            parser.ParseFile(PathOfExecuteFolder + @"\content\MainPatterns.bs");
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

            worldFrameBuffer = new FrameBuffer(window.Width / 2, window.Height / 2);

            var font = new Font("Consolas", 15f, FontStyle.Regular, GraphicsUnit.Point);
            statsRenderer = new TextRenderer(200, 200, Color.Gray, Color.White, font);

            World = World.Instance;
            LoadDefaultResources();
            if (!string.IsNullOrEmpty(PathOfPatternFile))
            {
                var parser = new Parser.BetterParser();
                Console.WriteLine($"Загружается шаблон {PathOfPatternFile}");
                parser.ParseFile(PathOfPatternFile);
            }

            World.InitPlayer();

            Input = new GameInput(window);
            Camera = new GameCamera(new Vector2(0, 0), GameCamera.MovementType.Linear, 1f, 0f);
            Stats = new GameStats();
            Time = new GameTime();

            isLoaded = true;
            isPaused = true;
            DrawHitboxes = false;
        }

        private static void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (!isLoaded) return;
            Time.DeltaTimeOfUpdate = (float)e.Time;

            if (Input.KeyPress(Key.Enter)) isPaused = !isPaused;
            if (Input.KeyPress(Key.C)) DrawHitboxes = !DrawHitboxes;


            if (!isPaused)
            {
                accumulatedTime += Time.DeltaTimeOfUpdate;
                while (accumulatedTime >= targetUpdatePeriod)
                {
                    accumulatedTime -= targetUpdatePeriod;
                    World.Update(targetUpdatePeriod);
                }
            }

            Input.Update();

            window.Title = "UPS: " + (1 / Time.DeltaTimeOfUpdate).ToString("0000") + " FPS: " + (1 / Time.DeltaTimeOfRender).ToString("0000") + " Gameobjects: " + World.gameObjects.Count;
        }

        private static void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            if (!isLoaded) return;
            Time.DeltaTimeOfRender = (float)e.Time;

            #region отрисовка через framebuffer
            // Рисуем мини-сцену
            worldFrameBuffer.Bind();
            Graphics.ApplyViewport((int)(World.Size.X * windowScale), (int)(World.Size.Y * windowScale));
            Graphics.ApplyProjection((int)World.Size.X, (int)World.Size.Y);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), new Vector2(1, -1), 0);
            Graphics.BeginDraw(Color.FromArgb(40, 40, 40));
            World.Render();

            // Рисуем большую сцену
            FrameBuffer.Unbind();
            Graphics.ApplyViewport(windowXOffset, windowYOffset,
                (int)(defaultWidth * windowScale),
                (int)(defaultHeight * windowScale));
            Graphics.ApplyProjection(defaultWidth, defaultHeight);
            Graphics.ApplyViewTransofrm(new Vector2(0, 0), 1, 0);
            Graphics.BeginDraw(Color.FromArgb(50, 50, 128));
            Graphics.Draw(worldFrameBuffer.Texture, new Vector2(-150, 0), World.Size, 0, 10);
            Graphics.Draw(TextureCollection["_projectile"], new Vector2(0, 0), new Vector2(defaultHeight * (MathF.Sin(Time.TotalElapsedSeconds)/2+1)), Time.TotalElapsedSeconds, 100);

            statsRenderer.Text = "IsPaused " + isPaused + 
                "\nRecord:".PadRight(10) + Stats.CurrentRecord.ToString("00000000") + 
                "\nLife:".PadRight(10) + Stats.LifeCount + 
                "\nBomb:".PadRight(10) + Stats.BombCount +
                 "\nGraze:".PadRight(10) + Stats.GrazeCount;
            Graphics.Draw(statsRenderer.Texture, new Vector2(250, 0), statsRenderer.Size, 0, 0);
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
            isPaused = true;
            windowScale = MathF.Min((float)window.Width / defaultWidth, (float)window.Height / defaultHeight);
            windowXOffset = (int)(window.Width - defaultWidth * windowScale) / 2;
            windowYOffset = (int)(window.Height - defaultHeight * windowScale) / 2;
            worldFrameBuffer.Resize((int)(World.Size.X * windowScale), (int)(World.Size.Y * windowScale));
        }

        private static void Window_FocusedChanged(object sender, EventArgs e)
        {
            isPaused = true;
        }

        private static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
