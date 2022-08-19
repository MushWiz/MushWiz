using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushAttack : MonoBehaviour
{

    public Transform pivotPoint;
    public Transform shootingPoint;

    public float lastAttackTime = 0f;

    public WeaponItem currentWeapon;

    public void AttackControl(MushController mushController)
    {
        PivotController(mushController);

        if (currentWeapon == null)
        {
            return;
        }

        if (currentWeapon.weaponType == WeaponType.Melee)
        {
            MeleeAttack(mushController);
        }
        else if (currentWeapon.weaponType == WeaponType.Ranged)
        {
            RangedAttack(mushController);
        }
    }

    public void PivotController(MushController mushController)
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(
            mousePos.x - transform.position.x,
            mousePos.y - transform.position.y
        );
        pivotPoint.up = direction;
    }

    public void MeleeAttack(MushController mushController)
    {
        if (Input.GetMouseButton(0) && Time.time - lastAttackTime > Mathf.Max(currentWeapon.attackTiming - mushController.GetStatValueByType(StatType.Intelligence) * 0.05f, 0.1f))
        {
            Debug.Log("Melee Attack");
        }
    }

    public void RangedAttack(MushController mushController)
    {
        if (Input.GetMouseButton(0) && Time.time - lastAttackTime > Mathf.Max(currentWeapon.attackTiming - mushController.GetStatValueByType(StatType.Intelligence) * 0.05f, 0.1f))
        {
            StartCoroutine(Shoot(mushController));
        }
    }

    private IEnumerator Shoot(MushController mushController)
    {

        lastAttackTime = Time.time;

        for (int i = 0; i < currentWeapon.projectileCount; i++)
        {
            GameObject bullet = Instantiate(currentWeapon.projectilePrefab, shootingPoint.position - shootingPoint.forward.normalized * 0.4f + shootingPoint.forward.normalized * currentWeapon.minRange, shootingPoint.rotation);
            ProjectileController projectileStats = bullet.GetComponent<ProjectileController>();

            projectileStats.maxTravelDistance = currentWeapon.maxRange;

            bullet.GetComponent<Rigidbody2D>().velocity = pivotPoint.up * currentWeapon.projectileSpeed;

            projectileStats.projectileDamage = currentWeapon.projectileDamage;
            projectileStats.projectileDamage *= mushController.GetStatValueByType(StatType.Intelligence) * 0.1f;

            bullet.transform.localScale = new Vector3(currentWeapon.projectileSize, currentWeapon.projectileSize, currentWeapon.projectileSize);

            projectileStats.shooter = mushController.transform;

            projectileStats.projectileMaxReflections = currentWeapon.projectileMaxReflections;

            if (currentWeapon.projectileLifetime > 0)
            {
                Destroy(bullet, currentWeapon.projectileLifetime);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void EquipWeapon(WeaponItem weapon)
    {
        currentWeapon = weapon;
    }

    public void UnequipWeapon()
    {
        currentWeapon = null;
    }

}
