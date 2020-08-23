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

namespace ProjectEvlly.src
{
    /// <summary>
    /// This class is for the game logics, so every time you quit a server, or quit to mainmenu,
    /// game class is gone do erease all and start again, so this is not a static class or final class, is a dynamic class.
    /// </summary>
    public class Client
    {
        public MainMenu _MainMenu;
        private InGameUI _InGameUI;
        private bool _isPlaying;

        private MidleWorld _MidleWorld;
        //private World _TimeWorld;

        public CharSaveInfo[] CharacterSaveList;
        
        public CharSaveInfo _CharacterInfo;

        public Action TickEvent;
        public Action<FrameEventArgs> DrawUpdate;
        public Action<FrameEventArgs> TransparentDrawUpdate;
        public Action<FrameEventArgs> UIDrawUpdate;

        public Client()
        {
            Game.Client = this;
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
                if (TickEvent != null)
                {
                    TickEvent.Invoke();
                }

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
                _MainMenu.OnResize();
            }
        }

        public void Draw(FrameEventArgs e)
        {
            if (_isPlaying)
            {
                DrawUpdate.Invoke(e);//Draw all opaque objects
                Utilitys.CheckGLError("End Of DrawUpdate");
                TransparentDrawUpdate.Invoke(e);
                Utilitys.CheckGLError("End Of Transparent Draw");
            }
            else//Menu Draw
            {

            }
        }

        public void Dispose()
        {
            if (!_isPlaying)
            {
                _MainMenu.Dispose();
            }

            if (_MidleWorld != null)
            {
                _MidleWorld.DisposeWorld();
            }

            if (_InGameUI != null)
            {
                _InGameUI.Dispose();
            }

            Network.Disconnect();

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
                _MidleWorld.DisposeWorld();
                _MidleWorld = null;
            }

            _MainMenu = new MainMenu(Game.GUI.GetCanvas);
            _isPlaying = false;
        }

        private void LoadPlayingWorld()
        {
            _InGameUI = new InGameUI(Game.GUI.GetCanvas);

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
        public static WorldBase World;
        public static InGameUI _InGameUI;

        public static WorldBase GetWorld
        {
            get
            {
                if (World != null)
                {
                    return World;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
