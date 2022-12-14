using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
public class MonsterActionPatrol : MonsterAction
{
    public override void Act(MonsterStateController controller)
    {
        if (controller.patrolPoints.Count == 0)
        {
            controller.navMeshAgent.stoppingDistance = 0;
            controller.navMeshAgent.isStopped = false;
            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending)
            {
                Vector2 newDestination = Random.insideUnitCircle * 4 + controller.homeBase;
                controller.navMeshAgent.destination = newDestination;
            }
            AdjustAnimation(controller);
            return;
        }
        if (controller.patrolPoints[controller.wayPointListIndex] == null)
        {
            Debug.LogError("Patrol point is null");
            return;
        }
        controller.navMeshAgent.destination = controller.patrolPoints[controller.wayPointListIndex].transform.position;
        controller.navMeshAgent.isStopped = false;

        if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending)
        {
            controller.wayPointListIndex = (controller.wayPointListIndex + 1) % controller.patrolPoints.Count;
        }

        AdjustAnimation(controller);
    }

    public void AdjustAnimation(MonsterStateController controller)
    {
        Animator animator = controller.animator;
        Vector2 direction = (controller.navMeshAgent.destination - controller.transform.position).normalized;

        string prefix = controller.animationPrefix;

        string state = prefix + "WalkRight";
        if (direction.x > 0)
        {
            state = prefix + "WalkRight";
        }
        else if (direction.x < 0)
        {
            state = prefix + "WalkLeft";
        }
        animator.CrossFade(state, 0, 0);
    }

}