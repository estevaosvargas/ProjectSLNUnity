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
    public List<Vector2> UVs = new List<Vector2>();
    Dictionary<string, Vector2[]> tileUVMap;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();

        GenerateFloor();
        UpdateFloor();
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

            tileUVMap.Add(s.name, uvs);
        }
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
    }

    void UpdateFloor()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UVs.ToArray();
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
