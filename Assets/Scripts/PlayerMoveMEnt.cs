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
    private Vector2 lastMoveDir;
    private bool    isAttacking;
    private bool    wantCombo;
    private Vector2 attackDir;
    private int     facing = 1;

    // Animator hashes
    private static readonly int HashRun       = Animator.StringToHash("Run");
    private static readonly int HashIdle      = Animator.StringToHash("Idle");
    private static readonly int HashAttack    = Animator.StringToHash("Attack");
    private static readonly int HashAttack2   = Animator.StringToHash("Attack2");
    private static readonly int HashHoriz     = Animator.StringToHash("horizontal");
    private static readonly int HashVert      = Animator.StringToHash("vertical");
    private const      string StateAttack    = "Attack";
    private const      string StateAttack2   = "Attack2";

    private void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // freeze if attacking
        rb.linearVelocity = isAttacking 
            ? Vector2.zero 
            : moveInput * speed;

        // flip based on active direction
        float dirX = isAttacking 
            ? attackDir.x 
            : (moveInput.x != 0 ? moveInput.x : lastMoveDir.x);
        if ((dirX > 0 && facing < 0) || (dirX < 0 && facing > 0))
            Flip();

        // Run/Idle Bools
        bool running = !isAttacking && moveInput.sqrMagnitude > 0.01f;
        anim.SetBool(HashRun, running);
        anim.SetBool(HashIdle, !running && !isAttacking);

        // driving blend tree when attacking
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

    // — PlayerInput Send Messages
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
            // start first attack
            StartCoroutine(DoAttackCombo());
        }
        else if (!wantCombo)
        {
            // queue combo if still in first attack
            wantCombo = true;
        }
    }

    private IEnumerator DoAttackCombo()
    {
        isAttacking = true;
        wantCombo   = false;

        // — determine attackDir just once
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
        yield return null;                                     // let state register
        float len1 = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;
        yield return new WaitForSeconds(len1);

        // reset first attack flag
        anim.SetBool(HashAttack, false);

        // —— COMBO? ——
        if (wantCombo)
        {
            wantCombo = false;
            anim.SetBool(HashAttack2, true);
            anim.Play(StateAttack2, 0, 0f);
            yield return null;
            float len2 = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;
            yield return new WaitForSeconds(len2);
            anim.SetBool(HashAttack2, false);
        }

        // done
        isAttacking = false;
    }
}
