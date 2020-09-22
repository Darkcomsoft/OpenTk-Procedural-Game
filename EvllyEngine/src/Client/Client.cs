using EvllyEngine;
using OpenTK;
using OpenTK.Input;
using ProjectEvlly.src.UI;
using ProjectEvlly.src.World;
using ProjectEvlly.src.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEvlly.src.save;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.Engine;

namespace ProjectEvlly.src
{
    /// <summary>
    /// This class is for the game logics, so every time you quit a server, or quit to mainmenu,
    /// game class is gone do erease all and start again, so this is not a static class or final class, is a dynamic class.
    /// </summary>
    public class Client : IDisposable
    {
        public GUIMainMenu _MainMenu;
        private InGameUI _InGameUI;
        private bool _isPlaying;

        private Sky _Sky;

        private MidleWorld _MidleWorld;
        //private World _TimeWorld;

        public CharSaveInfo[] CharacterSaveList;
        
        public CharSaveInfo _CharacterInfo;

        private TickSystem _RenderSystem;

        public Action<FrameEventArgs> UIDrawUpdate;

        public Client()
        {
            Game.Client = this;

            _RenderSystem = new TickSystem();
            _Sky = new Sky(AssetsManager.GetShader("Sky"), AssetsManager.GetTexture("devTexture2"));

            EvllyEngine.MouseCursor.UnLockCursor();

            CharacterSaveList = SaveManager.LoadChars();

            Network.OnServerStart += OnServerStart;
            Network.OnDisconnect += OnDsiconnect;
            Network.OnServerStop += OnServerClose;
            Network.OnClientStart += OnClientStart;
            Network.OnConnect += OnConnect;

            LoadMainMenu();
        }

        public void Tick()
        {
            Network.NetworkTick();

            if (_isPlaying)
            {
                _RenderSystem.Tick(Time._DeltaTime);

                _Sky.Tick();

                if (Input.GetKeyDown(Key.F11))
                {
                    if (Window.Instance.WindowState == WindowState.Fullscreen)
                    {
                        Window.Instance.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        Window.Instance.WindowState = WindowState.Fullscreen;
                    }
                }

                if (_InGameUI != null)
                {
                    _InGameUI.OnResize();
                }
            }
            else//Menu Update
            {
                
            }
        }

        public void Draw(FrameEventArgs e)
        {
            if (_isPlaying)
            {
                _Sky.Draw(e);
                _RenderSystem.RenderTick((float)e.Time);
                Utilitys.CheckGLError("End Of DrawUpdate");
            }
            else//Menu Draw
            {

            }
        }

        public void Dispose()
        {
            Network.Disconnect();

            if (!_isPlaying)
            {
                _MainMenu.Dispose();
            }

            if (_MidleWorld != null)
            {
                _MidleWorld.Dispose();
            }

            if (_InGameUI != null)
            {
                _InGameUI.Dispose();
            }

            if (_RenderSystem != null)
            {
                _RenderSystem.Dispose();
            }
            _Sky.Dispose();

            Network.OnServerStart -= OnServerStart;
            Network.OnDisconnect -= OnDsiconnect;
            Network.OnServerStop -= OnServerClose;
            Network.OnClientStart -= OnClientStart;
            Network.OnConnect -= OnConnect;
        }

        public void CreateWorldAndPlay(CharSaveInfo CharacterInfo)
        {
            List<CharSaveInfo> characterInfos = new List<CharSaveInfo>();
            characterInfos.AddRange(CharacterSaveList);

            characterInfos.Add(CharacterInfo);

            CharacterSaveList = characterInfos.ToArray();

            SaveManager.SaveChars(CharacterSaveList);
        }

        public void DeleteCharAndWorld(CharSaveInfo CharacterInfo)
        {
            List<CharSaveInfo> characterInfos = new List<CharSaveInfo>();
            characterInfos.AddRange(CharacterSaveList);

            characterInfos.Remove(CharacterInfo);

            CharacterSaveList = characterInfos.ToArray();

            SaveManager.SaveChars(CharacterSaveList);

            //Delete World Here
        }

        public void PlaySingleWorld(CharSaveInfo CharacterInfo)
        {
            _CharacterInfo = CharacterInfo;

            Network.CreateServer("127.0.0.1", 2500, 10);
        }

        public void LoadMainMenu()
        {
            if (_InGameUI != null)
            {
                _InGameUI.Dispose();
                _InGameUI = null;
            }

            if (_MidleWorld != null)
            {
                _MidleWorld.Dispose();
                _MidleWorld = null;
            }

            _RenderSystem.Dispose();
            _RenderSystem = null;



            _MainMenu = new GUIMainMenu();
            _isPlaying = false;
        }

        private void LoadPlayingWorld()
        {
            _InGameUI = new InGameUI(Game.GUI.GetCanvas);
            _RenderSystem = new TickSystem();

            _MidleWorld = new MidleWorld(_CharacterInfo.WorldName);
            _MidleWorld.SpawnPlayer(_CharacterInfo);

            _MainMenu.Dispose();
            _MainMenu = null;

            _isPlaying = true;
        }

        private void OnServerStart()
        {
            LoadPlayingWorld();
        }

        private void OnServerClose()
        {
            LoadMainMenu();
        }

        private void OnDsiconnect()
        {
            LoadMainMenu();
        }

        private void OnClientStart()
        {
            
        }

        private void OnConnect()
        {
            LoadPlayingWorld();
        }
    }

    public class Game
    {
        public static Client Client;
        public static GUI GUI;
        public static MidleWorld MidleWorld;
        public static InGameUI _InGameUI;
        public static Window Window;

        public static MidleWorld GetWorld
        {
            get
            {
                if (MidleWorld != null)
                {
                    return MidleWorld;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
