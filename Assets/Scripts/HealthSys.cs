using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class HealthSys : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image currentHealthBar;
    [SerializeField] private Text healthText;

    [Header("Health")]
    [SerializeField] private int maxHitPoint = 100;
    [SerializeField] private int hitPoint = 100;

    [Header("Damage Cooldown")]
    [SerializeField] private float invulDuration = 0.5f;
    private bool isInvulnerable = false;

    [Header("Audio")]
    [SerializeField] private AudioClip damageTakenSound;
    [SerializeField] private AudioClip deathSound;

    private Animator anim;
    private AudioSource audioSource;
    private bool isDying = false;

    private static readonly int HashDamage = Animator.StringToHash("DamageTaken");
    private static readonly int HashDeath = Animator.StringToHash("Death");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hitPoint = Mathf.Clamp(hitPoint, 0, maxHitPoint);
        UpdateHealthBar();
    }

    /// <summary>
    /// Change health by + or - amount.
    /// Negative = damage, Positive = heal.
    /// </summary>
    public void ChangeHealth(int amount)
    {
        if (isDying)
            return;

        // Prevent effects while invulnerable
        if (amount < 0 && isInvulnerable)
        {
            Debug.Log("Damage prevented: currently invulnerable");
            return;
        }

        int oldHP = hitPoint;
        hitPoint = Mathf.Clamp(hitPoint + amount, 0, maxHitPoint);

        // Update UI only if health changed
        if (hitPoint != oldHP)
            UpdateHealthBar();

        // Trigger damage feedback if we actually took damage
        if (amount < 0 && hitPoint < oldHP)
        {
            anim.SetTrigger(HashDamage);
            PlayDamageSound();
            StartCoroutine(DamageCooldown());
        }

        // Handle death
        if (hitPoint <= 0 && !isDying)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        float ratio = (float)hitPoint / maxHitPoint;
        if (currentHealthBar != null)
        {
            float fullWidth = currentHealthBar.rectTransform.rect.width;
            currentHealthBar.rectTransform.localPosition = new Vector3(fullWidth * ratio - fullWidth, 0, 0);
        }

        if (healthText != null)
        {
            healthText.text = $"{hitPoint:0} / {maxHitPoint:0}";
        }
    }

    private IEnumerator DamageCooldown()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulDuration);
        isInvulnerable = false;
    }

    private void Die()
    {
        isDying = true;
        anim.SetTrigger(HashDeath);
        PlayDeathSound();
        // FinalizeDeath will be called by animation event at end of death clip
    }

    /// <summary>
    /// Called by animation event at end of death animation
    /// </summary>
    public void FinalizeDeath()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Called by animation event during damage animation
    /// </summary>
    public void PlayDamageSound()
    {
        if (damageTakenSound != null)
            audioSource.PlayOneShot(damageTakenSound);
    }

    /// <summary>
    /// Called by animation event during death animation
    /// </summary>
    public void PlayDeathSound()
    {
        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);
    }
}