using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public string sceneToLoad;
    public string locationSpawnerTag;

    public float timeToLoad = 3f;

    bool playerIsOverlapping = false;
    bool teleporting = false;

    GameController controller;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIsOverlapping = true;
            StartCoroutine(LoadScene());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIsOverlapping = false;
            StopAllCoroutines();
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(timeToLoad);
        if (playerIsOverlapping && !teleporting)
        {
            teleporting = true;
            controller.LoadScene(sceneToLoad, this);
        }
    }
}
