using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    #region Variáveis
    InputHandler inputHandler;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private Transform gunFront;

    [Header("Configurações da Arma")]
    [SerializeField] private WeaponConfigSO config;
    [SerializeField] private WeaponConfigSO[] weaponConfigs; //array para futuras armas
    [SerializeField] private int currentWeaponIndex = 0;
    [SerializeField] private bool isOverheated;
    [SerializeField] private float currentHeat;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private float cooldownTimer;
    private Coroutine coolingCoroutine;

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
        #endregion

        //Iniciação de variáveis
        cooldownTimer = config.fireRate;
        currentHeat = 0;
    }

    private void Update()
    {
        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            SwitchWeapon();
        }

        if (canShoot && inputHandler.IsShooting)
        {
            Shoot();
        }

        //Se atirou, começa cooldown
        if (!canShoot)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0 && !isOverheated) //atingiu o tempo de cooldown e não está carregando
            {
                canShoot = true;
                cooldownTimer = config.fireRate;
            }
        }
    }

    void Shoot()
    {
        canShoot = false;
        cooldownTimer = config.fireRate;
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
        //retorna um bool
        bool shot = Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, config.maxRange);

        //Verifica se o objeto atingido é um inimigo e aplica dano
        if (shot && hitInfo.collider.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            //Aciona TakeDamage() do BaseEnemy
            enemy.TakeDamage(config.damage);
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
        Debug.Log("Arma superaquecida!");

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
            Acoes.OnHeatChanged?.Invoke(currentHeat, config.heatCapacity); //fillAmount barra);
            var emission = smokeEffect.emission;
            emission.rateOverTime = (currentHeat / config.heatCapacity) * config.smokeEmissionRate;
            yield return null;
        }

        smokeEffect.Stop();
        currentHeat = 0;
        isOverheated = false;
        coolingCoroutine = null;
        canShoot = true;
        Debug.Log("Arma resfriada, pronta para atirar novamente.");
    }

    void SwitchWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponConfigs.Length; //avança para o próximo indice, mas volta para 0 se ultrapassar o tamanho do array (%)
        config = weaponConfigs[currentWeaponIndex]; //Substitui SO atual pela nova configuração
        Debug.Log($"Arma trocada para: {config.weaponName}");

        StopAllCoroutines();
        canShoot = true;
        isOverheated = false;
        cooldownTimer = config.fireRate;
        currentHeat = 0;
        Acoes.OnHeatChanged?.Invoke(currentHeat, config.heatCapacity); //reset fillAmount barra);
    }

}