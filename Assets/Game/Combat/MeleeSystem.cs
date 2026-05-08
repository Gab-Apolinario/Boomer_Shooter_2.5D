using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MeleeSystem : MonoBehaviour
{
    InputHandler inputHandler;
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
        inputHandler = new InputHandler();
        canAttack = true;
    }

    void Update()
    {
        if (inputHandler.IsMeleeAttacking && canAttack)
        {
            StartCoroutine(MeleeAttack());
        }
    }

    IEnumerator MeleeAttack()
    {
        canAttack = false;
        meleeWeapon.SetActive(true);

        //Animação Slash
        float time = 0;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            meleeWeapon.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(-20f, 105f, -15f), Quaternion.Euler(-30f, 97f, 77f), time/0.3f);
            yield return null;
        }
        
        meleeWeapon.SetActive(false);

        //verificar colision com EnemyBase
        Collider[] hits = Physics.OverlapSphere(transform.position, meleeRange);
        
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                //Aciona TakeDamage() do BaseEnemy
                enemy.TakeDamage(meleeDamage);
            }
        }
        
        Acoes.OnMeleeAttack?.Invoke();
        yield return new WaitForSeconds(meleeCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}