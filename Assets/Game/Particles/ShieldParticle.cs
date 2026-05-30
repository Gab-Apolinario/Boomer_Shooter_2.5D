using UnityEngine;

public class ShieldParticle : MonoBehaviour
{
    public ParticleSystem shieldParticle;
    public ParticleSystem shieldParticle1;

    private void OnEnable()
    {
        Acoes.OnShieldPowerUp += PlayShieldParticle;
    }   

    private void OnDisable()
    {
        Acoes.OnShieldPowerUp -= PlayShieldParticle;
    }

    private void PlayShieldParticle()
    {
        shieldParticle.Play();
        shieldParticle1.Play();
    }
}