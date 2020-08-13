using EvllyEngine;
using OpenTK;
using ProjectEvlly.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using ProjectEvlly.src.Net;

namespace ProjectEvlly
{
    /// <summary>
    /// A base entity class, this is used for all entitys, all entitys is synced with multiplayer, if you want use a static entity dont use this
    /// </summary>
    public abstract class Entity
    {
        public Transform transform;
        public int _currentChannelID = 0;//This is the world id, like the normal world, all regions of chunks have ther owne channel id.

        private int HP;
        private int MaxHP;

        private bool _isDead;

        ///NetWork Values of the Entity
        private bool _IsEntityReady = false;
        public long _Owner;
        public int _ViewID = 0;
        public NetDeliveryMethod _DefaultNetDeliveryMethod = NetDeliveryMethod.Unreliable;

        public Entity()
        {
#if Client
            Game.Game.TickEvent += OnUpdate;
            Game.Game.DrawUpdate += OnDrawOpaque;
            Game.Game.TransparentDrawUpdate += OnDrawT;
#elif Server
            Server.Tick += OnUpdate;
#endif

            transform = new Transform();
            OnConstruc();
            OnStart();

            Debug.Log("Entity Creator1");
        }

        /// <summary>
        /// this is for network, instance entity and setup netview
        /// </summary>
        /// <param name="entity"></param>
        public Entity(NetViewSerializer entity)
        {
#if Client
            Game.Game.TickEvent += OnUpdate;
            Game.Game.DrawUpdate += OnDrawOpaque;
            Game.Game.TransparentDrawUpdate += OnDrawT;
#elif Server
            Server.Tick += OnUpdate;
#endif

            SetUpNet(entity.ViewID, 0, NetDeliveryMethod.Unknown);

            transform = new Transform();

            transform.Position = new Vector3(entity.p_x, entity.p_y, entity.p_z);
            transform.Rotation = new Quaternion(entity.r_x, entity.r_y, entity.r_z, 0);

            OnConstruc();
            OnStart();
            Debug.Log("Entity Creator2");
        }

        /// <summary>
        /// This is for seting up the entity in networking, only use in network class
        /// </summary>
        public void SetUpNet(int id, long owner, NetDeliveryMethod netDeliveryMethod)
        {
            _ViewID = id;
            _Owner = owner;
            _DefaultNetDeliveryMethod = netDeliveryMethod;

            _IsEntityReady = true;
        }

        public virtual void OnConstruc()
        {

        }

        public virtual void OnStart()
        {

        }

        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// Draw only opaque model
        /// </summary>
        public virtual void OnDrawOpaque(FrameEventArgs e)
        {

        }
        /// <summary>
        /// Draw only Transparency model
        /// </summary>
        public virtual void OnDrawT(FrameEventArgs e)
        {

        }

        public virtual void OnDead() { }

        public virtual void OnDestroy()
        {
#if Client
            Game.Game.TickEvent -= OnUpdate;
            Game.Game.DrawUpdate -= OnDrawOpaque;
            Game.Game.TransparentDrawUpdate -= OnDrawT;
#elif Server
            Server.Tick -= OnUpdate;
#endif

            _IsEntityReady = false;
        }

        public void RPC()
        {

        }

        public bool isMine { get { bool Mine = false; if (Network.Runing) { if (_Owner == Network.MyPeer.UniqueIdentifier) { Mine = true; } else { Mine = false; } } else { Mine = false; } return Mine; } }
        public bool isReady { get { return _IsEntityReady; } }
    }
}
