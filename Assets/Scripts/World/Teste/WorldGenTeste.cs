using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenTeste : MonoBehaviour
{

    public int ChunkSize = 16;
    public int RenderDistance = 4;
    public GameObject ChunkGO;
    Dictionary<Vector3, GameObject> chunkMap = new Dictionary<Vector3, GameObject>();
    public int semnomex = 0;
    public int semnomey = 0;
    public int semnomez = 0;
    void Start()
    {
        
    }



    void Update()
    {
        semnomex = (int)transform.position.x;
        semnomey = (int)transform.position.y;
        semnomez = (int)transform.position.z;

        for (int i = -RenderDistance; i < RenderDistance; i++)
        {
            for (int y = -RenderDistance; y < RenderDistance; y++)
            {
                for (int z = -RenderDistance; z < RenderDistance; z++)
                {
                    MakeChunkAt(semnomex + i, semnomey + y, semnomez + z);
                }
            }
        }

        DeleteChunk();
    }

    void MakeChunkAt(int x, int y, int z)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        y = Mathf.FloorToInt(y / (float)Chunk.Size) * Chunk.Size;
        z = Mathf.FloorToInt(z / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Vector3(x, y, z)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, y, z), Quaternion.identity);

            chunkMap.Add(new Vector3(x, y, z), go);
        }
    }

    void DeleteChunk()
    {
        List<GameObject> DeleteChuks = new List<GameObject>(chunkMap.Values);
        Queue<GameObject> deletechuks = new Queue<GameObject>();

        for (int i = 0; i < DeleteChuks.Count; i++)
        {
            float Distanc = Vector3.Distance(transform.position, DeleteChuks[i].transform.position);

            if (Distanc > RenderDistance * Chunk.Size)
            {
                deletechuks.Enqueue(DeleteChuks[i]);
            }

            while (deletechuks.Count > 0)
            {
                GameObject chuks = deletechuks.Dequeue();
                chunkMap.Remove(chuks.transform.position);
                Destroy(chuks.gameObject);
                Debug.Log("ok");
            }
        }
    }
}