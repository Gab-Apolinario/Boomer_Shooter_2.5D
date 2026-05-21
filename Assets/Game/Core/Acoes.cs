using System;
using UnityEngine;

public static class Acoes
{
    //PLAYER
    public static Action OnMeleeAttack;
    public static Action OnPlayerDeath;

    //WAVES
    public static Action<int> OnEnemyDie;
    public static Action OnAllEnemiesDead;
    public static Action OnLastWaveFinished;
    public static Action<int> OnWaveSpawn;
    public static Action<int> OnTimeBetweenWaves;

    //ESTADOS DE JOGO
    public static Action GameOver;
    public static Action Victory;
    public static Action TimeOver;

    //UI
    public static Action<float> ResolveTime;
    public static Action<int> ResolveScore;
    public static Action<float, float> OnHeatChanged; //currentHeat, maxHeat
    public static Action OnOverheat; //arma superaquecida
    public static Action<float, float> OnPlayerHealthChanged; //health, maxHealth
    public static Action OnHealthPickup; //pickup de vida

    //POWERUPS
    public static Action<PowerUpsSO> OnPowerUpSelected;
    public static Action OnPointCotrolled;
    public static Action OnPointCotrolledWithReward;
    public static Action<ControlPoint> OnPointReset;

    //PARTICULAS
    public static Action PlayerAtirou; //MUZZLE FLASH
    public static Action<Vector3> OnImpact; //IMPACTO
    public static Action<Vector3> OnDash; //DASH
}