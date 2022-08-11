using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShooterEnemy", menuName = "Mushroom Wizard/ShooterEnemy", order = 0)]
public class ShooterMovement : MonsterBehaviour
{

    public override void Think(MonsterController controller)
    {
        AdjustAnimation(controller);
        if (CanAttack(controller))
        {
            Attack(controller);
        }
        if (CanMove(controller))
        {
            Move(controller);
        }
    }

    public override bool CanMove(MonsterController controller)
    {
        GameObject target = controller.playerObject;
        Rigidbody2D rb = controller.rb;
        if (Vector2.Distance(rb.transform.position, target.transform.position) > controller.attackRange)
        {
            return true;
        }

        return false;
    }

    public override bool CanAttack(MonsterController controller)
    {
        GameObject target = controller.playerObject;
        Rigidbody2D rb = controller.rb;

        //Check line of sight
        Vector2 direction = target.transform.position - rb.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, direction, controller.attackRange, LayerMask.GetMask("Player"));
        if (hit.collider == null || hit.collider.gameObject.tag != "Player")
        {
            return false;
        }

        if (Vector2.Distance(rb.transform.position, target.transform.position) <= controller.attackRange)
        {
            controller.isAttacking = true;
            return true;
        }
        return false;
    }

    public override void Move(MonsterController controller)
    {
        GameObject target = controller.playerObject;
        Rigidbody2D rb = controller.rb;
        Animator animator = controller.animator;


        //Move the runner towards the player at a fixed speed using rigidbody
        Vector2 direction = target.transform.position - rb.transform.position;
        direction.Normalize();
        if (Vector2.Distance(rb.transform.position, target.transform.position) > controller.attackRange)
        {
            controller.isAttacking = false;
            rb.MovePosition(rb.position + direction * controller.movementSpeed * Time.deltaTime);
        }
    }

    public override void Attack(MonsterController controller)
    {
        //Stay in the same spot
        controller.rb.velocity = Vector2.zero;

        if (controller.canAttack)
        {
            //Shoot the player
            GameObject bullet = Instantiate(controller.projectilePrefab, controller.transform.position - Vector3.up * 0.1f, Quaternion.identity);
            bullet.tag = "EnemyProjectile";
            Vector2 direction = controller.playerObject.transform.position - controller.transform.position;
            direction.Normalize();
            bullet.GetComponent<Rigidbody2D>().velocity = direction * controller.projectileSpeed;
            bullet.GetComponent<ProjectileStats>().shooter = controller.transform;
            bullet.GetComponent<ProjectileStats>().maxTravelDistance = controller.projectileMaxTravel;
            bullet.GetComponent<ProjectileStats>().projectileDamage = controller.damageDealer;
            controller.canAttack = false;
        }

    }

    public void AdjustAnimation(MonsterController controller)
    {
        GameObject target = controller.playerObject;
        Rigidbody2D rb = controller.rb;
        Animator animator = controller.animator;
        Vector2 direction = target.transform.position - rb.transform.position;
        var state = "ShooterWalkRight";
        if (direction.x > 0)
        {
            state = "ShooterWalkRight";
            if (controller.isAttacking)
            {
                state = "ShooterAttackRight";
            }
        }
        else if (direction.x < 0)
        {
            state = "ShooterWalkLeft";
            if (controller.isAttacking)
            {
                state = "ShooterAttackLeft";
            }
        }
        animator.CrossFade(state, 0, 0);
    }

}