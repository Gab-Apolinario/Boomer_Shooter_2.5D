using System.Collections;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    InputHandler inputHandler;


    [SerializeField] private Transform gunFront;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float shotMaxRange;
    [SerializeField] private float shotCooldown;
    private float cooldownTimer;
    [SerializeField] private float shotDamage;
    [SerializeField] private int bulletsToShoot;
    [SerializeField] private int chargerCapacity;
    [SerializeField] private float reloadTime;
    [SerializeField] private bool isReloading;
    private bool canShoot = true;

    private int boltSize = 3;
    private float boltSpeed = 80f;
    private Vector3 boltPosition;
    private Vector3 boltDirection;


    private void Start()
    {
        inputHandler = new InputHandler();

        if (gunFront == null)
        {
            gunFront = transform.Find("Gun_Front");
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        cooldownTimer = shotCooldown;
        chargerCapacity = 10;
        bulletsToShoot = chargerCapacity;
        reloadTime = 1.5f;
    }

    private void Update()
    {
        if (bulletsToShoot > 0 && canShoot && inputHandler.IsShooting)
        {
            Shoot();
        }

        if (bulletsToShoot < chargerCapacity && canShoot && !isReloading && inputHandler.IsReloading)
        {
            StartCoroutine(Reload());
        }

        //Se atirou, começa cooldown
        if (!canShoot)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0 && !isReloading) //atingiu o tempo de cooldown e não está carregando
            {
                canShoot = true;
                cooldownTimer = shotCooldown;
            }
        }
    }

    void Shoot()
    {
        Debug.Log("Atirou!");
        bulletsToShoot--;

        //retorna um bool
        bool shot = Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, shotMaxRange);
        Vector3 laserDestination; //local onde o raycast bate

        //Verifica se o objeto atingido é um inimigo e aplica dano
        if (shot && hitInfo.collider.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            //Aciona TakeDamage() do BaseEnemy
            enemy.TakeDamage(shotDamage);
            Debug.Log("Tiro atingiu inimigo");
        }

        canShoot = false;
        cooldownTimer = shotCooldown;

        if (shot) //se atingiu algo
        {
            laserDestination = hitInfo.point;
            Debug.Log("Raycast bateu em: " + hitInfo.collider.name);
        }
        else //laser vai pra frente até distância máxima
        {
            laserDestination = mainCamera.transform.position + mainCamera.transform.forward * shotMaxRange;
        }

        boltDirection = mainCamera.transform.forward;
        StopAllCoroutines();
        StartCoroutine(ShowLaser(laserDestination));
    }

    IEnumerator ShowLaser(Vector3 laserDestination)
    {
        //laser começa na ponta da arma
        boltPosition = gunFront.position;

        //define ponto inicial e 'final' do laser. Iguais no começo
        lineRenderer.SetPosition(0, gunFront.position); //posição inicial
        lineRenderer.SetPosition(1, boltPosition); //posição final

        //ativa o lineRenderer
        lineRenderer.enabled = true;
        
        //Enquanto o laser não chegar no hit do raycast
        while(Vector3.Distance(boltPosition, laserDestination) > 0.1f)
        {
            //move o bolt para o destino
            boltPosition = Vector3.MoveTowards(boltPosition, laserDestination, boltSpeed * Time.deltaTime);
            yield return null; //espera um frame
            //atualiza os pontos com a posição nova
            lineRenderer.SetPosition(0, boltPosition); //traseira vira boltPosition atualizada
            lineRenderer.SetPosition(1, boltPosition + boltDirection * boltSize); //frente vai boltSize unidades para frente
        }

        //Quando chega no destino (sai do While) desativa o lineRenderer
        lineRenderer.enabled = false;
    }

    IEnumerator Reload()
    {
        Debug.Log("Recarregando Arma!");
        isReloading = true;
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        bulletsToShoot = chargerCapacity;
        isReloading = false;
        canShoot = true;
    }

}