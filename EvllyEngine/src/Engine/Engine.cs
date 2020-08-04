using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Threading;
using ProjectEvlly;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.UI;
using ProjectEvlly.src.Utility;
using System.Drawing;
using ProjectEvlly.src;

namespace EvllyEngine
{
    public class Window : GameWindow
    {
        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) {
            Instance = this; 
        }

        public static Window Instance;

        public Game OGame;

        private AssetsManager _assetsManager;
        private Physics _physics;
        private SplashScreen _SplashScreen;
        private Network _Network;
        private GUI _GUI;

        private Sky _Sky;

        private int FPS;
        private int UPS;

        protected override void OnLoad(EventArgs e)
        {
            //Load the Window/pre Graphics Stuff//
            VSync = VSyncMode.Off;
            WindowBorder = WindowBorder.Resizable;

            gl.ClearColor(Color4.DeepSkyBlue);

            //Load the Engine Stuff//
            _SplashScreen = new SplashScreen();

            Debug.Log("Start Loading Game...");
            _assetsManager = new AssetsManager();
            _assetsManager.LoadAssets();//Load All Assets

            _physics = new Physics();
            _Network = new Network();
            _GUI = new GUI(Width, Height);


            //_Sky = new Sky(AssetsManager.GetShader("Default"), AssetsManager.GetTexture("devTexture2"));

            Debug.Log("Finished Loading Game!");
            base.OnLoad(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            try
            {
                UPS = (int)(1f / e.Time);

                _GUI.Tick();

                if (Input.GetKeyDown(Key.F11))
                {
                    if (WindowState == WindowState.Fullscreen)
                    {
                        WindowState = WindowState.Normal;
                    }
                    else
                    {
                        WindowState = WindowState.Fullscreen;
                    }
                }

                if (Input.GetKeyDown(Key.Escape))
                {
                    Exit();
                }

                if (Focused) // check to see if the window is focused  
                {
                    MouseCursor.CursorLockPosition();
                }

                _physics.UpdatePhisics((float)e.Time);

                Time._Time = (float)e.Time;
                Time._Tick++;

                if (Time._Tick > 10000)
                {
                    GCollector.Collect();
                    Time._Tick = 0;
                }
            }
            catch (OutOfMemoryException memoryEx)
            {
                Debug.LogWarning("GC: " + memoryEx.Message);
                GC.Collect();
                return;
            }
            catch (Exception ex)
            {
                Debug.LogError("Crash: " + ex.Message);
            }
            base.OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            FPS = (int)(1f / e.Time);

            gl.Clear();//Clear Buffer
            gl.Enable(EnableCap.DepthTest);
                OGame.Draw(e);
            gl.Disable(EnableCap.DepthTest);
                _GUI.DrawUI();
            SwapBuffers();
            Utilitys.CheckGLError("Final Of Draw Frame");
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            gl.Viewport(0, 0, Width, Height);

            _GUI.OnResize();
            base.OnResize(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            _GUI.OnKeyDown(e);
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            _GUI.OnKeyUp(e);
            base.OnKeyUp(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _GUI.OnMouseDown(e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            _GUI.OnMouseUp(e);
            base.OnMouseUp(e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            Input.MouseWheelDelta = e.Delta;
            _GUI.OnMouseWheel(e);
            base.OnMouseWheel(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Input.GetMouse = e;
            _GUI.OnMouseMove(e);
            base.OnMouseMove(e);
        }

        private void ApplyFog()
        {
            gl.Enable(EnableCap.Fog);

            // Fog
            float[] colors = { 230, 230, 230 };
            GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
            GL.Hint(HintTarget.FogHint, HintMode.DontCare);
            GL.Fog(FogParameter.FogColor, colors);

            GL.Fog(FogParameter.FogStart, (float)1000 / 100.0f);
            GL.Fog(FogParameter.FogEnd, 250.0f);
        }

        protected override void OnUnload(EventArgs e)
        {
            //_Sky.OnDestroy();

            OGame.Dispose();

            _assetsManager.UnloadAll();
            _physics.Dispose();

            _GUI.Dispose();
            base.OnUnload(e);
        }

        public int GetFPS { get { return FPS; } }
        public int GetUPS { get { return UPS; } }
    }

    public static class Time
    {
        public static float _Time;
        public static float _Tick;
    }

    public static class MouseCursor
    {
        private static bool _isLocked;

        public static bool MouseLocked { get { return _isLocked; } }

        public static void LockCursor()
        {
            Window.Instance.CursorVisible = false;
            _isLocked = true;
        }

        public static void UnLockCursor()
        {
            _isLocked = false;
            Window.Instance.CursorVisible = true;
        }

        public static void CursorLockPosition()
        {
            if (_isLocked)
            {
                Mouse.SetPosition(Window.Instance.X + Window.Instance.Width / 2f, Window.Instance.Y + Window.Instance.Height / 2f);
            }
        }
    }
}

public enum CullType : byte
{
    Front, Back, Both
}