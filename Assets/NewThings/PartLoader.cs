/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class PartLoader : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton; // Button to fetch part names
    public GameObject contentPanel; // Panel to display part names
    public GameObject textPrefab; // Prefab for part name buttons

    [Header("Prefab Instantiation")]
    public GameObject namePrefab; // Prefab to instantiate when a part name is clicked
    public Transform spawnPoint; // Where to spawn the prefab

    [Header("Network Settings")]
    private PhotonView photonView;

    // Data for parts
    private PartDatas[] partDataArray; // Store part data
    private string currentPartId;
    private string currentPartName;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Part Loader Ready.");

    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchPartNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("PartRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void FetchPartNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetPartNamesFromAPI());
    }

    private IEnumerator GetPartNamesFromAPI()
    {
        string url = "https://medispherexr.com/api/src/name/view_names.php";
        Debug.Log($"Sending request to: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API request failed: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Text: {request.downloadHandler?.text}");
                yield break;
            }

            string jsonResult = request.downloadHandler.text;
            Debug.Log($"Raw API response: {jsonResult}");

            try
            {
                PartListResponse partListResponse = JsonUtility.FromJson<PartListResponse>(jsonResult);

                if (partListResponse != null && partListResponse.PartData != null && partListResponse.PartData.Length > 0)
                {
                    partDataArray = partListResponse.PartData;

                    // Clear existing part names
                    foreach (Transform child in contentPanel.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    // Create UI buttons for each part name
                    foreach (PartDatas part in partListResponse.PartData)
                    {
                        if (part != null && !string.IsNullOrEmpty(part.part_name))
                        {
                            AddPartNameToUI(part.id, part.part_name);
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid part data: ID={part?.id}, Name={part?.part_name}");
                        }
                    }
                }
                else
                {
                    Debug.LogError("Part list response is null, empty, or has no parts.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"JSON parsing error: {e.Message}");
                Debug.LogError($"Raw JSON: {jsonResult}");
            }
        }
    }

    private void AddPartNameToUI(string partId, string partName)
    {
        if (textPrefab != null)
        {
            // Instantiate the button prefab as a child of the contentPanel
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = partName;
                button.onClick.AddListener(() => OnPartNameClicked(partId, partName));

                // Optional: Adjust the RectTransform to ensure proper sizing (if needed)
                RectTransform rectTransform = newText.GetComponent<RectTransform>();
                rectTransform.localScale = Vector3.one; // Ensure scale is correct
            }
            else
            {
                Debug.LogError("The textPrefab does not have required components (TextMeshProUGUI or Button).");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

      private void OnPartNameClicked(string partId, string partName)
        {
            if (string.IsNullOrEmpty(partId))
            {
                Debug.LogError("Part ID is empty. Cannot instantiate prefab.");
                return;
            }

            currentPartId = partId;
            currentPartName = partName;

            // Instantiate the prefab for the local player
            if (photonView.IsMine)
            {
                GameObject partInstance = PhotonNetwork.Instantiate(namePrefab.name, spawnPoint.position, Quaternion.identity);
                UpdatePrefab(partInstance);

                // Sync with other clients
                photonView.RPC("SyncPartInstance", RpcTarget.OthersBuffered, partInstance.GetComponent<PhotonView>().ViewID, partId, partName);
            }
        }

        [PunRPC]
        void SyncPartInstance(int viewID, string partId, string partName)
        {
            currentPartId = partId;
            currentPartName = partName;

            PhotonView partView = PhotonView.Find(viewID);
            if (partView == null)
            {
                Debug.LogError($"Part with ViewID {viewID} not found.");
                return;
            }

            UpdatePrefab(partView.gameObject);
        }
 *//*   private void OnPartNameClicked(string partId, string partName)
    {
        if (string.IsNullOrEmpty(partId))
        {
            Debug.LogError("Part ID is empty. Cannot instantiate prefab.");
            return;
        }

        // Send an RPC to all clients to instantiate and sync the prefab
        photonView.RPC("InstantiatePartPrefab", RpcTarget.All, partId, partName);
    }

    [PunRPC]
    void InstantiatePartPrefab(string partId, string partName)
    {
        Debug.Log($"Instantiating prefab for part: {partName} (ID: {partId}) on client {PhotonNetwork.NickName}");
        // Instantiate the prefab for all clients
        GameObject partInstance = PhotonNetwork.Instantiate(namePrefab.name, spawnPoint.position, Quaternion.identity);

        // Update the prefab with the part data
        currentPartId = partId;
        currentPartName = partName;
        UpdatePrefab(partInstance);
    }*//*

    private void UpdatePrefab(GameObject prefabInstance)
    {
        // Add PhotonTransformView if missing
        PhotonTransformView photonTransformView = prefabInstance.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = prefabInstance.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        // Assign the part name to a text component
        TextMeshProUGUI label = prefabInstance.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = currentPartName;
        }
        else
        {
            Debug.LogWarning("No TextMeshProUGUI found in namePrefab instance to set part name.");
        }

        // Attach to selected model
        var selectedModel = SelectionManager.Instance?.GetSelectedModel();
        if (selectedModel != null)
        {
            prefabInstance.transform.SetParent(selectedModel.transform);
            Debug.Log("Prefab attached to selected model.");
        }
        else
        {
            Debug.LogWarning("No model selected. Prefab will remain unparented.");
        }

        // Add Deletable component
        if (!prefabInstance.GetComponent<Deletable>())
        {
            prefabInstance.AddComponent<Deletable>();
        }

        // Add BoxCollider if not present
        if (!prefabInstance.GetComponent<Collider>())
        {
            BoxCollider collider = prefabInstance.AddComponent<BoxCollider>();
            collider.isTrigger = true;

            // Optional: Adjust size if needed
            // collider.size = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }


    [System.Serializable]
    public class PartDatas
    {
        public string id;
        public string part_name;
    }

    [System.Serializable]
    public class PartListResponse
    {
        public PartDatas[] PartData;
    }
}



*/


//////////////////////////////////////////////////////////////////////////////////down i am  currently using

/*sing UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class PartLoader : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton; // Button to fetch part names
    public GameObject contentPanel; // Panel to display part names
    public GameObject textPrefab; // Prefab for part name buttons

    [Header("Prefab Instantiation")]
    public GameObject namePrefab; // Prefab to instantiate when a part name is clicked
    public Transform spawnPoint; // Where to spawn the prefab

    [Header("Network Settings")]
    private PhotonView photonView;

    // Data for parts
    private PartDatas[] partDataArray; // Store part data
    private string currentPartId;
    private string currentPartName;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Part Loader Ready.");

    }

    void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchPartNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("PartRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void FetchPartNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetPartNamesFromAPI());
    }

    private IEnumerator GetPartNamesFromAPI()
    {
        string url = "https://medispherexr.com/api/src/test/view_label.php";
        Debug.Log($"Sending request to: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API request failed: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Text: {request.downloadHandler?.text}");
                yield break;
            }

            string jsonResult = request.downloadHandler.text;
            Debug.Log($"Raw API response: {jsonResult}");

            try
            {
                PartListResponse partListResponse = JsonUtility.FromJson<PartListResponse>(jsonResult);

                if (partListResponse != null && partListResponse.Label != null && partListResponse.Label.Length > 0)
                {
                    partDataArray = partListResponse.Label;

                    // Clear existing part names
                    foreach (Transform child in contentPanel.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    // Create UI buttons for each part name
                    foreach (PartDatas part in partListResponse.Label)
                    {
                        if (part != null && !string.IsNullOrEmpty(part.part_name))
                        {
                            AddPartNameToUI(part.id, part.part_name);
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid part data: ID={part?.id}, Name={part?.part_name}");
                        }
                    }
                }
                else
                {
                    Debug.LogError("Part list response is null, empty, or has no parts.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"JSON parsing error: {e.Message}");
                Debug.LogError($"Raw JSON: {jsonResult}");
            }
        }
    }

    private void AddPartNameToUI(string partId, string partName)
    {
        if (textPrefab != null)
        {
            // Instantiate the button prefab as a child of the contentPanel
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = partName;
                button.onClick.AddListener(() => OnPartNameClicked(partId, partName));

                // Optional: Adjust the RectTransform to ensure proper sizing (if needed)
                RectTransform rectTransform = newText.GetComponent<RectTransform>();
                rectTransform.localScale = Vector3.one; // Ensure scale is correct
            }
            else
            {
                Debug.LogError("The textPrefab does not have required components (TextMeshProUGUI or Button).");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    private void OnPartNameClicked(string partId, string partName)
    {
        if (string.IsNullOrEmpty(partId))
        {
            Debug.LogError("Part ID is empty. Cannot instantiate prefab.");
            return;
        }

        currentPartId = partId;
        currentPartName = partName;

        // Instantiate the prefab for the local player
        if (photonView.IsMine)
        {
            GameObject partInstance = PhotonNetwork.Instantiate(namePrefab.name, spawnPoint.position, Quaternion.identity);
            UpdatePrefab(partInstance);

            // Sync with other clients
            photonView.RPC("SyncPartInstance", RpcTarget.OthersBuffered, partInstance.GetComponent<PhotonView>().ViewID, partId, partName);
        }
    }

    [PunRPC]
    void SyncPartInstance(int viewID, string partId, string partName)
    {
        currentPartId = partId;
        currentPartName = partName;

        PhotonView partView = PhotonView.Find(viewID);
        if (partView == null)
        {
            Debug.LogError($"Part with ViewID {viewID} not found.");
            return;
        }

        UpdatePrefab(partView.gameObject);
    }


    private void UpdatePrefab(GameObject prefabInstance)
    {
        // Add PhotonTransformView if missing
        PhotonTransformView photonTransformView = prefabInstance.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = prefabInstance.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        // Assign the part name to a text component
        TextMeshProUGUI label = prefabInstance.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = currentPartName;
        }
        else
        {
            Debug.LogWarning("No TextMeshProUGUI found in namePrefab instance to set part name.");
        }

        // Attach to selected model
        var selectedModel = SelectionManager.Instance?.GetSelectedModel();
        if (selectedModel != null)
        {
            prefabInstance.transform.SetParent(selectedModel.transform);
            Debug.Log("Prefab attached to selected model.");
        }
        else
        {
            Debug.LogWarning("No model selected. Prefab will remain unparented.");
        }

        // Add Deletable component
        if (!prefabInstance.GetComponent<Deletable>())
        {
            prefabInstance.AddComponent<Deletable>();
        }

        // Add BoxCollider if not present
        if (!prefabInstance.GetComponent<Collider>())
        {
            BoxCollider collider = prefabInstance.AddComponent<BoxCollider>();
            collider.isTrigger = true;

            // Optional: Adjust size if needed
            // collider.size = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }


    [System.Serializable]
    public class PartDatas
    {
        public string id;
        public string part_name;
    }

    [System.Serializable]
    public class PartListResponse
    {
        public PartDatas[] Label;
    }
}
*/











/*

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class PartLoader : MonoBehaviourPunCallbacks
{
    public GameObject contentPanel;
    public GameObject folderscroll;
    public GameObject partContentPanel;
    public GameObject partscroll;
    public GameObject Back;
    public GameObject folderButtonPrefab;
    public GameObject partButtonPrefab;
    public GameObject namePrefab;
    public Transform spawnPoint;

    private string currentPartName;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        if (partContentPanel != null && partContentPanel.transform.parent != null)
        {
            partContentPanel.transform.parent.gameObject.SetActive(false);
        }

        LoadFolders();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("PartRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public void LoadFolders()
    {
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(GetFoldersFromAPI());
    }

    IEnumerator GetFoldersFromAPI()
    {
        string url = "https://medispherexr.com/api/src/api/label/list_labels.php";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Folder load error: {request.error}");
                yield break;
            }

            PartListResponse folderData = JsonUtility.FromJson<PartListResponse>(request.downloadHandler.text);

            foreach (var folder in folderData.PartData)
            {
                GameObject folderGO = Instantiate(folderButtonPrefab, contentPanel.transform);
                folderGO.GetComponentInChildren<TextMeshProUGUI>().text = folder.part_name;

                folderGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    LoadPartsInFolder(folder.id, folder.part_name);
                    folderscroll.SetActive(false);
                    partscroll.SetActive(true);
                    Back.SetActive(true);
                });
            }
        }
    }

    void LoadPartsInFolder(string folderId, string folderName)
    {
        partContentPanel.transform.parent.gameObject.SetActive(true);

        foreach (Transform child in partContentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(GetPartsFromAPI(folderId));
    }

    IEnumerator GetPartsFromAPI(string folderId)
    {
        string url = $"https://medispherexr.com/api/src/api/label/list_sub_labels.php?parent_id={folderId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Part list load failed: {request.error}");
                yield break;
            }

            PartListResponse partData = JsonUtility.FromJson<PartListResponse>(request.downloadHandler.text);

            foreach (var part in partData.PartData)
            {
                GameObject partGO = Instantiate(partButtonPrefab, partContentPanel.transform);
                partGO.GetComponentInChildren<TextMeshProUGUI>().text = part.part_name;

                partGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentPartName = part.part_name;
                    photonView.RPC("SpawnPartNetworked", RpcTarget.AllBuffered, currentPartName);
                });
            }
        }
    }

    [PunRPC]
    void SpawnPartNetworked(string partName)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (namePrefab == null || spawnPoint == null)
        {
            Debug.LogError("namePrefab or spawnPoint is missing");
            return;
        }

        GameObject go = PhotonNetwork.Instantiate(namePrefab.name, spawnPoint.position, Quaternion.identity);
        UpdatePrefab(go, partName);

        photonView.RPC("UpdatePartName", RpcTarget.All, go.GetComponent<PhotonView>().ViewID, partName);
    }
    [PunRPC]
    void UpdatePartName(int viewID, string partName)
    {
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView != null)
        {
            GameObject go = photonView.gameObject;
            UpdatePrefab(go, partName);
        }
    }
    private void UpdatePrefab(GameObject go, string partName)
    {
        // Set part name
        TextMeshProUGUI label = go.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = partName;
        }

        // Attach to selected model if available
        var selectedModel = SelectionManager.Instance?.GetSelectedModel();
        if (selectedModel != null && selectedModel.CompareTag("ModelPart"))
        {
            go.transform.SetParent(selectedModel.transform);
        }

        // Ensure components
        if (!go.GetComponent<Deletable>())
        {
            go.AddComponent<Deletable>();
        }

        if (!go.GetComponent<Collider>())
        {
            var collider = go.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        // Optional: Add PhotonTransformView for sync
        PhotonTransformView photonTransform = go.GetComponent<PhotonTransformView>();
        if (photonTransform == null)
        {
            photonTransform = go.AddComponent<PhotonTransformView>();
            photonTransform.m_SynchronizePosition = true;
            photonTransform.m_SynchronizeRotation = true;
            photonTransform.m_SynchronizeScale = true;
        }
    }

    [System.Serializable]
    public class PartDatas
    {
        public string id;
        public string parent_name;
        public string child_name;
        public string part_name => !string.IsNullOrEmpty(parent_name) ? parent_name : child_name;
    }

    [System.Serializable]
    public class PartListResponse
    {
        public PartDatas[] PartData;
    }
}
*/

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PartLoader : MonoBehaviourPunCallbacks
{
    public GameObject contentPanel;
    public GameObject folderscroll;
    public GameObject partContentPanel;
    public GameObject partscroll;
    public GameObject Back;
    public GameObject folderButtonPrefab;
    public GameObject partButtonPrefab;
    public GameObject namePrefab;
    public Transform spawnPoint;

    private string currentPartName;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        if (partContentPanel != null && partContentPanel.transform.parent != null)
        {
            partContentPanel.transform.parent.gameObject.SetActive(false);
        }

        LoadFolders();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("PartRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // For late joiners, update existing networked objects with correct part names
        StartCoroutine(UpdateExistingPartsForLateJoiner());
    }

    IEnumerator UpdateExistingPartsForLateJoiner()
    {
        yield return new WaitForSeconds(0.2f); // Small delay to ensure all objects are loaded

        // Get all room custom properties that store part information
        ExitGames.Client.Photon.Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

        foreach (var prop in roomProps)
        {
            string key = prop.Key.ToString();
            if (key.StartsWith("part_"))
            {
                // Extract ViewID from the key
                string viewIdStr = key.Replace("part_", "");
                if (int.TryParse(viewIdStr, out int viewId))
                {
                    PhotonView pv = PhotonView.Find(viewId);
                    if (pv != null)
                    {
                        string partName = prop.Value.ToString();
                        UpdatePrefab(pv.gameObject, partName);
                    }
                }
            }
        }
    }

    public void LoadFolders()
    {
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(GetFoldersFromAPI());
    }

    IEnumerator GetFoldersFromAPI()
    {
        string url = "https://medispherexr.com/api/src/api/label/list_labels.php";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Folder load error: {request.error}");
                yield break;
            }

            PartListResponse folderData = JsonUtility.FromJson<PartListResponse>(request.downloadHandler.text);

            foreach (var folder in folderData.PartData)
            {
                GameObject folderGO = Instantiate(folderButtonPrefab, contentPanel.transform);
                folderGO.GetComponentInChildren<TextMeshProUGUI>().text = folder.part_name;

                folderGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    LoadPartsInFolder(folder.id, folder.part_name);
                    folderscroll.SetActive(false);
                    partscroll.SetActive(true);
                    Back.SetActive(true);
                });
            }
        }
    }

    void LoadPartsInFolder(string folderId, string folderName)
    {
        partContentPanel.transform.parent.gameObject.SetActive(true);

        foreach (Transform child in partContentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(GetPartsFromAPI(folderId));
    }

    IEnumerator GetPartsFromAPI(string folderId)
    {
        string url = $"https://medispherexr.com/api/src/api/label/list_sub_labels.php?parent_id={folderId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Part list load failed: {request.error}");
                yield break;
            }

            PartListResponse partData = JsonUtility.FromJson<PartListResponse>(request.downloadHandler.text);

            foreach (var part in partData.PartData)
            {
                GameObject partGO = Instantiate(partButtonPrefab, partContentPanel.transform);
                partGO.GetComponentInChildren<TextMeshProUGUI>().text = part.part_name;

                partGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentPartName = part.part_name;
                    photonView.RPC("SpawnPartNetworked", RpcTarget.All, currentPartName);
                });
            }
        }
    }

    [PunRPC]
    void SpawnPartNetworked(string partName)
    {
        // Only MasterClient spawns the actual networked object
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (namePrefab == null || spawnPoint == null)
        {
            Debug.LogError("namePrefab or spawnPoint is missing");
            return;
        }

        GameObject go = PhotonNetwork.Instantiate(namePrefab.name, spawnPoint.position, Quaternion.identity);

        // Store the part name in room custom properties using ViewID as key
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props[$"part_{go.GetComponent<PhotonView>().ViewID}"] = partName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        UpdatePrefab(go, partName);

        // Also send RPC to immediately update for existing players
        photonView.RPC("UpdatePartName", RpcTarget.Others, go.GetComponent<PhotonView>().ViewID, partName);
    }

    [PunRPC]
    void UpdatePartName(int viewID, string partName)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView != null)
        {
            GameObject go = targetPhotonView.gameObject;
            UpdatePrefab(go, partName);
        }
    }

    private void UpdatePrefab(GameObject go, string partName)
    {
        // Set part name
        TextMeshProUGUI label = go.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = partName;
        }

        // Attach to selected model if available
        var selectedModel = SelectionManager.Instance?.GetSelectedModel();
        if (selectedModel != null && selectedModel.CompareTag("ModelPart"))
        {
            go.transform.SetParent(selectedModel.transform);
        }

        // Ensure components
        if (!go.GetComponent<Deletable>())
        {
            go.AddComponent<Deletable>();
        }

        if (!go.GetComponent<Collider>())
        {
            var collider = go.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        // Optional: Add PhotonTransformView for sync
        PhotonTransformView photonTransform = go.GetComponent<PhotonTransformView>();
        if (photonTransform == null)
        {
            photonTransform = go.AddComponent<PhotonTransformView>();
            photonTransform.m_SynchronizePosition = true;
            photonTransform.m_SynchronizeRotation = true;
            photonTransform.m_SynchronizeScale = true;
        }
    }

    // Clean up custom properties when objects are destroyed
    public void CleanupPartProperty(int viewId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props[$"part_{viewId}"] = null; // Setting to null removes the property
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    [System.Serializable]
    public class PartDatas
    {
        public string id;
        public string parent_name;
        public string child_name;
        public string part_name => !string.IsNullOrEmpty(parent_name) ? parent_name : child_name;
    }

    [System.Serializable]
    public class PartListResponse
    {
        public PartDatas[] PartData;
    }
}