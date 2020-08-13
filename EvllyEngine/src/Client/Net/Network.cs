using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EvllyEngine;
using Lidgren.Network;
using ProjectEvlly.src.Entitys;

namespace ProjectEvlly.src.Net
{
    public class Network
    {
        internal static NetPeer MyPeer;
        internal static NetConnection MyConnection;
        internal static NetServer Server;
        internal static NetClient Client;
        internal static NetDeliveryMethod DefaultDeliveryMethod;
        internal static bool Runing;

        private static bool _ISSERVER = false;
        private static bool _ISCLIENT = false;

        private static Dictionary<int, Entity> Entitys = new Dictionary<int, Entity>();
        private static Queue<Entity> ToUnloadEntitys = new Queue<Entity>();

        public static bool IsServer { get { return _ISSERVER; } }
        public static bool IsClient { get { return _ISCLIENT; } }
        public static bool Ready { get; private set; }
        public static NetPeerStatistics PeerStat;

        public static Action<NetConnection> OnPlayerDisconnect;
        public static Action<NetConnection> OnPlayerConnect;
        public static Action<string, NetConnection> PlayerApproval;
        public static Action OnDisconnect;

        public static Action OnConnect;

        public static Action OnServerStart;
        public static Action OnClientStart;

        public Network()
        {

        }

        /// <summary>
        /// Create a server, local server or dedicated server
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="maxplayers"></param>
        /// <returns></returns>
        public static bool CreateServer(string ip, int port, int maxplayers)
        {
            if (Client == null && Server == null)
            {
                Runing = true;

                DefaultDeliveryMethod = NetDeliveryMethod.ReliableOrdered;

                bool Started = false;
                long ipe = 0;
                long.TryParse(ip, out ipe);

                NetPeerConfiguration config = new NetPeerConfiguration(NetConfig.AppIdentifier);
                config.MaximumConnections = maxplayers;

                config.EnableUPnP = !NetConfig.DedicatedServer;
                config.AutoFlushSendQueue = true;
                config.DefaultOutgoingMessageCapacity = NetConfig.DefaultOutgoingMessageCapacity;
                config.UseMessageRecycling = true;
                config.SendBufferSize = NetConfig.SendBufferSize;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                config.AcceptIncomingConnections = NetConfig.AcceptConnection;
                config.NetworkThreadName = "DarckNet - Server";

                config.Port = port;
                config.BroadcastAddress = new IPAddress(ipe);

                NetServer peer = new NetServer(config);
                peer.Start(); // needed for initialization

                MyPeer = peer;

                if (peer.Status == NetPeerStatus.Running)
                {
                    Started = true;
                }

                Server = peer;
                PeerStat = peer.Statistics;
                MyConnection = ServerConnection;

                _ISCLIENT = false;
                _ISSERVER = true;

                Debug.Log("Unique identifier is " + peer.UniqueIdentifier);
                Ready = true;

                if (OnServerStart != null)
                {
                    OnServerStart();
                }
                return Started;
            }
            else
            {
                Debug.LogError("Server already started");
                return false;
            }
        }

        /// <summary>
        /// Connect to remote server.
        /// </summary>
        /// <param name="Ip"> ip to connect </param>
        /// <param name="Port"></param>
        /// <param name="Password"></param>
        public static NetPeer Connect(string Ip, int Port, string Password)
        {
            if (Server == null && Client == null)
            {
                Runing = true;

                DefaultDeliveryMethod = NetDeliveryMethod.ReliableOrdered;

                NetPeerConfiguration config = new NetPeerConfiguration(NetConfig.AppIdentifier);

                config.AutoFlushSendQueue = true;
                config.DefaultOutgoingMessageCapacity = NetConfig.DefaultOutgoingMessageCapacity;
                config.UseMessageRecycling = true;
                config.SendBufferSize = NetConfig.SendBufferSize;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                config.ConnectionTimeout = NetConfig.ConnectionTimeout;
                config.NetworkThreadName = "DarckNet - Client";

                NetClient peer = new NetClient(config);
                peer.Start(); // needed for initialization

                MyPeer = peer;

                NetOutgoingMessage approval = peer.CreateMessage();
                approval.Write(NetConfig.SecretKey);

                peer.Connect(Ip, Port, approval);

                Client = peer;
                PeerStat = peer.Statistics;

                _ISCLIENT = true;
                _ISSERVER = false;

                Debug.Log("Unique identifier is " + peer.UniqueIdentifier);

                Ready = true;

                var om = peer.CreateMessage();
                peer.SendUnconnectedMessage(om, new IPEndPoint(IPAddress.Loopback, Port));
                try
                {
                    peer.SendUnconnectedMessage(om, new IPEndPoint(IPAddress.Loopback, Port));
                }
                catch (NetException nex)
                {
                    if (nex.Message != "This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently")
                        throw;
                }

                if (OnClientStart != null)
                {
                    OnClientStart();
                }
                return peer;
            }
            else
            {
                Debug.LogError("You already connected in some server");
                return null;
            }
        }

