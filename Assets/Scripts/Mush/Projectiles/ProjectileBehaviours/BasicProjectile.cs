using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicProjectile", menuName = "Mushroom Wizard/ProjectileBehaviours/BasicProjectile", order = 0)]
public class BasicProjectile : ProjectileBehaviour
{
    public override void OnProjectileCreated(ProjectileController projectile)
    {
        Debug.Log("Projectile created");
    }

    public override void OnProjectileDestroyed(ProjectileController projectile)
    {
        Destroy(projectile.gameObject);
    }

    public override void OnProjectileHit(ProjectileController projectile, Collision2D collisionHit)
    {
        if (projectile.shooter == collisionHit.gameObject.transform)
        {
            return;
        }
        Debug.Log("Hit " + collisionHit.gameObject.name);
        OnProjectileDestroyed(projectile);
    }

    public override void OnProjectileReflected(ProjectileController projectile, Collision2D collisionHit)
    {
        if (projectile.projectileMaxReflections > 0)
        {
            //reflect the projectile
            Vector2 newVelocity = Vector2.Reflect(projectile.currentVelocity, collisionHit.contacts[0].normal);
            projectile.rb.velocity = newVelocity * projectile.dampeningEffect;
            projectile.projectileMaxReflections--;
        }
        else
        {
            OnProjectileDestroyed(projectile);
        }
    }

    public override void OnProjectileTravel(ProjectileController projectile)
    {

    }
}