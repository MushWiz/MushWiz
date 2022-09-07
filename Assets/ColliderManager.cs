using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    public BoxCollider2D ourCollider;

    public void ActivateCollider()
    {
        ourCollider.enabled = true;
    }

    public void DeactivateCollider()
    {
        ourCollider.enabled = false;
    }

    public void PulseCollider()
    {
        StartCoroutine(PulseColliderCoroutine());
    }

    public IEnumerator PulseColliderCoroutine()
    {
        ActivateCollider();
        yield return new WaitForFixedUpdate();
        DeactivateCollider();
    }
}
