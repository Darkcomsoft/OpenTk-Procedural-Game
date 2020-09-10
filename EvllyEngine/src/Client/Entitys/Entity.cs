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
using System.Reflection;
using ProjectEvlly.src.Utility;

namespace ProjectEvlly
{
    /// <summary>
    /// A base entity class, this is used for all entitys, all entitys is synced with multiplayer, if you want use a static entity dont use this
    /// </summary>
    public abstract class Entity : IDisposable
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

        [NonSerialized]
        internal Dictionary<string, RPCALL> _methodlist = new Dictionary<string, RPCALL>();

        public Entity()
        {
#if Client
            Game.Client.TickEvent += Tick;
#elif Server
            Server.Tick += Tick;
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
            Game.Client.TickEvent += Tick;
#elif Server
            Server.Tick += Tick;
#endif

            transform = new Transform();

            transform.Position = new Vector3(entity.p_x, entity.p_y, entity.p_z);
            transform.Rotation = new Quaternion(entity.r_x, entity.r_y, entity.r_z, 0);

            SetUpNet(entity.ViewID, entity.Owner, NetDeliveryMethod.Unreliable);
            OnConstruc();
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
            OnStart();
        }

        /// <summary>
        /// this is called when the class is instanced, but some things on the class is not ready, like the setup of networkingview 
        /// </summary>
        public virtual void OnConstruc()
        {
            GetMethodList();
        }

        /// <summary>
        /// OnStartet is called when the entity is ready, all logic is done, like the network setup
        /// </summary>
        public virtual void OnStart()
        {

        }

        public virtual void Tick()
        {

        }

        public virtual void OnDead() { }

        public virtual void Dispose()
        {
#if Client
            Game.Client.TickEvent -= Tick;
#elif Server
            Server.Tick -= Tick;
#endif

            _IsEntityReady = false;
        }

        /// <summary>
        /// Send RPC with RPCmode and default NetDeliveryMethod
        /// </summary>
        /// <param name="funcname"></param>
        /// <param name="Mode"></param>
        /// <param name="param"></param>
        public void RPC(string funcname, RPCMode Mode, params object[] param)
        {
            if (Network.Ready)
            {
                SendRPC(funcname, Mode, _DefaultNetDeliveryMethod, param);
            }
        }

        /// <summary>
        /// Send RPC to Specifique User(NetPeer), with a default NetDeliveryMethod
        /// </summary>
        /// <param name="funcname"></param>
        /// <param name="player"></param>
        /// <param name="param"></param>
        public void RPC(string funcname, NetConnection player, params object[] param)
        {
            if (Network.Ready)
            {
                SendRPC(funcname, player, _DefaultNetDeliveryMethod, param);
            }
        }

        #region RPC
        private void SendRPC(string funcname, RPCMode Mode, NetDeliveryMethod DeliveryMethod, params object[] param)
        {
            var om = Network.MyPeer.CreateMessage();
            
            switch (Mode)
            {
                case RPCMode.All:
                    om.Write((byte)DataType.RPC_All);

                    om.Write(funcname);
                    om.Write(_ViewID);
                    //om.Write(Dimension);//Deisabled for now

                    DoData(om, param);

                    if (Network.IsClient)
                    {
                        Network.Client.SendMessage(om, DeliveryMethod);
                    }
                    else
                    {
                        Network.RPC_All(funcname, _ViewID, param);
                    }
                    break;
                case RPCMode.AllNoOwner:
                    om.Write((byte)DataType.RPC_AllOwner);

                    om.Write(funcname);
                    om.Write(_ViewID);
                    //om.Write(Dimension);

                    DoData(om, param);

                    if (Network.IsClient)
                    {
                        Network.Client.SendMessage(om, DeliveryMethod);
                    }
                    else
                    {
                        Network.RPC_AllOwner(funcname, _ViewID, param);
                    }
                    break;
                case RPCMode.Owner:
                    om.Write((byte)DataType.RPC_Owner);

                    om.Write(funcname);
                    om.Write(_ViewID);

                    DoData(om, param);

                    if (Network.IsClient)
                    {
                        Network.Client.SendMessage(om, DeliveryMethod);
                    }
                    else
                    {
                        Network.RPC_Owner(funcname, _ViewID, param);
                    }
                    break;
                case RPCMode.Server:
                    om.Write((byte)DataType.RPC);

                    om.Write(funcname);
                    om.Write(_ViewID);

                    DoData(om, param);

                    if (Network.IsClient == true)
                    {
                        Network.Client.SendMessage(om, DeliveryMethod);
                    }
                    else
                    {
                        Network.RPC_Server(funcname, _ViewID, param);
                    }
                    break;
            }
        }

        private void SendRPC(string funcname, NetConnection player, NetDeliveryMethod DeliveryMethod, params object[] param)
        {
            var om = Network.MyPeer.CreateMessage();

            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(_ViewID);

            DoData(om, param);

            Network.MyPeer.SendMessage(om, player, DeliveryMethod);
            return;
        }

