using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slots : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler, IDropHandler, ICancelHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int Index;
    public ItemData Item;
    public Image Icon;
    public Text Num;
    public bool Empty = true;
    public InventoryGUI InveGUI;

    public bool IsContainer = false;

    public void OnCancel(BaseEventData eventData)
    {
        Game.GameManager.IconHold.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Empty == false)
        {
            Game.GameManager.IconHold.SetActive(true);
            Game.GameManager.IconHold.transform.position = Input.mousePosition;
            Game.GameManager.IconHold.GetComponentInChildren<Image>().sprite = Icon.sprite;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Game.GameManager.IconHold.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Game.GameManager.IconHold.SetActive(false);

        Slots DragSlot = null;
        Slots DropSlot = null;

        if (eventData.pointerEnter != null) { DropSlot = eventData.pointerEnter.GetComponent<Slots>(); }//Add DropSlot, if is not null
        if (eventData.pointerDrag.GetComponent<Slots>() != null) { DragSlot = eventData.pointerDrag.GetComponent<Slots>(); }//Add DragSlot, if is not null

        if (DragSlot.Empty) { return; }//if Drag slot is empty, return, prevent error's

        if (eventData.pointerEnter == null)//Drop Handller
        {
            if (DragSlot.IsContainer == true)//Container Drop
            {
                InveGUI.InveCont.DropItem(DragSlot.Index);
            }
            else if (DragSlot.IsContainer == false)//Player Drop
            {
                InveGUI.Inve.DropItem(DragSlot.Index);
            }
        }
        else
        {
            if (DragSlot.IsContainer == false && DropSlot.IsContainer == true)//Player To Container
            {
                Game.GameManager.MoveCont(InveGUI.Inve, InveGUI.InveCont, DragSlot.Index, DropSlot.Index);
            }
            else if (DragSlot.IsContainer == true && DropSlot.IsContainer == false)//Container To Player
            {
                Game.GameManager.ContMove(InveGUI.Inve, InveGUI.InveCont, DragSlot.Index, DropSlot.Index);
            }
            else if (DragSlot.IsContainer == false && DropSlot.IsContainer == false)//Player To Player
            {
                InveGUI.Inve.Move(DragSlot.Index, DropSlot.Index);
            }
            else if (DragSlot.IsContainer == true && DropSlot.IsContainer == true)//Container To Container
            {
                InveGUI.InveCont.Move(DragSlot.Index, DropSlot.Index);
            }
        }
        DragSlot = null;
        DropSlot = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Empty == false)
        {
            Color32 color = Color.white;

            if (Item.itemRarity == ItemRarity.Common)
            {
                color = Color.white;
            }
            else if (Item.itemRarity == ItemRarity.Uncommon)
            {
                color = new Color32(119, 213, 255, 255);
            }
            else if (Item.itemRarity == ItemRarity.Epic)
            {
                color = new Color32(255, 86, 2, 255);
            }
            else if (Item.itemRarity == ItemRarity.Legendary)
            {
                color = new Color32(86, 2, 255, 255);
            }
            else if (Item.itemRarity == ItemRarity.Dark)
            {
                color = new Color32(68, 0, 125, 255);
            }
            else if (Item.itemRarity == ItemRarity.SelfMade)
            {
                color = Color.white;
            }

            Game.GameManager.CurrentPlayer.InveItemInfo.gameObject.SetActive(true);
            Game.GameManager.CurrentPlayer.InveItemInfo.SetInfo(Item.Name, Item.Description, Item.Icon, Item.itemRarity.ToString(), color);
            Game.GameManager.CurrentPlayer.InveItemInfo.transform.position = Input.mousePosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Game.GameManager.CurrentPlayer.InveItemInfo.gameObject.SetActive(false);
    }

    public void SetSlot(int index, int quanty, ItemData item, bool iscont)
    {
        Index = index;
        Item = item;
        IsContainer = iscont;

        if (quanty > 0)
        {
            Num.text = quanty.ToString();
            Icon.sprite = item.Icon;

            Empty = false;

            Icon.gameObject.SetActive(true);
            Num.gameObject.SetActive(true);
        }
        else
        {
            Empty = true;
            Icon.gameObject.SetActive(false);
            Num.gameObject.SetActive(false);
        }
    }

    public void UpdateSlot(int quanty, bool iscont)
    {
        IsContainer = iscont;

        if (quanty > 0)
        {
            Num.text = quanty.ToString();

            Empty = false;

            Icon.gameObject.SetActive(true);
            Num.gameObject.SetActive(true);
        }
        else
        {
            Empty = true;
            Icon.gameObject.SetActive(false);
            Num.gameObject.SetActive(false);
        }
    }
}