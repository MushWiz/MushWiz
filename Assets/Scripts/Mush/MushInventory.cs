using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushInventory : MonoBehaviour
{
    MushController mushController;
    public List<MushEquipment> items = new List<MushEquipment>();

    public List<MushEquipment> equipments = new List<MushEquipment>();

    private void Awake()
    {
        mushController = GetComponent<MushController>();
    }

    public void AddItem(Item item)
    {
        if (item.itemType == MushInventoryType.Item)
        {
            items.Add(new MushEquipment(item.itemType, item.itemIcon, item));
        }
        else
        {
            foreach (MushEquipment equipment in equipments)
            {
                if (equipment.type != item.itemType)
                {
                    continue;
                }
                if (equipment.item != null)
                {
                    RemoveItem(equipment.item);
                }
                equipment.item = item;
                equipment.icon = item.itemIcon;
            }
        }
        item.OnPickup(mushController);
    }

    public void RemoveItem(Item item)
    {

        foreach (MushEquipment equipment in items)
        {
            if (equipment.item == item)
            {
                items.Remove(equipment);
                equipment.item.OnDrop(mushController);
                Destroy(equipment.item);
                return;
            }
        }
    }

}
