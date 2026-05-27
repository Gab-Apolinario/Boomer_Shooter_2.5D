using UnityEngine;
using UnityEngine.UI;

public class RuneUIManager : MonoBehaviour
{
    [System.Serializable]
    struct RuneSlot
    {
        public Image runeImage;
        public Image fillBar;
    }

    [SerializeField] private RuneSlot[] slots;

    private ControlPoint[] orderedPoints;

    private void OnEnable()
    {
        Acoes.OnControlPointsOrdered += SetupSlots;
        Acoes.OnCaptureProgress += UpdateCaptureBar;
        Acoes.OnResetProgress += UpdateResetBar;
        Acoes.OnPointActivated += SetActiveSlot;
        Acoes.OnPointReset += HandleReset;
    }

    private void OnDisable()
    {
        Acoes.OnControlPointsOrdered -= SetupSlots;
        Acoes.OnCaptureProgress -= UpdateCaptureBar;
        Acoes.OnResetProgress -= UpdateResetBar;
        Acoes.OnPointActivated -= SetActiveSlot;
        Acoes.OnPointReset -= HandleReset;
    }

    void SetupSlots(ControlPoint[] points)
    {
        orderedPoints = points;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].runeImage.sprite = points[i].RuneSprite;
            slots[i].runeImage.color = new Color(1f, 1f, 1f, 0.3f); // opacidade baixa
            slots[i].fillBar.fillAmount = 0f;
        }

        // ativa o primeiro visualmente
        SetActiveSlot(points[0]);
    }

    void SetActiveSlot(ControlPoint point)
    {
        for (int i = 0; i < orderedPoints.Length; i++)
        {
            bool isActive = orderedPoints[i] == point;
            bool isControlled = orderedPoints[i].IsControlled;
            float alpha = (isActive || isControlled) ? 1f : 0.3f;
            slots[i].runeImage.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    void UpdateCaptureBar(ControlPoint point, float progress)
    {
        for (int i = 0; i < orderedPoints.Length; i++)
        {
            if (orderedPoints[i] == point)
            {
                slots[i].fillBar.fillAmount = progress;
                return;
            }
        }
    }

    void UpdateResetBar(ControlPoint point, float progress)
    {
        for (int i = 0; i < orderedPoints.Length; i++)
        {
            if (orderedPoints[i] == point)
            {
                slots[i].fillBar.fillAmount = progress;
                return;
            }
        }
    }

    void HandleReset(ControlPoint point)
    {
        for (int i = 0; i < orderedPoints.Length; i++)
        {
            if (orderedPoints[i] == point)
            {
                slots[i].runeImage.color = new Color(1f, 1f, 1f, 0.3f);
                return;
            }
        }
    }

    int GetActiveIndex()
    {
        for (int i = 0; i < orderedPoints.Length; i++)
            if (orderedPoints[i].IsActive) return i;
        return -1;
    }
}