using UnityEngine;

public class GargoyleEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;

    [Header("Patrol Bounds (Optional)")]
    [Tooltip("Leave empty to auto-detect screen edges or use Border colliders.")]
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;

    [Header("Audio")]
    [Tooltip("Assign wingflapsfx.mp3 here.")]
    [SerializeField] private float hearingRadius = 5f; 
    [SerializeField, Range(0f, 1f)] private float baseVolume = 1f;

    private SpriteRenderer spriteRenderer;
    private float leftLimit;
    private float rightLimit;
    private float direction = -1f; // -1 = left, 1 = right

    private AudioSource audioSource;
    private Transform player; // cached player transform

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Audio setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning($"{name} has no AudioSource component. Add one to play wingflap SFX.");
        }
        else
        {
            // Ensure audioSource.loop = true and playOnAwake = true in inspector OR play here
            if (!audioSource.isPlaying) audioSource.Play();
            audioSource.volume = baseVolume; // actual volume will be scaled each frame
        }

        // cache player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

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
        UpdateAudioVolume();
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

    private void UpdateAudioVolume()
    {
        if (audioSource == null) return;

        if (player == null)
        {
            // fallback: ensure audio is audible
            audioSource.volume = baseVolume;
            return;
        }

        float dist = Vector2.Distance(player.position, transform.position);
        // linear attenuation: 1 at 0 distance, 0 at hearingRadius or beyond
        float t = Mathf.Clamp01(1f - (dist / hearingRadius));
        audioSource.volume = baseVolume * t;
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
