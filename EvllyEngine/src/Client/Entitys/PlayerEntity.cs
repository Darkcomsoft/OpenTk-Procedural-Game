using OpenTK;
using OpenTK.Input;
using ProjectEvlly;
using ProjectEvlly.src;
using ProjectEvlly.src.Engine.Render;
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
        private PlayerController _playerController;

        public Block CurrentBlock;
#if Client
        public MeshRender _MeshRender;
        public MeshRender _MeshRenderSword;
        private bool renderMesh = false;
#elif Server
#endif

        public PlayerEntity() { }
        public PlayerEntity(NetViewSerializer entity) : base(entity) { }

        public override void OnStart()
        {
            transform.Position = new Vector3(0, 30, 0);

#if Client
            if (isMine)
            {
                renderMesh = false;
                _playerController = new PlayerController(this, 0.5f);//Start the player controller, only if you own this entity(if you spawened this)

                /*_MeshRenderSword = new MeshRender(Camera.Main._transformParent, AssetsManager.GetMesh("SwordMetal"), AssetsManager.GetShader("Default"), AssetsManager.GetTexture("MetalSword"));
                RenderSystem.AddRenderItem(_MeshRenderSword);*/
            }
            //_MeshRender = new MeshRender(transform, AssetsManager.GetMesh("Cube"), AssetsManager.GetShader("Default"), AssetsManager.GetTexture("devTexture"));
            //RenderSystem.AddRenderItem(_MeshRender);
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

                CurrentBlock = Game.GetWorld.GetTileAt(transform.Position.X, transform.Position.Z);

                if (Input.GetKeyDown(Key.V))
                {
                    if (renderMesh)
                    {
                        renderMesh = false;
                        //CurrentBlock = Game.GetWorld.GetTileAt((int)transform.Position.X, (int)transform.Position.Z);
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

        public override void Dispose()
        {
#if Client
            if (_playerController != null)
            {
                _playerController.Dispose();
            }
            //RenderSystem.RemoveRenderItem(_MeshRenderSword);
            //RenderSystem.RemoveRenderItem(_MeshRender);
#endif
            base.Dispose();
        }

        [RPC]
        public void RPC_SyncPosition(Vector3 position, Quaternion rotation)
        {
            transform.Position = position;
            transform.Rotation = rotation;
        }
    }
}
