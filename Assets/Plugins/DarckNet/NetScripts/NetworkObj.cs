using System;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using System.Reflection;
using System.Linq;

public enum IdMode : byte
{
    Manual_Id, AutomaticId
}

namespace DarckNet
{
    [Serializable]
    public class NetworkObj : MonoBehaviour
    {
        internal int Dimension = 0;
        internal int PrefabID = 0;
        public long Owner;
        public int ViewID = 0;
        public IdMode IdMode = IdMode.AutomaticId;
        public NetDeliveryMethod DefaultNetDeliveryMethod = NetDeliveryMethod.Unreliable;
        public bool isMine { get { bool Mine = false; if (Network.Runing) { if (Owner == Network.MyPeer.UniqueIdentifier) { Mine = true; } else { Mine = false; } } else { Mine = false; } return Mine; } }

        [NonSerialized]
        internal Dictionary<int, CallFunc> mDict0 = new Dictionary<int, CallFunc>();
        [NonSerialized]
        internal Dictionary<string, CallFunc> mDict1 = new Dictionary<string, CallFunc>();
        [NonSerialized]
        internal bool verifyd = false;

        void Awake()
        {
            if (IdMode == IdMode.Manual_Id)
            {
                PrefabID = -1;
                Network.NetworkViews.Add(ViewID, this);
            }
        }

        /// <summary>
        /// Configur This netviwer
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Prefabid"></param>
        /// ///<param name="uniqui"></param>
        public void SetID(int Id, int Prefabid, long uniqui)
        {
            ViewID = Id;
            PrefabID = Prefabid;
            Owner = uniqui;
        }

        /// <summary>
        /// Configur This netviwer
        /// </summary>
        ///<param name="Id"></param>
        ///<param name="uniqui"></param>
        public void SetID(int Id, long uniqui)
        {
            ViewID = Id;
            Owner = uniqui;
        }

        #region RPC-spicified-NetDeliveryMethod
        /// <summary>
        /// Send RPC with RPCmode
        /// </summary>
        /// <param name="funcname"></param>
        /// <param name="Mode"></param>
        /// <param name="param"></param>
        public void RPC(string funcname, RPCMode Mode, NetDeliveryMethod DeliveryMethod, params object[] param)
        {
            if (Network.Ready)
            {
                if (Network.NetworkViews.ContainsKey(ViewID))
                {
                    SendRPC(funcname, Mode, DeliveryMethod, param);
                }
            }
        }

        /// <summary>
        /// Send RPC to Specifique User(NetPeer)
        /// </summary>
        /// <param name="funcname"></param>
        /// <param name="player"></param>
        /// <param name="param"></param>
        public void RPC(string funcname, NetConnection player, NetDeliveryMethod DeliveryMethod, params object[] param)
        {
            if (Network.Ready)
            {
                if (Network.NetworkViews.ContainsKey(ViewID))
                {
                    SendRPC(funcname, player, DeliveryMethod, param);
                }
            }
        }
        #endregion