        public static void SpawnEntity(Entity entity)
        {
            if (Ready)
            {
                int viewid = UniqueID(5);

                if (IsClient)
                {
                    entity.SetUpNet(viewid, MyPeer.UniqueIdentifier, NetDeliveryMethod.Unreliable);
                    AddEntityList(entity);

                    Debug.Log("Spawned A Entity, ID: " + viewid);
                    var om = MyPeer.CreateMessage();

                    /*while (NetworkViews.ContainsKey(viewid))
                    {
                        viewid = UniqueID(5);
                    }*/

                    om.Write((byte)DataType.Instantiate);

                    om.Write((byte)GetEntityType(entity));

                    om.Write(viewid);
                    om.Write(entity._currentChannelID);
                    om.WriteVariableInt64(entity._Owner);

                    //Position
                    om.Write(entity.transform.Position.X);
                    om.Write(entity.transform.Position.Y);
                    om.Write(entity.transform.Position.Z);

                    //Rotation
                    om.Write(entity.transform.Rotation.X);
                    om.Write(entity.transform.Rotation.Y);
                    om.Write(entity.transform.Rotation.Z);

                    //Client.SendMessage(om, DefaultDeliveryMethod);
                }
                else
                {
                    entity.SetUpNet(viewid, MyPeer.UniqueIdentifier, NetDeliveryMethod.Unreliable);
                    AddEntityList(entity);

                    Debug.Log("Spawned A Entity, ID: " + viewid);
                    var om = MyPeer.CreateMessage();

                    /*while (NetworkViews.ContainsKey(viewid))
                    {
                        viewid = UniqueID(5);
                    }*/

                    om.Write((byte)DataType.Instantiate);

                    om.Write((byte)GetEntityType(entity));

                    om.Write(viewid);
                    om.Write(entity._currentChannelID);
                    om.WriteVariableInt64(entity._Owner);

                    //Position
                    om.Write(entity.transform.Position.X);
                    om.Write(entity.transform.Position.Y);
                    om.Write(entity.transform.Position.Z);

                    //Rotation
                    om.Write(entity.transform.Rotation.X);
                    om.Write(entity.transform.Rotation.Y);
                    om.Write(entity.transform.Rotation.Z);

                    List<NetConnection> listanet = new List<NetConnection>();
                    if (MyPeer.Connections.Count > 0)
                    {
                        for (int i = 0; i < MyPeer.Connections.Count; i++)
                        {
                            if (MyPeer.Connections[i].RemoteUniqueIdentifier != MyPeer.UniqueIdentifier)
                            {
                                listanet.Add(MyPeer.Connections[i]);
                            }
                        }

                        //Server_SendToAll(om, listanet.ToArray(), DefaultDeliveryMethod);
                    }
                }
            }
        }

        public static void DestroyEntity(Entity entity)
        {
            if (Ready)
            {
                int Dimension = entity._currentChannelID;
                int viewid = entity._ViewID;

                var om = MyPeer.CreateMessage();

                om.Write((byte)DataType.DestroyEntity);
                om.Write(viewid);

                if (IsClient)
                {
                    Client.SendMessage(om, DefaultDeliveryMethod);
                }
                else if (IsServer)
                {
                    List<NetConnection> listanet = new List<NetConnection>();
                    if (MyPeer.Connections.Count > 0)
                    {
                        for (int i = 0; i < MyPeer.Connections.Count; i++)
                        {
                            if (MyPeer.Connections[i].RemoteUniqueIdentifier != MyPeer.UniqueIdentifier)
                            {
                                listanet.Add(MyPeer.Connections[i]);
                            }
                        }

                        Server_SendToAll(om, listanet.ToArray(), DefaultDeliveryMethod);
                    }

                    RemoveEntityList(entity);
                }
            }
            else
            {
                Debug.LogError("You are disconnected from any server, you can't destroy objects!");
            }
        }

