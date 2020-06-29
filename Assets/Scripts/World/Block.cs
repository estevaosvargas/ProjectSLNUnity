using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using System;

[System.Serializable]
public class Block
{
    public int x;
    public float hight;
    public int z;

    public TypeBlock Type;
    public BiomeType TileBiome;
    public TypeVariante typeVariante;
    public Placer PLACER_DATA;
    public TakeGO typego;

    public byte HP = 100;//this have some limitatios because is byte a byte only support a max of 255

    public byte Hora;
    public byte Dia = 1;
    public byte Mes = 1;

    [System.NonSerialized]
    public VoxelVector3 _blockChunk;

    public Block(int _x, int _z, VoxelVector3 chunkPosition, float _density)
    {
        x = _x;
        z = _z;

        hight = _density;
        _blockChunk = chunkPosition;

        if (hight <= 0 && hight >= -2f)
        {
            TileBiome = BiomeType.Bench;
            Type = Biome.GetDensity(x, z, 0, this);
        }
        else if (hight < -2f)
        {
            TileBiome = BiomeType.Bench;
            Type = TypeBlock.WaterFloor;
        }
        else
        {
            GenerateBiome();
            Type = Biome.GetDensity(x, z, 0, this);
        }
    }

    public void GenerateBiome()
    {
        HeatType HeatType;
        MoistureType MoistureType;

        float heatValue;
        float MoistureValue;

        float XX = x;
        float ZZ = z;

        lock (Game.World.GetheatNoise)
        {
            Game.World.GetheatNoise.GradientPerturbFractal(ref XX, ref ZZ);
        }

        heatValue = Mathf.Abs(Game.World.GetheatNoise.GetCellular(XX, ZZ));
        MoistureValue = Mathf.Abs(Game.World.GetheatNoise.GetCellular(XX, ZZ));

        if (heatValue < Get.ColdestValue)
        {
            HeatType = HeatType.Coldest;
        }
        else if (heatValue < Get.ColderValue)
        {
            HeatType = HeatType.Colder;
        }
        else if (heatValue < Get.ColdValue)
        {
            HeatType = HeatType.Cold;
        }
        else if (heatValue < Get.WarmValue)
        {
            HeatType = HeatType.Warm;
        }
        else if (heatValue < Get.WarmerValue)
        {
            HeatType = HeatType.Warmer;
        }
        else
        {
            HeatType = HeatType.Warmest;
        }
        ///
        if (MoistureValue < Get.DryerValue)
        {
            MoistureType = MoistureType.Dryer;
        }
        else if (MoistureValue < Get.DryValue)
        {
            MoistureType = MoistureType.Dry;
        }
        else if (MoistureValue < Get.WetValue)
        {
            MoistureType = MoistureType.Wet;
        }
        else if (MoistureValue < Get.WetterValue)
        {
            MoistureType = MoistureType.Wetter;
        }
        else if (MoistureValue < Get.WettestValue)
        {
            MoistureType = MoistureType.Wettest;
        }
        else
        {
            MoistureType = MoistureType.Wettest;
        }

        TileBiome = Get.BiomeTable[(int)MoistureType, (int)HeatType];
    }

    public void RemoveBlock()
    {
        SetBlock(TypeBlock.Air);
        RefreshBlock();
    }

    public void PlaceBlock()
    {
        //SetBlock(blockType);
        RefreshBlock();
    }

    public void DamageBloco(float damage)
    {

    }

    /// <summary>
    /// refresh all data in this block
    /// </summary>
    public void RefreshBlock()
    {
        Game.World.GetChunk(_blockChunk).UpdateChunksSurround();
    }

    public Block[] GetNeighboors(bool diagonals = false)
    {
        Block[] neighbors;

        if (diagonals)
        {
            neighbors = new Block[8];

            neighbors[0] = Game.World.GetTileAt(x, z + 1);//cima
            neighbors[1] = Game.World.GetTileAt(x + 1, z);//direita
            neighbors[2] = Game.World.GetTileAt(x, z - 1);//baixo
            neighbors[3] = Game.World.GetTileAt(x - 1, z);//esquerda

            neighbors[4] = Game.World.GetTileAt(x + 1, z - 1);//corn baixo direita
            neighbors[5] = Game.World.GetTileAt(x - 1, z + 1);//corn cima esquerda
            neighbors[6] = Game.World.GetTileAt(x + 1, z + 1);//corn cima direita
            neighbors[7] = Game.World.GetTileAt(x - 1, z - 1);//corn baixo esuqerda

        }
        else
        {
            neighbors = new Block[4];

            neighbors[0] = Game.World.GetTileAt(x, z - 1);//Atras
            neighbors[1] = Game.World.GetTileAt(x, z + 1);//Frente
            neighbors[4] = Game.World.GetTileAt(x - 1,  z);//esquerda
            neighbors[5] = Game.World.GetTileAt(x + 1,  z);//direita
        }

        return neighbors;
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
        string tostringReturn = "(Type: " + Type + "), (TileBiome: " + TileBiome
            + "), (typeVariante: " + typeVariante + "), (PLACER_DATA: " + PLACER_DATA + "), (TileBiome: " + typego + "), (Dendity: " + hight + ")";

        return tostringReturn;
    }
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