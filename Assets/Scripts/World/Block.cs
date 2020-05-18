using UnityEngine;
using System.Collections;

[System.Serializable]
public class Block
{
    public int x;
    public float h;//height
    public int z;

    public TypeBlock Type;
    public BiomeType TileBiome;
    public TypeVariante typeVariante;
    public Placer PLACER_DATA;
    public TakeGO typego;

    public int HP = 100;

    public int Hora;
    public int Dia = 1;
    public int Mes = 1;

    public HeatType HeatType;
    public MoistureType MoistureType;

    public float heatValue;
    public float MoistureValue;

    ///Configs Values
    [System.NonSerialized]
    float ColdestValue = 0.05f;
    [System.NonSerialized]
    float ColderValue = 0.18f;
    [System.NonSerialized]
    float ColdValue = 0.4f;
    [System.NonSerialized]
    float WarmValue = 0.6f;
    [System.NonSerialized]
    float WarmerValue = 0.8f;
    [System.NonSerialized]
    float DryerValue = 0.27f;
    [System.NonSerialized]
    float DryValue = 0.4f;
    [System.NonSerialized]
    float WetValue = 0.6f;
    [System.NonSerialized]
    float WetterValue = 0.8f;
    [System.NonSerialized]
    float WettestValue = 0.9f;
    ///

    [System.NonSerialized]
    public bool OcupedByOther = false;
    [System.NonSerialized]
    public DataVector3 CityPoint;
    /// <summary>
    /// If AI Can Walk.
    /// </summary>
    [System.NonSerialized]
    public bool CanWalk = false;
    [System.NonSerialized]
    public bool IsServerTile = false;
    [System.NonSerialized]
    public int MaxHP = 100;
    [System.NonSerialized]
    public Vector3 CurrentChunk;
    [System.NonSerialized]
    public GameObject BlockObject;

    BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
};

    public Block(int _x, int _z, Vector3 _chunkPosition)
    {
        x = _x;
        z = _z;

        CurrentChunk = _chunkPosition;

        FastNoise heatNoise = new FastNoise(GameManager.Seed);
        FastNoise moistureNoise = new FastNoise(GameManager.Seed);

        float XX = x;
        float ZZ = z;

        heatNoise.SetFrequency(0.05f);

        heatNoise.SetGradientPerturbAmp(30f);
        heatNoise.GradientPerturbFractal(ref XX, ref ZZ);
        heatNoise.SetCellularNoiseLookup(new FastNoise());
        heatNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Manhattan);
        heatNoise.SetCellularReturnType(FastNoise.CellularReturnType.NoiseLookup);

        heatValue = Mathf.Abs(heatNoise.GetCellular(XX, ZZ));
        
        MoistureValue = Mathf.Abs(heatNoise.GetCellular(XX, ZZ));

        if (heatValue < ColdestValue)
        {
            HeatType = HeatType.Coldest;
        }
        else if (heatValue < ColderValue)
        {
            HeatType = HeatType.Colder;
        }
        else if (heatValue < ColdValue)
        {
            HeatType = HeatType.Cold;
        }
        else if (heatValue < WarmValue)
        {
            HeatType = HeatType.Warm;
        }
        else if (heatValue < WarmerValue)
        {
            HeatType = HeatType.Warmer;
        }
        else
        {
            HeatType = HeatType.Warmest;
        }
        ///
        if (MoistureValue < DryerValue)
        {
            MoistureType = MoistureType.Dryer;
        }
        else if (MoistureValue < DryValue)
        {
            MoistureType = MoistureType.Dry;
        }
        else if (MoistureValue < WetValue)
        {
            MoistureType = MoistureType.Wet;
        }
        else if (MoistureValue < WetterValue)
        {
            MoistureType = MoistureType.Wetter;
        }
        else if (MoistureValue < WettestValue)
        {
            MoistureType = MoistureType.Wettest;
        }
        else
        {
            MoistureType = MoistureType.Wettest;
        }

        TileBiome = GetBiomeType(this);
        float sample = (float)new LibNoise.Unity.Generator.Perlin(0.31f, 0.6f, 2.15f, 10, GameManager.Seed, LibNoise.Unity.QualityMode.Low).GetValue(x, z, 0);

        if (sample >= 50f)
        {
            Type = Biome.GetDensity(x, z, sample, this);
        }
        else
        {
            TileBiome = BiomeType.Bench;
            Type = Biome.GetDensity(x, z, sample, this);
        }
    }

    public BiomeType GetBiomeType(Block tile)
    {
        return BiomeTable[(int)tile.MoistureType, (int)tile.HeatType];
    }

    public void RemoveBlock()
    {
        SetBlock(TypeBlock.Air);
        Game.World.GetChunk(CurrentChunk).UpdateMeshChunk();
    }

    public void PlaceBlock(TypeBlock blockType)
    {
        SetBlock(blockType);
        Game.World.GetChunk(CurrentChunk).UpdateMeshChunk();
    }

    public void DamageBloco(float damage)
    {

    }

    public Block[] GetNeighboors(bool diagonals = false)
    {
        Block[] neighbors;

        if (diagonals)
        {
            neighbors = new Block[8];

            neighbors[0] = Game.World.GetTileAt(x, z + 1);//cima
            neighbors[1] = Game.World.GetTileAt(x + 1, z);//direita
            neighbors[2] = Game.World.GetTileAt(x,  z - 1);//baixo
            neighbors[3] = Game.World.GetTileAt(x - 1, z);//esquerda

            neighbors[4] = Game.World.GetTileAt(x + 1, z - 1);//corn baixo direita
            neighbors[5] = Game.World.GetTileAt(x - 1, z + 1);//corn cima esquerda
            neighbors[6] = Game.World.GetTileAt(x + 1, z + 1);//corn cima direita
            neighbors[7] = Game.World.GetTileAt(x - 1, z - 1);//corn baixo esuqerda

        }
        else
        {
            neighbors = new Block[4];

            neighbors[0] = Game.World.GetTileAt(x, z + 1);//cima
            neighbors[1] = Game.World.GetTileAt(x + 1, z);//direita
            neighbors[2] = Game.World.GetTileAt(x, z - 1);//baixo
            neighbors[3] = Game.World.GetTileAt(x - 1, z);//esquerda
        }

        return neighbors;
    }

    public void RefreshTile()
    {

    }

    public void SaveChunk()
    {

    }

    public void SetBlock(TypeBlock blockType)
    {
        Type = blockType;
    }

    public override string ToString()
    {
        string tostringReturn = "(Type: " + Type + "), (heatValue: " + heatValue + "), (MoistureType: " + MoistureType + "), (TileBiome: " + TileBiome
            + "), (typeVariante: " + typeVariante + "), (PLACER_DATA: " + PLACER_DATA + "), (TileBiome: " + typego + "), (HP: " + HP + "), DATA(H:"+Hora+", Day: "+ Dia + "Mes: " + Mes+ ")";

        return tostringReturn;
    }
}

public struct VoxelBlock
{
    public int x, y, z;
    public TypeBlock type;
}

public enum HeatType
{
    Coldest,
    Colder,
    Cold,
    Warm,
    Warmer,
    Warmest
}

public enum MoistureType
{
    Wettest,
    Wetter,
    Wet,
    Dry,
    Dryer,
    Dryest
}