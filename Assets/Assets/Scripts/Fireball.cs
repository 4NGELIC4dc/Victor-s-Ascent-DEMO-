using UnityEngine;

public class Fireball : MonoBehaviour
{
    private float direction = -1f;
    private float speed = 4f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool hasExploded = false;

    private float lifetime = 10f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (hasExploded) return;

        // Move fireball
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        // Backup lifetime (if never hits border)
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Border"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Stop moving
        speed = 0f;

        // Trigger explosion animation
        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }

        // Destroy after explosion
        Destroy(gameObject, 0.4f);
    }

    public void Initialize(float newDirection, float newSpeed)
    {
        direction = newDirection;
        speed = newSpeed;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = (direction > 0);
    }
}
