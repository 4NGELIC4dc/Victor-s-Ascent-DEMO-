using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Camera Settings")]
    [SerializeField] private float smoothSpeed = 0.25f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

    [Header("Camera Boundaries")]
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;
    [SerializeField] private Transform bottomBoundary;
    [SerializeField] private Transform topBoundary;

    private Camera cam;
    private float halfHeight;
    private float halfWidth;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        UpdateCameraDimensions();

        // Snap to player instantly on start
        if (playerTransform != null)
        {
            Vector3 startPosition = playerTransform.position + offset;
            startPosition = ClampToBounds(startPosition);
            transform.position = startPosition;
        }
    }

    private void LateUpdate()
    {
        if (playerTransform == null || leftBoundary == null || rightBoundary == null) return;

        UpdateCameraDimensions();

        // Target follow position
        Vector3 targetPosition = playerTransform.position + offset;

        // Clamp to boundaries
        targetPosition = ClampToBounds(targetPosition);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        float minX = leftBoundary.position.x + halfWidth - 0.05f;
        float maxX = rightBoundary.position.x - halfWidth + 0.05f;
        float minY = bottomBoundary != null ? bottomBoundary.position.y + halfHeight - 0.05f : position.y;
        float maxY = topBoundary != null ? topBoundary.position.y - halfHeight + 0.05f : position.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }

    private void UpdateCameraDimensions()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize camera boundaries in the Scene view
        Gizmos.color = Color.yellow;
        if (leftBoundary != null && rightBoundary != null)
        {
            Gizmos.DrawLine(new Vector3(leftBoundary.position.x, -100f, 0), new Vector3(leftBoundary.position.x, 100f, 0));
            Gizmos.DrawLine(new Vector3(rightBoundary.position.x, -100f, 0), new Vector3(rightBoundary.position.x, 100f, 0));
        }
    }
}
