using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3 chunkPosition;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public float QueeTime = 5;
    public Block[,] Blocks;//This is the 2d array for the terrain
    public VoxelBlock[,,] VoxelBlocks;//this is the 3d array for the caves and block voxeld based
    private List<Entity> Entitys = new List<Entity>();
    private List<long> Players = new List<long>();
    private double ChunkSeed;
    public Material m_Grass;
    private GameObject TerrainMesh;
    private int[] subdivision = new int[] { 0, 2, 3, 4, 6, 8, 9, 12, 16, 18, 24 };

    [Header("Subdive Mesh")]

    [Tooltip("Divide meshes in submeshes to generate more triangles")]
    [Range(0, 10)]
    public int subdivisionLevel;

    [Tooltip("Repeat the process this many times")]
    [Range(0, 10)]
    public int timesToSubdivide;

    void Start()
    {

    }

    public void StartUpChunk(Block[,] voxelData)
    {
        chunkPosition = transform.position;
        Blocks = voxelData;

        System.Random rand = new System.Random();

        double a = rand.NextDouble();
        double b = rand.NextDouble();

        ChunkSeed = chunkPosition.x * a + chunkPosition.z * b + GameManager.Seed;

        for (int x = 0; x < World.ChunkSize; x++)
        {
            for (int z = 0; z < World.ChunkSize; z++)
            {
                Blocks[x, z].CurrentChunk = transform.position;//Set this chunk to the tile
                SpawnTrees(Blocks[x, z]);
            }
        }
        //StartCoroutine(QueeObjects());
        Game.World.LoadNewChunks(this);
    }

    IEnumerator QueeObjects()
    {
        for (int i = 0; i < World.ChunkSize; i++)
        {
            for (int j = 0; j < World.ChunkSize; j++)
            {
                yield return new WaitForSeconds(QueeTime);
                SpawnTrees(Blocks[i, j]);
            }
        }
    }

    private void SpawnTrees(Block block)
    {
        if (block.typego != TakeGO.empty)
        {
            GameObject trees = null;
            trees = Instantiate(Game.SpriteManager.GetPrefabbyname(block.typego.ToString()), new Vector3(block.x, block.h, block.z), Quaternion.identity);
            trees.transform.SetParent(this.transform, true);
            block.BlockObject = trees;

            if (block.Type != TypeBlock.Rock)
            {
                if (block.typego != TakeGO.WeedTall)
                {
                    if (block.typego != TakeGO.Weed01)
                    {
                        if (block.typego != TakeGO.RockProp)
                        {
                            System.Random randomValue = new System.Random(GameManager.Seed + (int)block.x + (int)block.z);
                            float size = Random.Range(0f, 0.5f);
                            trees.transform.position = new Vector3(block.x + (float)randomValue.NextDouble(), block.h, block.z + (float)randomValue.NextDouble());
                            trees.transform.localScale = new Vector3(trees.transform.localScale.x + size, trees.transform.localScale.y + size, trees.transform.localScale.z + size);
                        }
                    }
                }
            }

            if (trees.GetComponent<Trees>())
            {
                trees.GetComponent<Trees>().ThisTreeTile = block;
            }
        }
    }

    public void UpdateMeshData(MeshDataThread meshDataThread)
    {
        if (TerrainMesh != null)
        {
            MeshFilter filter = TerrainMesh.GetComponent<MeshFilter>();
            MeshRenderer render = TerrainMesh.GetComponent<MeshRenderer>();
            MeshCollider mMeshcollider = TerrainMesh.GetComponent<MeshCollider>();

            render.material = m_Grass;

            Mesh mesh = filter.mesh;

            mesh.vertices = meshDataThread.vertices;
            mesh.triangles = meshDataThread.triangles;
            mesh.uv = meshDataThread.uvs;

            /*for (int i = 0; i < timesToSubdivide; i++)
            {
                MeshHelper.Subdivide(mesh, subdivision[subdivisionLevel]);
            }*/

            mesh.RecalculateNormals();
            filter.mesh = mesh;
            mMeshcollider.sharedMesh = mesh;
        }
        else
        {
            GameObject meshGO = new GameObject("TileLayer_" + transform.position.x + "_" + transform.position.z);
            meshGO.transform.SetParent(this.transform, true);

            TerrainMesh = meshGO;

            MeshFilter filter = meshGO.AddComponent<MeshFilter>();
            MeshRenderer render = meshGO.AddComponent<MeshRenderer>();
            MeshCollider mMeshcollider = meshGO.AddComponent<MeshCollider>();

            render.material = m_Grass;

            Mesh mesh = filter.mesh;

            mesh.vertices = meshDataThread.vertices;
            mesh.triangles = meshDataThread.triangles;
            mesh.uv = meshDataThread.uvs;

            /*for (int i = 0; i < timesToSubdivide; i++)
            {
                MeshHelper.Subdivide(mesh, subdivision[subdivisionLevel]);
            }*/

            mesh.RecalculateNormals();
            filter.mesh = mesh;
            mMeshcollider.sharedMesh = mesh;
        }
    }

    public void UpdateMeshChunk()
    {
        Game.World.UpdateMesh(this);
    }

    public void AddEntity(Entity entity)
    {
        if (!Entitys.Contains(entity))
        {
            Entitys.Add(entity);
        }
    }

    public void RemoveEntity(Entity entity)
    {
        if (Entitys.Contains(entity))
        {
            Entitys.Remove(entity);
        }
    }

    public double GetChunkSeed()
    {
        return ChunkSeed;
    }

    public static double GetChunkSeed(Vector3 chunkPosition)
    {
        System.Random rand = new System.Random();

        double a = rand.NextDouble();
        double b = rand.NextDouble();

        return chunkPosition.x * a + chunkPosition.z * b + GameManager.Seed;
    }

    private void OnDestroy()
    {
        Blocks = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(World.ChunkSize / 2, World.ChunkSize / 2, World.ChunkSize / 2), new Vector3(World.ChunkSize, World.ChunkSize, World.ChunkSize));
    }
}
