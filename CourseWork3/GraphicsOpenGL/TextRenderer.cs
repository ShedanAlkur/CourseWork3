using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CourseWork3.GraphicsOpenGL
{
    class TextRenderer
    {
        public static Font DefaultFont = new Font("Microsoft Sans Serif", 15f, FontStyle.Regular, GraphicsUnit.Point, (byte)204);

        Bitmap bmp;
        System.Drawing.Graphics gfx;
        Rectangle rectGFX;
        Point point;

        public Vector2 Size { get; private set; }
        Color bgColor;
        Font font;
        SolidBrush brush;

        public Texture2D Texture { get; private set; }

        string text;
        public string Text { get => text; set => SetText(value); }

        public TextRenderer(int width, int height, Color bgColor, Color textColor, Font font)
        {
            Size = new Vector2(width, height);
            Texture = new Texture2D(width, height);

            bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gfx = System.Drawing.Graphics.FromImage(bmp);
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            this.bgColor = bgColor;
            this.font = font;
            this.brush = new SolidBrush(textColor);
            this.point = new Point(0, 0);
            rectGFX = new Rectangle(0, 0, bmp.Width, bmp.Height);
            text = string.Empty;
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
            Texture.Resize(width, height);
        }

        private void SetText(string text)
        {
            if (text == this.text) return;
            this.text = text;

            gfx.Clear(bgColor);
            gfx.DrawString(text, font, brush, point);

            UploadBitmap();
        }

        protected void UploadBitmap()
        {
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(rectGFX,
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Texture.Bind();
            GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                rectGFX.X, rectGFX.Y, rectGFX.Width, rectGFX.Height,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            // Освобождаем память, занимаемую data
            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            Texture2D.Unbind();
        }
    }
}
