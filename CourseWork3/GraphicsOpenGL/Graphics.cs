using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;

namespace CourseWork3.GraphicsOpenGL
{

    partial class Graphics
    {
        public int quadVBO;
        private void LoadQuadVBO()
        {
            float[] quadAttrib = new float[]
            {
                //aPos        aTex
                -0.5f, -0.5f, 0.0f, 0.0f,
                +0.5f, -0.5f, 1.0f, 0.0f,
                +0.5f, +0.5f, 1.0f, 1.0f,
                -0.5f, +0.5f, 0.0f, 1.0f,
            };

            quadVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.VertexPointer(2, VertexPointerType.Float, 4 * sizeof(float), 0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
            GL.BufferData(BufferTarget.ArrayBuffer, quadAttrib.Length * sizeof(float),
                quadAttrib, BufferUsageHint.StaticDraw);

            //int vao = GL.GenVertexArray();
            //GL.BindVertexArray(vao);
            //GL.EnableVertexAttribArray(0);
            //GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(1);
            //GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 2 * sizeof(float));

            //GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.DeleteVertexArray(vao);
        }

        protected static Graphics instance;
        public static Graphics Instance
        {
            get
            {
                if (instance == null)
                    instance = new Graphics();
                return instance;
            }
        }
        private Graphics()
        {

        }

        public void Init()
        {
            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0.5f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            LoadQuadVBO();
        }

        public void ApplyViewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }
        public void ApplyViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }

        public void ApplyProjection(int width, int height)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-width / 2.0, width / 2.0, height / 2.0, -height / 2.0, 0.0, 1.0);
        }

        public void ApplyViewTransofrm(Vector2 position, Vector2 scale, float rotation)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 transform = Matrix4.Identity;
            transform *= Matrix4.CreateTranslation(-position.X, +position.Y, 0f);
            transform *= Matrix4.CreateRotationZ(-rotation);
            transform *= Matrix4.CreateScale(scale.X, scale.Y, 1.0f);
            GL.LoadMatrix(ref transform);
        }

        public void ApplyViewTransofrm(Vector2 position, float scale, float rotation)
        {
            ApplyViewTransofrm(position, new Vector2(scale), rotation);
        }

        public void BeginDraw(Color color)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
        }

        public void Draw(Texture2D texture, Vector2 position, Vector2 scale, float rotation, byte depth = 0)
        {
            GL.PushMatrix();
            texture.Bind();

            GL.Translate(position.X, -position.Y, depth / -255f);
            if (scale != Vector2.Zero) GL.Scale(scale.X, scale.Y, 1);
            if (rotation != 0) GL.Rotate(rotation, 0, 0, -1);

            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.PopMatrix();
        }
        public void Draw(Texture2D texture, Vector2 position, Vector2 scale, float rotation, Color color, byte depth = 0)
        {
            GL.PushMatrix();
            texture.Bind();

            GL.Translate(position.X, -position.Y, (depth + 0.5) / -255f);
            if (scale != Vector2.Zero) GL.Scale(scale.X, scale.Y, 1);
            if (rotation != 0) GL.Rotate(rotation, 0, 0, -1);

            GL.BlendColor(color);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.Translate(0, 0, 0.5f / 255f);
            GL.BlendFunc(BlendingFactor.ConstantColor, BlendingFactor.SrcColor);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);

            GL.PopMatrix();
        }

        ~Graphics()
        {
            var textures = Texture2D.AllCreatedTexturesId.ToArray();
            GL.DeleteTextures(textures.Length, textures);

            GL.DeleteBuffer(quadVBO);
        }
    }
}
