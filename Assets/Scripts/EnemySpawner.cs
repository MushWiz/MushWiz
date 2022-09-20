using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<GameObject> patrolPoints = new List<GameObject>();
    public float lastDeadTime = 0;
    public float spawnTime = 0;

    [HideInInspector] public List<GameObject> ownEnemy = new List<GameObject>();

    public void SpawnEnemy(GameController gameController, SpawnersManager spawnersManager, bool isBoss)
    {
        GameObject enemy;
        if (spawnersManager.useManagerPrefabs)
        {
            enemy = Instantiate(spawnersManager.enemiesPrefabs[Random.Range(0, spawnersManager.enemiesPrefabs.Count)], transform.position, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position, Quaternion.identity);
        }
        SpriteRenderer mobSprite = enemy.GetComponentInChildren<SpriteRenderer>();
        Color tmp = enemy.GetComponentInChildren<SpriteRenderer>().color;
        tmp.a = 0f;
        enemy.GetComponentInChildren<SpriteRenderer>().color = tmp;
        enemy.transform.SetParent(transform);
        MonsterController monsterController = enemy.GetComponentInChildren<MonsterController>();
        MonsterStateController monsterStateController = enemy.GetComponentInChildren<MonsterStateController>();
        monsterController.gameController = gameController;
        monsterController.playerObject = gameController.playerEntity;
        monsterController.enemyLevel = spawnersManager.enemyLevel;
        monsterController.Setup(spawnersManager.isWave);
        monsterStateController.homeBase = transform.position;
        StartCoroutine(FadeInMob(mobSprite));
        if (patrolPoints.Count > 0)
        {
            monsterStateController.SetPatrolPoints(patrolPoints);
        }
        spawnersManager.enemiesEntities.Add(enemy);
        ownEnemy.Add(enemy);
        spawnTime = Random.Range(25, 100);
        lastDeadTime = 0;
    }

    public IEnumerator FadeInMob(SpriteRenderer mobSprite)
    {
        {
            float alphaVal = mobSprite.color.a;
            Color tmp = mobSprite.color;

            while (mobSprite.color.a < 1)
            {
                alphaVal += 0.05f;
                tmp.a = alphaVal;
                mobSprite.color = tmp;

                yield return new WaitForSeconds(0.05f); // update interval
            }
        }

    }
}
