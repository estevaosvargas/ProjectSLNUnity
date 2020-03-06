using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Unity.Generator;

public enum BiomeType { ForestNormal = 0, Desert = 1, OceanNormal = 2, Jungle = 3, Plain = 4, Snow = 5, Bench = 6 , None, Montahas, Cave, ForestNormal_Dense }

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

    public static TypeBlock GetBiome(int x, int z, Tile tile, float sample, BiomeType biomeType)
    {
        float ForestNormalPerlin = Get_PerlinForestNormal(x, z);
        float ForestNormal_DensePerlin = Get_PerlinForestNormal_Dense(x, z);
        float DesertPerlin = Get_PerlinDesert(x, z);
        //float OceanNormalPerlin = ForestNormal(x, z);
        //float BenchPerlin = ForestNormal(x, z);
        float JunglePerlin = Get_PerlinJungle(x, z);
        float PlainePerlin = Get_PerlinPlaine(x, z);
        float ForestSnowPerlin = Get_PerlinForestSnow(x, z);
        float MontanhasPerlin = Get_PerlinMontanhas(x, z);

        //https://github.com/RedHenDev/Unity_scripts/blob/master/simpleVoxels_expanded_tuts/Assets/simpleVoxels/seeding/lerpTerrain.cs usa isso aqui como um exemplo

        //float noise + simplexnoise + simplexnoise_high;

        float last = 0;

        float finalheight = 0;
        float distance = 0;
        float divide = 0;

        //essse tile = 0
        //

        switch (biomeType)
        {
            case BiomeType.ForestNormal:
                tile.y = ForestNormalPerlin;
                tile.y += GetHeight(x, z, ForestNormalPerlin * 5, biomeType) + sample / 50;
                return ForestNormal(x, z, tile, sample, ForestNormalPerlin);
            case BiomeType.Desert:
                tile.y = DesertPerlin;
                tile.y += GetHeight(x, z, DesertPerlin * 5, biomeType) + sample / 50;
                return Desert(x, z, tile, sample, DesertPerlin);
            case BiomeType.Jungle:
                tile.y = JunglePerlin;
                tile.y += GetHeight(x, z, JunglePerlin * 5, biomeType) + sample / 50;
                return Jungle(x, z, tile, sample, JunglePerlin);
            case BiomeType.Plain:
                tile.y = PlainePerlin;
                tile.y += GetHeight(x, z, PlainePerlin * 5, biomeType) + sample / 50;
                return Plaine(x, z, tile, sample, PlainePerlin);
            case BiomeType.Snow:
                tile.y = ForestSnowPerlin;
                tile.y += GetHeight(x, z, ForestSnowPerlin * 5, biomeType) + sample / 50;
                return ForestSnow(x, z, tile, sample, ForestSnowPerlin);
            case BiomeType.Bench:
                tile.y = sample / 200;
                tile.y += GetHeight(x, z, sample / 200, biomeType) + 1;
                return Bench(x, z, tile, sample);
            case BiomeType.Montahas:
                tile.y = MontanhasPerlin;
                tile.y += GetHeight(x, z, MontanhasPerlin * 5, biomeType) + sample / 50;
                return Montanhas(x, z, tile, sample, MontanhasPerlin);
            case BiomeType.ForestNormal_Dense:
                tile.y = ForestNormal_DensePerlin;
                tile.y += GetHeight(x, z, ForestNormal_DensePerlin * 5, biomeType) + sample / 50;
                return ForestNormal_Dense(x, z, tile, sample, ForestNormal_DensePerlin);
            default:
                return TypeBlock.Air;
        }
    }

    public static Vector3 GetBiomeYPosition(int x, int z)
    {
        float ForestNormalPerlin = Get_PerlinForestNormal(x, z);
        float ForestNormal_DensePerlin = Get_PerlinForestNormal_Dense(x, z);
        float DesertPerlin = Get_PerlinDesert(x, z);
        //float OceanNormalPerlin = ForestNormal(x, z);
        //float BenchPerlin = ForestNormal(x, z);
        float JunglePerlin = Get_PerlinJungle(x, z);
        float PlainePerlin = Get_PerlinPlaine(x, z);
        float ForestSnowPerlin = Get_PerlinForestSnow(x, z);
        float MontanhasPerlin = Get_PerlinMontanhas(x, z);

        BiomeType biomeType = GetVizinhos(x,z);
        float sample = Get_PerlinGlobal(x, z);

        Vector3 final = new Vector3();

        switch (biomeType)
        {
            case BiomeType.ForestNormal:
                final.y = ForestNormalPerlin;
                final.y += GetHeight(x, z, ForestNormalPerlin * 5, biomeType) + sample / 50;
                break;
            case BiomeType.Desert:
                final.y = DesertPerlin;
                final.y += GetHeight(x, z, DesertPerlin * 5, biomeType) + sample / 50;
                break;
            case BiomeType.Jungle:
                final.y = JunglePerlin;
                final.y += GetHeight(x, z, JunglePerlin * 5, biomeType) + sample / 50;
                break;
            case BiomeType.Plain:
                final.y = PlainePerlin;
                final.y += GetHeight(x, z, PlainePerlin * 5, biomeType) + sample / 50;
                break;
            case BiomeType.Snow:
                final.y = ForestSnowPerlin;
                final.y += GetHeight(x, z, ForestSnowPerlin * 5, biomeType) + sample / 50;
                break;
            case BiomeType.Bench:
                final.y = sample / 200;
                final.y += GetHeight(x, z, sample / 200, biomeType) + 1;
                break;
            case BiomeType.Montahas:
                final.y = MontanhasPerlin;
                final.y += GetHeight(x, z, MontanhasPerlin * 5, biomeType) + sample / 50;
                break;
            case BiomeType.ForestNormal_Dense:
                final.y = ForestNormal_DensePerlin;
                final.y += GetHeight(x, z, ForestNormal_DensePerlin * 5, biomeType) + sample / 50;
                break;
        }

        return final;
    }

    public static float GetHeight(int x, int z, float height, BiomeType biomeType)
    {
        float high = height;

        if (biomeType != BiomeType.OceanNormal)
        {
            high = high + 0.5f;
        }

        /*for (int i = 1; i < 10; i++)
        {
            BiomeType biome = GetVizinhos(x + i, z);
            BiomeType biome1 = GetVizinhos(x - i, z);
            BiomeType biome2 = GetVizinhos(x, z + i);
            BiomeType biome3 = GetVizinhos(x, z - i);

            if (biome != biomeType)
            {
                float high2 = MakeTransition(x + i, z, biome);
                high = lerp(height, high2, 0.1f);
            }
            else if(biome1 != biomeType)
            {
                float high2 = MakeTransition(x - i, z, biome1);
                high = lerp(height, high2, 0.1f);
            }
            else if(biome2 != biomeType)
            {
                float high2 = MakeTransition(x, z + i, biome2);
                high = lerp(height, high2, 0.1f);
            }
            else if (biome3 != biomeType)
            {
                float high2 = MakeTransition(x, z - i, biome3);
                high = lerp(height, high2, 0.1f);
            }
        }*/

        /*float i = 0;
        for (int cx = -2; cx < 3; ++cx)
        {
            for (int cz = -2; cz < 3; ++cz)
            {
                BiomeType biome = GetVizinhos(x + cx, z + cz);

                if (biome != BiomeType.OceanNormal)
                {
                    if (biome != biomeType)
                    {
                        i++;
                        float high2 = MakeTransition(x + cx, z + cz, biome);
                        high += lerp(height, high2, 0.25f);
                        //high = high * 1.05f;
                        high /= 1.7f;
                    }
                }
            }
        }*/

        for (int cx = -3; cx < 4; ++cx) for (int cz = -3; cz < 4; ++cz)
                high += getBiomeHeiughtThingy(x + cx, z + cz);
        high /= 25;

        return high;
    }

    private static float getBiomeHeiughtThingy(int x, int z)
    {
        return MakeTransition(x, z, GetVizinhos(x, z));
    }

    static float lerp(float point1, float point2, float alpha)
    {
        return point1 + alpha * (point2 - point1);
    }

    public static float MakeTransition(int x, int z, BiomeType tilebiome)
    {
        switch (tilebiome)
        {
            case BiomeType.ForestNormal:
                return Get_PerlinForestNormal(x, z) * 5;
            case BiomeType.Desert:
                return Get_PerlinDesert(x, z) * 5;
            case BiomeType.Jungle:
                return Get_PerlinJungle(x, z) * 5;
            case BiomeType.Plain:
                return Get_PerlinPlaine(x, z) * 5;
            case BiomeType.Snow:
                return Get_PerlinForestSnow(x, z) * 5;
            case BiomeType.Montahas:
                return Get_PerlinMontanhas(x, z) * 5;
            case BiomeType.ForestNormal_Dense:
                return Get_PerlinForestNormal_Dense(x, z) * 5;
            case BiomeType.Bench:
                return Get_PerlinGlobal(x, z);
            default:
                return Get_PerlinGlobal(x, z);
        }
    }

    static BiomeType GetVizinhos(int x, int z)
    {
        float sample = (float)new LibNoise.Unity.Generator.Perlin(0.31f, 0.6f, 2.15f, 10, Game.GameManager.Seed, LibNoise.Unity.QualityMode.Low).GetValue(x, z, 0);

        if (sample >= 50f)
        {
            float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.01f, 1, Game.GameManager.Seed, false).GetValue(x, z, 0);
            sample2 *= 10;

            if ((int)sample2 == 0)
            {
                return BiomeType.ForestNormal;
            }
            else if ((int)sample2 == 1)
            {
                //Jungle
                return BiomeType.Jungle;
            }
            else if ((int)sample2 == 2)
            {
                //ForestNormal
                return BiomeType.ForestNormal;
            }
            else if ((int)sample2 == 3)
            {
                //Montahas
                return BiomeType.Montahas;
            }
            else if ((int)sample2 == 4)
            {
                //Plain
                return BiomeType.Plain;
            }
            else if ((int)sample2 == 5)
            {
                //Snow
                return BiomeType.Snow;
            }
            else if ((int)sample2 == 6)
            {
                //Jungle
                return BiomeType.Jungle;
            }
            else if ((int)sample2 == 7)
            {
                //Desert
                return BiomeType.Desert;
            }
            else if ((int)sample2 == -4)
            {
                //ForestNormal_Dense
                return BiomeType.ForestNormal_Dense;
            }
            else if ((int)sample2 == 8)
            {
                //ForestNormal_Dense
                return BiomeType.ForestNormal_Dense;
            }
            else if ((int)sample2 == -8)
            {
                //ForestNormal_Dense
                return BiomeType.ForestNormal_Dense;
            }
            else if ((int)sample2 == -2)
            {
                //ForestNormal_Dense
                return BiomeType.ForestNormal_Dense;
            }
            else
            {
                return BiomeType.ForestNormal_Dense;
            }
        }
        else if (sample > 0.08f)
        {
            return BiomeType.Bench;
        }
        else
        {
            return BiomeType.Bench;
        }
    }

    private static float Get_PerlinForestNormal(int x, int z)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
    }

    private static float Get_PerlinForestNormal_Dense(int x, int z)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
    }

    private static float Get_PerlinDesert(int x, int z)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale + Game.GameManager.Small_Seed;
        float zCorde = (float)z / height * Scale + Game.GameManager.Small_Seed;

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

        return Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
    }

    private static float Get_PerlinPlaine(int x, int z)
    {
        Scale = 10f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
    }

    private static float Get_PerlinForestSnow(int x, int z)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        return Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
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

        float xCordee = (float)octaves * x / width * Scale + Game.GameManager.Small_Seed;
        float zCordee = (float)octaves * z / height * Scale + Game.GameManager.Small_Seed;

        // modify with frequency
        xCordee *= frequency;
        zCordee *= frequency;

        return Mathf.PerlinNoise(xCordee, zCordee) * amplitude / persistence;
    }

    private static float Get_PerlinGlobal(int x, int z)
    {
        return (float)new LibNoise.Unity.Generator.Perlin(0.31f, 0.6f, 2.15f, 10, Game.GameManager.Seed, LibNoise.Unity.QualityMode.Low).GetValue(x, z, 0) / 200;
    }


    private static TypeBlock ForestNormal(int x, int z, Tile tile, float sample, float perlin)
    {
        LibNoise.Unity.Generator.Voronoi CityNoise = new LibNoise.Unity.Generator.Voronoi(0.009f, 1, Game.GameManager.Seed, false);
        VeronoiStruc sample2 = CityNoise.GetValueNPoint(x, z, 0);

        tile.CityPoint = new DataVector3(CityNoise.GetPoint(x, z, 0));

        VeronoiStruc Tile_Vila_Up = CityNoise.GetValueNPoint(x, z + 1, 0);
        VeronoiStruc Tile_Vila_Down = CityNoise.GetValueNPoint(x, z - 1, 0);
        VeronoiStruc Tile_Vila_Left = CityNoise.GetValueNPoint(x - 1, z, 0);
        VeronoiStruc Tile_Vila_Right = CityNoise.GetValueNPoint(x + 1, z, 0);

        sample2.Value *= 10;

        //float noise + simplexnoise + simplexnoise_high;

        /*Tile_Vila_Up.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x, z + 1);
        Tile_Vila_Down.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x, z - 1);
        Tile_Vila_Left.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x - 1, z);
        Tile_Vila_Right.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x + 1, z);*/


        if ((int)Tile_Vila_Up.Value == 1 || (int)Tile_Vila_Down.Value == 1 || (int)Tile_Vila_Left.Value == 1 || (int)Tile_Vila_Right.Value == 1)
        {
            if (tile.typego == TakeGO.empty && tile.z != 0)
            {
                tile.typego = TakeGO.RockWall;
            }
            return TypeBlock.Rock;
        }

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
                System.Random rand = new System.Random(Game.GameManager.Seed + x * z + (tile.chunkX + tile.chunkZ));
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

    private static TypeBlock ForestNormal_Dense(int x, int z, Tile tile, float sample, float perlin)
    {
        LibNoise.Unity.Generator.Voronoi CityNoise = new LibNoise.Unity.Generator.Voronoi(0.009f, 1, Game.GameManager.Seed, false);
        VeronoiStruc sample2 = CityNoise.GetValueNPoint(x, z, 0);

        tile.CityPoint = new DataVector3(CityNoise.GetPoint(x, z, 0));

        VeronoiStruc Tile_Vila_Up = CityNoise.GetValueNPoint(x, z + 1, 0);
        VeronoiStruc Tile_Vila_Down = CityNoise.GetValueNPoint(x, z - 1, 0);
        VeronoiStruc Tile_Vila_Left = CityNoise.GetValueNPoint(x - 1, z, 0);
        VeronoiStruc Tile_Vila_Right = CityNoise.GetValueNPoint(x + 1, z, 0);

        tile.CityPoint = new DataVector3(CityNoise.GetPoint(x, z, 0));


        /*Tile_Vila_Up.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x, z + 1);
        Tile_Vila_Down.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x, z - 1);
        Tile_Vila_Left.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x - 1, z);
        Tile_Vila_Right.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x + 1, z);*/

        if ((int)Tile_Vila_Up.Value == 1 || (int)Tile_Vila_Down.Value == 1 || (int)Tile_Vila_Left.Value == 1 || (int)Tile_Vila_Right.Value == 1)
        {
            if (tile.typego == TakeGO.empty && tile.z != 0)
            {
                tile.typego = TakeGO.RockWall;
            }
            return TypeBlock.Rock;
        }

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
                System.Random rand = new System.Random(Game.GameManager.Seed + x * z + (tile.chunkX + tile.chunkZ));
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

    private static TypeBlock Desert(int x, int z, Tile tile, float sample, float perlin)
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
                //grass and bushs and trees
                System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.chunkX + tile.chunkZ));
                int randnum = (rand.Next(1, 20));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.Cactu;
                    }
                    return TypeBlock.Sand;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.Cactu2;
                    }
                    return TypeBlock.Sand;
                }

                return TypeBlock.Sand;

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

    private static TypeBlock Bench(int x, int z, Tile tile, float sample)
    {
        if (sample > 0.08f)
        {
            System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.chunkX + tile.chunkZ));
            int randnum = (rand.Next(1, 100));

            if (randnum == 1)
            {
                if (tile.typego == TakeGO.empty)
                {
                    tile.typego = TakeGO.PalmTree;
                }
                return TypeBlock.BeachSand;
            }
            else if (randnum == 5)
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

    private static TypeBlock Jungle(int x, int z, Tile tile, float sample, float perlin)
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
                System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.chunkX + tile.chunkZ));
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

    private static TypeBlock Plaine(int x, int z, Tile tile, float sample, float perlin)
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

    private static TypeBlock ForestSnow(int x, int z, Tile tile, float sample, float perlin)
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
                System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.chunkX + tile.chunkZ));
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
                    return TypeBlock.Grass;
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

    private static TypeBlock Montanhas(int x, int z, Tile tile, float sample, float perlin)
    {

        if (perlin >= 0.0f && perlin <= 0.15f)
        {
            //Water
            return TypeBlock.Air;
        }
        else if (perlin > 0.15f && perlin < 0.2f)
        {
            //Sand Bench
            return TypeBlock.Air;
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
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.Weed01;
                    }
                    return TypeBlock.Air;
                }
                else if (randnum == 2)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.RockProp;
                    }
                    return TypeBlock.Air;
                }
                else if (randnum == 3)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.WeedTall;
                    }
                    return TypeBlock.Air;
                }
                else if (randnum == 4)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.Pine;
                    }
                    return TypeBlock.Air;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.Oak;
                    }
                    return TypeBlock.Air;
                }
                else if (randnum == 6)
                {
                    tile.typeVariante = TypeVariante.GrassFL1;
                    return TypeBlock.Air;
                }
                else if (randnum == 7)
                {
                    tile.typeVariante = TypeVariante.GrassFL2;
                    return TypeBlock.Air;
                }
                else if (randnum == 8)
                {
                    tile.typeVariante = TypeVariante.GrassRC;
                    return TypeBlock.Air;
                }
                else
                {
                    return TypeBlock.Air;
                }
            }
            else if (perlin > 0.6f && perlin < 0.605f)
            {
                //grass
                return TypeBlock.Air;
            }
            else if (perlin > 0.62f && perlin < 0.63f)
            {
                //grass
                return TypeBlock.Air;
            }
            else
            {
                //tall grass
                 return TypeBlock.Air;
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