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
            controller.navMeshAgent.isStopped = false;
            controller.transform.rotation = Quaternion.identity;
            controller.transform.position = new Vector3(controller.navMeshAgent.nextPosition.x, controller.navMeshAgent.nextPosition.y, 0);
            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending)
            {
                Vector2 newDestination = Random.insideUnitCircle * 4 + controller.homeBase;
                controller.navMeshAgent.destination = newDestination;
            }

            return;
        }
        if (controller.patrolPoints[controller.wayPointListIndex] == null)
        {
            Debug.LogError("Patrol point is null");
            return;
        }
        controller.navMeshAgent.destination = controller.patrolPoints[controller.wayPointListIndex].transform.position;
        controller.navMeshAgent.isStopped = false;
        controller.transform.rotation = Quaternion.identity;
        controller.transform.position = new Vector3(controller.navMeshAgent.nextPosition.x, controller.navMeshAgent.nextPosition.y, 0);

        if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending)
        {
            controller.wayPointListIndex = (controller.wayPointListIndex + 1) % controller.patrolPoints.Count;
        }

        AdjustAnimation(controller);
    }

    public void AdjustAnimation(MonsterStateController controller)
    {
        Animator animator = controller.animator;
        Vector2 direction = (controller.patrolPoints[controller.wayPointListIndex].transform.position - controller.transform.position).normalized;

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