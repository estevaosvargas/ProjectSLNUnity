using System.Collections;
 using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using ProceduralNoiseProject;
using MarchingCubesProject;

 public class TileMeshProcedural : MonoBehaviour
{
    public GameObject ChunkGO;
    public bool WorldRuning { get; private set; }
    public Vector3 PlayerPos { get; private set; }
    public Transform Player;
    private Thread worldGenerator;

    private Queue<Chunk3> pendingDeletions = new Queue<Chunk3>();
    private Queue<ChunkData2> pendingchunks = new Queue<ChunkData2>();

    Dictionary<Chunk3, ChunkSeria> chunkMap = new Dictionary<Chunk3, ChunkSeria>();
    public int renderDistance = 20;

    public int ThickRate = 1;

    public int seed = 0;

    public static int Snap(float i, int v) => (int)(Mathf.Round(i / v) * v);

    private void Start()
    {
        WorldRuning = true;
        PlayerPos = new Vector3((int)Player.position.x, (int)Player.position.y, (int)Player.position.z);
        worldGenerator = new Thread(new ThreadStart(MadeChunks));
        worldGenerator.Name = "WorldGenerator";
        worldGenerator.IsBackground = true;
        worldGenerator.Start();
    }

    void Update()
    {
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

                chunkMap.Add(new Chunk3(chunk.position.x, chunk.position.y, chunk.position.z), new ChunkSeria(chunk.position, obj));
            }
        }
        while (pendingDeletions.Count > 0)
        {
            Chunk3 vector = pendingDeletions.Dequeue();
            if (chunkMap.TryGetValue(vector, out ChunkSeria chunk))
            {
                Destroy(chunk.obj);
                chunkMap.Remove(vector);
            }
        }
    }

    private void OnDestroy()
    {
        WorldRuning = false;
        worldGenerator.Abort();
        worldGenerator = null;
    }

    private void MadeChunks()
    {
        while (WorldRuning)
        {
            Vector3 PlayerP = new Vector3(Snap(PlayerPos.x, Chunk.Size), Snap(PlayerPos.y, Chunk.Size), Snap(PlayerPos.z, Chunk.Size));
            int minX = (int)PlayerP.x - renderDistance;
            int maxX = (int)PlayerP.x + renderDistance;

            int minY = (int)PlayerP.y - renderDistance;
            int maxY = (int)PlayerP.y + renderDistance;

            int minZ = (int)PlayerP.z - renderDistance;
            int maxZ = (int)PlayerP.z + renderDistance;


            if (chunkMap.Count > 0)//If is up of zero, delete older chunks(far away chunks, of the center)
            {
                ChunkSeria[] chunks = chunkMap.Values.ToArray();

                foreach (var chunk in chunks)
                {
                    if (chunk.obj != null)
                    {
                        Chunk3 vector = new Chunk3((int)chunk.Position.x, (int)chunk.Position.y, (int)chunk.Position.z);

                        if (vector.x > maxX || vector.x < minX || vector.y > maxY || vector.y < minY || vector.z > maxZ || vector.z < minZ)
                        {
                            pendingDeletions.Enqueue(vector);
                        }
                    }
                }
            }

            for (int z = minZ; z < maxZ; z += Chunk.Size)
            {
                for (int y = minY; y < maxY; y += Chunk.Size)
                {
                    for (int x = minX; x < maxX; x += Chunk.Size)
                    {
                        Thread.Sleep(ThickRate);
                        Chunk3 vector = new Chunk3(x, y, z);
                        if (chunkMap.ContainsKey(vector) == false)
                        {
                            ChunkData2 nchunk = new ChunkData2();

                            nchunk.TileThreadHelp = MakeMarching(vector.x, vector.y, vector.z);

                            nchunk.position = vector;
                            pendingchunks.Enqueue(nchunk);
                        }
                    }
                }
            }
        }
    }

    public TileThreadHelp MakeMarching(int chunkx, int chunky, int chunkz)
    {
       

        //Set the mode used to create the mesh.
        //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
        Marching marching = new MarchingCubesProject.MarchingCubes();

        //Surface is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
        //The target value does not have to be the mid point it can be any value with in the range.
        marching.Surface = 0.0f;

        //The size of voxel array.
        int width = 11;
        int height = 11;
        int length = 11;

        TileNew[,,] voxels = new TileNew[width, height, length];

        //Fill voxels with values. Im using perlin noise but any method to create voxels will work.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    float fx = x / (width - 1.0f);
                    float fy = y / (height - 1.0f);
                    float fz = z / (length - 1.0f);

                    int idx = x + y * width + z * width * height;

                    float density = (float)new LibNoise.Unity.Generator.Perlin(5f, 0.6f, 2.15f, 10, seed, LibNoise.Unity.QualityMode.Low).GetValue(x + chunkx, y + chunky, z + chunkz) / 50;

                    voxels[x, y, z].x = x + chunkx;
                    voxels[x, y, z].y = y + chunky;
                    voxels[x, y, z].z = z + chunkz;


                    if ((y + chunky) <= -10 && (y + chunky) <= 10)
                    {
                        voxels[x, y, z].typeblock = TypeBlocks.stone;
                        voxels[x, y, z].tileObject = TakeGO.empty;
                    }
                    else
                    {
                        density = -CalculateDensity(x + chunkx, y + chunky, z + chunkz) + -CalculateDensity2(x + chunkx, y + chunky, z + chunkz);

                        float perlin = 1f - 100f / (100f + density);

                        if (perlin <= 0.15f)
                        {
                            voxels[x, y, z].typeblock = TypeBlocks.grass;
                        }
                        else if (perlin > 0.15f && perlin < 0.2f)
                        {
                            voxels[x, y, z].typeblock = TypeBlocks.grass;
                        }
                        else if (perlin > 0.2f && perlin <= 0.7f)
                        {
                            if (perlin > 0.2f && perlin < 0.6f)
                            {
                                voxels[x, y, z].typeblock = TypeBlocks.grass;
                            }
                            else if (perlin > 0.6f && perlin < 0.605f)
                            {
                                voxels[x, y, z].typeblock = TypeBlocks.grass;
                            }
                            else if (perlin > 0.62f && perlin < 0.63f)
                            {
                                voxels[x, y, z].typeblock = TypeBlocks.grass;
                            }
                            else
                            {
                                voxels[x, y, z].typeblock = TypeBlocks.grass;
                            }

                        }
                        else if (perlin > 0.7f && perlin <= 0.8f)
                        {
                            if (perlin > 0.7f && perlin < 0.72f)
                            {
                                voxels[x, y, z].typeblock = TypeBlocks.stone;
                            }
                            else if (perlin > 0.72f && perlin < 0.74f)
                            {
                                voxels[x, y, z].typeblock = TypeBlocks.stone;
                            }
                            else
                            {
                                voxels[x, y, z].typeblock = TypeBlocks.stone;
                            }

                        }
                        else
                        {
                            voxels[x, y, z].typeblock = TypeBlocks.stone;
                        }

                    }

                    voxels[x, y, z].density = math.clamp(density, -1, 1);


                    if (voxels[x, y, z].density >= -0.9f && voxels[x, y, z].density <= 0)
                    {
                        if (voxels[x, y, z].typeblock == TypeBlocks.grass)
                        {
                            System.Random ran = new System.Random(0 + voxels[x, y, z].x * voxels[x, y, z].y * voxels[x, y, z].z);

                            if (ran.Next(0, 20) == 5)
                            {
                                voxels[x, y, z].tileObject = TakeGO.Oak;
                            }
                            else if (ran.Next(0, 20) == 3)
                            {
                                voxels[x, y, z].tileObject = TakeGO.Pine;
                            }
                        }
                    }
                    //Debug.Log("Desntisy " + voxels[x, y, z].density);
                }
            }
        }

        List<VerticeVoxel> verts = new List<VerticeVoxel>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(voxels, width, height, length, verts, indices, uvs);

        //A mesh in unity can only be made up of 65000 verts.
        //Need to split the verts between multiple meshes.

        int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
        int numMeshes = verts.Count / maxVertsPerMesh + 1;

        TileThreadHelp tileThreadHelp = new TileThreadHelp();
        tileThreadHelp.MeshList = new Dictionary<TypeBlocks, List<Vector3>>();

        for (int i = 0; i < verts.Count; i++)
        {
            if (!tileThreadHelp.MeshList.ContainsKey(verts[i].type))
            {
                tileThreadHelp.MeshList.Add(verts[i].type, new List<Vector3>() { verts[i].vert });
            }
            else
            {
                tileThreadHelp.MeshList[verts[i].type].Add(verts[i].vert);
            }
        }

        tileThreadHelp.voxels = voxels;

        return tileThreadHelp;
    }

    protected static readonly int[,] VertexOffset = new int[,]
    {
            {0, 0, 0},{1, 0, 0},{1, 1, 0},{0, 1, 0},
            {0, 0, 1},{1, 0, 1},{1, 1, 1},{0, 1, 1}
    };

    public float CalculateDensity(int worldPositionX, int worldPositionY, int worldPositionZ)
    {
        return worldPositionY - (float)new LibNoise.Unity.Generator.Perlin(0.5f, 0.6f, 2.15f, 10, seed, LibNoise.Unity.QualityMode.Low).GetValue(worldPositionX, worldPositionZ, 0) / 50;
    }

    public float CalculateDensity2(int worldPositionX, int worldPositionY, int worldPositionZ)
    {
        return worldPositionY - (float)new LibNoise.Unity.Generator.Perlin(2f, 0.6f, 2.15f, 10, seed, LibNoise.Unity.QualityMode.Low).GetValue(worldPositionX, worldPositionZ, 0) / 50;
    }

    private float OctaveNoise(float x, float y, float frequency, int octaveCount)
    {
        float value = 0;

        for (int i = 0; i < octaveCount; i++)
        {
            int octaveModifier = (int)math.pow(2, i);

            // (x+1)/2 because noise.snoise returns a value from -1 to 1 so it needs to be scaled to go from 0 to 1.
            float pureNoise = (noise.snoise(new float2(octaveModifier * x * frequency, octaveModifier * y * frequency)) + 1) / 2f;
            value += pureNoise / octaveModifier;
        }

        return value;
    }
}


public struct ChunkSeria
{
    public Chunk3 Position;
    public GameObject obj;

    public ChunkSeria(Chunk3 _Position, GameObject _obj)
    {
        Position = _Position;
        obj = _obj;
    }
}

public struct TileThreadHelp
{
    public TileNew[,,] voxels;
    public Dictionary<TypeBlocks, List<Vector3>> MeshList;
}