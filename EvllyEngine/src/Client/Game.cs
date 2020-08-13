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
    public class Game : GameRef
    {
        public MainMenu _MainMenu;
        private bool _isPlaying;

        private MidleWorld _MidleWorld;
        //private World _TimeWorld;

        public CharSaveInfo[] CharacterSaveList;
        
        public CharSaveInfo _CharacterInfo;

        public Action TickEvent;
        public Action<FrameEventArgs> DrawUpdate;
        public Action<FrameEventArgs> TransparentDrawUpdate;
        public Action<FrameEventArgs> UIDrawUpdate;

        public Game()
        {
            Game = this;
            EvllyEngine.MouseCursor.UnLockCursor();

            CharacterSaveList = SaveManager.LoadChars();

            Network.OnServerStart += OnServerStart;
            Network.OnClientStart += OnClientStart;
            Network.OnConnect += OnConnect;

            LoadMainMenu();
        }

        public void Tick()
        {
            Network.NetworkTick();

            if (_isPlaying)
            {
                TickEvent.Invoke();

                if (_MidleWorld != null)
                {
                    _MidleWorld.Tick();
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

                if (Input.GetKeyDown(Key.Escape))
                {
                    Window.Instance.Exit();
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
                _MidleWorld.Draw(e);
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

            Network.Disconnect();

            Network.OnServerStart -= OnServerStart;
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

            _MainMenu.Dispose();
            _MainMenu = null;

            Network.CreateServer("127.0.0.1", 2500, 10);
        }

        public void LoadMainMenu()
        {
            if (_MidleWorld != null)
            {
                _MidleWorld.DisposeWorld();
                _MidleWorld = null;
            }

            _MainMenu = new MainMenu(GUI.GetCanvas);
            _isPlaying = false;
        }

        private void OnServerStart()
        {
            _MidleWorld = new MidleWorld(_CharacterInfo);
            _isPlaying = true;
        }

        private void OnClientStart()
        {
            
        }

        private void OnConnect()
        {
            _MainMenu.Dispose();
            _MainMenu = null;

            _MidleWorld = new MidleWorld(_CharacterInfo);
            _isPlaying = true;
        }
    }

    public class GameRef
    {
        public static Game Game;
        public static GUI GUI;
        public static WorldBase World;

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
