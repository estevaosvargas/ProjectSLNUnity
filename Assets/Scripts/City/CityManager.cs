using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CityManager : MonoBehaviour
{
    public Dictionary<DataVector3, City> CityList = new Dictionary<DataVector3, City>();

    private void Awake()
    {
        Game.CityManager = this;
    }

    public void SetUpCity(DataVector3 city)
    {
        if (!CityList.ContainsKey(city))
        {
            if (!LoadCity(city))
            {
                Debug.Log("Loaded this city: " + city);
                CityList.Add(city, new City("City Teste", city.GetHashCode(), 9999, (EconomicType)UnityEngine.Random.Range(0, 5), city));
                SaveCity(city);
            }
        }
    }

    public City GetCity(DataVector3 city)
    {
        if (CityList.TryGetValue(city, out City curretncity))
        {
            return curretncity;
        }
        else
        {
            Debug.Log("Sorry This City Don't Exits!");
            return null;
        }
    }

    public CitzenCredential GetEntity(DataVector3 city, string CitzendId)
    {
        City Currentcity = GetCity(city);

        if (Currentcity != null)
        {
            if (Currentcity.LivingEntity.TryGetValue(CitzendId, out CitzenCredential citzen))
            {
                return citzen;
            }
        }

        return null;
    }

    public CitzenCredential GetEntity(City city, string CitzendId)
    {
        if (city.LivingEntity.TryGetValue(CitzendId, out CitzenCredential citzen))
        {
            return citzen;
        }

        return null;
    }

    public CitzenCredential GetOutSideEntity(DataVector3 city, DataVector3 pos)
    {
        City CurrentCity = GetCity(city);
        if (CurrentCity.OutSide.TryGetValue(pos, out CitzenCredential entity))
        {
            return entity;
        }
        return null;
    }

    public GameObject SpawnNewEntity(CitzenCredential citzenCredential, Vector3 spawnPosition)
    {
        GameObject obj = DarckNet.Network.Instantiate(Game.SpriteManager.GetPrefabOnRecources("Prefabs/AI/Villager"), spawnPosition, Quaternion.identity, Game.WorldGenerator.World_ID);

        Vilanger villanger = obj.GetComponent<Vilanger>();

        villanger.ID = citzenCredential.Citzen_Id;
        villanger.CurrentCity = citzenCredential.currentcity;
        villanger.PrefabName = "Villager";
        return obj;
    }

    public void RemoveEntityFromWorld(DataVector3 currentCity, string iD, GameObject gameObject)
    {
        DarckNet.Network.Destroy(gameObject);
    }

    #region LoadSave
    public bool LoadCity(DataVector3 city)
    {
        CitySave save = SaveWorld.LoadCity(city.GetHashCode().ToString());

        if (save != null)
        {
            if (!CityList.ContainsKey(save.citypoint))
            {
                CityList.Add(save.citypoint, new City(save));
                return true;
            }
        }

        return false;
    }

    public void SaveCity(DataVector3 city)
    {
        SaveWorld.SaveCity(new CitySave(GetCity(city)), city.GetHashCode().ToString());
    }
    #endregion
}

