using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MonsterStateController : MonoBehaviour
{

    public string animationPrefix = "";

    public MonsterState currentState;
    public MonsterState remainState;

    public List<GameObject> patrolPoints = new List<GameObject>();

    [HideInInspector] public int wayPointListIndex = 0;

    [HideInInspector] public float stateTimeElapsed;
    public NavMeshAgent navMeshAgent;
    [HideInInspector] public GameObject target;
    [HideInInspector] public Animator animator;
    [HideInInspector] public MonsterController monsterController;
    [HideInInspector] public Vector2 homeBase;
    [HideInInspector] public float initialStoppingDistance;

    private void Awake()
    {
        monsterController = GetComponent<MonsterController>();
        navMeshAgent.stoppingDistance = initialStoppingDistance = Mathf.Max(monsterController.attackRange - Random.Range(0.5f, 1.5f), 0);
        Vector2 setPosition = transform.position;
        transform.position = setPosition;
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
    }

    public bool aiActive = true;

    public void CallUpdateState()
    {
        if (!aiActive)
        {
            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(navMeshAgent.nextPosition.x, navMeshAgent.nextPosition.y, 0);
            return;
        }
        currentState.UpdateState(this);
    }

    public void TransitionToState(MonsterState nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }

    public void ChangeAIState(bool isActive)
    {
        aiActive = isActive;
        if (monsterController.dead)
        {
            return;
        }
        navMeshAgent.isStopped = !isActive;
    }

    public void ResetAIState()
    {
        aiActive = true;
        navMeshAgent.isStopped = false;
    }

    public void SetPatrolPoints(List<GameObject> patrolPoints)
    {
        this.patrolPoints = patrolPoints;
    }

    public void UpdateNavMeshAgent()
    {
        navMeshAgent.speed = monsterController.movementSpeed + monsterController.GetStatValueByType(StatType.Speed) * 0.01f;
    }
}