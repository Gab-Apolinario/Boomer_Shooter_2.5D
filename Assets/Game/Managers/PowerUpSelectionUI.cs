using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpSelectionUI : MonoBehaviour
{
    [SerializeField] GameObject powerUpPanel;
    [SerializeField] Button[] powerUpButtons;
    [SerializeField] TextMeshProUGUI[] powerUpDescriptions;
    [SerializeField] Image[] powerUpIcons;
    [SerializeField] PowerUpsSO[] availablePowerUps;

    void OnEnable()
    {
        Acoes.OnPointCotrolledWithReward += ShowPowerUpSelection;
    }

    void OnDisable()
    {
        Acoes.OnPointCotrolledWithReward -= ShowPowerUpSelection;
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
            powerUpDescriptions[i].text = selectedPowerUps[i].description;
            powerUpButtons[i].onClick.RemoveAllListeners();
            powerUpButtons[i].onClick.AddListener(() => OnPowerUpSelected(selectedPowerUps[index]));
            powerUpButtons[i].interactable = false;
        }

        powerUpPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; //PAUSA O JOGO

        StartCoroutine(EnableButtonsAfterDelay());
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

    IEnumerator EnableButtonsAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.7f);
        foreach (Button button in powerUpButtons)
        {
            button.interactable = true;
        }
    }
}
