using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnersManager : MonoBehaviour
{
    public List<EnemySpawner> spawners;

    public bool randomizedSpanwers = false;
    public bool spawnersEnabled = true;
    public bool isWave = false;
    public int enemyLevel = 1;
    public int enemyLevelIncrease = 0;

    #region Normal Spawn
    [Header("Normal Spawn")]
    public int totalAmountOfEnemiesNotWave = 0;
    public int amountOfEnemiesSpawned = 0;
    public int amountOfEnemiesKilled = 0;
    public int numberOfEnemiesPerLevelUp = 0;
    public int numberOfEnemiesPerLevelUpIncrease = 0;
    #endregion

    #region Wave Spawn
    [Header("Wave Spawn")]
    public int enemiesPerWave = 0;
    public int enemiesPerWaveIncrease = 0;
    public int enemyLevelIncreaseAfterCounter = 0;
    public int enemyLevelIncreaseCounter = 0;

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

    public void SetupSpawn(GameController gameController)
    {
        this.gameController = gameController;
        uIHandler = gameController.uIHandler;
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
        foreach (GameObject enemy in gameController.enemiesEntities)
        {
            if (enemy && !enemy.GetComponent<MonsterController>().dead)
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
        if (amountOfEnemiesSpawned >= totalAmountOfEnemiesNotWave)
        {
            return;
        }

        if (canSpawnBoss && !isBossPresent && amountOfEnemiesKilled >= bossAfterEnemyAmount)
        {
            SpawnBoss();
        }

        SpawnEnemy();
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
    }

    public void SpawnBoss()
    {
        if (!isBossPresent)
        {
            isBossPresent = true;
            GameObject boss = Instantiate(bossPrefab, transform.position, Quaternion.identity);
            boss.GetComponent<MonsterController>().gameController = gameController;
            boss.GetComponent<MonsterController>().playerObject = gameController.playerEntity;
            gameController.enemiesEntities.Add(boss);
            if (isWave)
            {
                isEnemyWaveActive = true;
                preparingEnemyWave = false;
            }
        }
    }

    public void DeadEnemy()
    {
        if (isWave) return;

        amountOfEnemiesKilled++;
        amountOfEnemiesSpawned--;
    }

}
