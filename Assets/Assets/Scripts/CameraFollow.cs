using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Camera Settings")]
    [SerializeField] private float smoothSpeed = 0.3f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

    private void LateUpdate()
    {
        if (playerTransform == null)
            return;

        // Calculate desired position
        Vector3 desiredPosition = playerTransform.position + offset;

        // Smooth transition to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply new position
        transform.position = smoothedPosition;
    }
}
