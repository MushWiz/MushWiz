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
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileMaxTravel;

    public float movementSpeed = 5f;

    public bool randomStats = false;

    public bool dead = false;
    public bool isAttacking = false;
    public bool canAttack = false;

    public float experiencePoints = 3f;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public GameObject playerObject;
    [HideInInspector] public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (!gameController.enemiesEntities.Contains(gameObject))
        {
            gameController.enemiesEntities.Add(gameObject);
        }

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
            gameController.IncreaseScore();
            gameController.IncreaseExperience(experiencePoints);
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
            TakeDamage(other.gameObject.GetComponent<ProjectileStats>().projectileDamage);
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
            gameObject.GetComponent<MonsterStateController>().aiActive = false;
            return;
        }
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.Paused)
        {
            enabled = true;
            GetComponent<Collider2D>().enabled = true;
            return;
        }

        enabled = false;
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

}

public enum EnemyTier
{
    Basic,
    Advanced,
    Elite,
    Boss
}
