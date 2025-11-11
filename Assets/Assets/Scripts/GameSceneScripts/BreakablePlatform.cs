using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isPlayerOnPlatform = false;
    private bool isPlatformBroken = false;
    private float breakTimer = 0f;
    private float regenerateTimer = 0f;


    public float timeToBreak = 2f; // Time before platform breaks while player is on it
    public float timeToRegenerate = 2f; // Time before platform regenerates
    public float blinkSpeed = 0.1f; // Blink speed (lower = faster blink)

    // Red color for blinking warning
    private Color redColor = new Color(1f, 0f, 0f, 1f);
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        originalColor = spriteRenderer.color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;

            if (!isPlatformBroken)
                breakTimer = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
            breakTimer = 0f;
        }
    }

    private void Update()
    {
        if (isPlayerOnPlatform && !isPlatformBroken)
        {
            breakTimer += Time.deltaTime;

            float blinkValue = Mathf.Sin(breakTimer / blinkSpeed * Mathf.PI) * 0.5f + 0.5f;
            spriteRenderer.color = Color.Lerp(originalColor, redColor, blinkValue);

            if (breakTimer >= timeToBreak)
                BreakPlatform();
        }
        else if (!isPlayerOnPlatform && !isPlatformBroken)
        {
            spriteRenderer.color = originalColor;
            breakTimer = 0f;
        }

        if (isPlatformBroken)
        {
            regenerateTimer += Time.deltaTime;

            if (regenerateTimer >= timeToRegenerate)
                RegeneratePlatform();
        }
    }

    private void BreakPlatform()
    {
        isPlatformBroken = true;
        platformCollider.enabled = false;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
        regenerateTimer = 0f;
    }

    private void RegeneratePlatform()
    {
        isPlatformBroken = false;
        platformCollider.enabled = true;
        spriteRenderer.color = originalColor;
        breakTimer = 0f;
        regenerateTimer = 0f;
    }
}
