using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MushAttack : MonoBehaviour
{

    public float lockOnLimit = 10;
    public int lockOnAngle = 25;

    public bool autoRelock = true;

    public Transform pivotPoint;
    public Transform shootingPoint;

    public float lastAttackTime = 0f;

    public WeaponItem currentWeapon;

    public WeaponItem initialWeapon;

    bool lockedPivot = false;
    [SerializeField] bool lockOn = false;
    [SerializeField] MonsterController lockedMonster;

    private void Awake()
    {
        if (initialWeapon)
        {
            GetComponentInChildren<MushWeaponHolder>().EquipWeapon(initialWeapon);
        }
    }

    public void AttackControl(MushController mushController)
    {
        CheckLockOn();
        PivotController(mushController);

        if (Input.GetMouseButtonDown(2))
        {
            ToggleLockOn();
        }

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
        if (lockedPivot)
        {
            return;
        }
        Vector3 aimDirection = Vector3.zero;
        if (!lockOn || lockedMonster == null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = (mousePos - transform.position).normalized;
        }

        else
        {
            aimDirection = (lockedMonster.transform.position - transform.position).normalized;
        }

        aimDirection.z = 0;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        pivotPoint.eulerAngles = new Vector3(0, 0, angle);
    }

    public void MeleeAttack(MushController mushController)
    {
        float attackRate = Mathf.Max(currentWeapon.attackTiming - mushController.GetStatValueByType(StatType.Strength) * 0.05f, 0.1f);
        if (Input.GetMouseButtonDown(0))
        {
            float animationLength = currentWeapon.AttackAnimation(mushController);
        }
    }

    public IEnumerator LockPivot(float attackRate)
    {
        lockedPivot = true;
        yield return new WaitForSeconds(attackRate);
        lockedPivot = false;
    }

    public bool ToggleLockOn()
    {
        if (lockOn)
        {
            lockOn = false;
            return false;
        }

        float closestDistance = Mathf.Infinity;
        MonsterController checkedMob = null;

        for (int i = -lockOnAngle; i < lockOnAngle; i += 2)
        {
            Vector2 direction = Quaternion.Euler(0, 0, i) * pivotPoint.right;
            RaycastHit2D[] hits = Physics2D.RaycastAll(shootingPoint.position, direction, lockOnLimit);
            foreach (RaycastHit2D hit in hits)
            {
                if (!hit.transform.CompareTag("Enemy"))
                {
                    continue;
                }
                if (Vector2.Distance(transform.position, hit.transform.position) > closestDistance)
                {
                    continue;
                }
                checkedMob = hit.transform.gameObject.GetComponent<MonsterController>();
                closestDistance = Vector2.Distance(transform.position, hit.transform.position);
            }
        }

        if (checkedMob != null)
        {
            lockOn = true;
            lockedMonster = checkedMob;
            return true;
        }
        return false;
    }

    public void CheckLockOn()
    {
        if (!lockOn)
        {
            return;
        }

        if (lockedMonster == null || Vector2.Distance(lockedMonster.transform.position, transform.position) > lockOnLimit)
        {
            lockOn = false;
            if (autoRelock && ToggleLockOn())
            {
                return;
            }
            lockedMonster = null;
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
            GameObject bullet = Instantiate(currentWeapon.projectilePrefab, shootingPoint.position - shootingPoint.forward.normalized * 0.4f + shootingPoint.forward.normalized * currentWeapon.minRange, Quaternion.identity);
            ProjectileController projectileStats = bullet.GetComponent<ProjectileController>();

            projectileStats.maxTravelDistance = currentWeapon.maxRange;

            bullet.GetComponent<Rigidbody2D>().velocity = pivotPoint.right * currentWeapon.projectileSpeed;

            projectileStats.projectileDamage = currentWeapon.projectileDamage;
            projectileStats.projectileDamage *= mushController.GetStatValueByType(StatType.Intelligence) * 0.1f;

            bullet.transform.localScale = new Vector3(currentWeapon.projectileSize, currentWeapon.projectileSize, currentWeapon.projectileSize);

            projectileStats.shooter = transform;

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
