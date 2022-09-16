using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Shoot")]
public class MonsterActionShoot : MonsterAction
{

    public override void Act(MonsterStateController controller)
    {
        //controller.transform.rotation = Quaternion.identity;
        //controller.transform.position = new Vector3(controller.navMeshAgent.nextPosition.x, controller.navMeshAgent.nextPosition.y, 0);
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
            WeaponItem currentWeapon = controller.monsterController.starterWeapon;
            if (!currentWeapon)
            {
                Debug.LogError("No weapon on this mob");
                return;
            }
            GameObject bullet = Instantiate(currentWeapon.projectilePrefab, controller.monsterController.weaponHolder.position, Quaternion.identity);
            bullet.tag = "EnemyProjectile";
            bullet.GetComponent<Rigidbody2D>().velocity = controller.monsterController.pivotPoint.right * currentWeapon.projectileSpeed;
            ProjectileController projectileController = bullet.GetComponent<ProjectileController>();
            projectileController.shooter = controller.transform;
            controller.monsterController.canAttack = false;


            projectileController.maxTravelDistance = currentWeapon.maxRange;

            projectileController.projectileDamage = currentWeapon.projectileDamage;
            projectileController.projectileDamage *= controller.monsterController.GetStatValueByType(StatType.Intelligence) * 0.1f;

            bullet.transform.localScale = new Vector3(currentWeapon.projectileSize, currentWeapon.projectileSize, currentWeapon.projectileSize);

            projectileController.projectileMaxReflections = currentWeapon.projectileMaxReflections;

            if (currentWeapon.projectileLifetime > 0)
            {
                Destroy(bullet, currentWeapon.projectileLifetime);
            }
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