using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<GameObject> patrolPoints = new List<GameObject>();
    public float lastSpawnTime = 0;

    public void SpawnEnemy(GameController gameController, SpawnersManager spawnersManager, bool isBoss)
    {
        lastSpawnTime = Time.time;
        GameObject enemy;
        if (spawnersManager.useManagerPrefabs)
        {
            enemy = Instantiate(spawnersManager.enemiesPrefabs[Random.Range(0, spawnersManager.enemiesPrefabs.Count)], transform.position, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position, Quaternion.identity);
        }
        enemy.transform.SetParent(transform);
        MonsterController monsterController = enemy.GetComponent<MonsterController>();
        MonsterStateController monsterStateController = enemy.GetComponent<MonsterStateController>();
        monsterController.gameController = gameController;
        monsterController.playerObject = gameController.playerEntity;
        monsterController.enemyLevel = spawnersManager.enemyLevel;
        monsterController.Setup(spawnersManager.isWave);
        monsterStateController.homeBase = transform.position;
        if (patrolPoints.Count > 0)
        {
            monsterStateController.SetPatrolPoints(patrolPoints);
        }
        spawnersManager.enemiesEntities.Add(enemy);
    }


}
