using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    public static MiniMapManager manager;
    public RawImage ImageMap;

    public int width = 256;
    public int height = 256;

    private void Awake()
    {
        manager = this;
    }

    public void UpdateMap()
    {
        ImageMap.texture = GenerateTexture();
        ImageMap.texture.filterMode = FilterMode.Point;
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }

    Color CalculateColor(int x, int y)
    {
        Tile tile = new Tile();

        x = x + (int)Game.GameManager.CurrentPlayer.MyObject.transform.position.x -25;
        y = y + (int)Game.GameManager.CurrentPlayer.MyObject.transform.position.y -25;

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

        float sample = Mathf.PerlinNoise(xCordee + Game.GameManager.Seed, yCordee + Game.GameManager.Seed) * amplitude / persistence;

        if (Game.WorldGenerator.CurrentWorld == WorldType.Caves)
        {
            
        }
        else if (Game.WorldGenerator.CurrentWorld == WorldType.Normal)
        {
            // normal 0.8f
            if (sample > 0.5f)
            {
                float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.005f, 1, Game.GameManager.Seed, false).GetValue(x, y, 0);

                sample2 *= 10;

                return GetWhatBlocks(SetUpBiome(x, y, tile, sample, sample2));
            }
            else if (sample > 0.3f)
            {
                return GetWhatBlocks(Biome.Bench(x, y, tile, sample));
            }
            else
            {
                return GetWhatBlocks(Biome.OceanNormal(x, y, tile, sample));
            }
        }
        return Color.black;
    }

    Color GetWhatBlocks(TypeBlock block)
    {
        switch (block)
        {
            case TypeBlock.Water:
                return Color.blue;
            case TypeBlock.Grass:
                return Color.green;
            case TypeBlock.Rock:
                return Color.gray;
            case TypeBlock.Sand:
                return Color.yellow;
            default:
                return Color.black;
        }
    }

    public TypeBlock SetUpBiome(int x, int y, Tile tile, float sample, float sample2)
    {
        if ((int)sample2 == 0)
        {
            //sem nemhum
            return Biome.ForestNormal(x, y, tile, sample);
        }
        else if ((int)sample2 == 1)
        {
            //Jungle
            return Biome.Jungle(x, y, tile, sample);
        }
        else if ((int)sample2 == 2)
        {
            //Oceano Normal
            return Biome.ForestNormal(x, y, tile, sample);
        }
        else if ((int)sample2 == 3)
        {
            //Deserto
            return Biome.Montanhas(x, y, tile, sample);
        }
        else if ((int)sample2 == 4)
        {
            //sem nemhum
            return Biome.Plaine(x, y, tile, sample);
        }
        else if ((int)sample2 == 5)
        {
            //sem nemhum
            return Biome.ForestSnow(x, y, tile, sample);
        }
        else if ((int)sample2 == 6)
        {
            //sem nemhum
            return Biome.Montanhas(x, y, tile, sample);
        }
        else if ((int)sample2 == 7)
        {
            //sem nemhum
            return Biome.Desert(x, y, tile, sample);
        }
        else if ((int)sample2 == -4)
        {
            //sem nemhum
            return Biome.Plaine(x, y, tile, sample);
        }
        else
        {
            return Biome.ForestNormal(x, y, tile, sample);
        }
    }
}