using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class PlayerSkill : MonoBehaviour
{
    [Header("Input (New System)")]
    [SerializeField] private InputActionReference rightClickAction;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    private static readonly int FireBombHash = Animator.StringToHash("FireBomb");

    [Header("Audio")]
    [SerializeField] private AudioClip skillClip;    // assign your skill sound here
    private AudioSource audioSource;

    [Header("Damage Settings")]
    [SerializeField] private Collider2D attackCollider;      // your prefab-child trigger
    [SerializeField] private LayerMask enemyLayer;           // set to “Enemy” layer
    [SerializeField] private int damage = 1;
    private ContactFilter2D filter;
    private Collider2D[] hits = new Collider2D[10];

    [Header("Cooldown UI")]
    [SerializeField] private Image cooldownImage;            // Fill Method = Radial 360, Origin = Top
    [SerializeField] private float cooldownDuration = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float dimmedAlpha = 0.3f;

    private bool isOnCooldown = false;

    private void Awake()
    {
        // get AudioSource
        audioSource = GetComponent<AudioSource>();

        // configure filter to only hit your Enemy layer
        filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask    = enemyLayer,
            useTriggers  = true
        };

        // initialize UI
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f;
            var c = cooldownImage.color;
            c.a = 1f;
            cooldownImage.color = c;
        }

        // ensure animator starts in Idle
        animator.SetBool(FireBombHash, false);
    }

    private void OnEnable()
    {
        rightClickAction.action.performed += OnRightClick;
        rightClickAction.action.Enable();
    }

    private void OnDisable()
    {
        rightClickAction.action.performed -= OnRightClick;
        rightClickAction.action.Disable();
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        if (isOnCooldown) return;

        // play skill sound
        if (skillClip != null && audioSource != null)
            audioSource.PlayOneShot(skillClip);

        // trigger the skill animation
        animator.SetBool(FireBombHash, true);

        // start the cooldown UI
        StartCoroutine(CooldownRoutine());
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Called via Animation Event on the frame you want to deal damage
    // ─────────────────────────────────────────────────────────────────────────
    public void DealSkillDamage()
    {
        int count = Physics2D.OverlapCollider(attackCollider, filter, hits);
        for (int i = 0; i < count; i++)
        {
            var col = hits[i];
            if (!col) continue;
            var health = col.GetComponent<EnemyHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Called via Animation Event on the last frame of the Skill clip
    // ─────────────────────────────────────────────────────────────────────────
    public void EndSkillAnimation()
    {
        animator.SetBool(FireBombHash, false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Cooldown UI coroutine
    // ─────────────────────────────────────────────────────────────────────────
    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;

        // dim icon
        if (cooldownImage != null)
        {
            var c = cooldownImage.color;
            c.a = dimmedAlpha;
            cooldownImage.color = c;
        }

        float timer = cooldownDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (cooldownImage != null)
                cooldownImage.fillAmount = timer / cooldownDuration;
            yield return null;
        }

        // reset UI
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f;
            var c = cooldownImage.color;
            c.a = 1f;
            cooldownImage.color = c;
        }

        isOnCooldown = false;
    }
}
