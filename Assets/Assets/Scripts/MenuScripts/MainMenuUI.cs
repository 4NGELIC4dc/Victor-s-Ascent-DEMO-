using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Button Sounds")]
    [SerializeField] private AudioSource buttonSFX;

    public void OnStartPressed()
    {
        PlayButtonSound();
        SceneManager.LoadScene("Cutscene");
    }

    public void OnSettingsPressed()
    {
        PlayButtonSound();
        SceneManager.LoadScene("SettingsScene");
    }

    public void OnCreditsPressed()
    {
        PlayButtonSound();
        SceneManager.LoadScene("CreditsScene");
    }

    public void OnHowToPlayPressed()
    {
        PlayButtonSound();
        SceneManager.LoadScene("HowtoplayScene");
    }

    public void OnQuitPressed()
    {
        PlayButtonSound();
        Application.Quit();
    }

    private void PlayButtonSound()
    {
        if (buttonSFX != null)
            buttonSFX.Play();
    }
}
