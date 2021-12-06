using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CourseWork3.Game;
using CourseWork3.GraphicsOpenGL;
using CourseWork3.Patterns;
using CourseWork3.Game;
using OpenTK;
using ExpressionBuilder;
using CourseWork3.Parser;

namespace CourseWork3
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSite.ParserTest();
            //TestSite.MyActionBuilderTest();
            //TestSite.RandomDelegateTest();
            return;

            using (GameWindow window = new GameWindow(800, 600))
            {
                GameMain.Init(window);
            }
        }



    }
}
