using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    [Header("WaterColor")]
    public UnityStandardAssets.Water.WaterBase Water;
    public Gradient _BaseColor;
    public Gradient _ReflectionColor;
    public Gradient _SpecularColor;

    [Header("TimeSystem")]
    public Gradient AmbientColor;
    public Gradient FogColor;
    public bool IsDay = true;
    public NetWorkView Net;
    private float LastUpdate;

    [Range(0,24)]
    public float timeOfDay = 12;
    public float secondsPerMinute = 60;
    public float secondsPerHour;
    public float secondsPerDay;
    public float timeMultiplier = 1;

    void Awake()
    {
        Game.TimeOfDay = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("SERVER: Sun Started");
        timeOfDay = DataTime.skytime;
    }

    public Color CurrentAmbientColor
    {
        get
        {
            return AmbientColor.Evaluate(timeOfDay / 24);
        }
    }

    public Color CurrentFogColor
    {
        get
        {
            return FogColor.Evaluate(timeOfDay / 24);
        }
    }

    public Color Get_BaseColor
    {
        get
        {
            return _BaseColor.Evaluate(timeOfDay / 24);
        }
    }

    public Color Get_ReflectionColor
    {
        get
        {
            return _ReflectionColor.Evaluate(timeOfDay / 24);
        }
    }

    public Color Get_SpecularColor
    {
        get
        {
            return _SpecularColor.Evaluate(timeOfDay / 24);
        }
    }

    public void LastUpdateTime()
    {
        Net.RPC("RPC_Updatetime", DarckNet.RPCMode.AllNoOwner, timeOfDay);
        //Debug.Log("SERVER: SkyUpdate.");
    }

    void Update()
    {
        if (DarckNet.Network.IsServer || Game.GameManager.SinglePlayer)
        {
            DataTime.skytime = timeOfDay;

            timeOfDay += (Time.deltaTime / secondsPerDay) * timeMultiplier;
 
            if (timeOfDay >= 24) {
                timeOfDay = 0;
            }

            if (timeOfDay != LastUpdate)
            {
                LastUpdateTime();
            }
            LastUpdate = timeOfDay;
        }

        if (DarckNet.Network.IsClient || Game.GameManager.SinglePlayer)
        {
            //Use for sun rotation, but for now i gone use ambient color
            //transform.localRotation = Quaternion.Euler(((timeOfDay / 24) * 360f) - 90, 90, 0);

            RenderSettings.ambientLight = CurrentAmbientColor;
            RenderSettings.fogColor = CurrentFogColor;
            Water.sharedMaterial.SetColor("_BaseColor", Get_BaseColor);
            Water.sharedMaterial.SetColor("_ReflectionColor", Get_ReflectionColor);
            Water.sharedMaterial.SetColor("_SpecularColor", Get_SpecularColor);
        }

        DataTime.SetTimeData((int)timeOfDay);

        if (DataTime.Hora >= 6 && DataTime.Hora <= 18)
        {
            //Dia
            IsDay = true;
        }
        else if (DataTime.Hora >= 18)
        {
            //Noite
            IsDay = false;
        }
        else if (DataTime.Hora <= 6)
        {
            //Noite
            IsDay = false;
        }
    }

    [RPC]
    void RPC_Updatetime(float time)
    {
        timeOfDay = time;
    }
}