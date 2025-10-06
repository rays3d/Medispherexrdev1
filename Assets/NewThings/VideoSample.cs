/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Video;

public class VideoSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject loadingGif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Video Instantiation")]
    public GameObject prefabWithVideoPlayer; // Prefab with VideoPlayer component
    public Transform spawnPoint;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Video Download Variables
    private string currentVideoUrl = "";
    private int currentVideoViewID = -1;
    private string currentVideoName = "";
    private videos[] videoDataArray; // Store video data for lookup

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Video Loader Ready.");
    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchVideoNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("VideoRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void FetchVideoNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetVideoNamesFromAPI());
    }

    private IEnumerator GetVideoNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage_new/src/video/view_videos.php"; // Adjust API endpoint for videos
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching video names: {request.error}. URL: {url}");
                yield break;
            }

            string jsonResult = request.downloadHandler.text;
            VideoListResponse videoListResponse = JsonUtility.FromJson<VideoListResponse>(jsonResult);

            if (videoListResponse != null && videoListResponse.videos != null)
            {
                videoDataArray = videoListResponse.videos;

                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (videos video in videoListResponse.videos)
                {
                    if (video != null && !string.IsNullOrEmpty(video.name))
                    {
                        AddVideoNameToUI(video.id, video.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Video list response is null or empty.");
            }
        }
    }

    private void AddVideoNameToUI(string videoId, string videoName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = videoName;
                button.onClick.AddListener(() => OnVideoNameClicked(videoId, videoName));
            }
            else
            {
                Debug.LogError("The textPrefab does not have required components.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    private void OnVideoNameClicked(string videoId, string videoName)
    {
        if (string.IsNullOrEmpty(videoId))
        {
            Debug.LogError("Video ID is empty. Cannot fetch video.");
            return;
        }

        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Loading {videoName}...";
        }
        if (loadingGif != null)
        {
            loadingGif.SetActive(true);
        }

        currentVideoUrl = "http://192.168.1.26/storage_new/src/video/download_video.php?id=" + videoId; // Adjust API endpoint
        currentVideoName = videoName;

        Debug.Log($"Constructed Video URL: {currentVideoUrl}");

        photonView.RPC("RPC_LoadVideoFromUrl", RpcTarget.AllBuffered, currentVideoUrl, videoId, videoName);
    }

    [PunRPC]
    void RPC_LoadVideoFromUrl(string url, string videoId, string videoName)
    {
        currentVideoUrl = url;
        currentVideoName = videoName;
        StartCoroutine(SetupAndPlayVideo());
    }

    IEnumerator SetupAndPlayVideo()
    {
        Debug.Log("Starting Video Setup...");
        if (string.IsNullOrEmpty(currentVideoUrl))
        {
            Debug.LogError("Video URL is empty. Cannot load video.");
            UpdateDownloadUI(false);
            yield break;
        }

        if (photonView.IsMine)
        {
            GameObject videoInstance = PhotonNetwork.Instantiate(prefabWithVideoPlayer.name, spawnPoint.position, Quaternion.identity);
            currentVideoViewID = videoInstance.GetComponent<PhotonView>().ViewID;

            StartCoroutine(ConfigureVideoPlayer(videoInstance, currentVideoUrl));

            photonView.RPC("SyncVideoData", RpcTarget.OthersBuffered, currentVideoViewID, currentVideoUrl);
        }

        UpdateDownloadUI(true);
    }

    private IEnumerator ConfigureVideoPlayer(GameObject targetObject, string videoUrl)
    {
        VideoPlayer videoPlayer = targetObject.GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = targetObject.GetComponentInChildren<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.url = videoUrl;
            videoPlayer.isLooping = false;
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride; // Ensure proper rendering
            //videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetMaterialRenderer = targetObject.GetComponent<Renderer>(); // Assign renderer if using MaterialOverride
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                Debug.Log($"Preparing video: {videoUrl}");
                yield return null;
            }

            Debug.Log($"Video prepared. Width: {videoPlayer.width}, Height: {videoPlayer.height}, FrameRate: {videoPlayer.frameRate}");
            videoPlayer.Play();
            Debug.Log($"Video {currentVideoName} is playing.");
        }
        else
        {
            Debug.LogError("No VideoPlayer component found on the target object.");
            UpdateDownloadUI(false);
        }
    }




    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentVideoName} loaded successfully!"
                : $"Failed to load {currentVideoName}";

            StartCoroutine(HideDownloadUI());
        }

        if (loadingGif != null)
        {
            loadingGif.SetActive(false);
        }
    }

    private IEnumerator HideDownloadUI()
    {
        yield return new WaitForSeconds(2f);
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void SyncVideoData(int viewID, string url)
    {
        currentVideoViewID = viewID;
        currentVideoUrl = url;
        StartCoroutine(DownloadAndApplyVideo(viewID, url));
    }

    private IEnumerator DownloadAndApplyVideo(int viewID, string url)
    {
        PhotonView videoView = PhotonView.Find(viewID);
        if (videoView == null)
        {
            Debug.LogError("Video with ViewID " + viewID + " not found.");
            yield break;
        }

        yield return StartCoroutine(ConfigureVideoPlayer(videoView.gameObject, url));

        PhotonTransformView photonTransformView = videoView.gameObject.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = videoView.gameObject.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;
    }

    [System.Serializable]
    public class videos
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class VideoListResponse
    {
        public videos[] videos;
    }
}*/





