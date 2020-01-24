using System;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using System.Net;
using System.Reflection;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace DarckNet
{
    public enum RPCMode
    {
        All, AllNoOwner, AllNoDimension, Owner, Server
    }

    /// <summary>
    /// All Static Method, Connect, StartServer, Disconnect, Instantiate, Destroy
    /// </summary>
    public class Network
    {
        #region Vars
        public static string NetVersion = "V: Alpha 0.1.7";
        internal static NetPeer MyPeer;
        internal static NetConnection MyConnection;
        internal static NetServer Server;
        internal static NetClient Client;
        internal static List<NetPeer> Players = new List<NetPeer>();
        internal static Dictionary<int, NetworkObj> NetworkViews = new Dictionary<int, NetworkObj>();
        internal static Dictionary<int, WorldList> NetDimension = new Dictionary<int, WorldList>();
        internal static Dictionary<long, NetConnection> ConnectionList = new Dictionary<long, NetConnection>();
        internal static List<DarckMonoBehaviour> Events = new List<DarckMonoBehaviour>();
        internal static NetworkPrefabs PregabsList;
        internal static NetDeliveryMethod NetDeliveryMode = NetDeliveryMethod.UnreliableSequenced;
        internal static bool onconnectbool = false;
        internal static bool onstartserverbool = false;
        private static bool _ISSERVER = false;
        private static bool _ISCLIENT = false;
        public static bool ServerSinglePlayer = false;
        public static bool Runing { get; private set; }
        public static bool IsServer { get { return _ISSERVER; }}
        public static bool IsClient { get { return _ISCLIENT; }}
        public static bool Ready { get; private set; }
        public static NetPeerStatistics PeerStat;
        public static int DimensionGeral = -1;
        #endregion

        /// <summary>
        /// Create a server, local server or dedicated server
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="maxplayers"></param>
        /// <returns></returns>
        public static bool Create(string ip, int port, int maxplayers)
        {
            if (Client == null && Server == null)
            {
                Debug.Log("DarckNet " + NetVersion);

                Runing = true;

                NetDeliveryMode = NetDeliveryMethod.UnreliableSequenced;

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
                SETUP_MANUAL_VIEWSID();//setup manual views

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

                if (ServerSinglePlayer)
                {
                    WorldList wlist = new WorldList();
                    wlist.Players.Add(MyPeer.UniqueIdentifier);
                    NetDimension.Add(0, wlist);
                }
                else
                {
                    WorldList wlist = new WorldList();
                    NetDimension.Add(0, wlist);
                }

                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].OnServerStart();
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
                Debug.Log("DarckNet " + NetVersion);

                Runing = true;

                NetDeliveryMode = NetDeliveryMethod.UnreliableSequenced;

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
                SETUP_MANUAL_VIEWSID();//setup manual views

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

                return peer;
            }
            else
            {
                Debug.LogError("You already connected in some server");
                return null;
            }
        }

        /// <summary>
        /// Use Http or https, protocol to connect, its just resolve Url to ip, all the network isn't http(s) protocol!
        /// </summary>
        /// <param name="url"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public static void ConnectUrl(string url, int port, string password)
        {
            url = url.Replace("http://", ""); //remove http://
            url = url.Replace("https://", ""); //remove https://
            url = url.Substring(0, url.IndexOf("/")); //remove everything after the first /

            try
            {
                IPHostEntry hosts = Dns.GetHostEntry(url);
                if (hosts.AddressList.Length > 0)
                    Connect(hosts.AddressList[0].ToString(), port, password);
            }
            catch
            {
                Debug.LogError("Could not get IP for URL " + url);
            }
        }

        /// <summary>
        /// Generate a unique id. Length is for how long you want to be the id, 1 is normal(short)
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static int UniqueID(int Length)
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
            int z1 = UnityEngine.Random.Range(0, 1000000);
            int z2 = UnityEngine.Random.Range(0, 1000);
            return (currentEpochTime / z1 + z2 * Length);
        }

        /// <summary>
        /// Use for set your manual id, manual views is just for like a static viewid, to make like a global viewid
        /// </summary>
        private static void SETUP_MANUAL_VIEWSID()
        {
            foreach (var view in NetworkViews.Values)
            {
                if (view.IdMode == IdMode.Manual_Id)
                {
                    view.Owner = MyPeer.UniqueIdentifier;
                }
            }
        }

        /// <summary>
        /// Get a connection over the id of the connection
        /// </summary>
        /// <param name="uniq"></param>
        /// <returns></returns>
        public static NetConnection GetConnection(long uniq)
        {
            NetConnection net = null;
            ConnectionList.TryGetValue(uniq, out net);

            return net;
        }

        /// <summary>
        /// Get a connection over the id of the connection
        /// </summary>
        /// <param name="uniq"></param>
        /// <returns></returns>
        public static NetConnection[] GetConnections(long[] uniq)
        {
            List<NetConnection> nets = new List<NetConnection>();

            foreach (var item in uniq)
            {
                NetConnection net = null;
                ConnectionList.TryGetValue(item, out net);
                nets.Add(net);
            }
            return nets.ToArray();
        }

        public static NetworkObj GetNetworkViews(int view_id)
        {
            if (NetworkViews.ContainsKey(view_id))
            {
                return NetworkViews[view_id];
            }
            else
            {
                throw new Exception("Don't found this netview : " + view_id + " | maybe is destroyed or isn't over network");
            }
        }

        public static void Server_SendToAll(NetOutgoingMessage msg, NetConnection[] connections, NetDeliveryMethod method)
        {
            if (IsServer)
            {
                if (connections.Length <= 0)
                {
                    if (msg.m_isSent == false)
                        Server.Recycle(msg);
                    return;
                }

                Server.SendMessage(msg, connections, method, 0);
            }
            else
            {
                Debug.LogError("Sorry for now, is disable for client");
            }
        }

        public static NetConnection ServerConnection
        {
            get
            {
                NetConnection retval = null;
                if (MyPeer.m_connections.Count > 0)
                {
                    try
                    {
                        retval = MyPeer.m_connections[0];
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

        /// <summary>
        /// Disconnect From Server, or if is server Shutdown the server.
        /// </summary>
        public static void Disconnect()
        {
            if (IsServer)
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].OnServerClose();
                }

                var om = MyPeer.CreateMessage();

                Ready = false;//Stop any outgoing pakets

                om.Write((byte)DataType.ServerStop);
                Server.SendToAll(om, NetDeliveryMode);

                Debug.Log("SERVER-NET: Waiting for everyone disconnect, before shutdown the server");
                while (Server.Connections.Count > 0) { }
                Debug.Log("SERVER-NET: Everyone is out, now we can shutdown the server (:");
                
                Server.Shutdown("ServerClosed");

                Client = null;
                Server = null;
                MyPeer = null;
                Runing = false;
                onstartserverbool = false;

                _ISCLIENT = false;
                _ISSERVER = false;

                NetworkViews.Clear();
                NetDimension.Clear();
                Players.Clear();
                ConnectionList.Clear();

                DimensionGeral = -1;

                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].OnServerAfterClose();
                }

                Events.Clear();
                Debug.Log("SERVER-NET: Server is Offline, and memory is clear!");
            }
            else if (IsClient)
            {
                var om = MyPeer.CreateMessage();

                om.Write((byte)DataType.ExitDimension);
                om.Write(DimensionGeral);

                Client.SendMessage(om, NetDeliveryMode);

                Client.Disconnect("Disconnect");

                Client = null;
                Server = null;
                MyPeer = null;
                Runing = false;
                onconnectbool = false;
                Ready = false;

                _ISCLIENT = false;
                _ISSERVER = false;

                NetworkViews.Clear();
                NetDimension.Clear();
                Players.Clear();
                ConnectionList.Clear();

                DimensionGeral = -1;

                Events.Clear();
            }
        }

        /// <summary>
        /// Spawn a Unity.GameObject(Networkingview) over the networking.
        /// </summary>
        /// <param name="Object"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="g"></param>
        /// <returns>Game</returns>
        public static GameObject Instantiate(GameObject Object, Vector3 position, Quaternion rotation, int g)
        {
            if (Ready)
            {
                if (NetDimension.ContainsKey(g))
                {
                    var om = MyPeer.CreateMessage();

                    int viewid = UniqueID(5);

                    while (NetworkViews.ContainsKey(viewid))
                    {
                        viewid = UniqueID(5);
                    }

                    om.Write((byte)DataType.Instantiate);
                    om.Write(Object.GetComponent<NetworkObj>().PrefabID);
                    om.Write(viewid);
                    om.Write(g);

                    //Position
                    om.Write(position.x);
                    om.Write(position.y);
                    om.Write(position.z);

                    //Rotation
                    om.Write(rotation.x);
                    om.Write(rotation.y);
                    om.Write(rotation.z);

                    if (IsClient)
                    {
                        GameObject obj = GameObject.Instantiate(Object, position, rotation);
                        NetworkViews.Add(viewid, obj.GetComponent<NetworkObj>());
                        obj.GetComponent<NetworkObj>().SetID(viewid, Object.GetComponent<NetworkObj>().PrefabID, MyPeer.UniqueIdentifier);
                        obj.GetComponent<NetworkObj>().Dimension = g;

                        Client.SendMessage(om, NetDeliveryMode);
                    }
                    else
                    {

                        GameObject obj = GameObject.Instantiate(Object, position, rotation);
                        NetworkViews.Add(viewid, obj.GetComponent<NetworkObj>());
                        obj.GetComponent<NetworkObj>().SetID(viewid, Object.GetComponent<NetworkObj>().PrefabID, MyPeer.UniqueIdentifier);
                        obj.GetComponent<NetworkObj>().Dimension = g;

                        List<NetConnection> listanet = new List<NetConnection>(GetConnections(NetDimension[g].Players.ToArray()));
                        listanet.Remove(GetConnection(MyPeer.UniqueIdentifier));

                        Server_SendToAll(om, listanet.ToArray(), NetDeliveryMode);

                        return obj;
                    }

                    return null;
                }
                else
                {
                    Debug.LogError("This Dimension dont exist, or isnt loaded!");
                    return null;
                }
            }
            else
            {
                Debug.LogError("You still have not, finished Connection.");
                return null;
            }
        }

        /// <summary>
        /// Destroy with time, a "Unity.GameObject(Networkingview)", over the network, only objects spawned in network.
        /// </summary>
        /// <param name="Object">Networkingview Object.</param>
        /// <param name="time">Time to Destroy, In Seconds</param>
        public static void Destroy(GameObject Object, float time)
        {
            Destroy(Object);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Destroy a "Unity.GameObject(Networkingview)" over the network, only objects spawned in network.
        /// </summary>
        /// <param name="Object"></param>
        public static void Destroy(GameObject Object)
        {
            if (Object.GetComponent<NetworkObj>().IdMode == IdMode.AutomaticId)
            {
                if (Ready)
                {
                    int Dimension = Object.GetComponent<NetworkObj>().Dimension;
                    int viewid = Object.GetComponent<NetworkObj>().ViewID;

                    var om = MyPeer.CreateMessage();

                    om.Write((byte)DataType.Destroy);
                    om.Write(viewid);

                    if (NetDimension.ContainsKey(Dimension))
                    {
                        if (NetworkViews.ContainsKey(viewid))
                        {
                            if (IsClient)
                            {
                                Client.SendMessage(om, NetDeliveryMode);
                            }
                            else if (IsServer)
                            {
                                List<NetConnection> listanet = new List<NetConnection>(GetConnections(NetDimension[Dimension].Players.ToArray()));
                                listanet.Remove(GetConnection(MyPeer.UniqueIdentifier));

                                Server_SendToAll(om, listanet.ToArray(), NetDeliveryMode);

                                GameObject.Destroy(Object);
                                
                                NetworkViews.Remove(viewid);
                            }
                        }
                        else
                        {
                            Debug.LogError("Sorry, this object: " + Object.name + " already Destroyed or isn't over network");
                        }
                    }
                    else
                    {
                        Debug.LogError("Sorry this dimension is closed, or empty");
                    }
                }
                else
                {
                    Debug.LogError("You are disconnected from any server, you can't destroy objects!");
                }
            }
            else
            {
                Debug.LogError("This object is manual View, you cant destory using network, you need use unity destroy logic");
            }
        }

        /// <summary>
        /// Use to change your dimension
        /// </summary>
        public static void ChangeDimension(int NextDimension, int LastDimension)
        {
            DimensionGeral = NextDimension;

            var om2 = MyPeer.CreateMessage();

            om2.Write((byte)DataType.ChangeDimension);
            om2.Write(NextDimension);
            om2.Write(LastDimension);

            if (IsClient)
            {
                Client.SendMessage(om2, NetDeliveryMode);
            }
            else
            {
                NetworkObj[] obj = NetworkViews.Values.ToArray();

                for (int i = 0; i < obj.Length; i++)
                {
                    if (obj[i].Owner == Server.UniqueIdentifier)
                    {
                        if (obj[i].IdMode != IdMode.Manual_Id)
                        {
                            Destroy(obj[i].gameObject);
                            NetworkViews.Remove(obj[i].ViewID);
                        }
                    }
                }

                if (NetDimension.ContainsKey(NextDimension))
                {
                    if (NetDimension[NextDimension].Players.Contains(Server.UniqueIdentifier) == false)
                    {
                        NetDimension[NextDimension].Players.Add(Server.UniqueIdentifier);
                    }
                }
                else
                {
                    WorldList wlist = new WorldList();
                    wlist.Players.Add(Server.UniqueIdentifier);
                    NetDimension.Add(NextDimension, wlist);
                }

                if (NetDimension.ContainsKey(LastDimension))
                {
                    NetDimension[LastDimension].Players.Remove(Server.UniqueIdentifier);

                    if (NetDimension[LastDimension].Players.Count <= 0)
                    {
                        NetDimension.Remove(LastDimension);
                    }
                }
            }
        }

        /// <summary>
        /// Used to close a connection, just server can close the connection.
        /// </summary>
        /// <param name="Peer"></param>
        /// <param name="sendmenssagem"></param>
        public static void CloseConnection(NetConnection Peer, bool sendmenssagem)
        {
            if (IsServer)
            {
                var om = MyPeer.CreateMessage();

                om.Write((byte)DataType.CloseConnection);

                Server.SendMessage(om, Peer, NetDeliveryMode);
            }
        }

        /// <summary>
        ///  Update method network
        /// </summary>
        public static void Update()
        {
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
                                Debug.LogError(inc.ReadString());
                                break;
                            case NetIncomingMessageType.DebugMessage:
                                Debug.Log(inc.ReadString());
                                break;
                            case NetIncomingMessageType.WarningMessage:
                                Debug.LogWarning(inc.ReadString());
                                break;
                            case NetIncomingMessageType.ErrorMessage:
                                string erro = inc.ReadString();
                                Debug.LogError(erro);
                                if (erro == "Shutdown complete")
                                {
                                    for (int i = 0; i < Events.Count; i++)
                                    {
                                        Events[i].OnPlayerDisconnect(inc.SenderConnection);
                                    }
                                }
                                break;
                            case NetIncomingMessageType.Data:
                                ProceMenssagServer(inc);
                                break;
                            case NetIncomingMessageType.ConnectionApproval:
                                string s = inc.ReadString();

                                for (int i = 0; i < Events.Count; i++)
                                {
                                    Events[i].PlayerApproval(s, inc.SenderConnection);
                                }
                                break;
                            default:
                                if (inc.SenderConnection.Status == NetConnectionStatus.Connected)
                                {
                                    for (int i = 0; i < Events.Count; i++)
                                    {
                                        Events[i].OnPlayerConnect(inc.SenderConnection);
                                    }
                                    List<NetViewSerializer> netvi = new List<NetViewSerializer>();
                                    var om = MyPeer.CreateMessage();
                                    om.Write((byte)DataType.Instantiate_Pool);

                                    om.WriteVariableInt64(inc.SenderConnection.RemoteUniqueIdentifier);
                                    foreach (var kvp in GetAutomaticView(0))
                                    {
                                        NetViewSerializer neww = new NetViewSerializer();
                                        neww.PrefabID = kvp.PrefabID;
                                        neww.Owner = kvp.Owner;
                                        neww.ViewID = kvp.ViewID;
                                        neww.IdMode = kvp.IdMode;
                                        neww.Dimension = kvp.Dimension;

                                        neww.p_x = kvp.transform.position.x;
                                        neww.p_y = kvp.transform.position.y;
                                        neww.p_z = kvp.transform.position.z;

                                        neww.r_x = kvp.transform.rotation.x;
                                        neww.r_y = kvp.transform.rotation.y;
                                        neww.r_z = kvp.transform.rotation.z;

                                        netvi.Add(neww);
                                    }

                                    if (NetDimension.ContainsKey(0))
                                    {
                                        if (NetDimension[0].Players.Contains(inc.SenderConnection.RemoteUniqueIdentifier) == false)
                                        {
                                            NetDimension[0].Players.Add(inc.SenderConnection.RemoteUniqueIdentifier);
                                        }
                                    }
                                    else
                                    {
                                        WorldList wlist = new WorldList();
                                        wlist.Players.Add(inc.SenderConnection.RemoteUniqueIdentifier);
                                        NetDimension.Add(0, wlist);
                                    }

                                    string dimendata = JsonHelper.ToJson(NetDimension.Values.ToArray());
                                    om.Write(CompressString.StringCompressor.CompressString(dimendata));

                                    Debug.LogError(dimendata);

                                    string data = JsonHelper.ToJson(netvi.ToArray());
                                    om.Write(CompressString.StringCompressor.CompressString(data));

                                    Server.SendMessage(om, inc.SenderConnection, NetDeliveryMode);
                                }
                                else if (inc.SenderConnection.Status == NetConnectionStatus.RespondedConnect)
                                {
                                    Debug.Log("This Player : " + NetUtility.ToHexString(inc.SenderConnection.RemoteUniqueIdentifier) + " Are Accepted to server");
                                    ConnectionList.Add(inc.SenderConnection.RemoteUniqueIdentifier, inc.SenderConnection);
                                }
                                else if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected)
                                {
                                    for (int i = 0; i < Events.Count; i++)
                                    {
                                        Events[i].OnPlayerDisconnect(inc.SenderConnection);
                                    }

                                    NetworkObj[] obj = NetworkViews.Values.ToArray();

                                    for (int i = 0; i < obj.Length; i++)
                                    {
                                        if (obj[i].Owner == inc.SenderConnection.RemoteUniqueIdentifier)
                                        {
                                            Destroy(obj[i].gameObject);
                                            NetworkViews.Remove(obj[i].ViewID);
                                        }
                                    }

                                    ConnectionList.Remove(inc.SenderConnection.RemoteUniqueIdentifier);

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
                                Debug.LogError(inc.ReadString());
                                break;
                            case NetIncomingMessageType.DebugMessage:
                                Debug.LogError(inc.ReadString());
                                break;
                            case NetIncomingMessageType.WarningMessage:
                                Debug.LogWarning(inc.ReadString());
                                break;
                            case NetIncomingMessageType.ErrorMessage:
                                string erro = inc.ReadString();
                                Debug.LogError(erro);
                                if (erro == "Shutdown complete")
                                {
                                    for (int i = 0; i < Events.Count; i++)
                                    {
                                        Events[i].OnPlayerDisconnect(inc.SenderConnection);
                                    }
                                }
                                break;
                            case NetIncomingMessageType.Data:
                                ProceMenssagClient(inc);
                                break;
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus status = (NetConnectionStatus)inc.ReadByte();

                                if (status == NetConnectionStatus.Disconnected)
                                {
                                    for (int i = 0; i < Events.Count; i++)
                                    {
                                        Events[i].OnDisconnect();
                                    }
                                }
                                else if (status == NetConnectionStatus.Disconnecting)
                                {
                                    Debug.Log("Disconnect from server");
                                }
                                break;
                            default:
                                if (inc.SenderConnection.Status == NetConnectionStatus.Connected)
                                {

                                }
                                else if (inc.SenderConnection.Status == NetConnectionStatus.RespondedConnect)
                                {
                                    Debug.LogError("This Player : " + NetUtility.ToHexString(inc.SenderConnection.RemoteUniqueIdentifier) + " Are Accepted to server");

                                    ConnectionList.Add(inc.SenderConnection.RemoteUniqueIdentifier, inc.SenderConnection);
                                }
                                else if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected)
                                {
                                    for (int i = 0; i < Events.Count; i++)
                                    {
                                        Events[i].OnPlayerDisconnect(inc.SenderConnection);
                                    }

                                    NetworkObj[] obj = NetworkViews.Values.ToArray();
                                    for (int i = 0; i < obj.Length; i++)
                                    {
                                        if (obj[i].Owner == inc.SenderConnection.RemoteUniqueIdentifier)
                                        {
                                            NetworkViews.Remove(obj[i].ViewID);
                                            Destroy(obj[i].gameObject);
                                        }
                                    }

                                    ConnectionList.Remove(inc.SenderConnection.RemoteUniqueIdentifier);

                                    Debug.Log("Player Disconnected : " + NetUtility.ToHexString(inc.SenderConnection.RemoteUniqueIdentifier) + " Disconnected! From Server, Reson : " + inc.SenderConnection.m_disconnectMessage);
                                }
                                else if (inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                                {

                                }
                                break;
                        }
                        Client.Recycle(inc);
                    }
                }
            }
        }

        static void ProceMenssagServer(NetIncomingMessage inc)
        {
            DataType type = (DataType)inc.ReadByte();

            if (type == DataType.RPC)//RPC Normal
            {
                inc.ReadString(out string funcname);
                inc.ReadInt32(out int viewidd);

                NetworkObj Net = null;

                if (NetworkViews.TryGetValue(viewidd, out Net))
                {
                    Net.Execute(funcname, inc);
                }
            }
            else if (type == DataType.RPC_All)//all in dimension
            {
                RPC_All(inc);
            }
            else if (type == DataType.RPC_ALLDimension)//All in server, no in dimension
            {
                RPC_ALLDimension(inc);
            }
            else if (type == DataType.RPC_AllOwner)//All but no owner
            {
                RPC_AllOwner(inc);
            }
            else if (type == DataType.RPC_Owner)//Owener
            {
                RPC_Owner(inc);
            }
            else if (type == DataType.Instantiate)
            {
                Vector3 Pos = new Vector3();
                Quaternion Rot = new Quaternion();

                inc.ReadInt32(out int prefabid);
                inc.ReadInt32(out int viewid);
                inc.ReadInt32(out int dimension);

                //Position
                Pos.x = inc.ReadFloat();
                Pos.y = inc.ReadFloat();
                Pos.z = inc.ReadFloat();

                //Rotation
                Rot.x = inc.ReadFloat();
                Rot.y = inc.ReadFloat();
                Rot.z = inc.ReadFloat();
                Rot.w = 1;

                //long uniq = inc.ReadVariableInt64();


                GameObject Objc = GameObject.Instantiate(PregabsList.Prefabs[prefabid], Pos, Rot);
                NetworkViews.Add(viewid, Objc.GetComponent<NetworkObj>());
                Objc.GetComponent<NetworkObj>().SetID(viewid, prefabid, inc.SenderConnection.RemoteUniqueIdentifier);
                Objc.GetComponent<NetworkObj>().Dimension = dimension;

                //--------------------------------------------------//

                #region InstantiateSend
                var om = MyPeer.CreateMessage();

                om.Write((byte)DataType.Instantiate);
                om.Write(prefabid);
                om.Write(viewid);
                om.Write(dimension);

                //Position
                om.Write(Pos.x);
                om.Write(Pos.y);
                om.Write(Pos.z);

                //Rotation
                om.Write(Rot.x);
                om.Write(Rot.y);
                om.Write(Rot.z);

                om.WriteVariableInt64(inc.SenderConnection.RemoteUniqueIdentifier);

                List<NetConnection> listanet = new List<NetConnection>(GetConnections(NetDimension[dimension].Players.ToArray()));
                listanet.Remove(GetConnection(MyPeer.UniqueIdentifier));
                listanet.Remove(GetConnection(inc.SenderConnection.RemoteUniqueIdentifier));

                Server_SendToAll(om, listanet.ToArray(), NetDeliveryMode);
                #endregion
            }
            else if (type == DataType.Destroy)
            {
                inc.ReadInt32(out int viewid);

                NetworkObj net = null;

                if (NetworkViews.TryGetValue(viewid, out net))
                {
                    #region DestroySend
                    var om = MyPeer.CreateMessage();

                    om.Write((byte)DataType.Destroy);
                    om.Write(viewid);

                    List<NetConnection> listanet = new List<NetConnection>(GetConnections(NetDimension[net.Dimension].Players.ToArray()));
                    listanet.Remove(GetConnection(MyPeer.UniqueIdentifier));

                    Server_SendToAll(om, listanet.ToArray(), NetDeliveryMode);
                    #endregion

                    GameObject.Destroy(net.gameObject);
                    NetworkViews.Remove(viewid);
                }
            }
            else if (type == DataType.Destroy_Player)
            {
                NetworkObj[] obj = NetworkViews.Values.ToArray();

                for (int i = 0; i < obj.Length; i++)
                {
                    if (obj[i].Owner == inc.SenderConnection.RemoteUniqueIdentifier)
                    {
                        NetworkViews.Remove(obj[i].ViewID);
                        Destroy(obj[i].gameObject);
                    }
                }
            }
            else if (type == DataType.ChangeDimension)
            {
                inc.ReadInt32(out int dimension);
                inc.ReadInt32(out int lastdimension);

                List<NetViewSerializer> netvi = new List<NetViewSerializer>();

                var om = MyPeer.CreateMessage();

                om.Write((byte)DataType.Instantiate_PoolD);

                om.WriteVariableInt64(inc.SenderConnection.RemoteUniqueIdentifier);
                om.Write(dimension);
                om.Write(lastdimension);

                foreach (var kvp in GetAutomaticView(dimension))
                {
                    NetViewSerializer neww = new NetViewSerializer();
                    neww.PrefabID = kvp.PrefabID;
                    neww.Owner = kvp.Owner;
                    neww.ViewID = kvp.ViewID;
                    neww.IdMode = kvp.IdMode;
                    neww.Dimension = kvp.Dimension;

                    neww.p_x = kvp.transform.position.x;
                    neww.p_y = kvp.transform.position.y;
                    neww.p_z = kvp.transform.position.z;

                    neww.r_x = kvp.transform.rotation.x;
                    neww.r_y = kvp.transform.rotation.y;
                    neww.r_z = kvp.transform.rotation.z;

                    netvi.Add(neww);
                }

                if (NetDimension.ContainsKey(dimension))
                {
                    if (NetDimension[dimension].Players.Contains(inc.SenderConnection.RemoteUniqueIdentifier) == false)
                    {
                        NetDimension[dimension].Players.Add(inc.SenderConnection.RemoteUniqueIdentifier);
                    }
                }
                else
                {
                    WorldList wlist = new WorldList();
                    wlist.Players.Add(inc.SenderConnection.RemoteUniqueIdentifier);
                    NetDimension.Add(dimension, wlist);
                }

                if (NetDimension.ContainsKey(lastdimension))
                {
                    NetDimension[lastdimension].Players.Remove(inc.SenderConnection.RemoteUniqueIdentifier);

                    if (NetDimension[lastdimension].Players.Count <= 0)
                    {
                        NetDimension.Remove(lastdimension);
                    }
                }

                string data = JsonHelper.ToJson(netvi.ToArray());
                om.Write(CompressString.StringCompressor.CompressString(data));

                Server.SendMessage(om, inc.SenderConnection, NetDeliveryMode);

                NetworkObj[] obj = NetworkViews.Values.ToArray();

                for (int i = 0; i < obj.Length; i++)
                {
                    if (obj[i].Owner == inc.SenderConnection.RemoteUniqueIdentifier)
                    {
                        NetworkViews.Remove(obj[i].ViewID);
                        Destroy(obj[i].gameObject);
                    }
                }
            }
            else if (type == DataType.ExitDimension)
            {
                inc.ReadInt32(out int dimension);

                if (NetDimension.ContainsKey(dimension))
                {
                    if (NetDimension[dimension].Players.Count > 0)
                    {
                        NetDimension[dimension].Players.Remove(inc.SenderConnection.RemoteUniqueIdentifier);
                    }
                }

                var om = MyPeer.CreateMessage();
                om.Write((byte)DataType.ExitDimension);
                om.Write(dimension);

                List<NetConnection> listanet = new List<NetConnection>(GetConnections(NetDimension[dimension].Players.ToArray()));
                listanet.Remove(GetConnection(Server.UniqueIdentifier));

                Server_SendToAll(om, listanet.ToArray(), NetDeliveryMode);
            }
            else if (type == DataType.ServerStop)
            {
                Debug.LogError("Wait... you are the server... you can't do nothing with this! go back.");
            }
            else if (type == DataType.RequestStartData)
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].OnPlayerRequestStartData(inc.SenderConnection);
                }
            }
        }

        static void ProceMenssagClient(NetIncomingMessage inc)
        {
            DataType type = (DataType)inc.ReadByte();

            if (type == DataType.RPC)
            {
                inc.ReadString(out string funcname);
                inc.ReadInt32(out int viewidd);

                if (NetworkViews.TryGetValue(viewidd, out NetworkObj Net))
                {
                    Net.Execute(funcname, inc);
                }
            }
            else if (type == DataType.Instantiate)
            {
                Vector3 Pos = new Vector3();
                Quaternion Rot = new Quaternion();

                inc.ReadInt32(out int prefabid);
                inc.ReadInt32(out int viewid);
                inc.ReadInt32(out int dimension);

                //Position
                Pos.x = inc.ReadFloat();
                Pos.y = inc.ReadFloat();
                Pos.z = inc.ReadFloat();

                //Rotation
                Rot.x = inc.ReadFloat();
                Rot.y = inc.ReadFloat();
                Rot.z = inc.ReadFloat();
                Rot.w = 1;

                long uniq = inc.ReadVariableInt64();

                GameObject Objc = GameObject.Instantiate(PregabsList.Prefabs[prefabid], Pos, Rot);
                NetworkViews.Add(viewid, Objc.GetComponent<NetworkObj>());
                Objc.GetComponent<NetworkObj>().SetID(viewid, prefabid, uniq);
                Objc.GetComponent<NetworkObj>().Dimension = dimension;
            }
            else if (type == DataType.Destroy)
            {
                inc.ReadInt32(out int viewid);

                NetworkObj net = null;

                if (NetworkViews.TryGetValue(viewid, out net))
                {
                    GameObject.Destroy(net.gameObject);
                    NetworkViews.Remove(viewid);
                }
            }
            else if (type == DataType.Instantiate_Pool)
            {
                long uniq = inc.ReadVariableInt64();

                inc.ReadString(out string datadime);
                string datadecompress = CompressString.StringCompressor.DecompressString(datadime);
                WorldList[] world = JsonHelper.FromJson<WorldList>(datadecompress);

                inc.ReadString(out string data);
                string worlddecompress = CompressString.StringCompressor.DecompressString(data);
                NetViewSerializer[] net = JsonHelper.FromJson<NetViewSerializer>(worlddecompress);

                foreach (NetViewSerializer nv in net)
                {
                    GameObject Objc = GameObject.Instantiate(PregabsList.Prefabs[nv.PrefabID], new Vector3(nv.p_x, nv.p_y, nv.p_z), new Quaternion(nv.r_x, nv.r_y, nv.r_z, 1));
                    Objc.GetComponent<NetworkObj>().SetID(nv.ViewID, nv.PrefabID, nv.Owner);
                    Objc.GetComponent<NetworkObj>().Dimension = nv.Dimension;
                    NetworkViews.Add(nv.ViewID, Objc.GetComponent<NetworkObj>());
                }

                if (world.Length > 0)
                {
                    int f = -1;
                    foreach (WorldList nv in world)
                    {
                        f++;
                        NetDimension.Add(f, nv);
                    }
                }
                else
                {
                    if (NetDimension.ContainsKey(0))
                    {
                        if (NetDimension[0].Players.Contains(uniq) == false)
                        {
                            NetDimension[0].Players.Add(uniq);
                        }
                    }
                    else
                    {
                        WorldList wlist = new WorldList();
                        wlist.Players.Add(uniq);
                        NetDimension.Add(0, wlist);
                    }
                }

                Debug.LogError(datadecompress);
                DimensionGeral = 0;
                Ready = true;

                #region SendRequestData
                var RequestData = MyPeer.CreateMessage();
                RequestData.Write((byte)DataType.RequestStartData);
                Client.SendMessage(RequestData, NetDeliveryMode);
                #endregion

                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].OnConnect();
                }

                foreach (var item in MyPeer.Connections)
                {
                    ConnectionList.Add(item.Peer.UniqueIdentifier, item);
                }

                Debug.LogError("Ready To Play...");
            }
            else if (type == DataType.Instantiate_PoolD)
            {
                long uniq = inc.ReadVariableInt64();
                inc.ReadInt32(out int dimension);
                inc.ReadInt32(out int lastdimension);

                inc.ReadString(out string data);
                string datadecompress = CompressString.StringCompressor.DecompressString(data);
                NetViewSerializer[] net = JsonHelper.FromJson<NetViewSerializer>(datadecompress);

                foreach (NetViewSerializer nv in net)
                {
                    GameObject Objc = GameObject.Instantiate(PregabsList.Prefabs[nv.PrefabID], new Vector3(nv.p_x, nv.p_y, nv.p_z), new Quaternion(nv.r_x, nv.r_y, nv.r_z, 1));
                    Objc.GetComponent<NetworkObj>().SetID(nv.ViewID, nv.PrefabID, nv.Owner);
                    Objc.GetComponent<NetworkObj>().Dimension = nv.Dimension;
                    NetworkViews.Add(nv.ViewID, Objc.GetComponent<NetworkObj>());
                }

                if (NetDimension.ContainsKey(dimension))
                {
                    if (NetDimension[dimension].Players.Contains(uniq) == false)
                    {
                        NetDimension[dimension].Players.Add(uniq);
                    }
                }
                else
                {
                    WorldList wlist = new WorldList();
                    wlist.Players.Add(uniq);
                    NetDimension.Add(dimension, wlist);
                }

                if (NetDimension.ContainsKey(lastdimension))
                {
                    NetDimension[lastdimension].Players.Remove(uniq);

                    if (NetDimension[lastdimension].Players.Count <= 0)
                    {
                        NetDimension.Remove(lastdimension);
                    }
                }
            }
            else if (type == DataType.CloseConnection)
            {
                Disconnect();

                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].OnDisconnect();
                }
            }
            else if (type == DataType.ExitDimension)
            {
                inc.ReadInt32(out int dimension);

                if (NetDimension.ContainsKey(dimension))
                {
                    if (NetDimension[dimension].Players.Count > 0)
                    {
                        NetDimension[dimension].Players.Remove(inc.SenderConnection.RemoteUniqueIdentifier);
                    }
                }
            }
            else if (type == DataType.ServerStop)
            {
                Disconnect();

                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].OnDisconnect();
                }
            }
        }

        /// <summary>
        /// (server only) create dimension, like a channel, is good for other worlds, or big world
        /// </summary>
        /// <param name="dimension_id"></param>
        public static void CreateDimension(int dimension_id)
        {
            WorldList wlist = new WorldList();
            NetDimension.Add(dimension_id, wlist);
        }

        /// <summary>
        /// (server only) Destroy a dimension and all the player inside, and move to another dimension
        /// </summary>
        /// <param name="dimension_id"></param>
        public static void DestroyDimension(int dimension_id, int tomove_dimension_id)
        {
            if (NetDimension.ContainsKey(dimension_id))
            {
                NetDimension.Remove(dimension_id);
            }

            ///move dimension logic, send data etc. soon
        }

        /// <summary>
        /// (client and server) Move player to another dimension.
        /// </summary>
        /// <param name="player_id"></param>
        /// <param name="dimension_id"></param>
        public static void ChangeDimension(long player_id, int dimension_id)
        {
            //all logic of move player to another dimension, send data etc.
        }

        public static NetworkObj[] GetAutomaticView(int dimension)
        {
            List<NetworkObj> obj = new List<NetworkObj>();

            foreach (var kvp in NetworkViews.ToArray())
            {
                if (NetworkViews[kvp.Key].IdMode == IdMode.AutomaticId)
                {
                    if (NetworkViews[kvp.Key].Dimension == dimension)
                    {
                        obj.Add(NetworkViews[kvp.Key]);
                    }
                }
            }

            return obj.ToArray();
        }

        #region RPC_Type_Receive

        internal static void RPC_All(NetIncomingMessage inc)
        {
            inc.ReadString(out string funcname);
            inc.ReadInt32(out int viewidd);
            inc.ReadInt32(out int d);

            NetworkObj Net = NetworkViews[viewidd];

            object[] obj = Net.Execute(funcname, inc);

            var om = Network.MyPeer.CreateMessage();
            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(viewidd);

            DoData(om, obj);

            foreach (var item in GetConnections(NetDimension[d].Players.ToArray()))
            {
                Server.SendMessage(om, item, NetDeliveryMode);
            }
        }

        internal static void RPC_AllOwner(NetIncomingMessage inc)
        {
            inc.ReadString(out string funcname);
            inc.ReadInt32(out int viewidd);
            inc.ReadInt32(out int d);
            //inc.ReadInt64();

            NetworkObj Net = NetworkViews[viewidd];

            object[] obj = Net.Execute(funcname, inc);

            var om = Network.MyPeer.CreateMessage();
            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(viewidd);

            DoData(om, obj);

            List<NetConnection> listanet = new List<NetConnection>(GetConnections(NetDimension[d].Players.ToArray()));
            listanet.Remove(GetConnection(Net.Owner));

            Server_SendToAll(om, listanet.ToArray(), NetDeliveryMode);
        }

        internal static void RPC_Owner(NetIncomingMessage inc)
        {
            inc.ReadString(out string funcname);
            inc.ReadInt32(out int viewidd);
            NetworkObj Net = NetworkViews[viewidd];
            
            object[] obj = Net.ExecuteNo(funcname, inc);


            var om = Network.MyPeer.CreateMessage();
            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(viewidd);

            DoData(om, obj);

            Server.SendMessage(om, GetConnection(Net.Owner), inc.DeliveryMethod);
        }

        internal static void RPC_ALLDimension(NetIncomingMessage inc)
        {
            inc.ReadString(out string funcname);
            inc.ReadInt32(out int viewidd);

            NetworkObj Net = NetworkViews[viewidd];

            object[] obj = Net.Execute(funcname, inc);


            var om = Network.MyPeer.CreateMessage();
            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(viewidd);

            DoData(om, obj);

            Server.SendToAll(om, inc.DeliveryMethod);
        }

        internal static NetOutgoingMessage DoData(NetOutgoingMessage om, object[] param)
        {
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i].GetType() == typeof(string))
                {
                    om.Write((string)param[i]);
                }
                else if (param[i].GetType() == typeof(int))
                {
                    om.Write((int)param[i]);
                }
                else if (param[i].GetType() == typeof(bool))
                {
                    om.Write((bool)param[i]);
                }
                else if (param[i].GetType() == typeof(float))
                {
                    om.Write((float)param[i]);
                }
                else if (param[i].GetType() == typeof(Vector3))
                {
                    Vector3 vec = (Vector3)param[i];

                    om.Write(vec.x);
                    om.Write(vec.y);
                    om.Write(vec.z);
                }
                else if (param[i].GetType() == typeof(Vector2))
                {
                    Vector2 vec = (Vector2)param[i];

                    om.Write(vec.x);
                    om.Write(vec.y);
                }
                else if (param[i].GetType() == typeof(Quaternion))
                {
                    Quaternion vec = (Quaternion)param[i];

                    om.Write(vec.x);
                    om.Write(vec.y);
                    om.Write(vec.z);
                }
            }

            return om;
        }

        #endregion

        #region RPC_Type_Local

        internal static void RPC_All(string funcname, int viewid, int d, object[] param)
        {
            NetworkObj Net = NetworkViews[viewid];

            Net.ExecuteServer(funcname, param);

            var om = Network.MyPeer.CreateMessage();
            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(viewid);

            DoData(om, param);

            List<NetConnection> listanet = new List<NetConnection>(GetConnections(NetDimension[d].Players.ToArray()));
            listanet.Remove(GetConnection(Server.UniqueIdentifier));

            Server_SendToAll(om, listanet.ToArray(), NetDeliveryMode);
        }

        internal static void RPC_AllOwner(string funcname, int viewid, int d, object[] param)
        {
            NetworkObj Net = NetworkViews[viewid];

            if (Net.Owner != MyPeer.UniqueIdentifier)
            {
                Net.ExecuteServer(funcname, param);
            }

            var om = Network.MyPeer.CreateMessage();
            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(viewid);

            DoData(om, param);


            List<NetConnection> listanet = new List<NetConnection>(GetConnections(NetDimension[d].Players.ToArray()));
            listanet.Remove(GetConnection(Net.Owner));

            Server_SendToAll(om, listanet.ToArray(), NetDeliveryMode);
        }

        internal static void RPC_Owner(string funcname, int viewid, object[] param)
        {
            NetworkObj Net = NetworkViews[viewid];

            var om = Network.MyPeer.CreateMessage();
            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(viewid);

            DoData(om, param);

            Server.SendMessage(om, GetConnection(Net.Owner), Net.DefaultNetDeliveryMethod);
        }

        internal static void RPC_ALLDimension(string funcname, int viewid, object[] param)
        {
            NetworkObj Net = NetworkViews[viewid];
            Net.ExecuteServer(funcname, param);

            var om = Network.MyPeer.CreateMessage();
            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(viewid);

            DoData(om, param);

            Server.SendToAll(om, Net.DefaultNetDeliveryMethod);
        }

        internal static void RPC_Server(string funcname, int viewid, object[] param)
        {
            NetworkObj Net = NetworkViews[viewid];
            Net.ExecuteServer(funcname, param);
        }

        #endregion
    }

    /// <summary>
    /// Class to execute a rpc functions
    /// </summary>
    public class CallFunc
    {
        public object obj = null;
        public MethodInfo func;
        public ParameterInfo[] parameters;

        /// <summary>
        /// Execute this function with the specified number of parameters.
        /// </summary>

        public object Execute(params object[] pars)
        {
            if (func == null) return null;
            if (parameters == null)
                parameters = func.GetParameters();

            try
            {
                return (parameters.Length == 1 && parameters[0].ParameterType == typeof(object[])) ?
                    func.Invoke(obj, new object[] { pars }) :
                    func.Invoke(obj, pars);
            }
            catch (System.Exception ex)
            {
                if (ex.GetType() == typeof(System.NullReferenceException)) return null;
                Debug.LogException(ex);
                return null;
            }
        }
    }

    /// <summary>
    /// MainClass for Network callback, with UnityEngine.MonoBehaviour.
    /// </summary>
    public class DarckMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// (server only) Just used for you know when a player connect, if you want send data, use DarckMonoBehaviour.OnPlayerRequestStartData.
        /// </summary>
        /// <param name="Peer"></param>
        public virtual void OnPlayerConnect(NetConnection Peer) { }
        /// <summary>
        /// (server only) Just used for you know when a player disconnect, you cant sand any data for player with this.
        /// </summary>
        /// <param name="Peer"></param>
        public virtual void OnPlayerDisconnect(NetConnection Peer) { }
        public virtual void OnServerStart() { }
        public virtual void OnServerClose() { }
        public virtual void OnServerAfterClose() { }
        /// <summary>
        /// (client only) call when client finished your connection setup, and ready to play, can send data.
        /// </summary>
        public virtual void OnConnect() { }
        /// <summary>
        /// (client only), used when network is finished, cant sand any data from this.
        /// </summary>
        public virtual void OnDisconnect() { }

        /// <summary>
        /// (server only) used for handle all data you want when player is ready to play.
        /// </summary>
        /// <param name="Peer"></param>
        public virtual void OnPlayerRequestStartData(NetConnection Peer) { }
        /// <summary>
        /// (server only)Used to check data sended from client and aprove connection, Just use one of this, in one script.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="player_connection">Player_Sender.Approve() : to aprove connection, Player_Sender.Deny() : to deny</param>
        public virtual void PlayerApproval(string data, NetConnection player_connection) { }

        void OnEnable()
        {
            Network.Events.Add(this);
        }

        void OnDisable()
        {
            Network.Events.Remove(this);
        }
    }

    [Serializable]
    public class NetViewSerializer
    {
        public int Dimension = 0;
        public int PrefabID = 0;
        public long Owner;
        public int ViewID = 0;
        public IdMode IdMode;

        public float p_x;
        public float p_y;
        public float p_z;

        public float r_x;
        public float r_y;
        public float r_z;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }


        #region ArrayCord

        public static T[,] FromJsonChunk<T>(string json)
        {
            WrapperChunk<T> wrapper = JsonUtility.FromJson<WrapperChunk<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[,] array)
        {
            WrapperChunk<T> wrapper = new WrapperChunk<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }
        #endregion

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }

        [System.Serializable]
        private class WrapperChunk<T>
        {
            public T[,] Items;
        }
    }

    [Serializable]
    public class WorldList
    {
        public List<long> Players = new List<long>();
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

    public enum DataType : byte
    {
        RPC = 0,
        Destroy = 1,
        Destroy_Player = 2,
        Instantiate = 3,
        Instantiate_Pool = 4,
        CloseConnection = 5,
        ChangeDimension = 6,
        Instantiate_PoolD = 7,
        ExitDimension = 8,

        RPC_All = 9,
        RPC_AllOwner = 10,
        RPC_Owner = 11,
        RPC_ALLDimension = 12,

        ServerStop = 13,
        RequestStartData = 14
    }

}

