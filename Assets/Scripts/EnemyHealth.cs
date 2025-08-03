using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class EnemyHealth : MonoBehaviour
{
    public int health = 1;

    private Animator anim;
    private AudioSource audioSource;
    private bool isDying = false;

    private static readonly int HashDeath = Animator.StringToHash("Death");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int amount)
    {
        if (isDying) return;

        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {health}");

        if (health <= 0)
        {
            isDying = true;
            
            // Stop enemy movement
            StopEnemyMovement();
            
            // Play death animation and sound
            anim.SetTrigger(HashDeath);
            if (audioSource != null)
                audioSource.Play();
            
            // Don't call OnDeathAnimationFinished immediately - let animation finish first
        }
    }
    
    private void StopEnemyMovement()
    {
        // Stop rigidbody movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true; // Prevent physics interactions
        }
        
        // Disable enemy movement script if it exists
        Enemy_Movement enemyMovement = GetComponent<Enemy_Movement>();
        if (enemyMovement != null)
        {
            enemyMovement.enabled = false;
        }
        
        // Disable enemy attack script if it exists
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack != null)
        {
            enemyAttack.enabled = false;
        }
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}
