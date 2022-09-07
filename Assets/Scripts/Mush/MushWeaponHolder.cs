using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushWeaponHolder : MonoBehaviour
{
    public Transform weaponHolder;
    public GameObject currentWeapon;
    public int numberOfClicks = 0;
    public float maxComboDelay = 0.5f;
    public float nextComboTime = 0;

    float lastClickedTime;
    int animationToPlay = 1;
    float currentComboTime = 0;

    public void EquipWeapon(WeaponItem weapon, MushEquipment equipmentSlot)
    {
        if (weapon.weaponPrefab)
        {
            currentWeapon = Instantiate(weapon.weaponPrefab, weaponHolder.position, weaponHolder.rotation) as GameObject;
            currentWeapon.transform.SetParent(weaponHolder);
        }
        gameObject.GetComponentInParent<MushAttack>().EquipWeapon(weapon);
    }

    public void UnequipWeapon()
    {
        gameObject.GetComponentInParent<MushAttack>().UnequipWeapon();
        if (currentWeapon)
        {
            Destroy(currentWeapon);
        }
    }

    private void Update()
    {
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            animationToPlay = 1;
        }
    }

    public float UseWeapon(WeaponItem weaponToUse)
    {
        if (Time.time - currentComboTime < 0)
        {
            return 0;
        }

        Animator animator = currentWeapon.GetComponent<Animator>();

        lastClickedTime = Time.time;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
        {
            animator.CrossFade(weaponToUse.itemName + weaponToUse.weaponAnimationAttackType + "_" + animationToPlay, 0, 0);
            animationToPlay = animationToPlay > 2 ? 1 : animationToPlay + 1;
            if (animationToPlay == 3)
            {
                currentComboTime = Time.time + nextComboTime;
            }
        }

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == weaponToUse.itemName + weaponToUse.weaponAnimationAttackType + "_" + animationToPlay)
            {
                return clip.length;
            }

        }

        return 0;
    }

}
