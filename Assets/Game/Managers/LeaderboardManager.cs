using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LootLocker.Requests;

public class LeaderboardManager : MonoBehaviour
{
    private const string LeaderboardKey = "ranking_pgs";
    private const int MaxEntries = 15;

    [Serializable]
    public class Entry
    {
        public string name;
        public int score;
        public float timePlayed;
    }

    [Header("Inputs")]
    [SerializeField] private TMP_InputField gameOverInput;
    [SerializeField] private TMP_InputField victoryInput;
    [SerializeField] private TMP_InputField timeOverInput;

    [Header("Rankings")]
    [SerializeField] private TextMeshProUGUI gameOverRanking;
    [SerializeField] private TextMeshProUGUI victoryRanking;
    [SerializeField] private TextMeshProUGUI timeOverRanking;

    private int pendingScore;
    private float pendingTime;
    private bool sessionReady = false;
    private Coroutine refreshCoroutine;

    public void SetPendingScore(int score) => pendingScore = score;
    public void SetPendingTime(float time)  => pendingTime  = time;

    // ── Ciclo de vida ────────────────────────────────────────────────
    private void Start()
    {
        LootLockerSDKManager.StartGuestSession(response =>
        {
            if (response.success)
            {
                sessionReady = true;
                Debug.Log("[LL] Sessão iniciada.");
            }
            else
            {
                Debug.LogWarning("[LL] Falha ao iniciar sessão.");
            }
        });
    }

    // ── API pública (idêntica ao original) ───────────────────────────
    public void OpenGameOver()  => ShowLeaderboard(gameOverInput,  gameOverRanking);
    public void OpenVictory()   => ShowLeaderboard(victoryInput,   victoryRanking);
    public void OpenTimeOver()  => ShowLeaderboard(timeOverInput,  timeOverRanking);

    public void OnSubmitGameOver()  => SubmitScore(gameOverInput,  gameOverRanking);
    public void OnSubmitVictory()   => SubmitScore(victoryInput,   victoryRanking);
    public void OnSubmitTimeOver()  => SubmitScore(timeOverInput,  timeOverRanking);

    // ── Lógica interna ───────────────────────────────────────────────
    private void ShowLeaderboard(TMP_InputField input, TextMeshProUGUI rankingText)
    {
        input.text = "";
        input.interactable = true;
        FetchAndDisplay(rankingText);
        StartAutoRefresh(rankingText);       // atualiza a cada 10s enquanto modal aberto
    }

    private void SubmitScore(TMP_InputField input, TextMeshProUGUI rankingText)
    {
        if (!sessionReady) { Debug.LogWarning("[LL] Sessão não pronta."); return; }

        string playerName = string.IsNullOrWhiteSpace(input.text) ? "???" : input.text.Trim();
        input.interactable = false;

        rankingText.text = "Salvando Pontuação...";

        // metadata salva o tempo (se você marcou Enable Metadata no dashboard)
        string metadata = pendingTime.ToString("F1");

        LootLockerSDKManager.SubmitScore(playerName, pendingScore, LeaderboardKey, metadata, response =>
        {
            if (!response.success)
            {
                rankingText.text = "Falha ao salvar pontuação.";
                input.interactable = true;
                return;
            }
            
            FetchAndDisplay(rankingText);
        });
    }

    // Torna FetchAndDisplay pública para o MainMenuManager usar
    public void FetchAndDisplay(TextMeshProUGUI rankingText)
    {
        if (!sessionReady) { rankingText.text = "Conectando..."; return; }

        LootLockerSDKManager.GetScoreList(LeaderboardKey, MaxEntries, response =>
        {
            if (!response.success) { rankingText.text = "Erro ao carregar ranking."; return; }

            string display = "";
            foreach (var item in response.items)
            {
                string name = string.IsNullOrEmpty(item.member_id) ? "???" : item.member_id;
                string timeStr = "--:--";
                if (float.TryParse(item.metadata, out float t))
                {
                    int min = Mathf.FloorToInt(t / 60f);
                    int sec = Mathf.FloorToInt(t % 60f);
                    timeStr = $"{min:00}:{sec:00}";
                }
                display += $"{item.rank}. {name} — {item.score} — {timeStr}\n";
            }
            rankingText.text = display;
        });
    }

    // ── Auto-refresh ─────────────────────────────────────────────────
    // Chame StopAutoRefresh() ao fechar o modal se quiser parar o polling
    public void StopAutoRefresh()
    {
        if (refreshCoroutine != null) StopCoroutine(refreshCoroutine);
    }

    private void StartAutoRefresh(TextMeshProUGUI rankingText)
    {
        StopAutoRefresh();
        refreshCoroutine = StartCoroutine(AutoRefreshRoutine(rankingText));
    }

    private IEnumerator AutoRefreshRoutine(TextMeshProUGUI rankingText)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(10f);
            FetchAndDisplay(rankingText);
        }
    }
}