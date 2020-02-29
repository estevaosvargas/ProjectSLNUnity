using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnRender : MonoBehaviour
{
    public GameObject[] DisableEnableList;

    private void Start()
    {
        
    }

    private void OnBecameVisible()
    {
        foreach (var item in DisableEnableList)
        {
            item.SetActive(true);
        }
    }

    private void OnBecameInvisible()
    {
        foreach (var item in DisableEnableList)
        {
            item.SetActive(false);
        }
    }
}
