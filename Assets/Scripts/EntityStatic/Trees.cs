using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : StaticLife
{
    public SpriteRenderer Destroying;
    public GameObject FX;
    float color = 0;
    public Tile ThisTreeTile;
    public int ItemDrop = 2;
    public int DropQuanty = 10;
    public MaterialHitType MaterialHit;

    public void Damage(ItemData item, int damage)
    {
        _DoDamage(damage, item.MaterialHitBest);
    }

    public void Damage(HandData hand, int damage)
    {
        _DoDamage(damage, hand.MaterialHitBest);
    }

    void _DoDamage(int damage, MaterialHitType mat_type)
    {
        if (MaterialHit == mat_type)
        {
            damage = damage * 2;
        }
        else
        {
            damage = damage - 5;
        }

        DoDamage(damage, "none", false);
    }

    public override void OnDead()
    {
        GameObject obj = Instantiate(FX, transform.position, Quaternion.identity);
        obj.transform.rotation = new Quaternion(0, 180, 0, 1);
        Destroy(obj, 5);

        ThisTreeTile.typego = TakeGO.empty;
        ThisTreeTile.RefreshTile();
        ThisTreeTile.SaveChunk();

        ItemManager.Instance.SpawnItem(ItemDrop, DropQuanty, transform.position);
        Destroy(this.gameObject);
        base.OnDead();
    }
}
