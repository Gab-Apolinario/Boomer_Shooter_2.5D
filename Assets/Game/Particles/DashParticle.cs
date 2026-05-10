using UnityEngine;

public class DashParticle : MonoBehaviour
{
    //INICIAR A VARIAVEL DE PARTICULA
    [SerializeField] private ParticleSystem dashFrente;
    [SerializeField] private ParticleSystem dashTras;
    [SerializeField] private ParticleSystem dashRight;
    [SerializeField] private ParticleSystem dashLeft;

    private void OnEnable()
    {
        Acoes.OnDash += PlayDashParticle;
    }

    private void OnDisable()
    {
        Acoes.OnDash -= PlayDashParticle;
    }

    private void PlayDashParticle(Vector3 direction)
    {
        Vector3 localDir = transform.InverseTransformDirection(direction);

        if (localDir.z > 0.5f) dashFrente.Play();
        else if (localDir.z < -0.5f) dashTras.Play();
        else if (localDir.x > 0.5f) dashRight.Play();
        else if (localDir.x < -0.5f) dashLeft.Play();
    }
}
