using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;

public class ChunkGenerator : MonoBehaviour
{
    /*public Transform PlayerRoot;
    public int renderDistance;
    public int ThickRate = 1;

    public Vector3Int PlayerPos;

    public GameObject ChunkPrefab;

    private Thread worldGenerator;
    Dictionary<Vector3Int, ChunkData> chunks = new Dictionary<Vector3Int, ChunkData>();
    Queue<Vector3Int> pendingDeletions = new Queue<Vector3Int>();
    Queue<ChunkData> pendingchunks = new Queue<ChunkData>();


    public static int Snap(float i, int v) => (int)(Mathf.Round(i / v) * v);

    private void Awake()
    {
        PlayerRoot = Game.GameManager.Player.PlayerObj.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerPos = new Vector3Int((int)PlayerRoot.position.x, (int)PlayerRoot.position.y, (int)PlayerRoot.position.z);
        worldGenerator = new Thread(new ThreadStart(MadeChunks));
        worldGenerator.Name = "WorldGenerator";
        worldGenerator.IsBackground = true;
        worldGenerator.Start();
    }

    private void MadeChunks()
    {
        while (true)
        {
            //Debug.Log("Hello for another thread!");
            Vector3Int Player = new Vector3Int(Snap(PlayerPos.x, Chunk.Size), 0, Snap(PlayerPos.z, Chunk.Size));
            int minX = Player.x - renderDistance;
            int maxX = Player.x + renderDistance;
            int minZ = Player.z - renderDistance;
            int maxZ = Player.z + renderDistance;

            foreach (var chunk in chunks.Values.ToArray())
            {
                if (chunk != null)
                {
                    Vector3Int vector = chunk.position;
                    if (vector.x > maxX || vector.x < minX || vector.z > maxZ || vector.z < minZ)
                    {
                        pendingDeletions.Enqueue(vector);
                    }
                }
            }

            for (int z = minZ; z < maxZ; z += Chunk.Size)
            {
                for (int x = minX; x < maxX; x += Chunk.Size)
                {
                    Thread.Sleep(ThickRate);
                    Vector3Int vector = new Vector3Int(x, 0, z);
                    ChunkData chunk = null;
                    chunks.TryGetValue(vector, out chunk);
                    if (chunk == null)
                    {
                        ChunkData nchunk = new ChunkData();
                        nchunk.position = vector;
                        chunks.Add(vector, nchunk);
                        pendingchunks.Enqueue(nchunk);
                    }
                }
            }
        }
    }

    void Update()
    {
        PlayerPos = new Vector3Int((int)PlayerRoot.position.x, (int)PlayerRoot.position.y, (int)PlayerRoot.position.z);

        while (pendingchunks.Count > 0)
        {
            ChunkData chunk = pendingchunks.Dequeue();
            GameObject obj = Instantiate(ChunkPrefab, new Vector3Int(chunk.position.x, chunk.position.y, chunk.position.z), Quaternion.identity);
            obj.name = "Chunk : " + chunk.position.x + " : " + chunk.position.z;

            chunks.TryGetValue(new Vector3Int(chunk.position.x, chunk.position.y, chunk.position.z), out ChunkData chunkk);

            if (chunkk != null)
            {
                chunkk.obj = obj;
                chunkk.isReady = true;
            }
        }
        while (pendingDeletions.Count > 0)
        {
            Vector3Int vector = pendingDeletions.Dequeue();
            ChunkData chunk;
            if (chunks.TryGetValue(vector, out chunk))
            {
                Destroy(chunk.obj);
                chunks.Remove(vector);
            }

        }
    }*/
}
