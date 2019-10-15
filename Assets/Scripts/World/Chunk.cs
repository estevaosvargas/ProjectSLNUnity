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

    public List<Entity> Entitys = new List<Entity>();
    public List<TileSave> Tiles_Save = new List<TileSave>();
    public List<long> Players = new List<long>();
    Dictionary<Tile, GameObject> tileGOmap;

    private float timetemp = 0;
    private float timetemp2 = 0;
    public float TimeUpdate = 10;

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
                            if (tiles[i, j].typego == TakeGO.empty && tiles[i, j].placerObj == Placer.empty)
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
                        if (tiles[i, j].typego == TakeGO.empty && tiles[i, j].placerObj == Placer.empty)
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
        }
    }

    public TileSave[] MakeChunk()
    {
        tileGOmap = new Dictionary<Tile, GameObject>();
        tiles = new Tile[Size, Size];
        Game.WorldGenerator.ChunksList.Add(this);
        bool havesave = false;

        if (File.Exists(Path.GetFullPath("Saves./" + Game.GameManager.WorldName + "./" + "chunks./" + Game.WorldGenerator.CurrentWorld.ToString() + (int)transform.position.x + "," + (int)transform.position.z)))
        {
            tiles = SaveWorld.Load(Game.WorldGenerator.CurrentWorld.ToString() + (int)transform.position.x + "," + (int)transform.position.z);
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

                tiles[i, j].SetUpTile(tiles[i, j]);
                tiles[i, j].RegisterOnTileTypeChange(OnTileTypeChange);

                if (Game.GameManager.SinglePlayer || DarckNet.Network.IsClient)
                {
                    GameObject TileGo = GameObject.Instantiate(TileObj, new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z), Quaternion.identity);
                    TileGo.SetActive(true);
                    TileGo.name = "Tile_" + tiles[i, j].x + "_" + tiles[i, j].z;
                    TileGo.transform.position = new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z);
                    TileGo.transform.SetParent(this.transform, true);

                    TileGo.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                    tileGOmap.Add(tiles[i, j], TileGo);
                    tiles[i, j].TileObj = TileGo.GetComponent<TileObj>();
                }
                else
                {
                    tiles[i, j].IsServerTile = true;
                }

                //Set Up Tree OBject
                SetUpTileTree(tiles[i, j]);
                SetUpPlacer(tiles[i, j]);

                if (tiles[i, j].CanWalk)
                {
                    if (Random.Range(1, 125) > 120)
                    {
                        SpawnNetWorkObject(tiles[i, j]);
                    }
                }

                //OnTileTypeChange(tiles[i, j]);
                //TileTransitionChange(tiles[i, j]);

                TileSave tilesa = new TileSave();
                tilesa.x = tiles[i, j].x;
                tilesa.z = tiles[i, j].z;
                tilesa.type = tiles[i, j].type;
                tilesa.placer = tiles[i, j].placerObj;
                tilesa.typego = tiles[i, j].typego;
                tilesa.IsColider = tiles[i, j].IsColider;
                tilesa.Emessive = tiles[i, j].Emessive;
                tilesa.ConnecyToNightboors = tiles[i, j].ConnecyToNightboors;
                tilesa.Biomeofthis = tiles[i, j].TileBiome;

                Tiles_Save.Add(tilesa);
            }
        }
        #endregion

        return Tiles_Save.ToArray();
    }

    private void SpawnNetWorkObject(Tile tile)
    {
        GameObject obj = DarckNet.Network.Instantiate(SpriteManager.Instance.GetPrefabOnRecources("Prefabs/Villager/Villager"), new Vector3(tile.x, tile.y, tile.z), Quaternion.identity, Game.WorldGenerator.World_ID);
        obj.GetComponent<Vilanger>().Born("VillagerTeste", this);
        Entitys.Add(obj.GetComponent<Vilanger>());
    }

    public void OnTileTypeChange(Tile tile)
    {
        SpriteRenderer spritee = tileGOmap[tile].GetComponent<SpriteRenderer>();
        LightToD light = tileGOmap[tile].GetComponentInChildren<LightToD>();
        TileObj tilescript = tileGOmap[tile].GetComponentInChildren<TileObj>();

        spritee.color = GetPresets.ColorBiome(tile.TileBiome, tile.type);
        spritee.sprite = SpriteManager.Instance.GetSprite(tile);
        tile.spritetile = spritee;

        if (GetPresets.TileOrder(tile))
        {
            spritee.transform.position = new Vector3(tile.x, tile.y, tile.z);
            spritee.transform.SetParent(this.transform, true);

            if (spritee.transform.position.z > 0)
            {
                spritee.transform.position = new Vector3(tile.x, tile.y, tile.z);
            }
            else if (tileGOmap[tile].transform.position.z < 0)
            {
                spritee.transform.position = new Vector3(tile.x, tile.y, tile.z);
            }

            spritee.GetComponent<SpriteRenderer>().sortingOrder = -(int)spritee.transform.position.z;
            spritee.sortingLayerName = "Player";
        }
        else
        {
            spritee.transform.position = new Vector3(tile.x, tile.y, tile.z);
            spritee.GetComponent<SpriteRenderer>().sortingOrder = 0;
            spritee.sortingLayerName = "Default";
        }

        #region SetUpPathGrid
        if (!Game.PathGrid.tiles.ContainsKey(new Vector2(tile.x, tile.z)))
        {
            if (tile.CanWalk)
            {
                if (tile.typego == TakeGO.empty && tile.placerObj == Placer.empty)
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
            if (tile.typego == TakeGO.empty && tile.placerObj == Placer.empty)
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

        /*if (tile.IsColider)
        {
            if (spritee.sprite.name != "Rock_")
            {
                if (boxcol != null)
                {
                    RefreshColider(tileGOmap[tile].GetComponent<PolygonCollider2D>(), spritee.sprite);
                }
                else
                {
                    tileGOmap[tile].AddComponent<PolygonCollider2D>().autoTiling = true;
                    RefreshColider(tileGOmap[tile].GetComponent<PolygonCollider2D>(), spritee.sprite);
                }
            }
        }
        else
        {
            if (boxcol != null)
            {
                Destroy(boxcol);
            }
        }*/
    }

    public void TileTransitionChange(Tile tile)
    {
        TileObj tilescript = tileGOmap[tile].GetComponentInChildren<TileObj>();

        ///SetUp-Transsition
        tilescript.Clear();
        SpriteManager.Instance.GetTranssition(tile, tilescript);
        tilescript.SetUp();
        if (tile.type == TypeBlock.Water)
        {
            tilescript.TILEANIMATED = true;
            if (tilescript.TILEANIMATOR == null)
            {
                /*tilescript.TILEANIMATOR = tilescript.gameObject.AddComponent<Animator>();
                tilescript.TILEANIMATOR.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/WaterAnimator");*/
            }
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

        Game.WorldGenerator.ChunksList.Remove(this);
    }

    private void SetUpPlacer(Tile tile)
    {
        if (tile.placerObj != Placer.empty)
        {
            GameObject trees = null;
            trees = Instantiate(SpriteManager.Instance.Getplacerbyname(tile.placerObj.ToString()), new Vector3(tile.x, tile.y, tile.z), Quaternion.identity);

            if (trees.transform.position.z > 0)
            {
                trees.transform.position = new Vector3(tile.x, tile.y, tile.z);
            }
            else if (trees.transform.position.z < 0)
            {
                trees.transform.position = new Vector3(tile.x, tile.y, tile.z);
            }

            trees.transform.SetParent(this.transform, true);

            tile.ObjThis = trees;

            if (trees.GetComponent<Trees>())
            {
                trees.GetComponent<Trees>().ThisTreeTile = tile;
            }

            if (trees.GetComponent<SpriteRenderer>())
            {
                trees.GetComponent<SpriteRenderer>().sortingOrder = -(int)trees.transform.position.z;
            }
            else if (trees.GetComponentInChildren<SpriteRenderer>())
            {
                trees.GetComponentInChildren<SpriteRenderer>().sortingOrder = -(int)trees.transform.position.z;
            }
        }
    }

    private void SetUpTileTree(Tile tile)
    {
        if (tile.typego != TakeGO.empty)
        {
            GameObject trees = null;
            trees = Instantiate(SpriteManager.Instance.GetPrefabbyname(tile.typego.ToString()), new Vector3(tile.x, tile.y, tile.z), Quaternion.identity);
            trees.transform.SetParent(this.transform, true);
            tile.ObjThis = trees;

            trees.transform.position = new Vector3(tile.x + Random.Range(0f, 1f), tile.y, tile.z + Random.Range(0f, 1f));

            if (trees.GetComponent<Trees>())
            {
                trees.GetComponent<Trees>().ThisTreeTile = tile;
            }

            if (trees.GetComponent<SpriteRenderer>())
            {
                trees.GetComponent<SpriteRenderer>().sortingOrder = -(int)trees.transform.position.z;
            }
            else if (trees.GetComponentInChildren<SpriteRenderer>())
            {
                trees.GetComponentInChildren<SpriteRenderer>().sortingOrder = -(int)trees.transform.position.z;
            }
        }
    }

    public void SaveChunk()
    {
        if (Game.GameManager.SinglePlayer)
        {
            SaveWorld.Save(tiles, Game.WorldGenerator.CurrentWorld.ToString() + (int)transform.position.x + "," + (int)transform.position.z);
        }
        else if (Game.GameManager.MultiPlayer)
        {

        }
    }

    public void RefreshColider(PolygonCollider2D polygonCollider, Sprite sprite)
    {
        List<Vector2> path = new List<Vector2>();
        polygonCollider.pathCount = sprite.GetPhysicsShapeCount();
        path.Clear();
        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            sprite.GetPhysicsShape(i, path);
            polygonCollider.SetPath(i, path.ToArray());
        }
    }

    void Update()
    {
#if Client
        if (Time.time > timetemp + TimeUpdate)
        {
            UpdateChunk();

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
        foreach (var ai in Entitys.ToArray())
        {
            if (ai != null)
            {
                ai.GetComponent<Vilanger>().GetNewPostion();
            }
        }

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

    //MultiPlayer Client
    public void ClientSetUpChunk(TileSave[] tile)
    {
#region TileGen
        tileGOmap = new Dictionary<Tile, GameObject>();
        tiles = new Tile[Size, Size];
        Game.WorldGenerator.ChunksList.Add(this);

        for (int v = 0; v < tile.Length; v++)
        {
            int i = tile[v].x - (int)transform.position.x;
            int j = tile[v].z - (int)transform.position.z;

            tiles[i, j] = new Tile(tile[v], new ChunkInfo((int)transform.position.x, (int)transform.position.z, this));

            tiles[i, j].RegisterOnTileTypeChange(OnTileTypeChange);

            GameObject TileGo = GameObject.Instantiate(TileObj, new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z), Quaternion.identity);

            TileGo.name = "Tile_" + tiles[i, j].x + "_" + tiles[i, j].z;

            TileGo.transform.position = new Vector3(tiles[i, j].x, tiles[i, j].y, tiles[i, j].z);
            TileGo.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
            TileGo.transform.SetParent(this.transform, true);

            tiles[i, j].TileObj = TileGo.GetComponent<TileObj>();

            tiles[i, j].TileChunk = new ChunkInfo((int)transform.position.x, (int)transform.position.z, this);

            //SetUp Tree object, spawn tree object and transalte for a new position
            SetUpTileTree(tiles[i, j]);
            SetUpPlacer(tiles[i, j]);

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
#endregion
    }
}