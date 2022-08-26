using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float projectileDamage = 1f;

    public float maxTravelDistance = 10f;
    public float travelTime;
    public bool deleting = false;

    public int projectileMaxReflections = 3;
    public float dampeningEffect = 0.75f;

    public float accuracy = 100;

    public Transform shooter;

    Vector2 pauseVelocity;
    float trailTime = 0.1f;
    float pauseTime;
    float resumeTime;

    [HideInInspector] public Vector2 currentVelocity;
    [HideInInspector] public Rigidbody2D rb;
    bool paused = false;

    public List<ProjectileBehaviour> behaviours = new List<ProjectileBehaviour>();

    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        foreach (ProjectileBehaviour behaviour in behaviours)
        {
            behaviour.OnProjectileCreated(this);
        }
    }

    private void Update()
    {
        if (!shooter)
        {
            Destroy(gameObject);
            return;
        }

        currentVelocity = rb.velocity;

        if (Vector3.Distance(transform.position, shooter.position) > maxTravelDistance)
        {
            Destroy(gameObject);
        }

        if (paused)
        {
            return;
        }

        travelTime += Time.deltaTime;

        foreach (ProjectileBehaviour behaviour in behaviours)
        {
            behaviour.OnProjectileTravel(this);
        }
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.GameOver || newState == GameState.MainMenu)
        {
            Destroy(gameObject);
            return;
        }

        paused = newState == GameState.Paused && newState != GameState.Playing;

        if (paused)
        {
            OnPauseGame();
        }
        else
        {
            OnResumeGame();
        }
    }

    void OnPauseGame()
    {
        pauseVelocity = rb.velocity;
        rb.velocity = Vector2.zero;

        if (TryGetComponent<TrailRenderer>(out TrailRenderer trail))
        {
            pauseTime = Time.time;
            trail.time = Mathf.Infinity;
        }

    }

    void OnResumeGame()
    {
        rb.velocity = pauseVelocity;

        if (TryGetComponent<TrailRenderer>(out TrailRenderer trail))
        {
            resumeTime = Time.time;
            trail.time = (resumeTime - pauseTime) + trailTime;
            Invoke("SetTrailTime", trailTime);
        }

    }

    void SetTrailTime()
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        trail.time = trailTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        //check the layer of the collider
        if (other.gameObject.tag == "ReflectiveObstacle")
        {
            foreach (ProjectileBehaviour behaviour in behaviours)
            {
                behaviour.OnProjectileReflected(this, other);
            }
        }
        else
        {
            foreach (ProjectileBehaviour behaviour in behaviours)
            {
                behaviour.OnProjectileHit(this, other);
            }
        }

    }

}
