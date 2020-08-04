using EvllyEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwen.Control;
using Gwen.Control.Layout;
using ProjectEvlly.src.Utility;
using OpenTK.Input;

namespace ProjectEvlly.src.UI
{
    public class GUI
    {
        private int _Width, _Height;

        private Gwen.Input.OpenTK input;
        private Gwen.Renderer.OpenTK renderer;
        private Gwen.Skin.Base skin;
        private Gwen.Control.Canvas canvas;
        private Gwen.UnitTest.UnitTest test;

        private Matrix4 _projectionMatrix;

        public GUI(int Width, int Height)
        {
            Width = _Width;
            Height = _Height;

            renderer = new Gwen.Renderer.OpenTK();
            skin = new Gwen.Skin.TexturedBase(renderer, "Assets/UI/DefaultSkin.png");

            skin.DefaultFont = new Gwen.Font(renderer, "Arial", 10);
            canvas = new Gwen.Control.Canvas(skin);

            input = new Gwen.Input.OpenTK(Window.Instance);
            input.Initialize(canvas);

            canvas.SetSize(Width, Height);
            canvas.ShouldDrawBackground = false;
            canvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);

            test = new Gwen.UnitTest.UnitTest(canvas);
        }

        public void OnResize()
        {
            _projectionMatrix = Matrix4.CreateTranslation(new Vector3(-_Width / 2.0f, -_Height / 2.0f, 0)) * Matrix4.CreateScale(new Vector3(1, -1, 1)) * Matrix4.CreateOrthographic(_Width, _Height, -1.0f, 1.0f);

            renderer.Resize(ref _projectionMatrix, _Width, _Height);
            canvas.SetSize(_Width, _Height);
        }

        public void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (!EvllyEngine.MouseCursor.MouseLocked)
                input.ProcessKeyDown(e);
        }

        public void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (!EvllyEngine.MouseCursor.MouseLocked)
                input.ProcessKeyUp(e);
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!EvllyEngine.MouseCursor.MouseLocked)
                input.ProcessMouseMessage(e);
        }
        public void OnMouseUp(MouseButtonEventArgs e)
        {
            if (!EvllyEngine.MouseCursor.MouseLocked)
                input.ProcessMouseMessage(e);
        }
        public void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!EvllyEngine.MouseCursor.MouseLocked)
                input.ProcessMouseMessage(e);
        }
        public void OnMouseMove(MouseMoveEventArgs e)
        {
            if (!EvllyEngine.MouseCursor.MouseLocked)
                input.ProcessMouseMessage(e);
        }

        public void Tick()
        {
            renderer.Update(Time._Time);
        }

        public void DrawUI()
        {
            if ((Time._Tick % 60) == 0)
            {
                test.Note = String.Format("L1: {0} L2: {3} Invalidates: {4} Duplicate Invalidates: {5} Draw Calls: {1} Vertex Count: {2}", renderer.LevelOneCacheSize, renderer.DrawCallCount,
                    renderer.VertexCount, renderer.LevelTwoCacheSize, canvas.InvalidatesThisFrame, canvas.DuplicateInvalidates);
                test.Fps = Window.Instance.GetFPS;

                if (renderer.TextCacheSize > 1000)
                { // each cached string is an allocated texture, flush the cache once in a while in your real project
                    renderer.FlushTextCache();
                }
            }

            canvas.RenderCanvas();
            Utilitys.CheckGLError("End Of Canvas Draw");
        }

        public void Dispose()
        {
            canvas.Dispose();
            skin.Dispose();
            renderer.Dispose();
        }
    }
}
