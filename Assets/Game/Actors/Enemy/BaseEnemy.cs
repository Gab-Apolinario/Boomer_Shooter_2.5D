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
    protected bool collisionDetected;
    protected RaycastHit hitInfo;

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

        navAgent.stoppingDistance = attackingRange;

        ChasingFromStart();
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

        collisionDetected = Physics.Raycast(transform.position, directionToPlayer, out hitInfo, detectionRange);

        if (collisionDetected && hitInfo.collider.CompareTag("Player"))
        {
            playerDetected = true;
        }

        //Se o player estiver dentro do campo de visão e alcance de ataque, ataca
        if (playerDetected && angle <= fieldOfView / 2 && distance <= attackingRange)
        {
            currentState = EnemyState.Attacking;
            navAgent.updateRotation = false;

            //olha para o player, mas sem inclinar para cima ou para baixo
            Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(lookTarget);
        }
        else if (playerDetected)
        {
            currentState = EnemyState.Chasing;
            navAgent.updateRotation = true;
        }
        else if (!playerDetected)
        {
            currentState = EnemyState.Idle;
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
        Acoes.OnEnemyDie?.Invoke();
    }

    protected virtual void ChasingFromStart()
    {
        playerDetected = true;
        currentState = EnemyState.Chasing;
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
