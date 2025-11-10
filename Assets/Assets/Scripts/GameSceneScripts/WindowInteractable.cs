using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowInteractable : MonoBehaviour
{
    [SerializeField] private string goodEndingScene = "GoodEnding";
    [SerializeField] private string badEndingScene = "BadEnding";
    [SerializeField] private GameObject prompt;

    private void Awake()
    {
        if (prompt != null) prompt.SetActive(false);
    }

    public void Interact()
    {
        SceneManager.LoadScene(GameState.Instance.RopeCollected ? goodEndingScene : badEndingScene);
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
