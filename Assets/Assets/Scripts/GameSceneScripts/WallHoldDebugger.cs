// WallHoldDebugger.cs
using UnityEngine;

public class WallHoldDebugger : MonoBehaviour
{
    [Tooltip("Assign the same wallCheck Transform you used in VictorController.")]
    public Transform wallCheck;
    public float radius = 0.25f;
    public string chainLayerName = "Chain";

    private int frameCounter = 0;

    void Start()
    {
        Debug.Log("[WHD] Start() running. Component enabled? " + enabled + " GameObject active? " + gameObject.activeInHierarchy);
        if (wallCheck == null) Debug.LogWarning("[WHD] wallCheck is NULL. Assign it in Inspector.");
    }

    void Update()
    {
        frameCounter++;
        // SIMPLE heartbeat so we know Update() is firing
        if (frameCounter % 60 == 0) // every 60 frames
            Debug.Log("[WHD] heartbeat (frame " + frameCounter + ")");

        if (wallCheck == null) return;

        // Print the wallCheck world pos once (first frame)
        if (frameCounter == 1) Debug.Log($"[WHD] wallCheck position: {wallCheck.position}");

        // DO THE OVERLAP CHECK using explicit layer name
        int mask = LayerMask.GetMask(chainLayerName);
        Collider2D hit = Physics2D.OverlapCircle(wallCheck.position, radius, mask);

        if (hit != null)
        {
            Debug.Log($"[WHD] OVERLAP DETECTED -> collider name: '{hit.name}', collider layer: '{LayerMask.LayerToName(hit.gameObject.layer)}'");
        }
        else
        {
            // Print a single message once every 120 frames so console doesn't spam
            if (frameCounter % 120 == 0)
                Debug.Log($"[WHD] no overlap at {wallCheck.position} (radius {radius}) with layer '{chainLayerName}'");
        }
    }

    // Draw the circle in scene view for visual verification
    private void OnDrawGizmosSelected()
    {
        if (wallCheck == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(wallCheck.position, radius);
    }
}
