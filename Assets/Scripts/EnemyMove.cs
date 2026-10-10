using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Transform player;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    [SerializeField] private float detectionRange = 8f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updatePosition = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (player == null || !navMeshAgent.isOnNavMesh)
        {
            return;
        }

        if (CanSeePlayer())
        {
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.ResetPath();
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;



        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
