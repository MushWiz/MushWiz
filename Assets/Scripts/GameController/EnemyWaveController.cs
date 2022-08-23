using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public SpawnersManager currentSpawnersManager;

    private void Awake()
    {
        gameController = GetComponent<GameController>();
        uIHandler.SetTextToZeroAlpha(uIHandler.enemyWaveText);
        uIHandler.SetTextToZeroAlpha(uIHandler.currentWaveInitialization);
        uIHandler.SetTextToZeroAlpha(uIHandler.killCountText);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    public void UpdateSpawnerManager()
    {
        currentSpawnersManager = GameObject.FindGameObjectWithTag("SpawnersManager").GetComponent<SpawnersManager>();
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

        uIHandler.currentWaveInitialization.GetComponent<TextMeshProUGUI>().text = "Enemy to kill: ";
        StartCoroutine(uIHandler.FadeTextToFullAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(uIHandler.FadeTextToZeroAlpha(1f, uIHandler.currentWaveInitialization));
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(SendNewWave());
    }

    private IEnumerator SendNewWave()
    {
        isEnemyWaveActive = true;
        preparingEnemyWave = false;

        yield return new WaitForSeconds(0.1f);

        enemyWave++;
    }

    public void ControlWaveActivation()
    {
        if (!isEnemyWaveActive && !preparingEnemyWave)
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

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateSpawnerManager();
        Reset();
    }

}