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

    public GameObject[] enemiesPrefabs;

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

    private IEnumerator PrepareWave()
    {
        preparingEnemyWave = true;
        uIHandler.currentWaveInitialization.GetComponent<TextMeshProUGUI>().text = "Wave " + enemyWave.ToString();
        uIHandler.enemyWaveText.GetComponent<TextMeshProUGUI>().text = "Wave " + enemyWave.ToString();
        StartCoroutine(uIHandler.FadeTextToFullAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(uIHandler.FadeTextToZeroAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);

        uIHandler.currentWaveInitialization.GetComponent<TextMeshProUGUI>().text = "Enemy to kill: " + enemyPerWave.ToString();
        StartCoroutine(uIHandler.FadeTextToFullAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(uIHandler.FadeTextToZeroAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(SendNewWave());
    }

    private IEnumerator SendNewWave()
    {
        for (int i = 0; i < enemyPerWave; i++)
        {

            Vector2 spawnPoint = Random.insideUnitCircle.normalized * 10 * Random.Range(0.85f, 1.15f) + (Vector2)gameController.playerEntity.transform.position; ;

            //Find safe spawn point
            while (Physics2D.OverlapCircle(spawnPoint, 1f, LayerMask.GetMask("Obstacle")) != null)
            {
                spawnPoint = Random.insideUnitCircle.normalized * 10 * Random.Range(0.85f, 1.15f) + (Vector2)gameController.playerEntity.transform.position;
            }

            SpawnEnemy(spawnPoint);
            yield return new WaitForSeconds(0.1f);
        }
        enemyPerWave = Mathf.FloorToInt(enemyPerWave * 1.25f);
        isEnemyWaveActive = true;
        preparingEnemyWave = false;
        enemyWave++;
    }

    private void SpawnEnemy(Vector2 spawnPoint)
    {
        GameObject enemyToSpawn = enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)];

        GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity);

        gameController.enemiesEntities.Add(spawnedEnemy);
    }

    private void ControlWaveActivation()
    {
        if (!isEnemyWaveActive && !preparingEnemyWave && enemiesPrefabs.Length > 0)
        {
            preparingEnemyWave = true;
            StartCoroutine(PrepareWave());
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