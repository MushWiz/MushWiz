using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStats : MonoBehaviour
{
    public float abilityDamage;
    public MushAbilities sourceAbility;

    public void OnHit(MonsterController MonsterController)
    {
        MonsterController.TakeDamage(abilityDamage);
        if (sourceAbility.destroyable)
        {
            Destroy(gameObject);
        }
    }
}
