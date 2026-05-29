using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorUI : MonoBehaviour
{
    public static DamageIndicatorUI Instance;

    [SerializeField] private Image flashNorth;
    [SerializeField] private Image flashSouth;
    [SerializeField] private Image flashEast;
    [SerializeField] private Image flashWest;

    [SerializeField] private float flashDuration = 0.4f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.6f);

    private Transform playerTransform;

    void Awake()
    {
        Instance = this;
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {

        // Garante que todos começam invisíveis
        SetAlpha(flashNorth, 0);
        SetAlpha(flashSouth, 0);
        SetAlpha(flashEast, 0);
        SetAlpha(flashWest, 0);
    }

    public void TakeDamageFrom(Vector3 attackerWorldPos)
    {
        Vector3 dir = (attackerWorldPos - playerTransform.position).normalized;
        dir.y = 0; // ignora diferença de altura

        // Usa os eixos da câmera principal
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward; camForward.y = 0; camForward.Normalize();
        Vector3 camRight   = cam.right;   camRight.y = 0;   camRight.Normalize();

        float dotForward = Vector3.Dot(dir, camForward);
        float dotRight   = Vector3.Dot(dir, camRight);

        // Decide qual eixo é dominante
        if (Mathf.Abs(dotForward) >= Mathf.Abs(dotRight))
        {
            if (dotForward >= 0) TriggerFlash(flashNorth);  // dano vindo de frente
            else                 TriggerFlash(flashSouth);  // dano vindo de trás
        }
        else
        {
            if (dotRight >= 0) TriggerFlash(flashEast);     // dano vindo da direita
            else               TriggerFlash(flashWest);     // dano vindo da esquerda
        }
    }

    private void TriggerFlash(Image img)
    {
        StopCoroutine(nameof(FlashRoutine) + img.name); // evita sobreposição
        StartCoroutine(FlashRoutine(img));
    }

    private IEnumerator FlashRoutine(Image img)
    {
        // Fade in
        float t = 0;
        while (t < flashDuration * 0.3f)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(img, Mathf.Lerp(0, flashColor.a, t / (flashDuration * 0.3f)));
            yield return null;
        }

        // Fade out
        t = 0;
        while (t < flashDuration * 0.7f)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(img, Mathf.Lerp(flashColor.a, 0, t / (flashDuration * 0.7f)));
            yield return null;
        }

        SetAlpha(img, 0);
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = flashColor;
        c.a = alpha;
        img.color = c;
    }
}
