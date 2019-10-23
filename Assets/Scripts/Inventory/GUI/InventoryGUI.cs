using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGUI : MonoBehaviour {

    public List<Slots> Player_Slots = new List<Slots>();
    public List<Slots> Container_Slots = new List<Slots>();

    public int NumSlot = 2;

    public GameObject SlotPrefab;
    public GameObject SlotHotPrefab;

    public RectTransform InveRoot;
    public RectTransform ArmorRoot;
    public RectTransform ContRoot;
    public RectTransform HotRoot;

    public GameObject ContainerInve;

    public Inventory Inve;
    public Inventory InveCont;

    void Awake()
    {
        Game.InventoryGUI = this;
    }

    public void CloseInve(Inventory closeinve)
    {
        closeinve.NET_SendCloseInve();
    }

    public void OpenInev(Inventory inventory)
    {
        Inve = inventory;
        InveCont = null;
        NumSlot = Inve.ItemList.Count;

        Inve.RequestInveData();

        ContainerInve.SetActive(false);
        ClearCanvasPlayer();
        for (int i = 0; i < Inve.ItemList.Count; i++)
        {
            if (Inve.ItemList[i].Amount > 0)
            {
                LoadInventory(i, Inve.ItemList[i].Amount, ItemManager.Instance.GetItem(Inve.ItemList[i].Index));
            }
            else
            {
                LoadInventory(i, 0, null);
            }
        }
    }

    public void OpenInevContainer(Inventory inventory, Inventory container)
    {
        Inve = inventory;
        InveCont = container;
        NumSlot = Inve.ItemList.Count;

        inventory.RequestInveData();
        container.RequestInveData();

        ContainerInve.SetActive(true);

        ClearCanvasPlayer();
        ClearCanvasContainer();
        for (int i = 0; i < Inve.ItemList.Count; i++)
        {
            if (Inve.ItemList[i].Amount > 0)
            {
                LoadInventory(i, Inve.ItemList[i].Amount, ItemManager.Instance.GetItem(Inve.ItemList[i].Index));
            }
            else
            {
                LoadInventory(i, 0, null);
            }
        }

        for (int i = 0; i < container.ItemList.Count; i++)
        {
            if (container.ItemList[i].Amount > 0)
            {
                LoadContainer(i, container.ItemList[i].Amount, ItemManager.Instance.GetItem(container.ItemList[i].Index));
            }
            else
            {
                LoadContainer(i, 0, null);
            }
        }
    }

    int hotba;

    public void LoadContainer(int index, int qunty, ItemData item)
    {
        GameObject obj = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity);
        obj.name = "Slot: " + index;
        obj.GetComponent<Slots>().SetSlot(index, qunty, item, true);
        obj.GetComponent<Slots>().InveGUI = this;
        obj.transform.SetParent(ContRoot.gameObject.transform);
        Container_Slots.Add(obj.GetComponent<Slots>());
    }

    public void LoadInventory(int index,int qunty, ItemData item)
    {
        if (index >= NumSlot -6 && index < NumSlot)
        {
            GameObject obj = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity);
            obj.name = "ArmorSlot: " + index;
            obj.GetComponent<Slots>().SetSlot(index, qunty, item, false);
            obj.GetComponent<Slots>().InveGUI = this;
            obj.GetComponent<Slots>().SlotItemType = ItemType.Armor;
            Inve.ItemList[index].ItemType = ItemType.Armor;
            Player_Slots.Add(obj.GetComponent<Slots>());
            obj.transform.SetParent(ArmorRoot.gameObject.transform);
        }
        else if (index >= NumSlot -12 && index <= NumSlot -6)
        {
            GameObject obj = GameObject.Instantiate(SlotHotPrefab, Vector3.zero, Quaternion.identity);
            obj.name = "HotBar: " + index;
            obj.GetComponent<Slots>().SetSlot(index, qunty, item, false);
            obj.GetComponent<Slots>().InveGUI = this;
            obj.GetComponent<Slots>().SlotItemType = ItemType.none;
            Inve.ItemList[index].ItemType = ItemType.none;
            Player_Slots.Add(obj.GetComponent<Slots>());
            obj.GetComponent<HotSlot>().SetSlot(hotba);
            obj.transform.SetParent(HotRoot.gameObject.transform);
            hotba++;
        }
        else
        {
            GameObject obj = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity);

            obj.name = "Slot: " + index;

            obj.GetComponent<Slots>().SetSlot(index, qunty, item, false);
            obj.GetComponent<Slots>().InveGUI = this;
            obj.GetComponent<Slots>().SlotItemType = ItemType.none;
            Inve.ItemList[index].ItemType = ItemType.none;
            Player_Slots.Add(obj.GetComponent<Slots>());

            obj.transform.SetParent(InveRoot.gameObject.transform);
        }
    }

    public void Player_RefreshSlot(int index)
    {
        if (Inve.ItemList[index].Index >= 0)
        {
            Player_Slots[index].SetSlot(index, Inve.ItemList[index].Amount, ItemManager.Instance.GetItem(Inve.ItemList[index].Index), false);
        }
        else
        {
            Player_Slots[index].SetSlot(index, -1, null, false);
        }
    }

    public void Container_RefreshSlot(int index)
    {
        if (InveCont.ItemList[index].Index >= 0)
        {
            Container_Slots[index].SetSlot(index, InveCont.ItemList[index].Amount, ItemManager.Instance.GetItem(InveCont.ItemList[index].Index), true);
        }
        else
        {
            Container_Slots[index].SetSlot(index, 0, null, true);
        }
    }

    public void RefreshAll()
    {
        ClearCanvasContainer();
        ClearCanvasPlayer();
        for (int i = 0; i < Inve.ItemList.Count; i++)
        {
            if (Inve.ItemList[i].Amount > 0)
            {
                LoadInventory(i, Inve.ItemList[i].Amount, ItemManager.Instance.GetItem(Inve.ItemList[i].Index));
            }
            else
            {
                LoadInventory(i, 0, null);
            }
        }

        if (InveCont != null)
        {
            for (int i = 0; i < InveCont.ItemList.Count; i++)
            {
                if (Inve.ItemList[i].Amount > 0)
                {
                    LoadContainer(i, InveCont.ItemList[i].Amount, ItemManager.Instance.GetItem(InveCont.ItemList[i].Index));
                }
                else
                {
                    LoadContainer(i, 0, null);
                }
            }
        }
    }

    public void ClearCanvasPlayer()
    {
        hotba = 0;
        Player_Slots.Clear();

        foreach (Transform child in InveRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in HotRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in ArmorRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ClearCanvasContainer()
    {
        hotba = 0;
        Container_Slots.Clear();
        foreach (Transform child in ContRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
