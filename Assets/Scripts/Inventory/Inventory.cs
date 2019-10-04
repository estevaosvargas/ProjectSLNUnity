using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<InveItem> ItemList = new List<InveItem>();
    public GameObject Drop;
    public bool IsPlayer = false;

    void Awake()
    {

    }

    void Start()
    {
        if (IsPlayer == true)
        {
            SavePlayerInfo inve = SaveWorld.LoadPlayer(Game.GameManager.UserId);

            if (inve != null)
            {
                ItemList = inve.Inve.ItemList;

                transform.position = new Vector3(inve.x, 0, inve.z);

                GetComponent<EntityPlayer>().Status = inve.Status;
                GetComponent<EntityPlayer>().HP = inve.Life;
            }
            Game.MenuManager.InveGui.Inve = this;
            Game.MenuManager.InveGui.OpenInev(this);
        }
        else
        {
            SaveInventory inve = SaveWorld.LoadInve((transform.position.x + "," + transform.position.z).GetHashCode().ToString());

            if (inve != null)
            {
                ItemList = inve.ItemList;
            }
        }
    }

    #region Save/Delete
    public void Save()
    {
        if (IsPlayer == true)
        {
            SaveWorld.SavePlayer(new SavePlayerInfo(new SaveInventory(ItemList), transform.position, GetComponent<EntityLife>().HP, GetComponent<EntityPlayer>().Status), Game.GameManager.UserId);
        }
        else
        {
            SaveWorld.SaveInve(new SaveInventory(ItemList), (transform.position.x + "," + transform.position.z).GetHashCode().ToString());
        }
    }

    public void DeletSave()
    {
        if (IsPlayer == true)
        {
            SaveWorld.DeletPlayer(Game.GameManager.UserId);
        }
        else
        {
            SaveWorld.DeletCont((transform.position.x + "," + transform.position.z).GetHashCode().ToString());
        }
    }
    #endregion

    void Update()
    {

    }

    void _RemoveQuanty(int slot, int quanty)
    {
        ItemList[slot].Amount -= quanty;

        if (ItemList[slot].Amount <= 0)
        {
            ItemList[slot].Index = -1;
            ItemList[slot].Amount = -1;
            OnMove(slot);
        }

        UpdateUi(slot);
    }

    void _Additem(int index, int amount, int slot)
    {
        int Index = ItemList[slot].Index;
        int Amount = ItemList[slot].Amount;

        if (Index == index)
        {
            if (Amount >= ItemManager.Instance.GetItem(index).MaxAmount)
            {
                _Additem(index, amount);
            }
            else
            {
                Amount += amount;
            }
        }
        else
        {
            _Additem(index, amount);
        }

        ItemList[slot].Index = Index;
        ItemList[slot].Amount = Amount;
        Save();
        UpdateUi(slot);
    }

    void _Additem(int index, int amount)
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i].Index == index)
            {
                if (ItemList[i].Amount < ItemManager.Instance.GetItem(ItemList[i].Index).MaxAmount)
                {
                    ItemList[i].Amount += amount;
                    UpdateUi(i);
                    Save();
                    return;
                }
                else if (ItemList[i].Index == -1)
                {
                    ItemList[i].Index = index;
                    ItemList[i].Amount = amount;
                    UpdateUi(i);
                    Save();
                    return;
                }
            }
            else if (ItemList[i].Index == -1)
            {
                ItemList[i].Index = index;
                ItemList[i].Amount = amount;
                UpdateUi(i);
                Save();
                return;
            }
        }
    }

    void _Move(int on, int to)
    {
        if (on != to)
        {
            if (ItemList[to].Index >= 0)
            {
                if (ItemList[to].Index == ItemList[on].Index)
                {
                    if (ItemList[to].Amount < ItemManager.Instance.GetItem(ItemList[to].Index).MaxAmount)
                    {
                        ItemList[to].Index = ItemList[on].Index;
                        ItemList[to].Amount += ItemList[on].Amount;

                        ItemList[on].Index = -1;
                        ItemList[on].Amount = -1;

                        OnMove(on);

                        UpdateUi(to);
                        UpdateUi(on);
                        Save();
                        return;
                    }
                    else
                    {
                        _Additem(ItemList[to].Index, ItemList[on].Amount);

                        ItemList[on].Index = -1;
                        ItemList[on].Amount = -1;

                        OnMove(on);
                        UpdateUi(to);
                        UpdateUi(on);
                        Save();
                        return;
                    }
                }
                else
                {
                    _Additem(ItemList[to].Index, ItemList[on].Amount);

                    ItemList[on].Index = -1;
                    ItemList[on].Amount = -1;

                    OnMove(on);
                    UpdateUi(to);
                    UpdateUi(on);
                    Save();
                    return;
                }
            }
            else
            {
                ItemList[to].Index = ItemList[on].Index;
                ItemList[to].Amount = ItemList[on].Amount;

                ItemList[on].Index = -1;
                ItemList[on].Amount = -1;

                OnMove(on);
                UpdateUi(to);
                UpdateUi(on);
                Save();
                return;
            }
        }
        else
        {

        }
    }

    void _DropItem(int slot)
    {
        ItemData item = ItemManager.Instance.GetItem(ItemList[slot].Index);

        GameObject obj = DarckNet.Network.Instantiate(Drop, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0), Quaternion.identity, 0);

        obj.GetComponent<ItemDrop>().SetDrop(item, ItemList[slot].Amount);

        ItemList[slot].Index = -1;
        ItemList[slot].Amount = -1;
        OnMove(slot);
        UpdateUi(slot);
        Save();
    }

    void OnMove(int index)
    {
        if (IsPlayer)
        {
            if (HandManager.MyHand)
            {
                HandManager.MyHand.RemoveItem(index);
            }
        }
    }

    void UpdateUi(int slotindex)
    {
        if (IsPlayer)
        {
            Game.MenuManager.InveGui.Player_RefreshSlot(slotindex);
        }
        else
        {
            Game.MenuManager.InveGui.Container_RefreshSlot(slotindex);
        }
    }

    #region Singe-Multi
    public void RemoveQuanty(int slot, int quanty)
    {
        if (Game.GameManager.SinglePlayer)
        {
            _RemoveQuanty(slot, quanty);
        }
        else
        {
            //Send RPC, to server
        }
    }

    public void Additem(int index, int amount, int slot)
    {
        if (Game.GameManager.SinglePlayer)
        {
            _Additem(index, amount, slot);
        }
        else
        {
            //Send RPC, to server
        }
    }

    public void Additem(int index, int amount)
    {
        if (Game.GameManager.SinglePlayer)
        {
            _Additem(index, amount);
        }
        else
        {
            //Send RPC, to server
        }
    }

    public void Move(int on, int to)
    {
        if (Game.GameManager.SinglePlayer)
        {
            _Move(on, to);
        }
        else
        {
            GetComponent<NetWorkView>().RPC("RPC_MOVE", DarckNet.RPCMode.Server, on, to);
        }
    }

    public void DropItem(int slot)
    {
        if (Game.GameManager.SinglePlayer)
        {
            _DropItem(slot);
        }
        else
        {
            //Send RPC, to server
        }
    }
    #endregion

    #region RPC_INVE
    [RPC]
    void RPC_MOVE(int on, int to)
    {
        _Move(on, to);
    }
    #endregion
}

[System.Serializable]
public class InveItem
{
    public int Index = -1;
    public int Amount = -1;

    public InveItem(int index, int amount)
    {
        Index = index;
        Amount = amount;
    }
}

[System.Serializable]
public class SaveInventory
{
    public List<InveItem> ItemList = new List<InveItem>();

    public SaveInventory(List<InveItem> itemList)
    {
        ItemList = itemList;
    }
}