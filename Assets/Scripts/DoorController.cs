using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public string doorId;
    public DoorState initialState;
    public List<string> requiredSignalsList;
    public Dictionary<string, bool> requiredSignals = new Dictionary<string, bool>();

    public bool sendSignalOnTrigger = false;
    public string signalToSend = "";

    [HideInInspector] public DoorState currentState;

    private void Awake()
    {
        currentState = initialState;

        for (int i = 0; i < requiredSignalsList.Count; i++)
        {
            bool doorActive = initialState == DoorState.Active ? true : false;
            requiredSignals.Add(requiredSignalsList[i], doorActive);
        }
    }

}

public enum DoorState
{
    Active,
    Disabled,
}
