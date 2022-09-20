using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decision/MonsterDamaged")]
public class MonsterDecisionDamaged : MonsterDecision
{
    public override bool Decide(MonsterStateController controller)
    {
        if (controller.monsterController.isInvincible)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}