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
    public Block[,,] Blocks;

    private List<Entity> Entitys = new List<Entity>();
    private List<long> Players = new List<long>();

    public Material m_Grass;

    private List<Block> BlocksList = new List<Block>();

    void Start()
    {

    }

    public void MakeMesh(Block[,,] voxelData)
    {
        chunkPosition = transform.position;
        Blocks = voxelData;

        for (int x = 0; x < World.ChunkSize; x++)
        {
            for (int y = 0; y < World.ChunkSize; y++)
            {
                for (int z = 0; z < World.ChunkSize; z++)
                {
                    Blocks[x, y, z].CurrentChunk = transform.position;//Set this chunk to the tile
                    /*if (Blocks[x, y, z].HaveTree)
                    {
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        obj.transform.position = new Vector3(Blocks[x, y, z].x, Blocks[x, y, z].y, Blocks[x, y, z].z);
                        obj.transform.SetParent(transform, true);
                    }*/

                    BlocksList.Add(Blocks[x, y, z]);
                }
            }
        }

        UpdateMeshChunk();
    }

    public void UpdateMeshData(MeshDataThread meshDataThread)
    {
        GameObject meshGO = new GameObject("TileLayer_" + transform.position.x + "_" + transform.position.z);
        meshGO.transform.SetParent(this.transform, true);

        MeshFilter filter = meshGO.AddComponent<MeshFilter>();
        MeshRenderer render = meshGO.AddComponent<MeshRenderer>();
        MeshCollider mMeshcollider = meshGO.AddComponent<MeshCollider>();

        render.material = m_Grass;

        Mesh mesh = filter.mesh;

        mesh.vertices = meshDataThread.vertices;
        mesh.triangles = meshDataThread.triangles;
        mesh.uv = meshDataThread.uvs;

        mesh.RecalculateNormals();

        mMeshcollider.sharedMesh = mesh;
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

    private void OnDestroy()
    {
        Blocks = null;
        BlocksList.Clear();
    }

    private void OnDrawGizmos()
    {
        foreach (var item in BlocksList)
        {
            if (item.Type != TypeBlock.Air)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(new Vector3(item.x + 0.5f, item.y + 0.5f, item.z + 0.5f), Vector3.one);
            }
        }
        Gizmos.DrawWireCube(transform.position + new Vector3(World.ChunkSize / 2, World.ChunkSize / 2, World.ChunkSize / 2), new Vector3(World.ChunkSize, World.ChunkSize, World.ChunkSize));
    }
}
