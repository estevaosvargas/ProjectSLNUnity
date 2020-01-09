using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ChunkInfo
{
    public int x = 0;
    public int z = 0;
    public Chunk ThisChunk;

    public ChunkInfo(int x, int z, Chunk chu)
    {
        this.x = x;
        this.z = z;
        this.ThisChunk = chu;
    }
}

public class Chunk : MonoBehaviour
{
    public GameObject lightobj;
    public GameObject TileObj;
    public static int Size = 10;
    public Tile[,] tiles { get; private set; }
    public List<Tile> tilelist = new List<Tile>();

    public List<Entity> Entitys = new List<Entity>();
    public List<long> Players = new List<long>();
    public Dictionary<Tile, GameObject> TransTile = new Dictionary<Tile, GameObject>();
    Dictionary<Tile, GameObject> tileGOmap;

    private float timetemp = 0;
    private float timetemp2 = 0;
    public float TimeUpdate = 10;

    public bool HaveAnyBuild = false;
    public bool AsSave = false;

    public Material DefaultTileMaterial;
    public Material DefaultTransMaterial;

    public GameObject MeshTile;

    void Awake()
    {
        if (Game.GameManager.SinglePlayer)
            MakeChunk();
    }

    void Start()
    {
        if (Game.GameManager.SinglePlayer)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    tiles[i, j].RefreshTile();
                }
            }

            GenerateTilesLayer(tiles);

            Game.WorldGenerator.LoadNewChunks(this);

            if (HaveAnyBuild)
            {
                SaveChunk();
            }
        }
        else if (DarckNet.Network.IsServer)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    #region SetUpPathGrid
                    if (!Game.PathGrid.tiles.ContainsKey(new Vector2(tiles[i, j].x, tiles[i, j].z)))
                    {
                        if (tiles[i, j].CanWalk)
                        {
                            if (tiles[i, j].typego == TakeGO.empty && tiles[i, j].PLACER_DATA == Placer.empty)
                            {
                                int coust = tiles[i, j].CanWalk ? 0 : 1;
                                Game.PathGrid.tiles.Add(new Vector2(tiles[i, j].x, tiles[i, j].z), new Node(tiles[i, j].CanWalk, new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z), coust));
                            }
                            else
                            {
                                Game.PathGrid.tiles.Add(new Vector2(tiles[i, j].x, tiles[i, j].z), new Node(false, new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z), 1));
                            }
                        }
                        else
                        {
                            Game.PathGrid.tiles.Add(new Vector2(tiles[i, j].x, tiles[i, j].z), new Node(false, new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z), 1));
                        }
                    }
                    else
                    {
                        if (tiles[i, j].typego == TakeGO.empty && tiles[i, j].PLACER_DATA == Placer.empty)
                        {
                            int coust = tiles[i, j].CanWalk ? 0 : 1;
                            Game.PathGrid.tiles[new Vector2(tiles[i, j].x, tiles[i, j].z)] = new Node(tiles[i, j].CanWalk, new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z), coust);
                        }
                        else
                        {
                            Game.PathGrid.tiles[new Vector2(tiles[i, j].x, tiles[i, j].z)] = new Node(false, new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z), 1);
                        }
                    }
                    #endregion
                }
            }

            if (HaveAnyBuild)
            {
                SaveChunk();
            }
        }
    }

    public Tile[] MakeChunk()
    {
        tileGOmap = new Dictionary<Tile, GameObject>();
        tiles = new Tile[Size, Size];
        Game.WorldGenerator.ChunksList.Add(this);
        bool havesave = false;

        if (File.Exists(Path.GetFullPath("Saves./" + Game.GameManager.WorldName + "./" + "chunks./" + Game.WorldGenerator.CurrentWorld.ToString() + (int)transform.position.x + "," + (int)transform.position.z) + ".chunkdata"))
        {
            ChunkSerializable chunkloaded = SaveWorld.Load(Game.WorldGenerator.CurrentWorld.ToString() + (int)transform.position.x + "," + (int)transform.position.z);
            tiles = chunkloaded.tiles;
            AsSave = true;
            havesave = true;
        }

        #region TileGen
        for (int i = 0; i < Size * TileObj.transform.localScale.x; i++)
        {
            for (int j = 0; j < Size * TileObj.transform.localScale.y; j++)
            {
                if (!havesave) 
                { 
                    tiles[i, j] = new Tile(i + (int)transform.position.x, 0, j + (int)transform.position.z, new ChunkInfo((int)transform.position.x, (int)transform.position.z, this)); 
                } 
                else 
                { 
                    tiles[i, j].TileChunk = new ChunkInfo((int)transform.position.x, (int)transform.position.z, this); 
                }

                Vector3 point = new LibNoise.Unity.Generator.Voronoi(0.009f, 2, Game.GameManager.Seed, false).GetPoint(tiles[i, j].x, tiles[i, j].z, 0);

                tiles[i, j].CityPoint = new DataVector3((int)point.x, (int)point.y, 0);


                if (tiles[i, j].OwnedByCity)
                {
                    Game.CityManager.SetUpCity(tiles[i, j].CityPoint);//Spawn the vilangers, in world, out side the buildings
                }

                tiles[i, j].SetUpTile(tiles[i, j]);
                tiles[i, j].RegisterOnTileTypeChange(OnTileTypeChange);

                if (!Game.GameManager.SinglePlayer || !DarckNet.Network.IsClient)
                {
                    tiles[i, j].IsServerTile = true;
                }

                //Set Up Tree OBject
                SpawnCityEntity(tiles[i, j]);
                SetupObjects(tiles[i, j]);
                SetUpTileTree(tiles[i, j]);

                if (tiles[i, j].CanWalk)
                {
                    if (Random.Range(1, 125) > 123)
                    {
                        SpawnNetWorkObject(tiles[i, j]);
                    }
                }

                //OnTileTypeChange(tiles[i, j]);
                //TileTransitionChange(tiles[i, j]);
                tilelist.Add(tiles[i, j]);
            }
        }
        #endregion

        return tilelist.ToArray();
    }

    public void RefreshChunkTile()
    {
        if (MeshTile != null)
        {
            MeshFilter filter = MeshTile.GetComponent<MeshFilter>();
            MeshData data = new MeshData();
            data.UpdateUv(tiles);
            filter.mesh.uv = data.UVs.ToArray();
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

    private void SpawnNetWorkObject(Tile tile)
    {
        GameObject obj3 = DarckNet.Network.Instantiate(Game.SpriteManager.GetPrefabOnRecources("Prefabs/AI/Slime"), new Vector3(tile.x + Random.Range(1, 5), tile.y, tile.z + Random.Range(1, 5)), Quaternion.identity, Game.WorldGenerator.World_ID);
        Entitys.Add(obj3.GetComponent<EntityLife>());
        obj3.GetComponent<EntityLife>().PrefabName = "Slime";

        GameObject Cow = DarckNet.Network.Instantiate(Game.SpriteManager.GetPrefabOnRecources("Prefabs/AI/Cow"), new Vector3(tile.x + Random.Range(1, 5), tile.y, tile.z + Random.Range(1, 5)), Quaternion.identity, Game.WorldGenerator.World_ID);
        Entitys.Add(Cow.GetComponent<EntityLife>());
        Cow.GetComponent<EntityLife>().PrefabName = "Cow";
        Cow.GetComponent<SmartEntity>().Cuerrent_Chunk = this;
    }

    public void OnTileTypeChange(Tile tile)
    {
        /*SpriteRenderer spritee = tileGOmap[tile].GetComponent<SpriteRenderer>();
        TileObj tilescript = tileGOmap[tile].GetComponentInChildren<TileObj>();*/

        //spritee.color = Get.ColorBiome(tile.TileBiome, tile.type);
        //spritee.sprite = Game.SpriteManager.GetSprite(tile);
        //tile.spritetile = spritee;

        #region SetUpPathGrid
        if (!Game.PathGrid.tiles.ContainsKey(new Vector2(tile.x, tile.z)))
        {
            if (tile.CanWalk)
            {
                if (tile.typego == TakeGO.empty && tile.PLACER_DATA == Placer.empty)
                {
                    int coust = tile.CanWalk ? 0 : 1;
                    Game.PathGrid.tiles.Add(new Vector2(tile.x, tile.z), new Node(tile.CanWalk, new Vector3(tile.x, tile.y, tile.z), coust));
                }
                else
                {
                    Game.PathGrid.tiles.Add(new Vector2(tile.x, tile.z), new Node(false, new Vector3(tile.x, tile.y, tile.z), 1));
                }
            }
            else
            {
                Game.PathGrid.tiles.Add(new Vector2(tile.x, tile.z), new Node(false, new Vector3(tile.x, tile.y, tile.z), 1));
            }
        }
        else
        {
            if (tile.typego == TakeGO.empty && tile.PLACER_DATA == Placer.empty)
            {
                int coust = tile.CanWalk ? 0 : 1;
                Game.PathGrid.tiles[new Vector2(tile.x, tile.z)] = new Node(tile.CanWalk, new Vector3(tile.x, tile.y, tile.z), coust);
            }
            else
            {
                Game.PathGrid.tiles[new Vector2(tile.x, tile.z)] = new Node(false, new Vector3(tile.x, tile.y, tile.z), 1);
            }
        }
        #endregion
    }

    public void TileTransitionChange(Tile tile)
    {
        if (tile != null)
        {
            Game.SpriteManager.GetTranssition(tile);

            if (TransTile.ContainsKey(tile))
            {
                Destroy(TransTile[tile]);
                TransTile.Remove(tile);
            }

            for (int i = 0; i < tile.TileTran.Length; i++)
            {
                if (!TransTile.ContainsKey(tile))
                {
                    GameObject TileGo = new GameObject("Tile_Transition_" + tile.TileTran[i].Name);

                    SpriteRenderer Render = TileGo.AddComponent<SpriteRenderer>();
                    Render.material = DefaultTransMaterial;
                    TileGo.transform.position = new Vector3(tile.x, tile.y + 0.0001f, tile.z);
                    TileGo.transform.Rotate(new Vector3(90,0,0), Space.Self);
                    TileGo.transform.SetParent(this.transform, true);

                    Render.sprite = Game.SpriteManager.GetSprite(tile.TileTran[i].Name);

                    Render.color = Render.color * Get.ColorBiome(tile.TileTran[i].Biome, tile.TileTran[i].type);

                    Render.sortingOrder = Get.GetTileRenIndex(tile.TileTran[i].type);

                    TransTile.Add(tile, TileGo);
                }
            }
        }

        if (tile.type == TypeBlock.Water)
        {
            /*tilescript.TILEANIMATED = true;
            if (tilescript.TILEANIMATOR == null)
            {
                /*tilescript.TILEANIMATOR = tilescript.gameObject.AddComponent<Animator>();
                tilescript.TILEANIMATOR.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/WaterAnimator");
            }*/
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < Size * TileObj.transform.localScale.x; i++)
        {
            for (int j = 0; j < Size * TileObj.transform.localScale.y; j++)
            {
                if (Game.PathGrid.tiles.ContainsKey(new Vector2(tiles[i, j].x, tiles[i, j].z)))
                {
                    Game.PathGrid.tiles.Remove(new Vector2(tiles[i, j].x, tiles[i, j].z));
                }
            }
        }

        foreach (var ai in Entitys)
        {
            if (ai != null)
            {
                DarckNet.Network.Destroy(ai.gameObject);
            }
        }

        Game.WorldGenerator.ChunksList.Remove(this);
    }

    /// <summary>
    /// Setup ChunksObject, if the obejct is a entity, they need to be spawned on network, if dont just in local chunk
    /// </summary>
    /// <param name="tile"></param>
    private void SetupObjects(Tile tile)
    {
        if (Get.GetPlacerEntity(tile.PLACER_DATA))//Network Entity
        {
            if (tile.PLACER_DATA != Placer.empty)
            {
                GameObject obj = DarckNet.Network.Instantiate(Game.SpriteManager.Getplacerbyname(tile.PLACER_DATA.ToString()), new Vector3(tile.x, tile.y, tile.z), Quaternion.identity, Game.WorldGenerator.World_ID);
                tile.ObjThis = obj;
                Entitys.Add(obj.GetComponent<ObjectEntity>());
            }
        }
        else//Local Object, Spawned inside of chunk
        {
            if (tile.PLACER_DATA != Placer.empty)
            {
                string buildid = ((int)tile.CityPoint.x + (int)tile.CityPoint.y * tile.x + tile.z).ToString();

                GameObject PLACER_OBJ = Instantiate(Game.SpriteManager.Getplacerbyname(tile.PLACER_DATA.ToString()), new Vector3(tile.x, tile.y, tile.z), Quaternion.identity);
                PLACER_OBJ.transform.SetParent(this.transform, true);
                tile.ObjThis = PLACER_OBJ;

                if (PLACER_OBJ.GetComponent<CityBase>())
                {
                    City Currentcitty = Game.CityManager.GetCity(tile.CityPoint);

                    if (Currentcitty != null)
                    {
                        Currentcitty.IsLoaded = true;

                        PLACER_OBJ.GetComponent<CityBase>().BuildId = buildid;
                        PLACER_OBJ.GetComponent<CityBase>().citypoint = tile.CityPoint;

                        if (!Currentcitty.CityBuildings.ContainsKey(buildid))
                        {
                            Currentcitty.CityBuildings.Add(buildid, new CityBaseSerialization(buildid, tile.CityPoint, new DataVector3(tile.x, tile.y, tile.z), tile.PLACER_DATA));
                            Currentcitty.CityBuildings[buildid].Temp_objc = PLACER_OBJ.GetComponent<CityBase>();
                            PLACER_OBJ.GetComponent<CityBase>().NewBuild();
                        }
                        else
                        {
                            Currentcitty.CityBuildings[buildid].Temp_objc = PLACER_OBJ.GetComponent<CityBase>();
                            Currentcitty.CityBuildings[buildid].BuildType = tile.PLACER_DATA;
                            PLACER_OBJ.GetComponent<CityBase>().LoadBuild();
                        }

                        HaveAnyBuild = true;
                    }
                }
            }
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

    private void SpawnCityEntity(Tile tile)
    {
        if (tile.OwnedByCity)
        {
            CitzenCredential entity = Game.CityManager.GetOutSideEntity(tile.CityPoint, new DataVector3(tile.x, tile.y, tile.z));
            if (entity != null)
            {
                Game.CityManager.SpawnNewEntity(entity, new Vector3(tile.x, 0, tile.z));
            }
        }
    }

    public Tile GetTileAt(int x, int z)
    {
        return tiles[x - (int)transform.position.x, z - (int)transform.position.z];
    }

    public void SaveChunk()
    {
        if (Game.GameManager.SinglePlayer)
        {
            SaveWorld.Save(new ChunkSerializable(this), Game.WorldGenerator.CurrentWorld.ToString() + (int)transform.position.x + "," + (int)transform.position.z);
            AsSave = true;
        }
        else if (Game.GameManager.MultiPlayer)
        {

        }
    }

    void Update()
    {
#if Client
        if (Time.time > timetemp + TimeUpdate)
        {
            UpdateChunk();
            
            if (AsSave)// if this chunk can be saved.
            {
                SaveChunk();
            }

            Debug.Log("Chunk's Updated!");
            timetemp = Time.time;
        }

        /*if (Time.time > timetemp2 + 1)
        {
            //UpdateLiquid();

            //Debug.Log("Liquid Updated!");
            timetemp2 = Time.time;
        }*/
#endif
#if Server
        if (Time.time > timetemp + TimeUpdate)
        {
            UpdateChunk();

            timetemp = Time.time;
        }
#endif
    }

    public void UpdateLiquid()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (tiles[i, j].type == TypeBlock.Dirt)
                {
                    Tile[] ney = tiles[i, j].GetNeighboors();

#region Water
                    if (ney[0] != null && ney[0].type == TypeBlock.Water || ney[1] != null && ney[1].type == TypeBlock.Water || ney[2] != null && ney[2].type == TypeBlock.Water || ney[3] != null && ney[3].type == TypeBlock.Water)
                    {
                        //tiles[i, j].SetTileType(TypeBlock.Water);
                        tiles[i, j].Reset();
                        return;
                    }
#endregion Water
                }
            }
        }
    }

    int maxia = 0;

    public void UpdateChunk()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (tiles[i, j].type == TypeBlock.DirtGrass)
                {
                    if (DataTime.Hora >= tiles[i, j].Hora && DataTime.Dia >= tiles[i, j].Dia && DataTime.Mes >= tiles[i, j].Mes)
                    {
                        Tile[] ney = tiles[i, j].GetNeighboors();

#region Grama
                        if (ney[0] != null && ney[0].type == TypeBlock.Grass || ney[1] != null && ney[1].type == TypeBlock.Grass || ney[2] != null && ney[2].type == TypeBlock.Grass || ney[3] != null && ney[3].type == TypeBlock.Grass)
                        {
                            tiles[i, j].PerlinSetType(TypeBlock.Grass);
                            tiles[i, j].Reset();
                        }
#endregion
                    }
                    else if (DataTime.Hora <= tiles[i, j].Hora && DataTime.Dia > tiles[i, j].Dia && DataTime.Mes > tiles[i, j].Mes)
                    {
                        Tile[] ney = tiles[i, j].GetNeighboors();

#region Grama
                        if (ney[0] != null && ney[0].type == TypeBlock.Grass || ney[1] != null && ney[1].type == TypeBlock.Grass || ney[2] != null && ney[2].type == TypeBlock.Grass || ney[3] != null && ney[3].type == TypeBlock.Grass)
                        {
                            tiles[i, j].PerlinSetType(TypeBlock.Grass);
                            tiles[i, j].Reset();
                        }
#endregion
                    }
                }
            }
        }
    }

    //MultiPlayer Client, Romover Essa Parte e Adicionar em um lugar só
    public void ClientSetUpChunk(Tile[] tile)
    {
#region TileGen
        tileGOmap = new Dictionary<Tile, GameObject>();
        tiles = new Tile[Size, Size];
        Game.WorldGenerator.ChunksList.Add(this);

        for (int v = 0; v < tile.Length; v++)
        {
            int i = tile[v].x - (int)transform.position.x;
            int j = tile[v].z - (int)transform.position.z;

            tiles[i, j] = tile[v];

            tiles[i, j].RegisterOnTileTypeChange(OnTileTypeChange);

            GameObject TileGo = GameObject.Instantiate(TileObj, new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z), Quaternion.identity);

            TileGo.name = "Tile_" + tiles[i, j].x + "_" + tiles[i, j].z;

            TileGo.transform.position = new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z);
            TileGo.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
            TileGo.transform.SetParent(this.transform, true);

            tiles[i, j].TileObj = TileGo.GetComponent<TileObj>();

            tiles[i, j].TileChunk = new ChunkInfo((int)transform.position.x, (int)transform.position.z, this);

            //SetUp Tree object, spawn tree object and transalte for a new position
            SetupObjects(tiles[i, j]);
            SetUpTileTree(tiles[i, j]);

            if (tiles[i, j] == null)
            {
                Debug.Log("Null tiles[x, y]");
            }

            if (TileGo == null)
            {
                Debug.Log("Null TileGo");
            }

            tileGOmap.Add(tiles[i, j], TileGo);

            OnTileTypeChange(tiles[i, j]);
        }

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                tiles[i, j].RefreshTile();
            }
        }

        GenerateTilesLayer(tiles);

        Game.WorldGenerator.LoadNewChunks(this);
        #endregion
    }
}

