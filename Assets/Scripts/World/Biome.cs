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

    public static TypeBlock ForestNormal(int x, int z, Tile tile, float sample)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);

        LibNoise.Unity.Generator.Voronoi CityNoise = new LibNoise.Unity.Generator.Voronoi(0.009f, 2, Game.GameManager.Seed, false);

        VeronoiStruc sample2 = CityNoise.GetValueNPoint(x, z, 0);

        tile.CityPoint = new DataVector3(CityNoise.GetPoint(x, z, 0));

        //Debug.Log("village Chance : " + (int)sample2);

        //sample2 *= 10;

        VeronoiStruc Tile_Vila_Up = CityNoise.GetValueNPoint(x, z + 1, 0);
        VeronoiStruc Tile_Vila_Down = CityNoise.GetValueNPoint(x, z - 1, 0);
        VeronoiStruc Tile_Vila_Left = CityNoise.GetValueNPoint(x - 1, z, 0);
        VeronoiStruc Tile_Vila_Right = CityNoise.GetValueNPoint(x + 1, z, 0);

        /*Tile_Vila_Up.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x, z + 1);
        Tile_Vila_Down.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x, z - 1);
        Tile_Vila_Left.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x - 1, z);
        Tile_Vila_Right.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x + 1, z);*/

        if ((int)sample2.Value == 1)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            tile.OwnedByCity = true;

            /*if (Tile_Vila_Up.Point != sample2.Point || Tile_Vila_Down.Point != sample2.Point || Tile_Vila_Left.Point != sample2.Point || Tile_Vila_Right.Point != sample2.Point)
            {
                if (Tile_Vila_Up.TileType != Game.Color("FFFFFF") || Tile_Vila_Down.TileType != Game.Color("FFFFFF") || Tile_Vila_Left.TileType != Game.Color("FFFFFF") || Tile_Vila_Right.TileType != Game.Color("FFFFFF"))
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.RockWall;
                    }
                    return TypeBlock.Rock;
                }
            }*/

            if (color == Game.Color("FF0000") || color == Game.Color("7F7F7F"))//Somthing Is On this tile
            {
                return TypeBlock.Dirt;
            }
            else if (color == Game.Color("FF0048"))//House Spawn Origin
            {
                tile.PLACER_DATA = Placer.MainBuild2;
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

    public static TypeBlock ForestNormal_Dense(int x, int z, Tile tile, float sample)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);

        LibNoise.Unity.Generator.Voronoi CityNoise = new LibNoise.Unity.Generator.Voronoi(0.009f, 2, Game.GameManager.Seed, false);

        VeronoiStruc sample2 = CityNoise.GetValueNPoint(x, z, 0);

        tile.CityPoint = new DataVector3(CityNoise.GetPoint(x, z, 0));

        //Debug.Log("village Chance : " + (int)sample2);

        //sample2 *= 10;

        VeronoiStruc Tile_Vila_Up = CityNoise.GetValueNPoint(x, z + 1, 0);
        VeronoiStruc Tile_Vila_Down = CityNoise.GetValueNPoint(x, z - 1, 0);
        VeronoiStruc Tile_Vila_Left = CityNoise.GetValueNPoint(x - 1, z, 0);
        VeronoiStruc Tile_Vila_Right = CityNoise.GetValueNPoint(x + 1, z, 0);

        /*Tile_Vila_Up.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x, z + 1);
        Tile_Vila_Down.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x, z - 1);
        Tile_Vila_Left.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x - 1, z);
        Tile_Vila_Right.TileType = Game.WorldGenerator.HeightTeste.GetPixel(x + 1, z);*/

        if ((int)sample2.Value == 1)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            tile.OwnedByCity = true;

            /*if (Tile_Vila_Up.Point != sample2.Point || Tile_Vila_Down.Point != sample2.Point || Tile_Vila_Left.Point != sample2.Point || Tile_Vila_Right.Point != sample2.Point)
            {
                if (Tile_Vila_Up.TileType != Game.Color("FFFFFF") || Tile_Vila_Down.TileType != Game.Color("FFFFFF") || Tile_Vila_Left.TileType != Game.Color("FFFFFF") || Tile_Vila_Right.TileType != Game.Color("FFFFFF"))
                {
                    if (tile.typego == TakeGO.empty && tile.z != 0)
                    {
                        tile.typego = TakeGO.RockWall;
                    }
                    return TypeBlock.Rock;
                }
            }*/

            if (color == Game.Color("FF0000") || color == Game.Color("7F7F7F"))//Somthing Is On this tile
            {
                return TypeBlock.Dirt;
            }
            else if (color == Game.Color("FF0048"))//House Spawn Origin
            {
                tile.PLACER_DATA = Placer.MainBuild2;
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

    public static TypeBlock Desert(int x, int z, Tile tile, float sample)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale + Game.GameManager.Small_Seed;
        float zCorde = (float)z / height * Scale + Game.GameManager.Small_Seed;

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor, zCorde * noisefactor);

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
                tile.PLACER_DATA = Placer.MainBuild2;
                return TypeBlock.Sand;
            }
            else if (color == Game.Color("2D92FF"))//Spawn Chest(TesteOnly)
            {
                tile.PLACER_DATA = Placer.BauWood;
                return TypeBlock.Sand;
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

    public static TypeBlock OceanNormal(int x, int z, Tile tile, float sample)
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

    public static TypeBlock Bench(int x, int z, Tile tile, float sample)
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

    public static TypeBlock Jungle(int x, int z, Tile tile, float sample)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);

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

    public static TypeBlock Plaine(int x, int z, Tile tile, float sample)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);

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
                tile.PLACER_DATA = Placer.MainBuild2;
                return TypeBlock.Grass;
            }
            else if (color == Game.Color("2D92FF"))//Spawn Chest(TesteOnly)
            {
                tile.PLACER_DATA = Placer.BauWood;
                return TypeBlock.Grass;
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

    public static TypeBlock ForestSnow(int x, int z, Tile tile, float sample)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.GameManager.Small_Seed, zCorde * noisefactor + Game.GameManager.Small_Seed);

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

    public static TypeBlock Montanhas(int x, int z, Tile tile, float sample)
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

        float perlin = Mathf.PerlinNoise(xCordee, zCordee) * amplitude / persistence;


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
                System.Random rand = new System.Random(Game.GameManager.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
                int randnum = (rand.Next(1, 20));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.Weed01;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 2)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.RockProp;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 3)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.WeedTall;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 4)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
                    {
                        tile.typego = TakeGO.Pine;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty && tile.y != 0)
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