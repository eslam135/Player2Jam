using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Boss : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCoolDown = 2f;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private BossState enemyState;
    [SerializeField] private Vector2 dir;

    [Header("Summoning")]
    [SerializeField] private GameObject goblinPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip skillAttackSound;
    [SerializeField] private AudioClip summonSound;

    private float attackCoolDownTimer;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    private AudioSource audioSource;
    private int facingDir = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        ChangeState(BossState.Idle);
    }

    void Update()
    {
        if (GameManager.Instance.isTalking)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        if (player == null) return;

        if (attackCoolDownTimer > 0)
            attackCoolDownTimer -= Time.deltaTime;

        Vector2 delta = player.position - transform.position;
        float distance = delta.magnitude;

        if (distance <= attackRange && attackCoolDownTimer <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            attackCoolDownTimer = attackCoolDown;

            float rand = Random.value; 

            if (rand < 0.10f) 
            {
                ChangeState(BossState.Summoning);
            }
            else if (rand < 0.25f) 
            {
                ChangeState(BossState.SkillAttacking);
            }
            else
            {
                ChangeState(BossState.Attacking);
            }
        }
        else if (distance > attackRange)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        if (player.position.x > transform.position.x && facingDir == -1 ||
            player.position.x < transform.position.x && facingDir == 1)
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        dir = direction;
        rb.linearVelocity = direction * speed;

        ChangeState(BossState.Chasing);
    }

    public void SummonGoblins()
    {
        if (goblinPrefab == null) return;

        for (int i = 0; i < 5; i++)
        {
            Vector2 spawnOffset = new Vector2(
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f)
            );

            Instantiate(goblinPrefab, (Vector2)transform.position + spawnOffset, Quaternion.identity);
        }
    }

    public void Flip()
    {
        facingDir *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void ChangeState(BossState newState)
    {
        if (enemyState == newState) return;

        enemyState = newState;

        anim.SetBool("isIdle", newState == BossState.Idle);
        anim.SetBool("isChasing", newState == BossState.Chasing);
        anim.SetBool("isAttacking", newState == BossState.Attacking);
        anim.SetBool("isSkillAttacking", newState == BossState.SkillAttacking);
        anim.SetBool("isDying", newState == BossState.Dying);
        anim.SetBool("isSummoning", newState == BossState.Summoning);
    }

    /// <summary>
    /// Play attack sound - call this from animation events
    /// </summary>
    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);
    }

    /// <summary>
    /// Play skill attack sound - call this from animation events
    /// </summary>
    public void PlaySkillAttackSound()
    {
        if (skillAttackSound != null && audioSource != null)
            audioSource.PlayOneShot(skillAttackSound);
    }

    /// <summary>
    /// Play summon sound - call this from animation events or when summoning
    /// </summary>
    public void PlaySummonSound()
    {
        if (summonSound != null && audioSource != null)
            audioSource.PlayOneShot(summonSound);
    }
}

public enum BossState
{
    Idle,
    Chasing,
    Attacking,
    SkillAttacking,
    Dying,
    Summoning
}
