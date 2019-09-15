using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesteScript : MonoBehaviour
{
    public NetWorkView Net;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Net.RPC("Teste", DarckNet.RPCMode.All, "ALL");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Net.RPC("Teste", DarckNet.RPCMode.AllNoOwner, "ALL NO OWNER");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Net.RPC("Teste", DarckNet.RPCMode.Owner, "OWNER");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Net.RPC("TesteServer", DarckNet.RPCMode.Server, "SERVER");
        }
    }

    [RPC]
    void Teste(string teste)
    {
        Debug.LogError(teste);
    }

    [RPC]
    void TesteServer(string teste, DarckNet.DNetConnection net)
    {
        Debug.LogError(teste);
        Net.RPC("TesteClient", net.NetConnection, "SERVER CALLBACK");
    }

    [RPC]
    void TesteClient(string teste)
    {
        Debug.LogError(teste);
    }
}