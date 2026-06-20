using UnityEngine;
using UnityEngine.UI;

public class ControlPointPointer : MonoBehaviour
{
    [SerializeField] private Image pointerImagePrefab;
    [SerializeField] private float edgePadding = 50f;
    [Range(0f, 1f)]
    [SerializeField] private float circleSizeNormalized = 0.4f;

    private Camera cam;
    private Image pointerImage;

    private bool tutorialOpen = true;
    private bool hiddenByUI;

    private void Awake()
    {
        Acoes.OnHideTutorial += HandleTutorialClosed;
        Acoes.OnUIVisibilityChanged += HandleUIVisibility;
    }

    private void Start()
    {
        ControlPoint controlPoint = GetComponent<ControlPoint>();
        pointerImagePrefab.sprite = controlPoint.RuneSprite;
        cam = PlayerCamera.Instance;

        // instancia a imagem no Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        pointerImage = Instantiate(pointerImagePrefab, canvas.transform);
        pointerImage.gameObject.SetActive(ShouldBeVisible());
    }

    private void OnEnable()
    {
        if (pointerImage != null)
            pointerImage.gameObject.SetActive(ShouldBeVisible());
    }

    private void OnDisable()
    {
        if (pointerImage != null)
            pointerImage.gameObject.SetActive(ShouldBeVisible());
    }

    private void OnDestroy()
    {
        
        Acoes.OnHideTutorial -= HandleTutorialClosed;
        Acoes.OnUIVisibilityChanged -= HandleUIVisibility;

        if (pointerImage != null)
        {
            Destroy(pointerImage.gameObject);
        }
    }

    private void Update()
    {
        if (cam == null || pointerImage == null || !ShouldBeVisible()) return;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        bool isBehind = screenPos.z < 0;

        if (isBehind) screenPos *= -1f;

        bool isOnScreen = !isBehind
            && screenPos.x > edgePadding
            && screenPos.x < Screen.width - edgePadding
            && screenPos.y > edgePadding
            && screenPos.y < Screen.height - edgePadding;

        if (isOnScreen)
        {
            pointerImage.rectTransform.position = screenPos;
            pointerImage.rectTransform.rotation = Quaternion.identity;
        }
        else
        {
            Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 dir = (screenPos - center).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x);
            float radius = Mathf.Min(Screen.width, Screen.height) * 0.5f * circleSizeNormalized - edgePadding;

            pointerImage.rectTransform.position = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y + Mathf.Sin(angle) * radius, 0f);
            pointerImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
        }
    }

    private bool ShouldBeVisible() => enabled && !tutorialOpen && !hiddenByUI;

    private void HandleTutorialClosed()
    {
        tutorialOpen = false;
        if (pointerImage != null)
        {
            pointerImage.gameObject.SetActive(ShouldBeVisible());
        }
    }

    private void HandleUIVisibility(bool visible)
    {
        hiddenByUI = !visible;
        if ( pointerImage != null)
        {
            pointerImage.gameObject.SetActive(ShouldBeVisible());
        }
    }
}