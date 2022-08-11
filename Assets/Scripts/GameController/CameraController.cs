using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerEntity;

    //Follow the player with a smooth camera
    void Update()
    {
        if (!playerEntity)
        {
            return;
        }
        //Interpolate the camera position to the player position
        transform.position = Vector3.Lerp(transform.position, playerEntity.transform.position, Time.deltaTime * 5);
        //fix the camera z to -10
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
