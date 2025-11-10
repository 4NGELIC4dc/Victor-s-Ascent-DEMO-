using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        // Optional: Safety check to make sure AudioSource is assigned
        if (bgmSource == null)
            bgmSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Play the BGM as soon as GameScene starts
        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.Play();
    }
}
