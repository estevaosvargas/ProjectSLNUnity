using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAnimations : MonoBehaviour
{
    public static List<TileStrucAnimation> TILESANIMEATION = new List<TileStrucAnimation>();
    public float WATERFLOWS_SPEED = 0.5f;
    public bool DEBUGANIMATION = false;

    private int WaterAnimState;

    private void Awake()
    {
        Game.TileAnimations = this;
    }

    IEnumerator UpdateAnimation()
    {
        while (DEBUGANIMATION)
        {
            foreach (var tile in TILESANIMEATION)
            {
                WaterAnimState++;

                if (WaterAnimState >= tile.Sprite.Length)
                {
                    WaterAnimState = 0;
                }

                tile.SPRITERENDER.sprite = tile.Sprite[WaterAnimState];
            }
            yield return new WaitForSeconds(WATERFLOWS_SPEED);
        }
    }

    public void StartTileAnimation()
    {
        DEBUGANIMATION = true;
        StartCoroutine(UpdateAnimation());
    }

    public void StopTileAnimation()
    {
        DEBUGANIMATION = false;
        StopCoroutine(UpdateAnimation());
    }
}