        private static void AddEntityList(Entity entity)
        {
            Entitys.Add(entity._ViewID, entity);
        }

        private static void RemoveEntityList(Entity entity)
        {
            if (!ToUnloadEntitys.Contains(entity))
            {
                ToUnloadEntitys.Enqueue(entity);
                Entitys.Remove(entity._ViewID);
            }
        }

        private static void AddEntityFromType(NetViewSerializer entitySerializer)
        {
            switch (entitySerializer.EntityType)
            {
                case EntityType.none:
                    Entitys.Add(entitySerializer.ViewID, new TesteEntityRemote(entitySerializer));
                    break;
                case EntityType.PlayerEntity:
                    Entitys.Add(entitySerializer.ViewID, new PlayerEntityRemote(entitySerializer));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Move a entity to a Region
        /// </summary>
        /// <param name="entity"></param>
        public static void MoveEntityToRegion(Entity entity, int channelId)
        {
            RemoveEntityFromRegion(entity);//Firts Remove and move to the new one

            entity._currentChannelID = channelId;
        }

        public static void RemoveEntityFromRegion(Entity entity)
        {
            //Destroy the entity on the current region
        }

        public static void Server_SendToAll(NetOutgoingMessage msg, NetConnection[] connections, NetDeliveryMethod method)
        {
            if (IsServer)
            {
                Server.SendMessage(msg, connections, method, 0);
            }
            else
            {
                Debug.LogError("Sorry for now, is disable for client");
            }
        }

        public static EntityType GetEntityType(Entity entity)
        {
            if (typeof(PlayerEntity) == entity.GetType())
            {
                return EntityType.PlayerEntity;
            }
            else
            {
                return EntityType.none;
            }
        }

        public static NetConnection GetMyConnection()
        {
            foreach (var item in MyPeer.Connections)
            {
                if (item.RemoteUniqueIdentifier == MyPeer.UniqueIdentifier)
                {
                    return item;
                }
            }
            Debug.LogError("Sorry this connection don't exist!");
            return null;
        }

        public static NetConnection ServerConnection
        {
            get
            {
                NetConnection retval = null;
                if (MyPeer.Connections.Count > 0)
                {
                    try
                    {
                        retval = MyPeer.Connections[0];
                    }
                    catch
                    {
                        // preempted!
                        return null;
                    }
                }
                return retval;
            }
        }

        /// <summary>
        /// Generate a unique id. Length is for how long you want to be the id, 1 is normal(short)
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static int UniqueID(int Length)
        {
            Random random = new Random();
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
            int z1 = random.Next(0, 1000000);
            int z2 = random.Next(0, 1000);
            return (currentEpochTime / z1 + z2 * Length);
        }

        /// <summary>
        /// Disconnect From Server, or if is server Shutdown the server.
        /// </summary>
        public static void Disconnect()
        {
            if (IsServer)
            {
                var om = MyPeer.CreateMessage();

                Ready = false;//Stop any outgoing pakets

                om.Write((byte)DataType.ServerStop);
                Server.SendToAll(om, DefaultDeliveryMethod);

                Debug.Log("SERVER-NET: Waiting for everyone disconnect, before shutdown the server");
                while (Server.Connections.Count > 0) { }
                Debug.Log("SERVER-NET: Everyone is out, now we can shutdown the server (:");

                Server.Shutdown("ServerClosed");

                Client = null;
                Server = null;
                MyPeer = null;
                Runing = false;

                _ISCLIENT = false;
                _ISSERVER = false;

                //Clear the entitys
                foreach (var item in Entitys)
                {
                    ToUnloadEntitys.Enqueue(item.Value);
                }

                while (ToUnloadEntitys.Count > 0)
                {
                    Entity entity = ToUnloadEntitys.Dequeue();

                    entity.OnDestroy();
                }

                Entitys.Clear();
                ToUnloadEntitys.Clear();

                Entitys = null;
                ToUnloadEntitys = null;

                Debug.Log("SERVER-NET: Server is Offline, and memory is clear!");
            }
            else if (IsClient)
            {
                var om = MyPeer.CreateMessage();

                Client.Disconnect("Disconnect");

                Client = null;
                Server = null;
                MyPeer = null;
                Runing = false;

                Ready = false;

                _ISCLIENT = false;
                _ISSERVER = false;

                //Clear the entitys
                foreach (var item in Entitys)
                {
                    ToUnloadEntitys.Enqueue(item.Value);
                }

                while (ToUnloadEntitys.Count > 0)
                {
                    Entity entity = ToUnloadEntitys.Dequeue();

                    entity.OnDestroy();
                }

                Entitys.Clear();
                ToUnloadEntitys.Clear();

                Entitys = null;
                ToUnloadEntitys = null;
            }
        }

        /// <summary>
        ///  Update method network
        /// </summary>
        public static void NetworkTick()
        {
            if (ToUnloadEntitys != null)
            {
                while (ToUnloadEntitys.Count > 0)
                {
                    Entity entity = ToUnloadEntitys.Dequeue();
                    entity.OnDestroy();
                }
            }

            if (MyPeer != null)
            {
                if (IsServer)
                {
                    NetIncomingMessage inc;
                    while ((inc = Server.ReadMessage()) != null)
                    {
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.VerboseDebugMessage:
                                Debug.Log(inc.ReadString());
                                break;
                            case NetIncomingMessageType.DebugMessage:
                                Debug.Log(inc.ReadString());
                                break;
                            case NetIncomingMessageType.WarningMessage:
                                Debug.LogWarning(inc.ReadString());
                                break;
                            case NetIncomingMessageType.ErrorMessage:
                                string erro = inc.ReadString();
                                Debug.Log(erro);
                                if (erro == "Shutdown complete")
                                {
                                    if (OnPlayerDisconnect != null)
                                    {
                                        OnPlayerDisconnect(inc.SenderConnection);
                                    }
                                }
                                break;
                            case NetIncomingMessageType.Data:
                                ReadServerData(inc);
                                break;
                            case NetIncomingMessageType.ConnectionApproval:
                                string s = inc.ReadString();
                                if (PlayerApproval != null)
                                {
                                    PlayerApproval.Invoke(s, inc.SenderConnection);
                                }
                                else
                                {
                                    if (s == NetConfig.SecretKey)
                                    {
                                        inc.SenderConnection.Approve();
                                    }
                                    else
                                    {
                                        inc.SenderConnection.Deny("Sorry your data isnt equal to server!");
                                    }
                                }
                                break;
                            default:
                                if (inc.SenderConnection.Status == NetConnectionStatus.Connected)
                                {
                                    if (OnPlayerConnect != null)
                                    {
                                        OnPlayerConnect(inc.SenderConnection);
                                    }

                                    List<NetViewSerializer> netvi = new List<NetViewSerializer>();

                                    var om = MyPeer.CreateMessage();
                                    om.Write((byte)DataType.OnConnectData);

                                    om.WriteVariableInt64(inc.SenderConnection.RemoteUniqueIdentifier);

                                    foreach (var kvp in Entitys.Values)
                                    {
                                        NetViewSerializer neww = new NetViewSerializer();
                                        neww.EntityType = GetEntityType(kvp);

                                        neww.Owner = kvp._Owner;
                                        neww.ViewID = kvp._ViewID;
                                        neww.ChannelID = kvp._currentChannelID;

                                        neww.p_x = kvp.transform.Position.X;
                                        neww.p_y = kvp.transform.Position.Y;
                                        neww.p_z = kvp.transform.Position.Z;

                                        neww.r_x = kvp.transform.Rotation.X;
                                        neww.r_y = kvp.transform.Rotation.Y;
                                        neww.r_z = kvp.transform.Rotation.Z;

                                        netvi.Add(neww);
                                    }

                                    //Write the data to send when client connect
                                    string data = XMLHelper.ToXML(netvi.ToArray());
                                    om.Write(CompressString.StringCompressor.CompressString(data));


                                    Server.SendMessage(om, inc.SenderConnection, DefaultDeliveryMethod);//Send the data to Connection
                                }
                                else if (inc.SenderConnection.Status == NetConnectionStatus.RespondedConnect)
                                {
                                    Debug.Log("This Player : " + NetUtility.ToHexString(inc.SenderConnection.RemoteUniqueIdentifier) + " Are Accepted to server");
                                }
                                else if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected)
                                {
                                    if (OnPlayerDisconnect != null)
                                    {
                                        OnPlayerDisconnect(inc.SenderConnection);
                                    }

                                    Entity[] obj = Entitys.Values.ToArray();

                                    for (int i = 0; i < obj.Length; i++)
                                    {
                                        if (obj[i]._Owner == inc.SenderConnection.RemoteUniqueIdentifier)
                                        {
                                            DestroyEntity(obj[i]);
                                        }
                                    }

                                    Debug.Log("Player : " + NetUtility.ToHexString(inc.SenderConnection.RemoteUniqueIdentifier) + " Disconnected!");
                                }
                                else if (inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                                {
                                    //last paket sande to client, and after this is disconnect
                                }
                                break;
                        }
                        Server.Recycle(inc);
                    }
                }
                else if (IsClient)
                {
                    NetIncomingMessage inc;
                    while ((inc = Client.ReadMessage()) != null)
                    {
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.VerboseDebugMessage:
                                Debug.Log(inc.ReadString());
                                break;
                            case NetIncomingMessageType.DebugMessage:
                                Debug.Log(inc.ReadString());
                                break;
                            case NetIncomingMessageType.WarningMessage:
                                Debug.LogWarning(inc.ReadString());
                                break;
                            case NetIncomingMessageType.ErrorMessage:
                                string erro = inc.ReadString();
                                Debug.Log(erro);
                                if (erro == "Shutdown complete")
                                {
                                    if (OnPlayerDisconnect != null)
                                    {
                                        OnPlayerDisconnect(inc.SenderConnection);
                                    }
                                }
                                break;
                            case NetIncomingMessageType.Data:
                                ReadClientData(inc);
                                break;
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus status = (NetConnectionStatus)inc.ReadByte();

                                if (status == NetConnectionStatus.Disconnected)
                                {
                                    if (OnDisconnect != null)
                                    {
                                        OnDisconnect();
                                    }
                                }
                                else if (status == NetConnectionStatus.Disconnecting)
                                {
                                    Debug.Log("Disconnect from server");
                                }else if (status == NetConnectionStatus.Connected)
                                {
                                    if (OnConnect != null)
                                    {
                                        OnConnect();
                                    }
                                }
                                break;
                            default:
                                switch (inc.SenderConnection.Status)
                                {
                                    case NetConnectionStatus.None:
                                        break;
                                    case NetConnectionStatus.InitiatedConnect:
                                        break;
                                    case NetConnectionStatus.ReceivedInitiation:
                                        break;
                                    case NetConnectionStatus.RespondedAwaitingApproval:
                                        break;
                                    case NetConnectionStatus.RespondedConnect:
                                        Debug.LogError("This Player : " + NetUtility.ToHexString(inc.SenderConnection.RemoteUniqueIdentifier) + " Are Accepted to server");
                                        break;
                                    case NetConnectionStatus.Connected:
                                        break;
                                    case NetConnectionStatus.Disconnecting:
                                        break;
                                    case NetConnectionStatus.Disconnected:
                                        if (OnPlayerDisconnect != null)
                                        {
                                            OnPlayerDisconnect(inc.SenderConnection);
                                        }

                                        Entity[] obj = Entitys.Values.ToArray();

                                        for (int i = 0; i < obj.Length; i++)
                                        {
                                            if (obj[i]._Owner == inc.SenderConnection.RemoteUniqueIdentifier)
                                            {
                                                DestroyEntity(obj[i]);
                                            }
                                        }

                                        Debug.Log("Player Disconnected : " + NetUtility.ToHexString(inc.SenderConnection.RemoteUniqueIdentifier) + " Disconnected! From Server, Reson : ");
                                        break;
                                    default:
                                        break;
                                }
                                break;
                        }
                        Client.Recycle(inc);
                    }
                }
            }
        }

