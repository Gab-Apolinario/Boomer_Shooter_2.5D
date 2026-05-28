using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private BaseEnemy enemy;

    void Start()
    {
        enemy = GetComponentInParent<BaseEnemy>();
    }

    // Chamado pelo evento no último frame de Zombie_Death
    public void DestroyEnemy()
    {
        enemy.DestroyEnemy();
    }

    // Chamado pelo evento no último frame de Zombie_Damage
    public void OnDamageAnimationEnd()
    {
        enemy.OnDamageEnd();
    }
}
