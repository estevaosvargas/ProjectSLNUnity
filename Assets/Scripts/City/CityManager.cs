using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarckNet;
using UnityEngine;

[System.Serializable]
public class CityManager : MonoBehaviour
{
    public Dictionary<Vector3, City> CurrentCitysLoaded = new Dictionary<Vector3, City>();
    public List<Entity> EntitysSpawned = new List<Entity>();

    void Awake()
    {
        Game.CityManager = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AddCity(Vector3 point, Vector3 CityHall, bool havehall)
    {
        if (!CurrentCitysLoaded.ContainsKey(point))
        {
            CurrentCitysLoaded.Add(point, new City(("Testing City " + point).GetHashCode().ToString(), ("Testing City " + point).GetHashCode().ToString(), 150, (EconomicType)UnityEngine.Random.Range(0, 5), new DarckNet.DataVector3(point), new DarckNet.DataVector3(CityHall)));
            Save();
        }
    }

    public City GetCity(Vector3 cityPoint)
    {
        if (CurrentCitysLoaded.ContainsKey(cityPoint))
        {
            return CurrentCitysLoaded[cityPoint];
        }

        return null;
    }

    public CityBase GetBuild(Vector3 city, string buildid)
    {
        City current_city = GetCity(city);
        CityBase build = null;

        if (current_city.CityBuildings.TryGetValue(buildid, out build))
        {
            return build;
        }
        else
        {
            return null;
        }
    }

    public void WantInteract(Vector3 city, NPCTASK tasks, Vilanger entity)
    {
        City current_city = GetCity(city);
        CityBase build = null;
        string buildid = ((int)city.x + (int)city.y * (int)tasks.TaskPosition.x + (int)tasks.TaskPosition.z).ToString();

        if (current_city.CityBuildings.TryGetValue(buildid, out build))
        {
            switch (tasks.this_task)
            {
                case NPCTasks.GoGetTask:
                    build.WantInteract(entity);
                    break;
                case NPCTasks.GoHome:
                    build.WantInteract(entity);
                    break;
                case NPCTasks.EnterInBuild:
                    build.WantInteract(entity);
                    break;
            }
        }
        else
        {
            Debug.LogError("Uhmm somthing is not right, we dont found your location... : " + tasks.TaskPosition.ToString());
        }
    }

    public void UpdatePositionStaus(Vector3 position, Vector3 city, string citzenid)
    {
        City currentcity = GetCity(city);
        currentcity.LivingEntity[citzenid].WorldPostion = new DarckNet.DataVector3(position);
    }

    public void SpawnNewEntity(CitzenCredential status, Vector3 Position)
    {
        GameObject obj = DarckNet.Network.Instantiate(Game.SpriteManager.GetPrefabOnRecources("Prefabs/Villager/Villager"), Position, Quaternion.identity, Game.WorldGenerator.World_ID);
        EntitysSpawned.Add(obj.GetComponent<Vilanger>());
        obj.GetComponent<Vilanger>().Status = status;

        status.IsOutSide = true;
        status.WorldPostion = new DarckNet.DataVector3(Position);
       //Save();
    }

    /// <summary>
    /// Remove a Entity from world.
    /// </summary>
    /// <param name="city"></param>
    /// <param name="citzen_Id"></param>
    /// <param name="obj_entity"></param>
    /// <returns></returns>
    public bool RemoveEntityFromWorld(Vector3 city, string citzen_Id, GameObject obj_entity)
    {
        City currentcity = GetCity(city);

        if (currentcity.LivingEntity.TryGetValue(citzen_Id, out CitzenCredential entity))
        {
            entity.IsOutSide = false;
            DarckNet.Network.Destroy(obj_entity);
            return true;
        }
        else
        {
            Debug.LogError("Sorry this entity isnt in world!");
            return false;
        }
    }

    public void SetUpCityTile(Tile tile, Chunk currentchunk)
    {
        City Currentcitty = Game.CityManager.CurrentCitysLoaded[new Vector3((int)tile.CityPoint.x, (int)tile.CityPoint.y, 0)];
        System.Random rand = new System.Random(Game.WorldGenerator.Seed + tile.x * tile.z + ((int)currentchunk.transform.position.x + (int)currentchunk.transform.position.z));

        if (!Currentcitty.havehall)///CityHall
        {
            tile.PLACER_DATA = Placer.CityHall;
            Currentcitty.hallpos = new DarckNet.DataVector3(tile.x, tile.y, tile.z);
            Currentcitty.havehall = true;
        }
        else if (!Currentcitty.haveblack)
        {
            tile.PLACER_DATA = Placer.BlackSmith;
            Currentcitty.blackpos = new DarckNet.DataVector3(tile.x, tile.y, tile.z);
            Currentcitty.haveblack = true;
        }
    }

    #region Save/Load
    public void Save()
    {
        if (DarckNet.Network.IsServer || Game.GameManager.SinglePlayer)
        {
            List<CitySave> citySaves = new List<CitySave>();
            foreach (var itemcity in CurrentCitysLoaded.Values.ToArray())
            {
                citySaves.Add(new CitySave(itemcity.CityName, itemcity.CityId, itemcity.MaxPopulation, itemcity.economicType, itemcity.citypoint, itemcity.LivingEntity.Values.ToArray(), CityBaseSerialization.CityBase(itemcity.CityBuildings.Values.ToArray())));
            }

            SaveWorld.SaveCity(citySaves.ToArray(), "citys");
        }
    }

    public void Load()
    {
        CitySave[] cityarray = SaveWorld.LoadCity("citys");

        if (cityarray.Length > 0)
        {
            foreach (var itemcity in cityarray)
            {
                CurrentCitysLoaded.Add(new Vector3(itemcity.citypoint.x, itemcity.citypoint.y, 0), new City(itemcity.CityName, itemcity.CityId, itemcity.MaxPopulation, itemcity.economicType, itemcity.citypoint, itemcity.LivingEntity, itemcity.buildbase));
                Debug.Log(itemcity.CityId);
            }
        }
    }
    #endregion
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

    public Dictionary<string, CityBase> CityBuildings = new Dictionary<string, CityBase>();
    public Dictionary<string, CitzenCredential> LivingEntity = new Dictionary<string, CitzenCredential>();

    public DarckNet.DataVector3 citypoint;
    public DarckNet.DataVector3 hallpos;
    public DarckNet.DataVector3 blackpos;
    public bool havehall;
    public bool haveblack;

    public City(string city_Name, string city_ID, int max_polupation, EconomicType economic_Type, DarckNet.DataVector3 city_point, DarckNet.DataVector3 hall_pos, bool _havehall, bool _haveblack)
    {
        CityName = city_Name;
        CityId = city_ID;
        MaxPopulation = max_polupation;
        economicType = economic_Type;
        citypoint = city_point;
        hallpos = hall_pos;
        havehall = _havehall;
        haveblack = _haveblack;
    }

    public City(string city_Name, string city_ID, int max_polupation, EconomicType economic_Type, DarckNet.DataVector3 city_point, DarckNet.DataVector3 hall_pos)
    {
        CityName = city_Name;
        CityId = city_ID;
        MaxPopulation = max_polupation;
        economicType = economic_Type;
        citypoint = city_point;
        hallpos = hall_pos;
    }
    /// <summary>
    /// Used for loading save files
    /// </summary>
    /// <param name="city_Name"></param>
    /// <param name="city_ID"></param>
    /// <param name="max_polupation"></param>
    /// <param name="economic_Type"></param>
    /// <param name="city_point"></param>
    /// <param name="livingentitys"></param>
    public City(string city_Name, string city_ID, int max_polupation, EconomicType economic_Type, DarckNet.DataVector3 city_point, CitzenCredential[] livingentitys, CityBaseSerialization[] _citybases)
    {
        CityName = city_Name;
        CityId = city_ID;
        MaxPopulation = max_polupation;
        economicType = economic_Type;
        citypoint = city_point;

        foreach (var Citzen in livingentitys)
        {
            LivingEntity.Add(Citzen.Citzen_Id, Citzen);
        }

        foreach (var build in _citybases)
        {
            CityBuildings.Add(build.BuildId, null);
        }
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

    public CitzenCredential[] LivingEntity;
    public CityBaseSerialization[] buildbase;

    public DarckNet.DataVector3 citypoint;
    public DarckNet.DataVector3 hallpos;

    public CitySave(string city_Name, string city_ID, int max_polupation, EconomicType economic_Type, DarckNet.DataVector3 city_point, CitzenCredential[] _LivingEntity, CityBaseSerialization[] _buildbase)
    {
        CityName = city_Name;
        CityId = city_ID;
        MaxPopulation = max_polupation;
        economicType = economic_Type;

        citypoint = city_point;

        LivingEntity = _LivingEntity;
        buildbase = _buildbase;
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
    public DarckNet.DataVector3 currentcity;

    public bool IsOutSide;//Check if the entity is spawned on world
    public DarckNet.DataVector3 WorldPostion;//Position on world

    public CitzenCredential(string _Name, string _Citzen_Id, string _LivingHouseId, DarckNet.DataVector3 _currentcity, int _Age, SexualType _Sexual, FamilyPostiton _Family_Postiton, VilagerVocation _Vocation, int _Agility, int _Tolerance, int _IQ, int _Luck, int _Charisma)
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
        WorldPostion = DarckNet.DataVector3.zero;
    }

    public void UpdateWorldValues(bool isoutside, DarckNet.DataVector3 _WorldPostion)
    {
        IsOutSide = isoutside;
        WorldPostion = _WorldPostion;
    }

    public void UpdateWorldPosition(DarckNet.DataVector3 _WorldPostion)
    {
        WorldPostion = _WorldPostion;
    }
}

public struct NPCTASK
{
    public NPCTasks this_task;
    public Vector3 TaskPosition;
    

    public NPCTASK(NPCTasks tasks, Vector3 pos)
    {
        this_task = tasks;
        TaskPosition = pos;
    }
}