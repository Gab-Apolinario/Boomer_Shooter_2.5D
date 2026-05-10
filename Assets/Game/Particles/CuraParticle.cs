using UnityEngine;

public class CuraParticle : MonoBehaviour
{
    public ParticleSystem curaParticle;
    public ParticleSystem curaParticle1;

    private void OnEnable()
    {
        Acoes.OnHealthPickup += PlayCuraParticle;
    }   

    private void OnDisable()
    {
        Acoes.OnHealthPickup -= PlayCuraParticle;
    }

    private void PlayCuraParticle()
    {
        curaParticle.Play();
        curaParticle1.Play();
    }
}