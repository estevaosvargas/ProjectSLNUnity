using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityBase : MonoBehaviour
{
    public string BuildId;
    public DataVector3 citypoint;
    public DataVector3 BuildPosition;
    public Placer BuildType = Placer.empty;

    private void OnDestroy()
    {
        City currentcity = Game.CityManager.GetCity(citypoint.ToUnityVector());
        currentcity.CityBuildings[BuildId].Temp_objc = null;

        Game.CityManager.UnloadCity(currentcity, BuildId);

        Debug.Log("This Build : " + BuildId + " Are Removed form city build list");
    }

    public virtual void WantInteract(Entity entity)
    {

    }

    public virtual void NewBuild()
    {
        
    }

    public virtual void LoadBuild()
    {

    }
}

[System.Serializable]
public class CityBaseSerialization
{
    public string BuildId;
    public DataVector3 citypoint;
    public DataVector3 BuildPosition;
    public Placer BuildType = Placer.empty;

    [NonSerialized]
    public CityBase Temp_objc;

    public CityBaseSerialization(string _BuildId, DataVector3 _citypoint, DataVector3 _BuildPosition, Placer _BuildType)
    {
        BuildId = _BuildId;
        citypoint = _citypoint;
        BuildPosition = _BuildPosition;
        BuildType = _BuildType;
    }

    public static CityBaseSerialization[] CityBase(CityBase[] cityBase)
    {
        List<CityBaseSerialization> cityserial = new List<CityBaseSerialization>();
        if (cityBase.Length > 0)
        {
            foreach (var serial in cityBase)
            {
                cityserial.Add(new CityBaseSerialization(serial.BuildId, serial.citypoint, new DataVector3(serial.transform.position), serial.BuildType));
            }
        }

        return cityserial.ToArray();
    }
}
