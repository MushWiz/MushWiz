using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item/StatBoost")]
public class StatBoost : Item
{
    public StatType stat;
    public int boostAmount = 1;
}