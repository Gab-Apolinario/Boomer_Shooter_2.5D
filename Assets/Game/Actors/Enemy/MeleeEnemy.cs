using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    private MeleeEnemyDataSO meleeData;
    private Player playerScript;
    [SerializeField] protected float attackCooldownTimer;
    [SerializeField] protected bool canAttack = true;

    public override void Start()
    {
        base.Start();

        meleeData = enemyData as MeleeEnemyDataSO;

        if (meleeData == null)
        {
            Debug.LogError($"[{gameObject.name}] O ScriptableObject atribuído precisa ser do tipo MeleeEnemyDataSO, mas é {enemyData.GetType().Name}!");
            return;
        }

        attackCooldownTimer = meleeData.attackCooldown;
        playerScript = player.GetComponent<Player>();
    }

    public override void Update()
    {
        base.Update();

        if (!canAttack)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                canAttack = true;
                attackCooldownTimer = meleeData.attackCooldown;
            }
        }
    }

    public override void Move()
    {
        base.Move();
        if (currentState == EnemyState.Attacking)
        {
            navAgent.ResetPath();
            if (canAttack)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        canAttack = false;
        playerScript.TakeDamage(meleeData.damage);
    }
}
