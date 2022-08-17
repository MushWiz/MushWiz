using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireAbility", menuName = "Mushroom Wizard/FireAbility", order = 0)]
public class FireAbility : MushAbilities
{
    public GameObject firePrefab;

    public override void UseAbility(MushController mushController)
    {

        Ability(mushController);

    }

    private void Ability(MushController mushController)
    {
        //Pick a random direction
        Vector2 direction = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );

        direction.Normalize();

        //Transform the direction into quaternion
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);

        //Create the fire at the player's position and with the random direction
        GameObject fire = Instantiate(firePrefab, mushController.transform.position + (Vector3)direction, rotation);
        fire.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
        fire.GetComponent<AbilityStats>().abilityDamage = abilityDamage;
        fire.GetComponent<AbilityStats>().sourceAbility = this;
        fire.GetComponent<ProjectileController>().shooter = mushController.transform;
        fire.GetComponent<ProjectileController>().maxTravelDistance = projectileMaxTravel;

    }
}