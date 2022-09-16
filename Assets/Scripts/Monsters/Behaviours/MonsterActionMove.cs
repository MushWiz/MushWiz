using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
public class MonsterActionMove : MonsterAction
{
    public override void Act(MonsterStateController controller)
    {
        if (Vector2.Distance(controller.transform.position, controller.target.transform.position) > controller.monsterController.attackRange)
        {
            controller.monsterController.isAttacking = false;
        }
        controller.navMeshAgent.destination = controller.target.transform.position;
        controller.navMeshAgent.isStopped = false;
        AdjustAnimation(controller);
    }

    public void AdjustAnimation(MonsterStateController controller)
    {
        Animator animator = controller.animator;
        Vector2 direction = (controller.target.transform.position - controller.transform.position).normalized;

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
