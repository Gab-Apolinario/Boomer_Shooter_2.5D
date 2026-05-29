using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    [Header("Bob Settings")]
    [SerializeField] private float bobFrequency = 2.5f;
    [SerializeField] private float bobAmplitudeX = 0.05f;
    [SerializeField] private float bobAmplitudeY = 0.03f;

    [Header("Smoothing")]
    [SerializeField] private float bobSmoothing = 10f;

    private float bobTimer;
    private Vector3 restPosition;
    private PlayerController player;  // ou CharacterController

    void Start()
    {
        restPosition = transform.localPosition;
        player = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        bool isMoving = player.IsMoving && player.IsGrounded;

        if (isMoving)
            bobTimer += Time.deltaTime * bobFrequency;
        else
            bobTimer = Mathf.Lerp(bobTimer, 0f, Time.deltaTime * bobSmoothing);
            // ← retorna suavemente ao ciclo zero quando para

        Vector3 targetOffset = new Vector3(
            Mathf.Sin(bobTimer) * bobAmplitudeX,
            Mathf.Sin(bobTimer * 2f) * bobAmplitudeY,
            0f
        );

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            restPosition + targetOffset,
            Time.deltaTime * bobSmoothing
        );
    }
}
