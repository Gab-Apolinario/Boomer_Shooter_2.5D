using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float MaxHealth;

    void Start()
    {
        MaxHealth = 100;
        health = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        Acoes.OnPlayerHealthChanged?.Invoke(health, MaxHealth);

        if (health <= 0)
        {
            health = 0;
            Acoes.OnPlayerDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (health >= MaxHealth)
        {
            return;
        }

        health = Mathf.Min(health + amount, MaxHealth);
        Acoes.OnPlayerHealthChanged?.Invoke(health, MaxHealth);
    }
}