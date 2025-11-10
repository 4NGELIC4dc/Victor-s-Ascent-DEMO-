using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(VideoPlayer))]
public class EndingCutsceneController : MonoBehaviour
{
    [Header("Video / Audio")]
    [Tooltip("VideoClip to play (assign in inspector)")]
    public VideoClip videoClip; // assign GoodEndingCutscene or BadEndingCutscene in inspector
    private VideoPlayer vPlayer;
    [SerializeField] private AudioSource audioSource; // assign the AudioSource component

    [Header("UI Buttons (assign in inspector)")]
    public GameObject buttonMainMenu; // Good and Bad use this
    public GameObject buttonTryAgain;  // Bad ending only (can be null in GoodEnding)

    [Header("Timing")]
    [Tooltip("Seconds after which buttons appear (set to 8)")]
    public float showButtonsAfterSeconds = 10f;

    [Header("Scene names")]
    public string mainMenuSceneName = "MainMenu"; // set to your main menu scene name
    public string tryAgainSceneName = "GameScene"; // set to the scene to retry

    private bool hasPlayed = false;

    private void Awake()
    {
        vPlayer = GetComponent<VideoPlayer>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Safety: disable buttons at start
        if (buttonMainMenu != null) buttonMainMenu.SetActive(false);
        if (buttonTryAgain != null) buttonTryAgain.SetActive(false);
    }

    private void Start()
    {
        if (videoClip == null)
        {
            Debug.LogError("[EndingCutsceneController] No videoClip assigned!");
            return;
        }

        // Setup VideoPlayer
        vPlayer.playOnAwake = false;
        vPlayer.isLooping = false;
        vPlayer.clip = videoClip;

        // Route audio to audio source
        vPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        vPlayer.SetTargetAudioSource(0, audioSource);

        // Subscribe to end event (optional)
        vPlayer.loopPointReached += OnVideoFinished;

        // Play once
        StartCoroutine(PlayOnceAndShowButtons());
    }

    private IEnumerator PlayOnceAndShowButtons()
    {
        if (hasPlayed) yield break;
        hasPlayed = true;

        // Play
        vPlayer.Prepare();
        while (!vPlayer.isPrepared)
            yield return null;

        vPlayer.Play();
        if (audioSource != null)
            audioSource.Play();

        // Start coroutine to show buttons after fixed delay
        StartCoroutine(ShowButtonsAfterDelay(showButtonsAfterSeconds));

        // Optionally wait for video to finish if you need to do something on end:
        // while (vPlayer.isPlaying) yield return null;
    }

    private IEnumerator ShowButtonsAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (buttonMainMenu != null) buttonMainMenu.SetActive(true);
        if (buttonTryAgain != null) buttonTryAgain.SetActive(true);
    }

    // Optional: video finished handler
    private void OnVideoFinished(VideoPlayer vp)
    {
        // Do anything when video truly ends. We already show the buttons by timer separately.
        Debug.Log("[EndingCutsceneController] Video finished.");
    }

    // ---------- UI button handlers ----------
    public void OnButtonMainMenu()
    {
        // load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnButtonTryAgain()
    {
        // load the retry scene
        SceneManager.LoadScene(tryAgainSceneName);
    }

    private void OnDestroy()
    {
        if (vPlayer != null)
            vPlayer.loopPointReached -= OnVideoFinished;
    }
}
