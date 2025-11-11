using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSFX : MonoBehaviour
{
    public AudioClip buttonSound; 
    private AudioSource audioSource;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        // Find or create AudioSource
        audioSource = FindFirstObjectByType<AudioSource>();
        if (audioSource == null)
        {
            GameObject audioObj = new GameObject("UIAudio");
            audioSource = audioObj.AddComponent<AudioSource>();
        }

        // Add listener for click
        button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (buttonSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
        else
        {
            Debug.LogWarning("[ButtonSFX] Missing AudioClip or AudioSource!");
        }
    }
}
