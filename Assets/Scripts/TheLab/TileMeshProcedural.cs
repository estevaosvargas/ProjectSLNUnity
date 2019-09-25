using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 public class TileMeshProcedural : MonoBehaviour
{

    public int floorWidth;
    public int floorHeight;

    MeshCollider meshCollider;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        GenerateFloor();
        UpdateFloor();
    }

    void GenerateFloor()
    {
        vertices = new Vector3[(floorWidth + 1) * (floorHeight + 1)];
        for (int i = 0, z = 0; z <= floorHeight; z++)
        {
            for (int x = 0; x <= floorWidth; x++)
            {
                //float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2.0f;
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        triangles = new int[floorWidth * floorHeight * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < floorHeight; z++)
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

        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= floorHeight; z++)
        {
            for (int x = 0; x <= floorWidth; x++)
            {
                uvs[i] = new Vector2((float)x + floorHeight, (float)z + floorWidth);
                i++;
            }
        }
    }

    void UpdateFloor()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        mesh.RecalculateBounds();
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