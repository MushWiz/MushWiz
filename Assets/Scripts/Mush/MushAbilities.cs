using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushAbilities : ScriptableObject
{

    public GameObject abilityButtonPrefab;
    public float abilityDamage;
    public float cooldown;
    public float cooldownTimer;
    public float projectileSpeed;
    public float projectileMaxTravel;
    public bool destroyable = false;

    public int unlockLevel;

    virtual public void UseAbility(MushController mushController)
    {
        Debug.Log("UseAbility");
    }

}
