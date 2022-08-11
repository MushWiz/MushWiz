using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushMainShooter : MonoBehaviour
{

    public Transform pivotPoint;
    public Transform shootingPoint;

    public GameObject bulletPrefab;

    private float lastShotTime = 0f;

    public void ShootControl(MushController mushController)
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(
            mousePos.x - transform.position.x,
            mousePos.y - transform.position.y
        );
        pivotPoint.up = direction;

        //Shoot if the last shot was more than 0.2 seconds ago
        if (Input.GetMouseButton(0) && Time.time - lastShotTime > 0.2f)
        {
            Shoot(mushController);
        }
    }

    private void Shoot(MushController mushController)
    {
        if (!bulletPrefab)
        {
            return;
        }
        lastShotTime = Time.time;
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position - shootingPoint.forward.normalized * 0.4f, shootingPoint.rotation);
        ProjectileStats projectileStats = bullet.GetComponent<ProjectileStats>();
        bullet.GetComponent<Rigidbody2D>().velocity = pivotPoint.up * projectileStats.projectileSpeed;
        projectileStats.projectileDamage *= mushController.GetStatValueByName("Magic");
        projectileStats.shooter = mushController.transform;
    }

}
