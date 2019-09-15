using System.Collections.Generic;
using UnityEngine;
public class ItemDataBaseFile
{
    public static Dictionary<int, ItemData> LoadItemDataBase()
    {
        Debug.Log("Loading ItemData...");
        Dictionary<int, ItemData> itemlist = new Dictionary<int, ItemData>();

        itemlist.Add(0, SetUpItem(0, "Wood", "Wood Log Taked from a tree", SpriteManager.Getitemicon("Wood"), ItemRarity.Common, 150, true, false, ItemType.none, new ItemAbout()));
        itemlist.Add(1, SetUpItem(1, "PickAxe", "PickAxe PickAxe PickAxe PickAxe", SpriteManager.Getitemicon("PickAxe"), ItemRarity.Uncommon, 1, false, true, ItemType.Tools, new ItemAbout(35, 15, 15, 100, 2, 0.5f)));
        itemlist.Add(2, SetUpItem(2, "Wood Chest", "ChestChestChestChestChestChestChest", SpriteManager.Getitemicon("ChestWood"), ItemRarity.Common, 150, true, true, ItemType.Placer, new ItemAbout(Placer.BauWood, 2)));
        itemlist.Add(3, SetUpItem(3, "CampFire", "CampFireCampFireCampFireCampFire", SpriteManager.Getitemicon("CampFire"), ItemRarity.Common, 150, true, true, ItemType.Placer, new ItemAbout(Placer.CampFire, 2)));
        itemlist.Add(4, SetUpItem(4, "CampTend", "CampTendCampTendCampTendCampTendCampTend", SpriteManager.Getitemicon("CampTend"), ItemRarity.Common, 150, true, true, ItemType.Placer, new ItemAbout(Placer.CampTend, 2)));
        itemlist.Add(5, SetUpItem(5, "MainBuil01", "MainBuil01MainBuil01MainBuil01", SpriteManager.Getitemicon("MainBuil01"), ItemRarity.Common, 150, true, true, ItemType.Placer, new ItemAbout(Placer.MainBuild1, 2)));
        itemlist.Add(6, SetUpItem(6, "MainBuil02", "MainBuil02MainBuil02MainBuil02", SpriteManager.Getitemicon("MainBuil02"), ItemRarity.Common, 150, true, true, ItemType.Placer, new ItemAbout(Placer.MainBuild2, 2)));




        Debug.Log("ItemData Loading Finished!");
        return itemlist;
    }
    #region SetUpItemVoid
    static ItemData SetUpItem(int index, string namestring, string description, Sprite Icon, ItemRarity itemRarity, int MaxAmount, bool Stack, bool canEquip, ItemType itemType, ItemAbout About)
    {
        ItemData item = new ItemData();

        item.Index = index;

        item.Name = namestring;
        item.Description = description;

        item.Icon = Icon;
        item.itemRarity = itemRarity;
        item.ITEMTYPE = itemType;
        item.MaxAmount = MaxAmount;
        item.Stack = Stack;

        item.CanEquip = canEquip;

        item.About = About;

        return item;
    }
    #endregion
}
