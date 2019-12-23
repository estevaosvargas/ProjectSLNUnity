using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChunk : MonoBehaviour
{
    public Tile[,] tiles { get; private set; }
    public int Size = 10;

    public Material DefaultTileMaterial;
    public Material DefaultTransMaterial;

    public GameObject MeshTile;

    void Start()
    {
        MakeChunk();

        GenerateTilesLayer(tiles);
    }

    void Update()
    {
        
    }

    public void MakeChunk()
    {
        tiles = new Tile[Size, Size];

        #region TileGen
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                tiles[i, j] = new Tile(i + (int)transform.position.x, 0, j + (int)transform.position.z, new ChunkInfo((int)transform.position.x, (int)transform.position.z, null), true);

                Vector3 point = new LibNoise.Unity.Generator.Voronoi(0.009f, 2, Game.GameManager.Seed, false).GetPoint(tiles[i, j].x, tiles[i, j].z, 0);

                tiles[i, j].CityPoint = new DataVector3((int)point.x, (int)point.y, 0);


                tiles[i, j].SetUpTile(tiles[i, j]);
                tiles[i, j].RegisterOnTileTypeChange(OnTileTypeChange);

                if (!Game.GameManager.SinglePlayer || !DarckNet.Network.IsClient)
                {
                    tiles[i, j].IsServerTile = true;
                }

                //Set Up Tree OBject
                SetupObjects(tiles[i, j]);
                SetUpTileTree(tiles[i, j]);
            }
        }
        #endregion
    }

    private void SetupObjects(Tile tile)
    {
        if (tile.PLACER_DATA != Placer.empty)
        {
            string buildid = ((int)tile.CityPoint.x + (int)tile.CityPoint.y * tile.x + tile.z).ToString();

            GameObject PLACER_OBJ = Instantiate(Game.SpriteManager.Getplacerbyname(tile.PLACER_DATA.ToString()), new Vector3(tile.x, tile.y, tile.z), Quaternion.identity);
            PLACER_OBJ.transform.SetParent(this.transform, true);
            tile.ObjThis = PLACER_OBJ;
        }
    }

    private void SetUpTileTree(Tile tile)
    {
        if (tile.typego != TakeGO.empty)
        {
            GameObject trees = null;
            trees = Instantiate(Game.SpriteManager.GetPrefabbyname(tile.typego.ToString()), new Vector3(tile.x, tile.y, tile.z), Quaternion.identity);
            trees.transform.SetParent(this.transform, true);
            tile.ObjThis = trees;

            if (tile.type != TypeBlock.Rock)
            {
                System.Random randomValue = new System.Random(Game.GameManager.Seed + tile.x + tile.z);
                trees.transform.position = new Vector3(tile.x + (float)randomValue.NextDouble(), tile.y, tile.z + (float)randomValue.NextDouble());
            }

            if (trees.GetComponent<Trees>())
            {
                trees.GetComponent<Trees>().ThisTreeTile = tile;
            }
        }
    }

    void GenerateTilesLayer(Tile[,] tiles)
    {
        MeshData data = new MeshData(tiles);

        GameObject meshGO = new GameObject("TileLayer_" + transform.position.x + "_" + transform.position.z);
        meshGO.transform.SetParent(this.transform);

        MeshFilter filter = meshGO.AddComponent<MeshFilter>();
        MeshRenderer render = meshGO.AddComponent<MeshRenderer>();
        MeshCollider mMeshcollider = meshGO.AddComponent<MeshCollider>();
        render.material = DefaultTileMaterial;

        Mesh mesh = filter.mesh;

        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.triangles.ToArray();
        mesh.uv = data.UVs.ToArray();

        mesh.RecalculateNormals();

        mMeshcollider.sharedMesh = mesh;

        MeshTile = meshGO;
    }

    public void OnTileTypeChange(Tile tile)
    {

    }
}
