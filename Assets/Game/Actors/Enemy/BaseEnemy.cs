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

    [Header("Animation")]
    protected Animator animator;
    private bool isDamaged;
    private EnemyState previousState;
    private bool isDead;
    #endregion

    void OnEnable()
    {
        Acoes.OnAllEnemiesDead += DestroyEnemy;
    }

    void OnDisable()
    {
        Acoes.OnAllEnemiesDead -= DestroyEnemy;
    }
    
    public virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
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
        if (isDead) return; //proteção para não executar nada se já estiver morto
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
            Vector3 candidateDestination = player.position + offSet;

            if (NavMesh.SamplePosition(candidateDestination, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                randomDestination = navHit.position;
            }
            else
            {
                randomDestination = player.position;
            }
            yield return new WaitForSeconds(0.5f);
        }

        isUpdatingDestination = false;
    }

    public virtual void TakeDamage(float amount)
    {
        if(isDead) return; //proteção para não executar nada se já estiver morto

        isDamaged = true;
        playerDetected = true;
        currentState = EnemyState.Chasing;
        animator.SetInteger("State", 4);

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
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
            if (currentState == EnemyState.Attacking)
            {
                isUpdatingDestination = false; //para de atualizar o destino enquanto ataca
            }

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

        if (currentState != previousState && !isDamaged)
        {
            previousState = currentState;
            UpdateAnimationState(currentState);
        }
    }

    protected virtual void UpdateAnimationState(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Idle:
                    animator.SetInteger("State", 0);
                    break;
                case EnemyState.Chasing:
                    animator.SetInteger("State", 1);
                    break;
                case EnemyState.Attacking:
                    animator.SetInteger("State", 2);
                    break;
        }
    }

    public virtual void Die()
    {
        isDead = true;
        isDamaged = false;
        animator.SetInteger("State", 3); //Morte
        GetComponent<Collider>().enabled = false;
        navAgent.enabled = false;
        Acoes.OnEnemyDie?.Invoke(enemyData.score);
    }

    public virtual void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public virtual void OnDamageEnd()
    {
        Debug.Log("OnDamageEnd chamado, isDead = " + isDead);
        if(isDead) return; //proteção para não executar nada se já estiver morto
        isDamaged = false;
        currentState = EnemyState.Chasing;
        previousState = EnemyState.Chasing;
        animator.SetInteger("State", 1); //Correndo
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
