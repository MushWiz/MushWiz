using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Melee")]
public class MonsterActionMelee : MonsterAction
{
    public override void Act(MonsterStateController controller)
    {
        if (controller.CheckIfCountDownElapsed(controller.monsterController.attackRate))
        {
            controller.navMeshAgent.isStopped = true;
            controller.stateTimeElapsed = 0;
            controller.monsterController.StartCoroutine(controller.monsterController.MeleeAnimation(controller.target.transform.position, controller.monsterController.attackRate));
        }
    }
}