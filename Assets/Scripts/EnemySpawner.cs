using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<GameObject> patrolPoints = new List<GameObject>();

    public bool randomSettings = false;
    public bool permanent = false;

    public float lastSpawnTime = 0;
    public float spawnDelay = 1.5f;

    public int maxSpawnAmount = -1;

    public int groupSize = 1;

    public void SpawnEnemy(GameController gameController)
    {

        lastSpawnTime = Time.time;
        for (int i = 0; i < groupSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position, Quaternion.identity);
            enemy.GetComponent<MonsterController>().gameController = gameController;
            enemy.GetComponent<MonsterController>().playerObject = gameController.playerEntity;
            if (patrolPoints.Count > 0)
            {
                enemy.GetComponent<MonsterStateController>().SetPatrolPoints(patrolPoints);
            }
            gameController.enemiesEntities.Add(enemy);
        }

        if (!permanent || maxSpawnAmount == 0)
        {
            gameController.spawnPointsList.Remove(this.gameObject);
            Destroy(gameObject);
        }
        if (maxSpawnAmount > 0)
        {
            maxSpawnAmount--;
        }
    }

    public void RandomizeSettings(int increaseAmount = 0)
    {
        if (randomSettings)
        {
            groupSize = Mathf.Max(Random.Range(groupSize - 2 + increaseAmount / 2, groupSize + 2 + increaseAmount / 2), 1);
            spawnDelay = Mathf.Max(Random.Range(spawnDelay - 2 + increaseAmount / 2, spawnDelay + 1 + increaseAmount / 2), 0.3f);
        }
    }
}
