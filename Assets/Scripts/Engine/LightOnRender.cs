using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnRender : MonoBehaviour
{
    private Light _Light;

    private void Start()
    {
        gameObject.AddComponent<MeshRenderer>();
        _Light = GetComponent<Light>();
    }

    private void OnBecameVisible()
    {
        _Light.enabled = true;
    }

    private void OnBecameInvisible()
    {
        _Light.enabled = false;
    }
}
