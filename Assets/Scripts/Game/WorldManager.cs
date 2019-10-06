using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    public static WorldManager This;
    public GameObject playerobj;

    void Awake()
    {
        This = this;
    }

    public void StartNewWorld(string worldto)
    {
        SceneManager.LoadSceneAsync(worldto);
    }

    public void ChangeWorld(string worldto, int px, int py, int pz, int world_id)
    {
        if (Game.GameManager.MyPlayer.MyObject)
        {
            RemovePlayerWorld(Game.GameManager.MyPlayer.MyObject);
        }
        SpawnPlayer(px, py, pz, world_id);
        SceneManager.LoadSceneAsync(worldto);
    }

    public void SpawnPlayer(int x, int y, int z, int world_id)
    {
        NetManager.Instance.SetupPlayer(playerobj, x, y, z, world_id);
    }

    public void RemovePlayerWorld(GameObject player)
    {
        DarckNet.Network.Destroy(player);
    }
}