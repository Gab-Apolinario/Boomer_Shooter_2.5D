using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image reloadCircle;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveAnnouncementText;


    [Header("Modais")]
    [SerializeField] private GameObject gameOverModal;
    [SerializeField] private GameObject victoryModal;
    [SerializeField] private GameObject timeOverModal;
    [SerializeField] private GameObject pauseModal;
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private Slider gamepadSensSlider;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Leaderboard")]
    [SerializeField] private LeaderboardManager leaderboardManager;

    private void OnEnable()
    {
        Acoes.OnPlayerHealthChanged += UpdateHealth;
        Acoes.OnAmmoChanged += UpdateAmmo;
        Acoes.OnReloadChanged += UpdateReloadCircle;
        Acoes.GameOver += ShowGameOver;
        Acoes.Victory += ShowVictory;
        Acoes.TimeOver += ShowTimeOver;
        Acoes.ResolveScore += UpdateScore;
        Acoes.ResolveTime += leaderboardManager.SetPendingTime;
        Acoes.OnWaveSpawn += OnWaveSpawn;
        Acoes.OnTimeBetweenWaves += OnTimeBetweenWaves;
    }

    private void OnDisable()
    {
        Acoes.OnPlayerHealthChanged -= UpdateHealth;
        Acoes.OnAmmoChanged -= UpdateAmmo;
        Acoes.OnReloadChanged -= UpdateReloadCircle;
        Acoes.GameOver -= ShowGameOver;
        Acoes.Victory -= ShowVictory;
        Acoes.TimeOver -= ShowTimeOver;
        Acoes.ResolveScore -= UpdateScore;
        Acoes.ResolveTime -= leaderboardManager.SetPendingTime;
        Acoes.OnWaveSpawn -= OnWaveSpawn;
        Acoes.OnTimeBetweenWaves -= OnTimeBetweenWaves;
    }

    private void Start()
    {
        if (leaderboardManager == null)
        {
            leaderboardManager = FindAnyObjectByType<LeaderboardManager>();
        }

        gameOverModal.SetActive(false);
        victoryModal.SetActive(false);
        timeOverModal.SetActive(false);
        pauseModal.SetActive(false);
        healthBar.fillAmount = 1f;
        reloadCircle.fillAmount = 0f;
        reloadCircle.gameObject.SetActive(false);
    }

    public void UpdateTimer(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateWave(int waveIndex, int totalWaves)
    {
        waveText.text = string.Format("{0}/{1}", waveIndex + 1, totalWaves);
    }

    private void UpdateHealth(float health, float maxHealth)
    {
        healthBar.fillAmount = health / maxHealth;
    }

    private void UpdateAmmo(int current, int max)
    {
        ammoText.text = $"{current}/{max}";
    }

    private void UpdateReloadCircle(bool isReloading)
    {
        reloadCircle.gameObject.SetActive(isReloading);

        if (isReloading)
            StartCoroutine(AnimateReloadCircle());
    }

    IEnumerator AnimateReloadCircle()
    {
        float elapsed = 0f;
        reloadCircle.fillAmount = 0f;

        while (elapsed < WeaponSystem.ReloadTime)
        {
            elapsed += Time.deltaTime;
            reloadCircle.fillAmount = elapsed / WeaponSystem.ReloadTime;
            yield return null;
        }

        reloadCircle.fillAmount = 1f;
    }

    private void UpdateScore(int score)
    {
        scoreText.text = score.ToString("D4");
        leaderboardManager.SetPendingScore(score);
    }

    private void OnWaveSpawn(int waveIndex)
    {
        StartCoroutine(WaveSpawn(waveIndex));
    }

    IEnumerator WaveSpawn(int waveIndex)
    {
        waveAnnouncementText.gameObject.SetActive(true);
        waveAnnouncementText.text = $"Wave {waveIndex} Spawnada!";
        yield return new WaitForSeconds(2.5f);
        waveAnnouncementText.gameObject.SetActive(false);
    }

    private void OnTimeBetweenWaves(int timeRemaining)
    {
        waveAnnouncementText.gameObject.SetActive(true);
        waveAnnouncementText.text = $"Proxima wave em {timeRemaining}";
    }

    private void DisableUI()
    {
        healthBar.gameObject.SetActive(false);
        ammoText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        waveText.gameObject.SetActive(false);
        reloadCircle.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        waveAnnouncementText.gameObject.SetActive(false);
    }

    public void ShowPause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mouseSensSlider.value = SettingsManager.MouseSensibility;
        gamepadSensSlider.value = SettingsManager.GamepadSensibility;
        pauseModal.SetActive(true);
    }

    public void HidePause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseModal.SetActive(false);
    }

    public void OnMouseSliderChanged(float value)
    {
        SettingsManager.SetMouseSensibility(value);
    }

    public void OnGamepadSliderChanged(float value)
    {
        SettingsManager.SetGamepadSensibility(value);
    }

    private void ShowGameOver()
    {
        DisableUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverModal.SetActive(true);
        leaderboardManager.OpenGameOver();
    }
    private void ShowVictory()
    {
        DisableUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        victoryModal.SetActive(true);
        leaderboardManager.OpenVictory();
    }
    private void ShowTimeOver()
    {
        DisableUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        timeOverModal.SetActive(true);
        leaderboardManager.OpenTimeOver();
    }
}