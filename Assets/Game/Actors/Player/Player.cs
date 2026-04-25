using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float health;



    public void TakeDamage(float amount)
    {
        health -= amount;
    }
}