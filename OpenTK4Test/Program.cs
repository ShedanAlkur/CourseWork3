using OpenTK;
using System;

namespace OpenTK4Test
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    var window = new GameWindow(
        //        GameWindowSettings.Default,
        //        new NativeWindowSettings
        //        {
        //            Flags = OpenTK.Windowing.Common.ContextFlags.Default,
        //            Size = new OpenTK.Mathematics.Vector2i(800, 600),
        //        });
        //    var game = new Game(window);

        //    window.Run();
        //}

        static void Main(string[] args)
        {
            var window = new GameWindow(800, 600);
            var game = new Game(window);

            window.Run();
        }
    }
}
