using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorsManager : MonoBehaviour
{
    public GameObject doorHolder;
    public List<DoorController> doors;

    private void Awake()
    {
        GameStateManager.Instance.OnSignalReceived += OnSignalReceived;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnSignalReceived -= OnSignalReceived;
    }

    private void Start()
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

    void OnSignalReceived(GameObject source, string signal)
    {
        foreach (DoorController door in doors)
        {
            if (door.doorId == signal)
            {
                door.gameObject.SetActive(!door.gameObject.activeSelf);
                if (door.gameObject.activeSelf)
                {
                    door.currentState = DoorState.Active;
                }
                else
                {
                    door.currentState = DoorState.Disabled;
                }
                GetComponent<GameController>().UpdateNavMeshData();
            }
        }
    }
}