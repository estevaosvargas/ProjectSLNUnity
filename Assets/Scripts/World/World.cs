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

                obj.GetComponent<Chunk>().MakeMesh(chunk.density);

                lock (chunkMap)
                {
                    chunkMap.Add(chunk.position, obj.GetComponent<Chunk>());
                }
            }
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

                                nchunk.density = new Block[ChunkSize, ChunkSize, ChunkSize];
                                GenerateVoxelData(nchunk.density, vector);

                                nchunk.position = vector;

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
                }
            }

            Thread.Sleep(ThickRate);
        }
    }

    private void GenerateVoxelData(Block[,,] voxels, Vector3 chunkPos)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
                   
                    voxels[x, y, z] = new Block(x + (int)chunkPos.x, y + (int)chunkPos.y, z + (int)chunkPos.z);

                    voxels[x, y, z].Type = Biome.GetDensity(voxels[x, y, z].x, voxels[x, y, z].y, voxels[x, y, z].z, 0, voxels[x, y, z]);
                }
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

    public Block GetTileAt(int x, int y, int z)
    {
        Chunk chunk = GetChunkAt(x, y, z);

        if (chunk != null)
        {
            return chunk.Blocks[x - (int)chunk.chunkPosition.x, y - (int)chunk.chunkPosition.y, z - (int)chunk.chunkPosition.z];
        }
        return null;
    }

    public Block GetTileAt(float x, float y, float z)
    {
        int mx = Mathf.FloorToInt(x);
        int my = Mathf.FloorToInt(y);
        int mz = Mathf.FloorToInt(z);

        Chunk chunk = GetChunkAt(mx, my, mz);

        if (chunk != null)
        {
            return chunk.Blocks[mx - (int)chunk.chunkPosition.x, my - (int)chunk.chunkPosition.y, mz - (int)chunk.chunkPosition.z];
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
    public Vector3 position;
    public Block[,,] density;
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

    public MeshData(Block[,,] tile)
    {
        vertices = new List<Vector3>();
        UVs = new List<Vector2>();
        triangles = new List<int>();
        
        for (int x = 0; x < World.ChunkSize; x++)
        {
            for (int y = 0; y < World.ChunkSize; y++)
            {
                for (int z = 0; z < World.ChunkSize; z++)
                {
                    if (tile[x, y, z].density > 0f && tile[x, y, z].density <= 2f)
                    {
                        CreateSquare(tile[x, y, z].x, tile[x, y, z].y, tile[x, y, z].z, tile[x, y, z]);
                    }
                    else
                    {
                        tile[x, y, z].Type = TypeBlock.Air;
                    }
                }
            }
        }
    }

    void CreateSquare(int x, int y, int z, Block block)
    {
        float firts = GetTile(x, y, z);
        float Right = GetTile(x + 1, y, z);
        float FrenteRight = GetTile(x, y, z + 1);
        float FrenteLeft = GetTile(x + 1, y, z + 1);

        vertices.Add(new Vector3(x,      firts,         z));
        vertices.Add(new Vector3(x + 1,  Right,         z));
        vertices.Add(new Vector3(x,      FrenteRight,   z + 1));
        vertices.Add(new Vector3(x + 1,  FrenteLeft,    z + 1));

        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 4);

        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 4);

        UVs.AddRange(Game.SpriteManager.GetTileUVs(block));
    }

    float GetTile(int x, int y, int z)
    {
        return Biome.GetDensityRaw(x, y, z);
    }
}