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
    public CityLevel Level;

    public CityStatus cityStatus;

    public List<Vilanger> Vilangers = new List<Vilanger>();


    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.y;
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
    kigsdom, feudal, villanger, Capitalism
}