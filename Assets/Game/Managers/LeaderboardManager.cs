using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    private const string Key = "leaderboard";
    private const int MaxEntries = 15;

    [Serializable]
    public class Entry
    {
        public string name;
        public int score;
        public float timePlayed;
    }

    [Serializable]
    private class EntryList { public List<Entry> entries; }

    [Header("Inputs")]
    [SerializeField] private TMP_InputField gameOverInput;
    [SerializeField] private TMP_InputField victoryInput;
    [SerializeField] private TMP_InputField timeOverInput;

    [Header("Rankings")]
    [SerializeField] private TextMeshProUGUI gameOverRanking;
    [SerializeField] private TextMeshProUGUI victoryRanking;
    [SerializeField] private TextMeshProUGUI timeOverRanking;

    private int pendingScore;
    public void SetPendingScore(int score) => pendingScore = score;

    private float pendingTime;
    public void SetPendingTime(float time) => pendingTime = time;

    public void OpenGameOver()  => ShowLeaderboard(gameOverInput, gameOverRanking);
    public void OpenVictory()   => ShowLeaderboard(victoryInput, victoryRanking);
    public void OpenTimeOver()  => ShowLeaderboard(timeOverInput, timeOverRanking);

    public void OnSubmitGameOver()  => SubmitScore(gameOverInput, gameOverRanking);
    public void OnSubmitVictory()   => SubmitScore(victoryInput, victoryRanking);
    public void OnSubmitTimeOver()  => SubmitScore(timeOverInput, timeOverRanking);

    private void ShowLeaderboard(TMP_InputField input, TextMeshProUGUI rankingText)
    {
        input.text = "";
        input.interactable = true;
        RefreshDisplay(rankingText);
    }

    private void SubmitScore(TMP_InputField input, TextMeshProUGUI rankingText)
    {
        string playerName = string.IsNullOrWhiteSpace(input.text) ? "???" : input.text;
        SaveScore(playerName, pendingScore);
        input.interactable = false;
        RefreshDisplay(rankingText);
    }

        private void RefreshDisplay(TextMeshProUGUI rankingText)
    {
        var entries = LoadAll();
        string display = "";
        for (int i = 0; i < entries.Count; i++)
        {
            int minutes = Mathf.FloorToInt(entries[i].timePlayed / 60f);
            int seconds = Mathf.FloorToInt(entries[i].timePlayed % 60f);
            display += $"{i + 1}. {entries[i].name} — {entries[i].score} — {minutes:00}:{seconds:00}\n";
        }
        rankingText.text = display;
    }

    private void SaveScore(string playerName, int score)
    {
        var list = LoadAll();
        list.Add(new Entry { name = playerName, score = score, timePlayed = pendingTime });
        list.Sort((a, b) => b.score.CompareTo(a.score));
        if (list.Count > MaxEntries) list.RemoveRange(MaxEntries, list.Count - MaxEntries);
        PlayerPrefs.SetString(Key, JsonUtility.ToJson(new EntryList { entries = list }));
        PlayerPrefs.Save();
    }

    public List<Entry> LoadAll()
    {
        string json = PlayerPrefs.GetString(Key, "");
        if (string.IsNullOrEmpty(json)) return new List<Entry>();
        return JsonUtility.FromJson<EntryList>(json).entries;
    }

    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
    }
}