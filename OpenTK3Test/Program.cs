using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK3Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new GameWindow(800, 600);
            var game = new Game(window);

            window.Run();
        }
    }
}
