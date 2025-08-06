using UnityEngine;

public class FragmentController : MonoBehaviour
{
    private Animator anim;
    private CircleCollider2D coll;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            coll.radius = 0;
            anim.Play("death");
        }
    }
    public void destroyObject()
    {
        Destroy(gameObject);
    }
    public void updateLevelFrags()
    {
        Level2Manager.foundFragments += 1;
        Debug.Log(Level2Manager.foundFragments);
    }
}
