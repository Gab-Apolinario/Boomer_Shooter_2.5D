using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackCooldownTimer;
    [SerializeField] protected bool canAttack = true;
    private Player playerScript;

    public override void Start()
    {
        base.Start();
        attackCooldownTimer = attackCooldown;
        playerScript = player.GetComponent<Player>();

        score = 10;
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
                attackCooldownTimer = attackCooldown;
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
        playerScript.TakeDamage(damage);
    }
}
