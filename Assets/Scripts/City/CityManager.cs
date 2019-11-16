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
            CurrentCitysLoaded.Add(point, new City(("Testing City " + point).GetHashCode().ToString(), ("Testing City " + point).GetHashCode().ToString(), 150, (EconomicType)UnityEngine.Random.Range(0, 5), new DarckNet.DataVector3(point), new DarckNet.DataVector3(CityHall)));
            Save();
        }
    }

    public void Save()
    {
        List<CitySave> citySaves = new List<CitySave>();
        foreach (var itemcity in CurrentCitysLoaded.Values.ToArray())
        {
            citySaves.Add(new CitySave(itemcity.CityName, itemcity.CityId, itemcity.MaxPopulation, itemcity.economicType, itemcity.citypoint, itemcity.hallpos, itemcity.havehall, itemcity.haveblack));
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
                CurrentCitysLoaded.Add(new Vector3(itemcity.citypoint.x, itemcity.citypoint.y, 0), new City(itemcity.CityName, itemcity.CityId, itemcity.MaxPopulation, itemcity.economicType, itemcity.citypoint, itemcity.hallpos, itemcity.havehall, itemcity.haveblack));
                Debug.Log(itemcity.CityId);
            }
        }
    }

    public void WantInteract(Vector3 city, Vector3 interaction, NPCTasks task)
    {
        City current_city = GetCity(city);
        CityBase build = null;

        if (current_city.CityBuildings.TryGetValue(interaction, out build))
        {
            switch (task)
            {
                case NPCTasks.GoGetTask:
                    //build.GetComponent<CityHall>();
                    break;
            }
        }
        else
        {
            Debug.LogError("Uhmm somthing is not right, we dont found your location...");
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
    public Dictionary<Vector3, CityBase> CityBuildings = new Dictionary<Vector3, CityBase>();
    public DarckNet.DataVector3 citypoint;
    public DarckNet.DataVector3 hallpos;
    public DarckNet.DataVector3 blackpos;
    public bool havehall;
    public bool haveblack;

    [System.NonSerialized]
    public List<Vilanger> Vilangers = new List<Vilanger>();

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

    public DarckNet.DataVector3 citypoint;
    public DarckNet.DataVector3 hallpos;

    public bool havehall;
    public bool haveblack;

    public CitySave(string city_Name, string city_ID, int max_polupation, EconomicType economic_Type, DarckNet.DataVector3 city_point, DarckNet.DataVector3 hall_pos, bool have_hall, bool have_black)
    {
        CityName = city_Name;
        CityId = city_ID;
        MaxPopulation = max_polupation;
        economicType = economic_Type;

        citypoint = city_point;

        hallpos = hall_pos;

        havehall = have_hall;
        haveblack = have_black;
    }
}

[Serializable]
public struct VillangerStaus
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
    internal Vector3 currentcity;

    public VillangerStaus(string _Name, string _Citzen_Id, string _LivingHouseId, Vector3 _currentcity, int _Age, SexualType _Sexual, FamilyPostiton _Family_Postiton, VilagerVocation _Vocation, int _Agility, int _Tolerance, int _IQ, int _Luck, int _Charisma)
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

public class TaskBase
{
    
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
    ,padeiro, aventureiro, cientista, alquimista
}

public enum EconomicType
{
    kingdom, feudal, villanger, Capitalism, Comunumism
}

public enum SexualType
{
    Others, Man, Woman
}

public enum FirstCityName
{
    Dragon, Kilkenny, Eelry, Beckinsdale, Leefside, Azmar, Braedwardith, Ramshorn, Forstford, Aylesbury, Mountmend, Stawford
}

public enum SecondCityName
{
    Vally, Orrinshire, Wimborne, Panshaw, Holbeck, Hythe, Cromer, Gormsey, Wingston, Hempholme, Jedburgh, RedHawk, 
}

public enum FamilyPostiton
{
    none,Father, Mother, Son, GrandFather, GrandMother
}

public enum MaleHumanNames//14
{
    DarinHailey, GermanHarrison, WallyLee, RistonTownsend, LatimerDavenport,
    DeonteThorp,
    ThoraldNetley,
    JarvNetley,
    KristopherClifford,
    SiddelHuxley,
    TreDalton,
    FaraltAllerton,
    FidelisSwet,
    DacianSwett,
}

public enum FemaleHumanNames//20
{
    BerdineGale,
    KatarinaRylan,
    TatBrooks,
    JoyanneCamden,
    AyanaRodney,
    SavannaOakley,
    AdileneRoscoe,
    HerthaStratford,
    ChaunteHuckabee,
    RudelleHarrison,
    HarrietteWard,
    GesaHome,
    AmiteeBirkenhead,
    JulianneDenholm,
    GwendolynLincoln,
    CecilleFarnham,
    TanyaOldham,
    AshtynPaddle,
    FloridaAlden,
    HanneAppleton
}