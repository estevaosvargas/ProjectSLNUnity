using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour {

    public Dictionary<string, PlayerInfo> PlayerList;

    public static NetManager Instance;

    void Awake()
    {
        Instance = this;
        PlayerList = new Dictionary<string, PlayerInfo>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    /// <summary>
    /// Used for SetUp the player, Spawn/info/load-save etc.
    /// </summary>
    /// <param name="playerobj"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetupPlayer(GameObject playerobj, int x, int y, int z, int multiplayer_dimension)
    {
        DarckNet.Network.Instantiate(playerobj, new Vector3(x, y, z), Quaternion.identity, multiplayer_dimension);

        /*PlayerInfo playerinf = new PlayerInfo();
        playerinf.UserName = GameManager.Instance.UserName;
        playerinf.UserID = GameManager.Instance.UserId;
        playerinf.PlayerRoot = player.transform;
        //playerinf.Peer = DarckNet.Network.GetPeer(player.GetComponent<Networkviewr>().Owner);
        PlayerList.Add(playerinf.UserID, playerinf);*/
    }

    public void RemovePlayer(string id)
    {
        if (PlayerList.ContainsKey(id))
        {
            PlayerList.Remove(id);
        }
        else
        {
            return;
        }
    }

    public void RemovePlayer(PlayerInfo playerinf)
    {
        if (PlayerList.ContainsKey(playerinf.UserID))
        {
            PlayerList.Remove(playerinf.UserID);
        }
        else
        {
            return;
        }
    }
}
