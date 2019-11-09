using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CityManager : MonoBehaviour
{
    public Dictionary<Vector3, City> CurrentCitysLoaded = new Dictionary<Vector3, City>();
    
    void Awake()
    {
        Game.CityManager = this;
    }

    void Start()
    {
        //ComputCity();
    }

    void Update()
    {

    }

    public void AddCity(Vector3 point, Vector3 CityHall, bool havehall)
    {
        if (!CurrentCitysLoaded.ContainsKey(point))
        {
            CurrentCitysLoaded.Add(point, new City(("Testing City " + point).GetHashCode().ToString(), ("Testing City " + point).GetHashCode().ToString(), 150, EconomicType.kingdom, point, CityHall, havehall));
            Save();
        }
    }

    public void Save()
    {
        List<CitySave> citySaves = new List<CitySave>();
        foreach (var itemcity in CurrentCitysLoaded.Values.ToArray())
        {
            citySaves.Add(new CitySave(itemcity.CityName, itemcity.CityId, itemcity.MaxPopulation, itemcity.economicType, itemcity.citypoint, itemcity.hallpos, itemcity.havehall));
        }

        SaveWorld.SaveCity(citySaves.ToArray(), "citys");
    }

    public City GetCity(Vector3 cityPoint)
    {
        if (CurrentCitysLoaded.ContainsKey(cityPoint))
        {
            return CurrentCitysLoaded[cityPoint];
        }

        return null;
    }

    public void Load()
    {
        CitySave[] cityarray = SaveWorld.LoadCity("citys");

        if (cityarray.Length > 0)
        {
            foreach (var itemcity in cityarray)
            {
                CurrentCitysLoaded.Add(new Vector3(itemcity.citypointX, itemcity.citypointY, 0), new City(itemcity.CityName, itemcity.CityId, itemcity.MaxPopulation, itemcity.economicType, new Vector3(itemcity.citypointX, itemcity.citypointY, 0), new Vector3(itemcity.hallposX, itemcity.hallposY, 0), itemcity.havehall));
                Debug.Log(itemcity.CityId);
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
public class City
{
    public string CityName = "";
    public string CityId = "";
    public int Population = 0;
    public int MaxPopulation = 10;
    public EconomicType economicType;
    public CityStatus cityStatus;
    public Vector3 citypoint;
    public Vector3 hallpos;
    public bool havehall;

    [System.NonSerialized]
    public List<Vilanger> Vilangers = new List<Vilanger>();

    public City(string city_Name, string city_ID, int max_polupation, EconomicType economic_Type, Vector3 city_point, Vector3 hall_pos, bool have_hall)
    {
        CityName = city_Name;
        CityId = city_ID;
        MaxPopulation = max_polupation;
        economicType = economic_Type;
        citypoint = city_point;
        hallpos = hall_pos;
        havehall = have_hall;
    }
}

[System.Serializable]
public class CitySave
{
    public string CityName = "";
    public string CityId = "";
    public int Population = 0;
    public int MaxPopulation = 10;
    public EconomicType economicType;
    public CityStatus cityStatus;

    public int citypointX;
    public int citypointY;

    public int hallposX;
    public int hallposY;

    public bool havehall;

    public CitySave(string city_Name, string city_ID, int max_polupation, EconomicType economic_Type, Vector3 city_point, Vector3 hall_pos, bool have_hall)
    {
        CityName = city_Name;
        CityId = city_ID;
        MaxPopulation = max_polupation;
        economicType = economic_Type;

        citypointX = (int)city_point.x;
        citypointY = (int)city_point.y;

        hallposX = (int)hall_pos.x;
        hallposY = (int)hall_pos.y;

        havehall = have_hall;
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