using UnityEngine;

public class StoneEnemy : MonoBehaviour
{
    [Header("Fireball Spawn Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform spawnPoint; // Position where fireball spawns from
    [SerializeField] private float spawnInterval = 2f; // Seconds between fireball spawns
    [SerializeField] private float fireballSpeed = 4f; // Speed of spawned fireballs

    [Header("Direction")]
    [SerializeField] private float facingDirection = -1f; // -1 for left, 1 for right

    private SpriteRenderer spriteRenderer;
    private float spawnTimer = 0f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSpriteDirection();
        spawnTimer = spawnInterval; // Spawn first fireball immediately
    }

    private void Update()
    {
        SpawnFireball();
    }

    private void SpawnFireball()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
            GameObject fireballInstance = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

            Fireball fireball = fireballInstance.GetComponent<Fireball>();
            if (fireball != null)
            {
                fireball.Initialize(facingDirection, fireballSpeed);
            }

            spawnTimer = spawnInterval;
        }
    }

    private void UpdateSpriteDirection()
    {
        // Flip sprite based on direction: -1 (left) = normal, 1 (right) = flipped
        spriteRenderer.flipX = (facingDirection > 0);
    }
}
