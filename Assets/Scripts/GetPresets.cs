using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetPresets
{
    public static bool GetMouseIteract(Tile t)
    {
        if (t.type == TypeBlock.LightBlockON)
        {
            return true;
        }
        else if (t.placerObj == Placer.BauWood || t.placerObj == Placer.BauGold || t.placerObj == Placer.BauDiamond || t.placerObj == Placer.BauDark)
        {
            return true;
        }
        else if (t.type == TypeBlock.RockHoleUp)
        {
            return true;
        }
        else if (t.type == TypeBlock.RockHoleDown)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool OpenInveTile(Tile t)
    {
        switch (t.placerObj)
        {
            case Placer.BauDark:
                return true;
            case Placer.BauDiamond:
                return true;
            case Placer.BauGold:
                return true;
            case Placer.BauWood:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Used Yo Verify if this block ar surround by tile can have a transsition
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HaveBlend(TypeBlock type)
    {
        bool value = false;

        switch (type)
        {
            case TypeBlock.Grass:
                value = true;
                break;
            case TypeBlock.BeachSand:
                value = true;
                break;
            case TypeBlock.Rock:
                value = true;
                break;
            default:
                value = false;
                break;
        }

        return value;
    }

    /// <summary>
    /// if this tile can calculate, trassitions
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool TileCanDoBlend(TypeBlock type)
    {
        bool value = false;

        switch (type)
        {
            case TypeBlock.Rock:
                value = false;
                break;
            default:
                value = true;
                break;
        }

        return value;
    }

    public static int GetTileRenIndex(TypeBlock tile)
    {
        switch (tile)
        {
            case TypeBlock.Water:
                return 1;
            case TypeBlock.Sand:
                return 2;
            case TypeBlock.BeachSand:
                return 2;
            case TypeBlock.Dirt:
                return 3;
            case TypeBlock.Grass:
                return 4;
            case TypeBlock.Rock:
                return 10;
            default:
                return 0;
        }
    }


    /// <summary>
    /// Check if the Neighboors tile, can be tarsition by the main tile
    /// </summary>
    /// <returns></returns>
    public static bool CanTransitionTo(TypeBlock MainTile, TypeBlock Neighboor)
    {
        switch (MainTile)
        {
            case TypeBlock.Grass:
                return true;
            case TypeBlock.BeachSand:
                switch (Neighboor)
                {
                    case TypeBlock.Water:
                        return true;
                    case TypeBlock.Sand:
                        return true;
                    default:
                        return false;
                }
            default:
                return true;
        }
    }

    public static Color ColorBiome(BiomeType tilebiome, TypeBlock tile)
    {
        switch (tilebiome)
        {
            case BiomeType.Jungle:
                if (tile == TypeBlock.Grass)
                {
                    return new Color(0.3128338f, 0.6981132f, 0.3446094f, 1);
                }
                else
                {
                    return Color.white;
                }
            default:
                break;
        }
        return Color.white;
    }

    public static bool TileOrder(Tile tile)
    {
        switch (tile.type)
        {
            case TypeBlock.Rock:
                return true;
            default:
                return false;
        }
    }
}

public enum TypeBlock : byte
{
    Air, RockGround, RockHole,
    Grass, Water, GoldStone, IronStone,
    Rock, DirtGrass, Sand,
    Bloco, Dirt, RockHoleUp, RockHoleDown,
    DirtRoad, Lava, IceWater, Snow, LightBlockON,
    BeachSand
}

public enum TypeVariante : byte
{
    none, GrassFL1, GrassFL2, GrassRC
}

public enum TakeGO : byte
{
    empty,
    Pine, Oak,
    Bush, BigTree, BigTree2,
    Cactu, Cactu2, PalmTree,
    PalmTree2, PineSnow, Weed01, WeedTall, WeedTall_Jungle, WeedTall_Snow, RockProp, Grass
}

public enum Placer : byte
{
    empty, BauWood, BauGold, BauDiamond, BauDark, CampTend, CampFire, MainBuild1, MainBuild2
}