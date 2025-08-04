using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoPlayer : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "MainMenu";
    [SerializeField] private bool skipOnAnyKey = true;
    private bool isSkipping = false;

    private void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void Update()
    {
        if (skipOnAnyKey && !isSkipping && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            isSkipping = true;
            LoadNextScene();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (!isSkipping)
        {
            isSkipping = true;
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
} 