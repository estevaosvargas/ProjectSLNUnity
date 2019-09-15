using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObscure : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Workin");
        }
    }
}
