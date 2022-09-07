using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public bool useHealth = false;
    public int currentHits;
    public float currentHealth;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            if (useHealth)
            {
                currentHealth -= other.gameObject.GetComponent<ProjectileController>().projectileDamage;
                if (currentHealth <= 0)
                {
                    Destroy(gameObject);
                }
                return;
            }

            currentHits--;
            if (currentHits <= 0)
            {
                Destroy(gameObject);
            }

        }
    }
}
