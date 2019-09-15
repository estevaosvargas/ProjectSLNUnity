using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public ItemData CurrentItem;
    public SpriteRenderer HandRender;
    public static HandManager MyHand;
    public Inventory Inve;
    public bool OnHand = false;
    public int SlotIndex = -1;
    public HandData Hand;
    public Transform HandTransform;

    float timetemp;

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
        CurrentItem = item;
        HandRender.sprite = item.Icon;
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
        HandRender.sprite = null;
        CurrentItem = null;
        SlotIndex = -1;
    }

    public void Attack()
    {
        if (CurrentItem.ITEMTYPE == ItemType.Weapon)
        {
            
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

    public void Harvest()
    {
        if (Game.GameManager.hit.collider != null)
        {
            if (Game.GameManager.hit.collider.tag == "Tree" || Game.GameManager.hit.collider.tag == "TreeTrigger")
            {
                if (CurrentItem != null && CurrentItem.ITEMTYPE == ItemType.Tools)
                {
                    if (Time.time > timetemp + CurrentItem.About.FireRate)
                    {
                        if (Game.GameManager.hit.collider.GetComponent<Trees>())
                        {
                            Game.GameManager.hit.collider.GetComponent<Trees>().DoDamage(CurrentItem.About.BlockDamage);
                        }
                        else if (Game.GameManager.hit.collider.GetComponentInChildren<Trees>())
                        {
                            Game.GameManager.hit.collider.GetComponentInChildren<Trees>().DoDamage(CurrentItem.About.BlockDamage);
                        }
                        HandTransform.GetComponent<Animator>().SetTrigger("Play");
                        timetemp = Time.time;
                    }
                }
                else if (OnHand)
                {
                    if (Time.time > timetemp + Hand.FireRate)
                    {
                        if (Game.GameManager.hit.collider.GetComponent<Trees>())
                        {
                            Game.GameManager.hit.collider.GetComponent<Trees>().DoDamage(Hand.DamageBlock);
                        }
                        else if (Game.GameManager.hit.collider.GetComponentInChildren<Trees>())
                        {
                            Game.GameManager.hit.collider.GetComponentInChildren<Trees>().DoDamage(Hand.DamageBlock);
                        }
                        HandTransform.GetComponent<Animator>().SetTrigger("Play");
                        timetemp = Time.time;
                    }
                }
            }
            else
            {
                if (CurrentItem != null && CurrentItem.ITEMTYPE == ItemType.Tools)
                {
                    if (Time.time > timetemp + CurrentItem.About.FireRate)
                    {
                        Game.GameManager.t.DamageBloco(CurrentItem.About.BlockDamage);
                        HandTransform.GetComponent<Animator>().SetTrigger("Play");
                        timetemp = Time.time;
                    }
                }
                else if (OnHand)
                {
                    if (Time.time > timetemp + Hand.FireRate)
                    {
                        Game.GameManager.t.DamageBloco(Hand.DamageBlock);
                        HandTransform.GetComponent<Animator>().SetTrigger("Play");
                        timetemp = Time.time;
                    }
                }
            }
        }
        else
        {
            if (CurrentItem != null && CurrentItem.ITEMTYPE == ItemType.Tools)
            {
                if (Time.time > timetemp + CurrentItem.About.FireRate)
                {
                    Game.GameManager.t.DamageBloco(CurrentItem.About.BlockDamage);
                    HandTransform.GetComponent<Animator>().SetTrigger("Play");
                    timetemp = Time.time;
                }
            }
            else if (OnHand)
            {
                if (Time.time > timetemp + Hand.FireRate)
                {
                    Game.GameManager.t.DamageBloco(Hand.DamageBlock);
                    HandTransform.GetComponent<Animator>().SetTrigger("Play");
                    timetemp = Time.time;
                }
            }
        }
    }

    float Distance = 0;

    void Update()
    {
        Distance = Vector2.Distance(Camera.main.transform.position, new Vector2(Game.GameManager.mouseX, Game.GameManager.mouseY));

        if (Distance <= 3)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (GetPresets.OpenInveTile(Game.GameManager.t))
                {
                    Game.MenuManager.OpenInveContainer(Game.GameManager.t.ObjThis.GetComponent<Inventory>());
                }
            }
        }

        if (HandTransform.GetComponent<Animator>())
        {
            HandTransform.GetComponent<Animator>().SetInteger("Side", GetComponent<EntityPlayer>().NetStats.Side);
        }

        if (MouselockFake.IsLock == false && CurrentItem != null && Distance <= CurrentItem.About.Distance && OnHand == false)
        {
            if (Game.GameManager.t != null)
            {
                if (WorldGenerator.Instance.SlectedBlock != null)
                {
                    if (CurrentItem.ITEMTYPE == ItemType.Block || CurrentItem.ITEMTYPE == ItemType.Tools || CurrentItem.ITEMTYPE == ItemType.Placer)
                    {
                        WorldGenerator.Instance.SlectedBlock.gameObject.SetActive(true);
                        WorldGenerator.Instance.SlectedBlock.position = new Vector3(Game.GameManager.t.x, Game.GameManager.t.y, 0);
                    }
                    else
                    {
                        WorldGenerator.Instance.SlectedBlock.gameObject.SetActive(false);
                    }
                }
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (Game.GameManager.hit.collider != null)
                {
                    if (Game.GameManager.hit.collider.tag == "Tree")
                    {
                        Harvest();
                    }
                    else if (Game.GameManager.hit.collider.tag == "TreeTrigger")
                    {
                        Harvest();
                    }
                    else if (Game.GameManager.hit.collider.tag == "Entity")
                    {
                        if (Time.time > timetemp + CurrentItem.About.FireRate)
                        {
                            Game.GameManager.PopUpDamage(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0), CurrentItem.About.EntityDamage);

                            HandTransform.GetComponent<Animator>().SetTrigger("Play");
                            Game.GameManager.hit.collider.GetComponentInParent<EntityLife>().DoDamage(CurrentItem.About.EntityDamage, "Player", true);
                            timetemp = Time.time;
                        }
                    }
                    else
                    {
                        Harvest();
                    }
                }
                else
                {
                    Harvest();
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (Game.GameManager.hit.collider != null)
                {
                    if (GetPresets.OpenInveTile(Game.GameManager.t))
                    {
                        
                    }
                    else
                    {
                        #region PlaceBlock EnterCave
                        if (Game.GameManager.t.type == TypeBlock.RockHole || Game.GameManager.t.type == TypeBlock.RockHoleUp || Game.GameManager.t.type == TypeBlock.RockHoleDown)
                        {
                            if (Application.loadedLevel == 2)
                            {
                                WorldManager.This.ChangeWorld("Map", Game.GameManager.t.x, Game.GameManager.t.y);
                            }
                            else
                            {
                                WorldManager.This.ChangeWorld("Cave", Game.GameManager.t.x, Game.GameManager.t.y);
                            }
                            return;
                        }
                        else if (Game.GameManager.t.type == TypeBlock.LightBlockON)
                        {
                            
                            return;
                        }
                        else
                        {
                            PlaceBlock();
                        }
                        #endregion
                    }
                }
                else
                {
                    #region PlaceBlock EnterCave
                    if (Game.GameManager.t.type == TypeBlock.RockHole || Game.GameManager.t.type == TypeBlock.RockHoleUp || Game.GameManager.t.type == TypeBlock.RockHoleDown)
                    {
                        if (Application.loadedLevel == 2)
                        {
                            WorldManager.This.ChangeWorld("Map", Game.GameManager.t.x, Game.GameManager.t.y);
                        }
                        else
                        {
                            WorldManager.This.ChangeWorld("Cave", Game.GameManager.t.x, Game.GameManager.t.y);
                        }
                        return;
                    }
                    else if (Game.GameManager.t.type == TypeBlock.LightBlockON)
                    {

                        return;
                    }
                    else
                    {
                        PlaceBlock();
                    }
                    #endregion
                }
            }
        }
        else if (MouselockFake.IsLock == false && OnHand == true && Distance <= Hand.Distance)
        {
            if (Game.GameManager.t != null)
            {
                if (WorldGenerator.Instance.SlectedBlock != null)
                {
                    WorldGenerator.Instance.SlectedBlock.gameObject.SetActive(true);
                    WorldGenerator.Instance.SlectedBlock.position = new Vector3(Game.GameManager.t.x, Game.GameManager.t.y, 0);
                }
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (Game.GameManager.hit.collider != null)
                {
                    if (Game.GameManager.hit.collider.tag == "Tree")
                    {
                        Harvest();
                    }
                    else if (Game.GameManager.hit.collider.tag == "TreeTrigger")
                    {
                        Harvest();
                    }
                    else if (Game.GameManager.hit.collider.tag == "Entity")
                    {
                        if (Time.time > timetemp + Hand.FireRate)
                        {
                            Game.GameManager.PopUpDamage(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0), Hand.DamageEnity);

                            HandTransform.GetComponent<Animator>().SetTrigger("Play");
                            Game.GameManager.hit.collider.GetComponentInParent<EntityLife>().DoDamage(Hand.DamageEnity, "Player", true);
                            timetemp = Time.time;
                        }
                    }
                    else
                    {
                        Harvest();
                    }
                }
                else
                {
                    Harvest();
                }
            }
        }
        else
        {
            if (WorldGenerator.Instance)
            {
                if (WorldGenerator.Instance.SlectedBlock != null)
                {
                    WorldGenerator.Instance.SlectedBlock.gameObject.SetActive(false);
                }
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
}