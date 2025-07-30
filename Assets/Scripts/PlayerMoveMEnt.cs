using UnityEngine;

public class PlayerMoveMEnt : MonoBehaviour
{
    public int facingDirection = 1;
    [SerializeField] private float speed = 5;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
         

        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));


        rb.linearVelocity = new Vector2(horizontal, vertical) * speed; 
    }
    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
}
