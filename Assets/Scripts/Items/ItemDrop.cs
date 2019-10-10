using UnityEngine;
using UnityEngine.UI;

public class ItemDrop : Entity
{
    public ItemData ThisItem;
    public int Qunty = 0;
    public AudioClip PickUpSound;

    public void SetDrop(ItemData item, int quanty)
    {
        ThisItem = item;
        Qunty = quanty;

        name = item.Name + "(Clone)";

        GetComponent<SpriteRenderer>().sprite = item.Icon;
    }

    public void GetThisItem(Inventory inve)
    {
        if (Game.GameManager.SinglePlayer || DarckNet.Network.IsServer)
        {
            inve.Additem(ThisItem.Index, Qunty);
            GetComponent<AudioSource>().PlayOneShot(PickUpSound);
            DarckNet.Network.Destroy(this.gameObject, 0.05f);
        }
    }
}