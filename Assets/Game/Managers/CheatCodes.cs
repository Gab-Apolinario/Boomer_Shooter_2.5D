using UnityEngine;
using UnityEngine.InputSystem;

public class CheatCode : MonoBehaviour
{
    [Header("Leaderboard")]
    [SerializeField] private LeaderboardManager leaderboardManager;

    [Header("Wave")]
    [SerializeField] private WaveManager waveManager;

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Ctrl + Shift + W — próxima wave
        if (keyboard.leftCtrlKey.isPressed &&
            keyboard.leftShiftKey.isPressed &&
            keyboard.nKey.wasPressedThisFrame)
        {
            if (waveManager != null)
            {
                waveManager.ForceNextWave();
                Debug.Log("Wave forçada!");
            }
        }
    }
}