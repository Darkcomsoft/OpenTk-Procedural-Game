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
        private Gwen.Input.OpenTK input;
        private Gwen.Renderer.OpenTK renderer;
        private Gwen.Skin.Base skin;
        private Gwen.Control.Canvas canvas;
        private Gwen.UnitTest.UnitTest test;

        private Matrix4 _projectionMatrix;

        public Gwen.Control.Canvas GetCanvas { get { return canvas; } }

        public GUI()
        {
            Game.GUI = this;

            SplashScreen.SetState("Loading UI.Render", SplashScreenStatus.Loading);
            renderer = new Gwen.Renderer.OpenTK();
            SplashScreen.SetState("Loading UI.Skin's", SplashScreenStatus.Loading);
            skin = new Gwen.Skin.TexturedBase(renderer, "Assets/UI/DefaultSkin.png");

            SplashScreen.SetState("Loading UI.Fonts", SplashScreenStatus.Loading);
            skin.DefaultFont = new Gwen.Font(renderer, "Arial", 10);
            SplashScreen.SetState("Starting UI.Canvas", SplashScreenStatus.Loading);
            canvas = new Gwen.Control.Canvas(skin);

            SplashScreen.SetState("Starting UI.Input", SplashScreenStatus.Loading);
            input = new Gwen.Input.OpenTK(Window.Instance);
            input.Initialize(canvas);

            SplashScreen.SetState("Setting-UP UI", SplashScreenStatus.Loading);
            canvas.SetSize(Window.Instance.Width, Window.Instance.Height);
            canvas.ShouldDrawBackground = false;
            canvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);

            //test = new Gwen.UnitTest.UnitTest(canvas);
            SplashScreen.SetState("Finishe Loading UI!", SplashScreenStatus.Loading);
        }

        public void OnResize()
        {
            _projectionMatrix = Matrix4.CreateTranslation(new Vector3(-Window.Instance.Width / 2.0f, -Window.Instance.Height / 2.0f, 0)) * Matrix4.CreateScale(new Vector3(1, -1, 1)) * Matrix4.CreateOrthographic(Window.Instance.Width, Window.Instance.Height, -1.0f, 1.0f);

            renderer.Resize(ref _projectionMatrix, Window.Instance.Width, Window.Instance.Height);
            canvas.SetSize(Window.Instance.Width, Window.Instance.Height);
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
                /*test.Note = String.Format("L1: {0} L2: {3} Invalidates: {4} Duplicate Invalidates: {5} Draw Calls: {1} Vertex Count: {2}", renderer.LevelOneCacheSize, renderer.DrawCallCount,
                    renderer.VertexCount, renderer.LevelTwoCacheSize, canvas.InvalidatesThisFrame, canvas.DuplicateInvalidates);
                test.Fps = Window.Instance.GetFPS;*/

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
