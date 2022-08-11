using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunnerEnemy", menuName = "Mushroom Wizard/RunnerEnemy", order = 0)]
public class RunnerMovement : MonsterBehaviour
{

    public override void Think(MonsterController controller)
    {
        if (CanMove(controller))
        {
            Move(controller);
        }
    }

    public override void Move(MonsterController controller)
    {
        GameObject target = controller.playerObject;
        Rigidbody2D rb = controller.rb;
        Animator animator = controller.animator;


        //Move the runner towards the player at a fixed speed using rigidbody
        Vector2 direction = target.transform.position - rb.transform.position;
        direction.Normalize();
        rb.MovePosition(rb.position + direction * controller.movementSpeed * Time.deltaTime);
        AdjustAnimation(animator, direction);
    }

    public void AdjustAnimation(Animator animator, Vector2 direction)
    {
        var state = "RunnerWalkRight";
        if (direction.x > 0)
        {
            state = "RunnerWalkRight";
        }
        else if (direction.x < 0)
        {
            state = "RunnerWalkLeft";
        }
        animator.CrossFade(state, 0, 0);
    }

}
