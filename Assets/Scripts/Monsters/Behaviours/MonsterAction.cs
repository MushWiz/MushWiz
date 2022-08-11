using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterAction : ScriptableObject
{
    public abstract void Act(MonsterStateController controller);
}
