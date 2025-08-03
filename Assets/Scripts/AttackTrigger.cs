using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D attackCollider;  // drag your child trigger here
    [SerializeField] private PlayerMovement player;      // drag PlayerMovement component here

    [Header("Stats")]
    [SerializeField] private int damage = 1;

    private ContactFilter2D filter;
    private Collider2D[] hits = new Collider2D[10];

    private void Awake()
    {
        // only detect enemies on the Enemy layer
        filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask    = LayerMask.GetMask("Enemy"),
            useTriggers  = true
        };
    }

    // Called by Animation Events:
    public void DealAttack1() => DoDamage(damage);
    public void DealAttack2() => DoDamage(damage);

    private void DoDamage(int dmg)
    {
        int count = Physics2D.OverlapCollider(attackCollider, filter, hits);
        for (int i = 0; i < count; i++)
        {
            var col = hits[i];
            if (!col) continue;

            // Only deal damage if enemy is in front of the player
            float dirToEnemy = col.transform.position.x - player.transform.position.x;
            if (dirToEnemy * player.GetFacing() <= 0f)
                continue;

            var health = col.GetComponent<EnemyHealth>();
            if (health != null)
                health.TakeDamage(dmg);
        }
    }
}
