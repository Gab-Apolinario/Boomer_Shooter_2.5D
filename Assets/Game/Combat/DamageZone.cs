using System.Collections;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float damagePertick = 10f;
    [SerializeField] private float tickInterval = 0.5f;
    private Coroutine damageCoroutine;

    void Start()
    {
        meshRenderer.material.color = Color.purple;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageCoroutine = StartCoroutine(ApplyDamageOverTime(other.GetComponent<Player>()));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(damageCoroutine);
        }
    }

    IEnumerator ApplyDamageOverTime(Player player)
    {
        while (true)
        {
            player.TakeDamage(damagePertick);
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
