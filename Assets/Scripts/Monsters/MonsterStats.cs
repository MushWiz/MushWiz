using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MonsterStats
{
    public float value;

    public string name;

    public MonsterStats(float value, string name)
    {
        this.value = value;
        this.name = name;
    }
}