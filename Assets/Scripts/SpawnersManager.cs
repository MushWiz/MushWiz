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
    public int remainingWaves = 99999;

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

    private void Awake()
    {
        GetComponent<CircleCollider2D>().enabled = activeWhenTriggered && !onlySignalActivation;
        GameStateManager.Instance.OnSignalReceived += OnSignalReceived;

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
    }

    private void Update()
    {
        SetupSpawn();
        CheckEnemyDead();
    }

    public void ConnectController(GameController controller)
    {
        gameController = controller;
        uIHandler = gameController.uIHandler;
    }

    public void SetupSpawn()
    {
        if (!spawnersEnabled)
        {
            return;
        }
        if (activeWhenTriggered && !triggered)
        {
            return;
        }
        if (isWave)
        {
            PrepareWaveInitialCheck();
        }
        else
        {
            PrepareSpawnInitialCheck();
        }
    }

    public void PrepareWaveInitialCheck()
    {
        if (remainingWaves <= 0)
        {
            return;
        }
        if (!isEnemyWaveActive && !preparingEnemyWave)
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
        remainingWaves--;

        if (remainingWaves <= 0 && sendsSignalAfterEnemies)
        {
            SendSignal();
        }
    }

    private void SpawnEnemy()
    {
        if (spawners.Count == 0)
        {
            return;
        }

        int randomSpawner = Random.Range(0, spawners.Count);
        spawners[randomSpawner].SpawnEnemy(gameController, this, false);
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
        SpawnEnemy();
        enemyLevel = storedLevel;
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
