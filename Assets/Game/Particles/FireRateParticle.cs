using UnityEngine;

public class FireRateParticle : MonoBehaviour
{
    public ParticleSystem fireRateParticle;
    public ParticleSystem fireRateParticle1;

    private void OnEnable()
    {
        Acoes.OnFireRatePowerUp += PlayFireRateParticle;
    }   

    private void OnDisable()
    {
        Acoes.OnFireRatePowerUp -= PlayFireRateParticle;
    }

    private void PlayFireRateParticle()
    {
        fireRateParticle.Play();
        fireRateParticle1.Play();
    }
}