        private static void ReadServerData(NetIncomingMessage inc)
        {
            DataType type = (DataType)inc.ReadByte();
            List<NetConnection> listanet = new List<NetConnection>();

            switch (type)
            {
                case DataType.RPC:
                    break;
                case DataType.RPC_All:
                    break;
                case DataType.RPC_AllOwner:
                    break;
                case DataType.RPC_Owner:
                    break;
                case DataType.RPC_ALLDimension:
                    break;
                case DataType.Instantiate:
                    NetViewSerializer instantiateEntity = new NetViewSerializer();

                    instantiateEntity.EntityType = (EntityType)inc.ReadByte();

                    instantiateEntity.ViewID = inc.ReadInt32();
                    instantiateEntity.ChannelID = inc.ReadInt32();
                    instantiateEntity.Owner = inc.ReadVariableInt64();

                    //Position
                    instantiateEntity.p_x = inc.ReadFloat();
                    instantiateEntity.p_y = inc.ReadFloat();
                    instantiateEntity.p_z = inc.ReadFloat();

                    //Rotation
                    instantiateEntity.r_x = inc.ReadFloat();
                    instantiateEntity.r_y = inc.ReadFloat();
                    instantiateEntity.r_z = inc.ReadFloat();

                    AddEntityFromType(instantiateEntity);

                    ///Send to all to instantiate///

                    listanet.Clear();
                    for (int i = 0; i < MyPeer.Connections.Count; i++)
                    {
                        if (MyPeer.Connections[i].RemoteUniqueIdentifier != MyPeer.UniqueIdentifier)
                        {
                            listanet.Add(MyPeer.Connections[i]);
                        }
                    }

                    var om = MyPeer.CreateMessage();

                    om.Write((byte)DataType.Instantiate);

                    om.Write((byte)instantiateEntity.EntityType);

                    om.Write(instantiateEntity.ViewID);
                    om.Write(instantiateEntity.ChannelID);
                    om.WriteVariableInt64(instantiateEntity.Owner);

                    //Position
                    om.Write(instantiateEntity.p_x);
                    om.Write(instantiateEntity.p_y);
                    om.Write(instantiateEntity.p_z);

                    //Rotation
                    om.Write(instantiateEntity.r_x);
                    om.Write(instantiateEntity.r_y);
                    om.Write(instantiateEntity.r_z);

                    Server_SendToAll(om, listanet.ToArray(), DefaultDeliveryMethod);
                    break;
                case DataType.DestroyEntity:
                    int viewid = inc.ReadInt32();
                    RemoveEntityList(Entitys[viewid]);

                    //Send To all Clients
                    if (MyPeer.Connections.Count > 0)
                    {
                        var destroyMSG = MyPeer.CreateMessage();

                        destroyMSG.Write((byte)DataType.DestroyEntity);
                        destroyMSG.Write(viewid);

                        listanet.Clear();

                        for (int i = 0; i < MyPeer.Connections.Count; i++)
                        {
                            if (MyPeer.Connections[i].RemoteUniqueIdentifier != MyPeer.UniqueIdentifier)
                            {
                                listanet.Add(MyPeer.Connections[i]);
                            }
                        }

                        Server_SendToAll(destroyMSG, listanet.ToArray(), DefaultDeliveryMethod);
                    }
                    break;
                case DataType.OnConnectData:
                    break;
                case DataType.ExitDimension:
                    break;
                case DataType.ServerStop:
                    break;
                default:
                    break;
            }
        }

