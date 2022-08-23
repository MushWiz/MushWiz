using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    public EnemyTier tier = EnemyTier.Basic;

    public Image lifeBar;
    public GameController gameController;

    public float lifePoints = 4f;
    public float maxLifePoints = 4f;

    public float damageDealer = 1f;
    public float chaseRange = 5f;
    public float attackRange = 5f;
    public float attackRate = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileMaxTravel;

    public float movementSpeed = 5f;

    public bool randomStats = false;

    public bool dead = false;
    public bool isAttacking = false;
    public bool canAttack = false;

    public float experiencePointsGiven = 3f;

    public int enemyLevel = 1;
    public float currentExperiencePoints = 0f;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public GameObject playerObject;
    [HideInInspector] public Animator animator;
    [HideInInspector] public MonsterStateController monsterStateController;

    public List<Stats> stats = new List<Stats>(){
        new Stats(10f, StatType.Health),
        new Stats(5f, StatType.Intelligence),
        new Stats(5f, StatType.Speed),
        new Stats(0f, StatType.Defense),
        new Stats(0f, StatType.Evasion),
        new Stats(0f, StatType.BlockChance),
    };

    // Start is called before the first frame update
    void Start()
    {
        if (randomStats)
        {
            maxLifePoints = Random.Range(Mathf.Max(maxLifePoints / 1.5f, 1), maxLifePoints * 1.5f);
            damageDealer = Random.Range(Mathf.Max(damageDealer / 1.5f, 1), damageDealer * 1.5f);
            movementSpeed = Random.Range(Mathf.Max(movementSpeed / 1.5f, 1), movementSpeed * 1.5f);
        }

        lifePoints = maxLifePoints;

        transform.localScale = new Vector3(maxLifePoints / 8, maxLifePoints / 8, 1);

    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        monsterStateController = GetComponent<MonsterStateController>();
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    public void TakeDamage(float damage)
    {
        lifePoints -= damage;
        if (lifePoints <= 0)
        {
            dead = true;
            gameController.RegisterEnemyDeath(experiencePointsGiven);
        }
        lifeBar.fillAmount = lifePoints / maxLifePoints;
    }

    public void Heal(float heal)
    {
        lifePoints = Mathf.Min(maxLifePoints, lifePoints + heal);
        lifeBar.fillAmount = lifePoints / maxLifePoints;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ability")
        {
            other.gameObject.GetComponent<AbilityStats>().OnHit(this);
        }

        if (other.gameObject.tag == "Projectile")
        {
            TakeDamage(other.gameObject.GetComponent<ProjectileController>().projectileDamage);
            Destroy(other.gameObject);
        }
    }

    public void Reset()
    {
        lifePoints = maxLifePoints;
        lifeBar.fillAmount = lifePoints / maxLifePoints;
        dead = false;
    }

    private void Update()
    {

        if (!playerObject)
            return;

        if (dead)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<MonsterStateController>().ChangeAIState(false);
            return;
        }

        monsterStateController.CallUpdateState();
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.Paused)
        {
            enabled = true;
            gameObject.GetComponent<MonsterStateController>().ChangeAIState(true);
            GetComponent<Collider2D>().enabled = true;
            return;
        }

        enabled = false;
        gameObject.GetComponent<MonsterStateController>().ChangeAIState(false);
        GetComponent<Collider2D>().enabled = false;
    }

    public void CanAttack()
    {
        canAttack = true;
    }

    public void CannotAttack()
    {
        canAttack = false;
        isAttacking = false;
    }

    public void Setup(bool followsIndefinetly)
    {
        for (int i = 0; i < enemyLevel; i++)
        {
            //Pick a random stat to increase
            int randomStat = Random.Range(0, stats.Count);
            stats[randomStat].IncreaseValue(3);
        }
        monsterStateController.UpdateNavMeshAgent();
        if (followsIndefinetly)
        {
            chaseRange = Mathf.Infinity;
        }
    }

    public void LevelUp()
    {
        enemyLevel++;
        int randomStat = Random.Range(0, stats.Count);
        stats[randomStat].IncreaseValue(3);
    }

    public void IncreaseExperience(float experience)
    {
        currentExperiencePoints += experience;
        if (currentExperiencePoints >= enemyLevel * 10)
        {
            LevelUp();
        }
    }

    public float GetStatValueByType(StatType statType)
    {
        foreach (Stats stat in stats)
        {
            if (stat.GetStatType() == statType)
            {
                return stat.GetValue();
            }
        }
        Debug.LogError("Stat not found: " + statType.ToString());
        return 0f;
    }

    public IEnumerator MeleeAnimation(Vector2 targetPosition, float time)
    {
        Vector2 originalPosition = transform.position;
        Vector3 direction = (targetPosition - originalPosition).normalized;
        transform.position = Vector2.Lerp(transform.position + direction, targetPosition, time * 0.1f);
        yield return new WaitForSeconds(time * 0.1f);
        transform.position = originalPosition;
        monsterStateController.navMeshAgent.isStopped = false;
    }
}

public enum EnemyTier
{
    Basic,
    Advanced,
    Elite,
    Boss
}
