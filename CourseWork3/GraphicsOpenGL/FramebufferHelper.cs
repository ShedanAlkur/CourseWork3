using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace CourseWork3.GraphicsOpenGL
{
    class FramebufferHelper
    {
        public static int GenFramebuffer()
        {
            return GL.GenFramebuffer();
        }

        public static void BindFramebuffer(int frameBuffer)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
        }

        public static void UnbindFramebuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public static void DeleteFramebuffer(int frameBuffer)
        {
            GL.DeleteFramebuffer(frameBuffer);
        }

        public static int GenRenderbuffer()
        {
            return GL.GenRenderbuffer();
        }

        public static void BindRenderbuffer(int renderBuffer)
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBuffer);
        }

        public static void UnbindRenderbuffer()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public static void DeleteRenderbuffer(int renderBuffer)
        {
            GL.DeleteRenderbuffer(renderBuffer);
        }

        public static void FramebufferTexture2D(int texture)
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, texture, 0);
        }

        public static void SettingRenderbuffer(int width, int height)
        {
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8,
               width, height);
        }

        public static void FramebufferRenderbuffer(int renderBuffer)
        {
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderBuffer);
        }

    }
}
