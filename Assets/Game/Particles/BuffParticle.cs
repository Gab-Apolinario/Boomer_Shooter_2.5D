using UnityEngine;

public class BuffParticle : MonoBehaviour
{
    public ParticleSystem buffParticle;
    public ParticleSystem buffParticle1;

    private void OnEnable()
    {
        Acoes.OnBuffPowerUp += PlayBuffParticle;
    }   

    private void OnDisable()
    {
        Acoes.OnBuffPowerUp -= PlayBuffParticle;
    }

    private void PlayBuffParticle()
    {
        buffParticle.Play();
        buffParticle1.Play();
    }
}