        private void DoData(NetOutgoingMessage om, object[] param)
        {
            foreach (var item in param)
            {
                Type type = item.GetType();

                if (type == typeof(string))
                {
                    om.Write((string)item);
                }
                else if (type == typeof(int))
                {
                    om.Write((int)item);
                }
                else if (type == typeof(bool))
                {
                    om.Write((bool)item);
                }
                else if (type == typeof(float))
                {
                    om.Write((float)item);
                }
                else if (type == typeof(Vector3))
                {
                    Vector3 vec = (Vector3)item;

                    om.Write(vec.X);
                    om.Write(vec.Y);
                    om.Write(vec.Z);
                }
                else if (type == typeof(Vector2))
                {
                    Vector2 vec = (Vector2)item;

                    om.Write(vec.X);
                    om.Write(vec.Y);
                }
                else if (type == typeof(Quaternion))
                {
                    Quaternion vec = (Quaternion)item;

                    om.Write(vec.X);
                    om.Write(vec.Y);
                    om.Write(vec.Z);
                }
            }
        }

        public object[] Execute(string funcName, NetIncomingMessage msg)
        {
            RPCALL ent;

            if (_methodlist.TryGetValue(funcName, out ent))
            {
                if (ent._parameters == null)
                    ent._parameters = ent._function.GetParameters();

                try
                {
                    List<object> objects = new List<object>();

                    for (int i = 0; i < ent._parameters.Length; i++)
                    {
                        objects.Add(ReadArgument(msg, ent._parameters[i].ParameterType));
                    }

                    ent._function.Invoke(ent._obj, objects.ToArray());
                    return objects.ToArray();
                }
                catch (System.Exception ex)
                {
                    if (ex.GetType() == typeof(System.NullReferenceException)) return null;
                    Debug.LogException(ex.ToString());
                    return null;
                }
            }
            return null;
        }

        public object[] Execute(string funcName, object[] param)
        {
            RPCALL ent;

            if (_methodlist.TryGetValue(funcName, out ent))
            {
                if (ent._parameters == null)
                    ent._parameters = ent._function.GetParameters();

                try
                {
                    List<object> objects = new List<object>();

                    foreach (var item in ent._parameters)
                    {
                        if (item.ParameterType == typeof(DNetConnection))
                        {
                            DNetConnection dnet = new DNetConnection();
                            dnet.unique = Network.MyPeer.UniqueIdentifier;
                            objects.Add(dnet);
                        }
                        else
                        {
                            objects.Add(item);
                        }
                    }

                    ent._function.Invoke(ent._obj, objects.ToArray());
                    return objects.ToArray();
                }
                catch (System.Exception ex)
                {
                    if (ex.GetType() == typeof(System.NullReferenceException)) return null;
                    Debug.LogException(ex.ToString());
                    return null;
                }
            }
            return null;
        }

        private void GetMethodList()
        {
            _methodlist.Clear();

            System.Type type = this.GetType();

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo method = methods[i];

                if (method.IsDefined(typeof(RPC), true))
                {
                    RPCALL ent = new RPCALL();
                    ent._function = method;
                    ent._obj = this;

                    RPC tnc = (RPC)ent._function.GetCustomAttributes(typeof(RPC), true)[0];

                    _methodlist[method.Name] = ent;
                }
            }
        }

        private static object ReadArgument(NetIncomingMessage msg, Type type)
        {
            if (type == typeof(int))
            {
                return msg.ReadInt32();
            }
            else if (type == typeof(byte))
            {
                return msg.ReadByte();
            }
            else if (type == typeof(bool))
            {
                return msg.ReadBoolean();
            }
            else if (type == typeof(float))
            {
                return msg.ReadFloat();
            }
            else if (type == typeof(Vector3))
            {
                return NetBit.ReadVector3(msg);
            }
            else if (type == typeof(Vector2))
            {
                return NetBit.ReadVector2(msg);
            }
            else if (type == typeof(Quaternion))
            {
                return NetBit.ReadQuaternion(msg);
            }
            else if (type == typeof(DNetConnection))
            {
                return new DNetConnection(msg.SenderConnection);
            }
            else if (type == typeof(string))
            {
                return msg.ReadString();
            }
            else
            {
                throw new Exception("Unsupported argument type " + type);
            }
        }

        #endregion

        public bool isMine 
        { 
            get 
            { 
                if (Network.Runing) 
                { 
                    if (_Owner == Network.MyPeer.UniqueIdentifier) 
                    {
                        return true;
                    } 
                    else 
                    {
                        return false;
                    } 
                } 
                else 
                {
                    return false;
                } 
            } 
        }
        public bool isReady { get { return _IsEntityReady; } }
    }
}
