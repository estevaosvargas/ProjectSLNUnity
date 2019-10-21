using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TileObj : MonoBehaviour
{
    public List<TransData> Tiles = new List<TransData>();
    public List<GameObject> obj = new List<GameObject>();
    public GameObject TileObject;
    public bool ISVISIBLE = false;
    public bool TILEANIMATED = false;
    public Animator TILEANIMATOR;
    public TileStrucAnimation TILEANIMATIONDATA;

    public void SetUp()
    {
        TILEANIMATIONDATA = new TileStrucAnimation(GetComponent<SpriteRenderer>(), new Sprite[2] { Game.SpriteManager.GetSprite("Water_"), Game.SpriteManager.GetSprite("Water_01") });

        for (int i = 0; i < Tiles.Count; i++)
        {
            GameObject TileGo = null;

            TileGo = GameObject.Instantiate(TileObject, new Vector3(0, 0, 0), Quaternion.identity);
            TileGo.SetActive(true);
            SpriteRenderer Render = TileGo.GetComponent<SpriteRenderer>();

            TileGo.transform.SetParent(this.transform, false);
            TileGo.name = "Tile_Transition_" + Tiles[i].sprite.name;

            TileGo.transform.localPosition = new Vector3(0, 0, 0);
			TileGo.transform.Rotate(new Vector3(-0.1f, 0,0), Space.Self);

            Render.sprite = Tiles[i].sprite;

            Render.color = Render.color * GetPresets.ColorBiome(Tiles[i].Biome, Tiles[i].type);

            Render.sortingOrder = GetPresets.GetTileRenIndex(Tiles[i].type);

            obj.Add(TileGo);
        }

        Tiles.Clear();
    }

    public void Clear()
    {
        Tiles.Clear();

        foreach (GameObject t in obj)
        {
            Destroy(t);
        }

        obj.Clear();
    }

    private void OnBecameVisible()
    {
        ISVISIBLE = true;
        if (TILEANIMATED)
        {
            TileAnimations.TILESANIMEATION.Add(TILEANIMATIONDATA);
        }
        if (TILEANIMATOR != null)
            TILEANIMATOR.enabled = true;
    }

    private void OnBecameInvisible()
    {
        ISVISIBLE = false;
        if (TILEANIMATED)
        {
            TileAnimations.TILESANIMEATION.Remove(TILEANIMATIONDATA);
        }
        if (TILEANIMATOR != null)
            TILEANIMATOR.enabled = false;
    }
}

public class TileStrucAnimation
{
    public SpriteRenderer SPRITERENDER;
    public Sprite[] Sprite;
    public int animstate;

    public TileStrucAnimation(SpriteRenderer spriteRenderer, Sprite[] sprite)
    {
        SPRITERENDER = spriteRenderer;
        Sprite = sprite;
    }
}