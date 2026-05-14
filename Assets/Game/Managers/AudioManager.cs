using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource meleeSource;
    [SerializeField] private AudioSource shootSource;
    [SerializeField] private AudioSource overheatSource;
    [SerializeField] private AudioSource healthPickupSource;

    [Header("Clips")]
    [SerializeField] private AudioClip meleeClip;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip overheatClip;
    [SerializeField] private AudioClip healthPickupClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Acoes.OnMeleeAttack += PlayMelee;
        Acoes.PlayerAtirou += PlayShoot;
        Acoes.OnOverheat += PlayOverheat;
        Acoes.OnHealthPickup += PlayHealthPickup;
    }

    private void OnDisable()
    {
        Acoes.OnMeleeAttack -= PlayMelee;
        Acoes.PlayerAtirou -= PlayShoot;
        Acoes.OnOverheat -= PlayOverheat;
        Acoes.OnHealthPickup -= PlayHealthPickup;
    }

    private void PlayMelee()
    {
        meleeSource.PlayOneShot(meleeClip);
    }

    private void PlayShoot()
    {
        shootSource.PlayOneShot(shootClip);
    }

    private void PlayOverheat()
    {
        overheatSource.PlayOneShot(overheatClip);
    }

    private void PlayHealthPickup()
    {
        healthPickupSource.PlayOneShot(healthPickupClip);
    }
}