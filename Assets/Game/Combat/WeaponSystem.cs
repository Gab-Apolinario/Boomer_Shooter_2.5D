using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    #region Variáveis
    InputHandler inputHandler;
    PlayerController playerController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private Transform gunFront;

    [Header("Configurações da Arma")]
    [SerializeField] private WeaponConfigSO config;
    [SerializeField] private WeaponConfigSO[] weaponConfigs; //array para futuras armas
    [SerializeField] private int currentWeaponIndex = 0;
    [SerializeField] private bool isUsingMelee = false;
    [SerializeField] private float swapCooldown = 0.5f;
    [SerializeField] private RectTransform crosshair;

    [Header("Status da Arma")]
    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    [SerializeField] private bool isOverheated;
    [SerializeField] private float currentHeat;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private float cooldownTimer;
    [SerializeField] private float backwardSpread = 0.3f; //quanto mais para trás, mais espalhado o tiro
    private Coroutine coolingCoroutine;

    [Header("Melee System")]
    [SerializeField] private MeleeSystem meleeSystem;
    [SerializeField] private GameObject rangedWeaponModel;
    private float lastSwapTime = -999f;

    #endregion

    private void Start()
    {
        //Inicializar o InputHandler
        inputHandler = new InputHandler();
        playerController = GetComponentInParent<PlayerController>();

        #region SEGURANÇAS
        if (gunFront == null)
        {
            gunFront = transform.Find("Gun_Front");
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (meleeSystem == null)
        {
            meleeSystem = GetComponentInChildren<MeleeSystem>();
        }
        #endregion

        config = weaponConfigs[currentWeaponIndex]; //inicia com a primeira arma do array

        //Iniciação de variáveis
        cooldownTimer = config.fireRate * fireRateMultiplier;
        currentHeat = 0;
    }

    private void Update()
    {
        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            SwitchWeapon();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (Time.time - lastSwapTime >= swapCooldown)
            {
                SwapToMelee();
                lastSwapTime = Time.time;
            }
        }

        if (canShoot && inputHandler.IsShooting)
        {
            if (isUsingMelee)
            {
                MeleeAttack();
            }
            else
            {
                Shoot();
            }
        }

        if (currentHeat > 0 && !isOverheated && coolingCoroutine == null)
        {
            coolingCoroutine = StartCoroutine(GunCooling());
        }

        //Se atirou, começa cooldown
        if (!canShoot)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0 && (isUsingMelee || !isOverheated)) //atingiu o tempo de cooldown e não está carregando
            {
                canShoot = true;
                cooldownTimer = config.fireRate * fireRateMultiplier; //reseta o timer de cooldown, aplicando o multiplicador de taxa de fogo
            }
        }

        if (crosshair != null)
        {
            float dotProduct = playerController.GetMovementDot();
            float targetScale = (dotProduct < 0 )? 1.6f : 1f; //Aumenta o tamanho da mira se estiver atirando para trás
            crosshair.localScale = Vector3.Lerp(crosshair.localScale, Vector3.one * targetScale, Time.deltaTime * 8f);//Lerp para suavizar a transição do tamanho da mira
        }
    }

    void MeleeAttack()
    {
        if (meleeSystem != null && meleeSystem.TryAttack(damageMultiplier))
        {
            canShoot = false;
            cooldownTimer = meleeSystem.GetAttackDuration();
        }
    }
    void Shoot()
    {
        canShoot = false;
        cooldownTimer = config.fireRate * fireRateMultiplier;
        currentHeat += config.heatPerShot;
        float heatRatio = currentHeat / config.heatCapacity;
        var emission = smokeEffect.emission;
        emission.rateOverTime = heatRatio * config.smokeEmissionRate;
        smokeEffect.Play();
        
        Acoes.OnHeatChanged?.Invoke(currentHeat, config.heatCapacity); //fillAmount barra);
        Acoes.PlayerAtirou?.Invoke(); //PARTICULA DE MUZZLE FLASH

        if (currentHeat >= config.heatCapacity)
        {
            isOverheat();
        }

        if (currentHeat < config.heatCapacity && !isOverheated && !canShoot)
        {
            if (coolingCoroutine != null)
            {
                StopCoroutine(coolingCoroutine);
            }
            coolingCoroutine = StartCoroutine(GunCooling());
        }

#region raycast

        float dotProduct = playerController.GetMovementDot();
        //bool shot = Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, config.maxRange);
        Vector3 shootDirection = mainCamera.transform.forward; 
        
        if (dotProduct < 0) //Atirando e andando para trás
        {
            Vector2 randomCircle = Random.insideUnitCircle * backwardSpread; //gira o círculo para espalhar os tiros
            Vector3 spreadDirection = mainCamera.transform.forward + (mainCamera.transform.right * randomCircle.x) + (mainCamera.transform.up * randomCircle.y); 
            shootDirection = spreadDirection.normalized; //normaliza para manter a direção, mas com o spread aplicado
        }

        bool shot = Physics.Raycast(mainCamera.transform.position, shootDirection.normalized, out RaycastHit hitInfo, config.maxRange);
        //Verifica se o objeto atingido é um inimigo e aplica dano
        if (shot && hitInfo.collider.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            //Aciona TakeDamage() do BaseEnemy
            enemy.TakeDamage(config.damage * damageMultiplier);
        }

        if (shot) //se atingiu algo
        {
            //ACAO PARTICULA DE IMPACTO
            Acoes.OnImpact?.Invoke(hitInfo.point);
        }
#endregion        
    }

    private void isOverheat()
    {
        isOverheated = true;
        canShoot = false;
        Acoes.OnOverheat?.Invoke();

        if (coolingCoroutine != null)
        {
            StopCoroutine(coolingCoroutine);
        }
        coolingCoroutine = StartCoroutine(GunCooling());
    }

    IEnumerator GunCooling()
    {
        yield return new WaitForSeconds(config.overheatCooldownDelay);
        

        while (currentHeat > 0)
        {
            currentHeat -= config.coolingRate * Time.deltaTime;
            
            if (!isUsingMelee)
            {
                Acoes.OnHeatChanged?.Invoke(currentHeat, config.heatCapacity);
                
                if (smokeEffect.isPlaying)
                {
                    var emission = smokeEffect.emission;
                    emission.rateOverTime = (currentHeat / config.heatCapacity) * config.smokeEmissionRate;
                }
            }
            
            yield return null;
        }

        currentHeat = 0;
        isOverheated = false;
        coolingCoroutine = null;

        if (!isUsingMelee)
        {
            smokeEffect.Stop();
            Acoes.OnHeatChanged?.Invoke(currentHeat, config.heatCapacity);
            canShoot = true;
        }
    }

    void SwapToMelee()
    {
        isUsingMelee = !isUsingMelee;

        if (isUsingMelee)
        {
            Debug.Log("MODO MELEE");
            smokeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            config = weaponConfigs[weaponConfigs.Length - 1]; //melee = última do array

            if (rangedWeaponModel != null)
            {
                rangedWeaponModel.SetActive(false);                
            }
            if (crosshair != null)
            {
                crosshair.gameObject.SetActive(false);
            }

            if (meleeSystem != null)
            {
                meleeSystem.ShowWeapon();
            }
            
            canShoot = true;

            if (currentHeat > 0 && coolingCoroutine == null)
            {
                coolingCoroutine = StartCoroutine(GunCooling());
            }
        }
        else
        {
            Debug.Log($"MODO RANGED: {weaponConfigs[currentWeaponIndex].weaponName}");
            config = weaponConfigs[currentWeaponIndex]; //volta para a arma de distância atual

            if (isOverheated)
            {
                canShoot = false;
                smokeEffect.Play();
            }
            else
            {
                canShoot = true;
                cooldownTimer = config.fireRate * fireRateMultiplier;

                if (currentHeat > 0)
                {
                    smokeEffect.Play();
                    var emission = smokeEffect.emission;
                    emission.rateOverTime = (currentHeat / config.heatCapacity) * config.smokeEmissionRate;
                }
            }


            if (rangedWeaponModel != null)
            {
                rangedWeaponModel.SetActive(true);                
            }
            if (crosshair != null)
            {
                crosshair.gameObject.SetActive(true);
            }

            if (meleeSystem != null)
            {
                meleeSystem.HideWeapon();
            }

            Acoes.OnHeatChanged?.Invoke(currentHeat, config.heatCapacity);
        }
    }

    void SwitchWeapon()
    {
        if (isUsingMelee)
        {
            isUsingMelee = false;

            if (rangedWeaponModel != null)
            {
                rangedWeaponModel.SetActive(true);                
            }
            if (crosshair != null)
            {
                crosshair.gameObject.SetActive(true);
            }
        }

        currentWeaponIndex = (currentWeaponIndex + 1) % (weaponConfigs.Length -1); //avança para o próximo indice, mas volta para 0 se ultrapassar o tamanho do array (%)
        config = weaponConfigs[currentWeaponIndex]; //Substitui SO atual pela nova configuração
        Debug.Log($"Arma trocada para: {config.weaponName}");

        smokeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        StopAllCoroutines();
        canShoot = true;
        cooldownTimer = config.fireRate * fireRateMultiplier;
        //isOverheated = false;
        //currentHeat = 0;
        Acoes.OnHeatChanged?.Invoke(currentHeat, config.heatCapacity);
    }

}