/////////////////////////////////////////////////////////////////////////////working/////////////////////////
/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Video;

public class VideoSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject loadingGif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Video Instantiation")]
    public GameObject prefabWithVideoPlayer; // Prefab with VideoPlayer and UI
    public Transform spawnPoint;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Video Download Variables
    private string currentVideoUrl = "";
    private int currentVideoViewID = -1;
    private string currentVideoName = "";
    private videos[] videoDataArray; // Store video data for lookup

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Video Loader Ready.");
    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchVideoNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("VideoRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void FetchVideoNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetVideoNamesFromAPI());
    }

    private IEnumerator GetVideoNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage_new/src/video/view_videos.php";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching video names: {request.error}. URL: {url}");
                yield break;
            }

            string jsonResult = request.downloadHandler.text;
            VideoListResponse videoListResponse = JsonUtility.FromJson<VideoListResponse>(jsonResult);

            if (videoListResponse != null && videoListResponse.videos != null)
            {
                videoDataArray = videoListResponse.videos;

                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (videos video in videoListResponse.videos)
                {
                    if (video != null && !string.IsNullOrEmpty(video.name))
                    {
                        AddVideoNameToUI(video.id, video.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Video list response is null or empty.");
            }
        }
    }

    private void AddVideoNameToUI(string videoId, string videoName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = videoName;
                button.onClick.AddListener(() => OnVideoNameClicked(videoId, videoName));
            }
            else
            {
                Debug.LogError("The textPrefab does not have required components.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    private void OnVideoNameClicked(string videoId, string videoName)
    {
        if (string.IsNullOrEmpty(videoId))
        {
            Debug.LogError("Video ID is empty. Cannot fetch video.");
            return;
        }

        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Loading {videoName}...";
        }
        if (loadingGif != null)
        {
            loadingGif.SetActive(true);
        }

        currentVideoUrl = "http://192.168.1.26/storage_new/src/video/download_video.php?id=" + videoId;
        currentVideoName = videoName;

        Debug.Log($"Constructed Video URL: {currentVideoUrl}");

        photonView.RPC("RPC_LoadVideoFromUrl", RpcTarget.AllBuffered, currentVideoUrl, videoId, videoName);
    }

    [PunRPC]
    void RPC_LoadVideoFromUrl(string url, string videoId, string videoName)
    {
        currentVideoUrl = url;
        currentVideoName = videoName;
        StartCoroutine(SetupAndPlayVideo());
    }

    IEnumerator SetupAndPlayVideo()
    {
        Debug.Log("Starting Video Setup...");
        if (string.IsNullOrEmpty(currentVideoUrl))
        {
            Debug.LogError("Video URL is empty. Cannot load video.");
            UpdateDownloadUI(false);
            yield break;
        }

        if (photonView.IsMine)
        {
            GameObject videoInstance = PhotonNetwork.Instantiate(prefabWithVideoPlayer.name, spawnPoint.position, Quaternion.identity);
            currentVideoViewID = videoInstance.GetComponent<PhotonView>().ViewID;

            StartCoroutine(ConfigureVideoPlayer(videoInstance, currentVideoUrl));

            photonView.RPC("SyncVideoData", RpcTarget.OthersBuffered, currentVideoViewID, currentVideoUrl);
        }

        UpdateDownloadUI(true);
    }

    private IEnumerator ConfigureVideoPlayer(GameObject targetObject, string videoUrl)
    {
        VideoPlayer videoPlayer = targetObject.GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = targetObject.GetComponentInChildren<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.url = videoUrl;
            videoPlayer.isLooping = false;
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = targetObject.GetComponent<Renderer>();
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                Debug.Log($"Preparing video: {videoUrl}");
                yield return null;
            }

            // Adjust Quad scale to match video aspect ratio
            float aspectRatio = (float)videoPlayer.width / videoPlayer.height;
            targetObject.transform.localScale = new Vector3(aspectRatio * 1f, 1f, 1f);

            Debug.Log($"Video prepared. Width: {videoPlayer.width}, Height: {videoPlayer.height}, FrameRate: {videoPlayer.frameRate}");

            // Setup UI controller
            VideoPlayerUIController uiController = targetObject.GetComponent<VideoPlayerUIController>();
            if (uiController != null)
            {
                uiController.SetupSlider((float)videoPlayer.length);
            }

            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("No VideoPlayer component found on the target object.");
            UpdateDownloadUI(false);
        }
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentVideoName} loaded successfully!"
                : $"Failed to load {currentVideoName}";

            StartCoroutine(HideDownloadUI());
        }

        if (loadingGif != null)
        {
            loadingGif.SetActive(false);
        }
    }

    private IEnumerator HideDownloadUI()
    {
        yield return new WaitForSeconds(2f);
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void SyncVideoData(int viewID, string url)
    {
        currentVideoViewID = viewID;
        currentVideoUrl = url;
        StartCoroutine(DownloadAndApplyVideo(viewID, url));
    }

    private IEnumerator DownloadAndApplyVideo(int viewID, string url)
    {
        PhotonView videoView = PhotonView.Find(viewID);
        if (videoView == null)
        {
            Debug.LogError("Video with ViewID " + viewID + " not found.");
            yield break;
        }

        yield return StartCoroutine(ConfigureVideoPlayer(videoView.gameObject, url));

        PhotonTransformView photonTransformView = videoView.gameObject.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = videoView.gameObject.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;
    }

    [System.Serializable]
    public class videos
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class VideoListResponse
    {
        public videos[] videos;
    }
}*/


























