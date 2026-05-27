using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("VIDA")]
    [SerializeField] public float health;
    [SerializeField] private float MaxHealth;
    [SerializeField] public float shieldHealth;
    private float maxShieldHealth = 100f;
    PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        MaxHealth = 100;
        health = MaxHealth;
    }

    //Função para receber dano
    public void TakeDamage(float amount)
    {
        if (playerController.isInvincible)
        {
            return;
        }
        else if (shieldHealth > 0)
        {
            shieldHealth -= amount;
            if (shieldHealth < 0)
            {
                health += shieldHealth;
                shieldHealth = 0;
            }
            Acoes.OnShieldChanged?.Invoke(shieldHealth, maxShieldHealth);
        }
        else
        {
            health -= amount;
        }

        Acoes.OnPlayerHealthChanged?.Invoke(health, MaxHealth);

        //Se a vida chegar a 0, jogador morre
        if (health <= 0)
        {
            health = 0;
            Acoes.OnPlayerDeath?.Invoke();
        }
    }

    //Função para curar o jogador
    public void Heal(float amount)
    {
        //Se a vida já estiver cheia, não faz nada
        if (health >= MaxHealth)
        {
            return;
        }

        //Se a cura ultrapassar a vida máxima, ajusta para o máximo
        health = Mathf.Min(health + amount, MaxHealth);
        Acoes.OnPlayerHealthChanged?.Invoke(health, MaxHealth);
    }

    public void ActivateShield(float amount)
    {
        shieldHealth = Mathf.Min(shieldHealth + amount, maxShieldHealth);
        Acoes.OnShieldChanged?.Invoke(shieldHealth, maxShieldHealth);
    }
}