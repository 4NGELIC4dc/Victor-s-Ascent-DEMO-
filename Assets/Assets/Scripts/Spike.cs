using UnityEngine;

public class Spike : MonoBehaviour
{
    // This script marks spikes as hazards
    // They will damage Victor on collision through the Harmful layer system

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Optional: Add any spike-specific behavior here
        // For now, Victor's death system handles collision with Harmful layer
    }
}
