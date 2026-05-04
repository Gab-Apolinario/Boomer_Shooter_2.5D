using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource jumpSource;
    [SerializeField] private AudioSource shootSource;
    [SerializeField] private AudioSource overheatSource;
    [SerializeField] private AudioSource healthPickupSource;

    [Header("Clips")]
    [SerializeField] private AudioClip jumpClip;
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
        Acoes.OnJump += PlayJump;
        Acoes.PlayerAtirou += PlayShoot;
        Acoes.OnOverheat += PlayOverheat;
        Acoes.OnHealthPickup += PlayHealthPickup;
    }

    private void OnDisable()
    {
        Acoes.OnJump -= PlayJump;
        Acoes.PlayerAtirou -= PlayShoot;
        Acoes.OnOverheat -= PlayOverheat;
        Acoes.OnHealthPickup -= PlayHealthPickup;
    }

    private void PlayJump()
    {
        jumpSource.PlayOneShot(jumpClip);
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