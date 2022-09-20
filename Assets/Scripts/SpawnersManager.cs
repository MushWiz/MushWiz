using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnersManager : MonoBehaviour
{
    public List<EnemySpawner> spawners;
    public List<GameObject> enemiesPrefabs;
    [HideInInspector] public List<GameObject> enemiesEntities;

    public bool randomizedSpanwers = false;
    public bool spawnersEnabled = true;
    public bool isWave = false;
    public bool useManagerPrefabs = false;
    public bool checkSpawnTime = false;
    public bool activeWhenTriggered = false;
    public bool onlySignalActivation = true;
    [HideInInspector] public bool triggered = false;
    public string triggererId = "";
    public bool sendsSignalAfterEnemies = false;
    [HideInInspector] public bool signalSent = false;
    public List<string> signalsToSend = new List<string>();
    public bool disabeAfterEnemies = true;
    public int enemyLevel = 1;
    public int enemyLevelIncrease = 0;

    #region Normal Spawn
    [Header("Normal Spawn")]
    public int totalAmountOfEnemiesNotWave = 0;
    public int amountOfEnemiesSpawned = 0;
    public int amountOfEnemiesKilled = 0;
    public int numberOfEnemiesPerLevelUp = 0;
    public int numberOfEnemiesPerLevelUpIncrease = 0;
    public bool infiniteEnemies = false;
    public int remainingEnemiesToSpawn = 99999;
    #endregion

    #region Wave Spawn
    [Header("Wave Spawn")]
    public int enemiesPerWave = 0;
    public int enemiesPerWaveIncrease = 0;
    public int enemyLevelIncreaseAfterCounter = 0;
    public int enemyLevelIncreaseCounter = 0;
    public bool infiniteWaves = false;
    public int remainingWaves = 99999;

    public bool needSignalAfterWave = false;

    public bool sendSignalAfterWave = false;
    public List<string> signalsAfterWave;

    public int currentWave = 0;
    #endregion

    #region Boss Spawn
    [Header("Boss Spawn")]
    public bool canSpawnBoss = true;
    public int bossAfterEnemyAmount = 0;
    public int bossAfterEnemyAmountIncrease = 0;
    public int bossAfterWaveAmount = 0;
    public int bossAfterWaveAmountIncrease = 0;
    public int bossGuardsAmount = 0;
    public int bossGuardsAmountIncrease = 0;

    public int currentBossAmount = 0;
    public bool isBossPresent = false;
    #endregion

    [Header("Others")]
    public float respawnTime = 0;

    public GameObject bossPrefab;

    [HideInInspector] public GameController gameController;
    [HideInInspector] public UIHandler uIHandler;

    bool isEnemyWaveActive = false;
    bool preparingEnemyWave = false;
    bool paused = false;

    private void Awake()
    {
        GetComponent<CircleCollider2D>().enabled = activeWhenTriggered && !onlySignalActivation;
        GameStateManager.Instance.OnSignalReceived += OnSignalReceived;
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;

        if (randomizedSpanwers)
        {
            if (!isWave)
            {
                totalAmountOfEnemiesNotWave = Random.Range(-5, 6) + totalAmountOfEnemiesNotWave;
                numberOfEnemiesPerLevelUp = Random.Range(-5, 6) + numberOfEnemiesPerLevelUp;
                numberOfEnemiesPerLevelUpIncrease = Random.Range(-5, 6) + numberOfEnemiesPerLevelUpIncrease;
            }
        }
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnSignalReceived -= OnSignalReceived;
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.Paused)
        {
            paused = true;
            return;
        }
        paused = false;
    }

    private void Update()
    {
        CheckEnemyDead();
        if (!spawnersEnabled || paused)
        {
            return;
        }
        SetupSpawn();

    }

    public void ConnectController(GameController controller)
    {
        gameController = controller;
        uIHandler = gameController.uIHandler;
    }

    public void SetupSpawn()
    {
        if (isWave)
        {
            PrepareWaveInitialCheck();
            return;
        }

        if (activeWhenTriggered && !triggered)
        {
            return;
        }
        PrepareSpawnInitialCheck();

    }

    public void PrepareWaveInitialCheck()
    {
        if (remainingWaves <= 0 && !infiniteWaves)
        {
            return;
        }

        if (!isEnemyWaveActive && !preparingEnemyWave && (needSignalAfterWave && triggered))
        {
            preparingEnemyWave = true;
            PrepareWave();
            return;
        }

        if (!isEnemyWaveActive)
        {
            return;
        }

        if (CheckAllEnemiesDead())
        {
            if (sendSignalAfterWave)
            {
                foreach (string signal in signalsAfterWave)
                {
                    GameStateManager.Instance.SendSignal(gameObject, signal);
                }
            }
            isEnemyWaveActive = false;
        }
    }

    private void PrepareWave()
    {
        preparingEnemyWave = true;

        if (canSpawnBoss)
        {
            if (currentWave >= bossAfterWaveAmount)
            {
                SpawnBoss();
                return;
            }
        }
        StartCoroutine(SendNewWave());
    }

    private IEnumerator SendNewWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.1f);
        }
        AfterWaveSetup();
    }

    private bool CheckAllEnemiesDead()
    {
        foreach (GameObject enemy in enemiesEntities)
        {
            if (enemy && !enemy.GetComponentInChildren<MonsterController>().dead)
            {
                return false;
            }
        }
        return true;
    }

    private void AfterWaveSetup()
    {
        isEnemyWaveActive = true;
        preparingEnemyWave = false;
        enemiesPerWave += enemiesPerWaveIncrease;
        if (enemyLevelIncreaseAfterCounter > 0)
        {
            enemyLevelIncreaseCounter++;
            if (enemyLevelIncreaseCounter >= enemyLevelIncreaseAfterCounter)
            {
                enemyLevelIncreaseCounter = 0;
                enemyLevel += enemyLevelIncrease;
            }
        }
        currentWave++;

        if (needSignalAfterWave)
        {
            triggered = false;
        }

        if (infiniteWaves)
        {
            return;
        }

        remainingWaves--;

        if (remainingWaves <= 0 && sendsSignalAfterEnemies)
        {
            SendSignal();
        }
    }

    private bool SpawnEnemy()
    {
        if (spawners.Count == 0)
        {
            return false;
        }

        List<EnemySpawner> spawnersCopy = new List<EnemySpawner>(spawners);
        for (int i = 0; i < spawnersCopy.Count; i++)
        {
            EnemySpawner temp = spawnersCopy[i];
            int randomIndex = Random.Range(i, spawnersCopy.Count);
            spawnersCopy[i] = spawnersCopy[randomIndex];
            spawnersCopy[randomIndex] = temp;
        }

        int randomSpawner = Random.Range(0, spawnersCopy.Count);
        EnemySpawner spawner = spawnersCopy[randomSpawner];
        if (checkSpawnTime && gameController.currentTime - spawner.lastDeadTime < spawner.spawnTime)
        {
            return false;
        }
        spawner.SpawnEnemy(gameController, this, false);
        return true;
    }

    public void PrepareSpawnInitialCheck()
    {
        if (amountOfEnemiesSpawned >= totalAmountOfEnemiesNotWave || (remainingEnemiesToSpawn <= 0 && !infiniteEnemies))
        {
            return;
        }

        if (canSpawnBoss && !isBossPresent && amountOfEnemiesKilled >= bossAfterEnemyAmount)
        {
            SpawnBoss();
        }
        int storedLevel = enemyLevel;
        enemyLevel = Mathf.Max(Random.Range(-5, 6) + enemyLevel, 1);
        bool spawned = SpawnEnemy();
        enemyLevel = storedLevel;
        if (!spawned)
        {
            return;
        }
        PostEnemySpawnCheck();
    }

    public void PostEnemySpawnCheck()
    {
        amountOfEnemiesSpawned++;
        if (amountOfEnemiesKilled > numberOfEnemiesPerLevelUp)
        {
            enemyLevel += enemyLevelIncrease;
            numberOfEnemiesPerLevelUp += numberOfEnemiesPerLevelUpIncrease;
        }
        if (infiniteEnemies)
        {
            return;
        }
        remainingEnemiesToSpawn--;
    }

    public void SpawnBoss()
    {
        if (!isBossPresent)
        {
            isBossPresent = true;
            GameObject boss = Instantiate(bossPrefab, transform.position, Quaternion.identity);
            boss.GetComponentInChildren<MonsterController>().gameController = gameController;
            boss.GetComponentInChildren<MonsterController>().playerObject = gameController.playerEntity;
            enemiesEntities.Add(boss);
            if (isWave)
            {
                isEnemyWaveActive = true;
                preparingEnemyWave = false;
            }
        }
    }

    public void CheckEnemyDead()
    {
        if (enemiesEntities.Count == 0)
        {
            return;
        }
        foreach (GameObject enemyCheck in enemiesEntities.ToList())
        {
            if (enemyCheck == null || enemyCheck.GetComponentInChildren<MonsterController>() == null)
            {
                enemiesEntities.Remove(enemyCheck);
            }
            if (enemyCheck.GetComponentInChildren<MonsterController>().dead)
            {
                foreach (EnemySpawner enemySpawner in spawners)
                {
                    if (enemySpawner.ownEnemy.Contains(enemyCheck))
                    {
                        enemySpawner.ownEnemy.Remove(enemyCheck);
                        enemySpawner.lastDeadTime = gameController.currentTime;
                    }
                }

                enemiesEntities.Remove(enemyCheck);
                Destroy(enemyCheck);
                DeadEnemy();
            }
        }
    }

    public void DeadEnemy()
    {
        if (isWave) return;

        amountOfEnemiesKilled++;
        amountOfEnemiesSpawned--;

        if (amountOfEnemiesSpawned <= 0 && remainingEnemiesToSpawn <= 0 && sendsSignalAfterEnemies && !isWave && !infiniteEnemies)
        {
            SendSignal();
        }

    }

    private void OnSignalReceived(GameObject source, string signal)
    {
        if (signal == triggererId)
        {
            triggered = !triggered;
        }
    }

    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in enemiesEntities)
        {
            Destroy(enemy);
        }
        enemiesEntities.Clear();
        amountOfEnemiesSpawned = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = false;
            if (activeWhenTriggered && !onlySignalActivation)
            {
                ClearAllEnemies();
            }
        }
    }

    private void SendSignal()
    {
        if (sendsSignalAfterEnemies && !signalSent)
        {
            foreach (string signalToSend in signalsToSend)
            {
                GameStateManager.Instance.SendSignal(gameObject, signalToSend);
            }

            signalSent = true;
            if (disabeAfterEnemies)
            {
                spawnersEnabled = false;
            }
        }
    }

}
