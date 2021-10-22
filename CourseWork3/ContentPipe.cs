using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CourseWork3.GraphicsOpenGL;

namespace CourseWork3
{
    class ContentPipe
    {
        string rootFolder = "";
        public string RootFolder { get => rootFolder; set => rootFolder = value; }


        public void LoadTexture2D(string path)
        {
            if (!File.Exists(rootFolder + path))
            {
                throw new FileNotFoundException("File not found at '" + rootFolder + path + "'");
            }

        }
    }
}
