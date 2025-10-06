using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using Photon.Pun;

public class VideoPlayerUIController : MonoBehaviour
{
    public Button playButton; // Button for playing the video
    public Button pauseButton; // Button for pausing the video
    public Button forwardButton;
    public Button backwardButton;

    public Button loopButton;

    public Slider durationSlider;
    public TextMeshProUGUI currentTimeText; // Text to show current video time
    public TextMeshProUGUI totalDurationText; // Text to show total video duration

    private VideoPlayer videoPlayer;
    private PhotonView photonView;
    private bool isDraggingSlider = false;
    private const float skipTime = 5f;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        photonView = GetComponent<PhotonView>();

        // Validate component assignments
        if (videoPlayer == null)
            Debug.LogError("VideoPlayer component is missing!");
        if (photonView == null)
            Debug.LogError("PhotonView component is missing!");
        if (playButton == null)
            Debug.LogError("PlayButton is not assigned in the Inspector!");
        if (pauseButton == null)
            Debug.LogError("PauseButton is not assigned in the Inspector!");
        if (loopButton == null)

            Debug.LogError("LoopButton is not assigned in the Inspector!");

        // Setup button listeners
        if (playButton != null)
            playButton.onClick.AddListener(PlayVideo);
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseVideo);
        if (forwardButton != null)
            forwardButton.onClick.AddListener(SkipForward);
        if (backwardButton != null)
            backwardButton.onClick.AddListener(SkipBackward);
        if (loopButton != null)

            loopButton.onClick.AddListener(ToggleLoop);
        if (durationSlider != null)
        durationSlider.onValueChanged.AddListener(OnDurationSliderChanged);

        // Add listener for when the video reaches the end
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoEnded;

        // Initialize UI state
        UpdatePlayPauseButtonUI(false);

        UpdateLoopButtonUI(false);
        Debug.Log("Initial UI state set: Play button active, Pause button inactive");
    }

    void Update()
    {
        if (videoPlayer != null && videoPlayer.isPrepared && !isDraggingSlider)
        {
            durationSlider.value = (float)videoPlayer.time;
            if (currentTimeText != null)
            {
                float currentTime = (float)videoPlayer.time;
                currentTimeText.text = FormatTime(currentTime);
            }

            // Ensure UI reflects playing state
            if (videoPlayer.isPlaying && !pauseButton.gameObject.activeSelf)
            {
                Debug.LogWarning("Video is playing but pause button is not visible! Forcing UI update.");
                UpdatePlayPauseButtonUI(true);
            }
        }
    }

    private void PlayVideo()
    {
        if (videoPlayer == null) return;
        Debug.Log("PlayVideo called, requesting video playback");
        photonView.RPC("RPC_PlayVideo", RpcTarget.AllBuffered);
    }

    private void PauseVideo()
    {
        if (videoPlayer == null) return;
        Debug.Log("PauseVideo called, requesting video pause");
        photonView.RPC("RPC_PauseVideo", RpcTarget.AllBuffered);
    }

    private void SkipForward()
    {
        if (videoPlayer == null) return;
        float newTime = (float)videoPlayer.time + skipTime;
        newTime = Mathf.Clamp(newTime, 0, (float)videoPlayer.length);
        photonView.RPC("RPC_SetVideoTime", RpcTarget.AllBuffered, newTime);
    }

    private void SkipBackward()
    {
        if (videoPlayer == null) return;
        float newTime = (float)videoPlayer.time - skipTime;
        newTime = Mathf.Clamp(newTime, 0, (float)videoPlayer.length);
        photonView.RPC("RPC_SetVideoTime", RpcTarget.AllBuffered, newTime);
    }

    private void ToggleLoop()

    {

        if (videoPlayer == null) return;

        Debug.Log("ToggleLoop called, requesting loop toggle");

        photonView.RPC("RPC_ToggleLoop", RpcTarget.AllBuffered);

    }




    private void OnDurationSliderChanged(float value)
    {
        if (videoPlayer == null || !isDraggingSlider) return;
        photonView.RPC("RPC_SetVideoTime", RpcTarget.AllBuffered, value);
    }

    public void OnSliderDragStart()
    {
        isDraggingSlider = true;
    }

    public void OnSliderDragEnd()
    {
        isDraggingSlider = false;
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        Debug.Log("Video ended, showing play button");
        photonView.RPC("RPC_UpdatePlayPauseUI", RpcTarget.AllBuffered, false);
    }

    private void UpdatePlayPauseButtonUI(bool isPlaying)
    {
        if (playButton != null)
        {
            playButton.gameObject.SetActive(!isPlaying); // Show play button when paused or ended
            Debug.Log($"Play button set to active: {!isPlaying}");
            TextMeshProUGUI playText = playButton.GetComponentInChildren<TextMeshProUGUI>();
            if (playText != null)
                playText.text = "Play";
        }
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(isPlaying); // Show pause button only when playing
            Debug.Log($"Pause button set to active: {isPlaying}");
            TextMeshProUGUI pauseText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (pauseText != null)
                pauseText.text = "Pause";
        }
    }

    private void UpdateLoopButtonUI(bool isLooping)

    {

        if (loopButton != null)

        {

            TextMeshProUGUI loopText = loopButton.GetComponentInChildren<TextMeshProUGUI>();

            if (loopText != null)

                loopText.text = isLooping ? "On" : "Off";

            Debug.Log($"Loop button text set to: {(isLooping ? "On" : "Off")}");

        }

    }


    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    [PunRPC]
    void RPC_PlayVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            UpdatePlayPauseButtonUI(true);
            Debug.Log("RPC_PlayVideo: Video started, pause button should be visible, play button hidden");
        }
        else
        {
            Debug.LogError("RPC_PlayVideo: VideoPlayer is null!");
        }
    }

    [PunRPC]
    void RPC_PauseVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Pause();
            UpdatePlayPauseButtonUI(false);
            Debug.Log("RPC_PauseVideo: Video paused, play button should be visible, pause button hidden");
        }
        else
        {
            Debug.LogError("RPC_PauseVideo: VideoPlayer is null!");
        }
    }

    [PunRPC]
    void RPC_SetVideoTime(float time)
    {
        if (videoPlayer != null)
        {
            videoPlayer.time = time;
            Debug.Log($"RPC_SetVideoTime: Video time set to {time}");
        }
    }

    [PunRPC]
    void RPC_UpdatePlayPauseUI(bool isPlaying)
    {
        UpdatePlayPauseButtonUI(isPlaying);
        Debug.Log($"RPC_UpdatePlayPauseUI: isPlaying={isPlaying}, Play button active: {!isPlaying}, Pause button active: {isPlaying}");
    }


    [PunRPC]

    void RPC_ToggleLoop()

    {

        if (videoPlayer != null)

        {

            videoPlayer.isLooping = !videoPlayer.isLooping;

            UpdateLoopButtonUI(videoPlayer.isLooping);

            Debug.Log($"RPC_ToggleLoop: Looping set to {videoPlayer.isLooping}");

        }

        else

        {

            Debug.LogError("RPC_ToggleLoop: VideoPlayer is null!");

        }

    }


    public void SetupSlider(float duration)
    {
        if (durationSlider != null)
        {
            durationSlider.minValue = 0;
            durationSlider.maxValue = duration;
            durationSlider.value = 0;
        }
        if (totalDurationText != null)
        {
            totalDurationText.text = FormatTime(duration);
        }
    }

    void OnDestroy()
    {
        // Clean up the event listener
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnded;
    }
}



