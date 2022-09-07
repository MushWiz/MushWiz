using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Stats
{
    public int value;


    public StatType statType;

    public GameObject statButtonPrefab;

    private int initialValue;

    public Stats(int value, StatType statType)
    {
        this.value = value;
        this.initialValue = value;
        this.statType = statType;
    }

    public void IncreaseValue(int value = 1)
    {
        this.value += value;
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

    public int GetValue()
    {
        return value;
    }

    public StatType GetStatType()
    {
        return statType;
    }

    public void ResetValueToInitial()
    {
        this.value = initialValue;
    }

}

public enum StatType
{
    Health,
    Mana,
    Stamina,
    Strength,
    Agility,
    Intelligence,
    Luck,
    Speed,
    Defense,
    Resistance,
    Accuracy,
    Evasion,
    CritChance,
    CritDamage,
    BlockChance,
    BlockAmount,
    BlockDamage,
    BlockReflect
}