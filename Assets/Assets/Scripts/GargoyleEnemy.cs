using UnityEngine;

public class GargoyleEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;

    [Header("Patrol Bounds (Optional)")]
    [Tooltip("Leave empty to auto-detect screen edges or use Border colliders.")]
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;

    private SpriteRenderer spriteRenderer;
    private float leftLimit;
    private float rightLimit;
    private float direction = -1f; // -1 = left, 1 = right

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If no boundaries are assigned, use camera view edges
        if (leftBoundary == null || rightBoundary == null)
        {
            Camera cam = Camera.main;
            Vector3 leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
            Vector3 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));

            leftLimit = leftEdge.x + 0.5f;
            rightLimit = rightEdge.x - 0.5f;
        }
        else
        {
            leftLimit = leftBoundary.position.x;
            rightLimit = rightBoundary.position.x;
        }
    }

    private void Update()
    {
        MoveGargoyle();
    }

    private void MoveGargoyle()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Flip when reaching limits
        if (transform.position.x <= leftLimit)
        {
            direction = 1f;
            spriteRenderer.flipX = true;
        }
        else if (transform.position.x >= rightLimit)
        {
            direction = -1f;
            spriteRenderer.flipX = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            // Flip direction upon hitting border collider
            direction *= -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }
}
