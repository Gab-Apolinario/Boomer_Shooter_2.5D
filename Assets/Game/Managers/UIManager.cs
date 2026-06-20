using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    #region Variáveis
    [Header("HUD")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBarAmount;
    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image waveImage;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveAnnouncementText;
    private bool wasAnnouncementActive;
    private Coroutine waveSpawnCoroutine;
    [SerializeField] private Image heatBar;
    [SerializeField] private Gradient heatGradient;
    [SerializeField] private GameObject shieldContainer;
    [SerializeField] private Image shieldFill;
    [SerializeField] private GameObject dashContainer;
    [SerializeField] private Image dashFill;
    [SerializeField] private Image scoreImage;
    [SerializeField] private TextMeshProUGUI scoreText;
    
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
    [SerializeField] private GameObject tutorialModal;
    [SerializeField] private GameObject runesModal;

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
        Acoes.OnShieldChanged += UpdateShieldUI;
        Acoes.OnDashCooldown += UpdateDash;
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
        Acoes.OnShieldChanged -= UpdateShieldUI;
        Acoes.OnDashCooldown -= UpdateDash;
    }
    #endregion

    private void Start()
    {
        if (leaderboardManager == null)
        {
            leaderboardManager = FindAnyObjectByType<LeaderboardManager>();
        }

        ShowTutorial();

        gameOverModal.SetActive(false);
        victoryModal.SetActive(false);
        timeOverModal.SetActive(false);
        pauseModal.SetActive(false);
        healthBarAmount.fillAmount = 1f;
        heatBar.fillAmount = 0f;
        shieldFill.fillAmount = 0f;
        shieldContainer.SetActive(false);
        dashFill.fillAmount = 1f;
    }

    private void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            DisableUI();
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            EnableUI();
        }
    }

    private void ShowTutorial()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DisableUI();
        tutorialModal.gameObject.SetActive(true);
    }

    public void HideTutorial()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EnableUI();
        tutorialModal.gameObject.SetActive(false);
        Acoes.OnHideTutorial?.Invoke();
    }

    public void UpdateTimer(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateWave(int waveIndex)
    {
        waveText.text = $"{waveIndex + 1}";
    }

    void UpdateDash(float current, float max)
    {
        dashFill.fillAmount = current / max;
        dashFill.color = new Color(dashFill.color.r, dashFill.color.g, dashFill.color.b, dashFill.fillAmount);
    }

    private void UpdateHealth(float health, float maxHealth)
    {
        healthBarAmount.fillAmount = health / maxHealth;
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

    private void UpdateShieldUI(float current, float max)
    {
        shieldContainer.SetActive(current > 0);
        shieldFill.fillAmount = current / max;
    }
    
    private void OnWaveSpawn(int waveIndex)
    {
        if (waveSpawnCoroutine != null)
        {
            StopCoroutine(waveSpawnCoroutine);
        }

        waveSpawnCoroutine = StartCoroutine(WaveSpawn(waveIndex));
    }

    IEnumerator WaveSpawn(int waveIndex)
    {
        waveAnnouncementText.gameObject.SetActive(true);
        waveAnnouncementText.text = $"Wave {waveIndex} Spawnada!";
        yield return new WaitForSeconds(2.5f);
        waveAnnouncementText.gameObject.SetActive(false);
        waveSpawnCoroutine = null;
    }

    private void OnTimeBetweenWaves(int timeRemaining)
    {
        if (waveSpawnCoroutine != null)
        {
            StopCoroutine(waveSpawnCoroutine);
            waveSpawnCoroutine = null;
        }

        waveAnnouncementText.gameObject.SetActive(true);
        waveAnnouncementText.text = $"Próxima wave em {timeRemaining} segundos!";
    }

    private void DisableUI()
    {
        runesModal.gameObject.SetActive(false);
        dashContainer.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
        heatBar.gameObject.SetActive(false);
        timerImage.gameObject.SetActive(false);
        waveImage.gameObject.SetActive(false);
        scoreImage.gameObject.SetActive(false);
        waveAnnouncementText.gameObject.SetActive(false);
        Acoes.OnUIVisibilityChanged?.Invoke(false);
    }

    private void EnableUI()
    {
        runesModal.gameObject.SetActive(true);
        healthBar.gameObject.SetActive(true);
        heatBar.gameObject.SetActive(true);
        timerImage.gameObject.SetActive(true);
        waveImage.gameObject.SetActive(true);
        scoreImage.gameObject.SetActive(true);
        waveAnnouncementText.gameObject.SetActive(true);
        Acoes.OnUIVisibilityChanged?.Invoke(true);
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

    public void DisableText()
    {
        wasAnnouncementActive = waveAnnouncementText.gameObject.activeSelf;
        waveAnnouncementText.gameObject.SetActive(false);
    }

    public void EnableText()
    {
        waveAnnouncementText.gameObject.SetActive(wasAnnouncementActive);
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