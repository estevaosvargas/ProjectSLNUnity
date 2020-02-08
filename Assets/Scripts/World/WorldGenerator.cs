using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading;
using System.Linq;

public class WorldGenerator : MapManager
{
    public Transform SlectedBlock;
    public WorldType CurrentWorld;
    public int VillaPorcetage = 10;
    public Texture2D HeightTeste;
    public bool IsMainMenu = false;


    public Transform Player;
    public int renderDistance;
    public int ThickRate = 1;
    public Vector3Int PlayerPos;
    public bool WorldRuning = false;
    public GameObject ChunkGO;
    public GameObject SUN;

    private Thread worldGenerator;

    private System.Random randomValue;

    private Queue<Vector3Int> pendingDeletions = new Queue<Vector3Int>();
    private Queue<ChunkData> pendingchunks = new Queue<ChunkData>();

    Dictionary<Vector3, Chunk> chunkMap;
    Dictionary<Vector3, bool> ClientchunkMap;
    public List<Chunk> ChunksList = new List<Chunk>();

    [Header("TimeData")]
    public int h;
    public int d;
    public int m;

    public static int Snap(float i, int v) => (int)(Mathf.Round(i / v) * v);

    void Awake()
    {
        Game.WorldGenerator = this;
        chunkMap = new Dictionary<Vector3, Chunk>();
        ClientchunkMap = new Dictionary<Vector3, bool>();

        if (!IsMainMenu)
        {
            if (DarckNet.Network.IsServer)
            {
                DarckNet.Network.Instantiate(SUN, Vector3.zero, Quaternion.identity, World_ID);
                Debug.Log("SERVER: Sun Spawned");
            }

            randomValue = new System.Random(Game.GameManager.Seed);
        }
        else
        {
            
        }
    }

