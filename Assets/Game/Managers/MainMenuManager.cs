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
        var entries = leaderboardManager.LoadAll();
        string display = "";
        for (int i = 0; i < entries.Count; i++)
        {
            int minutes = Mathf.FloorToInt(entries[i].timePlayed / 60f);
            int seconds = Mathf.FloorToInt(entries[i].timePlayed % 60f);
            display += $"{i + 1}. {entries[i].name} — {entries[i].score} — {minutes:00}:{seconds:00}\n";
        }
        rankingText.text = display;
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