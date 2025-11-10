using UnityEngine;

public class FireballRight : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float flyDuration = 4f; // how long it flies before exploding
    [SerializeField] private float explosionDuration = 0.45f; // explosion animation time

    private Animator animator;
    private bool hasExploded = false;
    private float timer = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
            animator.Play("Fireball_Fly");
    }

    private void Update()
    {
        if (hasExploded) return;

        // Move fireball to the left
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Track flight time
        timer += Time.deltaTime;
        if (timer >= flyDuration)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Stop movement
        speed = 0f;

        // Play explosion animation
        if (animator != null)
            animator.Play("Fireball_Explode");

        // Destroy after explosion finishes
        Destroy(gameObject, explosionDuration);
    }

    public void Initialize(float newSpeed)
    {
        speed = newSpeed;
    }
}