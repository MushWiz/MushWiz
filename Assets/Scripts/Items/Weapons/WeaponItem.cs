using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/Weapon")]
public class WeaponItem : Item
{
    [Header("Base Weapon Properties")]
    public GameObject weaponPrefab;
    public WeaponType weaponType;

    public float attackTiming;

    [Header("Direct damages")]
    public float meleeDamage;
    public float rangedDamage;

    [Header("Weapon Range")]
    public float maxRange;
    public float minRange;

    [Header("Projectile Stats")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileLifetime;
    public float projectileDamage;
    public float projectileSize;
    public float projectileSpread;
    public int projectileCount;
    public int projectileMaxReflections;

    public override void OnEquip(MushController mushController, MushEquipment equipmentSlot)
    {
        Equip(mushController, equipmentSlot);
    }

    public override void Equip(MushController mushController, MushEquipment equipmentSlot)
    {
        mushController.gameObject.GetComponentInChildren<MushWeaponHolder>().EquipWeapon(this, equipmentSlot);
    }

    public override void OnUnequip(MushController mushController, MushEquipment equipmentSlot)
    {
        Unequip(mushController, equipmentSlot);
    }

    public override void Unequip(MushController mushController, MushEquipment equipmentSlot)
    {
        mushController.gameObject.GetComponentInChildren<MushWeaponHolder>().UnequipWeapon(this);
        CreateCollectible(mushController.transform);
    }
}

public enum WeaponType
{
    Melee,
    Ranged
}