        private static void ReadClientData(NetIncomingMessage inc)
        {
            DataType type = (DataType)inc.ReadByte();

            switch (type)
            {
                case DataType.RPC:
                    break;
                case DataType.RPC_All:
                    break;
                case DataType.RPC_AllOwner:
                    break;
                case DataType.RPC_Owner:
                    break;
                case DataType.RPC_ALLDimension:
                    break;
                case DataType.Instantiate:
                    NetViewSerializer instantiateEntity = new NetViewSerializer();

                    instantiateEntity.EntityType = (EntityType)inc.ReadByte();

                    instantiateEntity.ViewID = inc.ReadInt32();
                    instantiateEntity.ChannelID = inc.ReadInt32();
                    instantiateEntity.Owner = inc.ReadVariableInt64();

                    //Position
                    instantiateEntity.p_x = inc.ReadFloat();
                    instantiateEntity.p_y = inc.ReadFloat();
                    instantiateEntity.p_z = inc.ReadFloat();

                    //Rotation
                    instantiateEntity.r_x = inc.ReadFloat();
                    instantiateEntity.r_y = inc.ReadFloat();
                    instantiateEntity.r_z = inc.ReadFloat();

                    AddEntityFromType(instantiateEntity);
                    break;
                case DataType.DestroyEntity:
                    int viewid = inc.ReadInt32();
                    RemoveEntityList(Entitys[viewid]);
                    break;
                case DataType.OnConnectData:
                    break;
                case DataType.ExitDimension:
                    break;
                case DataType.ServerStop:
                    break;
                default:
                    break;
            }
        }
    }

