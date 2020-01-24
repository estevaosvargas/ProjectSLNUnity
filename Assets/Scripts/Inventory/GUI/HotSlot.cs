using UnityEngine.UI;
using UnityEngine;

public class HotSlot : MonoBehaviour
{
    public KeyCode Key;
    public int HotIndex;

    public Text HotNum;

    void Start()
    {

    }

    public void SetSlot(int index)
    {
        HotIndex = index;

        switch (index)
        {
            case 0:
                Key = KeyCode.Alpha1;
                HotNum.text = "1";
                break;
            case 1:
                Key = KeyCode.Alpha2;
                HotNum.text = "2";
                break;
            case 2:
                Key = KeyCode.Alpha3;
                HotNum.text = "3";
                break;
            case 3:
                Key = KeyCode.Alpha4;
                HotNum.text = "4";
                break;
            case 4:
                Key = KeyCode.Alpha5;
                HotNum.text = "5";
                break;
            case 5:
                Key = KeyCode.Alpha6;
                HotNum.text = "6";
                break;
            case 6:
                Key = KeyCode.Alpha7;
                HotNum.text = "7";
                break;
            case 7:
                Key = KeyCode.Alpha8;
                HotNum.text = "8";
                break;
            case 8:
                Key = KeyCode.Alpha9;
                HotNum.text = "9";
                break;
            case 9:
                Key = KeyCode.Alpha0;
                HotNum.text = "0";
                break;
            default:
                Key = KeyCode.Alpha0;
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            if (GetComponent<Slots>().Item != null)
            {
                if (GetComponent<Slots>().Item.CanEquip)
                {
                    if (HandManager.MyHand.SlotIndex == GetComponent<Slots>().Index)
                    {
                        HandManager.MyHand.RemoveItem();
                    }
                    else
                    {
                        HandManager.MyHand.PutItem(GetComponent<Slots>().Item, GetComponent<Slots>().Index);
                    }
                }
            }
        }
    }
}