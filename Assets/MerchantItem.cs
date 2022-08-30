using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MerchantItem", menuName = "MushWiz/MerchantItem", order = 0)]
public class MerchantItem : ScriptableObject
{
    public string itemName;
    public int sellAmount;
    public bool infiniteAmount = false;
    public bool canBuy = false;
    public int miceliumRequirement;

    public Item itemSold;
}

