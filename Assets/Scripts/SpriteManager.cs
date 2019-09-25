using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public List<ConnectDataBase> ConnectData = new List<ConnectDataBase>();
    public static SpriteManager Instance;

    Dictionary<string, ConnectDataDirec> diccondata = new Dictionary<string, ConnectDataDirec>();

    Dictionary<string, Sprite> tileSprites;
    Dictionary<string, Texture2D> Perlinimage;

    void Awake()
    {
        Instance = this;
        LoadSystems.LoadingSprites();
        tileSprites = new Dictionary<string, Sprite>();
        Perlinimage = new Dictionary<string, Texture2D>();

        foreach (ConnectDataBase data in ConnectData)
        {
            diccondata.Add(data.Type.ToString(), new ConnectDataDirec(data));
        }
        ConnectData.Clear();

        LoadSprites();
        LoadSystems.LoadedSprites();
    }

    public GameObject GetPrefabbyname(string name)
    {
        GameObject[] sprites = Resources.LoadAll<GameObject>("Prefabs/Trees/");

        foreach (var s in sprites)
        {
            if (s.name == name)
            {
                return s;
            }
        }
        Debug.LogError("Nao encontrado esse tipo: " + name);
        return null;
    }

    public static Sprite Getitemicon(string name)
    {
        Sprite obj = Resources.Load<Sprite>("ItemsSprites/" + "Icon_" + name);

        if (obj != null)
        {
            return obj;
        }

        Debug.LogError("Nao encontrado esse tipo: " + name);
        return null;
    }

    public GameObject Getmobbyname(string name)
    {
        GameObject[] sprites = Resources.LoadAll<GameObject>("Prefabs/Mobs/");

        foreach (var s in sprites)
        {
            if (s.name == name)
            {
                return s;
            }
        }
        Debug.LogError("Nao encontrado esse tipo: " + name);
        return null;
    }

    public GameObject Getplacerbyname(string name)
    {
        GameObject[] sprites = Resources.LoadAll<GameObject>("Prefabs/Placer/");

        foreach (var s in sprites)
        {
            if (s.name == name)
            {
                return s;
            }
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

    void LoadSprites()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Caves01");

        foreach (var s in textures)
        {
            Perlinimage.Add(s.name, s);
        }

        foreach (var s in Resources.LoadAll<Sprite>("Tiles/"))
        {
            tileSprites.Add(s.name, s);
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

    public void GetTranssition(Tile tile, TileObj script)
    {
        Tile[] neighbors = tile.GetNeighboors(true);

        List<TransTypes> types = new List<TransTypes>();
        List<Sprite> Sprites = new List<Sprite>();

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

            if (GetPresets.HaveBlend(types[i].type) == true)//Checar se esse Tile Vizinho, permita transsiao
            {
                if (GetPresets.TileCanDoBlend(tile.type))//Checar se esse tipo de tile, pode fazer os cauculos de transsiçao
                {
                    if (GetPresets.CanTransitionTo(types[i].type, tile.type))
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
                    if (tileSprites.ContainsKey(name))
                    {
                        script.Tiles.Add(new TransData(tileSprites[name], types[i].type, types[i].TileBiome));
                    }
                    else
                    {
                        Debug.LogError("Nao Encontrado Esse Sprite : " + name);
                    }
                }
            }
            else
            {

            }
        }
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
public class ConnectDataBase
{
    public TypeBlock Type;
    public TypeBlock[] TypeAllowed;
}

[System.Serializable]
public class ConnectDataDirec
{
    public TypeBlock Type;
    public Dictionary<string, TypeBlock> TypeBlocked = new Dictionary<string, TypeBlock>();

    public ConnectDataDirec(ConnectDataBase bases)
    {
        Type = bases.Type;
        foreach (TypeBlock block in bases.TypeAllowed)
        {
            TypeBlocked.Add(block.ToString(), block);
        }
    }
}

[System.Serializable]
public class TransData
{
    public Sprite sprite;
    public TypeBlock type;
    public BiomeType Biome;

    public TransData(Sprite Sprite, TypeBlock TileType, BiomeType biome)
    {
        sprite = Sprite;
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
