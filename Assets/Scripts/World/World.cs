using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class World : MonoBehaviour
{
    public static int ChunkSize = 10;
    public static int ChunkSizeY = 30;
    public GameObject ChunkGO;
    public bool WorldRuning { get; private set; }
    public Vector3 PlayerPos { get; private set; }

    public int renderDistance = 20;
    public int ThickRate = 1;
    public int MeshThickRate = 60;

    private Thread chunkGenerate;
    private Thread chunkMeshGenerate;

    private Queue<Vector3> pendingDeletions = new Queue<Vector3>();
    private Queue<Vector3> pendingchunks = new Queue<Vector3>();
    private Queue<Vector3> pendingUpdateMesh = new Queue<Vector3>();
    private Queue<MeshDataThread> pendingUpdateMeshFinal = new Queue<MeshDataThread>();
    private Dictionary<Vector3, Chunk> chunkMap = new Dictionary<Vector3, Chunk>();

    float terrainSurface = 0.5f;
    public float globalLightLevel = 0;

    private FastNoise globalNoise;
    private FastNoise CaveNoise;
    private FastNoise heatNoise;

    public int ChunksLoaded { get { return chunkMap.Count; } }
    public int ChunksQueue { get { return pendingchunks.Count; } }
    public int MeshDataQueue { get { return pendingUpdateMeshFinal.Count; } }
    public int UpdateMeshQueue { get { return pendingUpdateMesh.Count; } }
    public int ChunksDeleteQueue { get { return pendingDeletions.Count; } }
    public FastNoise GetglobalNoise { get { return globalNoise; } }
    public FastNoise GetCaveNoise { get { return CaveNoise; } }
    public FastNoise GetheatNoise { get { return heatNoise; } }

    public float distanceview = 100;

    private void Awake()
    {
        Game.World = this;
    }

    public void StartWorld()
    {
        //Start All Noises
        globalNoise = new FastNoise(GameManager.Seed);
        CaveNoise = new FastNoise(GameManager.Seed);
        heatNoise = new FastNoise(GameManager.Seed);
        globalNoise.SetFrequency(0.005f);
        CaveNoise.SetFrequency(0.1f);

        heatNoise.SetFrequency(0.05f);
        heatNoise.SetGradientPerturbAmp(30f);
        heatNoise.SetCellularNoiseLookup(new FastNoise());
        heatNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Manhattan);
        heatNoise.SetCellularReturnType(FastNoise.CellularReturnType.NoiseLookup);


        WorldRuning = true;
        PlayerPos = new Vector3((int)Game.GameManager.Player.PlayerObj.transform.position.x, (int)Game.GameManager.Player.PlayerObj.transform.position.y, (int)Game.GameManager.Player.PlayerObj.transform.position.z);

        /*chunkGenerate = new Thread(new ThreadStart(MadeChunks));
        chunkGenerate.Name = "chunkGenerate";
        chunkGenerate.Start();*/

        /*chunkMeshGenerate = new Thread(new ThreadStart(MakeChunkMeshes));
        chunkMeshGenerate.Name = "chunkMeshGenerate";
        chunkMeshGenerate.Start();*/
    }

    public float _WaveSpeed;

    void Update()
    {
        PlayerPos = new Vector3((int)Game.GameManager.Player.PlayerObj.transform.position.x, (int)Game.GameManager.Player.PlayerObj.transform.position.y, (int)Game.GameManager.Player.PlayerObj.transform.position.z);
        Shader.SetGlobalFloat("WaveTime", Time.time * _WaveSpeed);
        /*while (pendingUpdateMeshFinal.Count > 0)
        {
            lock (pendingUpdateMeshFinal)
            {
                MeshDataThread chunk = pendingUpdateMeshFinal.Dequeue();
                lock (chunkMap)
                {
                    if (chunkMap.TryGetValue(chunk.CurrentChunk, out Chunk chunkser))
                    {
                        if (chunk.isfirts)
                        {
                            chunkser.UpdateMeshDataFirst(chunk);
                        }
                        else
                        {
                            //chunkser.UpdateMeshData(chunk);
                        }
                    }
                }

                chunk.triangles = null;
                chunk.vertices = null;
                chunk.colors = null;
                chunk.CurrentChunk = Vector3.zero;
            }
        }

        while (pendingchunks.Count > 0)
        {
            lock (pendingchunks)
            {
                Vector3 chunk = pendingchunks.Dequeue();

                if (!chunkMap.ContainsKey(chunk))
                {
                    GameObject obj = Instantiate(ChunkGO, new Vector3(chunk.x, chunk.y, chunk.z), Quaternion.identity);
                    obj.name = "Chunk - X:" + chunk.x + " Y:" + chunk.y + " Z:" + chunk.z;

                    obj.GetComponent<Chunk>().StartUpChunk();

                    lock (chunkMap)
                    {
                        chunkMap.Add(chunk, obj.GetComponent<Chunk>());
                    }
                }
            }
        }

        while (pendingDeletions.Count > 0)
        {
            lock (pendingDeletions)
            {
                Vector3 vector = pendingDeletions.Dequeue();
                if (chunkMap.ContainsKey(vector))
                {
                    chunkMap[vector].ClearChunk();
                    Destroy(chunkMap[vector].gameObject);

                    lock (chunkMap)
                    {
                        chunkMap.Remove(vector);
                    }
                }
            }
        }*/

        if (!Game.MapManager.readyPlayer)
        {
            Game.MapManager.FinishedLoading();
        }
    }

    private void OnDestroy()
    {
        /*WorldRuning = false;
        chunkGenerate.Abort();
        chunkGenerate = null;

        chunkMeshGenerate.Abort();
        chunkMeshGenerate = null;*/
    }

    /*public Chunk GetChunkFromVector3(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x / ChunkSize);
        int z = Mathf.FloorToInt(pos.z / ChunkSize);
        return chunks[x, z];

    }*/

    public Vector2 GetChunkCoordFromVector3(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x / ChunkSize);
        int z = Mathf.FloorToInt(pos.z / ChunkSize);
        return new Vector2(x, z);

    }

    public void CheckViewDistance()
    {
        Vector2 coord = GetChunkCoordFromVector3(PlayerPos);
        //playerLastChunkCoord = playerChunkCoord;

        //List<Vector2> previouslyActiveChunks = new List<Vector2>(chunkMap);

        // Loop through all chunks currently within view distance of the player.
        for (int x = (int)coord.x - renderDistance; x < coord.x + renderDistance; x++)
        {
            for (int z = (int)coord.y - renderDistance; z < coord.y + renderDistance; z++)
            {
                Vector3 vector = new Vector3(x * ChunkSize, 0, z * ChunkSize);
                
                if (!chunkMap.ContainsKey(vector))
                {
                    /*if (!pendingchunks.Contains(mapdata))
                    {
                        lock (pendingchunks)
                        {
                            pendingchunks.Enqueue(mapdata);
                        }
                    }*/

                    GameObject obj = Instantiate(ChunkGO, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
                    obj.name = "Chunk - X:" + vector.x + " Y:" + vector.y + " Z:" + vector.z;

                    obj.GetComponent<Chunk>().StartUpChunk();

                    lock (chunkMap)
                    {
                        chunkMap.Add(vector, obj.GetComponent<Chunk>());
                    }
                }

                /*// Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {

                    if (previouslyActiveChunks[i].Equals(thisChunkCoord))
                        previouslyActiveChunks.RemoveAt(i);

                }*/

            }
        }

        lock (chunkMap)
        {
            Chunk[] chunks = chunkMap.Values.ToArray();
            foreach (var item in chunks)
            {
                if (Vector3.Distance(PlayerPos, item.chunkPosition) >= distanceview)
                {
                    /*if (!pendingDeletions.Contains(item.chunkPosition))
                    {
                        lock (pendingDeletions)
                        {
                            pendingDeletions.Enqueue(item.chunkPosition);
                        }
                    }*/
                    if (chunkMap.ContainsKey(item.chunkPosition))
                    {
                        chunkMap[item.chunkPosition].ClearChunk();
                        Destroy(chunkMap[item.chunkPosition].gameObject);

                        chunkMap.Remove(item.chunkPosition);
                    }
                }
            }
        }
        // Any chunks left in the previousActiveChunks list are no longer in the player's view distance, so loop through and disable them.
        /*foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;*/

    }

    private void MadeChunks()
    {
        while (WorldRuning)
        {
            Vector3 PlayerP = new Vector3((int)(Mathf.Round(PlayerPos.x / ChunkSize) * ChunkSize), (int)(Mathf.Round(PlayerPos.y / ChunkSizeY) * ChunkSizeY), (int)(Mathf.Round(PlayerPos.z / ChunkSize) * ChunkSize));
            int minX = (int)PlayerP.x - renderDistance;
            int maxX = (int)PlayerP.x + renderDistance;

            int minZ = (int)PlayerP.z - renderDistance;
            int maxZ = (int)PlayerP.z + renderDistance;

            for (int z = minZ; z < maxZ; z += ChunkSize)
            {
                for (int x = minX; x < maxX; x += ChunkSize)
                {
                    Vector3 vector = new Vector3(x, 0, z);

                    if (!chunkMap.ContainsKey(vector))
                    {
                        if (!pendingchunks.Contains(vector))
                        {
                            lock (pendingchunks)
                            {
                                pendingchunks.Enqueue(vector);
                            }
                        }
                    }
                }
            }

            lock (chunkMap)
            {
                foreach (var item in chunkMap.Values)
                {
                    if (item.chunkPosition.x > maxX || item.chunkPosition.x < minX || item.chunkPosition.z > maxZ || item.chunkPosition.z < minZ)
                    {
                        if (!pendingDeletions.Contains(item.chunkPosition))
                        {
                            lock (pendingDeletions)
                            {
                                pendingDeletions.Enqueue(item.chunkPosition);
                            }
                        }
                    }

                }
            }

            minX = 0;
            maxX = 0;
            minZ = 0;
            maxZ = 0;

            Thread.Sleep(ThickRate);
        }
    }

    public Chunk GetChunkAt(int xx, int zz)
    {
        Vector3 chunkpos = new Vector3(Mathf.FloorToInt(xx / (float)ChunkSize) * ChunkSize, 0, Mathf.FloorToInt(zz / (float)ChunkSize) * ChunkSize);
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

    public void LoadNewChunks(Vector3 current_chunk)
    {
        UpdateMesh(current_chunk);
        /*UpdateMesh(new Vector3((int)current_chunk.x, (int)current_chunk.y + ChunkSize, (int)current_chunk.z));//Cima
        UpdateMesh(new Vector3((int)current_chunk.x, (int)current_chunk.y - ChunkSize, (int)current_chunk.z));//Baixo
        UpdateMesh(new Vector3((int)current_chunk.x + ChunkSize, (int)current_chunk.y, (int)current_chunk.z));//Direita
        UpdateMesh(new Vector3((int)current_chunk.x - ChunkSize, (int)current_chunk.y, (int)current_chunk.z));//Esquerda

        UpdateMesh(new Vector3((int)current_chunk.x + ChunkSize, (int)current_chunk.y + ChunkSize, (int)current_chunk.z));//Cima Direita
        UpdateMesh(new Vector3((int)current_chunk.x + ChunkSize, (int)current_chunk.y - ChunkSize, (int)current_chunk.z));//Baixo Direita

        UpdateMesh(new Vector3((int)current_chunk.x - ChunkSize, (int)current_chunk.y + ChunkSize, (int)current_chunk.z));//Cima Esquerda
        UpdateMesh(new Vector3((int)current_chunk.x - ChunkSize, (int)current_chunk.y - ChunkSize, (int)current_chunk.z));//Baixo Esquerda

        UpdateMesh(new Vector3((int)current_chunk.x, (int)current_chunk.y, (int)current_chunk.z + ChunkSize));//Frente
        UpdateMesh(new Vector3((int)current_chunk.x, (int)current_chunk.y, (int)current_chunk.z - ChunkSize));//Atras

        UpdateMesh(new Vector3((int)current_chunk.x + ChunkSize, (int)current_chunk.y, (int)current_chunk.z + ChunkSize));//Frente Direita
        UpdateMesh(new Vector3((int)current_chunk.x + ChunkSize, (int)current_chunk.y, (int)current_chunk.z - ChunkSize));//Atras Direita

        UpdateMesh(new Vector3((int)current_chunk.x - ChunkSize, (int)current_chunk.y, (int)current_chunk.z + ChunkSize));//Frente Esquerda
        UpdateMesh(new Vector3((int)current_chunk.x - ChunkSize, (int)current_chunk.y, (int)current_chunk.z - ChunkSize));//Atras Esquerda*/
    }

    public Block GetTileAt(int x, int z)
    {
        Chunk chunk = GetChunkAt(x, z);

        if (chunk != null)
        {

            return chunk.Blocks[x - (int)chunk.chunkPosition.x, z - (int)chunk.chunkPosition.z];
        }
        return null;
    }

    public Block GetTileAt(Vector3 pos)
    {
        Chunk chunk = GetChunkAt((int)pos.x, (int)pos.z);

        if (chunk != null)
        {
            lock (chunk.Blocks)
                return chunk.Blocks[(int)pos.x - (int)chunk.chunkPosition.x, (int)pos.z - (int)chunk.chunkPosition.z];
        }
        return null;
    }

    public Block GetTileAt(float x, float z)
    {
        int mx = Mathf.FloorToInt(x);
        int mz = Mathf.FloorToInt(z);

        Chunk chunk = GetChunkAt(mx, mz);

        if (chunk != null)
        {
           return chunk.Blocks[mx - (int)chunk.chunkPosition.x, mz - (int)chunk.chunkPosition.z];
        }
        return null;
    }

    public Chunk GetChunk(VoxelVector3 pos)
    {
        if (chunkMap.TryGetValue(pos.ToVector3(), out Chunk chunk))
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

    public void UpdateMesh(Vector3 chunkpos)
    {
        lock (pendingUpdateMesh)
        {
            pendingUpdateMesh.Enqueue(chunkpos);
        }
    }

    public static readonly int[,] Tris = new int[6, 4]
{
        {0,3,1,2}, //Back Face 0
        {5,6,4,7},  //Front Face 1
        {3,7,2,6},  //Top Face 2
        {1,5,0,4},  //Bottom Face 3 
        {4,7,0,3},  //Left Face 4
        {1,2,5,6}   //Right Face 5
};

    public static readonly Vector3[] faceChecks = new Vector3[6] {

        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f)

    };
}

public struct VoxelBlock
{
    public TypeBlock Type;
    public BiomeType TileBiome;
    public TypeVariante typeVariante;
    public Placer PLACER_DATA;
    public TakeGO typego;

    public byte Hora;
    public byte Dia;
    public byte Mes;

    public byte LayerLevel;//10 is full block, 0 is lowest block but still have block

    public float LightLevel;
    public Vector3 CurrentChunk;
    public GameObject BlockObject;
}

/// <summary>
/// darkcomsoft solution for voxel system. (Vector3 INT)
/// </summary>
[System.Serializable]
public struct VoxelVector3
{
    public int _x;
    public int _y;
    public int _z;

    public VoxelVector3(int x, int y, int z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    /// <summary>
    /// Converte unity's vector3 to VoxelVector3, but this convert to int, if you need to pass a value axies with float you can't
    /// </summary>
    /// <param name="vector3Position"></param>
    public VoxelVector3(Vector3 vector3Position)
    {
        _x = (int)vector3Position.x;
        _y = (int)vector3Position.y;
        _z = (int)vector3Position.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(_x,_y,_z);
    }
}

public struct MeshDataThread
{
    public bool isfirts;
    public Vector3 CurrentChunk;
    public Vector3[] vertices;
    public int[] triangles;
    public Color[] colors;
    public Vector2[] uvs;
}

public class MeshData
{
    public List<Vector3> _vertices;
    public List<Vector2> _UVs;
    public List<int> _triangles;
    public List<Color> _colors;

    public bool _HaveWater;

    public MeshData(Block[,] tile)
    {
        _vertices = new List<Vector3>();
        _UVs = new List<Vector2>();
        _triangles = new List<int>();
        _colors = new List<Color>();

        for (int x = 0; x < World.ChunkSize; x++)
        {
            for (int z = 0; z < World.ChunkSize; z++)
            {
                if (tile[x, z] != null)
                {
                    if (tile[x, z].Type == TypeBlock.WaterFloor)
                    {
                        _HaveWater = true;
                    }

                    int xB = tile[x, z].x;
                    int zB = tile[x, z].z;

                    float Right = GetTile(xB + 1, zB, tile[x, z].hight, tile);
                    float FrenteRight = GetTile(xB, zB + 1, tile[x, z].hight, tile);
                    float FrenteLeft = GetTile(xB + 1, zB + 1, tile[x, z].hight, tile);

                    _vertices.Add(new Vector3(x, tile[x, z].hight, z));
                    _vertices.Add(new Vector3(x + 1, Right, z));
                    _vertices.Add(new Vector3(x, FrenteRight, z + 1));
                    _vertices.Add(new Vector3(x + 1, FrenteLeft, z + 1));

                    _triangles.Add(_vertices.Count - 1);
                    _triangles.Add(_vertices.Count - 3);
                    _triangles.Add(_vertices.Count - 4);

                    _triangles.Add(_vertices.Count - 2);
                    _triangles.Add(_vertices.Count - 1);
                    _triangles.Add(_vertices.Count - 4);

                    Color blockcolor = Get.GetColorTile(tile[x, z]);

                    _colors.Add(blockcolor);
                    _colors.Add(blockcolor);
                    _colors.Add(blockcolor);
                    _colors.Add(blockcolor);

                    _UVs.AddRange(Game.SpriteManager.GetTileUVs(tile[x, z]));
                }
            }
        }
    }

    public VoxelMesh MakeWaterMesh(Block[,] tile)
    {
        Vector3[] vertices;
        Vector2[] uvs;
        int[] triangles;
        List<Color> colors = new List<Color>();

        VoxelMesh mesh = new VoxelMesh();

        int widh = World.ChunkSize + 1;

        vertices = new Vector3[widh * widh];
        for (int y = 0; y < widh; y++)
        {
            for (int x = 0; x < widh; x++)
            {
                vertices[x + y * widh] = new Vector3(x, 0.0f, y);

                colors.Add(new Color(1,1,1,0));
            }
        }

        triangles = new int[3 * 2 * (widh * widh - widh - widh + 1)];
        int triangleVertexCount = 0;
        for (int vertex = 0; vertex < widh * widh - widh; vertex++)
        {
            if (vertex % widh != (widh - 1))
            {
                // First triangle
                int A = vertex;
                int B = A + widh;
                int C = B + 1;
                triangles[triangleVertexCount] = A;
                triangles[triangleVertexCount + 1] = B;
                triangles[triangleVertexCount + 2] = C;
                //Second triangle
                B += 1;
                C = A + 1;
                triangles[triangleVertexCount + 3] = A;
                triangles[triangleVertexCount + 4] = B;
                triangles[triangleVertexCount + 5] = C;
                triangleVertexCount += 6;
            }
        }

        uvs = new Vector2[widh * widh];
        int uvIndexCounter = 0;
        foreach (Vector3 vertex in vertices)
        {
            uvs[uvIndexCounter] = new Vector2(vertex.x, vertex.z);
            uvIndexCounter++;
        }

        mesh.verts = vertices;
        mesh.uvs = uvs;
        mesh.indices = triangles;
        mesh.colors = colors.ToArray();

        return mesh;
    }

    float GetTile(int x, int z, float hDeafult,Block[,] array)
    {
        Block block = Game.World.GetTileAt(x, z);

        if (block != null)
        {
            return block.hight;
        }

        return hDeafult;
    }
}