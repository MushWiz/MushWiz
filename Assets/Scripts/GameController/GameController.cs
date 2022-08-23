using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class GameController : MonoBehaviour
{
    public Scene currentScene;

    public NavMeshSurface navMeshSurface;

    public GameObject playerPrefab;
    public GameObject playerEntity;
    public MushController mushController;

    public CameraController cameraController;

    public float currentTime;

    public bool isLevellingUp = false;

    public int killCount = 0;
    private int killCountHighscore = 0;

    public UIHandler uIHandler;
    public SpawnersManager spawnersManager;

    public List<GameObject> enemiesEntities = new List<GameObject>();

    [SerializeField] private GameState gameState = GameState.MainMenu;

    public bool paused = false;

    public bool testing = false;

    public bool shouldWork = true;

    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        GameStateManager.Instance.OnGameEvent += OnGameEvent;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        GameStateManager.Instance.OnGameEvent -= OnGameEvent;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        if (!playerEntity)
        {
            playerEntity = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }

        mushController = playerEntity.GetComponent<MushController>();

        if (!testing)
        {
            mushController.isActive = false;
        }

        mushController.controller = this;

        cameraController.playerEntity = playerEntity;

        currentTime = Time.time;

        int highscore = PlayerPrefs.GetInt("KillCount", 0);
        uIHandler.killCountHighscoreText.text = highscore.ToString();
        uIHandler.killCountText.text = "Score: " + killCount.ToString();
        uIHandler.EnableUIByType(UIType.MainMenu);
        uIHandler.DisableUIByTypeList(new List<UIType>() { UIType.InGame, UIType.LevelUp, UIType.CharacterInfo });
        UpdateInfoPanel();
        UpdateActionBar();
        UpdateInventory();
    }

    private void Update()
    {

        RemoveDeadEnemiesFromList();

        if ((gameState == GameState.Playing || gameState == GameState.Paused) && Input.GetKeyDown(KeyCode.Escape) && !paused)
        {
            PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateInfoPanel();
            uIHandler.ToggleUIType(UIType.CharacterInfo);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveGame();
        }

        if (gameState == GameState.MainMenu)
        {

        }
        if (gameState == GameState.Playing)
        {
            currentTime = Time.time;
            uIHandler.UpdateUIByType(UIType.InGame);
            if (spawnersManager)
            {
                spawnersManager.SetupSpawn(this);
            }
        }
        if (gameState == GameState.GameOver)
        {
            ResetWorld();
            ChangeGameState(GameState.MainMenu);
            uIHandler.UpdateUIByType(UIType.MainMenu);
            uIHandler.DisableUIByType(UIType.InGame);
        }

    }

    private void RemoveDeadEnemiesFromList()
    {
        foreach (GameObject enemyCheck in enemiesEntities.ToList())
        {
            if (enemyCheck == null || enemyCheck.GetComponent<MonsterController>() == null)
            {
                enemiesEntities.Remove(enemyCheck);
            }
            if (enemyCheck.GetComponent<MonsterController>().dead)
            {
                enemiesEntities.Remove(enemyCheck);
                Destroy(enemyCheck);
            }
        }
    }

    private void ClearAllEnemies()
    {
        foreach (GameObject enemy in enemiesEntities)
        {
            Destroy(enemy);
        }
        enemiesEntities.Clear();
    }

    void ResetWorld()
    {
        ClearAllEnemies();
        ResetPlayer();
        killCount = 0;
        killCountHighscore = PlayerPrefs.GetInt("KillCount", 0);
        uIHandler.killCountText.text = "Score: " + killCount.ToString();
        currentTime = 0;
    }

    private void ResetPlayer()
    {
        mushController.Reset();
    }

    public void StartGame()
    {
        ChangeGameState(GameState.Playing);
        mushController.isActive = true;
        uIHandler.DisableUIByType(UIType.MainMenu);
        uIHandler.EnableUIByType(UIType.InGame);
    }

    public void ChangeGameState(GameState newState)
    {
        if (gameState == newState)
        {
            return;
        }
        gameState = newState;
    }

    public void CheckPlayerHealth()
    {
        if (mushController.isDead)
        {
            ChangeGameState(GameState.GameOver);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RegisterEnemyDeath(float experiencePoints)
    {
        IncreaseScore();
        IncreaseExperience(experiencePoints);
        spawnersManager.DeadEnemy();
    }

    public void IncreaseScore()
    {
        killCount++;
        uIHandler.killCountText.text = "Score: " + killCount.ToString();
        if (killCount > killCountHighscore)
        {
            killCountHighscore = killCount;
            PlayerPrefs.SetInt("KillCount", killCountHighscore);
            uIHandler.killCountHighscoreText.text = killCountHighscore.ToString();
        }
    }

    public void IncreaseExperience(float experiencePoints)
    {
        mushController.IncreaseExperience(experiencePoints);
    }

    private void OnGameStateChanged(GameState newState)
    {

        ChangeGameState(newState);

    }

    private void PauseGame(bool forced = false, GameState newState = GameState.Paused)
    {
        GameState currentGameState = GameStateManager.Instance.CurrentGameState;
        GameState newGameState = currentGameState == GameState.Playing ? GameState.Paused : GameState.Playing;

        if (forced)
        {
            newGameState = newState;
        }

        GameStateManager.Instance.SetState(newGameState);
    }

    private void OnGameEvent(GameEvent gameEvent)
    {
        if (gameEvent == GameEvent.LevelUp)
        {
            //StartCoroutine(OnLevelUp());
        }
    }

    public IEnumerator OnLevelUp()
    {
        PauseGame(true, GameState.Paused);
        paused = true;
        uIHandler.EnableUIByType(UIType.LevelUp);

        mushController.CreateLevelUpButtons();

        while (paused)
        {
            yield return null;
        }
        uIHandler.DisableUIByType(UIType.LevelUp);
        PauseGame(true, GameState.Playing);
    }

    public void OnLevelUpButtonPressed(StatType statType)
    {
        mushController.OnLevelUpButtonPressed(statType);
        //paused = false;
        //PauseGame(true, GameState.Playing);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        navMeshSurface.BuildNavMesh();
        spawnersManager = GameObject.FindGameObjectWithTag("SpawnersManager").GetComponent<SpawnersManager>();
    }

    public void UpdateInfoPanel()
    {
        uIHandler.UpdateInfoPanel(mushController);
    }

    public void UpdateActionBar()
    {
        uIHandler.UpdateActionBar(mushController);
    }

    public void UpdateInventory()
    {
        uIHandler.UpdateInventory(mushController);
    }

    public void SaveGame()
    {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();

        PlayerData playerData = data.player;
        mushController.LoadPlayer(playerData);

        foreach (int id in playerData.items)
        {
            ItemDatabase itemDatabase = uIHandler.mushInventory.database;
            //Search the database dictionary for the item with the matching id
            foreach (KeyValuePair<Item, int> entry in itemDatabase.itemsIDs)
            {
                if (entry.Value == id)
                {
                    Debug.Log("Found item: " + entry.Key.name);
                }
            }
        }
        foreach (int id in playerData.equipments)
        {
            ItemDatabase itemDatabase = uIHandler.mushInventory.database;
            //Search the database dictionary for the item with the matching id
            foreach (KeyValuePair<Item, int> entry in itemDatabase.itemsIDs)
            {
                if (entry.Value == id)
                {
                    Debug.Log("Found item: " + entry.Key.name);
                }
            }
        }
    }

}

public enum GameState
{
    MainMenu,
    Playing,
    GameOver,
    Paused
}
