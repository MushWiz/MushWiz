using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushWeaponHolder : MonoBehaviour
{
    public Transform weaponHolder;
    public GameObject currentWeapon;


    public void EquipWeapon(WeaponItem weapon, MushEquipment equipmentSlot)
    {
        currentWeapon = Instantiate(weapon.weaponPrefab, weaponHolder.position, weaponHolder.rotation) as GameObject;
        currentWeapon.transform.SetParent(weaponHolder);
        if (weapon.weaponType == WeaponType.Ranged)
        {
            gameObject.GetComponentInParent<MushMainShooter>().bulletPrefab = weapon.projectilePrefab;
        }
    }

    public void UnequipWeapon(WeaponItem weapon)
    {
        Destroy(currentWeapon);
    }

}
