using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWorld : MonoBehaviour
{
    public GameObject ChunkGO;
    public int RenderDistance = 10;
    Dictionary<Vector3, MenuChunk> chunkMap = new Dictionary<Vector3, MenuChunk>();
    void Start()
    {
        string seed = SystemInfo.deviceName + SystemInfo.graphicsDeviceName + SystemInfo.operatingSystem;
        Game.GameManager.Seed = seed.GetHashCode() + Random.Range(-9999, 9999);
        Game.GameManager.Small_Seed = Game.GameManager.Seed;
        FindChunksToLoad();
    }

    public void FindChunksToLoad()
    {
        int xPos = (int)transform.position.x;
        int zPos = (int)transform.position.z;

        for (int i = -RenderDistance; i < RenderDistance; i++)
        {
            for (int z = -RenderDistance; z < RenderDistance; z++)
            {
                MakeChunkAt(xPos + i, zPos + z);
            }
        }
    }

    void MakeChunkAt(int x, int z)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        z = Mathf.FloorToInt(z / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Vector3(x, 0, z)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, 0, z), Quaternion.identity);
            go.SetActive(true);
            chunkMap.Add(new Vector3(x, 0, z), go.GetComponent<MenuChunk>());
        }
    }
}
