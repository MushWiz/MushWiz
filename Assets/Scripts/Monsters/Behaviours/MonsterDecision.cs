using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterDecision : ScriptableObject
{
    public abstract bool Decide(MonsterStateController controller);
}