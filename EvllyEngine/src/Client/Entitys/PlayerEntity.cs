using BulletSharp;
using OpenTK;
using OpenTK.Input;
using ProjectEvlly;
using ProjectEvlly.src.Client;
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
#elif Server
#endif

        public PlayerEntity()
        {

        }

        public PlayerEntity(NetViewSerializer entity) : base(entity)
        {

        }

        public override void OnStart()
        {
            Instance = this;

            transform.Position = new Vector3(0, 5, 0);

            if (isMine)
            {
                _playerController = new PlayerController(this, 0.5f);//Start the player controller, only if you own this entity(if you spawened this)
            }

#if Client
            _MeshRender = new MeshRender(transform, AssetsManager.instance.GetErrorMesh, AssetsManager.GetShader("Default"), AssetsManager.GetTexture("devTexture"));
#elif Server
#endif

            base.OnStart();
        }

        public override void OnUpdate()
        {
#if Client
            if (_playerController != null)
            {
                _playerController.UpdateController();
            }

            if (isMine)//if this is my
            {

            }
            else
            {
                RPC("RPC_MouseLock", ProjectEvlly.src.Net.RPCMode.AllNoOwner, transform.Position, transform.Rotation);
            }
#elif Server

#endif
        }

        public override void OnDrawOpaque(FrameEventArgs e)
        {
#if Client
            _MeshRender.Draw(e);
#endif
            base.OnDrawOpaque(e);
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
        public void RPC_MouseLock(Vector3 position, Quaternion rotation)
        {
            transform.Position = position;
            transform.Rotation = rotation;
        }
    }
}
