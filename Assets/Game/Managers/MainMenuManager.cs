using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Leaderboard")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private TextMeshProUGUI rankingText;
    [SerializeField] private LeaderboardManager leaderboardManager;

    void Start()
    {
        leaderboardManager = FindAnyObjectByType<LeaderboardManager>();
        leaderboardPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void OpenLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        RefreshDisplay();
    }

    public void CloseLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    private void RefreshDisplay()
    {
        leaderboardManager.FetchAndDisplay(rankingText);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }
}