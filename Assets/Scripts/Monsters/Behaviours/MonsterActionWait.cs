using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Wait")]
public class MonsterActionWait : MonsterAction
{
    public override void Act(MonsterStateController controller)
    {
        controller.navMeshAgent.isStopped = true;
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