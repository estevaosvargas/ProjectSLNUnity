using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public ItemData CurrentItem;
    public GameObject CurrentItemObject;
    public static HandManager MyHand;
    public Inventory Inve;
    public bool OnHand = false;
    public int SlotIndex = -1;
    public HandData Hand;

    public Transform HandRoot;

    float timetemp;

    /// New System
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask EntitysLayer;


    void Awake()
    {
        MyHand = this;
    }

    void Start()
    {
        timetemp = Time.time;
        Inve = GetComponent<Inventory>();
        OnHand = true;
    }

    public void PutItem(ItemData item, int slot)
    {
        Clear();

        CurrentItemObject = Instantiate(Game.SpriteManager.GetHandItem(item.Name), HandRoot);

        CurrentItemObject.GetComponent<FPItemBase>().SetUpData(item);

        CurrentItem = item;
        SlotIndex = slot;
        OnHand = false;
    }

    public void RemoveItem()
    {
        Clear();
        OnHand = true;
    }

    public void RemoveItem(int slot)
    {
        if (slot == SlotIndex)
        {
            Clear();
            OnHand = true;
        }
    }

    void Clear()
    {
        Destroy(CurrentItemObject);
        CurrentItem = null;
        SlotIndex = -1;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    public void PhysicDamage(ItemData _CurrentItem, float range)
    {
        Collider[] Entitys = Physics.OverlapSphere(attackPoint.position, range, EntitysLayer);

        if (Time.time > timetemp + _CurrentItem.About.FireRate)
        {
            foreach (var item in Entitys)
            {
                if (item.GetComponent<EntityPlayer>())
                {
                    if (!item.GetComponent<EntityPlayer>().Net.isMine)//if isnt me
                    {
                        if (item.tag != "TreeTrigger")
                        {
                            if (item.GetComponent<EntityLife>() != null)
                            {
                                item.GetComponent<EntityLife>().DoDamage(_CurrentItem.About.EntityDamage, "Player", true);
                            }
                            if (item.GetComponent<Trees>() != null)
                            {
                                item.GetComponent<Trees>().Damage(_CurrentItem, _CurrentItem.About.BlockDamage);
                            }
                            GetComponent<Animator>().SetTrigger("Attack");
                            Game.GameManager.PopUpDamage(Camera.main.WorldToScreenPoint(item.transform.position), _CurrentItem.About.EntityDamage);
                            Debug.Log("Attacked : " + item.name);
                        }
                        else
                        {
                            GetComponent<Animator>().SetTrigger("Attack");
                            Debug.Log("Attacked : Air");
                        }
                    }
                }
                else
                {
                    if (item.tag != "TreeTrigger")
                    {
                        if (item.GetComponent<EntityLife>() != null)
                        {
                            item.GetComponent<EntityLife>().DoDamage(_CurrentItem.About.EntityDamage, "Player", true);
                        }
                        if (item.GetComponent<Trees>() != null)
                        {
                            item.GetComponent<Trees>().Damage(_CurrentItem, _CurrentItem.About.BlockDamage);
                        }
                        GetComponent<Animator>().SetTrigger("Attack");
                        Game.GameManager.PopUpDamage(Camera.main.WorldToScreenPoint(item.transform.position), _CurrentItem.About.EntityDamage);
                        Debug.Log("Attacked : " + item.name);
                    }
                    else
                    {
                        GetComponent<Animator>().SetTrigger("Attack");
                        Debug.Log("Attacked : Air");
                    }
                }
            }

            if (Entitys.Length <= 0)
            {
                GetComponent<Animator>().SetTrigger("Attack");
                Debug.Log("Attacked : Air");
            }

            timetemp = Time.time;
        }
    }

    public void PlaceBlock()
    {
        if (CurrentItem.ITEMTYPE == ItemType.Block)
        {
            Game.GameManager.t.PlaceBlockSet(CurrentItem.About.BlockType);
            Inve.RemoveQuanty(SlotIndex, 1);
        }
        else if (CurrentItem.ITEMTYPE == ItemType.Placer)
        {
            Game.GameManager.t.SetPlacer(CurrentItem.About.placer);
            Inve.RemoveQuanty(SlotIndex, 1);
        }
    }

    public void FirstAction()
    {
        
        /*Collider[] Entitys = Physics.OverlapSphere(attackPoint.position, attackRange, EntitysLayer);

        if (CurrentItem != null)
        {
            if (Time.time > timetemp + CurrentItem.About.FireRate)
            {
                foreach (var item in Entitys)
                {
                    if (item.tag != "TreeTrigger")
                    {
                        if (item.GetComponent<EntityLife>() != null)
                        {
                            item.GetComponent<EntityLife>().DoDamage(CurrentItem.About.EntityDamage, "Player", true);
                        }
                        if (item.GetComponent<Trees>() != null)
                        {
                            item.GetComponent<Trees>().Damage(CurrentItem, CurrentItem.About.BlockDamage);
                        }
                        GetComponent<Animator>().SetTrigger("Attack");
                        Game.GameManager.PopUpDamage(Camera.main.WorldToScreenPoint(item.transform.position), CurrentItem.About.EntityDamage);
                        Debug.Log("Attacked : " + item.name);
                    }
                    else
                    {
                        GetComponent<Animator>().SetTrigger("Attack");
                        Debug.Log("Attacked : Air");
                    }
                }

                if (Entitys.Length <= 0)
                {
                    GetComponent<Animator>().SetTrigger("Attack");
                    Debug.Log("Attacked : Air");
                }

                timetemp = Time.time;
            }
        }
        else if (OnHand)
        {
            if (Time.time > timetemp + Hand.FireRate)
            {
                foreach (var item in Entitys)
                {
                    if (item.tag != "TreeTrigger")
                    {
                        if (item.GetComponent<EntityLife>() != null)
                        {
                            item.GetComponent<EntityLife>().DoDamage(Hand.DamageEnity, "Player", true);
                        }
                        if (item.GetComponent<Trees>() != null)
                        {
                            item.GetComponent<Trees>().Damage(Hand, Hand.DamageBlock);
                        }
                        GetComponent<Animator>().SetTrigger("Attack");
                        Game.GameManager.PopUpDamage(Camera.main.WorldToScreenPoint(item.transform.position), Hand.DamageEnity);
                        Debug.Log("Attacked : " + item.name);
                    }
                    else
                    {
                        GetComponent<Animator>().SetTrigger("Attack");
                        Debug.Log("Attacked : Air");
                    }
                }

                if (Entitys.Length <= 0)
                {
                    GetComponent<Animator>().SetTrigger("Attack");
                    Debug.Log("Attacked : Air");
                }

                timetemp = Time.time;
            }
        }*/
    }


    public void SecondAction()
    {
        if (CurrentItem.ITEMTYPE == ItemType.Weapon)
        {

        }
        else if (CurrentItem.ITEMTYPE == ItemType.Block)
        {
            PlaceBlock();
        }
        else if (CurrentItem.ITEMTYPE == ItemType.Tools)
        {
            if (CurrentItem != null)
            {
                if (Time.time > timetemp + CurrentItem.About.FireRate)
                {
                    Game.GameManager.t.DamageBloco(CurrentItem.About.BlockDamage);
                    GetComponent<Animator>().SetTrigger("Attack");
                    timetemp = Time.time;
                }
            }
            else if (OnHand)
            {
                if (Time.time > timetemp + Hand.FireRate)
                {
                    Game.GameManager.t.DamageBloco(Hand.DamageBlock);
                    GetComponent<Animator>().SetTrigger("Attack");
                    timetemp = Time.time;
                }
            }
        }
        else
        {

        }
    }

    int Distance = 0;

    void Update()
    {
        if (Game.GameManager.t != null)
        {
            Distance = (int)Vector3.Distance(transform.position, new Vector3(Game.GameManager.t.x, Game.GameManager.t.y, Game.GameManager.t.z));
        }

        if (Distance <= 3)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (Get.OpenInveTile(Game.GameManager.t))
                {
                    Game.MenuManager.OpenInveContainer(Game.GameManager.t.ObjThis.GetComponent<Inventory>());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))//Hand01
        {
            if (Inve.HandOneIndex >= 0)
            {
                ItemData item = ItemManager.Instance.GetItem(Inve.ItemList[Inve.HandOneIndex].Index);

                if (item.CanEquip)
                {
                    if (SlotIndex == Inve.HandOneIndex)
                    {
                        RemoveItem();
                    }
                    else
                    {
                        PutItem(item, Inve.HandOneIndex);
                    }
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))//Hand02
        {
            if (Inve.HandTwoIndex >= 0)
            {
                ItemData item = ItemManager.Instance.GetItem(Inve.ItemList[Inve.HandTwoIndex].Index);

                if (item.CanEquip)
                {
                    if (SlotIndex == Inve.HandTwoIndex)
                    {
                        RemoveItem();
                    }
                    else
                    {
                        PutItem(item, Inve.HandTwoIndex);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Game.GameManager.t.DamageBloco(500);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Game.GameManager.t.PlaceBlockSet(Game.GameManager.t.type);
        }

        if (MouselockFake.IsLock == false && CurrentItem != null && Distance <= CurrentItem.About.Distance && OnHand == false)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                FirstAction();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                if (Game.GameManager.hit.collider != null)
                {
                    if (Get.OpenInveTile(Game.GameManager.t))
                    {
                        
                    }
                    else
                    {
                        SecondAction();
                    }
                }
                else
                {
                    SecondAction();
                }
            }
        }
        else if (MouselockFake.IsLock == false && OnHand == true && Distance <= Hand.Distance)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                FirstAction();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                FirstAction();
            }
        }
    }
}

[System.Serializable]
public class ItemHands
{
    public GameObject Item;
    public int SlotIndex = -1;
    
}

[System.Serializable]
public class HandData
{
    public int DamageBlock = 1;
    public int DamageEnity = 1;
    public int DamagePlayer = 1;

    public int Distance = 3;

    public float FireRate = 0.4f;

    public MaterialHitType MaterialHitBest = MaterialHitType.Entity;
}

public class FPItemBase : MonoBehaviour
{
    private ItemData ThisItem;

    public void SetUpData(ItemData item)
    {
        ThisItem = item;
    }

    public ItemData GetItem()
    {
        return ThisItem;
    }
}