using CourseWork3.Game;
using OpenTK;

namespace CourseWork3
{
    class Program
    {
        static void Main(string[] args)
        {
            using (GameWindow window = new GameWindow(800, 600))
            {
                GameMain.Init(window, args);
            }
        }
    }
}
