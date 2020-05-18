using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class World : MonoBehaviour
{
    public static int ChunkSize = 10;
    public GameObject ChunkGO;
    public bool WorldRuning { get; private set; }
    public Vector3 PlayerPos { get; private set; }
    private Vector3 LastPlayerP;

    public int renderDistance = 20;
    public int ThickRate = 1;

    private Thread chunkGenerate;
    private Thread chunkMeshGenerate;

    public static int Snap(float i, int v) => (int)(Mathf.Round(i / v) * v);

    public Queue<Vector3> pendingDeletions = new Queue<Vector3>();
    public Queue<MapData> pendingchunks = new Queue<MapData>();
    public Queue<Vector3> pendingUpdateMesh = new Queue<Vector3>();
    public Queue<MeshDataThread> pendingUpdateMeshFinal = new Queue<MeshDataThread>();
    Dictionary<Vector3, Chunk> chunkMap = new Dictionary<Vector3, Chunk>();

    public int ChunksLoaded { get { return chunkMap.Count; } }
    public int ChunksQueue { get { return pendingchunks.Count; } }
    public int MeshDataQueue { get { return pendingUpdateMeshFinal.Count; } }
    public int UpdateMeshQueue { get { return pendingUpdateMesh.Count; } }
    public int ChunksDeleteQueue { get { return pendingDeletions.Count; } }

    private void Awake()
    {
        Game.World = this;
    }

    public void StartWorld()
    {
        NoiseData.StartData();

        WorldRuning = true;
        PlayerPos = new Vector3((int)Game.GameManager.Player.PlayerObj.transform.position.x, (int)Game.GameManager.Player.PlayerObj.transform.position.y, (int)Game.GameManager.Player.PlayerObj.transform.position.z);

        chunkGenerate = new Thread(new ThreadStart(MadeChunks));
        chunkGenerate.Name = "chunkGenerate";
        chunkGenerate.IsBackground = true;
        chunkGenerate.Start();

        chunkMeshGenerate = new Thread(new ThreadStart(MakeChunkMeshes));
        chunkMeshGenerate.Name = "chunkMeshGenerate";
        chunkMeshGenerate.IsBackground = true;
        chunkMeshGenerate.Start();
    }

    void Update()
    {
        PlayerPos = new Vector3((int)Game.GameManager.Player.PlayerObj.transform.position.x, (int)Game.GameManager.Player.PlayerObj.transform.position.y, (int)Game.GameManager.Player.PlayerObj.transform.position.z);

        while (pendingchunks.Count > 0)
        {
            MapData chunk = pendingchunks.Dequeue();

            if (!chunkMap.ContainsKey(chunk.position))
            {
                GameObject obj = Instantiate(ChunkGO, new Vector3(chunk.position.x, chunk.position.y, chunk.position.z), Quaternion.identity);
                obj.name = "Chunk - X:" + chunk.position.x + " Y:" + chunk.position.y + " Z:" + chunk.position.z;

                if (chunk.isTerrain)
                {
                    obj.GetComponent<Chunk>().StartUpChunk(chunk.blocksArray);
                }
                else
                {
                    obj.GetComponent<Chunk>().chunkPosition = chunk.position;
                }

                lock (chunkMap)
                {
                    chunkMap.Add(chunk.position, obj.GetComponent<Chunk>());
                }
            }

            chunk.blocksArray = null;
        }

        while (pendingDeletions.Count > 0)
        {
            Vector3 vector = pendingDeletions.Dequeue();
            if (chunkMap.TryGetValue(vector, out Chunk chunk))
            {
                Destroy(chunk.gameObject);

                lock (chunkMap)
                {
                    chunkMap.Remove(vector);
                }
            }
        }

        while (pendingUpdateMeshFinal.Count > 0)
        {
            MeshDataThread chunk = pendingUpdateMeshFinal.Dequeue();

            if (chunkMap.TryGetValue(chunk.CurrentChunk, out Chunk chunkser))
            {
               chunkser.UpdateMeshData(chunk);
            }

            chunk.triangles = null;
            chunk.uvs = null;
            chunk.vertices = null;
        }

        if (!Game.MapManager.readyPlayer)
        {
            Game.MapManager.FinishedLoading();
        }
    }

    private void OnDestroy()
    {
        WorldRuning = false;
        chunkGenerate.Abort();
        chunkGenerate = null;

        chunkMeshGenerate.Abort();
        chunkMeshGenerate = null;
    }

    private void MadeChunks()
    {
        while (WorldRuning)
        {
            Vector3 PlayerP = new Vector3(Snap(PlayerPos.x, ChunkSize), Snap(PlayerPos.y, ChunkSize), Snap(PlayerPos.z, ChunkSize));
            int minX = (int)PlayerP.x - renderDistance;
            int maxX = (int)PlayerP.x + renderDistance;

            int minY = (int)PlayerP.y - renderDistance;
            int maxY = (int)PlayerP.y + renderDistance;

            int minZ = (int)PlayerP.z - renderDistance;
            int maxZ = (int)PlayerP.z + renderDistance;

            if (LastPlayerP != PlayerP)
            {
                for (int z = minZ; z < maxZ; z += ChunkSize)
                {
                    for (int y = minY; y < maxY; y += ChunkSize)
                    {
                        for (int x = minX; x < maxX; x += ChunkSize)
                        {
                            Vector3 vector = new Vector3(x, y, z);

                            if (!chunkMap.ContainsKey(vector))
                            {
                                MapData nchunk = new MapData();

                                nchunk.position = vector;

                                if (y == 0)
                                {
                                    nchunk.isTerrain = true;

                                    nchunk.blocksArray = new Block[ChunkSize, ChunkSize];
                                    GenerateVoxelData(nchunk.blocksArray, vector);
                                }
                                else
                                {
                                    nchunk.isTerrain = false;
                                }

                                pendingchunks.Enqueue(nchunk);
                            }
                        }
                    }
                }

                lock (chunkMap)
                {
                    foreach (var item in chunkMap.Values)
                    {
                        if (item.chunkPosition.x > maxX || item.chunkPosition.x < minX || item.chunkPosition.y > maxY || item.chunkPosition.y < minY || item.chunkPosition.z > maxZ || item.chunkPosition.z < minZ)
                        {
                            if (!pendingDeletions.Contains(item.chunkPosition))
                            {
                                pendingDeletions.Enqueue(item.chunkPosition);
                            }
                        }

                    }
                }
            }

            LastPlayerP = PlayerP;
            Thread.Sleep(ThickRate);
        }
    }


    private void MakeChunkMeshes()
    {
        while (WorldRuning)
        {
            while (pendingUpdateMesh.Count > 0)
            {
                Chunk chunk = GetChunkNullOption(pendingUpdateMesh.Dequeue());

                if (chunk != null)
                {
                    MeshDataThread meshDataThread = new MeshDataThread();
                    MeshData data = new MeshData(chunk.Blocks);

                    meshDataThread.CurrentChunk = chunk.chunkPosition;

                    meshDataThread.vertices = data.vertices.ToArray();
                    meshDataThread.triangles = data.triangles.ToArray();
                    meshDataThread.uvs = data.UVs.ToArray();


                    pendingUpdateMeshFinal.Enqueue(meshDataThread);

                    data.vertices = null;
                    data.triangles = null;
                    data.UVs = null;
                }
            }

            Thread.Sleep(ThickRate);
        }
    }

    private void GenerateVoxelData(Block[,] voxels, Vector3 chunkPos)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                voxels[x, z] = new Block(x + (int)chunkPos.x, z + (int)chunkPos.z, chunkPos);
            }
        }
    }

    public Chunk GetChunkAt(int xx, int yy, int zz)
    {
        Vector3 chunkpos = new Vector3(Mathf.FloorToInt(xx / (float)ChunkSize) * ChunkSize, Mathf.FloorToInt(yy / (float)ChunkSize) * ChunkSize, Mathf.FloorToInt(zz / (float)ChunkSize) * ChunkSize);
        lock (chunkMap)
        {
            if (chunkMap.ContainsKey(chunkpos))
            {
                return chunkMap[chunkpos];
            }
            else
            {
                return null;
            }
        }
    }

    public void LoadNewChunks(Chunk current_chunk)
    {
        List<Chunk> chunks = new List<Chunk>();

        current_chunk.UpdateMeshChunk();

        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x, 0, (int)current_chunk.transform.position.z + 10));//Cima
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x, 0,(int)current_chunk.transform.position.z - 10));//Baixo
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x + 10, 0, (int)current_chunk.transform.position.z));//Direita
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x - 10, 0, (int)current_chunk.transform.position.z));//Esquerda

        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x + 10, 0, (int)current_chunk.transform.position.z + 10));//Cima Direita
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x + 10, 0, (int)current_chunk.transform.position.z - 10));//Baixo Direita

        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x - 10, 0, (int)current_chunk.transform.position.z + 10));//Cima Esquerda
        chunks.Add(GetChunkAt((int)current_chunk.transform.position.x - 10, 0, (int)current_chunk.transform.position.z - 10));//Baixo Esquerda

        foreach (var item in chunks)
        {
            if (item != null)
            {
                item.UpdateMeshChunk();
            }
        }
    }

    /// <summary>
    /// Get tile tarrain not the block voxel
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Block GetTileAt(int x, int z)
    {
        Chunk chunk = GetChunkAt(x, 0, z);

        if (chunk != null)
        {
            return chunk.Blocks[x - (int)chunk.chunkPosition.x, z - (int)chunk.chunkPosition.z];
        }
        return null;
    }

    public Block GetTileAt(float x, float z)
    {
        int mx = Mathf.FloorToInt(x);
        int mz = Mathf.FloorToInt(z);

        Chunk chunk = GetChunkAt(mx, 0, mz);

        if (chunk != null)
        {
            return chunk.Blocks[mx - (int)chunk.chunkPosition.x,  mz - (int)chunk.chunkPosition.z];
        }
        return null;
    }

    public Chunk GetChunk(Vector3 pos)
    {
        if (chunkMap.TryGetValue(pos, out Chunk chunk))
        {
            return chunk;
        }

        throw new System.Exception("Dont found this chunk : " + pos.ToString());
    }

    public Chunk GetChunkNullOption(Vector3 pos)
    {
        if (chunkMap.TryGetValue(pos, out Chunk chunk))
        {
            return chunk;
        }

        return null;
    }

    public void UpdateMesh(Chunk chunk)
    {
        pendingUpdateMesh.Enqueue(chunk.chunkPosition);
    }
}

