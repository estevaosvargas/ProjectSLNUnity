using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NetworkingManager : MonoBehaviour
{
    public InputField IPInput;
    public InputField PortInput;

    public GameObject Obj;
    public GameObject InstaSpawned;

    public Text Text01;

    void Start()
    {
        //RakNet.Network.Ini();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            InstaSpawned = DarckNet.Network.Instantiate(Obj, Vector3.zero, Quaternion.identity, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DarckNet.Network.Destroy(InstaSpawned);
        }

        if (DarckNet.Network.IsClient)
        {
            Text01.text = DarckNet.Network.PeerStat.ToString();
        }

        if (DarckNet.Network.IsServer)
        { 
            Text01.text = DarckNet.Network.PeerStat.ToString();
        }
        DarckNet.Network.Update();
    }

    private void OnDestroy()
    {
        DarckNet.Network.Disconnect();
    }

    public void Connect()
    {
        DarckNet.Network.Connect(IPInput.text, int.Parse(PortInput.text), "");
    }

    public void StartServer()
    {
        DarckNet.Network.Create(IPInput.text, int.Parse(PortInput.text), 100);
    }
}