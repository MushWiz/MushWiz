using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MushStats
{
    public float value;


    public StatType statType;

    public float statIncreaseAmount = 0.1f;

    public GameObject statButtonPrefab;

    private float initialValue;

    public MushStats(float value, StatType statType)
    {
        this.value = value;
        this.initialValue = value;
        this.statType = statType;
    }

    public void IncreaseValue(float value = -1)
    {
        if (value < 0)
        {
            value = statIncreaseAmount;
        }
        this.value += value;
    }

    public void SetValue(float value)
    {
        this.value = value;
    }

    public float GetValue()
    {
        return value;
    }

    public float GetValueIncrease()
    {
        return statIncreaseAmount;
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