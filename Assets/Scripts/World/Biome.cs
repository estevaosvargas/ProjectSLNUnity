using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using LibNoise.Unity.Generator;

public struct BiomeOnly
{
    public Vector3 position;//this is position of the x,y,z of the biome, x,z is gona be the same of your input position
    public TypeBlock tileType;//the biome tile type, can be use to filtre what you want
    public BiomeType biomeType;//biome type and the same up

    public BiomeOnly(Vector3 _position, TypeBlock _tileType, BiomeType _biomeType)
    {
        position = _position;
        tileType = _tileType;
        biomeType = _biomeType;
    }
}

public static class Biome
{
    public static int width = 50;
    public static int height = 50;

    public static float Scale = 50f;
    static float noisefactor = 0.725f;

    public static float Seed = 100;

    public static float persistence = -0.04f;
    public static float frequency = 0.5f;
    public static float amplitude = 79.12f;
    public static int octaves = 26;

    public static BiomeType[] Biomeslayers = new BiomeType[10] { BiomeType.Desert,
    BiomeType.Savanna,
    BiomeType.TropicalRainforest,
    BiomeType.Grassland,
    BiomeType.Woodland,
    BiomeType.SeasonalForest,
    BiomeType.TemperateRainforest,
    BiomeType.BorealForest,
    BiomeType.Tundra,
    BiomeType.Ice};

    public static TypeBlock GetDensity(int x, int z, float sample, Block tile)
    {
        FastNoise globalNoise = new FastNoise();

        globalNoise.SetFrequency(0.002f);


        tile.h = globalNoise.GetPerlinFractal(x, z) + sample / 50;


        switch (tile.TileBiome)
        {
            case BiomeType.Grassland:
                return ForestNormal(x, z, tile, sample, Get_PerlinForestNormal(x, z));
            case BiomeType.Desert:
                return Desert(x, z, tile, sample, Get_PerlinDesert(x, z));
            case BiomeType.TropicalRainforest:
                return Jungle(x, z, tile, sample, Get_PerlinJungle(x, z));
            case BiomeType.Savanna:
                return Plaine(x, z, tile, sample, Get_PerlinPlaine(x, z));
            case BiomeType.Ice:
                return ForestSnow(x, z, tile, sample, Get_PerlinForestSnow(x, z));
            case BiomeType.Tundra:
                return Montanhas(x, z, tile, sample, Get_PerlinMontanhas(x, z));
            case BiomeType.Woodland:
                return ForestNormal_Dense(x, z, tile, sample, Get_PerlinForestNormal_Dense(x, z));
            case BiomeType.Bench:
                tile.h = sample / 50;
               return Bench(x, z, tile, tile.h);
            default:
                return TypeBlock.IronStone;
        }
    }

    /// <summary>
    /// Get Position on surface
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 GetBiomeYPosition(int x, int y, int z)
    {
        Vector3 final = new Vector3();

        /*FastNoise globalNoise = new FastNoise();
        FastNoise seialNoise = new FastNoise();

        seialNoise.SetFrequency(0.009f);

        globalNoise.SetFrequency(0.001f);

        int n = Biomeslayers.Length;
        float l = globalNoise.GetPerlin(x, z);
        float YY = 0;

        BiomeType biomeType = BiomeType.None;

        for (int i = 0; i < Biomeslayers.Length; i++)
        {
            if ((i - 1f) / n <= l && l <= (i + 1f) / n)
            {
                switch (Biomeslayers[i])
                {
                    case BiomeType.ForestNormal:
                        YY += (-Mathf.Abs(n * l - i) + 1) * seialNoise.GetPerlinFractal(x,z) * 4;
                        biomeType = Biomeslayers[i];
                        break;
                    case BiomeType.Desert:
                        YY += (-Mathf.Abs(n * l - i) + 1) * seialNoise.GetPerlinFractal(x, z) * 2;
                        biomeType = Biomeslayers[i];
                        break;
                    case BiomeType.Jungle:
                        YY += (-Mathf.Abs(n * l - i) + 1) * seialNoise.GetPerlinFractal(x, z) * 2;
                        biomeType = Biomeslayers[i];
                        break;
                    case BiomeType.Plain:
                        YY += (-Mathf.Abs(n * l - i) + 1) * seialNoise.GetPerlinFractal(x, z) * 2;
                        biomeType = Biomeslayers[i];
                        break;
                    case BiomeType.Snow:
                        YY += (-Mathf.Abs(n * l - i) + 1) * seialNoise.GetPerlinFractal(x, z) * 2;
                        biomeType = Biomeslayers[i];
                        break;
                    case BiomeType.Montahas:
                        YY += (-Mathf.Abs(n * l - i) + 1) * seialNoise.GetPerlinFractal(x, z) * 8;
                        biomeType = Biomeslayers[i];
                        break;
                    case BiomeType.ForestNormal_Dense:
                        YY += (-Mathf.Abs(n * l - i) + 1) * seialNoise.GetPerlinFractal(x, z) * 5;
                        biomeType = Biomeslayers[i];
                        break;
                }
            }

            if (biomeType == BiomeType.None)
            {
                YY += -(-Mathf.Abs(n * l - i) + 1) * l * 30;
                biomeType = BiomeType.Bench;
            }
        }

        final = new Vector3(x, YY, z);
        */
        return final;
    }

