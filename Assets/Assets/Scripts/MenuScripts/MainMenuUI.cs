using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Button Sounds")]
    [SerializeField] private AudioSource buttonSFX;

    [Header("Scene Load Delay (seconds)")]
    [SerializeField] private float sceneLoadDelay = 0.3f;

    public void OnStartPressed()
    {
        PlayButtonSound();
        Invoke(nameof(LoadStartScene), sceneLoadDelay);
    }

    public void OnSettingsPressed()
    {
        PlayButtonSound();
        Invoke(nameof(LoadSettingsScene), sceneLoadDelay);
    }

    public void OnCreditsPressed()
    {
        PlayButtonSound();
        Invoke(nameof(LoadCreditsScene), sceneLoadDelay);
    }

    public void OnHowToPlayPressed()
    {
        PlayButtonSound();
        Invoke(nameof(LoadHowToPlayScene), sceneLoadDelay);
    }

    public void OnQuitPressed()
    {
        PlayButtonSound();
        Invoke(nameof(QuitGame), sceneLoadDelay);
    }

    private void PlayButtonSound()
    {
        if (buttonSFX != null)
            buttonSFX.Play();
    }

    // Load scene methods
    private void LoadStartScene() => SceneManager.LoadScene("Cutscene");
    private void LoadSettingsScene() => SceneManager.LoadScene("SettingsScene");
    private void LoadCreditsScene() => SceneManager.LoadScene("CreditsScene");
    private void LoadHowToPlayScene() => SceneManager.LoadScene("HowtoplayScene");
    private void QuitGame() => Application.Quit();
}
