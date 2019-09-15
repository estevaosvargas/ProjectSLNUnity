using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetWork : MonoBehaviour
{
    public NetWorkView Net;

    public List<MonoBehaviour> ScriptToDisable = new List<MonoBehaviour>();
    public List<GameObject> ObjectToDisable = new List<GameObject>();

    void Start()
    {
        if (Net.isMine)
        {
            foreach (var item in ScriptToDisable)
            {
                item.enabled = true;
            }
        }
        else
        {
            foreach (var item in ScriptToDisable)
            {
                item.enabled = false;
            }
        }
    }

    [RPC]
    void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
    }
}