using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile
{
    public TypeBlock type { get; private set; }
    public TypeVariante typeVariante;
    public Placer placerObj { get; set; }
    public TakeGO typego { get; set; }

    public bool ConnecyToNightboors = false;
    public bool IsColider = false;

    public int x { get; private set; }
    public int y { get; private set; }
    public int z { get; private set; }

    public int RenderLevel = 0;

    public BiomeType TileBiome;

    public bool Emessive = false;

    public object InsideTile;

    public int Hora;
    public int Dia = 1;
    public int Mes = 1;

    [NonSerialized]
    public bool CanWalk = false;

    public int HP = 100;
    [NonSerialized]
    public int MaxHP = 100;
    [NonSerialized]
    public int LightOnTile = 0;

    [NonSerialized]
    public Action<Tile> OnTileTypeChange;

    [NonSerialized]
    public GameObject ObjThis;

    [NonSerialized]
    public SpriteRenderer spritetile;

    [NonSerialized]
    public ChunkInfo TileChunk;

    [NonSerialized]
    public TileObj TileObj;

    public void DataTimeSave(int h, int d, int m)
    {
        Hora = DataTime.Hora + h;
        Dia = DataTime.Dia + d;
        Mes = DataTime.Mes + m;
    }

    public void DataTimeSave(int h)
    {
        if (DataTime.Hora + h > 24)
        {
            Hora = 0;
            Hora += h;

            Dia = DataTime.Dia + 1;
            Mes = DataTime.Mes;
        }
        else
        {
            Hora = DataTime.Hora + h;
            Dia = DataTime.Dia;
            Mes = DataTime.Mes;
        }
    }

    public void Reset()
    {
        Hora = -1;
        Dia = -1;
        Mes = -1;
    }

    public Tile()
    {

    }

    public Tile(TileSave tile)
    {
        x = tile.x;
        y = tile.y;
        type = tile.type;
        typego = tile.typego;
        placerObj = tile.placer;
        IsColider = tile.IsColider;
        Emessive = tile.Emessive;
        ConnecyToNightboors = tile.ConnecyToNightboors;
        TileBiome = tile.Biomeofthis;
    }

    public Tile(int x, int y, TypeBlock Type)
    {
        this.x = x;
        this.y = y;
        type = Type;
    }

    public Tile(int x, int y, int z, BiomeType biotype)
    {
        this.x = x;
        this.y = y;
        this.z = z;

        float persistence = 39.9f;
        float frequency = 0.001f;
        float amplitude = 52.79f;
        int octaves = 184;

        int width = 50;
        int height = 50;

        float Scale = 1f;

        float xCordee = (float)octaves * x / width * Scale;
        float yCordee = (float)octaves * y / height * Scale;

        xCordee *= frequency;
        yCordee *= frequency;

        float sample = Mathf.PerlinNoise(xCordee + WorldGenerator.Instance.Seed, yCordee + WorldGenerator.Instance.Seed) * amplitude / persistence;

        if (WorldGenerator.Instance.CurrentWorld == WorldType.Caves)
        {
            Debug.LogError("Sorry Caves Is Not enable in alpha!");
        }
        else if (WorldGenerator.Instance.CurrentWorld == WorldType.Normal)
        {
            // normal 0.8f
            if (sample > 0.5f)
            {
                //float xbiome = (float)1 * x / 50 * 0.5f;
                //float ybiome = (float)1 * y / 50 * 0.5f;

                //xbiome *= 0.01f;
                //ybiome *= 0.01f;

                //float sample2 = Mathf.PerlinNoise(xbiome + WorldGenerator.Instance.Seed, ybiome + WorldGenerator.Instance.Seed) * sample / 0.11f;

                float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.005f, 1, WorldGenerator.Instance.Seed, false).GetValue(x, y, 0);

                sample2 *= 10;

                PerlinSetType(SetUpBiome(x, y, this, sample, sample2));
            }
            else if (sample > 0.3f)
            {
                TileBiome = BiomeType.Bench;
                PerlinSetType(Biome.Bench(x, y, this, sample));
            }
            else
            {
                TileBiome = BiomeType.OceanNormal;
                PerlinSetType(Biome.OceanNormal(x, y, this, sample));
            }
        }
    }

    public void PerlinSetType(TypeBlock type)
    {
        if (typego == TakeGO.empty)
        {
            if (type == TypeBlock.Grass)
            {
                typego = TakeGO.Grass;
            }
        }

        this.type = type;
        SetUpTile(this);
    }

    public void DamageBloco(int damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            switch (type)
            {
                case TypeBlock.Grass:
                    DamageTypeSet(TypeBlock.DirtGrass);
                    break;
                case TypeBlock.Water:
                    DamageTypeSet(TypeBlock.Water);
                    break;
                case TypeBlock.IceWater:
                    DamageTypeSet(TypeBlock.Water);
                    break;
                case TypeBlock.Rock:
                    DamageTypeSet(TypeBlock.RockGround);
                    break;
                case TypeBlock.IronStone:
                    DamageTypeSet(TypeBlock.RockGround);
                    break;
                case TypeBlock.GoldStone:
                    DamageTypeSet(TypeBlock.RockGround);
                    break;
                case TypeBlock.RockGround:
                    DamageTypeSet(getcavetile(x, y));
                    break;
                case TypeBlock.DirtGrass:
                    DamageTypeSet(TypeBlock.Dirt);
                    break;
                case TypeBlock.Dirt:
                    DamageTypeSet(getcavetile(x, y));
                    break;
                default:
                    DamageTypeSet(TypeBlock.Air);
                    break;
            }
        }
    }

    TypeBlock getcavetile(int x, int y)
    {
        Color color = SpriteManager.Instance.GetPerlinImage("Caves01").GetPixel(x + WorldGenerator.Instance.Seed, y + WorldGenerator.Instance.Seed);

        if (WorldGenerator.Instance.CurrentWorld == WorldType.Normal)
        {
            if (color.r == 1 && color.g == 0 && color.b == 0)
            {
                return TypeBlock.RockHoleDown;
            }
            else if (color.r == 1 && color.g == 1 && color.b == 1)
            {
                return TypeBlock.RockHoleDown;
            }
            else
            {
                return TypeBlock.RockGround;
            }
        }
        else if (WorldGenerator.Instance.CurrentWorld == WorldType.Caves)
        {
            if (color.r == 1 && color.g == 0 && color.b == 0)
            {
                return TypeBlock.RockHoleUp;
            }
            else if (color.r == 1 && color.g == 1 && color.b == 1)
            {
                return TypeBlock.RockHoleUp;
            }
            else
            {
                return TypeBlock.RockGround;
            }
        }
        else
        {
            if (color.r == 1 && color.g == 0 && color.b == 0)
            {
                return TypeBlock.RockHole;
            }
            else if (color.r == 1 && color.g == 1 && color.b == 1)
            {
                return TypeBlock.RockHole;
            }
            else
            {
                return TypeBlock.RockGround;
            }
        }

    }

    //Place a gameobject on tile, and don't remove any thing on tile, object like chest,torch, or other things
    public void SetPlacer(Placer type)
    {
        if (placerObj != Placer.empty || typego != TakeGO.empty)// if have some object placed on this tile is dont place another with this line
        {
            return;
        }

        placerObj = type;

        GameObject trees = GameObject.Instantiate(SpriteManager.Instance.Getplacerbyname(placerObj.ToString()), new Vector3(x, y, 0), Quaternion.identity);

        if (trees == null) { return; }//if gameobject are null, code get error and come back

        if (TileChunk.ThisChunk.transform.position.y > 0)
        {
            trees.transform.position = new Vector3(x, y, -0.05f);
        }
        else if (TileChunk.ThisChunk.transform.position.y < 0)
        {
            trees.transform.position = new Vector3(x, y, 0.05f);
        }

        trees.transform.SetParent(TileChunk.ThisChunk.transform, true);

        ObjThis = trees;

        if (trees.GetComponent<Trees>())
        {
            trees.GetComponent<Trees>().ThisTreeTile = this;
        }

        if (trees.GetComponent<SpriteRenderer>())
        {
            trees.GetComponent<SpriteRenderer>().sortingOrder = -(int)trees.transform.position.y;
        }
        else if (trees.GetComponentInChildren<SpriteRenderer>())
        {
            trees.GetComponentInChildren<SpriteRenderer>().sortingOrder = -(int)trees.transform.position.y;
        }

        SaveChunk();
    }

    //For Change the tile to any thing do you want, like change to other, or destroy the tile
    public void DamageTypeSet(TypeBlock type)
    {
        if (typego != TakeGO.empty || placerObj != Placer.empty)//only to remove or change this tile, need are empty, dont have any object placed on this tile 
        {
            return;
        }

        this.type = type;

        Reset();

        SetUpTile(this);
        DamageSetUp(this);
        SaveChunk();

        OnTileTypeChange(this);
        TileChunk.ThisChunk.TileTransitionChange(this);


        Tile[] neighbor = GetNeighboors(true);

        for (int i = 0; i < neighbor.Length; i++)
        {
            neighbor[i].TileChunk.ThisChunk.TileTransitionChange(neighbor[i]);
            neighbor[i].OnTileTypeChange(neighbor[i]);
        }
    }

    public void PlaceBlockSet(TypeBlock type)
    {
        if (typego != TakeGO.empty || placerObj != Placer.empty)//only to remove or change this tile, need are empty, dont have any object placed on this tile 
        {
            return;
        }

        this.type = type;
        Reset();
        SetUpTile(this);
        SaveChunk();

        OnTileTypeChange(this);
        TileChunk.ThisChunk.TileTransitionChange(this);

        Tile[] neighbor = GetNeighboors(true);

        for (int i = 0; i < neighbor.Length; i++)
        {
            neighbor[i].TileChunk.ThisChunk.TileTransitionChange(neighbor[i]);
            neighbor[i].OnTileTypeChange(neighbor[i]);
        }
    }

    //Save all chunk
    public void SaveChunk()
    {
        TileChunk.ThisChunk.SaveChunk();
    }

    //to set/add light to tile
    public void SetLight(Color LightColor, int lightontile, LightToD lighttype)
    {
        LightOnTile = lightontile;

        spritetile.color = LightColor;
    }

    public void RegisterOnTileTypeChange(Action<Tile> callback)
    {
        OnTileTypeChange += callback;
    }

    //To get all neighboors, laterals and diagonals
    public Tile[] GetNeighboors(bool diagonals = false)
    {
        Tile[] neighbors;

        if (diagonals)
        {
            neighbors = new Tile[8];

            neighbors[0] = WorldGenerator.Instance.GetTileAt(x, y + 1);//cima
            neighbors[1] = WorldGenerator.Instance.GetTileAt(x + 1, y);//direita
            neighbors[2] = WorldGenerator.Instance.GetTileAt(x, y - 1);//baixo
            neighbors[3] = WorldGenerator.Instance.GetTileAt(x - 1, y);//esquerda

            neighbors[4] = WorldGenerator.Instance.GetTileAt(x + 1, y - 1);//corn baixo direita
            neighbors[5] = WorldGenerator.Instance.GetTileAt(x - 1, y + 1);//corn cima esquerda
            neighbors[6] = WorldGenerator.Instance.GetTileAt(x + 1, y + 1);//corn cima direita
            neighbors[7] = WorldGenerator.Instance.GetTileAt(x - 1, y - 1);//corn baixo esuqerda

        }
        else
        {
            neighbors = new Tile[4];

            neighbors[0] = WorldGenerator.Instance.GetTileAt(x, y + 1);//cima
            neighbors[1] = WorldGenerator.Instance.GetTileAt(x + 1, y);//direita
            neighbors[2] = WorldGenerator.Instance.GetTileAt(x, y - 1);//baixo
            neighbors[3] = WorldGenerator.Instance.GetTileAt(x - 1, y);//esquerda
        }

        return neighbors;
    }

    //Valus to determine whear what biome is on the positions
    public TypeBlock SetUpBiome(int x, int y, Tile tile, float sample, float sample2)
    {
        if ((int)sample2 == 0)
        {
            //sem nemhum
            TileBiome = BiomeType.ForestNormal;
            return Biome.ForestNormal(x, y, this, sample);
        }
        else if ((int)sample2 == 1)
        {
            //Jungle
            TileBiome = BiomeType.Jungle;
            return Biome.Jungle(x, y, this, sample);
        }
        else if ((int)sample2 == 2)
        {
            //Oceano Normal
            TileBiome = BiomeType.ForestNormal;
            return Biome.ForestNormal(x, y, this, sample);
        }
        else if ((int)sample2 == 3)
        {
            //Deserto
            TileBiome = BiomeType.Montahas;
            return Biome.Montanhas(x, y, this, sample);
        }
        else if ((int)sample2 == 4)
        {
            //sem nemhum
            TileBiome = BiomeType.Plain;
            return Biome.Plaine(x, y, this, sample);
        }
        else if ((int)sample2 == 5)
        {
            //sem nemhum
            TileBiome = BiomeType.Snow;
            return Biome.ForestSnow(x, y, this, sample);
        }
        else if ((int)sample2 == 6)
        {
            //sem nemhum
            TileBiome = BiomeType.Montahas;
            return Biome.Montanhas(x, y, this, sample);
        }
        else if ((int)sample2 == 7)
        {
            //sem nemhum
            TileBiome = BiomeType.Desert;
            return Biome.Desert(x, y, this, sample);
        }
        else if ((int)sample2 == -4)
        {
            //sem nemhum
            TileBiome = BiomeType.Plain;
            return Biome.Plaine(x, y, this, sample);
        }
        else
        {
            TileBiome = BiomeType.ForestNormal;
            return Biome.ForestNormal(x, y, this, sample);
        }
    }

    private void DamageSetUp(Tile tile)
    {
        switch (tile.type)
        {
            case TypeBlock.DirtGrass:
                tile.DataTimeSave(8);
                Debug.Log(Hora + " : " + Dia);
                break;
            default:
                break;
        }
    }

    public void SetUpTile(Tile tile)
    {
        switch (tile.type)
        {
            case TypeBlock.Sand:
                tile.ConnecyToNightboors = false;
                tile.IsColider = false;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 0;
                tile.CanWalk = true;
                break;
            case TypeBlock.Dirt:
                tile.ConnecyToNightboors = false;
                tile.IsColider = false;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 1;
                tile.CanWalk = true;
                break;
            case TypeBlock.Water:
                tile.ConnecyToNightboors = false;
                tile.IsColider = false;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 2;
                tile.CanWalk = true;
                break;
            case TypeBlock.Bloco:
                tile.ConnecyToNightboors = true;
                tile.IsColider = true;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = -1;
                tile.CanWalk = false;
                break;
            case TypeBlock.Grass:
                tile.ConnecyToNightboors = false;
                tile.IsColider = false;
                tile.HP = 35;
                tile.MaxHP = 35;
                tile.RenderLevel = 3;
                tile.CanWalk = true;
                break;
            case TypeBlock.Rock:
                tile.ConnecyToNightboors = true;
                tile.IsColider = true;
                tile.HP = 200;
                tile.MaxHP = 200;
                tile.RenderLevel = 4;
                tile.CanWalk = false;
                break;
            case TypeBlock.RockGround:
                tile.ConnecyToNightboors = false;
                tile.IsColider = false;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 0;
                tile.CanWalk = true;
                break;
            case TypeBlock.RockHoleDown:
                tile.ConnecyToNightboors = false;
                tile.IsColider = true;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 0;
                tile.CanWalk = false;
                break;
            case TypeBlock.RockHoleUp:
                tile.ConnecyToNightboors = false;
                tile.IsColider = true;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 0;
                tile.CanWalk = false;
                break;
            case TypeBlock.DirtGrass:
                tile.ConnecyToNightboors = false;
                tile.IsColider = false;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 0;
                tile.CanWalk = true;
                break;
            case TypeBlock.BeachSand:
                tile.ConnecyToNightboors = true;
                tile.IsColider = false;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 0;
                tile.CanWalk = true;
                break;
            default:
                tile.ConnecyToNightboors = false;
                tile.IsColider = false;
                tile.HP = 100;
                tile.MaxHP = 100;
                tile.RenderLevel = 0;
                tile.CanWalk = false;
                break;
        }
    }
    public void RefreshTile()
    {
        SetUpTile(this);

        Tile[] neighbor = GetNeighboors();

        OnTileTypeChange(this);
        TileChunk.ThisChunk.TileTransitionChange(this);

        for (int i = 0; i < neighbor.Length; i++)
        {
            if (neighbor[i] != null)
            {
                neighbor[i].TileChunk.ThisChunk.TileTransitionChange(neighbor[i]);
                neighbor[i].OnTileTypeChange(neighbor[i]);
            }
        }
    }
}

[System.Serializable]
public class TileSave
{
    public TypeBlock type;
    public Placer placer;
    public TakeGO typego;
    public bool ConnecyToNightboors = false;
    public bool IsColider = false;
    public int x;
    public int y;
    public BiomeType Biomeofthis;
    public bool HaveHosue = false;
    public bool Emessive = false;
}