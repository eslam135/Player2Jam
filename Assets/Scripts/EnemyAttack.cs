using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float weaponRange = 1f;
    [SerializeField] private LayerMask playerLayer;

    // Called by an Animation Event at the exact swing frame
    public void Attack()
    {
        StartCoroutine(AttackWithDelay());
    }
    
    private IEnumerator AttackWithDelay()
    {
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            weaponRange, 
            playerLayer
        );

        foreach (var hit in hits)
        {
            var health = hit.GetComponent<HealthSys>();
            if (health != null)
                health.ChangeHealth(-damage);
                yield return new WaitForSeconds(1f);
        }
    }

    // (Optional) visualize range in editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
