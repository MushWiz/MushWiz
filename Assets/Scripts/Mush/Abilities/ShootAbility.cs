using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootAbility", menuName = "Mushroom Wizard/ShootAbility", order = 0)]
public class ShootAbility : MushAbilities
{
    public GameObject bulletPrefab;

    public override void UseAbility(MushController mushController)
    {

        Ability(mushController);

    }

    private void Ability(MushController mushController)
    {
        //For each direction around the player divided by 15 degrees
        for (int i = 0; i < 360; i += 15)
        {
            //Pick a random direction
            Vector2 direction = new Vector2(
                Mathf.Cos(i * Mathf.Deg2Rad),
                Mathf.Sin(i * Mathf.Deg2Rad)
            );

            direction.Normalize();

            //Transform the direction into quaternion
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);

            //Create the bullet at the player's position and with the random direction
            GameObject bullet = Instantiate(bulletPrefab, mushController.transform.position + (Vector3)direction, rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
            bullet.GetComponent<ProjectileStats>().projectileDamage = abilityDamage;
            bullet.GetComponent<ProjectileStats>().maxTravelDistance = projectileMaxTravel;
            bullet.GetComponent<ProjectileStats>().shooter = mushController.transform;
            bullet.GetComponent<AbilityStats>().sourceAbility = this;
        }

    }
}