    void Start()
    {
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            Player = Game.GameManager.Player.RequestSpawnPlayer(new Vector3(0, 55, 0), World_ID).transform;
            Game.ConsoleInGame.LoadingScreen_Hide();
            //StartCoroutine("StrartGenerator");
        }
        if (!IsMainMenu)
        {
            WorldRuning = true;
            PlayerPos = new Vector3Int((int)Player.position.x, 0, (int)Player.position.z);
            worldGenerator = new Thread(new ThreadStart(MadeChunks));
            worldGenerator.Name = "WorldGenerator";
            worldGenerator.IsBackground = true;
            worldGenerator.Start();
        }
    }

    void Update()
    {
        PlayerPos = new Vector3Int((int)Player.position.x, 0, (int)Player.position.z);
        while (pendingchunks.Count > 0)
        {
            ChunkData chunk = pendingchunks.Dequeue();

            if (!chunkMap.ContainsKey(new Vector3Int(chunk.position.x, 0, chunk.position.z)))
            {
                GameObject obj = Instantiate(ChunkGO, new Vector3Int(chunk.position.x, 0, chunk.position.z), Quaternion.identity);
                obj.name = "Chunk : " + chunk.position.x + " : " + chunk.position.z;

                obj.GetComponent<Chunk>().position = new Vector3Int(chunk.position.x, 0, chunk.position.z);

                chunkMap.Add(new Vector3Int(chunk.position.x, 0, chunk.position.z), obj.GetComponent<Chunk>());
            }
        }
        while (pendingDeletions.Count > 0)
        {
            Vector3Int vector = pendingDeletions.Dequeue();
            if (chunkMap.TryGetValue(vector, out Chunk chunk))
            {
                Destroy(chunk.gameObject);
                chunkMap.Remove(vector);
            }
        }

#if Client
        if (MouselockFake.IsLock == false)
        {
            h = DataTime.Hora;
            d = DataTime.Dia;
            m = DataTime.Mes;
        }
#endif
    }

    private void OnDestroy()
    {
        WorldRuning = false;
        worldGenerator.Abort();
        worldGenerator = null;
    }

    public void Setplayer_data()
    {
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            Player = Game.GameManager.Player.PlayerObj.transform;
            Player.GetComponent<EntityPlayer>().World = this.transform;
        }
    }

    public void LoadNewChunks(Chunk current_chunk)
    {
        List<Chunk> chunks = new List<Chunk>();

        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x, (int)current_chunk.transform.position.z + 10));//Cima
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x, (int)current_chunk.transform.position.z - 10));//Baixo
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x + 10, (int)current_chunk.transform.position.z));//Direita
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x - 10, (int)current_chunk.transform.position.z));//Esquerda

        foreach (var item in chunks)
        {
            if (item != null)
            {
                item.RefreshChunkTile();
            }
        }
    }

    public override void OnRespawn()
    {
        Setplayer_data();
        base.OnRespawn();
    }

    private void MadeChunks()
    {
        while (WorldRuning)
        {
            Vector3Int PlayerP = new Vector3Int(Snap(PlayerPos.x, Chunk.Size), 0, Snap(PlayerPos.z, Chunk.Size));
            int minX = PlayerP.x - renderDistance;
            int maxX = PlayerP.x + renderDistance;
            int minZ = PlayerP.z - renderDistance;
            int maxZ = PlayerP.z + renderDistance;

            for (int z = minZ; z < maxZ; z += Chunk.Size)
            {
                for (int x = minX; x < maxX; x += Chunk.Size)
                {
                    Thread.Sleep(ThickRate);
                    Vector3Int vector = new Vector3Int(x + PlayerP.x, 0, z + PlayerP.z);

                    if (!chunkMap.ContainsKey(vector))
                    {
                        ChunkData nchunk = new ChunkData();
                        nchunk.position = vector;
                        pendingchunks.Enqueue(nchunk);
                    }
                }
            }

            Chunk[] chunks = chunkMap.Values.ToArray();

            foreach (var chunk in chunks)
            {
                Thread.Sleep(ThickRate);
                if (chunk != null)
                {
                    Vector3Int vector = new Vector3Int(chunk.position.x, chunk.position.y, chunk.position.z);
                    if (vector.x > maxX || vector.x < minX || vector.z > maxZ || vector.z < minZ)
                    {
                        pendingDeletions.Enqueue(vector);
                    }
                }
            }
            
        }
    }

    public Chunk GetChunkAt(int x, int z)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        z = Mathf.FloorToInt(z / (float)Chunk.Size) * Chunk.Size;

        Vector3 chunkpos = new Vector3(x, 0, z);

        if (chunkMap.ContainsKey(chunkpos))
        {
            return chunkMap[chunkpos];
        }
        else
        {
            return null;
        }
    }

    public Tile GetTileAt(int x, int z)
    {
        Chunk chunk = GetChunkAt(x, z);

        if (chunk != null)
        {
            return chunk.tiles[x - (int)chunk.transform.position.x, z - (int)chunk.transform.position.z];
        }
        return null;
    }

    public Vector3 GetPos(float x, float z)
    {
        int mx = Mathf.FloorToInt(x);
        int mz = Mathf.FloorToInt(z);

        Chunk chunk = GetChunkAt(mx, mz);

        if (chunk != null)
        {
            return new Vector3(mx - (int)chunk.transform.position.x, 0, mz - (int)chunk.transform.position.z);
        }
        return Vector3.zero;
    }

    public int RandomNumber(int Min, int Max)
    {
        return randomValue.Next(Min, Max);
    }

    public float RandomNumber()
    {
        return (float)randomValue.NextDouble();
    }

    [Obsolete]
    public void FindChunksToLoad()
    {
        if (Player)
        {
            int xPos = (int)Player.position.x;
            int zPos = (int)Player.position.z;

            /*for (int i = -RenderDistance; i < RenderDistance; i++)
            {
                for (int z = -RenderDistance; z < RenderDistance; z++)
                {
                    MakeChunkAt(xPos + i, zPos + z);
                }
            }*/
        }
    }

    [Obsolete]
    void MakeChunkAt(int x, int z)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        z = Mathf.FloorToInt(z / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Vector3(x, 0, z)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, 0, z), Quaternion.identity);
            go.SetActive(true);
            chunkMap.Add(new Vector3(x, 0,z), go.GetComponent<Chunk>());
        }
    }

    [Obsolete]
    public void DeleteChunk()
    {
        if (Player)
        {
            List<Chunk> DeleteChuks = new List<Chunk>(chunkMap.Values);
            Queue<Chunk> deletechuks = new Queue<Chunk>();

            for (int i = 0; i < DeleteChuks.Count; i++)
            {
                float Distanc = Vector3.Distance(Player.position, DeleteChuks[i].transform.position);

                if (Distanc > renderDistance * Chunk.Size)
                {
                    deletechuks.Enqueue(DeleteChuks[i]);
                }

                while (deletechuks.Count > 0)
                {
                    Chunk chuks = deletechuks.Dequeue();

                    chunkMap.Remove(chuks.transform.position);
                    Destroy(chuks.gameObject);
                }
            }
        }
    }

    [Obsolete]
    void NetFindChunksToLoad()
    {
        if (Player)
        {
            int xPos = (int)Player.position.x;
            int zPos = (int)Player.position.z;

            int xx = 0;
            int zz = 0;

            for (int i = -renderDistance; i < renderDistance; i++)
            {
                for (int z = -renderDistance; z < renderDistance; z++)
                {
                    xx = xPos + i;
                    zz = zPos + z;

                    xx = Mathf.FloorToInt(xx / (float)Chunk.Size) * Chunk.Size;
                    zz = Mathf.FloorToInt(zz / (float)Chunk.Size) * Chunk.Size;

                    if (ClientchunkMap.ContainsKey(new Vector3(xx, 0, zz)) == false)
                    {
                        Game.GameManager.GenerateChunkNet(xPos + i, zPos + z);
                        ClientchunkMap.Add(new Vector3(xx, 0, zz), true);
                    }
                }
            }
        }
    }

    [Obsolete]
    void NetFindDeleteChunk()
    {
        if (Player)
        {
            List<Chunk> DeleteChuks = new List<Chunk>(chunkMap.Values);
            Queue<Chunk> deletechuks = new Queue<Chunk>();
            for (int i = 0; i < DeleteChuks.Count; i++)
            {
                float Distanc = Vector3.Distance(Player.position, DeleteChuks[i].transform.position);
                if (Distanc > renderDistance * Chunk.Size)
                {
                    deletechuks.Enqueue(DeleteChuks[i]);
                }
                while (deletechuks.Count > 0)
                {
                    Chunk chuks = deletechuks.Dequeue();
                    Game.GameManager.DeleteChunkNet((int)chuks.transform.position.x, (int)chuks.transform.position.z);
                    ClientchunkMap.Remove(chuks.transform.position);
                    chunkMap.Remove(chuks.transform.position);
                    Destroy(chuks.gameObject);
                }
            }
        }
    }

    [Obsolete]
    public void ClientMakeChunkAt(int x, int z, Tile[] tile)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        z = Mathf.FloorToInt(z / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Vector3(x, 0, z)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, 0, z), Quaternion.identity);
            chunkMap.Add(new Vector3(x, 0,z), go.GetComponent<Chunk>());
            go.GetComponent<Chunk>().ClientSetUpChunk(tile);
        }
    }

    [Obsolete]
    public Tile[] ServerMakeChunkAt(int x, int z, long unique)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
       z = Mathf.FloorToInt(z / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Vector3(x, 0, z)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, 0, z), Quaternion.identity);

            chunkMap.Add(new Vector3(x, 0,z), go.GetComponent<Chunk>());

            go.GetComponent<Chunk>().Players.Add(unique);

            return go.GetComponent<Chunk>().MakeChunk();
        }
        else
        {
            return chunkMap[new Vector3(x, 0, z)].tilelist.ToArray();
        }
    }

    [Obsolete]
    public void NetDeleteChunk(int x, int z, long player)
    {
        if (chunkMap.ContainsKey(new Vector3(x, 0,z)) == true && chunkMap[new Vector3(x, 0,z)].Players.Count == 0 || chunkMap.ContainsKey(new Vector3(x, 0, z)) == true && chunkMap[new Vector3(x, 0, z)].Players.Count == 1 && chunkMap[new Vector3(x, 0, z)].Players[0] == player)
        {
            Chunk chunk = chunkMap[new Vector3(x, 0,z)];

            chunkMap[new Vector3(x, 0, z)].Players.RemoveAt(0);
            chunkMap.Remove(new Vector3(x, 0, z));
            Destroy(chunk.gameObject);
        }
        else if (chunkMap.ContainsKey(new Vector3(x, 0, z)) == true)
        {
            chunkMap[new Vector3(x, 0, z)].Players.Remove(player);
        }
    }
}

public enum WorldType
{
    Normal = 0, DemondWorld = 1, Caves = 2, Dungeons = 3, Sky = 4
}
