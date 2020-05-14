using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    Dictionary<string, Sprite> tileSprites;
    Dictionary<string, Texture2D> Perlinimage;
    Dictionary<string, GameObject> Objects = new Dictionary<string, GameObject>();
    Dictionary<string, Sprite> Icons = new Dictionary<string, Sprite>();
    Dictionary<string, Vector2[]> tileUVMap = new Dictionary<string, Vector2[]>();

    public Texture2D terrainTiles;

    void Awake()
    {
        Game.SpriteManager = this;

        LoadSystems.LoadingSprites();
        tileSprites = new Dictionary<string, Sprite>();
        Perlinimage = new Dictionary<string, Texture2D>();

        LoadAssets();
        LoadTileUvs();
        LoadSystems.LoadedSprites();
    }

    void LoadTileUvs()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Tiles/");

        float imageWidth = terrainTiles.width;
        float imageHeight = terrainTiles.height;

        foreach (Sprite s in sprites)
        {

            Vector2[] uvs = new Vector2[4];

            uvs[0] = new Vector2(s.rect.x / imageWidth, s.rect.y / imageHeight);
            uvs[1] = new Vector2((s.rect.x + s.rect.width) / imageWidth, s.rect.y / imageHeight);
            uvs[2] = new Vector2(s.rect.x / imageWidth, (s.rect.y + s.rect.height) / imageHeight);
            uvs[3] = new Vector2((s.rect.x + s.rect.width) / imageWidth, (s.rect.y + s.rect.height) / imageHeight);

            tileUVMap.Add(s.name, uvs);
        }
    }

    public Vector2[] GetTileUVs(Block tile)
    {
        string key = tile.Type.ToString() + "_";

        if (tile.typeVariante != TypeVariante.none)
        {
            key = tile.typeVariante.ToString();
        }

        if (tileUVMap.ContainsKey(key) == true)
        {

            return tileUVMap[key];
        }
        else
        {

            Debug.LogError("There is no UV map for tile type: " + key);
            return tileUVMap["Air_"];
        }
    }

    public GameObject GetHandItem(string name)
    {
        if (Objects.ContainsKey(name))
        {
            return Objects[name];
        }
        Debug.LogError("Nao encontrado esse tipo: " + name);
        return null;
    }

    public GameObject GetPrefabbyname(string name)
    {
        if (Objects.ContainsKey(name))
        {
            return Objects[name];
        }
        Debug.LogError("Nao encontrado esse tipo: " + name);
        return null;
    }

    public Sprite Getitemicon(string name)
    {
        name = "Icon_" + name;

        if (Icons.ContainsKey(name))
        {
            return Icons[name];
        }

        Debug.LogError("Nao encontrado esse tipo: " + name);
        return null;
    }

    public GameObject Getmobbyname(string name)
    {
        if (Objects.ContainsKey(name))
        {
            return Objects[name];
        }
        Debug.LogError("Nao encontrado esse tipo: " + name);
        return null;
    }

    public GameObject Getplacerbyname(string name)
    {
        if (Objects.ContainsKey(name))
        {
            return Objects[name];
        }
        Debug.LogError("Nao encontrado esse tipo: " + name);
        return null;
    }

    public Texture2D GetPerlinImage(string name)
    {
        if (Perlinimage.ContainsKey(name))
        {
            return Perlinimage[name];
        }

        Debug.LogError("Nao encontrado esse tipo: " + name);

        return null;
    }

    public GameObject GetPrefabOnRecources(string path)
    {
        GameObject sprites = Resources.Load<GameObject>(path);
        
        if (sprites)
        {
            return sprites;
        }

        Debug.LogError("Don't find this file: " + path);
        return null;
    }

    void LoadAssets()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Caves01");

        foreach (var s in textures)
        {
            Perlinimage.Add(s.name, s);
        }

        foreach (var icon in Resources.LoadAll<Sprite>("ItemsSprites/"))
        {
            Icons.Add(icon.name, icon);
        }

        foreach (var icon in Resources.LoadAll<Sprite>("Tiles/Old/"))
        {
            tileSprites.Add(icon.name, icon);
        }

        foreach (var s in Resources.LoadAll<GameObject>("Prefabs/Trees/"))
        {
            Objects.Add(s.name, s);
        }

        foreach (var s in Resources.LoadAll<GameObject>("Prefabs/Placer/"))
        {
            Objects.Add(s.name, s);
        }

        foreach (var s in Resources.LoadAll<GameObject>("Prefabs/AI/"))
        {
            Objects.Add(s.name, s);
        }

        foreach (var s in Resources.LoadAll<GameObject>("Prefabs/HandItems/"))
        {
            Objects.Add(s.name, s);
        }
    }
}

[System.Serializable]
public class TransData
{
    public string Name;
    public TypeBlock type;
    public BiomeType Biome;

    public TransData(string name, TypeBlock TileType, BiomeType biome)
    {
        Name = name;
        type = TileType;
        Biome = biome;
    }
}

public struct TransTypes
{
    public TypeBlock type;
    public BiomeType TileBiome;

    public TransTypes(Block tile)
    {
        type = tile.Type;
        TileBiome = tile.TileBiome;
    }
}
