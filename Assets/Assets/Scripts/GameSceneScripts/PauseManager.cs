using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private FadeController fadeController; 
    private bool isPaused = false;

    private void Start()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        if (pauseUI != null)
            pauseUI.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    private void ResumeGame()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartWithFade());
    }
    public void GoToMainMenu()
    {
        StartCoroutine(GoToMainMenuWithFade());
    }

    private IEnumerator GoToMainMenuWithFade()
    {
        Time.timeScale = 1f;

        if (fadeController != null)
        {
            // Play fade out animation before switching
            yield return StartCoroutine(fadeController.FadeOut());
        }

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator RestartWithFade()
    {
        Time.timeScale = 1f;

        if (fadeController != null)
        {
            // Play fade out animation (fade to black)
            yield return StartCoroutine(fadeController.FadeOut());
        }

        // Once fade completes, reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
