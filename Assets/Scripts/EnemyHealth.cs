using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public int health = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip deathClip;     // assign your explosion/death sound here
    private AudioSource audioSource;

    private Animator anim;
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
            StopEnemyMovement();

            // trigger death animation
            anim.SetTrigger(HashDeath);

            // Option A: play immediately
            // PlayDeathSound();

            // Option B: play via Animation Event on the “Death” clip
            //   → Add an event on the frame you want the explosion sound to fire calling PlayDeathSound()
        }
    }

    private void StopEnemyMovement()
    {
        // Stop Rigidbody2D movement
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Disable movement/attack scripts
        if (TryGetComponent<Enemy_Movement>(out var move)) move.enabled = false;
        if (TryGetComponent<EnemyAttack>(out var atk)) atk.enabled = false;
    }

    /// <summary>
    /// Plays the explosion/death sound clip.
    /// Call this either right after SetTrigger, or via an Animation Event.
    /// </summary>
    public void PlayDeathSound()
    {
        if (deathClip != null && audioSource != null)
            audioSource.PlayOneShot(deathClip);
    }

    /// <summary>
    /// Hook this up as an Animation Event on the last frame of your Death clip
    /// so the object is destroyed right after the animation & sound finish.
    /// </summary>
    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}