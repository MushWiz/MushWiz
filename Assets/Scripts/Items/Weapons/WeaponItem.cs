using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/Weapon")]
public class WeaponItem : Item
{
    public GameObject weaponPrefab;
    public WeaponType weaponType;

    public GameObject projectilePrefab;

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
