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
        private GUI _GUI;

        private Sky _Sky;

        private int FPS;
        private int UPS;

        private bool GameLoaded = false;
        private bool EngineIsReady = false;
        private int crashTimeOut = 0;

        protected override void OnLoad(EventArgs e)//Load the Window, but not the systems, importantem only the splash screen
        {
            gl.ClearColor(Color4.DeepSkyBlue);

            VSync = VSyncMode.Off;
            WindowBorder = WindowBorder.Resizable;

            _SplashScreen = new SplashScreen();
            base.OnLoad(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            try
            {
                if (EngineIsReady)
                {
                    UPS = (int)(1f / e.Time);

                    _GUI.Tick();
                    OGame.Tick();

                    if (Focused) // check to see if the window is focused  
                    {
                        MouseCursor.CursorLockPosition();
                    }

                    _physics.UpdatePhisics((float)e.Time);
                }
                else if (!GameLoaded)//first load the window, before load the systems
                {
                    LoadGame();
                }

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
                if (crashTimeOut >= 10)
                {
                    Debug.Log("We cant start again the game, before the crash! Shutingdown....");
                    Exit();
                }

                crashTimeOut++;
                ReloadGame();
                Debug.Log("Crashed Trying Yo Start Back!");

                throw ex;
            }
            base.OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            FPS = (int)(1f / e.Time);

            gl.Clear();//Clear Buffer
            Utilitys.CheckGLError("GL.Clear");

            if (EngineIsReady)
            {
                gl.Enable(EnableCap.DepthTest);
                OGame.Draw(e);
                gl.Disable(EnableCap.DepthTest);
                _GUI.DrawUI();
                Utilitys.CheckGLError("Final Of Game Draw Frame");
            }
            else
            {

            }

            SwapBuffers();
            Utilitys.CheckGLError("Window SwapBuffers");
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            gl.Viewport(0, 0, Width, Height);

            if (EngineIsReady)
                _GUI.OnResize();
            base.OnResize(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (EngineIsReady)
                _GUI.OnKeyDown(e);
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (EngineIsReady)
                _GUI.OnKeyUp(e);
            base.OnKeyUp(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (EngineIsReady)
                _GUI.OnMouseDown(e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (EngineIsReady)
                _GUI.OnMouseUp(e);
            base.OnMouseUp(e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (EngineIsReady)
            {
                Input.MouseWheelDelta = e.Delta;
                _GUI.OnMouseWheel(e);
            }
            base.OnMouseWheel(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (EngineIsReady)
            {
                Input.GetMouse = e;
                _GUI.OnMouseMove(e);
            }
            base.OnMouseMove(e);
        }

        private void LoadGame()
        {
            GameLoaded = true;//let this on the top

            SplashScreen.SetState("Loading Assets...", SplashScreenStatus.Loading);
            _assetsManager = new AssetsManager();
            _assetsManager.LoadAssets();//Load All Assets

            SplashScreen.SetState("Starting Physics", SplashScreenStatus.Loading);
            _physics = new Physics();
            _GUI = new GUI();
            SplashScreen.SetState("Starting Game Scripts", SplashScreenStatus.Loading);
            //_Sky = new Sky(AssetsManager.GetShader("Default"), AssetsManager.GetTexture("devTexture2"));

            OGame = new Game();
            _GUI.OnResize();//Resize the GUI
            SplashScreen.SetState("Setting Up OpenGL", SplashScreenStatus.Loading);
            EngineIsReady = true;

            SplashScreen.SetState("Finished Loading.", SplashScreenStatus.finished);
        }

        /// <summary>
        /// This is gona disconnect/quit you from any world or server etc. and unload assets, and other things, this is for reload the game, like the firts time
        /// </summary>
        public void ReloadGame()
        {
            OGame.Dispose();
            _physics.Dispose();
            _GUI.Dispose();

            _assetsManager.UnloadAll();

            _assetsManager = null;
            _GUI = null;
            _physics = null;
            OGame = null;

            GameLoaded = false;
            EngineIsReady = false;
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
            if (EngineIsReady)
            {
                //_Sky.OnDestroy();

                OGame.Dispose();
                _assetsManager.UnloadAll();
                _physics.Dispose();

                _GUI.Dispose();
            }
            else
            {
                Debug.Log("Window is finished, but the game isnt, somthing is not right (:");
            }

            _SplashScreen.Dispose();
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