using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Shoot")]
public class MonsterActionShoot : MonsterAction
{

    public override void Act(MonsterStateController controller)
    {
        controller.transform.rotation = Quaternion.identity;
        controller.transform.position = new Vector3(controller.navMeshAgent.nextPosition.x, controller.navMeshAgent.nextPosition.y, 0);
        AdjustAnimation(controller);
        if (CanAttack(controller))
        {
            Attack(controller);
        }
    }

    public void Attack(MonsterStateController controller)
    {
        if (controller.target == null)
        {
            Debug.LogError("No target");
            return;
        }

        if (controller.monsterController.canAttack)
        {
            //Shoot the player
            GameObject bullet = Instantiate(controller.monsterController.projectilePrefab, controller.transform.position - Vector3.up * 0.1f, Quaternion.identity);
            bullet.tag = "EnemyProjectile";
            Vector2 direction = controller.target.transform.position - controller.transform.position;
            direction.Normalize();
            bullet.GetComponent<Rigidbody2D>().velocity = direction * controller.monsterController.projectileSpeed;
            bullet.GetComponent<ProjectileController>().shooter = controller.transform;
            bullet.GetComponent<ProjectileController>().maxTravelDistance = controller.monsterController.projectileMaxTravel;
            bullet.GetComponent<ProjectileController>().projectileDamage = controller.monsterController.damageDealer;
            controller.monsterController.canAttack = false;
        }
    }

    public bool CanAttack(MonsterStateController controller)
    {
        //Check line of sight
        Vector2 direction = controller.target.transform.position - controller.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, direction, controller.monsterController.attackRange, LayerMask.GetMask("Player"));
        if (hit.collider == null || hit.collider.gameObject.tag != "Player")
        {
            return false;
        }

        if (Vector2.Distance(controller.transform.position, controller.target.transform.position) <= controller.monsterController.attackRange)
        {
            controller.monsterController.isAttacking = true;
            return true;
        }
        return false;
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
            if (controller.monsterController.isAttacking)
            {
                state = prefix + "AttackRight";
            }
        }
        else if (direction.x < 0)
        {
            state = prefix + "WalkLeft";
            if (controller.monsterController.isAttacking)
            {
                state = prefix + "AttackLeft";
            }
        }
        animator.CrossFade(state, 0, 0);
    }

}