using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Melee")]
public class MonsterActionMelee : MonsterAction
{
    public override void Act(MonsterStateController controller)
    {
        controller.transform.rotation = Quaternion.identity;
        controller.transform.position = new Vector3(controller.navMeshAgent.nextPosition.x, controller.navMeshAgent.nextPosition.y, 0);

        if (controller.CheckIfCountDownElapsed(controller.monsterController.attackRate))
        {
            controller.navMeshAgent.isStopped = true;
            controller.stateTimeElapsed = 0;
            controller.monsterController.StartCoroutine(controller.monsterController.MeleeAnimation(controller.target.transform.position, controller.monsterController.attackRate * 0.85f));
        }
    }
}