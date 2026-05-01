using System.Collections;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    #region Variáveis
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
    public static float ReloadTime { get; private set; }

    [SerializeField] private bool isReloading;
    private bool canShoot = true;

    private int boltSize = 4;
    private float boltSpeed = 100f;
    private Vector3 boltPosition;
    private Vector3 boltDirection;
    #endregion

    private void Start()
    {
        //Inicializar o InputHandler
        inputHandler = new InputHandler();

        #region Segurança para pegar referências se esquecer de arrastar no inspector
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
        #endregion

        //Iniciação de variáveis
        lineRenderer.enabled = false;
        cooldownTimer = shotCooldown;
        chargerCapacity = 15;
        bulletsToShoot = chargerCapacity;
        ReloadTime = 1.2f;
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
        Acoes.OnAmmoChanged?.Invoke(bulletsToShoot, chargerCapacity);
        Acoes.PlayerAtirou?.Invoke(); //PARTICULA DE MUZZLE FLASH

        //retorna um bool
        bool shot = Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, shotMaxRange);

        //Verifica se o objeto atingido é um inimigo e aplica dano
        if (shot && hitInfo.collider.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            //Aciona TakeDamage() do BaseEnemy
            enemy.TakeDamage(shotDamage);
        }

        canShoot = false;
        cooldownTimer = shotCooldown;

        if (shot) //se atingiu algo
        {
            //ACAO PARTICULA DE IMPACTO
            Acoes.OnImpact?.Invoke(hitInfo.point);
            Debug.Log("Raycast bateu em: " + hitInfo.collider.name);
        }
    }

    IEnumerator Reload()
    {
        Acoes.OnReloadChanged?.Invoke(true);
        Debug.Log("Recarregando Arma!");
        isReloading = true;
        canShoot = false;
        yield return new WaitForSeconds(ReloadTime);
        bulletsToShoot = chargerCapacity;
        isReloading = false;
        Acoes.OnReloadChanged?.Invoke(false);
        Acoes.OnAmmoChanged?.Invoke(bulletsToShoot, chargerCapacity);
        canShoot = true;
    }

}