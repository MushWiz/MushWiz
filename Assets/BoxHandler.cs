using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxHandler : MonoBehaviour
{
    public float timeBetweenPushes = 1;
    public GameObject colliderCheck;
    float lastPush;
    BoxCollider2D ourCollider;
    public LayerMask collideMask;

    private void Start()
    {
        ourCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && (Time.time - lastPush) > timeBetweenPushes)
        {
            lastPush = Time.time;
            Vector2 direction = (colliderCheck.transform.position - other.transform.position).normalized;
            float xMagnitude = Mathf.Abs(direction.x);
            float yMagnitude = Mathf.Abs(direction.y);

            if (xMagnitude > yMagnitude)
            {
                int amount = direction.x > 0 ? 1 : -1;
                Vector2 movementDirection = new Vector2(amount, 0);
                RaycastHit2D[] hits = Physics2D.RaycastAll(colliderCheck.transform.position, movementDirection, 2f);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        continue;
                    }
                    if (hit.collider.gameObject != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    {
                        return;
                    }
                }
                transform.position += (Vector3)movementDirection;
            }
            else
            {
                int amount = direction.y > 0 ? 1 : -1;
                Vector2 movementDirection = new Vector2(0, amount);
                RaycastHit2D[] hits = Physics2D.RaycastAll(colliderCheck.transform.position, movementDirection, 2f);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        continue;
                    }
                    if (hit.collider.gameObject != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    {
                        return;
                    }
                }
                transform.position += (Vector3)movementDirection;
            }
        }
    }
}
