using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/Weapon")]
public class WeaponItem : Item
{
    public GameObject weaponPrefab;
    public WeaponType weaponType;

    public GameObject projectilePrefab;

    public override void OnPickup(MushController mushController)
    {
        Equip(mushController);
    }

    public override void Equip(MushController mushController)
    {
        mushController.gameObject.GetComponentInChildren<MushWeaponHolder>().EquipWeapon(this);
    }

    public override void Unequip(MushController mushController)
    {
        CreateCollectible(mushController.transform);
    }
}

public enum WeaponType
{
    Melee,
    Ranged
}
