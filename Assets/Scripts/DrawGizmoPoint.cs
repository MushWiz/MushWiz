using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmoPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
