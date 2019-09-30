using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    public float TimeH = 0.5f;
    public float TimePerDay = 20;
    public Gradient LightColor;
    public bool IsDay = true;
    public float time;
    public Light light;

    public static TimeOfDay Instance;

    void Start()
    {
        Instance = this;

        light = GetComponent<Light>();

        TimeH = DataTime.skytime;
    }

    public Color CurrentSkyTint
    {
        get
        {
            return LightColor.Evaluate(TimeH);
        }
    }

    void Update()
    {
        TimeH += TimePerDay * Time.deltaTime;
        time = TimeH * 24;

        DataTime.skytime = TimeH;

        DataTime.SetTimeData((int)time);

        if (TimeH >= 1)
        {
            TimeH = 0;
            time = 0;
        }

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

        light.color = CurrentSkyTint;
    }
}