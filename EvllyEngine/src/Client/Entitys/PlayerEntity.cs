using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using ProjectEvlly;
using ProjectEvlly.src;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.UI.GUIElements;
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

        private GUILable PlayerPosition;
        private GUILable BlockInfo;
#elif Server
#endif

        public PlayerEntity() { }
        public PlayerEntity(NetViewSerializer entity) : base(entity) { }

        public override void OnStart()
        {
            System.Random rand = new Random(GlobalData.Seed);
            //transform.Position = new Vector3(rand.Next(-100,100), 30, rand.Next(-100, 100));
            transform.Position = new Vector3(0, 30, 0);
#if Client
            if (isMine)
            {
                PlayerPosition = new GUILable("No Player Data", new System.Drawing.Rectangle(5, 50, 200, 20), ProjectEvlly.src.UI.UIDock.TopLeft);
                PlayerPosition.SetColor(Color4.White);
                PlayerPosition.SetTextAling(ProjectEvlly.src.UI.Font.TextAling.Left);
                PlayerPosition.ShowBackGround = false;

                BlockInfo = new GUILable("No Player Data", new System.Drawing.Rectangle(5, 70, 200, 20), ProjectEvlly.src.UI.UIDock.TopLeft);
                BlockInfo.SetColor(Color4.White);
                BlockInfo.SetTextAling(ProjectEvlly.src.UI.Font.TextAling.Left);
                BlockInfo.ShowBackGround = false;


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
                //RPC("RPC_SyncPosition", ProjectEvlly.src.Net.RPCMode.AllNoOwner, transform.Position, transform.Rotation);

                CurrentBlock = Game.GetWorld.GetTileAt(transform.Position.X, transform.Position.Z);
                PlayerPosition.SetText(string.Format("Position({0})", transform.Position.ToString()));
                BlockInfo.SetText(string.Format("Block:{0}", CurrentBlock.ToString()));
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
            if (PlayerPosition != null)
            {
                PlayerPosition.Dispose();
            }

            if (BlockInfo != null)
            {
                BlockInfo.Dispose();
            }

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
