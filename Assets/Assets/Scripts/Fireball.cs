using UnityEngine;

public class Fireball : MonoBehaviour
{
    private float direction = -1f; // -1 for left, 1 for right
    private float speed = 4f;
    private SpriteRenderer spriteRenderer;
    private float lifetime = 10f; // Destroy fireball after 10 seconds if it doesn't hit anything

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Move fireball
        transform.Translate(new Vector3(direction * speed * Time.deltaTime, 0, 0));

        // Destroy after lifetime
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float newDirection, float newSpeed)
    {
        direction = newDirection;
        speed = newSpeed;

        // Flip sprite based on direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direction > 0);
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = (direction > 0);
        }
    }
}
