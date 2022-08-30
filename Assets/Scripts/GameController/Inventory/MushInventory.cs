using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MushInventory : MonoBehaviour
{
    public MushController mushController;
    public ItemDatabase database;

    public MushInventoryItemOverlay itemOverlay;

    public MushInventorySlot currentWeaponSlot;
    public Item defaultWeapon;

    public int miceliumAmount = 0;
    public List<MushInventorySlot> inventorySlots = new List<MushInventorySlot>();

    public bool dragging = false;

    private void Start()
    {
        HideOverlay();
    }

    public bool AddToInventory(Item item)
    {
        if (item.itemType == MushInventoryType.Item)
        {
            return AddItem(item);
        }

        else if (item.itemType == MushInventoryType.Money)
        {
            return AddCurrency((CoinItem)item);
        }

        else if (item.itemType == MushInventoryType.StatBoost)
        {
            return IncreaseStat((StatBoost)item);
        }

        else
        {
            return AddEquipment(item);
        }
    }

    public bool AddCurrency(CoinItem item)
    {
        if (item == null)
        {
            Debug.LogError("Item is null");
            return false;
        }

        miceliumAmount += item.value;

        return true;
    }

    public bool UseCurrency(int amountToUse)
    {
        if (miceliumAmount < amountToUse)
        {
            Debug.Log("Not enough micelium!");
            return false;
        }

        miceliumAmount -= amountToUse;
        return true;
    }

    public bool IncreaseStat(StatBoost boost)
    {
        return mushController.IncreaseStatValue(boost.stat, boost.boostAmount);
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
            return false;
        }
        if (currentWeaponSlot.itemEquipment.item == null)
        {
            currentWeaponSlot.itemEquipment.item = item;
            currentWeaponSlot.itemEquipment.icon = item.itemIcon;
            currentWeaponSlot.inventoryIcon = item.itemIcon;
            currentWeaponSlot.inventorySlot.GetComponent<Image>().sprite = item.itemIcon;
            item.OnEquip(mushController, currentWeaponSlot.itemEquipment);
            return true;
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
        if (currentWeaponSlot.itemEquipment.item == item)
        {
            currentWeaponSlot.itemEquipment.item = null;
            currentWeaponSlot.itemEquipment.icon = null;
            currentWeaponSlot.inventoryIcon = null;
            currentWeaponSlot.inventorySlot.GetComponent<Image>().sprite = null;
            item.OnUnequip(mushController, currentWeaponSlot.itemEquipment);
        }
    }

    public void RemoveEquipment()
    {
        currentWeaponSlot.itemEquipment.item = null;
        currentWeaponSlot.itemEquipment.icon = null;
        currentWeaponSlot.inventoryIcon = null;
        currentWeaponSlot.inventorySlot.GetComponent<Image>().sprite = null;
        mushController.gameObject.GetComponentInChildren<MushWeaponHolder>().UnequipWeapon();
    }

    public void SwapInventories(GameObject inventorySlot1, GameObject inventorySlot2)
    {

        if (inventorySlot1 == null || inventorySlot2 == null)
        {
            return;
        }

        if (inventorySlot1 == inventorySlot2)
        {
            return;
        }

        if (inventorySlot1 == currentWeaponSlot.inventorySlot || inventorySlot2 == currentWeaponSlot.inventorySlot)
        {
            SwapEquipment(inventorySlot1, inventorySlot2);
            return;
        }

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

    public void SwapEquipment(GameObject inventorySlot1, GameObject inventorySlot2)
    {
        int inventorySlot = -1;

        if (inventorySlot1 == currentWeaponSlot.inventorySlot)
        {
            foreach (MushInventorySlot slot in inventorySlots)
            {
                if (slot.inventorySlot == inventorySlot2)
                {
                    inventorySlot = inventorySlots.IndexOf(slot);
                }
            }
        }
        else
        {
            foreach (MushInventorySlot slot in inventorySlots)
            {
                if (slot.inventorySlot == inventorySlot1)
                {
                    inventorySlot = inventorySlots.IndexOf(slot);
                }
            }
        }

        Item tempInventory = inventorySlots[inventorySlot].itemEquipment.item;
        Item tempEquipment = currentWeaponSlot.itemEquipment.item;

        RemoveEquipment();
        RemoveItem(inventorySlot);

        AddEquipment(tempInventory);
        AddItem(tempEquipment, inventorySlot);

    }

    public void ShowOverlay(GameObject inventorySlot)
    {
        MushInventorySlot slot = null;
        foreach (MushInventorySlot checkedSlot in inventorySlots)
        {
            if (checkedSlot.inventorySlot == inventorySlot)
            {
                slot = checkedSlot;
            }
        }

        if (slot == null)
        {
            slot = currentWeaponSlot;
        }

        if (slot == null)
        {
            return;
        }

        itemOverlay.ShowOverlay(slot.itemEquipment.item);
    }

    public void HideOverlay()
    {
        itemOverlay.HideOverlay();
    }

}
