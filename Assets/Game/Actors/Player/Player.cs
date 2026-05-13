using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("VIDA")]
    [SerializeField] private float health;
    [SerializeField] private float MaxHealth;
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
}