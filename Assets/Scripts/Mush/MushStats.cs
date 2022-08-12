using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MushStats
{
    public float value;

    public string name;

    public float statIncreaseAmount = 0.1f;

    public GameObject statButtonPrefab;

    private float initialValue;

    public MushStats(float value, string name)
    {
        this.value = value;
        this.initialValue = value;
        this.name = name;
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

    public string GetName()
    {
        return name;
    }

    public void ResetValueToInitial()
    {
        this.value = initialValue;
    }

}