/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Video;

public class VideoSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject loadingGif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Video Instantiation")]
    public GameObject prefabWithVideoPlayer; // Prefab with VideoPlayer and UI
    public Transform spawnPoint;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Video Download Variables
    private string currentVideoUrl = "";
    private int currentVideoViewID = -1;
    private string currentVideoName = "";
    private videos[] videoDataArray; // Store video data for lookup

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Video Loader Ready.");
    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchVideoNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("VideoRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void FetchVideoNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetVideoNamesFromAPI());
    }

    private IEnumerator GetVideoNamesFromAPI()
    {
        string url = "https://medispherexr.com/api/src/video/view_videos.php";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching video names: {request.error}. URL: {url}");
                yield break;
            }

            string jsonResult = request.downloadHandler.text;
            VideoListResponse videoListResponse = JsonUtility.FromJson<VideoListResponse>(jsonResult);

            if (videoListResponse != null && videoListResponse.videos != null)
            {
                videoDataArray = videoListResponse.videos;

                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (videos video in videoListResponse.videos)
                {
                    if (video != null && !string.IsNullOrEmpty(video.name))
                    {
                        AddVideoNameToUI(video.id, video.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Video list response is null or empty.");
            }
        }
    }

    private void AddVideoNameToUI(string videoId, string videoName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = videoName;
                button.onClick.AddListener(() => OnVideoNameClicked(videoId, videoName));
            }
            else
            {
                Debug.LogError("The textPrefab does not have required components.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    private void OnVideoNameClicked(string videoId, string videoName)
    {
        if (string.IsNullOrEmpty(videoId))
        {
            Debug.LogError("Video ID is empty. Cannot fetch video.");
            return;
        }

        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Loading {videoName}...";
        }
        if (loadingGif != null)
        {
            loadingGif.SetActive(true);
        }

        currentVideoUrl = "https://medispherexr.com/api/src/video/download_video.php?id=" + videoId;
        currentVideoName = videoName;

        Debug.Log($"Constructed Video URL: {currentVideoUrl}");

        photonView.RPC("RPC_LoadVideoFromUrl", RpcTarget.AllBuffered, currentVideoUrl, videoId, videoName);
    }

    [PunRPC]
    void RPC_LoadVideoFromUrl(string url, string videoId, string videoName)
    {
        currentVideoUrl = url;
        currentVideoName = videoName;
        StartCoroutine(SetupAndPlayVideo());
    }

    IEnumerator SetupAndPlayVideo()
    {
        Debug.Log("Starting Video Setup...");
        if (string.IsNullOrEmpty(currentVideoUrl))
        {
            Debug.LogError("Video URL is empty. Cannot load video.");
            UpdateDownloadUI(false); // Show failure message
            yield break;
        }

        if (photonView.IsMine)
        {
            // Start configuring the video player; instantiation happens later
            yield return StartCoroutine(ConfigureVideoPlayer(null, currentVideoUrl));
        }
    }

         private IEnumerator ConfigureVideoPlayer(GameObject targetObject, string videoUrl)
          {
              bool isMasterClient = (targetObject == null && photonView.IsMine);

              if (isMasterClient)
              {
                  targetObject = PhotonNetwork.Instantiate(prefabWithVideoPlayer.name, spawnPoint.position, Quaternion.identity);
                  currentVideoViewID = targetObject.GetComponent<PhotonView>().ViewID;
              }

              // Temporarily hide the renderer and canvas
              Renderer targetRenderer = targetObject.GetComponent<Renderer>();
              Canvas canvas = targetObject.GetComponentInChildren<Canvas>(); // Find the canvas in the prefab

              if (targetRenderer != null)
              {
                  targetRenderer.enabled = false; // Hide renderer initially
              }
             *//* if (canvas != null)
              {
                  canvas.enabled = false; // Hide canvas initially
              }*//*

              VideoPlayer videoPlayer = targetObject.GetComponent<VideoPlayer>();
              if (videoPlayer == null)
              {
                  videoPlayer = targetObject.GetComponentInChildren<VideoPlayer>();
              }

              if (videoPlayer != null)
              {
                  videoPlayer.url = videoUrl;
                  videoPlayer.isLooping = false;
                  videoPlayer.playOnAwake = false;
                  videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
                  videoPlayer.targetMaterialRenderer = targetRenderer;
                  videoPlayer.Prepare();

                  while (!videoPlayer.isPrepared)
                  {
                      Debug.Log($"Preparing video: {videoUrl}");
                      yield return null;
                  }

                  float aspectRatio = (float)videoPlayer.width / videoPlayer.height;
                  targetObject.transform.localScale = new Vector3(aspectRatio * 1f, 1f, 1f);

                  Debug.Log($"Video prepared. Width: {videoPlayer.width}, Height: {videoPlayer.height}, FrameRate: {videoPlayer.frameRate}");

                  VideoPlayerUIController uiController = targetObject.GetComponent<VideoPlayerUIController>();
                  if (uiController != null)
                  {
                      uiController.SetupSlider((float)videoPlayer.length);
                  }

                  if (isMasterClient)
                  {
                      photonView.RPC("SyncVideoData", RpcTarget.OthersBuffered, currentVideoViewID, videoUrl);
                  }

                  UpdateDownloadUI(true);

                  // Show renderer and canvas just before playing
                  if (targetRenderer != null)
                  {
                      targetRenderer.enabled = true; // Show renderer
                  }
                 *//* if (canvas != null)
                  {
                      canvas.enabled = true; // Show canvas
                  }*//*

                  videoPlayer.Play();
              }
              else
              {
                  Debug.LogError("No VideoPlayer component found on the target object.");
                  UpdateDownloadUI(false);
              }
          }
         



    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentVideoName} loaded successfully!"
                : $"Failed to load {currentVideoName}";

            // Only hide the UI if the video failed or after a delay for success
            if (!success)
            {
                StartCoroutine(HideDownloadUI());
            }
            else
            {
                // Delay hiding the UI briefly to show the success message
                StartCoroutine(HideDownloadUI(2f));
            }
        }

        if (loadingGif != null)
        {
            loadingGif.SetActive(false); // Hide GIF when preparation is done
        }
    }

    private IEnumerator HideDownloadUI(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void SyncVideoData(int viewID, string url)
    {
        currentVideoViewID = viewID;
        currentVideoUrl = url;
        StartCoroutine(DownloadAndApplyVideo(viewID, url));
    }

    private IEnumerator DownloadAndApplyVideo(int viewID, string url)
    {
        PhotonView videoView = PhotonView.Find(viewID);
        if (videoView == null)
        {
            Debug.LogError("Video with ViewID " + viewID + " not found.");
            yield break;
        }

        yield return StartCoroutine(ConfigureVideoPlayer(videoView.gameObject, url));

        PhotonTransformView photonTransformView = videoView.gameObject.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = videoView.gameObject.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;
    }

    [System.Serializable]
    public class videos
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class VideoListResponse
    {
        public videos[] videos;
    }
}
*/


