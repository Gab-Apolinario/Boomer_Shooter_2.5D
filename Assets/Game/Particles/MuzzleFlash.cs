using JetBrains.Annotations;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    //INICIAR A VARIAVEL DE PARTICULA
    [SerializeField] private ParticleSystem muzzleFlash;

    private void OnEnable()
    {
        Acoes.PlayerAtirou += Atirar;
    }

    private void OnDisable()
    {
        Acoes.PlayerAtirou -= Atirar;
    }

    void Start()
    {
        if (muzzleFlash == null)
        {
            muzzleFlash = GetComponent<ParticleSystem>();
        }
    }

    private void Atirar()
    {
        muzzleFlash.Play();
    }
}
