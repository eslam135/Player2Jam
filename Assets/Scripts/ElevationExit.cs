using UnityEngine;

public class ElevationExit : MonoBehaviour
{
    [SerializeField] Collider2D[] entryColliders;
    [SerializeField] Collider2D[] boundariesColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (Collider2D collider in entryColliders)
            {
                collider.enabled = true;
            }
            foreach (Collider2D collider in boundariesColliders)
            {
                collider.enabled = false;
            }
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
        }
    }
}
