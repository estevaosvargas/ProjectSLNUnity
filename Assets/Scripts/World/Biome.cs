using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Unity.Generator;

public enum BiomeType { ForestNormal = 0, Desert = 1, OceanNormal = 2, Jungle = 3, Plain = 4, Snow = 5, Bench = 6 , None, Montahas, Cave}

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

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.WorldGenerator.Seed, zCorde * noisefactor + Game.WorldGenerator.Seed);
        float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.01f, 5, Game.WorldGenerator.Seed, false).GetValue(x, z, 0);

        //Debug.Log("village Chance : " + (int)sample2);

        //sample2 *= 10;
        if ((int)sample2 == 2 || (int)sample2 == -2)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            #region Villa

            if (color == new Color(1, 0, 0, 1))
            {
                tile.placerObj = Placer.MainBuild2;
                return TypeBlock.Air;
            }
            else if (color == new Color(1, 1, 1, 1))
            {
                return TypeBlock.DirtRoad;
            }
            else
            {
                System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
                int randnum = (rand.Next(1, 30));

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
            #endregion
        }
        else
        {
            if (perlin <= 0.15f)
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
                    System.Random rand = new System.Random(Game.WorldGenerator.Seed + x * z + (tile.TileChunk.x + tile.TileChunk.z));
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

    public static TypeBlock Desert(int x, int z, Tile tile, float sample)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale + Game.WorldGenerator.Seed;
        float zCorde = (float)z / height * Scale + Game.WorldGenerator.Seed;

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor, zCorde * noisefactor);

        float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.01f, 5, Game.WorldGenerator.Seed, false).GetValue(x, z, 0);

        if ((int)sample2 == 2 || (int)sample2 == -2)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            #region Villa

            if (color == new Color(1, 0, 0, 1))
            {
                tile.placerObj = Placer.MainBuild2;
                return TypeBlock.Air;
            }
            else if (color == new Color(1, 1, 1, 1))
            {
                return TypeBlock.DirtRoad;
            }
            else
            {
                System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
            #endregion
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
                    System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
        return TypeBlock.Water;
    }

    public static TypeBlock Bench(int x, int z, Tile tile, float sample)
    {
        if (sample > 0.48f)
        {
            System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
            int randnum = (rand.Next(1, 20));

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
        else if (sample > 0.47f)
        {
            return TypeBlock.BeachSand;
        }
        else
        {
            return TypeBlock.Water;
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

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.WorldGenerator.Seed, zCorde * noisefactor + Game.WorldGenerator.Seed);

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
                System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
                int randnum = (rand.Next(1, 20));

                if (randnum == 1)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.BigTree;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 3)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.BigTree2;
                    }
                    return TypeBlock.Grass;
                }
                else if (randnum == 5)
                {
                    if (tile.typego == TakeGO.empty)
                    {
                        tile.typego = TakeGO.WeedTall_Jungle;
                    }
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
        return TypeBlock.Grass;
    }

    public static TypeBlock Plaine(int x, int z, Tile tile, float sample)
    {
        Scale = 30f;
        height = 50;
        width = 50;

        noisefactor = 0.1f;

        float xCorde = (float)x / width * Scale;
        float zCorde = (float)z / height * Scale;

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.WorldGenerator.Seed, zCorde * noisefactor + Game.WorldGenerator.Seed);

        float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.01f, 5, Game.WorldGenerator.Seed, false).GetValue(x, z, 0);

        if ((int)sample2 == 2 || (int)sample2 == -2)
        {
            Color color = Game.WorldGenerator.HeightTeste.GetPixel(x, z);

            #region Villa

            if (color == new Color(1, 0, 0, 1))
            {
                tile.placerObj = Placer.MainBuild2;
                return TypeBlock.Grass;
            }
            else if (color == new Color(1, 1, 1, 1))
            {
                return TypeBlock.DirtRoad;
            }
            else
            {
                System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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
            #endregion
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

                    System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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

        float perlin = Mathf.PerlinNoise(xCorde * noisefactor + Game.WorldGenerator.Seed, zCorde * noisefactor + Game.WorldGenerator.Seed);

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
                System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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

        float xCordee = (float)octaves * x / width * Scale + Game.WorldGenerator.Seed;
        float zCordee = (float)octaves * z / height * Scale + Game.WorldGenerator.Seed;

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
                System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * (tile.TileChunk.x + tile.TileChunk.z));
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