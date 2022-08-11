using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterStateTransition
{
    public MonsterDecision decision;
    public MonsterState trueState;
    public MonsterState falseState;
}