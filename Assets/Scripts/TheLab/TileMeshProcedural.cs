using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 public class TileMeshProcedural : MonoBehaviour
{
<<<<<<< HEAD
    public static TileMeshProcedural Instance;
    public GameObject ChunkGO;
    public bool WorldRuning { get; private set; }
    public Vector3 PlayerPos { get; private set; }
    public Transform Player;
    private Thread worldGenerator;
    private Thread chunksPopulate;

    private Queue<Chunk3> pendingDeletions = new Queue<Chunk3>();
    private Queue<ChunkData2> pendingchunks = new Queue<ChunkData2>();

    Dictionary<Chunk3, ChunkSeria> chunkMap = new Dictionary<Chunk3, ChunkSeria>();
    public int renderDistance = 20;
=======
    public int floorWidth;
    public int floorHeight;
>>>>>>> parent of 67432a6... Some backups, working in merching cube generation

    MeshCollider meshCollider;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    Dictionary<string, Vector2[]> tileUVMap;
    public float scaleFactor = 0.5f;
    public int[] triss;

    public Vector3[] verts;
    public Vector2[] uvs;

<<<<<<< HEAD
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        WorldRuning = true;
        PlayerPos = new Vector3((int)Player.position.x, (int)Player.position.y, (int)Player.position.z);
        worldGenerator = new Thread(new ThreadStart(MadeChunks));
        worldGenerator.Name = "WorldGenerator";
        worldGenerator.IsBackground = true;
        worldGenerator.Start();

        /*chunksPopulate = new Thread(new ThreadStart(MadeChunks));
        chunksPopulate.Name = "chunksPopulate";
        chunksPopulate.IsBackground = true;
        chunksPopulate.Start();*/
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "WorldSeed: " + GameManager.Seed);
        GUI.Label(new Rect(10, 30, 200, 20), "Chunks: " + chunkMap.Count);
        GUI.Label(new Rect(10, 50, 200, 20), "ChunksCache: " + pendingchunks.Count);
        GUI.Label(new Rect(10, 70, 200, 20), "ChunksDeleteCache: " + pendingDeletions.Count);
=======
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();

        GenerateFloor();
        UpdateFloor();
