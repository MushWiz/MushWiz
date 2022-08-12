using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public Sprite itemIcon;
    public string itemName;
    public MushInventoryType itemType;

    public GameObject collectiblePrefab;

    public virtual void CreateCollectible(Transform position)
    {
        GameObject collectible = Instantiate(collectiblePrefab, position.position, Quaternion.identity) as GameObject;
        collectible.GetComponent<Collectible>().item = this;
    }

    public virtual void Use(MushController mushController, MushEquipment equipmentSlot)
    {
        Debug.Log("Using " + itemName);
    }

    public virtual void Discard(MushController mushController, MushEquipment equipmentSlot)
    {
        Debug.Log("Discarding " + itemName);
    }

    public virtual void Equip(MushController mushController, MushEquipment equipmentSlot)
    {
        Debug.Log("Equipping " + itemName);
    }

    public virtual void Unequip(MushController mushController, MushEquipment equipmentSlot)
    {
        Debug.Log("Unequipping " + itemName);
    }

    public virtual void OnPickup(MushController mushController, MushEquipment equipmentSlot)
    {
        Debug.Log("Picking up " + itemName);
    }

    public virtual void OnDrop(MushController mushController, MushEquipment equipmentSlot)
    {
        Debug.Log("Dropping " + itemName);
    }

    public virtual void OnEquip(MushController mushController, MushEquipment equipmentSlot)
    {
        Debug.Log("Equipping " + itemName);
    }

    public virtual void OnUnequip(MushController mushController, MushEquipment equipmentSlot)
    {
        Debug.Log("Unequipping " + itemName);
    }
}