        #region RPC-NetDeliveryMethod-Default
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
                if (Network.NetworkViews.ContainsKey(ViewID))
                {
                    SendRPC(funcname, Mode, DefaultNetDeliveryMethod, param);
                }
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
                if (Network.NetworkViews.ContainsKey(ViewID))
                {
                    SendRPC(funcname, player, DefaultNetDeliveryMethod, param);
                }
            }
        }

        #endregion

        void SendRPC(string funcname, RPCMode Mode, NetDeliveryMethod DeliveryMethod, params object[] param)
        {
            var om = Network.MyPeer.CreateMessage();
            if (Mode == RPCMode.All)
            {
                om.Write((byte)DataType.RPC_All);

                om.Write(funcname);
                om.Write(ViewID);
                om.Write(Dimension);

                DoData(om, funcname, param);

                if (Network.IsClient)
                {
                    Network.Client.SendMessage(om, DeliveryMethod);
                }
                else
                {
                    Network.RPC_All(funcname, ViewID, Dimension, param);
                }
            }
            else if (Mode == RPCMode.AllNoDimension)
            {
                om.Write((byte)DataType.RPC_ALLDimension);

                om.Write(funcname);
                om.Write(ViewID);

                DoData(om, funcname, param);

                if (Network.IsClient)
                {
                    Network.Client.SendMessage(om, DeliveryMethod);
                }
                else
                {
                    Network.RPC_ALLDimension(funcname, ViewID, param);
                }
            }
            else if (Mode == RPCMode.AllNoOwner)
            {
                om.Write((byte)DataType.RPC_AllOwner);

                om.Write(funcname);
                om.Write(ViewID);
                om.Write(Dimension);

                DoData(om, funcname, param);

                if (Network.IsClient)
                {
                    Network.Client.SendMessage(om, DeliveryMethod);
                }
                else
                {
                    Network.RPC_AllOwner(funcname, ViewID, Dimension, param);
                }
            }
            else if (Mode == RPCMode.Server)
            {
                om.Write((byte)DataType.RPC);

                om.Write(funcname);
                om.Write(ViewID);

                DoData(om, funcname, param);

                if (Network.IsClient == true)
                {
                    Network.Client.SendMessage(om, DeliveryMethod);
                }
                else
                {
                    Network.RPC_Server(funcname, ViewID, param);
                }
            }
            else if (Mode == RPCMode.Owner)
            {
                om.Write((byte)DataType.RPC_Owner);

                om.Write(funcname);
                om.Write(ViewID);

                DoData(om, funcname, param);

                if (Network.IsClient)
                {
                    Network.Client.SendMessage(om, DeliveryMethod);
                }
                else
                {
                    Network.RPC_Owner(funcname, ViewID, param);
                }
            }
        }

        void SendRPC(string funcname, NetConnection player, NetDeliveryMethod DeliveryMethod, params object[] param)
        {
            var om = Network.MyPeer.CreateMessage();

            om.Write((byte)DataType.RPC);

            om.Write(funcname);
            om.Write(ViewID);

            DoData(om, funcname, param);

            Network.MyPeer.SendMessage(om, player, DeliveryMethod);

            return;
        }

        NetOutgoingMessage DoData(NetOutgoingMessage om, string funcname, object[] param)
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

        internal object[] Execute(string funcName, NetIncomingMessage msg)
        {
            //if (mParent != null) return mParent.Execute(funcName, parameters);

            if (verifyd == false)
            {
                RebuildMethodList();
            }

            CallFunc ent;

            if (mDict1.TryGetValue(funcName, out ent))
            {
                if (ent.parameters == null)
                    ent.parameters = ent.func.GetParameters();

                try
                {
                    List<object> objects = new List<object>();

                    for (int i = 0; i < ent.parameters.Length; i++)
                    {
                        objects.Add(ReadArgument(msg, ent.parameters[i].ParameterType));
                    }

                    ent.func.Invoke(ent.obj, objects.ToArray());
                    return objects.ToArray();
                }
                catch (System.Exception ex)
                {
                    if (ex.GetType() == typeof(System.NullReferenceException)) return null;
                    Debug.LogException(ex);
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Exute RPC on server side, or for host, not for the client
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="peer"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        internal void ExecuteServer(string funcName, object[] param)
        {
            //if (mParent != null) return mParent.Execute(funcName, parameters);
            if (verifyd == false)
            {
                RebuildMethodList();
            }

            CallFunc ent;

            if (mDict1.TryGetValue(funcName, out ent))
            {
                if (ent.parameters == null)
                    ent.parameters = ent.func.GetParameters();
                try
                {
                    List<object> objects = new List<object>();
                    for (int i = 0; i < ent.parameters.Length; i++)
                    {
                        if (ent.parameters[i].ParameterType == typeof(DNetConnection))
                        {
                            DNetConnection dnet = new DNetConnection();
                            dnet.unique = Network.MyPeer.m_uniqueIdentifier;
                            objects.Add(dnet);
                        }
                        else
                        {
                            objects.Add(param[i]);
                        }
                    }
                    //Debug.Log(param[2].ToString());

                    ent.func.Invoke(ent.obj, objects.ToArray());
                }
                catch (System.Exception ex)
                {
                    //if (ex.GetType() == typeof(System.NullReferenceException));
                    Debug.LogException(ex);
                }
            }
        }

        internal object[] ExecuteNo(string funcName, NetIncomingMessage msg)
        {
            //if (mParent != null) return mParent.Execute(funcName, parameters);

            if (verifyd == false)
            {
                RebuildMethodList();
            }

            CallFunc ent;

            if (mDict1.TryGetValue(funcName, out ent))
            {
                if (ent.parameters == null)
                    ent.parameters = ent.func.GetParameters();

                try
                {
                    List<object> objects = new List<object>();

                    for (int i = 0; i < ent.parameters.Length; i++)
                    {
                        objects.Add(ReadArgument(msg, ent.parameters[i].ParameterType));
                    }
                    return objects.ToArray();
                }
                catch (System.Exception ex)
                {
                    if (ex.GetType() == typeof(System.NullReferenceException)) return null;
                    Debug.LogException(ex);
                    return null;
                }
            }
            return null;
        }

        void RebuildMethodList()
        {
            mDict0.Clear();
            mDict1.Clear();
            MonoBehaviour[] mbs = GetComponentsInChildren<MonoBehaviour>(true);

            for (int i = 0, imax = mbs.Length; i < imax; ++i)
            {
                MonoBehaviour mb = mbs[i];
                System.Type type = mb.GetType();

                MethodInfo[] methods = type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance);

                for (int b = 0, bmax = methods.Length; b < bmax; ++b)
                {
                    MethodInfo method = methods[b];

                    if (method.IsDefined(typeof(RPC), true))
                    {
                        CallFunc ent = new CallFunc();
                        ent.obj = mb;
                        ent.func = method;

                        RPC tnc = (RPC)ent.func.GetCustomAttributes(typeof(RPC), true)[0];

                        verifyd = true;
                        mDict1[method.Name] = ent;
                    }
                }
            }
        }

        static object ReadArgument(NetIncomingMessage msg, Type type)
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
                DNetConnection dnet = new DNetConnection();

                dnet.unique = msg.SenderConnection.m_remoteUniqueIdentifier;

                return dnet;
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

        /// <summary>
        /// Use to Debug, Get some information about this Networkviwer
        /// </summary>
        /// <returns></returns>
        public string NetToString()
        {
            return "NetView = ViewId :" + ViewID + " : " + "Owner : " + Owner + " : " + "PrefabId : " + PrefabID;
        }
    }

    public static class NetBit
    {
        /// <summary>
        /// RPC Read Vector3
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Vector3 ReadVector3(NetIncomingMessage msg)
        {
            Vector3 vec = new Vector3();

            vec.x = msg.ReadFloat();
            vec.y = msg.ReadFloat();
            vec.z = msg.ReadFloat();

            return vec;
        }

        /// <summary>
        /// RPC Read Vector2
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Vector2 ReadVector2(NetIncomingMessage msg)
        {
            Vector2 vec = new Vector2();

            vec.x = msg.ReadFloat();
            vec.y = msg.ReadFloat();

            return vec;
        }

        /// <summary>
        /// RPC Read Quaternion
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Quaternion ReadQuaternion(NetIncomingMessage msg)
        {
            Quaternion vec = new Quaternion();

            vec.x = msg.ReadFloat();
            vec.y = msg.ReadFloat();
            vec.z = msg.ReadFloat();
            vec.w = 1;

            return vec;
        }

    }

    /// <summary>
    /// Sender class to send rpc back to the sender
    /// </summary>
    public struct DNetConnection
    {
        /// <summary>
        /// UniqueId of the sender connection
        /// </summary>
        public long unique;

        /// <summary>
        /// Get your connection to send a menssage to this connection, if you are the server you can't use this
        /// </summary>
        public NetConnection NetConnection { get { return Network.GetConnection(unique); } private set { } }
        /// <summary>
        /// Check if you send this messagen for you, if you is the sender, i gone return true.
        /// </summary>
        public bool IsMine { get { if (unique == Network.MyPeer.UniqueIdentifier) { return true; } else { return false; } } private set { } }
    }
}