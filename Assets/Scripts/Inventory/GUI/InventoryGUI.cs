using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGUI : MonoBehaviour {

    public List<Slots> Player_Slots = new List<Slots>();
    public List<Slots> Container_Slots = new List<Slots>();

    public int NumSlot = 2;
    public int Size = 5;
    public int SizeC = 5;
    public float Spacing = 3;

    public GameObject SlotPrefab;
    public GameObject SlotHotPrefab;

    public RectTransform InveRoot;
    public RectTransform ContRoot;
    public RectTransform HotRoot;

    public GameObject ContainerInve;

    private int numRaw;
    private int numItem;


    private int numRaw2;
    private int numItem2;

    public Inventory Inve;
    public Inventory InveCont;

    void Start()
    {
        if (Game.GameManager.MyPlayer.MyInventory)
        {
            Inve = Game.GameManager.MyPlayer.MyInventory;
        }

        ContainerInve.SetActive(false);
        ClearCanvasPlayer();
        for (int i = 0; i < 30; i++)
        {
            LoadInventory(i, 0, null);
        }
    }

    public void OpenInev(Inventory inventory)
    {
        Inve = inventory;
        InveCont = null;
        NumSlot = Inve.ItemList.Count;


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

        ContainerInve.SetActive(true);
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
        GameObject obj = (GameObject)GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity);

        obj.name = "Slot: " + index;

        obj.GetComponent<Slots>().SetSlot(index, qunty, item, true);
        obj.GetComponent<Slots>().InveGUI = this;

        obj.transform.SetParent(ContRoot.gameObject.transform);
        RectTransform rect = obj.GetComponent<RectTransform>();

        Container_Slots.Add(obj.GetComponent<Slots>());

        rect.anchoredPosition = new Vector2(((rect.sizeDelta.x + Spacing) * numItem2) + Spacing, -(((rect.sizeDelta.y + Spacing) * numRaw2) + Spacing));
        rect.localScale = SlotPrefab.gameObject.transform.localScale;
        numItem2++;

        if (numItem2 >= SizeC)
        {
            numItem2 = 0;
            numRaw2 += 1;
        }

        ContRoot.sizeDelta = new Vector2(ContRoot.sizeDelta.x, (SlotPrefab.GetComponent<RectTransform>().sizeDelta.y + Spacing) * (numRaw2 + 1));
    }

    public void LoadInventory(int index,int qunty, ItemData item)
    {
        if (index >= NumSlot -6 && index < NumSlot)
        {
            //numItem = 0;
            numRaw = 0;
            GameObject obj = (GameObject)GameObject.Instantiate(SlotHotPrefab, Vector3.zero, Quaternion.identity);

            obj.name = "HotBar: " + index;

            obj.GetComponent<Slots>().SetSlot(index, qunty, item, false);
            obj.GetComponent<Slots>().InveGUI = this;

            Player_Slots.Add(obj.GetComponent<Slots>());

            obj.GetComponent<HotSlot>().SetSlot(hotba);

            obj.transform.SetParent(HotRoot.gameObject.transform);
            RectTransform rect = obj.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(((rect.sizeDelta.x + Spacing) * hotba) + Spacing, -(((rect.sizeDelta.y + Spacing) * numRaw) + Spacing));
            rect.localScale = SlotPrefab.gameObject.transform.localScale;
            hotba++;
        }
        else
        {
            GameObject obj = (GameObject)GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity);

            obj.name = "Slot: " + index;

            obj.GetComponent<Slots>().SetSlot(index, qunty, item, false);
            obj.GetComponent<Slots>().InveGUI = this;

            Player_Slots.Add(obj.GetComponent<Slots>());

            obj.transform.SetParent(InveRoot.gameObject.transform);
            RectTransform rect = obj.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(((rect.sizeDelta.x + Spacing) * numItem) + Spacing, -(((rect.sizeDelta.y + Spacing) * numRaw) + Spacing));
            rect.localScale = SlotPrefab.gameObject.transform.localScale;
            numItem++;

            if (numItem >= SizeC)
            {
                numItem = 0;
                numRaw += 1;
            }

            InveRoot.sizeDelta = new Vector2(InveRoot.sizeDelta.x, (SlotPrefab.GetComponent<RectTransform>().sizeDelta.y + Spacing) * (numRaw + 1));
        }
    }

    public void Player_RefreshSlot(int index)
    {
        if (Inve.ItemList[index].Amount > 0)
        {
            Player_Slots[index].SetSlot(index, Inve.ItemList[index].Amount, ItemManager.Instance.GetItem(Inve.ItemList[index].Index), false);
        }
        else
        {
            Player_Slots[index].SetSlot(index, 0, null, false);
        }
    }

    public void Container_RefreshSlot(int index)
    {
        if (InveCont.ItemList[index].Amount > 0)
        {
            Container_Slots[index].SetSlot(index, InveCont.ItemList[index].Amount, ItemManager.Instance.GetItem(InveCont.ItemList[index].Index), true);
        }
        else
        {
            Container_Slots[index].SetSlot(index, 0, null, false);
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
                if (InveCont.ItemList[i].Amount > 0)
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

        numItem = 0;
        numRaw = 0;

        numItem2 = 0;
        numRaw2 = 0;

        Player_Slots.Clear();

        foreach (Transform child in InveRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in HotRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ClearCanvasContainer()
    {
        hotba = 0;

        numItem = 0;
        numRaw = 0;

        numItem2 = 0;
        numRaw2 = 0;
        Container_Slots.Clear();
        foreach (Transform child in ContRoot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
