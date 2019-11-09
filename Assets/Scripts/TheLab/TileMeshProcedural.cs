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

    Dictionary<string, Vector2[]> tileUVMap;
    public float scaleFactor = 0.5f;
    public int[] triss;

    public Vector3[] verts;
    public Vector2[] uvs;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();

        GenerateFloor();
        UpdateFloor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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

        triss = triangles;
        verts = vertices;
        uvs = new Vector2[verts.Length];
    }

    void UpdateFloor()
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
