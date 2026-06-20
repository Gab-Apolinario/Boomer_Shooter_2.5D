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
    public static Action PlayMainMusic;
    public static Action StopMainMusic;

    //UI
    public static Action<float> ResolveTime;
    public static Action<int> ResolveScore;
    public static Action<float, float> OnHeatChanged; //currentHeat, maxHeat
    public static Action OnOverheat; //arma superaquecida
    public static Action<float, float> OnPlayerHealthChanged; //health, maxHealth
    public static Action OnHealthPickup; //pickup de vida
    public static Action OnHideTutorial;
    public static Action<bool> OnUIVisibilityChanged;

    //CONTROL POINTS
    public static Action<ControlPoint[]> OnControlPointsOrdered; // passa a ordem do shuffle
    public static Action<ControlPoint> OnPointActivated;         // qual ponto ficou ativo
    public static Action<ControlPoint, float> OnCaptureProgress; // 0-1 enchendo
    public static Action<ControlPoint, float> OnResetProgress;   // ponto + 0-1 esvaziando

    //POWERUPS
    public static Action<PowerUpsSO> OnPowerUpSelected;
    public static Action OnPointCotrolled;
    public static Action OnPointCotrolledWithReward;
    public static Action<ControlPoint> OnPointReset;
    public static Action<float, float> OnShieldChanged;

    //PARTICULAS
    public static Action PlayerAtirou; //MUZZLE FLASH
    public static Action<Vector3> OnImpact; //IMPACTO
    public static Action<float, float> OnDashCooldown; //DASH
    public static Action<Vector3> OnDash; //DASH
    public static Action OnShieldPowerUp; //SHIELD POWER-UP
    public static Action OnBuffPowerUp; //BUFF POWER-UP
    public static Action OnFireRatePowerUp; //FIRE RATE POWER-UP
}