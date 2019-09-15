using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour {

    public int HP = 100;
    public SpriteRenderer Destroying;
    public GameObject FX;
    float color = 0;
    public Tile ThisTreeTile;
    public int ItemDrop = 2;
    public int DropQuanty = 10;

    public void DoDamage(int damage)
    {
        HP -= damage;

        color += 0.1f;

        Destroying.color = new Color(Destroying.color.r, Destroying.color.g, Destroying.color.b, color);

        if (HP <= 0)
        {
            GameObject obj = Instantiate(FX, transform.position, Quaternion.identity);
            obj.transform.rotation = new Quaternion(0,180,0, 1);
            Destroy(obj, 5);
            ThisTreeTile.typego = TakeGO.empty;
            ThisTreeTile.RefreshTile();
            ThisTreeTile.SaveChunk();
            ItemManager.Instance.SpawnItem(ItemDrop, DropQuanty);
            Destroy(this.gameObject);
        }
    }
}