    private static float Get_PerlinForestNormal(int x, int z)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + GameManager.Seed, zCorde * noisefactor + GameManager.Seed);
    }

    private static float Get_PerlinForestNormal_Dense(int x, int z)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + GameManager.Seed, zCorde * noisefactor + GameManager.Seed);
    }

    private static float Get_PerlinDesert(int x, int z)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale + GameManager.Seed;
        float zCorde = (float)z / height * Scale + GameManager.Seed;

        return Mathf.PerlinNoise(xCorde * noisefactor, zCorde * noisefactor);
    }

    private static float Get_PerlinJungle(int x, int z)
    {
        Scale = 5f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + GameManager.Seed, zCorde * noisefactor + GameManager.Seed);
    }

    private static float Get_PerlinPlaine(int x, int z)
    {
        Scale = 10f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + GameManager.Seed, zCorde * noisefactor + GameManager.Seed);
    }

    private static float Get_PerlinForestSnow(int x, int z)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + GameManager.Seed, zCorde * noisefactor + GameManager.Seed);
    }

    private static float Get_PerlinMontanhas(int x, int z)
    {
        Scale = 20f;
        height = 50;
        width = 50;

        noisefactor = 0.01f;

        float persistence = 39.9f;
        float frequency = 0.001f;
        float amplitude = 52.79f;
        int octaves = 184;

        float xCordee = (float)octaves * x / width * Scale + GameManager.Seed;
        float zCordee = (float)octaves * z / height * Scale + GameManager.Seed;

        // modify with frequency
        xCordee *= frequency;
        zCordee *= frequency;

        return Mathf.PerlinNoise(xCordee, zCordee) * amplitude / persistence;
    }

    private static float Get_PerlinGlobal(int x, int z)
    {
        return (float)new LibNoise.Unity.Generator.Perlin(0.31f, 0.6f, 2.15f, 10, GameManager.Seed, LibNoise.Unity.QualityMode.Low).GetValue(x, z, 0) / 200;
    }


    private static TypeBlock ForestNormal(int x, int z, Block tile, float sample, float perlin)
    {
        /*LibNoise.Unity.Generator.Voronoi CityNoise = new LibNoise.Unity.Generator.Voronoi(0.009f, 1, GameManager.Seed, false);
        VeronoiStruc sample2 = CityNoise.GetValueNPoint(x, z, 0);

        tile.CityPoint = new DataVector3(CityNoise.GetPoint(x, z, 0));*/

        if (perlin <= 0.15f)
        {
            //Water
            return TypeBlock.WaterFloor;
        }
        else if (perlin > 0.15f && perlin < 0.2f)
        {
            //Sand Bench
            return TypeBlock.BeachSand;
        }
        else if (perlin > 0.2f && perlin <= 0.7f)
        {
            if (perlin > 0.2f && perlin < 0.6f)
            {
                //grass and bushs and trees
                System.Random rand = new System.Random((int)Chunk.GetChunkSeed(tile.CurrentChunk) + x * z);
                int randnum = (rand.Next(1, 50));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.Weed01;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 2)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.RockProp;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 3)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.WeedTall;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 4)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.Pine;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.Oak;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 6)
                {
                    tile.typeVariante = TypeVariante.GrassFL1;
                    return TypeBlock.Grass;
                }
                else if (randnum == 7)
                {
                    tile.typeVariante = TypeVariante.GrassFL2;
                    return TypeBlock.Grass;
                }
                else if (randnum == 8)
                {
                    tile.typeVariante = TypeVariante.GrassRC;
                    return TypeBlock.Grass;
                }
                else if (randnum == 9)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.Pine_Tall;
                    }
                    return TypeBlock.Grass;
                }
                else
                {
                    return TypeBlock.Grass;
                }
            }
            else if (perlin > 0.6f && perlin < 0.605f)
            {
                //grass
                return TypeBlock.Grass;
            }
            else if (perlin > 0.62f && perlin < 0.63f)
            {
                //grass
                return TypeBlock.Grass;
            }
            else
            {
                //tall grass
                return TypeBlock.Grass;
            }

        }
        else if (perlin > 0.7f && perlin <= 0.8f)
        {
            if (perlin > 0.7f && perlin < 0.72f)
            {
                //grama do pe do morro com arvores
                if (tile.typego == TakeGO.empty && tile.z != 0)
                {
                    tile.typego = TakeGO.RockWall;
                }
                return TypeBlock.Rock;
            }
            else if (perlin > 0.72f && perlin < 0.74f)
            {
                //grama do pe do morro
                if (tile.typego == TakeGO.empty && tile.z != 0)
                {
                    tile.typego = TakeGO.RockWall;
                }
                return TypeBlock.Rock;
            }
            else
            {
                //pe do morro
                if (tile.typego == TakeGO.empty && tile.z != 0)
                {
                    tile.typego = TakeGO.RockWall;
                }
                return TypeBlock.Rock;
            }

        }
        else
        {
            //topo do morro
            if (tile.typego == TakeGO.empty && tile.z != 0)
            {
                tile.typego = TakeGO.RockWall;
            }
            return TypeBlock.Rock;
        }

    }

    private static TypeBlock ForestNormal_Dense(int x, int z, Block tile, float sample, float perlin)
    {
        /*LibNoise.Unity.Generator.Voronoi CityNoise = new LibNoise.Unity.Generator.Voronoi(0.009f, 1, GameManager.Seed, false);
        VeronoiStruc sample2 = CityNoise.GetValueNPoint(x, z, 0);

        tile.CityPoint = new DataVector3(CityNoise.GetPoint(x, z, 0));*/

        if (perlin <= 0.15f)
        {
            //Water
            return TypeBlock.WaterFloor;
        }
        else if (perlin > 0.15f && perlin < 0.2f)
        {
            //Sand Bench
            return TypeBlock.BeachSand;
        }
        else if (perlin > 0.2f && perlin <= 0.7f)
        {
            if (perlin > 0.2f && perlin < 0.6f)
            {
                //grass and bushs and trees
                System.Random rand = new System.Random((int)Chunk.GetChunkSeed(tile.CurrentChunk) + x * z);
                int randnum = (rand.Next(1, 20));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.Weed01;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 2)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.RockProp;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 3)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.WeedTall;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 4)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.Pine;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.Oak;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 6)
                {
                    tile.typeVariante = TypeVariante.GrassFL1;
                    return TypeBlock.Grass;
                }
                else if (randnum == 7)
                {
                    tile.typeVariante = TypeVariante.GrassFL2;
                    return TypeBlock.Grass;
                }
                else if (randnum == 8)
                {
                    tile.typeVariante = TypeVariante.GrassRC;
                    return TypeBlock.Grass;
                }
                else
                {
                    return TypeBlock.Grass;
                }
            }
            else if (perlin > 0.6f && perlin < 0.605f)
            {
                //grass
                return TypeBlock.Grass;
            }
            else if (perlin > 0.62f && perlin < 0.63f)
            {
                //grass
                return TypeBlock.Grass;
            }
            else
            {
                //tall grass
                return TypeBlock.Grass;
            }

        }
        else if (perlin > 0.7f && perlin <= 0.8f)
        {
            if (perlin > 0.7f && perlin < 0.72f)
            {
                //grama do pe do morro com arvores
                if (tile.typego == TakeGO.empty && tile.z != 0)
                {
                    tile.typego = TakeGO.RockWall;
                }
                return TypeBlock.Rock;
            }
            else if (perlin > 0.72f && perlin < 0.74f)
            {
                //grama do pe do morro
                if (tile.typego == TakeGO.empty && tile.z != 0)
                {
                    tile.typego = TakeGO.RockWall;
                }
                return TypeBlock.Rock;
            }
            else
            {
                //pe do morro
                if (tile.typego == TakeGO.empty && tile.z != 0)
                {
                    tile.typego = TakeGO.RockWall;
                }
                return TypeBlock.Rock;
            }

        }
        else
        {
            //topo do morro
            if (tile.typego == TakeGO.empty && tile.z != 0)
            {
                tile.typego = TakeGO.RockWall;
            }
            return TypeBlock.Rock;
        }

    }

    private static TypeBlock Desert(int x, int z, Block tile, float sample, float perlin)
    {

        if (perlin >= 0.0f && perlin <= 0.15f)
        {
            //Water
            return TypeBlock.Water;
        }
        else if (perlin > 0.15f && perlin < 0.2f)
        {
            //Sand Bench
            return TypeBlock.BeachSand;
        }
        else if (perlin > 0.2f && perlin <= 0.7f)
        {
            if (perlin > 0.2f && perlin < 0.6f)
            {
                System.Random rand = new System.Random((int)Chunk.GetChunkSeed(tile.CurrentChunk) + x * z);
                int randnum = (rand.Next(1, 50));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.Cactu;
                    }
                    return TypeBlock.Sand;
                }
                else if (randnum == 2)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.Cactu2;
                    }
                    return TypeBlock.Sand;
                }
                else
                {
                    return TypeBlock.Sand;
                }
            }
            else if (perlin > 0.6f && perlin < 0.605f)
            {
                //grass
                return TypeBlock.Sand;
            }
            else if (perlin > 0.62f && perlin < 0.63f)
            {
                //grass
                return TypeBlock.Sand;
            }
            else
            {
                //tall grass
                return TypeBlock.Sand;
            }

        }
        else if (perlin > 0.7f && perlin <= 0.8f)
        {
            if (perlin > 0.7f && perlin < 0.72f)
            {
                //grama do pe do morro com arvores
                return TypeBlock.Rock;
            }
            else if (perlin > 0.72f && perlin < 0.74f)
            {
                //grama do pe do morro
                return TypeBlock.Rock;
            }
            else
            {
                //pe do morro
                return TypeBlock.Rock;
            }

        }
        else
        {
            //topo do morro
            return TypeBlock.Rock;
        }
    }

    private static TypeBlock Bench(int x, int z, Block tile, float sample)
    {
        if (sample > -0.3f)
        {
            System.Random rand = new System.Random((int)Chunk.GetChunkSeed(tile.CurrentChunk) + x * z);
            int randnum = (rand.Next(1, 50));

            if (randnum == 1)
            {
                if (tile.typego == TakeGO.empty)
                {
                    tile.typego = TakeGO.PalmTree;
                }
                return TypeBlock.BeachSand;
            }
            else if (randnum == 2)
            {
                if (tile.typego == TakeGO.empty)
                {
                    tile.typego = TakeGO.PalmTree2;
                }
                return TypeBlock.BeachSand;
            }
            else
            {
                return TypeBlock.BeachSand;
            }
        }
        else
        {
            return TypeBlock.WaterFloor;
        }
    }

    private static TypeBlock Jungle(int x, int z, Block tile, float sample, float perlin)
    {
        //tile.y *= perlin;

        if (perlin >= 0.0f && perlin <= 0.15f)
        {
            //Water
            return TypeBlock.WaterFloor;
        }
        else if (perlin > 0.15f && perlin < 0.2f)
        {
            //Sand Bench
            return TypeBlock.BeachSand;
        }
        else if (perlin > 0.2f && perlin <= 0.7f)
        {
            if (perlin > 0.2f && perlin < 0.6f)
            {
                //grass and bushs and trees
                System.Random rand = new System.Random((int)Chunk.GetChunkSeed(tile.CurrentChunk) + x * z);
                int randnum = (rand.Next(1, 20));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.BigTree;
                    }
                    return TypeBlock.JungleGrass;
                }
                else if (randnum == 3)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.BigTree2;
                    }
                    return TypeBlock.JungleGrass;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.WeedTall_Jungle;
                    }
                    return TypeBlock.JungleGrass;
                }
            }
            else if (perlin > 0.6f && perlin < 0.605f)
            {
                //grass
                return TypeBlock.JungleGrass;
            }
            else if (perlin > 0.62f && perlin < 0.63f)
            {
                //grass
                return TypeBlock.JungleGrass;
            }
            else
            {
                //tall grass
                return TypeBlock.JungleGrass;
            }

        }
        else if (perlin > 0.7f && perlin <= 0.8f)
        {
            if (perlin > 0.7f && perlin < 0.72f)
            {
                //grama do pe do morro com arvores
                return TypeBlock.Rock;
            }
            else if (perlin > 0.72f && perlin < 0.74f)
            {
                //grama do pe do morro
                return TypeBlock.Rock;
            }
            else
            {
                //pe do morro
                return TypeBlock.Rock;
            }

        }
        else
        {
            //topo do morro
            return TypeBlock.Rock;
        }
        return TypeBlock.JungleGrass;
    }

    private static TypeBlock Plaine(int x, int z, Block tile, float sample, float perlin)
    {
        /*float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.009f, 2, Game.GameManager.Seed, false).GetValue(x, z, 0);

        if ((int)sample2 == 1)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            tile.OwnedByCity = true;

            if (color == Game.Color("FF0000") || color == Game.Color("7F7F7F"))//Somthing Is On this tile
            {
                return TypeBlock.Dirt;
            }
            else if (color == Game.Color("FF0048"))//House Spawn Origin
            {
                tile.PLACER_DATA = Placer.VillagerHouse;
                return TypeBlock.Dirt;
            }
            else if (color == Game.Color("2D92FF"))//Spawn Chest(TesteOnly)
            {
                tile.PLACER_DATA = Placer.BauWood;
                return TypeBlock.Dirt;
            }
            else if (color == Game.Color("303030"))//Spawn BlackSmith
            {
                tile.PLACER_DATA = Placer.BlackSmith;
                return TypeBlock.Dirt;
            }
            else if (color == Game.Color("FF6A00"))//Spawn BlackSmith
            {
                tile.PLACER_DATA = Placer.AlchemistHouse;
                return TypeBlock.Dirt;
            }
            else if (color == Game.Color("FFFFFF"))//Road
            {
                return TypeBlock.DirtRoad;
            }
            else
            {
                #region Normal
                if (perlin >= 0.0f && perlin <= 0.15f)
                {
                    //Water
                    return TypeBlock.Water;
                }
                else if (perlin > 0.15f && perlin < 0.2f)
                {
                    //Sand Bench
                    return TypeBlock.BeachSand;
                }
                else if (perlin > 0.2f && perlin <= 0.7f)
                {
                    if (perlin > 0.2f && perlin < 0.6f)
                    {
                        //grass and bushs and trees

                        System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.chunkX + tile.chunkZ));
                        int randnum = (rand.Next(1, 20));

                        if (randnum == 1)
                        {
                            tile.typeVariante = TypeVariante.GrassFL1;
                            return TypeBlock.Grass;
                        }
                        else if (randnum == 3)
                        {
                            tile.typeVariante = TypeVariante.GrassFL2;
                            return TypeBlock.Grass;
                        }
                        else if (randnum == 5)
                        {
                            tile.typeVariante = TypeVariante.GrassRC;
                            return TypeBlock.Grass;
                        }
                        else
                        {
                            return TypeBlock.Grass;
                        }
                    }
                    else if (perlin > 0.6f && perlin < 0.605f)
                    {
                        //grass
                        return TypeBlock.Grass;
                    }
                    else if (perlin > 0.62f && perlin < 0.63f)
                    {
                        //grass
                        return TypeBlock.Grass;
                    }
                    else
                    {
                        //tall grass
                        return TypeBlock.Grass;
                    }

                }
                else if (perlin > 0.7f && perlin <= 0.8f)
                {
                    if (perlin > 0.7f && perlin < 0.72f)
                    {
                        //grama do pe do morro com arvores
                        return TypeBlock.Grass;
                    }
                    else if (perlin > 0.72f && perlin < 0.74f)
                    {
                        //grama do pe do morro
                        return TypeBlock.Grass;
                    }
                    else
                    {
                        //pe do morro
                        return TypeBlock.Grass;
                    }

                }
                else if (perlin > 1f)
                {
                    //topo do morro
                    return TypeBlock.Grass;
                }
                else
                {
                    return TypeBlock.Grass;
                }
                #endregion
            }
        }
        else
        {
          
        }*/
        #region Normal
        if (perlin >= 0.0f && perlin <= 0.15f)
        {
            //Water
            return TypeBlock.WaterFloor;
        }
        else if (perlin > 0.15f && perlin < 0.2f)
        {
            //Sand Bench
            return TypeBlock.BeachSand;
        }
        else if (perlin > 0.2f && perlin <= 0.7f)
        {
            if (perlin > 0.2f && perlin < 0.6f)
            {
                //grass and bushs and trees

                System.Random rand = new System.Random((int)Chunk.GetChunkSeed(tile.CurrentChunk) + x * z);
                int randnum = (rand.Next(1, 20));

                if (randnum == 1)
                {
                    tile.typeVariante = TypeVariante.GrassFL1;
                    return TypeBlock.Grass;
                }
                else if (randnum == 3)
                {
                    tile.typeVariante = TypeVariante.GrassFL2;
                    return TypeBlock.Grass;
                }
                else if (randnum == 5)
                {
                    tile.typeVariante = TypeVariante.GrassRC;
                    return TypeBlock.Grass;
                }
                else
                {
                    return TypeBlock.Grass;
                }
            }
            else if (perlin > 0.6f && perlin < 0.605f)
            {
                //grass
                return TypeBlock.Grass;
            }
            else if (perlin > 0.62f && perlin < 0.63f)
            {
                //grass
                return TypeBlock.Grass;
            }
            else
            {
                //tall grass
                return TypeBlock.Grass;
            }

        }
        else if (perlin > 0.7f && perlin <= 0.8f)
        {
            if (perlin > 0.7f && perlin < 0.72f)
            {
                //grama do pe do morro com arvores
                return TypeBlock.Grass;
            }
            else if (perlin > 0.72f && perlin < 0.74f)
            {
                //grama do pe do morro
                return TypeBlock.Grass;
            }
            else
            {
                //pe do morro
                return TypeBlock.Grass;
            }

        }
        else if (perlin > 1f)
        {
            //topo do morro
            return TypeBlock.Grass;
        }
        else
        {
            return TypeBlock.Grass;
        }
        #endregion
    }

    private static TypeBlock ForestSnow(int x, int z, Block tile, float sample, float perlin)
    {
        //tile.y *= perlin;

        if (perlin >= 0.0f && perlin <= 0.15f)
        {
            //Water
            return TypeBlock.IceWater;
        }
        else if (perlin > 0.15f && perlin < 0.2f)
        {
            //Sand Bench
            return TypeBlock.BeachSand;
        }
        else if (perlin > 0.2f && perlin <= 0.7f)
        {
            if (perlin > 0.2f && perlin < 0.6f)
            {
                //grass and bushs and trees
                System.Random rand = new System.Random((int)Chunk.GetChunkSeed(tile.CurrentChunk) + x * z);
                int randnum = (rand.Next(1, 20));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.WeedTall_Snow;
                    }
                    return TypeBlock.Snow;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.PineSnow;
                    }
                    return TypeBlock.Snow;
                }
                else if (randnum == 9)
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.PineSnow_Tall;
                    }
                    return TypeBlock.Snow;
                }
                else
                {
                    return TypeBlock.Snow;
                }
            }
            else if (perlin > 0.6f && perlin < 0.605f)
            {
                //grass
                return TypeBlock.Snow;
            }
            else if (perlin > 0.62f && perlin < 0.63f)
            {
                //grass
                return TypeBlock.Snow;
            }
            else
            {
                //tall grass
                return TypeBlock.Snow;
            }

        }
        else if (perlin > 0.7f && perlin <= 0.8f)
        {
            if (perlin > 0.7f && perlin < 0.72f)
            {
                //grama do pe do morro com arvores
                return TypeBlock.Rock;
            }
            else if (perlin > 0.72f && perlin < 0.74f)
            {
                //grama do pe do morro
                return TypeBlock.Rock;
            }
            else
            {
                //pe do morro
                return TypeBlock.Rock;
            }

        }
        else
        {
            //topo do morro
            return TypeBlock.Rock;
        }

    }

    private static TypeBlock Montanhas(int x, int z, Block tile, float sample, float perlin)
    {

        if (perlin >= 0.0f && perlin <= 0.15f)
        {
            //Water
            return TypeBlock.Grass;
        }
        else if (perlin > 0.15f && perlin < 0.2f)
        {
            //Sand Bench
            return TypeBlock.Grass;
        }
        else if (perlin > 0.2f && perlin <= 0.7f)
        {
            if (perlin > 0.2f && perlin < 0.6f)
            {
                //grass and bushs and trees
                System.Random rand = new System.Random((int)Chunk.GetChunkSeed(tile.CurrentChunk) + x * z);
                int randnum = (rand.Next(1, 20));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.Weed01;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 2)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.RockProp;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 3)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.WeedTall;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 4)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.Pine;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.Oak;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 6)
                {
                    tile.typeVariante = TypeVariante.GrassFL1;
                    return TypeBlock.Grass;
                }
                else if (randnum == 7)
                {
                    tile.typeVariante = TypeVariante.GrassFL2;
                    return TypeBlock.Grass;
                }
                else if (randnum == 8)
                {
                    tile.typeVariante = TypeVariante.GrassRC;
                    return TypeBlock.Grass;
                }
                else
                {
                    return TypeBlock.Grass;
                }
            }
            else if (perlin > 0.6f && perlin < 0.605f)
            {
                //grass
                return TypeBlock.Grass;
            }
            else if (perlin > 0.62f && perlin < 0.63f)
            {
                //grass
                return TypeBlock.Grass;
            }
            else
            {
                //tall grass
                return TypeBlock.Grass;
            }

        }
        else if (perlin > 0.7f && perlin <= 0.8f)
        {
            if (perlin > 0.7f && perlin < 0.72f)
            {
                //grama do pe do morro com arvores
                return TypeBlock.Rock;
            }
            else if (perlin > 0.72f && perlin < 0.74f)
            {
                //grama do pe do morro
                return TypeBlock.Rock;
            }
            else
            {
                //pe do morro
                return TypeBlock.Rock;
            }

        }
        else
        {
            //topo do morro
            return TypeBlock.Rock;
        }

    }
}
public static class NoiseData
{
    public static FastNoise MainNoise;
    public static FastNoise BiomeNoise;
    public static FastNoise BiomeNoiseTran;
    public static FastNoise ForestNoise;

