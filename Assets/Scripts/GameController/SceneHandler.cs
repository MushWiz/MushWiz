using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler
{
    public static GameController controller;

    public static SceneHandler instance;
    public static SceneHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SceneHandler();
            }
            return instance;
        }
    }

    private void ConnectController()
    {
        if (controller) { return; }
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void LoadPortalScene(string sceneName, PortalController portal)
    {
        ConnectController();
        controller.LoadScene(sceneName, portal);
    }

    public void LoadScene(string sceneName)
    {
        controller.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
    {
        SceneManager.LoadScene(sceneName, loadSceneMode);
    }

    public void LoadScene(int sceneIndex, LoadSceneMode loadSceneMode)
    {
        SceneManager.LoadScene(sceneIndex, loadSceneMode);
    }

}