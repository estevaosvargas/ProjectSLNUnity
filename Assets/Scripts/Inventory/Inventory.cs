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
    public NetWorkView Net;

    public List<Lidgren.Network.NetConnection> PlayerOpen = new List<Lidgren.Network.NetConnection>();

    public int HandOneIndex;
    public int HandTwoIndex;

    void Awake()
    {

    }

    void Start()
    {
        Net = GetComponent<NetWorkView>();

        if (IsPlayer == true)
        {
            SavePlayerInfo inve = SaveWorld.LoadPlayer(Game.GameManager.Player.UserID);

            if (inve != null)
            {
                ItemList = inve.Inve.ItemList;

                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Inside")
                {
                    transform.position = new Vector3(inve.x, 1, inve.z);
                }

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
        if (Game.GameManager.SinglePlayer)
        {
            if (IsPlayer == true)
            {
                SaveWorld.SavePlayer(new SavePlayerInfo(new SaveInventory(ItemList), transform.position, GetComponent<EntityLife>().HP, GetComponent<EntityPlayer>().Status), Game.GameManager.Player.UserID);
            }
            else
            {
                SaveWorld.SaveInve(new SaveInventory(ItemList), (transform.position.x + "," + transform.position.z).GetHashCode().ToString());
            }
        }
    }

    public void DeletSave()
    {
        if (Game.GameManager.SinglePlayer)
        {
            if (IsPlayer == true)
            {
                SaveWorld.DeletPlayer(Game.GameManager.Player.UserID);
            }
            else
            {
                SaveWorld.DeletCont((transform.position.x + "," + transform.position.z).GetHashCode().ToString());
            }
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

    void _AdditemSlot(int index, int amount, int slot, bool first)
    {
        int Index = ItemList[slot].Index;
        int Amount = ItemList[slot].Amount;

        if (Index == index)
        {
            if (Amount > ItemManager.Instance.GetItem(index).MaxAmount)
            {
                _Additem(index, amount, slot, first);
            }
            else
            {
                Amount += amount;

                ItemList[slot].Index = Index;
                ItemList[slot].Amount = Amount;
                Save();
                UpdateUi(slot);

                if (!Net.isMine || !Game.GameManager.SinglePlayer)
                {
                    foreach (var connection in PlayerOpen)
                    {
                        Net.RPC("RPC_SyncOneSlot", connection, slot, ItemList[slot].Index, ItemList[slot].Amount);
                    }
                }
            }
        }
        else
        {
            _Additem(index, amount, slot, first);
        }
    }

    void _Additem(int index, int amount, int slotchek, bool first)
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i].ItemType == ItemManager.Instance.GetItem(index).ITEMTYPE || ItemList[i].ItemType == ItemType.none)
            {
                if (ItemList[i].Index == index)
                {
                    if (i != slotchek || first)
                    {
                        if (ItemList[i].Amount < ItemManager.Instance.GetItem(ItemList[i].Index).MaxAmount)
                        {
                            ItemList[i].Amount += amount;
                            UpdateUi(i);
                            Save();

                            if (!Net.isMine || !Game.GameManager.SinglePlayer)
                            {
                                foreach (var connection in PlayerOpen)
                                {
                                    Net.RPC("RPC_SyncOneSlot", connection, i, ItemList[i].Index, ItemList[i].Amount);
                                }
                            }
                            return;
                        }
                        else if (ItemList[i].Index == -1)
                        {
                            ItemList[i].Index = index;
                            ItemList[i].Amount = amount;
                            UpdateUi(i);
                            Save();

                            if (!Net.isMine || !Game.GameManager.SinglePlayer)
                            {
                                foreach (var connection in PlayerOpen)
                                {
                                    Net.RPC("RPC_SyncOneSlot", connection, i, ItemList[i].Index, ItemList[i].Amount);
                                }
                            }
                        }
                    }
                }
                else if (ItemList[i].Index == -1)
                {
                    ItemList[i].Index = index;
                    ItemList[i].Amount = amount;
                    UpdateUi(i);
                    Save();

                    if (!Net.isMine || !Game.GameManager.SinglePlayer)
                    {
                        foreach (var connection in PlayerOpen)
                        {
                            Net.RPC("RPC_SyncOneSlot", connection, i, ItemList[i].Index, ItemList[i].Amount);
                        }
                    }
                    return;
                }
            }
            else
            {
                Game.PrintError("Your inventory is full, Can't add this item on your invetory!", false);
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
                        _Additem(ItemList[on].Index, ItemList[on].Amount, on, false);

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
                    _Additem(ItemList[on].Index, ItemList[on].Amount, on, false);

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

    bool _DropItem(int slot, Vector3 pos)
    {
        if (ItemList[slot].Index >= 0)
        {
            ItemData item = ItemManager.Instance.GetItem(ItemList[slot].Index);

            GameObject obj = DarckNet.Network.Instantiate(Drop, pos, Quaternion.identity, 0);

            obj.GetComponent<ItemDrop>().SetDrop(item, ItemList[slot].Amount);

            ItemList[slot].Index = -1;
            ItemList[slot].Amount = -1;
            OnMove(slot);
            UpdateUi(slot);
            Save();
            return true;
        }
        else
        {
            return false;
        }
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
        if (Game.GameManager.SinglePlayer || Game.GameManager.MultiPlayer)
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
            UnityEngine.Debug.LogError("Sorry you don't have permission to do this!");
        }
    }

    public void Additem(int index, int amount, int slot)
    {
        if (Game.GameManager.SinglePlayer)
        {
            _Additem(index, amount, slot, true);
        }
        else
        {
            UnityEngine.Debug.LogError("Sorry you don't have permission to do this!");
        }
    }

    public void Additem(int index, int amount)
    {
        if (Game.GameManager.SinglePlayer || DarckNet.Network.IsServer)
        {
            _Additem(index, amount, -1 ,true);
        }
        else
        {
            UnityEngine.Debug.LogError("Sorry you don't have permission to do this!");
        }
    }

    public void Move(int on, int to)
    {
        Net.RPC("RPC_MOVE", DarckNet.RPCMode.Server, on, to);
    }

    public void DropItem(int slot)
    {
        Net.RPC("RPC_DROP", DarckNet.RPCMode.Server, slot, new Vector3(Game.GameManager.hit.point.x, 0.5f, Game.GameManager.hit.point.z));
    }

    public void RequestInveData()
    {
        Net.RPC("RPC_REQUESTDATA", DarckNet.RPCMode.Server, JsonHelper.ToJson(ItemList.ToArray()).GetHashCode());
    }

    public void NET_SendCloseInve()
    {
        Net.RPC("RPC_REQUESTCLOSE", DarckNet.RPCMode.Server);
    }
    #endregion

    #region RPC_INVE_Client
    [RPC]
    void RPC_SyncSlot(int on, int to, int index_on, int index_to, int Ammount_on, int Ammount_to)
    {
        ItemList[on].Index = index_on;
        ItemList[on].Amount = Ammount_on;

        ItemList[to].Index = index_to;
        ItemList[to].Amount = Ammount_to;

        OnMove(on);
        UpdateUi(to);
        UpdateUi(on);
    }

    [RPC]
    void RPC_SyncOneSlot(int slot, int index, int Ammount)
    {
        ItemList[slot].Index = index;
        ItemList[slot].Amount = Ammount;

        OnMove(slot);
        UpdateUi(slot);
    }

    [RPC]
    void RPC_RECEIVEDATA(string compressed_data)
    {
        string decompresed = CompressString.StringCompressor.DecompressString(compressed_data);
        ItemList = new List<InveItem>(JsonHelper.FromJson<InveItem>(decompresed));
        UnityEngine.Debug.Log("Receive data : " + decompresed);
    }

    #endregion

    #region RPC_INVE
    [RPC]
    void RPC_REQUESTCLOSE(DarckNet.DNetConnection sender)
    {
        Lidgren.Network.NetConnection netcon = sender.NetConnection;
        if (PlayerOpen.Contains(netcon))
        {
            PlayerOpen.Remove(sender.NetConnection);
            UnityEngine.Debug.Log("This player (" + sender.unique + ") ask to remove him from inventory!");
        }
        else
        {
            UnityEngine.Debug.Log("This player (" + sender.unique + ") ask to remove him from inventory, but he isn't in inventory");
        }
    }

    [RPC]
    void RPC_REQUESTDATA(int hashcode ,DarckNet.DNetConnection sender)
    {
        PlayerOpen.Add(sender.NetConnection);

        string json = JsonHelper.ToJson(ItemList.ToArray());

        if (json.GetHashCode() != hashcode)
        {
            string compresded = CompressString.StringCompressor.CompressString(json);
            Net.RPC("RPC_RECEIVEDATA", sender.NetConnection, compresded);
            UnityEngine.Debug.Log("Hash Code : " + hashcode + " : Sended inve data to : " + sender.unique + " : Data: " + compresded);
        }
    }

    [RPC]
    void RPC_DROP(int slot, Vector3 world_pos, DarckNet.DNetConnection sender)
    {
        if (Net.Owner == sender.unique)
        {
            if (_DropItem(slot, world_pos))
            {
                UnityEngine.Debug.Log("Request Drop: " + slot + " ::: " + sender.unique);

                if (!Net.isMine || !Game.GameManager.SinglePlayer)
                {
                    Net.RPC("RPC_SyncOneSlot", sender.NetConnection, slot, ItemList[slot].Index, ItemList[slot].Amount);
                }
            }
        }
    }

    [RPC]
    void RPC_MOVE(int on, int to, DarckNet.DNetConnection sender)
    {
        UnityEngine.Debug.Log("ON: " + on + " | TO: " + to + " ::: " + sender.unique);
        _Move(on, to);

        if (!Net.isMine || !Game.GameManager.SinglePlayer)
        {
            Net.RPC("RPC_SyncSlot", sender.NetConnection, on, to, ItemList[on].Index, ItemList[to].Index, ItemList[on].Amount, ItemList[to].Amount);
        }
    }
    #endregion
}

[System.Serializable]
public class InveItem
{
    public int Index = -1;
    public int Amount = -1;

    [System.NonSerialized]
    public ItemType ItemType = ItemType.none;

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