using OpenTK;
using OpenTK.Input;
using ProjectEvlly;
using ProjectEvlly.src;
using ProjectEvlly.src.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace EvllyEngine
{
    public class PlayerEntity : Entity
    {
        public static PlayerEntity Instance;

        private PlayerController _playerController;

        public Block CcurrentBlock;
#if Client
        public MeshRender _MeshRender;
        private bool renderMesh = true;
#elif Server
#endif

        public PlayerEntity()
        {
            Instance = this;
        }

        public PlayerEntity(NetViewSerializer entity) : base(entity)
        {
            
        }

        public override void OnStart()
        {
            transform.Position = new Vector3(0, 5, 0);
#if Client
            if (isMine)
            {
                renderMesh = false;
                _playerController = new PlayerController(this, 0.5f);//Start the player controller, only if you own this entity(if you spawened this)
            }
            _MeshRender = new MeshRender(transform, AssetsManager.GetMesh("Cube"), AssetsManager.GetShader("Default"), AssetsManager.GetTexture("devTexture"));
#elif Server
#endif

            base.OnStart();
        }

        public override void Tick()
        {
#if Client
            if (_playerController != null)
            {
                _playerController.UpdateController();
            }

            if (isMine)//if this is my
            {
                RPC("RPC_SyncPosition", ProjectEvlly.src.Net.RPCMode.AllNoOwner, transform.Position, transform.Rotation);

                if (Input.GetKeyDown(Key.V))
                {
                    if (renderMesh)
                    {
                        renderMesh = false;
                    }
                    else
                    {
                        renderMesh = true;
                    }
                }
            }
            else
            {
                
            }
#elif Server

#endif
            base.Tick();
        }

        public override void Draw()
        {
#if Client
            if (renderMesh)
            {
                _MeshRender.Draw();
            }
#endif
            base.Draw();
        }

        public override void OnDestroy()
        {
#if Client
            if (_playerController != null)
            {
                _playerController.DisposeController();
            }

            _MeshRender.OnDestroy();
#endif
            base.OnDestroy();
        }

        [RPC]
        public void RPC_SyncPosition(Vector3 position, Quaternion rotation)
        {
            transform.Position = position;
            transform.Rotation = rotation;
        }
    }
}
