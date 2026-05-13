using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField] private GameObject meleeWeapon;
    [SerializeField] private float meleeDamage;
    [SerializeField] private float meleeCooldown;
    [SerializeField] private float meleeRange;
    [SerializeField] private bool canAttack;

    void Start()
    {
        if (meleeWeapon == null)
        {
            meleeWeapon = transform.Find("MeleeWeapon").gameObject;
        }

        meleeWeapon.SetActive(false);
        canAttack = true;
    }

    public void ShowWeapon()
    {
        if (meleeWeapon != null)
        {
            meleeWeapon.SetActive(true);
            meleeWeapon.transform.localRotation = Quaternion.Euler(-20f, 105f, -15f);
        }
    }

    public void HideWeapon()
    {
        if (meleeWeapon != null)
        {
            meleeWeapon.SetActive(false);
        }
    }
    public bool TryAttack(float damageMultiplier)
    {
        if (!canAttack)
        {
            return false;
        }

        StartCoroutine(MeleeAttack(damageMultiplier));
        return true;
    }

    IEnumerator MeleeAttack(float damageMultiplier)
    {
        canAttack = false;

        //Animação Slash
        float time = 0;
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            meleeWeapon.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(-20f, 105f, -15f), Quaternion.Euler(-30f, 97f, 77f), time/0.4f);
            yield return null;
        }
        
        meleeWeapon.transform.localRotation = Quaternion.Euler(-20f, 105f, -15f);

        //verificar colision com EnemyBase
        Collider[] hits = Physics.OverlapSphere(transform.position, meleeRange);
        
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                //Aciona TakeDamage() do BaseEnemy
                enemy.TakeDamage(meleeDamage * damageMultiplier);
            }
        }
        
        Acoes.OnMeleeAttack?.Invoke();
        yield return new WaitForSeconds(meleeCooldown);
        canAttack = true;
    }

    public float GetAttackDuration()
    {
        return 0.4f + meleeCooldown;
    }

    public bool CanAttack => canAttack;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}