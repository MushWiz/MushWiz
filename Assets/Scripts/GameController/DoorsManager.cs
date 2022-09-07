using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorsManager : MonoBehaviour
{
    public List<DoorController> doors;
    public List<DoorController> blackScreens;

    private void Awake()
    {
        GameStateManager.Instance.OnSignalReceived += OnSignalReceived;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnSignalReceived -= OnSignalReceived;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        ConnectDoorsAndScreens();
    }

    private void ConnectDoorsAndScreens()
    {

        doors.Clear();
        blackScreens.Clear();

        GameObject doorHolder = GameObject.FindGameObjectWithTag("DoorsHolder");
        if (doorHolder)
        {
            doors = doorHolder.GetComponentsInChildren<DoorController>().ToList<DoorController>();
            foreach (DoorController door in doors)
            {
                if (door.initialState == DoorState.Active)
                {
                    door.gameObject.SetActive(true);
                }
                else
                {
                    door.gameObject.SetActive(false);
                }
            }
        }

        GameObject blackScreensHolder = GameObject.FindGameObjectWithTag("BlackScreensHolder");
        if (blackScreensHolder)
        {
            blackScreens = blackScreensHolder.GetComponentsInChildren<DoorController>().ToList<DoorController>();
            foreach (DoorController blackScreen in blackScreens)
            {
                if (blackScreen.initialState == DoorState.Active)
                {
                    blackScreen.gameObject.SetActive(true);
                }
                else
                {
                    blackScreen.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ConnectDoorsAndScreens();
    }

    void OnSignalReceived(GameObject source, string signal)
    {
        foreach (DoorController door in doors)
        {
            if (door.doorId == signal)
            {
                ChangeDoorState(door);
                continue;
            }

            foreach (string signalToCheck in door.otherSignals)
            {
                if (signalToCheck == signal)
                {
                    ChangeDoorState(door);
                    break;
                }
            }
        }

        foreach (DoorController blackScreen in blackScreens)
        {
            if (blackScreen.doorId == signal)
            {
                ChangeDoorState(blackScreen);
                continue;
            }

            foreach (string signalToCheck in blackScreen.otherSignals)
            {
                if (signalToCheck == signal)
                {
                    ChangeDoorState(blackScreen);
                    break;
                }
            }
        }

        GetComponent<GameController>().UpdateNavMeshData();
    }

    public void ChangeDoorState(DoorController doorToChange)
    {
        doorToChange.gameObject.SetActive(!doorToChange.gameObject.activeSelf);
        if (doorToChange.gameObject.activeSelf)
        {
            doorToChange.currentState = DoorState.Active;
        }
        else
        {
            doorToChange.currentState = DoorState.Disabled;
        }
    }
}