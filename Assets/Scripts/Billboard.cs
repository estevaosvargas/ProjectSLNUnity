using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera cameraToLookAt;
    bool IsVisible = false;
    public float smoth = 2;

    void Start()
    {
        cameraToLookAt = Camera.main;
    }

    void Update()
    {
        if (IsVisible)
        {
            Vector3 v = cameraToLookAt.transform.position - transform.position;

            //v.x = v.z = 0.0f;
            v.y = v.z = 0.0f;

            transform.LookAt(cameraToLookAt.transform.position - v * smoth * Time.deltaTime);

            if (v.x > 3.6f)
            {
                v.x = 3.6f;
            }
            else if (v.x < -3.6f)
            {
                v.x = -3.6f;
            }

            if (v.y > 15)
            {
                v.y = 15;
            }
            else if (v.y < -15)
            {
                v.y = -15;
            }
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
