using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBase : MonoBehaviour
{
    public string BuildId;
    public DarckNet.DataVector3 citypoint;

    private void OnDestroy()
    {
        City currentcity = Game.CityManager.GetCity(citypoint.ToUnityVector());
        currentcity.CityBuildings.Remove(BuildId);

        if (currentcity.CityBuildings.Count <= 0)
        {
            currentcity.IsLoaded = false;
            Debug.Log("This City : " + citypoint.ToString() + "Is not more loaded!");
        }

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
    public DarckNet.DataVector3 citypoint;

    public CityBaseSerialization(string _BuildId, DarckNet.DataVector3 _citypoint)
    {
        BuildId = _BuildId;
        citypoint = _citypoint;
    }

    public static CityBaseSerialization[] CityBase(CityBase[] cityBase)
    {
        List<CityBaseSerialization> cityserial = new List<CityBaseSerialization>();
        if (cityBase.Length > 0)
        {
            foreach (var serial in cityBase)
            {
                cityserial.Add(new CityBaseSerialization(serial.BuildId, serial.citypoint));
            }
        }
        else
        {
            
        }

        return cityserial.ToArray();
    }
}
