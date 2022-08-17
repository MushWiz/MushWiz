using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Splitter", menuName = "Mushroom Wizard/ProjectileBehaviours/Splitter", order = 0)]
public class Splitter : ProjectileBehaviour
{
    public override void OnProjectileCreated(ProjectileController projectile)
    {

    }

    public override void OnProjectileDestroyed(ProjectileController projectile)
    {

    }

    public override void OnProjectileHit(ProjectileController projectile, Collision2D collisionHit)
    {

    }

    public override void OnProjectileReflected(ProjectileController projectile, Collision2D collisionHit)
    {

    }

    public override void OnProjectileTravel(ProjectileController projectile)
    {
        //split the projectile into two projectiles after traveling for a certain amount of time
        if (projectile.travelTime < 0.5f || projectile.deleting)
        {
            return;
        }
        projectile.deleting = true;
        //split the projectile into two projectiles
        GameObject projectile1 = Instantiate(projectile.gameObject, projectile.transform.position, projectile.transform.rotation);
        GameObject projectile2 = Instantiate(projectile.gameObject, projectile.transform.position, projectile.transform.rotation);

        Vector2 originalDirection = projectile.currentVelocity.normalized;
        Vector2 leftOfOriginalDirection = Quaternion.Euler(0, 0, -90) * originalDirection;
        Vector2 rightOfOriginalDirection = Quaternion.Euler(0, 0, 90) * originalDirection;

        projectile1.transform.position += (Vector3)leftOfOriginalDirection * 0.25f;
        projectile2.transform.position += (Vector3)rightOfOriginalDirection * 0.25f;

        Vector2 fourtyFiveDegreesRight = Quaternion.Euler(0, 0, 45) * originalDirection;
        Vector2 fourtyFiveDegreesLeft = Quaternion.Euler(0, 0, -45) * originalDirection;

        projectile1.GetComponent<Rigidbody2D>().velocity = fourtyFiveDegreesLeft * projectile.currentVelocity.magnitude;
        projectile2.GetComponent<Rigidbody2D>().velocity = fourtyFiveDegreesRight * projectile.currentVelocity.magnitude;

        Destroy(projectile.gameObject);
    }
}