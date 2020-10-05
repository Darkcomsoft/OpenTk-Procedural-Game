using EvllyEngine;
using ProjectEvlly.src.game;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.save;
using ProjectEvlly.src.UI;
using ProjectEvlly.src.User;
using ProjectEvlly.src.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectEvlly.src
{
    /// <summary>
    /// All global info of the gameplay world.
    /// </summary>
    public class GamePlay : IDisposable
    {
        public static PlayerEntity CurrentPlayerEntity;

        private ClientType _SessionType;

        private GameUI _InGameUI;

        private SkyTime _skyTime;
        private WorldManager _worldManager;

        public GamePlay(ClientType SessionType)
        {
            Game.GamePlay = this;

            _SessionType = SessionType;

            _InGameUI = new GameUI();

            _skyTime = new SkyTime();
            _worldManager = new WorldManager();

            Network.OnReceivedServerData += OnReceivedServerData;

            switch (SessionType)
            {
                case ClientType.SinglePlayer:
                    _InGameUI.SetCharData(SaveManager.LoadChars());
                    break;
                case ClientType.Multiplayer:
                    //Network.RequestServerData(chara.CharName, AuthManager.GetUserId);
                    break;
            }
        }

        public void Tick()
        {
            _InGameUI.Tick();
            _skyTime.Tick();
        }

        public void TickRender()
        {
            _skyTime.TickRender();
        }

        public void Dispose()
        {
            _InGameUI.Dispose();

            Network.OnReceivedServerData -= OnReceivedServerData;

            _skyTime.Dispose();
            _worldManager.Dispose();
        }

        private void LoadWorld()
        {
            _worldManager.LoadWorld(new MidleWorld());
            Thread.Sleep(100);
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
#if Client
            CurrentPlayerEntity = new PlayerEntity();

            if (Network.IsServer)//is singleplayer
            {
                Network.SpawnEntity(CurrentPlayerEntity);
            }
            else
            {
                Network.SpawnEntity(CurrentPlayerEntity);
            }
#endif
        }

        private void OnReceivedServerData()
        {
            
        }

        internal void PlayClick()
        {
            switch (_SessionType)
            {
                case ClientType.SinglePlayer:
                    LoadWorld();
                    break;
                case ClientType.Multiplayer:
                    
                    break;
            }
        }

        internal void DeleteClick()
        {

        }
    }
}