    [Serializable]
    public class NetViewSerializer
    {
        public int ChannelID = 0;
        public EntityType EntityType;
        public long Owner;
        public int ViewID = 0;

        public float p_x;
        public float p_y;
        public float p_z;

        public float r_x;
        public float r_y;
        public float r_z;
    }

    public static class XMLHelper
    {
        public static string SerializeObject<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T DeSerializeObject<T>(string xmlstring)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(xmlstring))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static T[] FromXML<T>(string json)
        {
            Wrapper<T> wrapper = DeSerializeObject<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToXML<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return SerializeObject(wrapper);
        }

        #region ArrayCord

        public static T[,] FromXMLChunk<T>(string json)
        {
            WrapperChunk<T> wrapper = DeSerializeObject<WrapperChunk<T>>(json);
            return wrapper.Items;
        }

        public static string ToXML<T>(T[,] array)
        {
            WrapperChunk<T> wrapper = new WrapperChunk<T>();
            wrapper.Items = array;
            return SerializeObject(wrapper);
        }
        #endregion

        [System.Serializable]
        public class Wrapper<T>
        {
            public T[] Items;
        }

        [System.Serializable]
        public class WrapperChunk<T>
        {
            public T[,] Items;
        }
    }

    public static class NetConfig
    {
        public static string SecretKey = "secret";
        public static int DefaultOutgoingMessageCapacity = 99999;
        public static int SendBufferSize = 131071;
        public static float ConnectionTimeout = 50;
        public static bool AcceptConnection = true;
        public static string AppIdentifier = "UnityGame";
        /// <summary>
        /// TiketRate is the milliseconds to Thread stop and continue. 15 is Recommended
        /// </summary>
        public static int TickRate = 15;
        /// <summary>
        /// If is true Unat is false, if is false Unat is true.
        /// </summary>
        public static bool DedicatedServer = false;
    }

    namespace CompressString
    {
        internal static class StringCompressor
        {
            /// <summary>
            /// Compresses the string.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns></returns>
            public static string CompressString(string text)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;

                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);

                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                return Convert.ToBase64String(gZipBuffer);
            }

            /// <summary>
            /// Decompresses the string.
            /// </summary>
            /// <param name="compressedText">The compressed text.</param>
            /// <returns></returns>
            public static string DecompressString(string compressedText)
            {
                byte[] gZipBuffer = Convert.FromBase64String(compressedText);
                using (var memoryStream = new MemoryStream())
                {
                    int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                    var buffer = new byte[dataLength];

                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }

                    return Encoding.UTF8.GetString(buffer);
                }
            }
        }
    }

    public struct EntityChannel
    {
        public int ViewId;
        public int ChannelId;

        public EntityChannel(int viewid, int channelid)
        {
            ViewId = viewid;
            ChannelId = channelid;
        }
    }

    public enum DataType : byte
    {
        RPC,
        RPC_All,
        RPC_AllOwner,
        RPC_Owner,
        RPC_ALLDimension,

        Instantiate,
        DestroyEntity,
        OnConnectData,

        ExitDimension,
        ServerStop
    }

    [Serializable]
    public enum EntityType : byte
    {
        none, PlayerEntity
    }
}
