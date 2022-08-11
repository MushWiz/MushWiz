using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeElevationChildrenAddon : MonoBehaviour
{

    public List<GameObject> children;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //return if the other collider is also a trigger
        if (other.isTrigger)
        {
            return;
        }
        gameObject.GetComponentInParent<ChangeElevation>().PullTrigger(other);
    }

}