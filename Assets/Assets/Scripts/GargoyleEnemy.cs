using UnityEngine;

public class GargoyleEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolDistance = 5f; // Distance to patrol left and right from start position
    [SerializeField] private float patrolSpeed = 2f; // Speed of patrol movement

    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private float currentDirection = -1f; // -1 for left, 1 for right

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        UpdateSpriteDirection();
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        // Move in current direction
        transform.Translate(new Vector3(currentDirection * patrolSpeed * Time.deltaTime, 0, 0));

        // Check if reached patrol boundary
        float distanceFromStart = Mathf.Abs(transform.position.x - startPosition.x);
        if (distanceFromStart >= patrolDistance)
        {
            // Reverse direction and flip sprite
            currentDirection *= -1f;
            UpdateSpriteDirection();
        }
    }

    private void UpdateSpriteDirection()
    {
        // Flip sprite based on direction: -1 (left) = normal, 1 (right) = flipped
        spriteRenderer.flipX = (currentDirection > 0);
    }
}
