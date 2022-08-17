using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveAbility", menuName = "Mushroom Wizard/WaveAbility", order = 0)]
public class WaveAbility : MushAbilities
{
    public GameObject wavePrefab;

    public override void UseAbility(MushController mushController)
    {

        Ability(mushController);

    }

    private void Ability(MushController mushController)
    {
        //Use the direction of the player pivot to create the wave
        Vector2 direction = mushController.gameObject.GetComponent<MushAttack>().pivotPoint.up;

        direction.Normalize();

        //Transform the direction into quaternion
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);

        //Create the fire at the player's position and with the random direction
        GameObject wave = Instantiate(wavePrefab, mushController.transform.position + (Vector3)direction, rotation);
        wave.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
        wave.GetComponent<AbilityStats>().abilityDamage = abilityDamage;
        wave.GetComponent<AbilityStats>().sourceAbility = this;
        wave.GetComponent<ProjectileController>().shooter = mushController.transform;
        wave.GetComponent<ProjectileController>().maxTravelDistance = projectileMaxTravel;

    }
}