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

    [SerializeField] protected EnemyDataSO enemyData;

    [SerializeField] protected float currentHealth;
    //[SerializeField] protected float moveSpeed;
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected Transform player;
    protected float flanckAngle;
    [SerializeField] protected Vector3 randomDestination;
    [SerializeField] protected bool isUpdatingDestination;
    [SerializeField] protected float offSetRadius;
    [SerializeField] protected bool playerDetected;
    protected bool collisionDetected;
    protected RaycastHit hitInfo;
    protected float currentSprintSpeed;

    #endregion

    public virtual void Start()
    {
        currentHealth = enemyData.maxHealth;
        currentSprintSpeed = enemyData.sprintSpeed + Random.Range(-0.5f, 0.8f);

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

        navAgent.stoppingDistance = enemyData.attackRange;

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
            navAgent.speed = currentSprintSpeed;
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

        currentHealth -= amount;

        if (currentHealth <= 0)
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

        collisionDetected = Physics.Raycast(transform.position, directionToPlayer, out hitInfo, enemyData.detectionRange);

        if (collisionDetected && hitInfo.collider.CompareTag("Player"))
        {
            playerDetected = true;
        }

        //Se o player estiver dentro do campo de visão e alcance de ataque, ataca
        if (playerDetected && angle <= enemyData.fieldOfView / 2 && distance <= enemyData.attackRange)
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
        Acoes.OnEnemyDie?.Invoke(enemyData.score);
    }

    protected virtual void ChasingFromStart()
    {
        playerDetected = true;
        currentState = EnemyState.Chasing;
        StartCoroutine(UpdateChaseDestination());
    }

    private void OnDrawGizmos()
    {
        if (enemyData == null) return; //proteção

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);
        Vector3 leftDir = Quaternion.Euler(0, -enemyData.fieldOfView / 2, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, enemyData.fieldOfView / 2, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftDir * enemyData.detectionRange);
        Gizmos.DrawRay(transform.position, rightDir * enemyData.detectionRange);
    }
}
