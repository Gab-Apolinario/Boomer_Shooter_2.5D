using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "Boomer Shooter/WeaponConfig")]
public class WeaponConfigSO : ScriptableObject
{
    [Header("ID da Arma")]
    public string weaponName;

    [Header("VFX")]
    public float smokeEmissionRate; //quantidade de fumaça emitida quando superaquecida

    [Header("Configurações de Disparo")]
    public float damage;
    public float fireRate; //tempo entre tiros
    public float maxRange;

    [Header("Configurações de Aquecimento")]
    public float heatPerShot; //quanto aquece a cada tiro
    public float heatCapacity; //capacidade máxima de calor]
    public float coolingRate; //quanto tempo resfriar por segundo
    public float overheatCooldownDelay; //tempo que a arma fica superaquecida antes do resfriamento iniciarr
}