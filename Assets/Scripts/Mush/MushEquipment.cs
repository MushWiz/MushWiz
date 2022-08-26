using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MushEquipment
{
    public MushInventoryType type;
    public Sprite icon;
    public Item item;

    public MushEquipment(MushInventoryType type = MushInventoryType.Item, Sprite icon = null, Item item = null)
    {
        this.type = type;
        this.icon = icon;
        this.item = item;
    }
}

public enum MushInventoryType
{
    Item,
    Weapon,
    Armor,
    Accessory,
    Money,
}