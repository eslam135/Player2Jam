using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D attackCollider;  // your child trigger
    [SerializeField] private PlayerMovement player;      // PlayerMovement reference

    [Header("Stats")]
    [SerializeField] private int damage = 1;

    private ContactFilter2D filter;
    private Collider2D[] hits = new Collider2D[10];

    private void Awake()
    {
        filter = new ContactFilter2D {
            useLayerMask = true,
            layerMask    = LayerMask.GetMask("Enemy"),
            useTriggers  = true
        };
    }

    public void DealAttack1() => DoDamage(damage);
    public void DealAttack2() => DoDamage(damage);

    private void DoDamage(int dmg)
    {
        int count = Physics2D.OverlapCollider(attackCollider, filter, hits);
        Vector2 face = player.FacingVector; // full 2D facing

        for (int i = 0; i < count; i++)
        {
            var col = hits[i];
            if (!col) continue;

            Vector2 toEnemy = (col.transform.position - player.transform.position);
            // dot > 0 means enemy is within ±90° of facing direction
            if (Vector2.Dot(toEnemy.normalized, face) <= 0f)
                continue;

            var health = col.GetComponent<EnemyHealth>();
            if (health != null)
                health.TakeDamage(dmg);
        }
    }
}
