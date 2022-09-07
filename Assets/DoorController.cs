using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public string doorId;
    public DoorState initialState;
    public List<string> otherSignals;

    [HideInInspector] public DoorState currentState;

    private void Start()
    {
        currentState = initialState;
    }
}

public enum DoorState
{
    Active,
    Disabled,
}
