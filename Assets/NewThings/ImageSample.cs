/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class ImageSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Prefab")]
    public GameObject prefabWithImage;
    public Transform spawnPoint;

    private PhotonView photonView;

    private string currentImageUrl = "";
    private string currentImageName = "";
    private int currentImageViewID = -1;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        fetchButton.onClick.AddListener(FetchImageNames);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ImageRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void FetchImageNames()
    {
        StartCoroutine(GetImageNamesFromAPI());
    }

    private IEnumerator GetImageNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage_new/src/models/view_images.php";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;
                ImageListResponse imageList = JsonUtility.FromJson<ImageListResponse>(jsonResult);

                foreach (Transform child in contentPanel.transform)
                    Destroy(child.gameObject);

                foreach (ImageData img in imageList.images)
                    AddImageNameToUI(img.id, img.name);
            }
            else
            {
                Debug.LogError("Failed to fetch image names: " + request.error);
            }
        }
    }

    private void AddImageNameToUI(string imageId, string imageName)
    {
        GameObject newItem = Instantiate(textPrefab, contentPanel.transform);
        TextMeshProUGUI text = newItem.GetComponent<TextMeshProUGUI>();
        Button btn = newItem.GetComponent<Button>();

        if (text != null && btn != null)
        {
            text.text = imageName;
            btn.onClick.AddListener(() => OnImageNameClicked(imageId, imageName));
        }
    }

    private void OnImageNameClicked(string imageId, string imageName)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {imageName}...";
        }
        if (gif != null) gif.SetActive(true);

        currentImageUrl = "http://192.168.1.26/storage_new/src/models/download_image.php?id=" + imageId;
        currentImageName = imageName;

        StartCoroutine(DownloadAndSpawnImage(currentImageUrl));

        // Tell others to do the same
        photonView.RPC("RPC_DownloadAndSpawnImage", RpcTarget.OthersBuffered, currentImageUrl);
    }

    [PunRPC]
    void RPC_DownloadAndSpawnImage(string imageUrl)
    {
        StartCoroutine(DownloadAndSpawnImage(imageUrl));
    }

    private IEnumerator DownloadAndSpawnImage(string imageUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Image Download Failed: " + request.error);
                UpdateDownloadUI(false);
                yield break;
            }

            Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(downloadedTexture, new Rect(0, 0, downloadedTexture.width, downloadedTexture.height), new Vector2(0.5f, 0.5f));

            if (photonView.IsMine)
            {
                GameObject imageInstance = PhotonNetwork.Instantiate(prefabWithImage.name, spawnPoint.position, Quaternion.identity);
                currentImageViewID = imageInstance.GetComponent<PhotonView>().ViewID;
                ApplySpriteToPrefab(imageInstance, sprite);

                photonView.RPC("RPC_ApplySpriteToPrefab", RpcTarget.OthersBuffered, currentImageViewID, imageUrl);
            }
            else
            {
                Debug.Log("Non-owner downloaded image.");
            }

            UpdateDownloadUI(true);
        }
    }

    [PunRPC]
    void RPC_ApplySpriteToPrefab(int viewID, string imageUrl)
    {
        StartCoroutine(DownloadAndAssignToView(imageUrl, viewID));
    }

    private IEnumerator DownloadAndAssignToView(string imageUrl, int viewID)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Texture re-download failed: " + request.error);
                yield break;
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            PhotonView view = PhotonView.Find(viewID);
            if (view != null)
            {
                ApplySpriteToPrefab(view.gameObject, sprite);
            }
        }
    }

    private void ApplySpriteToPrefab(GameObject target, Sprite sprite)
    {
        Image image = target.GetComponentInChildren<Image>();
        if (image != null)
        {
            image.sprite = sprite;
        }
        else
        {
            Debug.LogError("No Image component found in prefab.");
        }
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentImageName} downloaded!"
                : $"Failed to download {currentImageName}";
            StartCoroutine(HideDownloadUI());
        }

        if (gif != null)
        {
            gif.SetActive(false);
        }
    }

    private IEnumerator HideDownloadUI()
    {
        yield return new WaitForSeconds(2f);
        downloadingMessage?.gameObject.SetActive(false);
    }

    [System.Serializable]
    public class ImageData
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ImageListResponse
    {
        public ImageData[] images;
    }
}
*/
















