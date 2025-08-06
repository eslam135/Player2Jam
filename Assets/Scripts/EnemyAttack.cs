using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float weaponRange = 1f;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private float proximityRadius = 5f;
    [SerializeField] private int proximityDamage = 10;

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
            {
                health.ChangeHealth(-damage);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void CheckProximityDamage()
    {
        Collider2D[] nearbyPlayers = Physics2D.OverlapCircleAll(
            transform.position,
            proximityRadius,
            playerLayer
        );

        foreach (var player in nearbyPlayers)
        {
            var health = player.GetComponent<HealthSys>();
            if (health != null)
            {
                health.ChangeHealth(-proximityDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, proximityRadius);
    }
}
