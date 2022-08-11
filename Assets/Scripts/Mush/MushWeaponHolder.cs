using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushWeaponHolder : MonoBehaviour
{
    public Transform weaponHolder;
    public GameObject currentWeapon;

    public void EquipWeapon(WeaponItem weapon)
    {
        if (currentWeapon != null)
        {
            DropCurrentWeapon();
        }
        currentWeapon = Instantiate(weapon.weaponPrefab, weaponHolder.position, weaponHolder.rotation) as GameObject;
        currentWeapon.transform.parent = weaponHolder;
        currentWeapon.GetComponent<WeaponManager>().weaponBase = weapon;
        if (weapon.weaponType == WeaponType.Ranged)
        {
            gameObject.GetComponentInParent<MushMainShooter>().bulletPrefab = weapon.projectilePrefab;
        }

    }

    public void UnequipWeapon()
    {
        DropCurrentWeapon();
    }

    public void DropCurrentWeapon()
    {
        if (currentWeapon != null)
        {
            WeaponItem currentWeaponItem = currentWeapon.GetComponent<WeaponManager>().weaponBase;
            currentWeaponItem.Unequip(gameObject.GetComponentInParent<MushController>());
            currentWeaponItem.OnDrop(gameObject.GetComponentInParent<MushController>());
            if (currentWeaponItem.weaponType == WeaponType.Ranged)
            {
                gameObject.GetComponentInParent<MushMainShooter>().bulletPrefab = null;
            }
            Destroy(currentWeapon);
            currentWeapon = null;
        }
    }

}
