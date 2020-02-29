using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Unity.Generator;

public enum BiomeType { ForestNormal = 0, Desert = 1, OceanNormal = 2, Jungle = 3, Plain = 4, Snow = 5, Bench = 6 , None, Montahas, Cave, ForestNormal_Dense }

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
                tile.y += GetHeight(x, z, ForestNormalPerlin * 5, biomeType)+ sample / 50;
                return ForestNormal(x, z, tile, sample, ForestNormalPerlin);
            case BiomeType.Desert:
                tile.y = DesertPerlin;
                tile.y += GetHeight(x, z, DesertPerlin * 5, biomeType) + sample / 50;
                return Desert(x, z, tile, sample, DesertPerlin);
            case BiomeType.OceanNormal:
                return OceanNormal(x, z, tile, sample);
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

    public static float GetHeight(int x, int z, float height, BiomeType biomeType)
    {
        float high = height;

        /*for (int i = 1; i < 5; i++)
        {
            if (GetVizinhos(x + i, z) != biomeType)
            {
                float high2 = MakeTransition(x + i, z);
                high = lerp(high, high2, 0.125f);
            }
            else if(GetVizinhos(x - i, z) != biomeType)
            {
                float high2 = MakeTransition(x - i, z);
                high = lerp(high, high2, 0.125f);
            }
            else if(GetVizinhos(x, z + i) != biomeType)
            {
                float high2 = MakeTransition(x, z + i);
                high = lerp(high, high2, 0.125f);
            }
            else if (GetVizinhos(x, z - i) != biomeType)
            {
                float high2 = MakeTransition(x, z - i);
                high = lerp(high, high2, 0.125f);
            }
        }*/

        float i = 0;

        for (int cx = -4; cx < 5; ++cx)
        {
            for (int cz = -4; cz < 5; ++cz)
            {
                if (GetVizinhos(x + cx, z + cz) != biomeType)
                {
                    float high2 = MakeTransition(x + cx, z + cz);
                    high = Mathf.Lerp(high, high2, 0.125f);
                    //high *= 1.16f;
                }
                i++;
            }
        }


        return high;
    }

    static float lerp(float point1, float point2, float alpha)
    {
        return point1 + alpha * (point2 - point1);
    }

    public static float MakeTransition(int x, int z)
    {
        switch (GetVizinhos(x, z))
        {
            case BiomeType.ForestNormal:
                return Get_PerlinForestNormal(x, z);
            case BiomeType.Desert:
                return Get_PerlinDesert(x, z);
            case BiomeType.OceanNormal:
                return 0;
            case BiomeType.Jungle:
                return Get_PerlinJungle(x, z);
            case BiomeType.Plain:
                return Get_PerlinPlaine(x, z);
            case BiomeType.Snow:
                return Get_PerlinForestSnow(x, z);
            case BiomeType.Bench:
                return 0;
            case BiomeType.Montahas:
                return Get_PerlinMontanhas(x, z);
            case BiomeType.ForestNormal_Dense:
                return Get_PerlinForestNormal_Dense(x, z);
            default:
                return 0;
        }
    }

    static BiomeType GetVizinhos(int x, int z)
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
        Scale = 30f;
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

        if ((int)sample2.Value == 1)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            tile.OwnedByCity = true;

            if (Tile_Vila_Up.Point != sample2.Point || Tile_Vila_Down.Point != sample2.Point || Tile_Vila_Left.Point != sample2.Point || Tile_Vila_Right.Point != sample2.Point)
            {
                if (Tile_Vila_Up.TileType != Game.Color("FFFFFF") || Tile_Vila_Down.TileType != Game.Color("FFFFFF") || Tile_Vila_Left.TileType != Game.Color("FFFFFF") || Tile_Vila_Right.TileType != Game.Color("FFFFFF"))
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.RockWall;
                    }
                    return TypeBlock.Rock;
                }
            }

            if (color == Game.Color("FF0000") || color == Game.Color("7F7F7F"))//Somthing Is On this tile
            {
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("FF0048"))//House Spawn Origin
            {
                tile.PLACER_DATA = Placer.VillagerHouse;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("2D92FF"))//Spawn Chest(TesteOnly)
            {
                tile.PLACER_DATA = Placer.BauWood;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("303030"))//Spawn BlackSmith
            {
                tile.PLACER_DATA = Placer.BlackSmith;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("FF6A00"))//Spawn AlchemistHouse
            {
                tile.PLACER_DATA = Placer.AlchemistHouse;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("FFD800"))//Spawn PostLight
            {
                tile.PLACER_DATA = Placer.PostLight;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("00FF90"))//Spawn HumanTradeTand
            {
                tile.PLACER_DATA = Placer.TendaHumanos;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("7FC9FF"))//Spawn TradeCart
            {
                tile.PLACER_DATA = Placer.TradeCart;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("FFFFFF"))//Road
            {
                return TypeBlock.DirtRoad;
            }
            else
            {
                System.Random rand = new System.Random(Game.GameManager.Seed + x * z + (tile.TileChunk.x + tile.TileChunk.z));
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
        }
        else
        {
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
                    System.Random rand = new System.Random(Game.GameManager.Seed + x * z + (tile.TileChunk.x + tile.TileChunk.z));
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

        if ((int)sample2.Value == 1)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            tile.OwnedByCity = true;

            if (Tile_Vila_Up.Point != sample2.Point || Tile_Vila_Down.Point != sample2.Point || Tile_Vila_Left.Point != sample2.Point || Tile_Vila_Right.Point != sample2.Point)
            {
                if (Tile_Vila_Up.TileType != Game.Color("FFFFFF") || Tile_Vila_Down.TileType != Game.Color("FFFFFF") || Tile_Vila_Left.TileType != Game.Color("FFFFFF") || Tile_Vila_Right.TileType != Game.Color("FFFFFF"))
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.RockWall;
                    }
                    return TypeBlock.Rock;
                }
            }

            if (color == Game.Color("FF0000") || color == Game.Color("7F7F7F"))//Somthing Is On this tile
            {
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("FF0048"))//House Spawn Origin
            {
                tile.PLACER_DATA = Placer.VillagerHouse;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("2D92FF"))//Spawn Chest(TesteOnly)
            {
                tile.PLACER_DATA = Placer.BauWood;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("303030"))//Spawn BlackSmith
            {
                tile.PLACER_DATA = Placer.BlackSmith;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("FF6A00"))//Spawn AlchemistHouse
            {
                tile.PLACER_DATA = Placer.AlchemistHouse;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("FFD800"))//Spawn PostLight
            {
                tile.PLACER_DATA = Placer.PostLight;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("00FF90"))//Spawn HumanTradeTand
            {
                tile.PLACER_DATA = Placer.TendaHumanos;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("7FC9FF"))//Spawn TradeCart
            {
                tile.PLACER_DATA = Placer.TradeCart;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("FFFFFF"))//Road
            {
                return TypeBlock.DirtRoad;
            }
            else
            {
                System.Random rand = new System.Random(Game.GameManager.Seed + x * z + (tile.TileChunk.x + tile.TileChunk.z));
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
        }
        else
        {
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
                    System.Random rand = new System.Random(Game.GameManager.Seed + x * z + (tile.TileChunk.x + tile.TileChunk.z));
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
    }

    private static TypeBlock Desert(int x, int z, Tile tile, float sample, float perlin)
    {
        float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.009f, 2, Game.GameManager.Seed, false).GetValue(x, z, 0);

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
                #region Desert
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
                        System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
                #endregion
            }
        }
        else
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
                    System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
    }

    private static TypeBlock OceanNormal(int x, int z, Tile tile, float sample)
    {
        float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.009f, 2, Game.GameManager.Seed, false).GetValue(x, z, 0);

        if ((int)sample2 == 1)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            tile.OwnedByCity = true;

            /*if (color == Game.Color("FFFFFF"))//Caiz
            {
                return TypeBlock.DirtRoad;
            }*/
            return TypeBlock.WaterFloor;
        }
        else
        {
            return TypeBlock.WaterFloor;
        }
    }

    private static TypeBlock Bench(int x, int z, Tile tile, float sample)
    {
        System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
                System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
        float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.009f, 2, Game.GameManager.Seed, false).GetValue(x, z, 0);

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

                        System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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

                    System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
                System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
                System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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



    public static float GetBiomeMap(float x, float z, Tile tile, float sample)
    {
        float readysample = sample;

        float Scale = 30f;
        int height = 50;
        int width = 50;
        float noisefactor = 0.1f;

        float xCorde = 0;
        float zCorde = 0;

        switch (tile.TileBiome)
        {
            case BiomeType.ForestNormal:
                Scale = 30f;
                height = 50;
                width = 50;

                noisefactor = 0.1f;

                xCorde = (float)x / width * Scale;
                zCorde = (float)z / height * Scale;
                readysample *= Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);

                return readysample;
            case BiomeType.Desert:
                Scale = 30f;
                height = 50;
                width = 50;

                noisefactor = 0.1f;

                xCorde = (float)x / width * Scale;
                zCorde = (float)z / height * Scale;
                readysample *= Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
                return readysample;
            case BiomeType.OceanNormal:

                return readysample;
            case BiomeType.Jungle:
                Scale = 30f;
                height = 50;
                width = 50;

                noisefactor = 0.1f;

                xCorde = (float)x / width * Scale;
                zCorde = (float)z / height * Scale;
                readysample *= Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
                return readysample;
            case BiomeType.Plain:
                Scale = 30f;
                height = 50;
                width = 50;

                noisefactor = 0.1f;

                xCorde = (float)x / width * Scale;
                zCorde = (float)z / height * Scale;
                readysample /= Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
                return readysample;
            case BiomeType.Snow:
                Scale = 30f;
                height = 50;
                width = 50;

                noisefactor = 0.1f;

                xCorde = (float)x / width * Scale;
                zCorde = (float)z / height * Scale;
                readysample *= Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
                return readysample;
            case BiomeType.Bench:
                return readysample;
            case BiomeType.None:
                return readysample;
            case BiomeType.Montahas:
                Scale = 20f;
                height = 50;
                width = 50;

                noisefactor = 0.01f;

                float persistence = 39.9f;
                float frequency = 0.001f;
                float amplitude = 52.79f;
                int octaves = 184;

                xCorde = (float)octaves * x / width * Scale + Game.GameManager.Small_Seed;
                zCorde = (float)octaves * z / height * Scale + Game.GameManager.Small_Seed;

                // modify with frequency
                xCorde *= frequency;
                zCorde *= frequency;

                readysample *= Mathf.PerlinNoise(xCorde, zCorde) * amplitude / persistence;
                return readysample;
            case BiomeType.Cave:
                return readysample;
            case BiomeType.ForestNormal_Dense:
                Scale = 30f;
                height = 50;
                width = 50;

                noisefactor = 0.1f;

                xCorde = (float)x / width * Scale;
                zCorde = (float)z / height * Scale;
                readysample *= Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);
                return readysample;
            default:
                return readysample;
        }
    }
}