[System.Serializable]
public class CityDataBase
{
    public static void UpdateEntityTask(NPCTASK nPCTASK, DataVector3 city, string citzen_Id)
    {
        CitzenCredential citzen = Game.CityManager.GetEntity(city, citzen_Id);

        if (citzen != null)
        {
            citzen.CurrentTask = nPCTASK;
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
    public string CityName;
    public int CityId;
    public int Population;
    public int MaxPopulation;
    public EconomicType economicType;
    public CityStatus cityStatus;

    public List<Placer> BuildingType = new List<Placer>();

    public Dictionary<string, CityBaseSerialization> CityBuildings = new Dictionary<string, CityBaseSerialization>();
    public Dictionary<string, CitzenCredential> LivingEntity = new Dictionary<string, CitzenCredential>();

    public Dictionary<DataVector3, CitzenCredential> OutSide = new Dictionary<DataVector3, CitzenCredential>();

    public DataVector3 citypoint;
    public bool IsLoaded;

    public City(string city_Name, int city_ID, int max_polupation, EconomicType economic_Type, DataVector3 city_point)
    {
        CityName = city_Name;
        CityId = city_ID;
        MaxPopulation = max_polupation;
        economicType = economic_Type;
        citypoint = city_point;
    }

    public City(CitySave Save)
    {
        CityName = Save.CityName;
        CityId = Save.CityId;
        Population = Save.Population;
        MaxPopulation = Save.MaxPopulation;
        economicType = Save.economicType;
        cityStatus = Save.cityStatus;
        citypoint = Save.citypoint;

        foreach (var build in Save.buildbase)
        {
            CityBuildings.Add(build.BuildId, build);
        }

        foreach (var entity in Save.LivingEntity)
        {
            LivingEntity.Add(entity.Citzen_Id, entity);

            if (entity.IsOutSide)
            {
                OutSide.Add(entity.WorldPostion, entity);
            }
        }
    }
}

[System.Serializable]
public class CitySave
{
    public string CityName;
    public int CityId;
    public int Population;
    public int MaxPopulation;
    public EconomicType economicType;
    public CityStatus cityStatus;

    public CitzenCredential[] LivingEntity;
    public CityBaseSerialization[] buildbase;

    public DataVector3 citypoint;

    public CitySave(City city)
    {
        CityName = city.CityName;
        CityId = city.CityId;
        MaxPopulation = city.Population;
        economicType = city.economicType;

        citypoint = city.citypoint;

        LivingEntity = city.LivingEntity.Values.ToArray();
        buildbase = city.CityBuildings.Values.ToArray();
    }
}

[System.Serializable]
public class CitzenCredential
{
    public string Name;
    public string Citzen_Id;
    public string LivingHouseId;
    public int Age;
    public SexualType Sexual;
    public FamilyPostiton Family_Postiton;
    public VilagerVocation Vocation;

    public int Agility;//this is Reflexes of the char
    public int Tolerance;//This is for the rage level of char
    public int IQ;//inteligence of the cahr
    public int Luck;//the luck of the char
    public int Charisma;//social factor of char
    public DataVector3 currentcity;

    public NPCTASK CurrentTask = new NPCTASK(NPCTasks.none, DataVector3.zero);

    public bool IsOutSide;//Check if the entity is spawned on world
    public DataVector3 WorldPostion;//Position on world

    public CitzenCredential(string _Name, string _Citzen_Id, string _LivingHouseId, DataVector3 _currentcity, int _Age, SexualType _Sexual, FamilyPostiton _Family_Postiton, VilagerVocation _Vocation, int _Agility, int _Tolerance, int _IQ, int _Luck, int _Charisma, NPCTASK nPCTASK)
    {
        Name = _Name;
        Citzen_Id = _Citzen_Id;
        Age = _Age;
        Sexual = _Sexual;
        Agility = _Agility;
        Tolerance = _Tolerance;
        IQ = _IQ;
        Luck = _Luck;
        Charisma = _Charisma;
        Family_Postiton = _Family_Postiton;
        LivingHouseId = _LivingHouseId;
        Vocation = _Vocation;
        currentcity = _currentcity;
        IsOutSide = false;
        WorldPostion = DataVector3.zero;
        CurrentTask = nPCTASK;
    }

    public void UpdateWorldValues(bool isoutside, DataVector3 _WorldPostion)
    {
        IsOutSide = isoutside;
        WorldPostion = _WorldPostion;
    }

    public void UpdateWorldPosition(DataVector3 _WorldPostion)
    {
        WorldPostion = _WorldPostion;
    }
}

[System.Serializable]
public struct NPCTASK
{
    public NPCTasks this_task;
    public DataVector3 TaskPosition;
    

    public NPCTASK(NPCTasks tasks, DataVector3 pos)
    {
        this_task = tasks;
        TaskPosition = pos;
    }
}