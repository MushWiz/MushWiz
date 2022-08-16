using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MushInventory : MonoBehaviour
{
    public MushController mushController;
    public ItemDatabase database;
    public List<MushEquipment> equipments = new List<MushEquipment>();

    public List<MushInventorySlot> inventorySlots = new List<MushInventorySlot>();

    public bool dragging = false;

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
        if (item == null)
        {
            Debug.LogError("Item is null");
            return false;
        }
        //find a spot in the inventory
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemEquipment.item == null)
            {
                inventorySlots[i].itemEquipment.item = item;
                inventorySlots[i].itemEquipment.icon = item.itemIcon;
                inventorySlots[i].inventoryIcon = item.itemIcon;
                item.OnPickup(mushController, inventorySlots[i].itemEquipment);
                inventorySlots[i].inventorySlot.GetComponent<Image>().sprite = item.itemIcon;
                return true;
            }
        }
        //if no spot found, warn the player that the inventory is full
        Debug.Log("Inventory is full");
        return false;
    }

    public bool AddEquipment(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Item is null");
            return false;
        }
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

    public void AddItem(Item item, int slot)
    {

        if (item == null)
        {
            return;
        }

        inventorySlots[slot].itemEquipment.item = item;
        inventorySlots[slot].itemEquipment.icon = item.itemIcon;
        inventorySlots[slot].inventoryIcon = item.itemIcon;
        inventorySlots[slot].inventorySlot.GetComponent<Image>().sprite = item.itemIcon;
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemEquipment.item == item)
            {
                inventorySlots[i].itemEquipment.item = null;
                inventorySlots[i].itemEquipment.icon = null;
                inventorySlots[i].inventoryIcon = null;
                item.OnDrop(mushController, inventorySlots[i].itemEquipment);
                inventorySlots[i].inventorySlot.GetComponent<Image>().sprite = null;
                return;
            }
        }
    }

    public void RemoveItem(int slot)
    {
        inventorySlots[slot].itemEquipment.item = null;
        inventorySlots[slot].itemEquipment.icon = null;
        inventorySlots[slot].inventoryIcon = null;
        inventorySlots[slot].inventorySlot.GetComponent<Image>().sprite = null;
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

    public void SwapInventories(GameObject inventorySlot1, GameObject inventorySlot2)
    {
        int firstSlot = -1;
        int secondSlot = -1;
        foreach (MushInventorySlot slot in inventorySlots)
        {
            if (slot.inventorySlot == inventorySlot1)
            {
                firstSlot = inventorySlots.IndexOf(slot);
            }
            if (slot.inventorySlot == inventorySlot2)
            {
                secondSlot = inventorySlots.IndexOf(slot);
            }
        }
        SwapInventories(firstSlot, secondSlot);
    }

    public void SwapInventories(int inventoryIndex1, int inventoryIndex2)
    {
        Item tempInventory1 = inventorySlots[inventoryIndex1].itemEquipment.item;
        Item tempInventory2 = inventorySlots[inventoryIndex2].itemEquipment.item;

        RemoveItem(inventoryIndex1);
        RemoveItem(inventoryIndex2);

        AddItem(tempInventory1, inventoryIndex2);
        AddItem(tempInventory2, inventoryIndex1);
    }

}
