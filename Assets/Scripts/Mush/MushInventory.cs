using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushInventory : MonoBehaviour
{
    MushController mushController;
    public ItemDatabase database;
    public List<MushEquipment> items = new List<MushEquipment>();
    public List<MushEquipment> equipments = new List<MushEquipment>();

    private void Awake()
    {
        mushController = GetComponent<MushController>();
    }

    public bool AddToInventory(Item item)
    {
        if (item.itemType == MushInventoryType.Item)
        {
            return AddItem(item);
        }
        else
        {
            return AddEquipment(item);
        }
    }

    public bool AddItem(Item item)
    {
        //find a spot in the inventory
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == null)
            {
                items[i].item = item;
                items[i].icon = item.itemIcon;
                item.OnPickup(mushController, items[i]);
                return true;
            }
        }
        //if no spot found, warn the player that the inventory is full
        Debug.Log("Inventory is full");
        return false;
    }

    public bool AddEquipment(Item item)
    {
        //Find a spot in the equipment list that is not currently equipped by the same type
        for (int i = 0; i < equipments.Count; i++)
        {
            if (equipments[i].item == null && equipments[i].type == item.itemType)
            {
                equipments[i].item = item;
                equipments[i].icon = item.itemIcon;
                item.OnEquip(mushController, equipments[i]);
                return true;
            }
        }
        //if no spot found, put the item in the items
        return AddItem(item);
    }

    public void AddItemToSlot(Item item, int slot)
    {
        items[slot].item = item;
        items[slot].icon = item.itemIcon;
        item.OnPickup(mushController, items[slot]);
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                items[i].item = null;
                items[i].icon = null;
                item.OnDrop(mushController, items[i]);
                return;
            }
        }
    }

    public void RemoveEquipment(Item item)
    {
        for (int i = 0; i < equipments.Count; i++)
        {
            if (equipments[i].item == item)
            {
                equipments[i].item = null;
                equipments[i].icon = null;
                item.OnUnequip(mushController, equipments[i]);
                return;
            }
        }
    }

}
