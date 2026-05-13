using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region Variáveis
    [Header("HUD")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveAnnouncementText;
    [SerializeField] private Image heatBar;
    [SerializeField] private Gradient heatGradient;
    [Header("Modais")]
    
    [SerializeField] private GameObject gameOverModal;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameObject victoryModal;
    [SerializeField] private TextMeshProUGUI victoryScoreText;
    [SerializeField] private GameObject timeOverModal;
    [SerializeField] private TextMeshProUGUI timeOverScoreText;
    [SerializeField] private GameObject pauseModal;
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private TextMeshProUGUI mouseSensValue;
    [SerializeField] private Slider gamepadSensSlider;
    [SerializeField] private TextMeshProUGUI gamepadSensValue;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Leaderboard")]
    [SerializeField] private LeaderboardManager leaderboardManager;
    #endregion

    #region Actions
    private void OnEnable()
    {
        Acoes.OnPlayerHealthChanged += UpdateHealth;
        Acoes.OnHeatChanged += UpdateHeatBar;
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
        Acoes.OnHeatChanged -= UpdateHeatBar;
        Acoes.GameOver -= ShowGameOver;
        Acoes.Victory -= ShowVictory;
        Acoes.TimeOver -= ShowTimeOver;
        Acoes.ResolveScore -= UpdateScore;
        Acoes.ResolveTime -= leaderboardManager.SetPendingTime;
        Acoes.OnWaveSpawn -= OnWaveSpawn;
        Acoes.OnTimeBetweenWaves -= OnTimeBetweenWaves;
    }
    #endregion

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
        heatBar.fillAmount = 0f;
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

    private void UpdateHeatBar(float currentHeat, float maxHeat)
    {
        heatBar.fillAmount = currentHeat / maxHeat;
        heatBar.color = heatGradient.Evaluate(heatBar.fillAmount);
    }

    private void UpdateScore(int score)
    {
        gameOverScoreText.text = score.ToString("D4");
        victoryScoreText.text = score.ToString("D4");
        timeOverScoreText.text = score.ToString("D4");
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
        heatBar.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        waveText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        waveAnnouncementText.gameObject.SetActive(false);
    }

    public void ShowPause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mouseSensSlider.value = SettingsManager.MouseSensibility;
        mouseSensValue.text = SettingsManager.MouseSensibility.ToString("F1");
        gamepadSensSlider.value = SettingsManager.GamepadSensibility;
        gamepadSensValue.text = SettingsManager.GamepadSensibility.ToString("F1");
        mouseSensSlider.onValueChanged.AddListener(value => mouseSensValue.text = value.ToString("F1"));
        gamepadSensSlider.onValueChanged.AddListener(value => gamepadSensValue.text = value.ToString("F1"));
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