///////////////////////////////////////////////////////////below is working properly/////////////////////////////////////

/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class ImageSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Image Instantiation")]
    public GameObject prefabWithImage;  // Your prefab with an empty Image component
    public Transform spawnPoint;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Image Download Variables
    private string currentImageUrl = "";
    private int currentImageViewID = -1;
    private string currentImageName = "";

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Image Loader Ready.");
    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchImageNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ImageRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void FetchImageNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetImageNamesFromAPI());
    }

    private IEnumerator GetImageNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage_new/src/models/view_images.php";  // Adjust this to your actual endpoint
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;
                ImageListResponse imageListResponse = JsonUtility.FromJson<ImageListResponse>(jsonResult);

                if (imageListResponse != null && imageListResponse.images != null)
                {
                    // Clear existing content
                    foreach (Transform child in contentPanel.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    foreach (ImageData image in imageListResponse.images)
                    {
                        if (image != null && !string.IsNullOrEmpty(image.image_name))
                        {
                            AddImageNameToUI(image.id, image.image_name);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Image list response is null or empty.");
                }
            }
            else
            {
                Debug.LogError("Error fetching image names: " + request.error);
            }
        }
    }

    private void AddImageNameToUI(string imageId, string imageName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = imageName;
                button.onClick.AddListener(() => OnImageNameClicked(imageId, imageName));
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

    private void OnImageNameClicked(string imageId, string imageName)
    {
        // Show downloading UI
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {imageName}...";
        }
        if (gif != null)
        {
            gif.SetActive(true);
        }

        // Construct the image URL
        currentImageUrl = "http://192.168.1.26/storage_new/src/models/download_image.php?id=" + imageId;
        currentImageName = imageName;

        // Start the download locally for the user initiating the action
        StartCoroutine(DownloadAndDisplayImage());

        // Synchronize the image download with all other users
        photonView.RPC("RPC_LoadImageFromUrl", RpcTarget.OthersBuffered, currentImageUrl);
    }

    [PunRPC]
    void RPC_LoadImageFromUrl(string url)
    {
        currentImageUrl = url;
        StartCoroutine(DownloadAndDisplayImage());
    }

    IEnumerator DownloadAndDisplayImage()
    {
        Debug.Log("Starting Image Download...");
        string uniqueImageUrl = currentImageUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uniqueImageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Image Download Failed: " + request.error);
                UpdateDownloadUI(false);
                yield break;
            }

            Debug.Log("Image Downloaded Successfully!");

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            // Create Sprite from the downloaded texture
            Sprite newSprite = CreateSpriteFromTexture(downloadedTexture);

            // Instantiate the prefab on the network only if it's the local player's action
             if (photonView.IsMine)
             {
                 // Create network instance of the prefab at the spawn point
                 GameObject imageInstance = PhotonNetwork.Instantiate(prefabWithImage.name, spawnPoint.position, Quaternion.identity);
                 currentImageViewID = imageInstance.GetComponent<PhotonView>().ViewID;

                 // Apply sprite to the prefab
                 ApplySpriteToImageComponent(imageInstance, newSprite);

                 // Sync the image with other clients
                 photonView.RPC("SyncImageData", RpcTarget.OthersBuffered, currentImageViewID, currentImageUrl);
             }
          



            UpdateDownloadUI(true);
        }
    }

    private Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    private void ApplySpriteToImageComponent(GameObject targetObject, Sprite sprite)
    {
        Image imageComponent = targetObject.GetComponent<Image>();
        if (imageComponent == null)
        {
            imageComponent = targetObject.GetComponentInChildren<Image>();
        }

        if (imageComponent != null)
        {
            imageComponent.sprite = sprite;
            // Preserve aspect ratio if desired
            imageComponent.preserveAspect = true;
        }
        else
        {
            Debug.LogError("No Image component found on the target object.");
        }
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentImageName} downloaded successfully!"
                : $"Failed to download {currentImageName}";

            // Hide UI after a short delay
            StartCoroutine(HideDownloadUI());
        }

        if (gif != null)
        {
            gif.SetActive(false);
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
    void SyncImageData(int viewID, string url)
    {
        currentImageViewID = viewID;
        currentImageUrl = url;
        StartCoroutine(DownloadAndApplyImage(viewID, url));
    }

    private IEnumerator DownloadAndApplyImage(int viewID, string url)
    {
        PhotonView imageView = PhotonView.Find(viewID);
        if (imageView == null)
        {
            Debug.LogError("Image with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download image: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite newSprite = CreateSpriteFromTexture(downloadedTexture);

            ApplySpriteToImageComponent(imageView.gameObject, newSprite);

            // Configure PhotonTransformView for synchronization
            PhotonTransformView photonTransformView = imageView.gameObject.GetComponent<PhotonTransformView>();
            if (photonTransformView == null)
            {
                photonTransformView = imageView.gameObject.AddComponent<PhotonTransformView>();
            }
            photonTransformView.m_SynchronizePosition = true;
            photonTransformView.m_SynchronizeRotation = true;
            photonTransformView.m_SynchronizeScale = true;
        }
    }

    [System.Serializable]
    public class ImageData
    {
        public string id;
        public string image_name;
    }

    [System.Serializable]
    public class ImageListResponse
    {
        public ImageData[] images;
    }
}*/











/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class ImageSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;
    // public TextMeshProUGUI descriptionText;  // Optional: for main UI description display

    [Header("Image Instantiation")]
    public GameObject prefabWithImage;  // Prefab with Image and TextMeshProUGUI for description
    public Transform spawnPoint;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Image Download Variables
    private string currentImageUrl = "";
    private int currentImageViewID = -1;
    private string currentImageName = "";
    private string currentDescription = "";
    private ImageData[] imageDataArray;  // Store image data for lookup

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Image Loader Ready.");
    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchImageNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ImageRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void FetchImageNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetImageNamesFromAPI());
    }

    private IEnumerator GetImageNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage_new/src/models/view_images.php";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching image names: {request.error}. URL: {url}");
                yield break;
            }

            string jsonResult = request.downloadHandler.text;
            ImageListResponse imageListResponse = JsonUtility.FromJson<ImageListResponse>(jsonResult);

            if (imageListResponse != null && imageListResponse.images != null)
            {
                imageDataArray = imageListResponse.images;

                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (ImageData image in imageListResponse.images)
                {
                    if (image != null && !string.IsNullOrEmpty(image.image_name))
                    {
                        AddImageNameToUI(image.id, image.image_name);
                    }
                }
            }
            else
            {
                Debug.LogError("Image list response is null or empty.");
            }
        }
    }

    private void AddImageNameToUI(string imageId, string imageName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = imageName;
                button.onClick.AddListener(() => OnImageNameClicked(imageId, imageName));
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

    private void OnImageNameClicked(string imageId, string imageName)
    {
        if (string.IsNullOrEmpty(imageId))
        {
            Debug.LogError("Image ID is empty. Cannot fetch image.");
            return;
        }

        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {imageName}...";
        }
        if (gif != null)
        {
            gif.SetActive(true);
        }

        currentImageUrl = "http://192.168.1.26/storage_new/src/models/download_image.php?id=" + imageId;
        currentImageName = imageName;

        currentDescription = "No description available";
        if (imageDataArray != null)
        {
            foreach (ImageData image in imageDataArray)
            {
                if (image.id == imageId)
                {
                    currentDescription = string.IsNullOrEmpty(image.description) ? "No description available" : image.description;
                    break;
                }
            }
        }

        Debug.Log($"Constructed Image URL: {currentImageUrl}");

        StartCoroutine(DownloadAndDisplayImage());
        photonView.RPC("RPC_LoadImageFromUrl", RpcTarget.OthersBuffered, currentImageUrl, imageId, imageName, currentDescription);
    }

    [PunRPC]
    void RPC_LoadImageFromUrl(string url, string imageId, string imageName, string description)
    {
        currentImageUrl = url;
        currentImageName = imageName;
        currentDescription = description;
        StartCoroutine(DownloadAndDisplayImage());
    }

    IEnumerator DownloadAndDisplayImage()
    {
        Debug.Log("Starting Image Download...");
        if (string.IsNullOrEmpty(currentImageUrl))
        {
            Debug.LogError("Image URL is empty. Cannot download image.");
            UpdateDownloadUI(false);
            yield break;
        }

        string uniqueImageUrl = currentImageUrl + "&timestamp=" + DateTime.Now.Ticks;
        Debug.Log("Downloading from URL: " + uniqueImageUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uniqueImageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Image Download Failed: {request.error}. URL: {uniqueImageUrl}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Text: {request.downloadHandler?.text}");
                UpdateDownloadUI(false);
                yield break;
            }

            Debug.Log("Image Downloaded Successfully!");

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite newSprite = CreateSpriteFromTexture(downloadedTexture);

            if (photonView.IsMine)
            {
                GameObject imageInstance = PhotonNetwork.Instantiate(prefabWithImage.name, spawnPoint.position, Quaternion.identity);
                currentImageViewID = imageInstance.GetComponent<PhotonView>().ViewID;

                ApplySpriteToImageComponent(imageInstance, newSprite, currentDescription);

                photonView.RPC("SyncImageData", RpcTarget.OthersBuffered, currentImageViewID, currentImageUrl, currentDescription);
            }

            UpdateDownloadUI(true);
        }
    }

    private Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    private void ApplySpriteToImageComponent(GameObject targetObject, Sprite sprite, string description = "")
    {
        Image imageComponent = targetObject.GetComponent<Image>();
        if (imageComponent == null)
        {
            imageComponent = targetObject.GetComponentInChildren<Image>();
        }

        if (imageComponent != null)
        {
            imageComponent.sprite = sprite;
            imageComponent.preserveAspect = true;
        }
        else
        {
            Debug.LogError("No Image component found on the target object.");
        }

        TextMeshProUGUI descComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();
        if (descComponent != null)
        {
            descComponent.text = string.IsNullOrEmpty(description) ? "No description available" : description;
        }
        else
        {
            Debug.LogError("No TextMeshProUGUI component found for description in the prefab.");
        }
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentImageName} downloaded successfully!"
                : $"Failed to download {currentImageName}";

            StartCoroutine(HideDownloadUI());
        }

        if (gif != null)
        {
            gif.SetActive(false);
        }

        // if (descriptionText != null)
        //{
        //     descriptionText.text = success ? currentDescription : "";
        // }
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
    void SyncImageData(int viewID, string url, string description)
    {
        currentImageViewID = viewID;
        currentImageUrl = url;
        currentDescription = description;
        StartCoroutine(DownloadAndApplyImage(viewID, url, description));
    }

    private IEnumerator DownloadAndApplyImage(int viewID, string url, string description)
    {
        PhotonView imageView = PhotonView.Find(viewID);
        if (imageView == null)
        {
            Debug.LogError("Image with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download image: {request.error}. URL: {url}");
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite newSprite = CreateSpriteFromTexture(downloadedTexture);

            ApplySpriteToImageComponent(imageView.gameObject, newSprite, description);

            PhotonTransformView photonTransformView = imageView.gameObject.GetComponent<PhotonTransformView>();
            if (photonTransformView == null)
            {
                photonTransformView = imageView.gameObject.AddComponent<PhotonTransformView>();
            }
            photonTransformView.m_SynchronizePosition = true;
            photonTransformView.m_SynchronizeRotation = true;
            photonTransformView.m_SynchronizeScale = true;
        }
    }

    [System.Serializable]
    public class ImageData
    {
        public string id;
        public string image_name;
        public string description;
    }

    [System.Serializable]
    public class ImageListResponse
    {
        public ImageData[] images;
    }
}*/





