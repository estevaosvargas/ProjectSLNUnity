using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    public float TimeH = 0.5f;
    public float TimePerDay = 20;
    public Gradient LightColor;
    public Gradient FogColor;
    public bool IsDay = true;
    public float time;
    public NetWorkView Net;
    private float LastUpdate;

    void Awake()
    {
        Game.TimeOfDay = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("SERVER: Sun Started");
        TimeH = DataTime.skytime;
    }

    public Color CurrentSkyTint
    {
        get
        {
            return LightColor.Evaluate(TimeH);
        }
    }

    public Color CurrentFogColor
    {
        get
        {
            return FogColor.Evaluate(TimeH);
        }
    }

    public void LastUpdateTime()
    {
        Net.RPC("RPC_Updatetime", DarckNet.RPCMode.AllNoOwner, TimeH);
        //Debug.Log("SERVER: SkyUpdate.");
    }

    void Update()
    {
        if (DarckNet.Network.IsServer || Game.GameManager.SinglePlayer)
        {
            TimeH += TimePerDay * Time.deltaTime;
            time = TimeH * 24;

            DataTime.skytime = TimeH;

            if (TimeH >= 1)
            {
                TimeH = 0;
                time = 0;
            }

            if (time != LastUpdate)
            {
                LastUpdateTime();
            }
            LastUpdate = time;
        }

        if (DarckNet.Network.IsClient || Game.GameManager.SinglePlayer)
        {
            time = TimeH * 24;

            RenderSettings.ambientLight = CurrentSkyTint;
            RenderSettings.fogColor = CurrentFogColor;
        }

        DataTime.SetTimeData((int)time);

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
        TimeH = time;
    }
}