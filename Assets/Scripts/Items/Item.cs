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

    public virtual void Use(MushController mushController)
    {
        Debug.Log("Using " + itemName);
    }

    public virtual void Discard(MushController mushController)
    {
        Debug.Log("Discarding " + itemName);
    }

    public virtual void Equip(MushController mushController)
    {
        Debug.Log("Equipping " + itemName);
    }

    public virtual void Unequip(MushController mushController)
    {
        Debug.Log("Unequipping " + itemName);
    }

    public virtual void OnPickup(MushController mushController)
    {
        Debug.Log("Picking up " + itemName);
    }

    public virtual void OnDrop(MushController mushController)
    {
        Debug.Log("Dropping " + itemName);
    }

    public virtual void OnEquip(MushController mushController)
    {
        Debug.Log("Equipping " + itemName);
    }

    public virtual void OnUnequip(MushController mushController)
    {
        Debug.Log("Unequipping " + itemName);
    }
}
