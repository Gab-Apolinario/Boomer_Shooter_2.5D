using UnityEngine;
using System.Collections;

public class RangedEnemy : BaseEnemy
{
    private RangedEnemyDataSO rangedData;

    protected ParticleSystem muzzleFlashInimigo;
    protected LineRenderer lineRenderer;
    protected Transform gunFront;
    [SerializeField] protected LayerMask wallLayer;
    protected Player playerScript;
    protected float cooldownTimer;
    protected bool canShoot;

    public override void Start()
    {
        base.Start();
        
        rangedData = enemyData as RangedEnemyDataSO;

        if (rangedData == null)
        {
            Debug.LogError($"[{gameObject.name}] O ScriptableObject atribuído precisa ser do tipo RangedEnemyDataSO, mas é {enemyData.GetType().Name}!");
            return;
        }

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (gunFront == null)
        {
            gunFront = transform.Find("SpriteHolder/GunFront");
        }
        // if( muzzleFlashInimigo == null)
        // {
        //     muzzleFlashInimigo = transform.Find("GunHolder/Gun_Front/Muzzle_Flash_Inimigo").GetComponent<ParticleSystem>();
        // }

        playerScript = player.GetComponent<Player>();

        canShoot = true;
        cooldownTimer = rangedData.shotCooldown;

        navAgent.stoppingDistance = rangedData.attackRange - 1f;
        lineRenderer.enabled = false;
    }

    public override void Update()
    {
        base.Update();

        if (canShoot && currentState == EnemyState.Attacking)
        {
            Shoot();
        }

        if (!canShoot)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                canShoot = true;
                cooldownTimer = rangedData.shotCooldown;
            }
        }
    }

    void Shoot()
    {
        animator.SetTrigger("Shoot");
        canShoot = false;
        cooldownTimer = rangedData.shotCooldown;
        StartCoroutine(DelayedShot());
    }

    IEnumerator DelayedShot()
    {
        yield return new WaitForSeconds(0.2f); // meio segundo de delay antes do tiro sair, para sincronizar com a animação
        
        //muzzleFlashInimigo.Play();
        Vector3 shotSpread = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
        Vector3 directionToPlayer = ((player.position - transform.position) + shotSpread).normalized;
        
        bool shotFired = Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hitInfo, rangedData.detectionRange, ~wallLayer);
        Vector3 laserDestination;

        if (shotFired && hitInfo.collider.CompareTag("Player"))
        {
            playerScript.TakeDamage(rangedData.damage, transform);
            laserDestination = hitInfo.point;
            StartCoroutine(ShowLaser(laserDestination));
        }
        else if (shotFired)
        {
            laserDestination = hitInfo.point;
            StartCoroutine(ShowLaser(laserDestination));
        }
    }

    protected override void UpdateAnimationState(EnemyState newState)
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
                animator.SetInteger("State", 1); //mantem andando no state attacking, pq o tiro é um trigger separado
                break;
        }
    }
    IEnumerator ShowLaser(Vector3 laserDestination)
    {

        //define ponto inicial e 'final' do laser. Iguais no começo
        lineRenderer.SetPosition(0, gunFront.position); //posição inicial
        lineRenderer.SetPosition(1, laserDestination); //posição final

        //ativa o lineRenderer
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.2f);
        lineRenderer.enabled = false;
    }

    public override void DetectPlayer()
    {
        base.DetectPlayer();
    }
}
