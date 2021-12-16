using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CourseWork3.GraphicsOpenGL
{
    class FrameBuffer : IDisposable
    {
        public Texture2D Texture;
        public int FramebufferID;
        public int RenderbufferID;

        int width;
        int height;

        public FrameBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;

            this.Texture = new Texture2D(width, height);
            FramebufferID = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferID);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, Texture.ID, 0);

            RenderbufferID = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RenderbufferID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8,
                width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, RenderbufferID);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public void Resize(int width, int height)
        {
            if (this.width == width && this.height == height) return;
            this.width = width;
            this.height = height;

            Texture.Resize(width, height);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RenderbufferID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8,
                width, height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferID);
        }
        public static void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                Texture.Dispose();
                GL.DeleteFramebuffer(FramebufferID);
                GL.DeleteRenderbuffer(RenderbufferID);
                Console.WriteLine($"Буферы {FramebufferID} и {RenderbufferID} выгружены");
                disposed = true;
            }
        }
        ~FrameBuffer()
        {
            Dispose();
        }
    }
}