public struct MapData
{
    public bool isTerrain;
    public Vector3 position;
    public Block[,] blocksArray;
}

public struct MeshDataThread
{
    public Vector3 CurrentChunk;
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
}

public struct MeshGroup
{
    public TypeBlock type;
    public List<Vector3> vertices;
    public List<int> triangles;
}

public struct VoxelDataItem
{
    public float density;
    public TypeBlock Type;
    public BiomeType TileBiome;
    public TypeVariante typeVariante;
    public Placer PLACER_DATA;
    public TakeGO typego;
}

public struct VertVoxel
{
    public Vector3 Vertice;
    public Color VertColor;

    public VertVoxel(Vector3 _Vertice, Color _VertColor)
    {
        Vertice = _Vertice;
        VertColor = _VertColor;
    }
}

public struct TileThreadHelp
{
    public float[,,] voxels;
    public Dictionary<TypeBlock, List<Vector3>> MeshList;
}

public class MeshData
{
    public List<Vector3> vertices;
    public List<Vector2> UVs;
    public List<int> triangles;

    public MeshData(Block[,] tile)
    {
        vertices = new List<Vector3>();
        UVs = new List<Vector2>();
        triangles = new List<int>();
        
        for (int x = 0; x < World.ChunkSize; x++)
        {
            for (int z = 0; z < World.ChunkSize; z++)
            {
                CreateSquare(tile[x, z].x, tile[x, z].z, x,z,tile[x, z], tile);
            }
        }
    }

    void CreateSquare(int x, int z, int xR, int zR, Block block, Block[,] blocks)
    {
        float Right = GetTile(x + 1, z, xR + 1,zR, block.h, blocks);
        float FrenteRight = GetTile(x, z + 1, xR, zR + 1, block.h, blocks);
        float FrenteLeft = GetTile(x + 1, z + 1, xR + 1, zR + 1, block.h, blocks);

        vertices.Add(new Vector3(x,     block.h,       z));
        vertices.Add(new Vector3(x + 1, Right,         z));
        vertices.Add(new Vector3(x,     FrenteRight,   z + 1));
        vertices.Add(new Vector3(x + 1, FrenteLeft,    z + 1));

        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 4);

        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 4);

        UVs.AddRange(Game.SpriteManager.GetTileUVs(block));
    }

    float GetTile(int x, int z, int xR, int zR, float hDeafult,Block[,] array)
    {
        if (xR < World.ChunkSize && zR < World.ChunkSize)
        {
            return array[xR, zR].h;
        }
        else
        {
            Block block = Game.World.GetTileAt(x, z);

            if (block != null)
            {
                return block.h;
            }
        }

        return hDeafult;
    }
}