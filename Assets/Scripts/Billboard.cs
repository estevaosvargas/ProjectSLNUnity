using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera cameraToLookAt;
    bool IsVisible = false;

    void Start()
    {
        cameraToLookAt = Camera.main;
    }

    void Update()
    {
        if (IsVisible)
        {
            Vector3 v = cameraToLookAt.transform.position - transform.position;
            v.z = 0.0f;
            transform.LookAt(cameraToLookAt.transform.position - v);
        }
    }

    public void OnBecameInvisible()
    {
        IsVisible = false;
    }

    private void OnBecameVisible()
    {
        IsVisible = true;
    }
}