///////////////////description



using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class ImageSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;
    //public TextMeshProUGUI descriptionText;  // Optional: for main UI description display

    [Header("Image Instantiation")]
    public GameObject prefabWithImage;  // Prefab with Image and TextMeshProUGUI for description
    public Transform spawnPoint;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Image Download Variables
    private string currentImageUrl = "";
    private int currentImageViewID = -1;
    private string currentImageName = "";
    private string currentDescription = "";
    private ImageData[] imageDataArray;  // Store image data for lookup

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Image Loader Ready.");
    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchImageNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

  /*  public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ImageRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }*/

    public void FetchImageNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetImageNamesFromAPI());
    }

    private IEnumerator GetImageNamesFromAPI()
    {
        string url = "https://medispherexr.com/api/src/api/image/view_images.php";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching image names: {request.error}. URL: {url}");
                yield break;
            }

            string jsonResult = request.downloadHandler.text;
            ImageListResponse imageListResponse = JsonUtility.FromJson<ImageListResponse>(jsonResult);

            if (imageListResponse != null && imageListResponse.images != null)
            {
                imageDataArray = imageListResponse.images;

                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (ImageData image in imageListResponse.images)
                {
                    if (image != null && !string.IsNullOrEmpty(image.image_name))
                    {
                        AddImageNameToUI(image.id, image.image_name);
                    }
                }
            }
            else
            {
                Debug.LogError("Image list response is null or empty.");
            }
        }
    }

    private void AddImageNameToUI(string imageId, string imageName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = imageName;
                button.onClick.AddListener(() => OnImageNameClicked(imageId, imageName));
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
    /*  public void LoadImage(string imageId, string imageName, string description = "")
      {
          OnImageNameClicked(imageId, imageName, description);                                     //////////////new
      }
      private void OnImageNameClicked(string imageId, string imageName, string description = "")
      {
          if (string.IsNullOrEmpty(imageId))
          {
              Debug.LogError("Image ID is empty. Cannot fetch image.");
              return;
          }

          if (downloadingMessage != null)
          {
              downloadingMessage.gameObject.SetActive(true);
              downloadingMessage.text = $"Downloading {imageName}...";
          }
          if (gif != null)
          {
              gif.SetActive(true);
          }

          // currentImageUrl = "https://medispherexr.com/api/src/image/download_image.php?id=" + imageId;
          currentImageUrl = "https://medispherexr.com/api/src/api/image/download_image.php?id=" + imageId;
          currentImageName = imageName;

          currentDescription = "No description available";
          if (imageDataArray != null)
          {
              foreach (ImageData image in imageDataArray)
              {
                  if (image.id == imageId)
                  {
                      currentDescription = string.IsNullOrEmpty(image.description) ? "No description available" : image.description;
                      break;
                  }
              }
          }

          Debug.Log($"Constructed Image URL: {currentImageUrl}");

          StartCoroutine(DownloadAndDisplayImage());
          photonView.RPC("RPC_LoadImageFromUrl", RpcTarget.OthersBuffered, currentImageUrl, imageId, imageName, currentDescription);
      }*/

    public void LoadImage(string imageId, string imageName, string description = "") // Add description parameter
    {
        OnImageNameClicked(imageId, imageName, description); // Pass description
    }

    private void OnImageNameClicked(string imageId, string imageName, string description = "") // Add description parameter
    {
        if (string.IsNullOrEmpty(imageId))
        {
            Debug.LogError("Image ID is empty. Cannot fetch image.");
            return;
        }

        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {imageName}...";
        }
        if (gif != null)
        {
            gif.SetActive(true);
        }

        currentImageUrl = "https://medispherexr.com/api/src/api/image/download_image.php?id=" + imageId;
        currentImageName = imageName;
        currentDescription = string.IsNullOrEmpty(description) ? "No description available" : description; // Use passed description

        if (string.IsNullOrEmpty(description) && imageDataArray != null)
        {
            foreach (ImageData image in imageDataArray)
            {
                if (image.id == imageId)
                {
                    currentDescription = string.IsNullOrEmpty(image.description) ? "No description available" : image.description;
                    break;
                }
            }
        }

        Debug.Log($"Constructed Image URL: {currentImageUrl}, Description: {currentDescription}");
        StartCoroutine(DownloadAndDisplayImage());
        photonView.RPC("RPC_LoadImageFromUrl", RpcTarget.OthersBuffered, currentImageUrl, imageId, imageName, currentDescription);
    }


    [PunRPC]
    void RPC_LoadImageFromUrl(string url, string imageId, string imageName, string description)
    {
        currentImageUrl = url;
        currentImageName = imageName;
        currentDescription = description;
        StartCoroutine(DownloadAndDisplayImage());
    }

    IEnumerator DownloadAndDisplayImage()
    {
        Debug.Log("Starting Image Download...");
        if (string.IsNullOrEmpty(currentImageUrl))
        {
            Debug.LogError("Image URL is empty. Cannot download image.");
            UpdateDownloadUI(false);
            yield break;
        }

        string uniqueImageUrl = currentImageUrl + "&timestamp=" + DateTime.Now.Ticks;
        Debug.Log("Downloading from URL: " + uniqueImageUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uniqueImageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Image Download Failed: {request.error}. URL: {uniqueImageUrl}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Text: {request.downloadHandler?.text}");
                UpdateDownloadUI(false);
                yield break;
            }

            Debug.Log("Image Downloaded Successfully!");

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite newSprite = CreateSpriteFromTexture(downloadedTexture);

            if (photonView.IsMine)
            {
                GameObject imageInstance = PhotonNetwork.Instantiate(prefabWithImage.name, spawnPoint.position, Quaternion.identity);
                currentImageViewID = imageInstance.GetComponent<PhotonView>().ViewID;

                ApplySpriteToImageComponent(imageInstance, newSprite, currentDescription);

                photonView.RPC("SyncImageData", RpcTarget.OthersBuffered, currentImageViewID, currentImageUrl, currentDescription);
            }

            UpdateDownloadUI(true);
        }
    }

    private Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    /*   private void ApplySpriteToImageComponent(GameObject targetObject, Sprite sprite, string description = "")
       {
           Image imageComponent = targetObject.GetComponent<Image>();
           if (imageComponent == null)
           {
               imageComponent = targetObject.GetComponentInChildren<Image>();
           }

           if (imageComponent != null)
           {
               imageComponent.sprite = sprite;
               imageComponent.preserveAspect = true;
           }
           else
           {
               Debug.LogError("No Image component found on the target object.");
           }

           TextMeshProUGUI descComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();
           if (descComponent != null)
           {
               descComponent.text = string.IsNullOrEmpty(description) ? "No description available" : description;
           }
           else
           {
               Debug.LogError("No TextMeshProUGUI component found for description in the prefab.");
           }
       }*/

    private void ApplySpriteToImageComponent(GameObject targetObject, Sprite sprite, string description = "")
    {
        Image imageComponent = targetObject.GetComponent<Image>() ?? targetObject.GetComponentInChildren<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = sprite;
            imageComponent.preserveAspect = true;
            Debug.Log($"Applied sprite to {targetObject.name}");
        }
        else
        {
            Debug.LogError($"No Image component found on {targetObject.name} or its children.");
        }

        TextMeshProUGUI descComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();
        if (descComponent != null)
        {
            descComponent.text = string.IsNullOrEmpty(description) ? "No description available" : description;
            Debug.Log($"Set description to: {descComponent.text} on {targetObject.name}");
        }
        else
        {
            Debug.LogError($"No TextMeshProUGUI component found on {targetObject.name} or its children.");
        }
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentImageName} downloaded successfully!"
                : $"Failed to download {currentImageName}";

            StartCoroutine(HideDownloadUI());
        }

        if (gif != null)
        {
            gif.SetActive(false);
        }

        // if (descriptionText != null)
        //{
        //     descriptionText.text = success ? currentDescription : "";
        // }
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
    void SyncImageData(int viewID, string url, string description)
    {
        currentImageViewID = viewID;
        currentImageUrl = url;
        currentDescription = description;
        StartCoroutine(DownloadAndApplyImage(viewID, url, description));
    }

    private IEnumerator DownloadAndApplyImage(int viewID, string url, string description)
    {
        PhotonView imageView = PhotonView.Find(viewID);
        if (imageView == null)
        {
            Debug.LogError("Image with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download image: {request.error}. URL: {url}");
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite newSprite = CreateSpriteFromTexture(downloadedTexture);

            ApplySpriteToImageComponent(imageView.gameObject, newSprite, description);

            PhotonTransformView photonTransformView = imageView.gameObject.GetComponent<PhotonTransformView>();
            if (photonTransformView == null)
            {
                photonTransformView = imageView.gameObject.AddComponent<PhotonTransformView>();
            }
            photonTransformView.m_SynchronizePosition = true;
            photonTransformView.m_SynchronizeRotation = true;
            photonTransformView.m_SynchronizeScale = true;
        }
    }

    [System.Serializable]
    public class ImageData
    {
        public string id;
        public string image_name;
        public string description;
    }

    [System.Serializable]
    public class ImageListResponse
    {
        public ImageData[] images;
    }
}