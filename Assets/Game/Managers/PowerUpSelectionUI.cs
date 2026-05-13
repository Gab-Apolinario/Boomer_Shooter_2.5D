using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpSelectionUI : MonoBehaviour
{
    [SerializeField] GameObject powerUpPanel;
    [SerializeField] Button[] powerUpButtons;
    [SerializeField] Image[] powerUpIcons;
    [SerializeField] PowerUpsSO[] availablePowerUps;

    void OnEnable()
    {
        Acoes.OnPointCotrolled += ShowPowerUpSelection;
    }

    void OnDisable()
    {
        Acoes.OnPointCotrolled -= ShowPowerUpSelection;
    }

    void Start()
    {
        powerUpPanel.SetActive(false);
    }

    void ShowPowerUpSelection()
    {
        PowerUpsSO[] selectedPowerUps = GetRandomPowerUps(3);
        for (int i = 0; i < powerUpButtons.Length; i++)
        {
            int index = i;
            TextMeshProUGUI buttonText = powerUpButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = selectedPowerUps[i].powerUpName;
            powerUpIcons[i].sprite = selectedPowerUps[i].icon;

            powerUpButtons[i].onClick.RemoveAllListeners();
            powerUpButtons[i].onClick.AddListener(() => OnPowerUpSelected(selectedPowerUps[index]));
        }

        powerUpPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; //PAUSA O JOGO
    }

    public void OnPowerUpSelected(PowerUpsSO selectedPowerUp)
    {
        Debug.Log($"UI: PowerUp selecionado = {selectedPowerUp.powerUpName}");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Acoes.OnPowerUpSelected?.Invoke(selectedPowerUp);
        Debug.Log("UI: Evento invocado");
        powerUpPanel.SetActive(false);
        Time.timeScale = 1f; //RETOMA O JOGO
    }

    PowerUpsSO[] GetRandomPowerUps(int count)
    {
        PowerUpsSO[] result = new PowerUpsSO[count];
        PowerUpsSO[] pool = (PowerUpsSO[])availablePowerUps.Clone();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(i, pool.Length);
            result[i] = pool[randomIndex];

            PowerUpsSO temp = pool[i];
            pool[i] = pool[randomIndex];
            pool[randomIndex] = temp;
        }

        return result;
    }
}
