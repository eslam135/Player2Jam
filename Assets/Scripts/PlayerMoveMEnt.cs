using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Animator    anim;

    private Vector2 moveInput;
    private Vector2 lastMoveDir;          // ← store the last direction you moved
    private bool    isAttacking;
    private Vector2 attackDir;
    private int     facing = 1;

    private static readonly int HashRun        = Animator.StringToHash("Run");
    private static readonly int HashIdle       = Animator.StringToHash("Idle");
    private static readonly int HashAttackBool = Animator.StringToHash("Attack");
    private static readonly int HashHoriz      = Animator.StringToHash("horizontal");
    private static readonly int HashVert       = Animator.StringToHash("vertical");
    private const      string StateAttack     = "Attack";

    private void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Freeze if attacking, otherwise move
        rb.linearVelocity = isAttacking
            ? Vector2.zero
            : moveInput * speed;

        // Flip by whichever direction is active
        float dirX = isAttacking ? attackDir.x : (moveInput.x != 0 ? moveInput.x : lastMoveDir.x);
        if ((dirX > 0 && facing < 0) || (dirX < 0 && facing > 0))
            Flip();

        // Drive Run/Idle
        bool running = !isAttacking && moveInput.sqrMagnitude > 0.01f;
        anim.SetBool(HashRun, running);
        anim.SetBool(HashIdle, !running && !isAttacking);

        // Drive Attack bool
        anim.SetBool(HashAttackBool, isAttacking);

        // Feed blend-tree when attacking
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

    // Send Messages callbacks:
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // update lastMoveDir whenever there's real input
        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDir = moveInput.normalized;
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isAttacking)
            StartCoroutine(DoAttackRoutine());
    }

    private IEnumerator DoAttackRoutine()
    {
        isAttacking = true;

        // choose which direction to attack:
        Vector2 dir = moveInput.sqrMagnitude > 0.01f
            ? moveInput
            : lastMoveDir;

        // snap to cardinal
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dir = new Vector2(Mathf.Sign(dir.x), 0f);
        else
            dir = new Vector2(0f, Mathf.Sign(dir.y));

        attackDir = dir;

        // set blend-tree & flag
        anim.SetFloat(HashHoriz, attackDir.x);
        anim.SetFloat(HashVert,  attackDir.y);
        anim.SetBool(HashAttackBool, true);

        // force the Attack state
        anim.Play(StateAttack, 0, 0f);
        yield return null;  // wait a frame for state to register

        // grab true clip length
        var info    = anim.GetCurrentAnimatorStateInfo(0);
        float length = info.length / anim.speed;
        yield return new WaitForSeconds(length);

        // end attack
        isAttacking = false;
        anim.SetBool(HashAttackBool, false);
    }
}
