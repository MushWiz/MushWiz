using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ActionBarSlot
{

    public GameObject actionBarSlot;
    public Sprite abilityIcon;
    public Image abilityCooldownOverlay;
    public MushAbilities ability;

    public Image abilityButton;

    public float abilityCooldown;
    public float abilityCooldownTimer;
    public bool abilityOnCooldown;

    public void UseAbility(MushController mushController)
    {
        ability.UseAbility(mushController);
        abilityOnCooldown = true;
        abilityCooldownTimer += abilityCooldown;
    }
}