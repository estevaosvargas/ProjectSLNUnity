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

    public void ChangeWorld(string worldto, float px, float py)
    {
        if (Game.GameManager.MyPlayer.MyObject)
        {
            RemovePlayerWorld(Game.GameManager.MyPlayer.MyObject);
        }
        SetUpPlayer(px, py);
        SceneManager.LoadSceneAsync(worldto);
    }

    public void SetUpPlayer(float x, float y)
    {
        NetManager.Instance.SetupPlayer(playerobj, x, y);
    }

    public void RemovePlayerWorld(GameObject player)
    {
        DarckNet.Network.Destroy(player);
    }
}