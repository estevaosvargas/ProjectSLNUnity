using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConvercetion : MonoBehaviour
{
    public CitzenCredential CurrentNPC;

    private void Awake()
    {
        Game.NPCTALK = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void InsertText(string text)
    {
        string[] textsplite = text.Split(" "[0]);

        bool asking = false;
        bool afirmative = false;

        foreach (var palavra in textsplite)
        {
            if (palavra.EndsWith("?"))
            {
                asking = true;
            }

            if (palavra.EndsWith("!"))
            {
                afirmative = true;
            }
        }
    }
}
