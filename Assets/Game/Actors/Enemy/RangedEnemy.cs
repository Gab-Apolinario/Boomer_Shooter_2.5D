using UnityEngine;
using System.Collections;

public class RangedEnemy : BaseEnemy
{
    protected ParticleSystem muzzleFlashInimigo;
    protected LineRenderer lineRenderer;
    protected Transform gunFront;
    [SerializeField] protected LayerMask wallLayer;
    protected Player playerScript;

    [SerializeField] protected float shotCooldown;
    protected float cooldownTimer;
    protected bool canShoot;

    public override void Start()
    {
        base.Start();

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (gunFront == null)
        {
            gunFront = transform.Find("GunHolder/Gun_Front");
        }
        if( muzzleFlashInimigo == null)
        {
            muzzleFlashInimigo = transform.Find("GunHolder/Gun_Front/Muzzle_Flash_Inimigo").GetComponent<ParticleSystem>();
        }

        playerScript = player.GetComponent<Player>();

        canShoot = true;
        cooldownTimer = shotCooldown;

        score = 15;

        navAgent.stoppingDistance = attackingRange - 1f;
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
                cooldownTimer = shotCooldown;
            }
        }
    }

    void Shoot()
    {
        Debug.Log("Ranged Enemy Shooting!");

        muzzleFlashInimigo.Play();
        Vector3 shotSpread = new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f));

        Vector3 directionToPlayer = (player.position - transform.position).normalized + shotSpread;
        
        bool shotFired = Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hitInfo, detectionRange, ~wallLayer);
        Vector3 laserDestination;

        if (shotFired && hitInfo.collider.CompareTag("Player"))
        {
            playerScript.TakeDamage(damage);
            laserDestination = hitInfo.point;
            StartCoroutine(ShowLaser(laserDestination));
        }
        else if (shotFired)
        {
            laserDestination = hitInfo.point;
            StartCoroutine(ShowLaser(laserDestination));
        }

        canShoot = false;
        cooldownTimer = shotCooldown;
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

        if (collisionDetected && !hitInfo.collider.CompareTag("Player"))
        {
            canShoot = false;
        }
}
}
