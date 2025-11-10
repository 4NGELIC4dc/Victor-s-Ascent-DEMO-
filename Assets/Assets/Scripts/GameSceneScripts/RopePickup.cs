using UnityEngine;

public class RopePickup : MonoBehaviour
{
    [SerializeField] private GameObject prompt;

    private void Awake()
    {
        if (prompt != null) prompt.SetActive(false);
    }

    public void Interact()
    {
        GameState.Instance.RopeCollected = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && prompt != null)
            prompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && prompt != null)
            prompt.SetActive(false);
    }
}