    public static void StartData()
    {
        MainNoise = new FastNoise(GameManager.Seed);
        BiomeNoise = new FastNoise(GameManager.Seed);
        ForestNoise = new FastNoise(GameManager.Seed);
        BiomeNoiseTran = new FastNoise(GameManager.Seed);

        ForestNoise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);

        ForestNoise.SetFractalOctaves(2);
        ForestNoise.SetFractalLacunarity(3.0f);
        ForestNoise.SetFractalGain(0.2f);

        ForestNoise.SetFrequency(0.01f);
        ForestNoise.SetInterp(FastNoise.Interp.Quintic);

        MainNoise.SetFrequency(0.01f);
        MainNoise.SetInterp(FastNoise.Interp.Quintic);


        ///Biome Noise
        BiomeNoise.SetNoiseType(FastNoise.NoiseType.Cellular);

        BiomeNoise.SetFrequency(0.001f);
        BiomeNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
        BiomeNoise.SetCellularReturnType(FastNoise.CellularReturnType.CellValue);

        ///Biome Tran
        BiomeNoiseTran.SetNoiseType(FastNoise.NoiseType.Cellular);

        BiomeNoiseTran.SetFrequency(0.01f);
        BiomeNoiseTran.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
        BiomeNoiseTran.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Sub);
    }
}