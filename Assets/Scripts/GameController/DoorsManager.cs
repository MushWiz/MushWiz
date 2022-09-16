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
            Dictionary<string, bool> doorSignals = new Dictionary<string, bool>(door.requiredSignals);
            foreach (KeyValuePair<string, bool> requiredSignal in doorSignals)
            {
                if (requiredSignal.Key == signal)
                {
                    door.requiredSignals[requiredSignal.Key] = !requiredSignal.Value;
                }
            }
            ChangeDoorState(door);
        }

        foreach (DoorController blackScreen in blackScreens)
        {
            Dictionary<string, bool> blackScreenSignals = new Dictionary<string, bool>(blackScreen.requiredSignals);
            foreach (KeyValuePair<string, bool> requiredSignal in blackScreenSignals)
            {
                if (requiredSignal.Key == signal)
                {
                    blackScreen.requiredSignals[requiredSignal.Key] = !requiredSignal.Value;
                }
            }
            ChangeDoorState(blackScreen);
        }

        GetComponent<GameController>().UpdateNavMeshData();
    }

    public void ChangeDoorState(DoorController doorToChange)
    {
        if (doorToChange.requiredSignals.Count == 0)
        {
            return;
        }
        Dictionary<string, bool> doorSignals = new Dictionary<string, bool>(doorToChange.requiredSignals);
        foreach (KeyValuePair<string, bool> requiredSignal in doorSignals)
        {
            if (requiredSignal.Key == "null")
            {
                return;
            }
            if (doorToChange.currentState == DoorState.Active)
            {
                if (doorToChange.requiredSignals[requiredSignal.Key])
                {
                    return;
                }
            }
            else
            {
                if (!doorToChange.requiredSignals[requiredSignal.Key])
                {
                    return;
                }
            }
        }
        doorToChange.gameObject.SetActive(!doorToChange.gameObject.activeSelf);
        if (doorToChange.gameObject.activeSelf)
        {
            doorToChange.currentState = DoorState.Active;
        }
        else
        {
            doorToChange.currentState = DoorState.Disabled;
        }

        if (doorToChange.sendSignalOnTrigger)
        {
            GameStateManager.Instance.SendSignal(doorToChange.gameObject, doorToChange.signalToSend);
        }
    }
}