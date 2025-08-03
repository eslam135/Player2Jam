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
    private Vector2 lastMoveDir;
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
        // freeze if attacking, else move
        rb.linearVelocity = isAttacking ? Vector2.zero : moveInput * speed;

        // flip sprite by the active x‐direction
        float dirX = isAttacking
            ? attackDir.x
            : (moveInput.x != 0 ? moveInput.x : lastMoveDir.x);

        if ((dirX > 0 && facing < 0) || (dirX < 0 && facing > 0))
            Flip();

        // drive Run/Idle bools
        bool running = !isAttacking && moveInput.sqrMagnitude > 0.01f;
        anim.SetBool(HashRun, running);
        anim.SetBool(HashIdle, !running && !isAttacking);

        // feed blend‐tree floats when attacking
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

    // PlayerInput Send Messages
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
        {
            StartCoroutine(DoAttackCombo());
        }
        else if (!wantCombo)
        {
            wantCombo = true;
        }
    }

    private IEnumerator DoAttackCombo()
    {
        isAttacking = true;
        wantCombo   = false;

        // determine attack direction
        Vector2 d = moveInput.sqrMagnitude > 0.01f
            ? moveInput
            : lastMoveDir;

        // snap to cardinal
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

        // play swipe1
        if (swipe1 != null)
            audioSource.PlayOneShot(swipe1);

        yield return null;  // wait a frame
        float len1 = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;
        yield return new WaitForSeconds(len1);

        anim.SetBool(HashAttack, false);

        // —— COMBO ATTACK? ——
        if (wantCombo)
        {
            wantCombo = false;

            anim.SetBool(HashAttack2, true);
            anim.Play(StateAttack2, 0, 0f);

            // play swipe2
            if (swipe2 != null)
                audioSource.PlayOneShot(swipe2);

            yield return null;
            float len2 = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;
            yield return new WaitForSeconds(len2);

            anim.SetBool(HashAttack2, false);
        }

        // done
        isAttacking = false;
    }

    // Exposed for AttackTrigger:
    public int GetFacing() => facing;
    public bool IsAttacking() => isAttacking;
    public Vector2 GetAttackDirection() => attackDir;
}