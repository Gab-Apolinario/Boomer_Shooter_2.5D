using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 30f;
    [SerializeField] private Color beamColor = Color.red;
    [SerializeField] private float beamWidth = 0.05f;
    [SerializeField] private Transform beamOrigin;
    [SerializeField] private Material beamMaterial;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
        lineRenderer.SetPosition(0, beamOrigin.position);
        lineRenderer.SetPosition(1, beamOrigin.position + Vector3.up * 1000f);
        lineRenderer.material = beamMaterial;
    }


    private void OnTriggerEnter(Collider other)
    {
        Acoes.OnHealthPickup?.Invoke();
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            return;
        }

        player.Heal(healAmount);
        Destroy(gameObject);
    }
}