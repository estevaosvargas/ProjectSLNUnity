using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CityManager : MonoBehaviour
{
    public string CityName = "";
    public string CityId = "";
    public int Population = 0;
    public int MaxPopulation = 10;
    public int CityDistance = 30;
    public CityLevel Level;
    public CityStatus cityStatus;
    public List<Vilanger> Vilangers = new List<Vilanger>();
    public Chunk ThisChunk;
    public List<CityVector2> OthersBuild = new List<CityVector2>();

    void Start()
    {
        ComputCity();
    }

    void Update()
    {

    }

    public void Upgrade()
    {

    }

    public void AddVilanger(Vilanger ai)
    {
        if (Population >= MaxPopulation)
        {
            Debug.Log("City Is Full!");
        }
        else
        {
            Vilangers.Add(ai);
            Population += 1;
        }
    }

    public void RemoveVilanger(Vilanger ai)
    {
        Vilangers.Remove(ai);
        Population -= 1;
    }

    public void ChangeName(string name)
    {
        CityName = name;
    }

    public void OnDrawGizmos()
    {
        foreach (CityVector2 n in OthersBuild)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(n.x, 0, n.z), Vector3.one);
        }
    }

    public void ComputCity()
    {
        for (int x = -CityDistance * 1; x < CityDistance* 1; x++)
        {
            for (int z = -CityDistance * 1; z < CityDistance * 1; z++)
            {
                Tile tile = Game.WorldGenerator.GetTileAt(x + (int)ThisChunk.transform.position.x, z + (int)ThisChunk.transform.position.z);
                float sample2 = (float)new LibNoise.Unity.Generator.Voronoi(0.01f, 5, Game.WorldGenerator.Seed, false).GetValue(x + (int)ThisChunk.transform.position.x, z + (int)ThisChunk.transform.position.z, 0);

                if (tile != null)
                {
                    if ((int)sample2 == 2 || (int)sample2 == -2)
                    {
                        Color color = Game.WorldGenerator.HeightTeste.GetPixel(x + (int)ThisChunk.transform.position.x, z + (int)ThisChunk.transform.position.z);

                        if (color == Game.Color("FF0000") || color == Game.Color("7F7F7F"))//Somthing Is On this tile
                        {
                            tile.PerlinSetType(TypeBlock.Dirt);
                        }
                        else if (color == Game.Color("FF0048"))//House Spawn Origin
                        {
                            tile.SetPlacer(Placer.MainBuild2);
                            tile.PerlinSetType(TypeBlock.Dirt);
                        }
                        else if (color == Game.Color("FFD800"))//CityHall Spawn Origin
                        {

                        }
                        else if (color == Game.Color("2D92FF"))//Spawn Chest(TesteOnly)
                        {
                            tile.SetPlacer(Placer.BauWood);
                            tile.PerlinSetType(TypeBlock.Dirt);
                        }
                        else if (color == Game.Color("FFFFFF"))//Road
                        {
                            tile.PerlinSetType(TypeBlock.Dirt);
                        }
                        else
                        {
                            System.Random rand = new System.Random(Game.WorldGenerator.Seed * x + z * ((int)ThisChunk.transform.position.x + (int)ThisChunk.transform.position.z));
                            int randnum = (rand.Next(1, 30));

                            if (randnum == 1)
                            {
                                if (tile.typego == TakeGO.empty && tile.z != 0)
                                {
                                    tile.typego = TakeGO.Weed01;
                                }
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                            else if (randnum == 2)
                            {
                                if (tile.typego == TakeGO.empty && tile.z != 0)
                                {
                                    tile.typego = TakeGO.RockProp;
                                }
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                            else if (randnum == 3)
                            {
                                if (tile.typego == TakeGO.empty && tile.z != 0)
                                {
                                    tile.typego = TakeGO.WeedTall;
                                }
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                            else if (randnum == 4)
                            {
                                if (tile.typego == TakeGO.empty && tile.z != 0)
                                {
                                    tile.typego = TakeGO.Pine;
                                }
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                            else if (randnum == 5)
                            {
                                if (tile.typego == TakeGO.empty && tile.z != 0)
                                {
                                    tile.typego = TakeGO.Oak;
                                }
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                            else if (randnum == 6)
                            {
                                tile.typeVariante = TypeVariante.GrassFL1;
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                            else if (randnum == 7)
                            {
                                tile.typeVariante = TypeVariante.GrassFL2;
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                            else if (randnum == 8)
                            {
                                tile.typeVariante = TypeVariante.GrassRC;
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                            else
                            {
                                tile.PerlinSetType(TypeBlock.Grass);
                            }
                        }
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class ItemUpdate
{
    public ItemData Item;
    public int quanty;
}

[System.Serializable]
public class CityStatus
{
    public int WorldPopularidade = 0;
    public int Agrssive = 0;
    
}

[System.Serializable]
public struct CityVector2
{
    public int x;
    public int z;

    public CityVector2(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}


public enum CityLevel
{
    Camp, Level0, Level1, Level2, Level3, Level4, Level5, Level6, Level7
}

public enum CityType
{
    none, Camp, farm, bigfarm, smalltown, bigtown, portcity, turistcity
}

public enum VilagerVocation
{
    none, Guerreiro, Fazendeiro, açogueiro, lenhador, medico, engenheiro, bibliotecario, padre, guarda, mineiro, marinheiro, Vendedor, ferreiro
    , padeiro, aventureiro, cientista, alquimista
}

public enum EconomicType
{
    kingdom, feudal, villanger, Capitalism, Comunumism
}