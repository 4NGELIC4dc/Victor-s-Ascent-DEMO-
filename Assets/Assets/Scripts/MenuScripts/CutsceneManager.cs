using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    [Header("Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "GameScene";

    private void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer not assigned in CutsceneManager!");
            return;
        }

        // When video finishes, call OnVideoEnd
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // When video is done, load next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
