using EvllyEngine;
using QuickFont;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using ProjectEvlly.src.Utility;
using System.Drawing;

namespace ProjectEvlly.src.UI
{
    public abstract class GUIBase
    {
        private Mesh _mesh;
        private Shader _Shader;
        private Texture _texture;
        private int IBO, VAO, vbo, dbo, tbo;

        private QFont _font;
        private QFontDrawing _drawing;
        private QFontRenderOptions RendeTextOption;

        private GuiElementType _guiElementType;

        private bool IsEnabled = true;
        private bool UseTexture = false;
        private bool UseText = false;

        private string Text = "";

        public GUIBase(GuiElementType guiElementType)
        {
            _guiElementType = guiElementType;

            _Shader = new Shader(AssetsManager.LoadShader("Assets/Shaders/", "UI"));

            if (UseTexture)
            {
                _texture = new Texture(AssetsManager.LoadImage("Assets/Images/", "Darkcomsoft", "png"), TextureMinFilter.Linear, TextureMagFilter.Linear);
            }

            CreateMesh();

            IBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

            vbo = GL.GenBuffer();
            dbo = GL.GenBuffer();
            tbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _mesh._indices.Length * sizeof(int), _mesh._indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _mesh._vertices.Length * Vector3.SizeInBytes, _mesh._vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            //Colors
            GL.BindBuffer(BufferTarget.ArrayBuffer, dbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _mesh._Colors.Length * sizeof(float), _mesh._Colors, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            //Texture
            GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _mesh._texCoords.Length * Vector2.SizeInBytes, _mesh._texCoords, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(2);
        }

        public virtual void Enable()
        {
            IsEnabled = true;
        }

        public virtual void Disable()
        {
            IsEnabled = false;
        }

        public void SetText(string text)
        {
            Text = text;
        }

        public void SetImage(Texture texture)
        {

        }

        public void RemoveTexture()
        {

        }

        public void Draw()
        {
            if (IsEnabled)
            {
                gl.ClearColor(Color.Black);

                gl.Clear();
                gl.Disable(EnableCap.DepthTest);

                if (UseText)
                {
                    _drawing.DrawingPrimitives.Clear();
                    _drawing.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(Window.Instance.ClientRectangle.X, Window.Instance.ClientRectangle.Width, Window.Instance.ClientRectangle.Y, Window.Instance.ClientRectangle.Height, 0f, 5.0f);
                    _drawing.Print(_font, Text, new Vector3(Window.Instance.Width / 2, Window.Instance.ClientRectangle.Top + 40, 0), new SizeF(Window.Instance.ClientRectangle.Width, Window.Instance.ClientRectangle.Height), QFontAlignment.Centre, RendeTextOption);
                    _drawing.RefreshBuffers();
                    _drawing.Draw();
                }

                //Draw Image Logo
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                gl.Enable(EnableCap.Blend);

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);

                if (UseTexture)
                {
                    if (_texture != null)
                    {
                        _texture.Use();
                    }
                }

                if (_Shader != null)
                {
                    _Shader.Use();
                }

                GL.DrawElements(BeginMode.Triangles, _mesh._indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);

                gl.Disable(EnableCap.Blend);
                gl.Enable(EnableCap.DepthTest);

                gl.ClearColor(Color.DeepSkyBlue);
                Window.Instance.SwapBuffers();
            }
        }

        public void Destroy()
        {
            if (_font != null)
            {
                _font.Dispose();
            }

            if (_drawing != null)
            {
                _drawing.Dispose();
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(1);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(2);

            GL.DeleteBuffer(IBO);

            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(dbo);
            GL.DeleteBuffer(tbo);

            GL.DeleteVertexArray(VAO);
        }

        private void CreateMesh()
        {
            _mesh = new Mesh();

            _mesh._vertices = new Vector3[]
            {
                 //Position          Texture coordinates
                 new Vector3(0.5f,  0.6f, 0.0f), // top right
                new Vector3( 0.5f, -0.6f, 0.0f), // bottom right
               new Vector3( -0.5f, -0.6f, 0.0f), // bottom left
                new Vector3(-0.5f,  0.6f, 0.0f) // top left
            };

            _mesh._indices = new int[]
            {
                0, 1, 3,   // first triangle
                1, 2, 3    // second triangle
            };

            _mesh._Colors = new Color4[] {
                new Color4(1.0f,1.0f,1.0f,1.0f),
                new Color4(1.0f,1.0f,1.0f,1.0f),
               new Color4(1.0f,1.0f,1.0f,1.0f)
            };

            _mesh._texCoords = new Vector2[] {
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f)
            };
        }
    }
}