public class MeshData
{
    public List<Vector3> vertices;
    public List<Vector2> UVs;
    public List<int> triangles;

    public MeshData(Tile[,] tile)
    {
        vertices = new List<Vector3>();
        UVs = new List<Vector2>();
        triangles = new List<int>();

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                CreateSquare(tile[x, z].x, tile[x, z].z, tile[x, z]);
            }
        }
    }

    public MeshData()
    {
        UVs = new List<Vector2>();
    }

    public void UpdateUv(Tile[,] tile)
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                UVs.AddRange(Game.SpriteManager.GetTileUVs(tile[x, z]));
            }
        }
    }

    void CreateSquare(int x, int z, Tile tile)
    {
        vertices.Add(new Vector3(x + 0, 0, z + 0));
        vertices.Add(new Vector3(x + 1, 0, z + 0));
        vertices.Add(new Vector3(x + 0, 0, z + 1));
        vertices.Add(new Vector3(x + 1, 0, z + 1));

        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 4);

        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 4);

        UVs.AddRange(Game.SpriteManager.GetTileUVs(tile));
    }
}

[System.Serializable]
public class ChunkSerializable
{
    public Tile[,] tiles;

    public ChunkSerializable(Chunk chunk)
    {
        tiles = chunk.tiles;
    }
}
