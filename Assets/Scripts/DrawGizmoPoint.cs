using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmoPoint : MonoBehaviour
{

    public Color color = Color.red;
    public float radius = 0.1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
