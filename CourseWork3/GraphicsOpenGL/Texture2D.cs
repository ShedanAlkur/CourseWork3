using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace CourseWork3.GraphicsOpenGL
{
    class Texture2D : IDisposable
    {
        public int ID { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public System.Drawing.Rectangle Bounds
        {
            get
            {
                return new System.Drawing.Rectangle(0, 0, this.Width, this.Height);
            }
        }

        public Texture2D(Stream stream)
        {

            Bitmap bmp = new Bitmap(stream);
            this.Width = bmp.Width;
            this.Height = bmp.Height;
            this.ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, this.ID);
            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.Clamp);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);


            Texture2D.Unbind();
        }

        public Texture2D(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, this.ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                width, height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, new byte[0]);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            Texture2D.Unbind();
        }

        public Texture2D(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Bitmap bmp = new Bitmap(stream);
                this.Width = bmp.Width;
                this.Height = bmp.Height;
                this.ID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, this.ID);
                BitmapData data = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                    (int)TextureWrapMode.Clamp);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                    (int)TextureWrapMode.Clamp);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);

                Texture2D.Unbind();
            }
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public static void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                GL.DeleteTexture(ID);
                disposed = true;
            }
        }
        ~Texture2D()
        {
            Dispose();
        }
    }
}
