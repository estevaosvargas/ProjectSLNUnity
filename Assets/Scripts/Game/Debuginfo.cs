using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debuginfo : MonoBehaviour
{
    public Text Version;
    public Text WorldSeed;
    public Text EntityLoade;
    public Text ChunksLoade;
    public Text Biome;
    public Text CurrentBlock;
    public Text Position;

    void Start()
    {

    }

    void Update()
    {
        if (Game.GameManager.CurrentPlayer.MyObject != null)
        {
            Version.text = Application.productName + " On ("+ Game.GameManager.Version + ")" + " On (" + SystemInfo.graphicsDeviceName + ") - (" + SystemInfo.operatingSystem + ")";
            WorldSeed.text = "WorldSeed : " + Game.GameManager.Seed;
            EntityLoade.text = "Entity : " + Game.Entity_viewing.Count;
            ChunksLoade.text = "Chunks Loaded : " + Game.WorldGenerator.ChunksList.Count;
            Biome.text = "Biome : " + Game.GameManager.CurrentPlayer.MyPlayerMove.NetStats.CurrentBiome;
            CurrentBlock.text = "Tile : " + Game.GameManager.CurrentPlayer.MyPlayerMove.NetStats.CurrentTile;
            Position.text = "X:" + Game.GameManager.CurrentPlayer.MyObject.transform.position.x + ", Y:" + Game.GameManager.CurrentPlayer.MyObject.transform.position.y + ", Z:" + Game.GameManager.CurrentPlayer.MyObject.transform.position.z;
        }
    }
}