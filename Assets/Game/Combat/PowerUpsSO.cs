using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "Boomer Shooter/PowerUp")]
public class PowerUpsSO : ScriptableObject
{
    public enum PowerUpType
    {
        Health,
        Damage,
        Speed,
        Shield,
        FireRate,
        ScoreMultiplier
    }

    [Header("ID do PowerUp")]
    public string powerUpName;
    public PowerUpType powerUpType;
    public float powerUpValue;
    public float duration; //duração do efeito do power-up
    public string description; //descrição do power-up para exibir na UI
    public Sprite icon;
}
