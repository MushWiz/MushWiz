using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    //To what scene should the player be transferred?
    public string sceneToLoad;

    public float timeToLoad = 3f;

    bool playerIsOverlapping = false;

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
        if (playerIsOverlapping)
        {
            SceneHandler.Instance.LoadScene(sceneToLoad);
            GameStateManager.Instance.SendEvent(GameEvent.ChangeScene);
        }
    }
}