using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Video;

public class VideoSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject loadingGif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Video Instantiation")]
    public GameObject prefabWithVideoPlayer; // Prefab with VideoPlayer and UI
    public Transform spawnPoint;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Video Download Variables
    private string currentVideoUrl = "";
    private int currentVideoViewID = -1;
    private string currentVideoName = "";
    private videos[] videoDataArray; // Store video data for lookup

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Video Loader Ready.");
    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchVideoNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

  /*  public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("VideoRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }*/

    public void FetchVideoNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetVideoNamesFromAPI());
    }

    private IEnumerator GetVideoNamesFromAPI()
    {
        string url = "https://medispherexr.com/api/src/api/video/view_videos.php";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching video names: {request.error}. URL: {url}");
                yield break;
            }

            string jsonResult = request.downloadHandler.text;
            VideoListResponse videoListResponse = JsonUtility.FromJson<VideoListResponse>(jsonResult);

            if (videoListResponse != null && videoListResponse.videos != null)
            {
                videoDataArray = videoListResponse.videos;

                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (videos video in videoListResponse.videos)
                {
                    if (video != null && !string.IsNullOrEmpty(video.name))
                    {
                        AddVideoNameToUI(video.id, video.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Video list response is null or empty.");
            }
        }
    }

    private void AddVideoNameToUI(string videoId, string videoName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = videoName;
                button.onClick.AddListener(() => OnVideoNameClicked(videoId, videoName));
            }
            else
            {
                Debug.LogError("The textPrefab does not have required components.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }
    public void LoadVideo(string videoId, string videoName)
    {
        OnVideoNameClicked(videoId, videoName);                                          //change
    }
    private void OnVideoNameClicked(string videoId, string videoName)
    {
        if (string.IsNullOrEmpty(videoId))
        {
            Debug.LogError("Video ID is empty. Cannot fetch video.");
            return;
        }

        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Loading {videoName}...";
        }
        if (loadingGif != null)
        {
            loadingGif.SetActive(true);
        }

        //  currentVideoUrl = "https://medispherexr.com/api/src/video/download_video.php?id=" + videoId;
        currentVideoUrl = "https://medispherexr.com/api/src/api/video/download_video.php?id=" + videoId;
        currentVideoName = videoName;

        Debug.Log($"Constructed Video URL: {currentVideoUrl}");

        photonView.RPC("RPC_LoadVideoFromUrl", RpcTarget.AllBuffered, currentVideoUrl, videoId, videoName);
    }

    [PunRPC]
    void RPC_LoadVideoFromUrl(string url, string videoId, string videoName)
    {
        currentVideoUrl = url;
        currentVideoName = videoName;
        StartCoroutine(SetupAndPlayVideo());
    }

    IEnumerator SetupAndPlayVideo()
    {
        Debug.Log("Starting Video Setup...");
        if (string.IsNullOrEmpty(currentVideoUrl))
        {
            Debug.LogError("Video URL is empty. Cannot load video.");
            UpdateDownloadUI(false); // Show failure message
            yield break;
        }

        if (photonView.IsMine)
        {
            // Start configuring the video player; instantiation happens later
            yield return StartCoroutine(ConfigureVideoPlayer(null, currentVideoUrl));
        }
    }

    private IEnumerator ConfigureVideoPlayer(GameObject targetObject, string videoUrl)
    {
        bool isMasterClient = (targetObject == null && photonView.IsMine);

        if (isMasterClient)
        {
            targetObject = PhotonNetwork.Instantiate(prefabWithVideoPlayer.name, spawnPoint.position, Quaternion.identity);
            currentVideoViewID = targetObject.GetComponent<PhotonView>().ViewID;
        }

        // Temporarily hide the renderer and canvas
        Renderer targetRenderer = targetObject.GetComponent<Renderer>();
        Canvas canvas = targetObject.GetComponentInChildren<Canvas>(); // Find the canvas in the prefab

        if (targetRenderer != null)
        {
            targetRenderer.enabled = false; // Hide renderer initially
        }
        /* if (canvas != null)
         {
             canvas.enabled = false; // Hide canvas initially
         }*/

        VideoPlayer videoPlayer = targetObject.GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = targetObject.GetComponentInChildren<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.url = videoUrl;
            videoPlayer.isLooping = false;
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = targetRenderer;
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                Debug.Log($"Preparing video: {videoUrl}");
                yield return null;
            }

            float aspectRatio = (float)videoPlayer.width / videoPlayer.height;
            targetObject.transform.localScale = new Vector3(aspectRatio * 1f, 1f, 1f);

            Debug.Log($"Video prepared. Width: {videoPlayer.width}, Height: {videoPlayer.height}, FrameRate: {videoPlayer.frameRate}");

            VideoPlayerUIController uiController = targetObject.GetComponent<VideoPlayerUIController>();
            if (uiController != null)
            {
                uiController.SetupSlider((float)videoPlayer.length);
            }

            if (isMasterClient)
            {
                photonView.RPC("SyncVideoData", RpcTarget.OthersBuffered, currentVideoViewID, videoUrl);
            }

            UpdateDownloadUI(true);

            // Show renderer and canvas just before playing
            if (targetRenderer != null)
            {
                targetRenderer.enabled = true; // Show renderer
            }
            /* if (canvas != null)
             {
                 canvas.enabled = true; // Show canvas
             }*/

            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("No VideoPlayer component found on the target object.");
            UpdateDownloadUI(false);
        }
    }




    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentVideoName} loaded successfully!"
                : $"Failed to load {currentVideoName}";

            // Only hide the UI if the video failed or after a delay for success
            if (!success)
            {
                StartCoroutine(HideDownloadUI());
            }
            else
            {
                // Delay hiding the UI briefly to show the success message
                StartCoroutine(HideDownloadUI(2f));
            }
        }

        if (loadingGif != null)
        {
            loadingGif.SetActive(false); // Hide GIF when preparation is done
        }
    }

    private IEnumerator HideDownloadUI(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void SyncVideoData(int viewID, string url)
    {
        currentVideoViewID = viewID;
        currentVideoUrl = url;
        StartCoroutine(DownloadAndApplyVideo(viewID, url));
    }

    private IEnumerator DownloadAndApplyVideo(int viewID, string url)
    {
        PhotonView videoView = PhotonView.Find(viewID);
        if (videoView == null)
        {
            Debug.LogError("Video with ViewID " + viewID + " not found.");
            yield break;
        }

        yield return StartCoroutine(ConfigureVideoPlayer(videoView.gameObject, url));

        PhotonTransformView photonTransformView = videoView.gameObject.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = videoView.gameObject.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;
    }

    [System.Serializable]
    public class videos
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class VideoListResponse
    {
        public videos[] videos;
    }
}


