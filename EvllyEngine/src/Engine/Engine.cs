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
using ProjectEvlly.src.Utility;
using ProjectEvlly.src.Net;

namespace EvllyEngine
{
    public class Engine : GameWindow
    {
        public Engine(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { Instance = this; }

        public static Engine Instance;
        private AssetsManager _assetsManager;
        private UIManager _UIManager;
        private Physics _physics;
        private SplashScreen _SplashScreen;
        private Network _Network;

        public Action<FrameEventArgs> DrawUpdate;
        public Action<FrameEventArgs> TransparentDrawUpdate;

        private int FPS;
        private int UPS;
        public bool EngineReady;

        private Dictionary<string, Dimension> _Dimensions;
        private Queue<Dimension> _ToUnloadDimension;

        public List<GameObject> GameObjects = new List<GameObject>();

        protected override void OnLoad(EventArgs e)
        {
            _SplashScreen = new SplashScreen();

            //Load the Window/pre Graphics Stuff//
            VSync = VSyncMode.Off;
            WindowBorder = WindowBorder.Resizable;

            GL.ClearColor(Color4.DeepSkyBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Front);
            GL.Viewport(0, 0, Width, Height);

            //Load the Engine Stuff//
            Debug.Log("Start Loading Game...");
            _assetsManager = new AssetsManager();
            _UIManager = new UIManager();
            _physics = new Physics();
            _Network = new Network();

            _Dimensions = new Dictionary<string, Dimension>();
            _ToUnloadDimension = new Queue<Dimension>();

            Debug.Log("Finished Loading Game!");

            _Dimensions.Add("VAK:MAINMENU", new MainMenuDimension());

            SceneManager.LoadDontDestroyScene();
            SceneManager.LoadDefaultScene();
            
            GameObject Player = GameObject.Instantiate("Player", 1);
            Player._transform.Position = new Vector3(0, 50, 0);
            Player.AddRigidBody();
            Player.AddScript(new PlayerEntity());

            GameObject camobj = GameObject.Instantiate("Camera", 1);
            camobj._transform.Position = new Vector3(0,1,0);
            camobj.AddCamera();

            camobj._transform.SetChild(Player._transform);

            GameObject World = GameObject.Instantiate(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 1);
            World.AddScript(new World());

            GameObject obj2 = GameObject.Instantiate(new Vector3(0, 20, 0), new Quaternion(-90,0,0,0), 1);
            MeshRender mesh2 = obj2.AddMeshRender(new MeshRender(obj2, _assetsManager.GetErrorMesh, new Shader(AssetsManager.instance.GetShader("Default"))));
            mesh2._shader.AddTexture(new Texture(AssetsManager.instance.GetTexture("RedTexture", "png")));

            EngineReady = true;
            base.OnLoad(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            try
            {
                UPS = (int)(1f / e.Time);

                while (_ToUnloadDimension.Count > 0)
                {
                    Dimension dime = _ToUnloadDimension.Dequeue();
                    dime.OnUnloadDimension();

                    _Dimensions[dime._DimensionID] = null;
                    _Dimensions.Remove(dime._DimensionID);
                }

                if (Input.GetKey(Key.C))
                {
                    GameObject monkeyRigid = GameObject.Instantiate(Camera.Main.gameObject._transform.Position, new Quaternion(0, 0, 0, 0), 1);
                    ///monkeyRigid.AddScript(new ScriptTest());
                    MeshRender meshLaucherPed = monkeyRigid.AddMeshRender(new MeshRender(monkeyRigid, new Mesh(AssetsManager.instance.GetMesh("Monkey")), new Shader(AssetsManager.instance.GetShader("Default"))));
                    meshLaucherPed._shader.AddTexture(new Texture(AssetsManager.instance.GetTexture("woodplank01", "png")));
                    RigidBody body = monkeyRigid.AddRigidBody();
                    meshLaucherPed.Transparency = true;
                }

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
                _UIManager.Text = "FPS: " + FPS + " UPS: " + UPS + " Tick: " + Time._Tick % 60 + " Objects: " + GameObjects.Count + " CameraPosition: " + Camera.Main.gameObject._transform.Position.ToString() + " CameraRotation: " + Camera.Main.gameObject._transform.Rotation.ToString();

                _physics.UpdatePhisics((float)e.Time);

                if (Focused) // check to see if the window is focused  
                {
                    MouseCursor.CursorLockPosition();
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
                Debug.LogError("Crash: " + ex.Message);
            }
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.CullFace);
                FPS = (int)(1f / e.Time);
                DrawUpdate.Invoke(e);//Draw all opaque objects
                //TransparentDrawUpdate.Invoke(e);//Draw all transparency Objects
            GL.Disable(EnableCap.CullFace);
                if (EngineReady)
                {
                    //_UIManager.DrawUI();
                }
            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Console.WriteLine("MousePressed: "+ e.Button);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Console.WriteLine("MouseUnPressed: " + e.Button);
            base.OnMouseUp(e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            Console.WriteLine("MouseWheel: " + e.Delta);
            base.OnMouseWheel(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Input.GetMouse = e;
            base.OnMouseMove(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            for (int i = 0; i < SceneManager.GetSceneArray.Count; i++)
            {
                SceneManager.GetSceneArray[i].OnUnloadScene();
            }

            _assetsManager.UnloadAll();
            _UIManager.Dispose();
            _physics.Dispose();
            base.OnUnload(e);
        }

        public void RemoveObject(GameObject obj)
        {
            if (GameObjects.Contains(obj))
            {
                obj.OnDestroy();
                GameObjects.Remove(obj);
                obj = null;
            }
        }
        public void AddObject(GameObject obj)
        {
            GameObjects.Add(obj);
        }

        public void LoadDimension(string dimensionID, Dimension dimension)
        {
            _Dimensions.Add(dimensionID, dimension);
        }

        public void UnloadDimension(Dimension dimension)
        {
            _ToUnloadDimension.Enqueue(dimension);
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
            _isLocked = true;
            Engine.Instance.CursorVisible = false;
        }

        public static void UnLockCursor()
        {
            _isLocked = false;
            Engine.Instance.CursorVisible = true;
        }

        public static void CursorLockPosition()
        {
            if (_isLocked)
            {
                Mouse.SetPosition(Engine.Instance.X + Engine.Instance.Width / 2f, Engine.Instance.Y + Engine.Instance.Height / 2f);
            }
        }
    }
}
