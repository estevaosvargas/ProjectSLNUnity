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

    public Vector2[] GetTileUVs(Tile tile)
    {
        string key = tile.type.ToString() + "_";

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

    public Sprite RawSprite(Tile tile)
    {
        if (tileSprites.ContainsKey(tile.type.ToString() + "_"))
        {
            return tileSprites[tile.type.ToString() + "_"];
        }

        Debug.LogError("Nao encontrado esse tipo: " + tile.type.ToString() + "_");

        return null;
    }

    public Sprite RawSprite(TypeBlock tile)
    {
        if (tileSprites.ContainsKey(tile.ToString() + "_"))
        {
            return tileSprites[tile.ToString() + "_"];
        }

        Debug.LogError("Nao encontrado esse tipo: " + tile.ToString() + "_");

        return null;
    }

    public void GetTranssition(Tile tile)
    {
        Tile[] neighbors = tile.GetNeighboors(true);

        List<TransTypes> types = new List<TransTypes>();
        List<Sprite> Sprites = new List<Sprite>();
        List<TransData> tiletrans = new List<TransData>();

        //0 cima
        //1 direita
        //2 baixo
        //3 esquerda

        //4 corn baixo direita
        //5 corn cima esquerda
        //6 corn cima direita
        //7 corn baixo esuqerda

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                if (!types.Contains(new TransTypes(neighbors[i])))
                {
                    types.Add(new TransTypes(neighbors[i]));
                }
            }
        }

        for (int i = 0; i < types.Count; i++)
        {
            string name = "";

            if (Get.HaveBlend(types[i].type) == true)//Checar se esse Tile Vizinho, permita transsiao
            {
                if (Get.TileCanDoBlend(tile.type))//Checar se esse tipo de tile, pode fazer os cauculos de transsiçao
                {
                    if (Get.CanTransitionTo(types[i].type, tile.type))
                    {
                        if (types[i].type != tile.type)//Checar se este tile n e o memso da trassisao, se for o mesmo nao tem por que fazer transiçao
                        {
                            if (neighbors[0] != null && neighbors[1] != null && neighbors[2] != null && neighbors[3] != null && neighbors[4] != null && neighbors[5] != null && neighbors[6] != null && neighbors[7] != null)
                            {
                                if (neighbors[0].type == types[i].type && neighbors[1].type == types[i].type && neighbors[3].type == types[i].type && neighbors[2].type == types[i].type)//Todos Os Lados = 0123
                                {
                                    if (tile.type != types[i].type)
                                    {
                                        name = types[i].type.ToString() + "_" + "0";
                                    }
                                }
                                else if (neighbors[0].type != types[i].type && neighbors[1].type == types[i].type && neighbors[2].type == types[i].type && neighbors[3].type == types[i].type)//123
                                {
                                    name = types[i].type.ToString() + "_" + "1";
                                }
                                else if (neighbors[0].type == types[i].type && neighbors[1].type == types[i].type && neighbors[2].type == types[i].type && neighbors[3].type != types[i].type)//012
                                {
                                    name = types[i].type.ToString() + "_" + "2";
                                }
                                else if (neighbors[0].type != types[i].type && neighbors[1].type == types[i].type && neighbors[2].type == types[i].type && neighbors[3].type != types[i].type)//12
                                {
                                    name = types[i].type.ToString() + "_" + "3";
                                }
                                else if (neighbors[0].type == types[i].type && neighbors[1].type != types[i].type && neighbors[2].type == types[i].type && neighbors[3].type == types[i].type)//023
                                {
                                    name = types[i].type.ToString() + "_" + "4";
                                }
                                else if (neighbors[0].type != types[i].type && neighbors[1].type != types[i].type && neighbors[2].type == types[i].type && neighbors[3].type == types[i].type)//23
                                {
                                    name = types[i].type.ToString() + "_" + "5";
                                }
                                else if (neighbors[0].type == types[i].type && neighbors[1].type != types[i].type && neighbors[2].type == types[i].type && neighbors[3].type != types[i].type)//02
                                {
                                    name = types[i].type.ToString() + "_" + "6";
                                }
                                else if (neighbors[0].type != types[i].type && neighbors[1].type != types[i].type && neighbors[2].type == types[i].type && neighbors[3].type != types[i].type)//2
                                {
                                    name = types[i].type.ToString() + "_" + "7";
                                }
                                else if (neighbors[0].type == types[i].type && neighbors[1].type == types[i].type && neighbors[2].type != types[i].type && neighbors[3].type == types[i].type)//013
                                {
                                    name = types[i].type.ToString() + "_" + "8";
                                }
                                else if (neighbors[0].type != types[i].type && neighbors[1].type == types[i].type && neighbors[2].type != types[i].type && neighbors[3].type == types[i].type)//13
                                {
                                    name = types[i].type.ToString() + "_" + "9";
                                }
                                else if (neighbors[0].type == types[i].type && neighbors[1].type == types[i].type && neighbors[2].type != types[i].type && neighbors[3].type != types[i].type)//012
                                {
                                    name = types[i].type.ToString() + "_" + "10";
                                }
                                else if (neighbors[0].type != types[i].type && neighbors[1].type == types[i].type && neighbors[2].type != types[i].type && neighbors[3].type != types[i].type)//1
                                {
                                    name = types[i].type.ToString() + "_" + "11";
                                }
                                else if (neighbors[0].type == types[i].type && neighbors[1].type != types[i].type && neighbors[2].type != types[i].type && neighbors[3].type == types[i].type)//03
                                {
                                    name = types[i].type.ToString() + "_" + "12";
                                }
                                else if (neighbors[0].type != types[i].type && neighbors[1].type != types[i].type && neighbors[2].type != types[i].type && neighbors[3].type == types[i].type)//3
                                {
                                    name = types[i].type.ToString() + "_" + "13";
                                }
                                else if (neighbors[0].type == types[i].type && neighbors[1].type != types[i].type && neighbors[2].type != types[i].type && neighbors[3].type != types[i].type)//0
                                {
                                    name = types[i].type.ToString() + "_" + "14";
                                }
                                else if (neighbors[4].type == types[i].type && neighbors[5].type != types[i].type && neighbors[6].type != types[i].type && neighbors[7].type != types[i].type)//corn baixo direita
                                {
                                    name = types[i].type.ToString() + "_" + "16";
                                }
                                else if (neighbors[4].type != types[i].type && neighbors[5].type != types[i].type && neighbors[6].type != types[i].type && neighbors[7].type == types[i].type)//corn baixo esuqerda
                                {
                                    name = types[i].type.ToString() + "_" + "17";
                                }
                                else if (neighbors[4].type != types[i].type && neighbors[5].type != types[i].type && neighbors[6].type == types[i].type && neighbors[7].type != types[i].type)//corn cima direita
                                {
                                    name = types[i].type.ToString() + "_" + "18";
                                }
                                else if (neighbors[4].type != types[i].type && neighbors[5].type == types[i].type && neighbors[6].type != types[i].type && neighbors[7].type != types[i].type)//corn cima esquerda
                                {
                                    name = types[i].type.ToString() + "_" + "19";
                                }
                                else if (neighbors[4].type == types[i].type && neighbors[5].type != types[i].type && neighbors[6].type != types[i].type && neighbors[7].type == types[i].type)//corn baixo 2
                                {
                                    name = types[i].type.ToString() + "_" + "20";
                                }
                                else if (neighbors[4].type != types[i].type && neighbors[5].type == types[i].type && neighbors[6].type == types[i].type && neighbors[7].type != types[i].type)//corn cima 2
                                {
                                    name = types[i].type.ToString() + "_" + "21";
                                }
                                else if (neighbors[4].type != types[i].type && neighbors[5].type == types[i].type && neighbors[6].type != types[i].type && neighbors[7].type == types[i].type)//corn Esquerda Esquerda(Cima Baixo)
                                {
                                    name = types[i].type.ToString() + "_" + "22";
                                }
                                else if (neighbors[4].type == types[i].type && neighbors[5].type != types[i].type && neighbors[6].type == types[i].type && neighbors[7].type != types[i].type)//corn Direita Esquerda(Cima Baixo)
                                {
                                    name = types[i].type.ToString() + "_" + "23";
                                }
                            }
                        }
                    }
                }

                if (name != "")
                {
                    if (!tiletrans.Contains(new TransData(name, types[i].type, types[i].TileBiome)))
                    {
                        tiletrans.Add(new TransData(name, types[i].type, types[i].TileBiome));
                    }
                }
            }
            else
            {

            }
        }

        tile.TileTran = tiletrans.ToArray();
    }

    public Sprite GetSprite(string spriteName)
    {
        if (tileSprites.ContainsKey(spriteName))
        {
            return tileSprites[spriteName];
        }
        Debug.LogError("Dont found this sprite : " + spriteName);
        return null;
    }

    public Sprite GetSprite(Tile tile)
    {
        string name = tile.type.ToString() + "_";

        if (tile.typeVariante != TypeVariante.none)
        {
            name = tile.typeVariante.ToString();
        }

        if (tile.ConnecyToNightboors)
        {
            Tile[] neighbors = tile.GetNeighboors(true);

            //nameredy = GetName(neighbors, tile, name);

            //0 cima
            //1 direita
            //2 baixo
            //3 esquerda

            //4 corn baixo direita
            //5 corn cima esquerda
            //6 corn cima direita
            //7 corn baixo esuqerda

            if (tile.type == TypeBlock.Rock)
            {
                if (neighbors[0] != null && neighbors[1] != null && neighbors[2] != null && neighbors[3] != null && neighbors[4] != null && neighbors[5] != null && neighbors[6] != null && neighbors[7] != null)
                {
                    if (neighbors[0].type == tile.type && neighbors[1].type != tile.type && neighbors[2].type != tile.type && neighbors[3].type != tile.type)//123
                    {
                        name = tile.type.ToString() + "_tile_" + "1";
                    }
                    else if (neighbors[0].type != tile.type && neighbors[1].type != tile.type && neighbors[2].type != tile.type && neighbors[3].type == tile.type)//012
                    {
                        name = tile.type.ToString() + "_tile_" + "2";
                    }
                    else if (neighbors[0].type == tile.type && neighbors[1].type != tile.type && neighbors[2].type != tile.type && neighbors[3].type == tile.type)//12
                    {
                        name = tile.type.ToString() + "_tile_" + "3";
                    }
                    else if (neighbors[0].type != tile.type && neighbors[1].type == tile.type && neighbors[2].type != tile.type && neighbors[3].type != tile.type)//023
                    {
                        name = tile.type.ToString() + "_tile_" + "4";
                    }
                    else if (neighbors[0].type == tile.type && neighbors[1].type == tile.type && neighbors[2].type != tile.type && neighbors[3].type != tile.type)//23
                    {
                        name = tile.type.ToString() + "_tile_" + "5";
                    }
                    else if (neighbors[0].type != tile.type && neighbors[1].type == tile.type && neighbors[2].type != tile.type && neighbors[3].type == tile.type)//02
                    {
                        name = tile.type.ToString() + "_tile_" + "6";
                    }
                    else if (neighbors[0].type == tile.type && neighbors[1].type == tile.type && neighbors[2].type != tile.type && neighbors[3].type == tile.type)//2
                    {
                        name = tile.type.ToString() + "_tile_" + "7";
                    }
                    else if (neighbors[0].type != tile.type && neighbors[1].type != tile.type && neighbors[2].type == tile.type && neighbors[3].type != tile.type)//013
                    {
                        name = tile.type.ToString() + "_tile_" + "8";
                    }
                    else if (neighbors[0].type == tile.type && neighbors[1].type != tile.type && neighbors[2].type == tile.type && neighbors[3].type != tile.type)//13
                    {
                        name = tile.type.ToString() + "_tile_" + "9";
                    }
                    else if (neighbors[0].type != tile.type && neighbors[1].type != tile.type && neighbors[2].type == tile.type && neighbors[3].type == tile.type)//012
                    {
                        name = tile.type.ToString() + "_tile_" + "10";
                    }
                    else if (neighbors[0].type == tile.type && neighbors[1].type != tile.type && neighbors[2].type == tile.type && neighbors[3].type == tile.type)//1
                    {
                        name = tile.type.ToString() + "_tile_" + "11";
                    }
                    else if (neighbors[0].type != tile.type && neighbors[1].type == tile.type && neighbors[2].type == tile.type && neighbors[3].type != tile.type)//03
                    {
                        name = tile.type.ToString() + "_tile_" + "12";
                    }
                    else if (neighbors[0].type == tile.type && neighbors[1].type == tile.type && neighbors[2].type == tile.type && neighbors[3].type != tile.type)//3
                    {
                        name = tile.type.ToString() + "_tile_" + "13";
                    }
                    else if (neighbors[0].type != tile.type && neighbors[1].type == tile.type && neighbors[2].type == tile.type && neighbors[3].type == tile.type)//0
                    {
                        name = tile.type.ToString() + "_tile_" + "14";
                    }
                    else if (neighbors[4].type != tile.type && neighbors[5].type == tile.type && neighbors[6].type == tile.type && neighbors[7].type == tile.type)//corn baixo direita
                    {
                        name = tile.type.ToString() + "_tile_" + "16";
                    }
                    else if (neighbors[4].type == tile.type && neighbors[5].type == tile.type && neighbors[6].type == tile.type && neighbors[7].type != tile.type)//corn baixo esuqerda
                    {
                        name = tile.type.ToString() + "_tile_" + "17";
                    }
                    else if (neighbors[4].type == tile.type && neighbors[5].type == tile.type && neighbors[6].type != tile.type && neighbors[7].type == tile.type)//corn cima direita
                    {
                        name = tile.type.ToString() + "_tile_" + "18";
                    }
                    else if (neighbors[4].type == tile.type && neighbors[5].type != tile.type && neighbors[6].type == tile.type && neighbors[7].type == tile.type)//corn cima esquerda
                    {
                        name = tile.type.ToString() + "_tile_" + "19";
                    }
                    else if (neighbors[4].type != tile.type && neighbors[5].type == tile.type && neighbors[6].type == tile.type && neighbors[7].type != tile.type)//corn baixo 2
                    {
                        name = tile.type.ToString() + "_tile_" + "20";
                    }
                    else if (neighbors[4].type == tile.type && neighbors[5].type != tile.type && neighbors[6].type != tile.type && neighbors[7].type == tile.type)//corn cima 2
                    {
                        name = tile.type.ToString() + "_tile_" + "21";
                    }
                    else if (neighbors[4].type == tile.type && neighbors[5].type != tile.type && neighbors[6].type == tile.type && neighbors[7].type != tile.type)//corn Esquerda Esquerda(Cima Baixo)
                    {
                        name = tile.type.ToString() + "_tile_" + "22";
                    }
                    else if (neighbors[4].type != tile.type && neighbors[5].type == tile.type && neighbors[6].type != tile.type && neighbors[7].type == tile.type)//corn Direita Esquerda(Cima Baixo)
                    {
                        name = tile.type.ToString() + "_tile_" + "23";
                    }
                    else if (neighbors[0].type != tile.type && neighbors[1].type != tile.type && neighbors[2].type != tile.type && neighbors[3].type != tile.type)//Check if tile is solo
                    {
                        name = tile.type.ToString() + "_S";
                    }
                }
            }
        }

        if (tileSprites.ContainsKey(name))
        {
            return tileSprites[name];
        }
        return null;
    }

    public string GetName(Tile[] neighbors, Tile tile, string name)
    {
        if (neighbors.Length < 5)
        {
            if (neighbors[0] != null && neighbors[0].type == tile.type)
            {
                name += "N";
            }
        }
        else if (neighbors.Length > 4)
        {
            if (neighbors[0] != null && neighbors[0].type == tile.type)
            {
                name += "N";
            }
        }

        return "";
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

    public TransTypes(Tile tile)
    {
        type = tile.type;
        TileBiome = tile.TileBiome;
    }
}
