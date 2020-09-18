using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using EvllyEngine;
using System.Drawing;
using OpenTK.Input;
using ProjectEvlly.src.Utility;

namespace ProjectEvlly.src.UI
{
    public class GUIRender : IDisposable
    {
        private int VAO, IBO, vbo;
        private RectangleMesh RecMesh;
        private Shader _Shader;
        private Texture _testTexture;
        private List<GUIBase> GuiBaseList;

        public int GUISize = 1;

        public GUIRender()
        {
            GuiBaseList = new List<GUIBase>();
            RecMesh = new RectangleMesh();

            _Shader = AssetsManager.GetShader("GUI");
            _testTexture = AssetsManager.GetTexture("Darkcomsoft");

            VAO = GL.GenVertexArray();
            IBO = GL.GenBuffer();
            vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, RecMesh._indices.Length * sizeof(int), RecMesh._indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, RecMesh._vertices.Length * Vector2.SizeInBytes, RecMesh._vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            /*for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    GuiBaseList.Add(new GUIImage(new UIRectangle(x * 100, y * 100, 50, 50)));
                }
            }*/

            GuiBaseList.Add(new GUIImage(new Rectangle(0, 25, 0, 50), UIDock.SizeBottom, null, Color4.Indigo));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 25, 0, 50), UIDock.SizeTop, null, Color4.Red));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 0, 50, 0), UIDock.SizeLeft, null, Color4.Blue));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 0, 50, 0), UIDock.SizeRight, null, Color4.Gold));
            GuiBaseList.Add(new GUITextInput(new Rectangle(0, 25, 0, 50), UIDock.SizeBottom));

            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.Cennter));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 25, 50, 50), UIDock.CenterTop));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 25, 50, 50), UIDock.CenterBottom));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 25, 50, 50), UIDock.CenterLeft));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 25, 50, 50), UIDock.CenterRight));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 25, 50, 50), UIDock.TopLeft));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 25, 50, 50), UIDock.TopRight));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 25, 50, 50), UIDock.BottomLeft));
            GuiBaseList.Add(new GUIImage(new Rectangle(25, 25, 50, 50), UIDock.BottomRight));
        }

        public void Tick()
        {
            for (int i = 0; i < GuiBaseList.Count; i++)
            {
                if (GuiBaseList[i].IsEnabled)
                {
                    GuiBaseList[i].Tick();
                }
            }
        }

        public void TickRender()
        {
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);

            for (int i = 0; i < GuiBaseList.Count; i++)
            {
                if (GuiBaseList[i].IsEnabled)
                {
                    _Shader.Use();

                    if (GuiBaseList[i].GetType() == typeof(GUIImage))
                    {
                        GUIImage img = (GUIImage)GuiBaseList[i];

                        if (img != null)
                        {
                            if (img.ImageRaw != null)
                            {
                                img.ImageRaw.Use();
                                _Shader.SetVector4("MainColor", img.Color);
                                _Shader.Setbool("HaveTexture", true);
                            }
                            else
                            {
                                _Shader.SetVector4("MainColor", img.Color);
                                _Shader.Setbool("HaveTexture", false);
                            }
                        }
                        else
                        {
                            _Shader.SetVector4("MainColor", Vector4.Zero);
                            _Shader.Setbool("HaveTexture", false);
                        }
                    }
                    else if (GuiBaseList[i].GetType() == typeof(GUITextInput))
                    {
                        GUITextInput textInput = (GUITextInput)GuiBaseList[i];

                        _Shader.SetVector4("MainColor", Vector4.Zero);
                        _Shader.Setbool("HaveTexture", false);
                    }

                    _Shader.SetMatrix4("world", GetGUITransformMatrix(GuiBaseList[i].GetRectangle));
                    _Shader.SetMatrix4("projection", Matrix4.CreateOrthographic(Window.Instance.Width, Window.Instance.Height, 0f, 5.0f));
                    //GL.DrawArrays(PrimitiveType.TriangleStrip, 0, RecMesh.Vertices_Positions.Length);
                    GL.DrawElements(BeginMode.Triangles, RecMesh._indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        public void Dispose()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(0);

            GL.DeleteBuffer(IBO);
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(VAO);

            RecMesh.Dispose();
        }

        public void AddGuiElement(GUIBase baseGui)
        {
            if (baseGui.GetType() == typeof(GUIImage))
            {

            }
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
            Vector2 pos = Utilitys.ScreenToWorldCoords(e.X, e.Y, Window.Instance.Width, Window.Instance.Height);

            var mouseX = e.X - Window.Instance.X;
            var mouseY = e.Y - Window.Instance.Y - 38;

            Point point = new Point((int)mouseX, (int)mouseY);

            //point.Offset(Window.Instance.Width, Window.Instance.Height);
            if (In(GuiBaseList[0]._Rectangle, point))
            {
                Debug.Log("Mouse em cima do Bottom!");
            }
        }
        public bool In(Rectangle rec, Point Point)
        {
            return Point.X >= rec.Location.X && rec.Y >= rec.Location.Y &&
                Point.X < rec.Location.X + rec.Size.Width && Point.Y < rec.Location.Y + rec.Size.Height;
        }


        private Matrix4 GetGUITransformMatrix(Rectangle rec)
        {
            return Matrix4.CreateScale(rec.Width * GUISize, rec.Height * GUISize, 1) * Matrix4.CreateRotationX(0) * Matrix4.CreateRotationY(0) * Matrix4.CreateRotationZ(0) * Matrix4.CreateTranslation(rec.X / Window.Instance.Width * GUISize, rec.Y / Window.Instance.Height * GUISize, 1);
        }
    }

    public enum UIDock : byte
    {
        Free,
        Cennter, CenterTop, CenterBottom, CenterLeft, CenterRight,
        TopLeft, TopRight,
        BottomLeft, BottomRight,

        SizeBottom, SizeTop, SizeLeft, SizeRight//this is for dock position, but resize the ui element
    }

    public struct UIRectangle
    {
        public float x;
        public float y;
        public float w;
        public float h;

        public UIRectangle(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }
}
