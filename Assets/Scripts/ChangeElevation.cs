using UnityEngine;
public class ChangeElevation : MonoBehaviour
{
    public GameObject firstPoint;
    public GameObject secondPoint;


    public void BuildPoints(bool northFacing)
    {
        //destroy the points if they exists
        if (firstPoint != null)
        {
            DestroyImmediate(firstPoint);
        }
        if (secondPoint != null)
        {
            DestroyImmediate(secondPoint);
        }

        firstPoint = new GameObject("FirstPoint");
        secondPoint = new GameObject("SecondPoint");
        //set the parent of the points to the ChangeElevation gameobject
        firstPoint.transform.parent = transform;
        secondPoint.transform.parent = transform;
        //set the points rotation to 0,0,0
        firstPoint.transform.localRotation = Quaternion.identity;
        secondPoint.transform.localRotation = Quaternion.identity;

        float elevation = northFacing ? 1 : -1;

        firstPoint.transform.position = transform.position + new Vector3(.25f * elevation, .25f, 0);
        secondPoint.transform.position = transform.position + new Vector3(-.25f * elevation, -.25f, 0);

        //scale the points to 2,1,1
        firstPoint.transform.localScale = new Vector3(2, 1, 1);
        secondPoint.transform.localScale = new Vector3(2, 1, 1);

        //add rigidbody2d to the points and set them to static
        Rigidbody2D firstPointRigidbody = firstPoint.AddComponent<Rigidbody2D>();
        firstPointRigidbody.bodyType = RigidbodyType2D.Static;
        Rigidbody2D secondPointRigidbody = secondPoint.AddComponent<Rigidbody2D>();
        secondPointRigidbody.bodyType = RigidbodyType2D.Static;

        //add a box collider 2d to the points
        EdgeCollider2D firstEdge = firstPoint.AddComponent<EdgeCollider2D>();
        EdgeCollider2D secondEdge = secondPoint.AddComponent<EdgeCollider2D>();

        //set the colliders to trigger
        firstEdge.isTrigger = true;
        secondEdge.isTrigger = true;

        //add the changeelevationchildreaddon to the points
        firstPoint.AddComponent<ChangeElevationChildrenAddon>();
        secondPoint.AddComponent<ChangeElevationChildrenAddon>();
    }

    public void PullTrigger(Collider2D other)
    {
        Debug.Log("PullTrigger");
    }

}