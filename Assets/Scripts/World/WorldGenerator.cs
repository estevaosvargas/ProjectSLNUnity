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
    public Vector3 PlayerPos;
    public bool WorldRuning = false;
    public GameObject ChunkGO;
    public GameObject SUN;

    private Thread worldGenerator;

    private System.Random randomValue;

    private Queue<Chunk2> pendingDeletions = new Queue<Chunk2>();
    private Queue<ChunkData> pendingchunks = new Queue<ChunkData>();

    Dictionary<Chunk2, Chunk> chunkMap;
    Dictionary<Vector3, bool> ClientchunkMap;
    public List<Chunk> ChunksList = new List<Chunk>();

    [Header("TimeData")]
    public int h;
    public int d;
    public int m;

    public static int Snap(float i, int v) => (int)(Mathf.Round(i / v) * v);

    private bool isFirstFinished = false;
    private bool doneFirstCheck = false;

    void Awake()
    {
        Game.WorldGenerator = this;
        chunkMap = new Dictionary<Chunk2, Chunk>();
        ClientchunkMap = new Dictionary<Vector3, bool>();

        if (!IsMainMenu)
        {
            if (DarckNet.Network.IsServer)
            {
                DarckNet.Network.Instantiate(SUN, Vector3.zero, Quaternion.identity, World_ID);
                Debug.Log("SERVER: Sun Spawned");
            }

            randomValue = new System.Random(GameManager.Seed);
        }
        else
        {
            
        }
    }

    void Start()
    {
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            Player = Game.GameManager.Player.RequestSpawnPlayer(Biome.GetBiomeYPosition(UnityEngine.Random.Range(-100, 100), 0,UnityEngine.Random.Range(-100, 100)), World_ID).transform;
            //StartCoroutine("StrartGenerator");
        }
    }

    void Update()
    {
        PlayerPos = new Vector3(Player.position.x, 0, Player.position.z);

        if (!doneFirstCheck)
        {
            if (isFirstFinished)
            {
                Game.GameManager.Player.PlayerObj.body.enabled = true;
                Game.ConsoleInGame.LoadingScreen_Hide();
                doneFirstCheck = false;
            }
        }

        if (Game.GameManager.t != null)
        {
            if (Game.WorldGenerator.SlectedBlock != null)
            {
                Game.WorldGenerator.SlectedBlock.gameObject.SetActive(true);
                Game.WorldGenerator.SlectedBlock.position = new Vector3(Game.GameManager.t.x, Game.GameManager.t.y, Game.GameManager.t.z);
            }
        }

        while (pendingchunks.Count > 0)
        {
            ChunkData chunk = pendingchunks.Dequeue();

            if (!chunkMap.ContainsKey(new Chunk2(chunk.position.x, chunk.position.z)))
            {
                GameObject obj = Instantiate(ChunkGO, new Vector3(chunk.position.x, 0, chunk.position.z), Quaternion.identity);
                obj.name = "Chunk : " + chunk.position.x + " : " + chunk.position.z;

                obj.GetComponent<Chunk>().position = new Vector3(chunk.position.x, 0, chunk.position.z);

                obj.GetComponent<Chunk>().MakeChunk(chunk.tiles);

                LoadNewChunks(obj.GetComponent<Chunk>());

                chunkMap.Add(new Chunk2(chunk.position.x, chunk.position.z), obj.GetComponent<Chunk>());
            }
        }
        while (pendingDeletions.Count > 0)
        {
            Chunk2 vector = pendingDeletions.Dequeue();
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

    /// <summary>
    /// this start the world generation(start the thread), this is called by player
    /// </summary>
    public void StartWorld()
    {
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            Player = Game.GameManager.Player.PlayerObj.transform;
            Player.GetComponent<EntityPlayer>().World = this.transform;
        }

        WorldRuning = true;
        PlayerPos = new Vector3((int)Player.position.x, 0, (int)Player.position.z);
        worldGenerator = new Thread(new ThreadStart(MadeChunks));
        worldGenerator.Name = "WorldGenerator";
        worldGenerator.IsBackground = true;
        worldGenerator.Start();
    }

    public void LoadNewChunks(Chunk current_chunk)
    {
        List<Chunk> chunks = new List<Chunk>();

        current_chunk.RefreshChunkTile();

        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x, (int)current_chunk.transform.position.z + 10));//Cima
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x, (int)current_chunk.transform.position.z - 10));//Baixo
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x + 10, (int)current_chunk.transform.position.z));//Direita
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x - 10, (int)current_chunk.transform.position.z));//Esquerda

        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x + 10, (int)current_chunk.transform.position.z + 10));//Cima Direita
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x + 10, (int)current_chunk.transform.position.z - 10));//Baixo Direita

        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x - 10, (int)current_chunk.transform.position.z + 10));//Cima Esquerda
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x - 10, (int)current_chunk.transform.position.z - 10));//Baixo Esquerda

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
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
        {
            Player = Game.GameManager.Player.PlayerObj.transform;
            Player.GetComponent<EntityPlayer>().World = this.transform;
        }
        base.OnRespawn();
    }

    private void MadeChunks()
    {
        while (WorldRuning)
        {
            Vector3 PlayerP = new Vector3(Snap(PlayerPos.x, Chunk.Size), 0, Snap(PlayerPos.z, Chunk.Size));
            int minX = (int)PlayerP.x - renderDistance;
            int maxX = (int)PlayerP.x + renderDistance;
            int minZ = (int)PlayerP.z - renderDistance;
            int maxZ = (int)PlayerP.z + renderDistance;

            for (int z = minZ; z < maxZ; z +=Chunk.Size)
            {
                for (int x = minX; x < maxX; x +=Chunk.Size)
                {
                    Thread.Sleep(ThickRate);
                    Chunk2 vector = new Chunk2(x, z);
                    if (chunkMap.ContainsKey(vector) == false)
                    {
                        ChunkData nchunk = new ChunkData();

                        nchunk.tiles = new Tile[Chunk.Size, Chunk.Size];

                        for (int i = 0; i < Chunk.Size; i++)
                        {
                            for (int j = 0; j < Chunk.Size; j++)
                            {
                                //nchunk.tiles[i,j] = new Tile(i + (int)vector.x, j + (int)vector.z, (int)vector.x, (int)vector.z, null);
                            }
                        }

                        nchunk.position = vector;
                        pendingchunks.Enqueue(nchunk);
                    }
                }
            }

            Chunk[] chunks = chunkMap.Values.ToArray();

            foreach (var chunk in chunks)
            {
                if (chunk != null)
                {
                    Chunk2 vector = new Chunk2((int)chunk.position.x, (int)chunk.position.z);

                    if (vector.x > maxX || vector.x < minX || vector.z > maxZ || vector.z < minZ)
                    {
                        pendingDeletions.Enqueue(vector);
                    }
                }
            }

            isFirstFinished = true;
        }
    }

    public Chunk GetChunkAt(int xx, int zz)
    {
        xx = Mathf.FloorToInt(xx / (float)Chunk.Size) * Chunk.Size;
        zz = Mathf.FloorToInt(zz / (float)Chunk.Size) * Chunk.Size;

        Chunk2 chunkpos = new Chunk2(xx, zz);

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

    public Tile GetTileAt(float x, float z)
    {
        int mx = Mathf.FloorToInt(x);
        int mz = Mathf.FloorToInt(z);

        Chunk chunk = GetChunkAt(mx, mz);

        if (chunk != null)
        {
            return chunk.tiles[mx - (int)chunk.transform.position.x, mz - (int)chunk.transform.position.z];
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

            for (int i = -renderDistance; i < renderDistance; i++)
            {
                for (int z = -renderDistance; z < renderDistance; z++)
                {
                    MakeChunkAt(xPos + i, zPos + z);
                }
            }
        }
    }

    [Obsolete]
    void MakeChunkAt(int x, int z)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
        z = Mathf.FloorToInt(z / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Chunk2(x,z)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, 0, z), Quaternion.identity);
            go.SetActive(true);
            chunkMap.Add(new Chunk2(x,z), go.GetComponent<Chunk>());
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

                    chunkMap.Remove(new Chunk2 ((int)chuks.transform.position.x, (int)chuks.transform.position.z));
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
                    chunkMap.Remove(new Chunk2((int)chuks.transform.position.x, (int)chuks.transform.position.z));
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

        if (chunkMap.ContainsKey(new Chunk2(x, z)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, 0, z), Quaternion.identity);
            chunkMap.Add(new Chunk2(x, z), go.GetComponent<Chunk>());
            //go.GetComponent<Chunk>().ClientSetUpChunk(tile);
        }
    }

    [Obsolete]
    public Tile[] ServerMakeChunkAt(int x, int z, long unique)
    {
        x = Mathf.FloorToInt(x / (float)Chunk.Size) * Chunk.Size;
       z = Mathf.FloorToInt(z / (float)Chunk.Size) * Chunk.Size;

        if (chunkMap.ContainsKey(new Chunk2(x, z)) == false)
        {
            GameObject go = Instantiate(ChunkGO, new Vector3(x, 0, z), Quaternion.identity);

            chunkMap.Add(new Chunk2(x,z), go.GetComponent<Chunk>());

            go.GetComponent<Chunk>().Players.Add(unique);

            return go.GetComponent<Chunk>().MakeChunk(null);
        }
        else
        {
            return chunkMap[new Chunk2(x, z)].tilelist.ToArray();
        }
    }

    [Obsolete]
    public void NetDeleteChunk(int x, int z, long player)
    {
        if (chunkMap.ContainsKey(new Chunk2(x,z)) == true && chunkMap[new Chunk2(x, z)].Players.Count == 0 || chunkMap.ContainsKey(new Chunk2(x, z)) == true && chunkMap[new Chunk2(x, z)].Players.Count == 1 && chunkMap[new Chunk2(x,  z)].Players[0] == player)
        {
            Chunk chunk = chunkMap[new Chunk2(x,z)];

            chunkMap[new Chunk2(x, z)].Players.RemoveAt(0);
            chunkMap.Remove(new Chunk2(x,z));
            Destroy(chunk.gameObject);
        }
        else if (chunkMap.ContainsKey(new Chunk2(x, z)) == true)
        {
            chunkMap[new Chunk2(x,  z)].Players.Remove(player);
        }
    }
}

public enum WorldType
{
    Normal = 0, DemondWorld = 1, Caves = 2, Dungeons = 3, Sky = 4
}


[System.Serializable]
public struct ChunkData
{
    internal bool isReady;
    internal Chunk2 position;
    internal Chunk ChunkC;
    internal GameObject obj;
    public Tile[,] tiles;
}


[System.Serializable]
public struct ChunkData2
{
    internal bool isReady;
    internal Chunk3 position;
    public TileThreadHelp TileThreadHelp;
}

public struct Chunk2
{
    public int x;
    public int z;

    public Chunk2(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}


public struct Chunk3
{
    public int x;
    public int y;
    public int z;

    public Chunk3(int _x, int _y,int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}