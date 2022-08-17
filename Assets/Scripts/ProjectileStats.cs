using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStats : MonoBehaviour
{
    public float projectileDamage = 1f;

    public float maxTravelDistance = 10f;

    public int projectileMaxReflections = 3;
    public float dampeningEffect = 0.75f;

    public Transform shooter;

    Vector2 pauseVelocity;
    float trailTime = 0.1f;
    float pauseTime;
    float resumeTime;

    Vector2 currentVelocity;
    Rigidbody2D rb;

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
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.GameOver || newState == GameState.MainMenu)
        {
            Destroy(gameObject);
            return;
        }

        bool paused = newState == GameState.Paused && newState != GameState.Playing;

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
            if (projectileMaxReflections > 0)
            {
                //reflect the projectile
                Vector2 newVelocity = Vector2.Reflect(currentVelocity, other.contacts[0].normal);
                rb.velocity = newVelocity * dampeningEffect;
                projectileMaxReflections--;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }

}
