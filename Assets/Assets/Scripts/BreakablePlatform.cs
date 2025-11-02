using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private Vector3 originalPosition;
    private bool isPlayerOnPlatform = false;
    private bool isPlatformBroken = false;
    private float breakTimer = 0f;
    private float regenerateTimer = 0f;

    // ADJUSTABLE: Time before platform breaks while player is on it
    public float timeToBreak = 3f;

    // ADJUSTABLE: Time before platform regenerates
    public float timeToRegenerate = 3f;

    // ADJUSTABLE: Blink speed (lower = faster blink)
    public float blinkSpeed = 0.1f;

    // Red color for blinking warning
    private Color redColor = new Color(1f, 0f, 0f, 1f);
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        originalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;

            // Only start breaking if platform is not already broken
            if (!isPlatformBroken)
            {
                breakTimer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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

            // Break the platform when timer reaches timeToBreak
            if (breakTimer >= timeToBreak)
            {
                BreakPlatform();
            }
        }
        else if (!isPlayerOnPlatform && !isPlatformBroken)
        {
            spriteRenderer.color = originalColor;
            breakTimer = 0f;
        }

        // Handle regeneration
        if (isPlatformBroken)
        {
            regenerateTimer += Time.deltaTime;

            if (regenerateTimer >= timeToRegenerate)
            {
                RegeneratePlatform();
            }
        }
    }

    private void BreakPlatform()
    {
        isPlatformBroken = true;
        platformCollider.enabled = false;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f); // Make semi-transparent
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
