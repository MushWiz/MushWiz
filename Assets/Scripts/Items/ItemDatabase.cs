using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Database")]
public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public Item[] items;
    public Dictionary<Item, int> itemsIDs = new Dictionary<Item, int>();

    public void OnAfterDeserialize()
    {
        itemsIDs = new Dictionary<Item, int>();
        for (int i = 0; i < items.Length; i++)
        {
            itemsIDs.Add(items[i], i);
            Debug.Log(items[i].itemName + " " + i);
        }
    }

    public void OnBeforeSerialize()
    {
        itemsIDs = new Dictionary<Item, int>();
        Debug.Log("Serializing");
    }
}