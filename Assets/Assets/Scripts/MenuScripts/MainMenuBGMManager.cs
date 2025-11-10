using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBGMManager : MonoBehaviour
{
    private static MainMenuBGMManager instance;

    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only play in menu-related scenes
        if (scene.name == "MainMenu" || scene.name == "SettingsScene" || scene.name == "CreditsScene" || scene.name == "HowtoplayScene")
        {
            if (!bgmSource.isPlaying)
                bgmSource.Play();
        }
        else
        {
            bgmSource.Stop();
        }
    }
}
