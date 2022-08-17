using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBehaviour : ScriptableObject
{
    public abstract void OnProjectileCreated(ProjectileController projectile);

    public abstract void OnProjectileDestroyed(ProjectileController projectile);

    public abstract void OnProjectileHit(ProjectileController projectile, Collision2D collisionHit);

    public abstract void OnProjectileReflected(ProjectileController projectile, Collision2D collisionHit);

    public abstract void OnProjectileTravel(ProjectileController projectile);
}