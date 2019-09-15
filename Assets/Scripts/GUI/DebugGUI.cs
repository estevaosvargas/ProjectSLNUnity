using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Easy way to add ui for debug,
/// </summary>
public class DebugGUI : MonoBehaviour
{
    public GameObject Root;
    

    void Start()
    {
        Game.DebugGUI = this;
    }

    public static void AddDUI(DebugUiType type, RecDebugUi rec)
    {
        Debug.Log(type.ToString());
    }


    public static void RemoveDUI()
    {

    }
}

public struct RecDebugUi
{
    public float x;
    public float y;
    public float width;
    public float height;
}

public enum DebugUiType
{
    none, Text, Image, TextBackGround
}
