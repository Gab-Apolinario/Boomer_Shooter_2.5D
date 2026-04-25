using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    #region ID
    protected enum EnemyState
    {
        Idle,
        Chasing,
        Attacking
    }

    [SerializeField] protected EnemyState currentState;

    [SerializeField] protected Color originalColor;
    [SerializeField] protected MeshRenderer meshRenderer;

    [SerializeField] protected float health;
    //[SerializeField] protected float moveSpeed;
    [SerializeField] protected float sprintSpeed;
    [SerializeField] protected float damage;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected Transform player;
    [SerializeField] protected float detectionRange;
    [SerializeField] protected float attackingRange;
    [SerializeField] protected float fieldOfView;
    [SerializeField] protected bool playerDetected;

    #endregion

    public virtual void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        if (navAgent == null)
        {
            navAgent = GetComponent<NavMeshAgent>();
        }
    }

    public virtual void Update()
    {
        Move();

        DetectPlayer();
    }

    public virtual void Move()
    {
        if (currentState == EnemyState.Chasing)
        {
            navAgent.SetDestination(player.position);
            navAgent.speed = sprintSpeed;
        }
        else if (currentState == EnemyState.Attacking)
        {
            //atacar
            navAgent.ResetPath();
        }
        else
        {
            //parado e patrulhadno depois
            navAgent.ResetPath();
        }
    }

    public virtual void TakeDamage(float amount)
    {
        StartCoroutine(FlashRed());

        playerDetected = true;
        currentState = EnemyState.Chasing;

        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        meshRenderer.material.color = originalColor;
    }

    public virtual void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        bool collisionDetected = Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hitInfo, detectionRange);

        if (collisionDetected && hitInfo.collider.CompareTag("Player"))
        {
            playerDetected = true;
        }

        if (playerDetected && angle <= fieldOfView / 2 && distance <= attackingRange)
        {
            currentState = EnemyState.Attacking;
        }
        else if (playerDetected && angle <= fieldOfView/2 && distance <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
        else if (!playerDetected)
        {
            currentState = EnemyState.Idle;
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Vector3 leftDir = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftDir * detectionRange);
        Gizmos.DrawRay(transform.position, rightDir * detectionRange);
    }
}
