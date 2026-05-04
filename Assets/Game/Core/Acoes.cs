using System;
using UnityEngine;

public static class Acoes
{
    public static Action OnJump;
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
    public static Action<float, float> OnHeatChanged; //currentHeat, maxHeat
    public static Action<bool> OnReloadChanged;   //isReloading true/false
    public static Action OnOverheat; //arma superaquecida
    public static Action<float, float> OnPlayerHealthChanged; //health, maxHealth
    public static Action OnHealthPickup; //pickup de vida


    //PARTICULAS
    public static Action PlayerAtirou; //MUZZLE FLASH
    public static Action<Vector3> OnImpact; //IMPACTO
    public static Action OnDash; //DASH
}