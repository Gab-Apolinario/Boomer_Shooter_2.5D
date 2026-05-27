using System.Collections;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    PowerUpsSO activeBuff;
    Player player;
    PlayerController playerController;
    WeaponSystem weaponSystem;

    void OnEnable()
    {
        Acoes.OnPowerUpSelected += ApplyBuff;
    }
    void OnDisable()
    {
        Acoes.OnPowerUpSelected -= ApplyBuff;
    }

    private void Start()
    {
        player = GetComponent<Player>();
        playerController = GetComponent<PlayerController>();
        weaponSystem = GetComponentInChildren<WeaponSystem>();
    }

    public void ApplyBuff(PowerUpsSO buff)
    {
        Debug.Log($"BuffManager: RECEBEU buff = {buff.powerUpName}");
        switch (buff.powerUpType)
        {
            case PowerUpsSO.PowerUpType.Health:
                Debug.Log($"Health ANTES: {player.health}");
                player.Heal(buff.powerUpValue);
                Acoes.OnHealthPickup?.Invoke();
                Debug.Log($"Health DEPOIS: {player.health}");
                break;
            case PowerUpsSO.PowerUpType.Damage:
                Debug.Log($"Damage ANTES: {weaponSystem.damageMultiplier}");
                weaponSystem.damageMultiplier = buff.powerUpValue;
                Debug.Log($"Damage DEPOIS: {weaponSystem.damageMultiplier}");
                StartCoroutine(BuffDuration(buff));
                break;
            case PowerUpsSO.PowerUpType.Speed:
                Debug.Log($"Speed ANTES: {playerController.speedMultiplier}");
                playerController.speedMultiplier = buff.powerUpValue;
                Debug.Log($"Speed DEPOIS: {playerController.speedMultiplier}");
                StartCoroutine(BuffDuration(buff));
                break;
            case PowerUpsSO.PowerUpType.Shield:
                player.ActivateShield(buff.powerUpValue);
                Debug.Log($"SHIELD: {player.shieldHealth}");
                break;
            case PowerUpsSO.PowerUpType.FireRate:
                Debug.Log($"FireRate ANTES: {weaponSystem.fireRateMultiplier}");
                weaponSystem.fireRateMultiplier = buff.powerUpValue;
                Debug.Log($"FireRate DEPOIS: {weaponSystem.fireRateMultiplier}");
                StartCoroutine(BuffDuration(buff));
                break;
        }
    }

    IEnumerator BuffDuration(PowerUpsSO buff)
    {
        float elapsedTime = 0f; // contador de tempo
    
        // Loop que roda todo frame até atingir a duração
        while (elapsedTime < buff.duration)
        {
            elapsedTime += Time.deltaTime; // adiciona o tempo do frame
            yield return null; // espera próximo frame
        }
    
        // Quando sair do loop (tempo acabou), reseta o multiplicador
        switch (buff.powerUpType)
        {
            case PowerUpsSO.PowerUpType.Damage:
                weaponSystem.damageMultiplier = 1f;
                break;
            case PowerUpsSO.PowerUpType.Speed:
                playerController.speedMultiplier = 1f;
                break;
            case PowerUpsSO.PowerUpType.Shield:
                player.ActivateShield(0f);
                break;
            case PowerUpsSO.PowerUpType.FireRate:
                weaponSystem.fireRateMultiplier = 1f;
                break;
        }
    }
}
