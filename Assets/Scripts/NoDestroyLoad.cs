using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDestroyLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
