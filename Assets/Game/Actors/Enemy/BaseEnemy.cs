using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    #region VARIÁVEIS
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
    [SerializeField] protected int score;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected Transform player;
    [SerializeField] protected float detectionRange;
    protected float flanckAngle;
    [SerializeField] protected Vector3 randomDestination;
    [SerializeField] protected bool isUpdatingDestination;
    [SerializeField] protected float attackingRange;
    [SerializeField] protected float offSetRadius;
    [SerializeField] protected float fieldOfView;
    [SerializeField] protected bool playerDetected;
    protected bool collisionDetected;
    protected RaycastHit hitInfo;

    #endregion

    public virtual void Start()
    {
        sprintSpeed += Random.Range(-0.5f, 1f);
        flanckAngle = Random.Range(0f, 360f);
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
            navAgent.SetDestination(randomDestination);
            navAgent.speed = sprintSpeed;
        }
    }

    IEnumerator UpdateChaseDestination()
    {
        isUpdatingDestination = true;
        
        while (currentState == EnemyState.Chasing)
        {
            Vector3 offSet = Quaternion.Euler(0, flanckAngle, 0) * Vector3.forward * offSetRadius;
            randomDestination = player.position + offSet;
            yield return new WaitForSeconds(0.5f);
        }

        isUpdatingDestination = false;
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

            if (!isUpdatingDestination)
            {
                StartCoroutine(UpdateChaseDestination());
            }
        }
        else if (!playerDetected)
        {
            currentState = EnemyState.Idle;
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
        Acoes.OnEnemyDie?.Invoke(score);
    }

    protected virtual void ChasingFromStart()
    {
        playerDetected = true;
        currentState = EnemyState.Chasing;
        StartCoroutine(UpdateChaseDestination());
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
