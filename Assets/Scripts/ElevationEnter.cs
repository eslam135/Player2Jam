using UnityEngine;

public class ElevationEnter : MonoBehaviour
{
    [SerializeField] Collider2D[] entryColliders;
    [SerializeField] Collider2D[] boundariesColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (Collider2D collider in entryColliders)
            {
                collider.enabled = false;
            }
            foreach (Collider2D collider in boundariesColliders)
            {
                collider.enabled = true;    
            }
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 20;
        }
    }
}
