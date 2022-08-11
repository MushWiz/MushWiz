using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decision/TargetAttack")]
public class MonsterDecisionTargetAttack : MonsterDecision
{
    public override bool Decide(MonsterStateController controller)
    {
        if (Vector2.Distance(controller.target.transform.position, controller.transform.position) <= controller.monsterController.attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}