using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3 chunkPosition;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public float QueeTime = 5;
    public Block[,] Blocks;
    private List<Entity> Entitys = new List<Entity>();
    private List<long> Players = new List<long>();
    private double ChunkSeed;
    public Material WaterMaterial;
    public bool Ready = false;
    private bool _isVisible;
    private VoxelMesh TerrainMesh;
    private VoxelMesh WaterMesh;
    private MeshFilter waterObject;

    private System.Action ActionUpdateMEsh;

    public float _WaveSpeed = 1;
    public float _WaveFrequency = 5;
    private FastNoise _WaveNoise;

    void Start()
    {
        _WaveNoise = new FastNoise((transform.position.ToString()).GetHashCode());
        _WaveNoise.SetFrequency(_WaveFrequency);
    }

    private void Update()
    {
        if (ActionUpdateMEsh != null)
        {
            ActionUpdateMEsh.Invoke();
            ActionUpdateMEsh = null;
        }

        if (_isVisible)
        {
            /*if (waterObject != null)
            {
                _WaveNoise.SetFrequency(_WaveFrequency);
                var verts = waterObject.mesh.vertices;

                for (int x = 0; x < World.ChunkSize; x++)
                {
                    for (int z = 0; z < World.ChunkSize; z++)
                    {
                        verts[index(x, z)].y = Mathf.Lerp(getWaveHeight(x + (int)chunkPosition.x, z + (int)chunkPosition.z), 0, waterObject.mesh.colors[index(x, z)].a);
                    }
                }

                
                waterObject.mesh.vertices = verts;
                waterObject.mesh.RecalculateNormals();
            }*/
        }
    }

    private int index(int x, int z)
    {
        return x * (10 + 1) + z;
    }

    private float getWaveHeight(int x, int z)
    {
        //return _WaveNoise.GetPerlinFractal(x + Time.timeSinceLevelLoad * _WaveSpeed, z + Time.timeSinceLevelLoad * _WaveSpeed);
        return _WaveNoise.GetPerlin(x + Time.time * _WaveSpeed, z + Time.time * _WaveSpeed) * 20;
    }

    public void StartUpChunk()
    {
        Blocks = new Block[World.ChunkSize, World.ChunkSize];

        chunkPosition = transform.position;

        System.Random rand = new System.Random();

        double a = rand.NextDouble();
        double b = rand.NextDouble();

        ChunkSeed = chunkPosition.x * a + chunkPosition.z * b + GameManager.Seed;

        Thread PopVoxelThread = new Thread(new ThreadStart(ThreadPopulateVoxel));
        PopVoxelThread.Start();

        //UpdateMeshChunk();
        //StartCoroutine(QueeObjects());
        //Game.World.LoadNewChunks(this);
    }

    #region ThreadRegion
    private void ThreadPopulateVoxel()
    {
        for (int x = 0; x < World.ChunkSize; x++)
        {
            for (int z = 0; z < World.ChunkSize; z++)
            {
                Blocks[x, z] = new Block(x + (int)chunkPosition.x, z + (int)chunkPosition.z, new VoxelVector3(chunkPosition), Game.World.GetglobalNoise.GetPerlin(x + (int)chunkPosition.x, z + (int)chunkPosition.z) * 20);
            }
        }

        UpdateChunksSurround();
    }

    private void ThreadMakeMesh()
    {
        MeshData data = new MeshData(Blocks);
        TerrainMesh = new VoxelMesh();

        TerrainMesh.verts = data._vertices.ToArray();
        TerrainMesh.uvs = data._UVs.ToArray();
        TerrainMesh.indices = data._triangles.ToArray();
        TerrainMesh.colors = data._colors.ToArray();

        if (data._HaveWater)
        {
            WaterMesh = data.MakeWaterMesh(Blocks);
        }

        ActionUpdateMEsh = () => UpdateMeshData();
    }
    #endregion

    private void SpawnTrees(Block block)
    {
        if (block.typego != TakeGO.empty)
        {
            GameObject trees = null;
            trees = Instantiate(Game.SpriteManager.GetPrefabbyname(block.typego.ToString()), new Vector3(block.x, block.hight, block.z), Quaternion.identity);
            trees.transform.SetParent(this.transform, true);
            //block.BlockObject = trees; Subistoir esse por uma lista de objetos

            if (block.typego != TakeGO.RockWall)
            {
                System.Random randomValue = new System.Random(GameManager.Seed + (int)block.x + (int)block.z);
                float size = Random.Range(0f, 0.5f);
                trees.transform.position = new Vector3(block.x + (float)randomValue.NextDouble(), block.hight - 0.2f, block.z + (float)randomValue.NextDouble());
                trees.transform.localScale = new Vector3(trees.transform.localScale.x + size, trees.transform.localScale.y + size, trees.transform.localScale.z + size);
            }

            if (trees.GetComponent<Trees>())
            {
                trees.GetComponent<Trees>().ThisTreeTile = block;
            }
        }
    }

    public void SpawnEntitys(Block block)
    {
        Vector3 position = new Vector3(block.x, block.hight, block.z);

        GameObject obj = DarckNet.Network.Instantiate(Game.SpriteManager.GetPrefabOnRecources("Prefabs/AI/Skeleton"), position, Quaternion.identity, Game.MapManager.World_ID);
        AddEntity(obj.GetComponent<Entity>());
        obj.GetComponent<DumbEntity>().SetUp(this);
    }

    private void UpdateMeshData()
    {
        if (!Ready)
        {
            FirstSpawnChunk();
            Ready = true;
        }

        Mesh mesh = new Mesh();

        mesh.vertices = TerrainMesh.verts;
        mesh.triangles = TerrainMesh.indices;
        mesh.uv = TerrainMesh.uvs;
        mesh.colors = TerrainMesh.colors;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;


        if (WaterMesh != null)
        {
            if (waterObject != null)
            {
                MeshFilter waterMeshFilter = waterObject.GetComponent<MeshFilter>();
                MeshRenderer waterMeshRenderer = waterObject.GetComponent<MeshRenderer>();

                Mesh meshWater = new Mesh();

                meshWater.SetVertices(WaterMesh.verts);
                meshWater.SetTriangles(WaterMesh.indices, 0);
                meshWater.SetUVs(0, WaterMesh.uvs);
                meshWater.SetColors(WaterMesh.colors);

                meshWater.RecalculateNormals();
                meshWater.RecalculateBounds();
                meshWater.RecalculateTangents();

                meshWater.Optimize();

                waterMeshFilter.mesh = meshWater;
                waterMeshRenderer.material = WaterMaterial;

                WaterMesh.ClearMesh();
            }
            else
            {
                GameObject obj = new GameObject("WaterMesh");
                waterObject = obj.AddComponent<MeshFilter>();
                MeshRenderer waterMeshRenderer = obj.AddComponent<MeshRenderer>();
                waterObject.transform.position = transform.position + new Vector3(0, -1.9f, 0);
                waterObject.transform.SetParent(transform, true);

                Mesh meshWater = new Mesh();

                meshWater.SetVertices(WaterMesh.verts);
                meshWater.SetTriangles(WaterMesh.indices, 0);
                meshWater.SetUVs(0, WaterMesh.uvs);
                meshWater.SetColors(WaterMesh.colors);

                meshWater.RecalculateNormals();
                meshWater.RecalculateBounds();
                meshWater.RecalculateTangents();

                meshWater.Optimize();

                waterObject.mesh = meshWater;
                waterMeshRenderer.material = WaterMaterial;

                WaterMesh.ClearMesh();
            }
        }

        TerrainMesh.ClearMesh();
    }

    public void FirstSpawnChunk()
    {
        for (int x = 0; x < World.ChunkSize; x++)
        {
            for (int z = 0; z < World.ChunkSize; z++)
            {
                SpawnTrees(Blocks[x, z]);

                /*if (Get.EntityCanSpawn(Blocks[x, y, z]))
                {
                    if (Blocks[x, y, z].typego == TakeGO.empty)
                    {
                        System.Random randomValue = new System.Random((int)ChunkSeed + Blocks[x, y, z].x * Blocks[x, y, z].z);
                        if (randomValue.Next(0, 100) == 10)
                        {
                            SpawnEntitys(Blocks[x, y, z]);
                        }
                    }
                }*/
            }
        }
    }
    /// <summary>
    /// Frefresh mesh data with new changed voxel data
    /// </summary>
    public void RefreshMeshData()
    {
        Thread MakeMeshThread = new Thread(new ThreadStart(ThreadMakeMesh));
        MakeMeshThread.Start();
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

    public static double GetChunkSeed(VoxelVector3 chunkPosition)
    {
        System.Random rand = new System.Random();

        double a = rand.NextDouble();
        double b = rand.NextDouble();

        return chunkPosition._x * a + chunkPosition._z * b + GameManager.Seed;
    }

    public void ClearChunk()
    {
        meshFilter.mesh.Clear();

        meshFilter.mesh = null;
        meshCollider.sharedMesh = null;

        if (TerrainMesh != null)
        {
            TerrainMesh.ClearMesh();
        }

        if (WaterMesh != null)
        {
            WaterMesh.ClearMesh();
        }

        foreach (var item in Entitys)
        {
            if (item != null)
            {
                DarckNet.Network.Destroy(item.gameObject);
            }
        }

        Entitys.Clear();
        Players.Clear();
        Entitys = null;
        Blocks = null;
        Players = null;

        TerrainMesh = null;
        WaterMesh = null;
    }

    private void OnBecameVisible()
    {
        _isVisible = true;
    }

    private void OnBecameInvisible()
    {
        _isVisible = false;
    }

    private void OnDestroy()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(World.ChunkSize / 2, World.ChunkSizeY / 2, World.ChunkSize / 2), new Vector3(World.ChunkSize, World.ChunkSizeY, World.ChunkSize));
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
    public static readonly Vector3[] Verts = new Vector3[8]
    {
        new Vector3(0.0f,0.0f,0.0f),
        new Vector3(1.0f,0.0f,0.0f),
        new Vector3(1.0f,0.0f,0.0f),
        new Vector3(0.0f,0.0f,0.0f),
        new Vector3(0.0f,0.0f,1.0f),
        new Vector3(1.0f,0.0f,1.0f),
        new Vector3(1.0f,0.0f,1.0f),
        new Vector3(0.0f,0.0f,1.0f)
    };

    IEnumerator QueeObjects()
    {
        for (int i = 0; i < World.ChunkSize; i++)
        {
            for (int j = 0; j < World.ChunkSize; j++)
            {
                yield return new WaitForSeconds(QueeTime);
                //SpawnTrees(Blocks[i, j]);
            }
        }
    }

    public void UpdateChunksSurround()
    {
        RefreshMeshData();

        Chunk Direita = Game.World.GetChunkAt((int)chunkPosition.x + World.ChunkSize, (int)chunkPosition.z);//Direita
        Chunk Esquerda = Game.World.GetChunkAt((int)chunkPosition.x - World.ChunkSize, (int)chunkPosition.z);//Esquerda

        Chunk Frente = Game.World.GetChunkAt((int)chunkPosition.x, (int)chunkPosition.z + World.ChunkSize);//Frente
        Chunk Atras = Game.World.GetChunkAt((int)chunkPosition.x, (int)chunkPosition.z - World.ChunkSize);//Atras

        if (Direita)
        {
            Direita.RefreshMeshData();//Direita
        }

        if (Esquerda)
        {
            Esquerda.RefreshMeshData();//Esquerda
        }

        if (Frente)
        {
            Frente.RefreshMeshData();//Frente
        }

        if (Atras)
        {
            Atras.RefreshMeshData();//Atras
        }
    }
}


public class VoxelMesh
{
    public Vector3[] verts;
    public int[] indices;
    public Vector2[] uvs;
    public Color[] colors;

    public void ClearMesh()
    {
        verts = null;
        indices = null;
        uvs = null;
        colors = null;
    }
}