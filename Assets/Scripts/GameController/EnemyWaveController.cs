using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyWaveController : MonoBehaviour
{
    public float enemySpawnTime = 3f;
    private float initialEnemyTimer = 3f;

    public int maxEnemySpawns = 100;
    public EnemyTier initialEnemyTier = EnemyTier.Basic;

    private float enemyTimer;
    private EnemyTier enemyTier;

    public int enemyWave = 1;
    public int enemyPerWave = 5;
    public bool preparingEnemyWave = false;
    public bool isEnemyWaveActive = false;

    public List<GameObject> enemySpawners = new List<GameObject>();

    public UIHandler uIHandler;

    private GameController gameController;

    private void Awake()
    {
        gameController = GetComponent<GameController>();
        uIHandler.SetTextToZeroAlpha(uIHandler.enemyWaveText);
        uIHandler.SetTextToZeroAlpha(uIHandler.currentWaveInitialization);
        uIHandler.SetTextToZeroAlpha(uIHandler.killCountText);
    }

    public void Reset()
    {
        enemyTimer = initialEnemyTimer;
        enemyTier = initialEnemyTier;
        enemyWave = 1;
        enemyPerWave = 5;
        preparingEnemyWave = false;
        isEnemyWaveActive = false;
    }

    private IEnumerator PrepareWave(List<GameObject> enemySpawnPoints)
    {
        preparingEnemyWave = true;
        uIHandler.currentWaveInitialization.GetComponent<TextMeshProUGUI>().text = "Wave " + enemyWave.ToString();
        uIHandler.enemyWaveText.GetComponent<TextMeshProUGUI>().text = "Wave " + enemyWave.ToString();
        StartCoroutine(uIHandler.FadeTextToFullAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(uIHandler.FadeTextToZeroAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);

        List<GameObject> enemySpawnPointsCopy = new List<GameObject>(enemySpawnPoints);
        List<GameObject> spawnPoints = new List<GameObject>();
        for (int i = 0; i < Mathf.Max(enemyWave / 2, 1); i++)
        {
            if (spawnPoints.Count == enemySpawnPoints.Count)
            {
                break;
            }

            spawnPoints.Add(enemySpawnPointsCopy[Random.Range(0, enemySpawnPointsCopy.Count)]);
            enemySpawnPointsCopy.Remove(spawnPoints[i]);
        }


        int enemiesAmount = 0;
        foreach (GameObject enemySpawnPoint in spawnPoints)
        {
            enemySpawnPoint.GetComponent<EnemySpawner>().RandomizeSettings(enemyWave);
            enemiesAmount += enemySpawnPoint.GetComponent<EnemySpawner>().groupSize;
        }

        uIHandler.currentWaveInitialization.GetComponent<TextMeshProUGUI>().text = "Enemy to kill: " + enemiesAmount.ToString();
        StartCoroutine(uIHandler.FadeTextToFullAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(uIHandler.FadeTextToZeroAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(SendNewWave(spawnPoints));
    }

    private IEnumerator SendNewWave(List<GameObject> enemySpawnPoints)
    {
        isEnemyWaveActive = true;
        preparingEnemyWave = false;

        foreach (GameObject spawnPoint in enemySpawnPoints)
        {
            EnemySpawner spawner = spawnPoint.GetComponent<EnemySpawner>();
            spawner.SpawnEnemy(gameController);
            yield return new WaitForSeconds(0.1f);
        }
        enemyWave++;
    }

    public void ControlWaveActivation(List<GameObject> enemySpawnPoints)
    {
        if (!isEnemyWaveActive && !preparingEnemyWave && enemySpawnPoints.Count > 0)
        {
            preparingEnemyWave = true;
            StartCoroutine(PrepareWave(enemySpawnPoints));
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

}