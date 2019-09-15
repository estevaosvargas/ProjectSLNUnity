using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRenderTeste : MonoBehaviour
{
    public void OnWillRenderObject()
    {
        Debug.Log("OnWillRenderObject!");
    }

    public void OnBecameInvisible()
    {
        Debug.Log("Objecto nsao esta Visivel!");
    }

    private void OnBecameVisible()
    {
        Debug.Log("Objecto Esta Visivel!");
    }
}
