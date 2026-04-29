using System;

public static class Acoes
{
    public static Action<int> OnEnemyDie;
    public static Action OnPlayerDeath;
    public static Action OnAllEnemiesDead;
    public static Action OnLastWaveFinished;
    public static Action<int> OnWaveSpawn;
    public static Action<int> OnTimeBetweenWaves;
    public static Action GameOver;
    public static Action Victory;
    public static Action TimeOver;
    public static Action<float> ResolveTime;
    public static Action<int> ResolveScore;
    public static Action<int, int> OnAmmoChanged; //bulletsToShoot, chargerCapacity
    public static Action<bool> OnReloadChanged;   //isReloading true/false
    public static Action<float, float> OnPlayerHealthChanged; //health, maxHealth
}