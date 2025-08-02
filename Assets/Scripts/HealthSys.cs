using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class HealthSys : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image currentHealthBar; // Image that we will shift
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

    private void Start()
    {
        UpdateHealthBar();
    }

    /// <summary>
    /// Call this to change health by + or - amount. Negative = damage, Positive = heal.
    /// </summary>
    public void ChangeHealth(int amount)
    {
        if (isDying) return;
        if (amount < 0 && isInvulnerable) return;

        hitPoint += amount;
        hitPoint = Mathf.Clamp(hitPoint, 0, maxHitPoint);

        UpdateHealthBar();

        if (amount < 0)
        {
            anim.SetTrigger(HashDamage);
            StartCoroutine(DamageCooldown());
        }

        if (hitPoint <= 0)
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
            healthText.text = hitPoint.ToString("0") + " / " + maxHitPoint.ToString("0");
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
        if (isDying) return;

        isDying = true;
        anim.SetTrigger(HashDeath);
        // Death animation will call FinalizeDeath() via animation event
    }

    public void FinalizeDeath()
    {
        Destroy(gameObject);
    }

    // These should be called via Animation Events
    public void PlayDamageSound()
    {
        if (damageTakenSound != null)
        {
            audioSource.PlayOneShot(damageTakenSound);
        }
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }
}
