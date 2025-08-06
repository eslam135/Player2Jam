using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCoolDown = 2f;
    [SerializeField] private float playerDetectRange = 5f;


    [SerializeField] private Transform detectionPoint;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private EnemyState enemyState;
    [SerializeField] private Vector2 dir;


    private float attackCoolDownTimer;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    private int facingDir = 1;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isTalking) return;
        CheckForPlayer();
        if (attackCoolDownTimer > 0) attackCoolDownTimer -= Time.deltaTime; 


        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if(enemyState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Chase()
    {

        if (player.position.x > transform.position.x && facingDir == -1 ||
            player.position.x < transform.position.x && facingDir == 1)
        {
            Flip();
        }

        Vector2 normalVec = player.position - transform.position;
        dir = normalVec.normalized;
        rb.linearVelocity = dir * speed;

    }


    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerMask);

        if (hits.Length > 0)
        {
            player = hits[0].transform;

            Vector2 delta = player.position - transform.position;
            float dis = delta.magnitude;

            if (dis <= attackRange && attackCoolDownTimer <= 0)
            {
                attackCoolDownTimer = attackCoolDown;

                float angle = Vector2.Angle(Vector2.up, delta);
                float yDiff = delta.y;
                float xDiff = delta.x;

                if (Mathf.Abs(yDiff) > Mathf.Abs(xDiff))
                {
                    if (yDiff < 0)
                        ChangeState(EnemyState.AttackingUp);
                    else
                        ChangeState(EnemyState.AttackingDown);
                }
                else
                {
                    ChangeState(EnemyState.Attacking); 
                }
            }
            else if (dis > attackRange)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    public void Flip()
    {
        facingDir *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void ChangeState(EnemyState newState)
    {
        enemyState = newState;

        anim.SetBool("isIdle", newState == EnemyState.Idle);
        anim.SetBool("isChasing", newState == EnemyState.Chasing);
        anim.SetBool("isAttacking", newState == EnemyState.Attacking);
        anim.SetBool("isAttackingUp", newState == EnemyState.AttackingUp);
        anim.SetBool("isAttackingDown", newState == EnemyState.AttackingDown);


    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    AttackingUp,
    AttackingDown,
}
