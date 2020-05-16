using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[System.Serializable]
public class ItemData
{
    public int Index = 0;
    [XmlIgnore]
    public ItemRarity itemRarity = ItemRarity.Common;
    public ItemType ITEMTYPE = ItemType.none;
    public MaterialHitType MaterialHitBest = MaterialHitType.all;
    public Sprite Icon;
    public string Name = "";
    [TextArea(0, 200)]
    public string Description = "";
    public int MaxAmount = 100;
    public bool Stack = false;
    public ItemAbout About;

    public bool CanEquip = false;

    public ItemData(int index, Sprite icon, string name, string description, int maxamount, bool stack)
    {
        Index = index;
        Icon = icon;
        Name = name;
        Description = description;
        MaxAmount = maxamount;
        Stack = stack;
    }

    public ItemData()
    {

    }
}
[System.Serializable]
public class ItemAbout
{
    public TypeBlock BlockType;
    public Placer placer;

    public int BlockDamage = -1;
    public int EntityDamage = -1;
    public int PlayerDamage = -1;

    public int DurabilityDefault = 100;
    public int Distance = 3;

    public float FireRate = 0.4f;

    public ItemAbout(TypeBlock blockType, Placer placerObj, int blockDamage, int bntityDamage, int playerDamage, int durabilityDefault, int distance, float fireRate)
    {
        BlockType = blockType;
        placer = placerObj;

        BlockDamage = blockDamage;
        EntityDamage = bntityDamage;
        PlayerDamage = playerDamage;

        DurabilityDefault = durabilityDefault;
        Distance = distance;

        FireRate = fireRate;
    }
    public ItemAbout(int blockDamage, int bntityDamage, int playerDamage, int durabilityDefault, int distance, float fireRate)
    {
        BlockType = TypeBlock.Air;
        placer = Placer.empty;

        BlockDamage = blockDamage;
        EntityDamage = bntityDamage;
        PlayerDamage = playerDamage;

        DurabilityDefault = durabilityDefault;
        Distance = distance;

        FireRate = fireRate;
    }
    public ItemAbout(TypeBlock blockType, int distance)
    {
        BlockType = blockType;
        placer = Placer.empty;

        BlockDamage = -1;
        EntityDamage = -1;
        PlayerDamage = -1;

        DurabilityDefault = 1;
        Distance = distance;

        FireRate = 1;
    }
    public ItemAbout(Placer placertype, int distance)
    {
        placer = placertype;
        BlockType = TypeBlock.Air;

        BlockDamage = -1;
        EntityDamage = -1;
        PlayerDamage = -1;

        DurabilityDefault = 1;
        Distance = distance;

        FireRate = 1;
    }

    public ItemAbout()
    {

    }
}

public class ItemManager : MonoBehaviour {

    public Dictionary<int, ItemData> ItemList = new Dictionary<int, ItemData>();

    public static ItemManager Instance;

    public GameObject Drop;

    private void Start()
    {
        Instance = this;

        ItemList = ItemDataBaseFile.LoadItemDataBase();
    }

    /// <summary>
    /// Get Item by index/id (int)
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ItemData GetItem(int index)
    {
        if (ItemList.ContainsKey(index))
        {
            return ItemList[index];
        }

        Debug.LogError("Nao encontrado este item : " + index);
        return null;
    }

    /// <summary>
    /// Get Item by name(String)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ItemData GetItem(string name)
    {
       foreach (var item in ItemList.Values)
        {
            if (item.Name == name)
            {
                return item;
            }
        }

        Debug.LogError("Nao encontrado este item : " + name);
        return null;
    }

    public void SpawnItem(int index, int quanty)
    {
        ItemData item = ItemManager.Instance.GetItem(index);

        GameObject obj = GameObject.Instantiate(Drop, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0), Quaternion.identity);

        obj.GetComponent<ItemDrop>().SetDrop(item, quanty);
    }

    public void SpawnItem(int index, int quanty, Vector3 world_position)
    {
        ItemData item = ItemManager.Instance.GetItem(index);

        GameObject obj = DarckNet.Network.Instantiate(Drop, world_position, Quaternion.identity, Game.MapManager.World_ID);

        obj.GetComponent<ItemDrop>().SetDrop(item, quanty);
    }
}

[System.Serializable]
public enum ItemRarity : byte
{
    SelfMade, Common, Uncommon, Epic, Legendary, Dark, Money
}

public enum ItemType
{
    none, Weapon, Tools, Armor, Block, Placer, Trade, Head, Torso, Pants, Foot, Neck, Cape,
    Bag,
    Ring,
    Lag,
    Face
}
