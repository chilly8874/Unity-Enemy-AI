using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Transform player;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;

    [SerializeField] private float detectionRange = 8f;

    private Vector3 lastKnownPlayerPosition;
    [SerializeField] private EnemyState currentState;
    private enum EnemyState
    {
        Patrol,
        Chase,
        Search
    }

    [SerializeField] private Transform[] patrolPoints;

    private int currentPatrolIndex = 0;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updatePosition = false;
    }

    void Start()
    {
        if (patrolPoints.Length > 0)
        {
            rb.position = patrolPoints[currentPatrolIndex].position + Vector3.up * navMeshAgent.baseOffset;
            navMeshAgent.Warp(patrolPoints[currentPatrolIndex].position);
            GoCurrentPatrolPoint();
            currentState = EnemyState.Patrol;
        }
    }

    void Update()
    {
        if (player == null || !navMeshAgent.isOnNavMesh)
        {
            return;
        }
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (CanSeePlayer())
                {
                    currentState = EnemyState.Chase;
                }
                else if (HasArrived())
                {
                    GoNextPatrolPoint();
                }
                break;

            case EnemyState.Chase:
                if (CanSeePlayer())
                {
                    navMeshAgent.SetDestination(player.position);
                    lastKnownPlayerPosition = player.position;
                }
                else
                {
                    currentState = EnemyState.Search;
                    navMeshAgent.SetDestination(lastKnownPlayerPosition);
                }
                break;

            case EnemyState.Search:
                if (CanSeePlayer())
                {
                    currentState = EnemyState.Chase;
                }
                else if (HasArrived())
                {
                    GoCurrentPatrolPoint();
                    currentState = EnemyState.Patrol;
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if (player == null || !navMeshAgent.isOnNavMesh)
        {
            return;
        }

        Vector3 direction = Vector3.ClampMagnitude(navMeshAgent.desiredVelocity, navMeshAgent.speed);

        rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
        navMeshAgent.nextPosition = rb.position;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, detectionRange);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2, currentState.ToString(), style);
#endif     
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void GoCurrentPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    private void GoNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        GoCurrentPatrolPoint();
    }

    private bool HasArrived()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }
}
