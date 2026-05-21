using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Boomer Shooter/Base Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    public string enemyName;
    public float maxHealth;
    public float damage;
    public float moveSpeed;
    public float sprintSpeed;
    public float detectionRange;
    public float attackRange;
    public float fieldOfView;
    public int score;
}
