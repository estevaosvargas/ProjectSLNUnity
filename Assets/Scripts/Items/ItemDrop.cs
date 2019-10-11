﻿using UnityEngine;
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

        if (DarckNet.Network.IsServer)
        {
            Net.RPC("RPC_DROPSYNC", DarckNet.RPCMode.AllNoOwner, item.Index);
        }
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

    [RPC]
    void RPC_DROPSYNC(int index)
    {
        ItemData item = ItemManager.Instance.GetItem(index);
        SetDrop(item, 1);
    }
}