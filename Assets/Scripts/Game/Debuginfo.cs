using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debuginfo : MonoBehaviour
{

    public Text Position;

    void Start()
    {

    }

    void Update()
    {
        if (Game.GameManager.MyPlayer.MyObject != null)
        {
            Position.text = "X:" + Game.GameManager.MyPlayer.MyObject.transform.position.x + "," + "Y:" + Game.GameManager.MyPlayer.MyObject.transform.position.y;
        }
    }
}