>>>>>>> parent of 67432a6... Some backups, working in merching cube generation
    }

    void Update()
    {
<<<<<<< HEAD
        PlayerPos = new Vector3((int)Player.position.x, (int)Player.position.y, (int)Player.position.z);

        while (pendingchunks.Count > 0)
        {
            ChunkData2 chunk = pendingchunks.Dequeue();

            if (!chunkMap.ContainsKey(new Chunk3(chunk.position.x, chunk.position.y, chunk.position.z)))
            {
                GameObject obj = Instantiate(ChunkGO, new Vector3(chunk.position.x, chunk.position.y, chunk.position.z), Quaternion.identity);
                obj.name = "Chunk : " + chunk.position.x + " : " + chunk.position.z;

                obj.GetComponent<MarchingCubesProject.Example>().MakeMesh(chunk.TileThreadHelp);

                //Clear ThreadHelper Memory
                chunk.TileThreadHelp.voxels = null;
                chunk.TileThreadHelp.MeshList.Clear();

                chunkMap.Add(new Chunk3(chunk.position.x, chunk.position.y, chunk.position.z), new ChunkSeria(chunk.position, obj, obj.GetComponent<MarchingCubesProject.Example>()));
            }
        }
        while (pendingDeletions.Count > 0)
=======
        if (Input.GetKeyDown(KeyCode.Space))
>>>>>>> parent of 67432a6... Some backups, working in merching cube generation
        {
            UpdateFloor2();
        }
    }

    void Awake()
    {
        tileUVMap = new Dictionary<string, Vector2[]>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Tiles/");

        float imageWidth = 0f;
        float imageHeight = 0f;

        foreach (Sprite s in sprites)
        {

            if (s.rect.x + s.rect.width > imageWidth)
                imageWidth = s.rect.x + s.rect.width;

            if (s.rect.y + s.rect.height > imageHeight)
                imageHeight = s.rect.y + s.rect.height;
        }

        foreach (Sprite s in sprites)
        {
            Vector2[] uvs = new Vector2[1];

            uvs[0] = new Vector2(s.rect.x / imageWidth, s.rect.y / imageHeight);
            //uvs[1] = new Vector2((s.rect.x + s.rect.width) / imageWidth, s.rect.y / imageHeight);
            //uvs[2] = new Vector2(s.rect.x / imageWidth, (s.rect.y + s.rect.height) / imageHeight);
            //uvs[3] = new Vector2((s.rect.x + s.rect.width) / imageWidth, (s.rect.y + s.rect.height) / imageHeight);

            //tileUVMap.Add(s.name, uvs);
        }
    }

    void GenerateFloor()
    {
<<<<<<< HEAD
        //The size of voxel array.
        int width = 11;
        int height = 11;
        int length = 11;

        //Set the mode used to create the mesh.
        //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
        Marching marching = new MarchingCubesProject.MarchingCubes();
        VoxelDataItem[,,] voxels = new VoxelDataItem[width, height, length];
        List<VerticeVoxel> verts = new List<VerticeVoxel>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        TileThreadHelp tileThreadHelp = new TileThreadHelp();
        tileThreadHelp.MeshList = new Dictionary<TypeBlock, List<Vector3>>();

        //Surface is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
        //The target value does not have to be the mid point it can be any value with in the range.
        marching.Surface = 0.0f;

        //Fill voxels with values. Im using perlin noise but any method to create voxels will work.
        for (int x = 0; x < width; x++)
=======
        vertices = new Vector3[(floorWidth + 1) * (floorHeight + 1)];
        for (int i = 0, z = 0; z <= floorHeight; z++)
>>>>>>> parent of 67432a6... Some backups, working in merching cube generation
        {
            for (int x = 0; x <= floorWidth; x++)
            {
<<<<<<< HEAD
                for (int z = 0; z < length; z++)
                {
                    voxels[x, y, z] = Biome.GetDensity(x + chunkx, y + chunky, z + chunkz);
                }
            }
        }

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(voxels, width, height, length, verts, indices, uvs);

        for (int i = 0; i < verts.Count; i++)
=======
                //float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2.0f;
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        triangles = new int[floorWidth * floorHeight * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < floorHeight; z++)
>>>>>>> parent of 67432a6... Some backups, working in merching cube generation
        {
            for (int x = 0; x < floorWidth; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + floorWidth + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + floorWidth + 1;
                triangles[tris + 5] = vert + floorWidth + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        triss = triangles;
        verts = vertices;
        uvs = new Vector2[verts.Length];
    }

<<<<<<< HEAD
    public MarchingCubesProject.Example GetChunkAt(int xx, int yy,int zz)
    {
        xx = Mathf.FloorToInt(xx / (float)Chunk.Size) * Chunk.Size;
        yy = Mathf.FloorToInt(yy / (float)Chunk.Size) * Chunk.Size;
        zz = Mathf.FloorToInt(zz / (float)Chunk.Size) * Chunk.Size;

        Chunk3 chunkpos = new Chunk3(xx,yy, zz);

        if (chunkMap.ContainsKey(chunkpos))
        {
            return chunkMap[chunkpos].ChunkScript;
        }
        else
        {
            return null;
        }
    }

    public Tile GetTileAt(int x, int y,int z)
    {
        MarchingCubesProject.Example chunk = GetChunkAt(x, y,z);

        if (chunk != null)
        {
            return chunk.Tiles[x - (int)chunk.transform.position.x, y - (int)chunk.transform.position.y, z - (int)chunk.transform.position.z];
        }
        return null;
    }

    public Tile GetTileAt(float x, float y,float z)
    {
        int mx = Mathf.FloorToInt(x);
        int my = Mathf.FloorToInt(y);
        int mz = Mathf.FloorToInt(z);

        MarchingCubesProject.Example chunk = GetChunkAt(mx, my,mz);

        if (chunk != null)
        {
            return chunk.Tiles[mx - (int)chunk.transform.position.x, my - (int)chunk.transform.position.y, mz - (int)chunk.transform.position.z];
        }
        return null;
    }

    protected static readonly int[,] VertexOffset = new int[,]
    {
            {0, 0, 0},{1, 0, 0},{1, 1, 0},{0, 1, 0},
            {0, 0, 1},{1, 0, 1},{1, 1, 1},{0, 1, 1}
    };

    public float CalculateDensity(int worldPositionX, int worldPositionY, int worldPositionZ)
=======
    void UpdateFloor()
>>>>>>> parent of 67432a6... Some backups, working in merching cube generation
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        ClaculateUvs();
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        mesh.RecalculateBounds();
    }

    void UpdateFloor2()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        mesh.RecalculateBounds();
    }
    /// <summary>
    /// 
    /// </summary>
    public void ClaculateUvs()
    {
        // Iterate over each face (here assuming triangles)
        for (int index = 0; index < triss.Length; index += 3)
        {
            // Get the three vertices bounding this triangle.
            Vector3 v1 = verts[triss[index]];
            Vector3 v2 = verts[triss[index + 1]];
            Vector3 v3 = verts[triss[index + 2]];

            // Compute a vector perpendicular to the face.
            Vector3 normal = Vector3.Cross(v3 - v1, v2 - v1);

            Debug.Log("surface " + index / 3 + " : " + (normal * 0.5f).magnitude);

            // Form a rotation that points the z+ axis in this perpendicular direction.
            // Multiplying by the inverse will flatten the triangle into an xy plane.
            Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(normal));

<<<<<<< HEAD
public struct ChunkSeria
{
    public Chunk3 Position;
    public GameObject obj;
    public MarchingCubesProject.Example ChunkScript;

    public ChunkSeria(Chunk3 _Position, GameObject _obj, MarchingCubesProject.Example _ChunkScript)
    {
        Position = _Position;
        obj = _obj;
        ChunkScript = _ChunkScript;
    }
}

public struct TileThreadHelp
{
    public VoxelDataItem[,,] voxels;
    public Dictionary<TypeBlock, List<Vector3>> MeshList;
}
=======
            // Assign the uvs, applying a scale factor to control the texture tiling.
            uvs[triss[index]] = (Vector2)(rotation * v1) * scaleFactor;
            uvs[triss[index + 1]] = (Vector2)(rotation * v2) * scaleFactor;
            uvs[triss[index + 2]] = (Vector2)(rotation * v3) * scaleFactor;
        }
    }

    void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.05f);
        }
    }
}
>>>>>>> parent of 67432a6... Some backups, working in merching cube generation
