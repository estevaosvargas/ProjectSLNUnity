using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimation : MonoBehaviour
{
    public SpriteRenderer Render;
    

    public CharColorStruc SkinColor;
    public CharColorStruc EyesColor;

    void Start()
    {
        Render = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
       
    }

    public void ProcessSprites()
    {

    }

    public void SetSpriteAnim(int index)
    {
        Debug.Log("AnimIndex : " + index);
        Render.sprite = Game.GameManager.charcustom.CharSprites[index];
    }
}
