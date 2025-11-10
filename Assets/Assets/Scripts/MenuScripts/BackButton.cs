using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource buttonAudio;

    [Header("Main Menu Scene")]
    public string mainMenuSceneName = "MainMenu";

    public void OnBackButtonClicked()
    {
        // Play sound
        if (buttonAudio != null)
            buttonAudio.Play();

        // Delay scene load slightly so the sound can play
        Invoke(nameof(LoadMainMenu), 0.3f);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