/// <summary>
/// Used For Store Vector data, compatible with serialization
/// </summary>
[Serializable]
public struct DataVector3
{
    public float x;
    public float y;
    public float z;

    public static DataVector3 zero { get { return new DataVector3(0, 0, 0); } }
    public static DataVector3 one { get { return new DataVector3(1, 1, 1); } }

    public DataVector3(float X, float Y, float Z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public DataVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    /// <summary>
    /// Tranfosrm the VectorData To unity Vector
    /// </summary>
    /// <returns></returns>
    public Vector3 ToUnityVector()
    {
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Get Vector3Data formated in string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "DataVector3 (" + x + ", " + y + ", " + z + ")";
    }
}

[Serializable]
public struct DataVector3Int
{
    public int x;
    public int y;
    public int z;

    public DataVector3Int(float X, float Y, float Z)
    {
        x = (int)X;
        y = (int)Y;
        z = (int)Z;
    }

    public DataVector3Int(Vector3 vector)
    {
        x = (int)vector.x;
        y = (int)vector.y;
        z = (int)vector.z;
    }

    /// <summary>
    /// Tranfosrm the VectorData To unity Vector
    /// </summary>
    /// <returns></returns>
    public Vector3 ToUnityVector()
    {
        return new Vector3(x, y, z);
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class RPC : Attribute
{

}