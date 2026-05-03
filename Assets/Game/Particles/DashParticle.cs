using UnityEngine;

public class DashParticle : MonoBehaviour
{
    //INICIAR A VARIAVEL DE PARTICULA
    [SerializeField] private ParticleSystem dashParticle;

    private void OnEnable()
    {
        Acoes.OnDash += PlayDashParticle;
    }

    private void OnDisable()
    {
        Acoes.OnDash -= PlayDashParticle;
    }

    void Start()
    {
        if (dashParticle == null)
        {
            dashParticle = GetComponent<ParticleSystem>();
        }
    }

    private void PlayDashParticle()
    {
        dashParticle.Play();
    }
}
