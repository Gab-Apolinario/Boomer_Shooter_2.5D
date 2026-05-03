using UnityEngine;

public class ImpactoParticle : MonoBehaviour
{
    [SerializeField] private GameObject preFabImpacto;

    void OnEnable()
    {
        Acoes.OnImpact += OnImpacto;
    }

    void OnDisable()
    {
        Acoes.OnImpact -= OnImpacto;
    }

    void OnImpacto(Vector3 position)
    {
        GameObject instance = Instantiate(preFabImpacto, position, Quaternion.identity);
        Destroy(instance, 1f);
    }
}