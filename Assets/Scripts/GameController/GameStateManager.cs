using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager
{
    private static GameStateManager _instance;

    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStateManager();
            }

            return _instance;
        }
    }

    public void SetState(GameState newState)
    {
        if (newState == CurrentGameState) return;

        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public GameState CurrentGameState { get; private set; }

    public delegate void GameStateChangeHandler(GameState newState);
    public event GameStateChangeHandler OnGameStateChanged;

    public void SendEvent(GameEvent gameEvent)
    {
        OnGameEvent?.Invoke(gameEvent);
    }

    public delegate void GameEventHandler(GameEvent gameEvent);
    public event GameEventHandler OnGameEvent;

    private GameStateManager()
    {

    }

    public void SendSignal(GameObject source, string signal)
    {
        OnSignalReceived?.Invoke(source, signal);
    }

    public delegate void SignalHandler(GameObject source, string signal);
    public event SignalHandler OnSignalReceived;

}

public enum GameEvent
{
    LevelUp,
    Teleported,
    ChangeScene,
}