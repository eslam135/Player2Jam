using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip swipe1;    // sound for first slash
    [SerializeField] private AudioClip swipe2;    // sound for combo slash

    private Rigidbody2D rb;
    private Animator    anim;
    private AudioSource audioSource;

    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;   // default facing down
    private bool    isAttacking;
    private bool    wantCombo;
    private Vector2 attackDir;
    private int     facing = 1;

    // Animator hashes & state names
    private static readonly int HashRun     = Animator.StringToHash("Run");
    private static readonly int HashIdle    = Animator.StringToHash("Idle");
    private static readonly int HashAttack  = Animator.StringToHash("Attack");
    private static readonly int HashAttack2 = Animator.StringToHash("Attack2");
    private static readonly int HashHoriz   = Animator.StringToHash("horizontal");
    private static readonly int HashVert    = Animator.StringToHash("vertical");
    private const      string StateAttack  = "Attack";
    private const      string StateAttack2 = "Attack2";

    private void Awake()
    {
        rb          = GetComponent<Rigidbody2D>();
        anim        = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        // move or freeze during attack
        rb.linearVelocity = isAttacking ? Vector2.zero : moveInput * speed;

        // decide flip direction
        Vector2 dirVec = isAttacking ? attackDir : lastMoveDir;
        float dirX = dirVec.x;
        if ((dirX > 0 && facing < 0) || (dirX < 0 && facing > 0))
            Flip();

        // drive Run/Idle
        bool running = !isAttacking && moveInput.sqrMagnitude > 0.01f;
        anim.SetBool(HashRun, running);
        anim.SetBool(HashIdle, !running && !isAttacking);

        // feed blend-tree while attacking
        if (isAttacking)
        {
            anim.SetFloat(HashHoriz, attackDir.x);
            anim.SetFloat(HashVert,  attackDir.y);
        }
    }

    private void Flip()
    {
        facing *= -1;
        var s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    // Input callbacks
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDir = moveInput.normalized;
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        if (!isAttacking)
            StartCoroutine(DoAttackCombo());
        else if (!wantCombo)
            wantCombo = true;
    }

    private IEnumerator DoAttackCombo()
    {
        isAttacking = true;
        wantCombo   = false;

        // pick direction (snap to cardinal)
        Vector2 d = moveInput.sqrMagnitude > 0.01f ? moveInput : lastMoveDir;
        if (Mathf.Abs(d.x) > Mathf.Abs(d.y))
            d = new Vector2(Mathf.Sign(d.x), 0f);
        else
            d = new Vector2(0f, Mathf.Sign(d.y));
        attackDir = d;

        // —— FIRST ATTACK —— 
        anim.SetFloat(HashHoriz, attackDir.x);
        anim.SetFloat(HashVert,  attackDir.y);
        anim.SetBool(HashAttack, true);
        anim.Play(StateAttack, 0, 0f);
        if (swipe1 != null) audioSource.PlayOneShot(swipe1);

        yield return null;
        float len1 = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;
        yield return new WaitForSeconds(len1);
        anim.SetBool(HashAttack, false);

        // —— COMBO? —— 
        if (wantCombo)
        {
            wantCombo = false;
            anim.SetBool(HashAttack2, true);
            anim.Play(StateAttack2, 0, 0f);
            if (swipe2 != null) audioSource.PlayOneShot(swipe2);

            yield return null;
            float len2 = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;
            yield return new WaitForSeconds(len2);
            anim.SetBool(HashAttack2, false);
        }

        isAttacking = false;
    }

    /// <summary>
    /// Full 2D facing vector: uses attackDir during an attack, otherwise lastMoveDir.
    /// </summary>
    public Vector2 FacingVector => isAttacking ? attackDir : lastMoveDir;
}
