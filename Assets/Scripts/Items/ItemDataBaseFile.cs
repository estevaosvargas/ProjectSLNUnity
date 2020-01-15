using System.Collections.Generic;
using UnityEngine;
public class ItemDataBaseFile
{
    public static Dictionary<int, ItemData> LoadItemDataBase()
    {
        Debug.Log("Loading ItemData...");
        Dictionary<int, ItemData> itemlist = new Dictionary<int, ItemData>();

        itemlist.Add(0, SetUpItem(0, "Wood", "Wood Log Taked from a tree", Game.SpriteManager.Getitemicon("Wood"), ItemRarity.Common, 150, true, false, ItemType.none, MaterialHitType.none, new ItemAbout()));
        itemlist.Add(1, SetUpItem(1, "PickAxe", "PickAxe PickAxe PickAxe PickAxe", Game.SpriteManager.Getitemicon("PickAxe"), ItemRarity.Common, 1, false, true, ItemType.Tools, MaterialHitType.Rock, new ItemAbout(25, 45, 45, 100, 2, 0.7f)));
        itemlist.Add(2, SetUpItem(2, "Wood Chest", "ChestChestChestChestChestChestChest", Game.SpriteManager.Getitemicon("ChestWood"), ItemRarity.Common, 150, true, true, ItemType.Placer, MaterialHitType.none, new ItemAbout(Placer.BauWood, 2)));
        itemlist.Add(3, SetUpItem(3, "Axe", "Is Good for take wood, cut down a tree!", Game.SpriteManager.Getitemicon("Axe"), ItemRarity.Common, 1, false, true, ItemType.Tools, MaterialHitType.Wood, new ItemAbout(35, 15, 15, 100, 2, 0.7f)));
        itemlist.Add(4, SetUpItem(4, "GoldCoin", "GoldCoin Is a global coin to trade/buy.", Game.SpriteManager.Getitemicon("GoldCoin"), ItemRarity.Money, 1000, true, false, ItemType.Trade, MaterialHitType.none, new ItemAbout()));
        itemlist.Add(5, SetUpItem(5, "SilverCoin", "SilverCoin Is a global coin to trade/buy.", Game.SpriteManager.Getitemicon("SilverCoin"), ItemRarity.Money, 1000, true, false, ItemType.Trade, MaterialHitType.none, new ItemAbout()));
        itemlist.Add(6, SetUpItem(6, "CoperCoin", "CoperCoin Is a global coin to trade/buy.", Game.SpriteManager.Getitemicon("CoperCoin"), ItemRarity.Money, 1000, true, false, ItemType.Trade, MaterialHitType.none, new ItemAbout()));
        itemlist.Add(7, SetUpItem(7, "HeadTeste", "HeadTeste", Game.SpriteManager.Getitemicon("Head"), ItemRarity.Legendary, 1, false, false, ItemType.Armor, MaterialHitType.none, new ItemAbout()));
        itemlist.Add(8, SetUpItem(8, "Book", "You can lern alot of things with books!", Game.SpriteManager.Getitemicon("Book"), ItemRarity.Common, 5, true, false, ItemType.none, MaterialHitType.none, new ItemAbout()));
        itemlist.Add(9, SetUpItem(9, "Magic Book", "You can lern alot of things with books!", Game.SpriteManager.Getitemicon("MagicalBook"), ItemRarity.Legendary, 5, true, false, ItemType.none, MaterialHitType.none, new ItemAbout()));
        itemlist.Add(10, SetUpItem(10, "Dark Magic Book", "You can lern alot of things with books!", Game.SpriteManager.Getitemicon("DarkBook"), ItemRarity.Dark, 5, true, false, ItemType.none, MaterialHitType.none, new ItemAbout()));

        Debug.Log("ItemData Loading Finished!");
        return itemlist;
    }
    #region SetUpItemVoid
    static ItemData SetUpItem(int index, string namestring, string description, Sprite Icon, ItemRarity itemRarity, int MaxAmount, bool Stack, bool canEquip, ItemType itemType, MaterialHitType MaterialHit_Best, ItemAbout About)
    {
        ItemData item = new ItemData();

        item.Index = index;

        item.Name = namestring;
        item.Description = description;

        item.Icon = Icon;
        item.itemRarity = itemRarity;
        item.ITEMTYPE = itemType;
        item.MaterialHitBest = MaterialHit_Best;
        item.MaxAmount = MaxAmount;
        item.Stack = Stack;

        item.CanEquip = canEquip;

        item.About = About;

        return item;
    }
    #endregion
}
