using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decision/TargetFound")]
public class MonsterDecisionTargetFound : MonsterDecision
{
    public override bool Decide(MonsterStateController controller)
    {
        if (Vector2.Distance(controller.target.transform.position, controller.transform.position) <= controller.monsterController.chaseRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}