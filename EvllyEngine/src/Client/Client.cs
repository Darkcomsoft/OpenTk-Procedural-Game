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
using ProjectEvlly.src.User;
using System.Threading;

namespace ProjectEvlly.src
{
    /// <summary>
    /// This class is for the game logics, so every time you quit a server, or quit to mainmenu,
    /// game class is gone do erease all and start again, so this is not a static class or final class, is a dynamic class.
    /// </summary>
    public class Client : IDisposable
    {
        public GUIMainMenu _MainMenu;

        private Sky _Sky;
        private TickSystem _RenderSystem;

        private GamePlay _GamePlayManager;

        private bool _isPlaying = false;
        public static ClientType _clientType;

        public CharSaveInfo[] CharacterSaveList;
        public CharSaveInfo _CharacterInfo;

        public Action<FrameEventArgs> UIDrawUpdate;

        public Client()
        {
            Game.Client = this;

            _RenderSystem = new TickSystem();
            
            _Sky = new Sky(AssetsManager.GetShader("Sky"), AssetsManager.GetTexture("devTexture2"));

            EvllyEngine.MouseCursor.UnLockCursor();

            CharacterSaveList = SaveManager.LoadChars();//Load All SinglePlayer CharInfo

            Network.OnServerStart += OnServerStart;
            Network.OnDisconnect += OnDsiconnect;
            Network.OnServerStop += OnServerClose;
            Network.OnClientStart += OnClientStart;
            Network.OnConnect += OnConnect;

            LoadMainMenu();
        }
        private bool isPused = true;
        public void Tick()
        {
            Network.NetworkTick();

            if (_isPlaying)
            {
                _RenderSystem.Tick(Time._DeltaTime);
                _GamePlayManager.Tick();
                _Sky.Tick();

                if (Input.GetKeyDown(OpenTK.Input.Key.Escape))
                {
                    if (isPused)
                    {
                        isPused = false;
                        EvllyEngine.MouseCursor.LockCursor();
                    }
                    else
                    {
                        isPused = true;
                        EvllyEngine.MouseCursor.UnLockCursor();
                    }
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
            }
            else//Menu Update
            {
                _MainMenu.Tick();
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

            if (_GamePlayManager != null)
            {
                _GamePlayManager.Dispose();
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

        public void ConnectToASever(string ip, int port, string pass)
        {
            _MainMenu.Open_ConnectingScreen(ClientType.Multiplayer);
            Network.Connect(ip, port, pass);
        }

        public void StartSingleServer()
        {
            _MainMenu.Open_ConnectingScreen(ClientType.SinglePlayer);

            NextFrameQueue.Enqueue(() => { Network.CreateServer("127.0.0.1", 25000, 1); });
        }

        private void LoadMainMenu()
        {
            _isPlaying = false;

            if (_GamePlayManager != null)
            {
                _GamePlayManager.Dispose();
                _GamePlayManager = null;
            }

            _MainMenu = new GUIMainMenu(this);
        }

        private void LoadGamePlay()
        {
            _isPlaying = true;

            _GamePlayManager = new GamePlay(_clientType);

            _MainMenu.Dispose();
            _MainMenu = null;
        }

        private void OnServerStart()
        {
            _clientType = ClientType.SinglePlayer;
            LoadGamePlay();
        }

        private void OnServerClose()
        {
            LoadMainMenu();
            _clientType = ClientType.Offine;
        }

        private void OnDsiconnect()
        {
            LoadMainMenu();
            _clientType = ClientType.Offine;
        }

        private void OnClientStart()
        {
            
        }

        private void OnConnect()
        {
            _clientType = ClientType.Multiplayer;
            LoadGamePlay();
        }
    }

    public class Game
    {
        public static Client Client;
        public static GUI GUI;
        public static MidleWorld MidleWorld;
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

    public enum ClientType : byte
    {
        Offine, SinglePlayer, Multiplayer
    }
}
