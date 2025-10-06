/*using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

[System.Serializable]
public class Patient
{
    public int id;
    public string name;
    public string model_ids;
    public string image_ids;
    public string video_ids;
}

[System.Serializable]
public class PatientResponse
{
    public bool success;
    public string message;
    public Patient[] patients;
}

[System.Serializable]
public class files
{
    public int id;
    public string name;
    public string description;
    public string file_type;
    public int is_transparent;
}

[System.Serializable]
public class FileResponse
{
    public bool success;
    public string message;
    public files[] files;
}

public class PatientFileManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchPatientsButton;
    public GameObject patientContentPanel;   public GameObject patientContentPanel1; 
    public GameObject fileContentPanel; public GameObject fileContentPanel1;
    public GameObject Back;
    public GameObject folderButtonPrefab;
    public GameObject modelButtonPrefab;
    public GameObject imageButtonPrefab;
    public GameObject videoButtonPrefab;
    public GameObject loadingGif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Script References")]
    public ImageSample imageSample;
    public VideoSample videoSample;
    public ModelSample modelSample;

    private PhotonView photonView;
    private string patientApiUrl = "http://192.168.1.26/storage_new/src/patient/view_patient.php";
    private string fileApiUrl = "http://192.168.1.26/storage_new/src/patient/get_patient_files_list.php?patient_id=";

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (fileContentPanel != null && fileContentPanel.transform.parent != null)
        {
            fileContentPanel.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("fileContentPanel or its parent is not assigned!");
        }

        if (fetchPatientsButton != null)
        {
            fetchPatientsButton.onClick.AddListener(LoadPatients);
        }
        else
        {
            Debug.LogError("fetchPatientsButton is not assigned!");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("PatientRoom", new RoomOptions { MaxPlayers = 50 }, TypedLobby.Default);
    }

    public void LoadPatients()
    {
        if (patientContentPanel == null)
        {
            Debug.LogError("patientContentPanel is not assigned!");
            return;
        }
        foreach (Transform child in patientContentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(GetPatientsFromAPI());
    }

    IEnumerator GetPatientsFromAPI()
    {
        string url = patientApiUrl;
        Debug.Log($"Fetching patients from URL: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Patient load error: {request.error}");
                yield break;
            }

            string json = request.downloadHandler.text;
            Debug.Log($"Patient JSON: {json}");

            try
            {
                PatientResponse patientData = JsonUtility.FromJson<PatientResponse>(json);
                if (patientData == null || patientData.patients == null)
                {
                    Debug.LogError("Failed to parse patient JSON or patients is null");
                    yield break;
                }

                Debug.Log($"Found {patientData.patients.Length} patients");
                foreach (var patient in patientData.patients)
                {
                    if (folderButtonPrefab == null)
                    {
                        Debug.LogError("folderButtonPrefab is not assigned!");
                        continue;
                    }

                    GameObject patientGO = Instantiate(folderButtonPrefab, patientContentPanel.transform);
                    Debug.Log($"Instantiated patient: {patient.name} (ID: {patient.id})");

                    TextMeshProUGUI label = patientGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = patient.name;
                    }
                    else
                    {
                        Debug.LogError($"TextMeshProUGUI not found in patientGO for {patient.name}");
                    }

                    Button button = patientGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() =>
                        {
                            Debug.Log($"Patient clicked: {patient.name} (ID: {patient.id})");
                            LoadFilesInPatient(patient.id, patient.name);
                            patientContentPanel1.SetActive(false);
                            fileContentPanel1.SetActive(true);
                            Back.SetActive(true);
                        });
                    }
                    else
                    {
                        Debug.LogError($"Button component not found in patientGO for {patient.name}");
                    }
                }
                photonView.RPC("RPC_SyncPatientList", RpcTarget.OthersBuffered, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Patient JSON parsing error: {e.Message}");
            }
        }
    }

    [PunRPC]
    void RPC_SyncPatientList(string json)
    {
        try
        {
            PatientResponse patientData = JsonUtility.FromJson<PatientResponse>(json);
            if (patientData.success)
            {
                foreach (Transform child in patientContentPanel.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (var patient in patientData.patients)
                {
                    GameObject patientGO = Instantiate(folderButtonPrefab, patientContentPanel.transform);
                    TextMeshProUGUI label = patientGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = patient.name;
                    }
                    Button button = patientGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => LoadFilesInPatient(patient.id, patient.name));
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Sync patient list error: {e.Message}");
        }
    }

    void LoadFilesInPatient(int patientId, string patientName)
    {
        Debug.Log($"Loading files for patient: {patientName} (ID: {patientId})");

        if (fileContentPanel == null || fileContentPanel.transform.parent == null)
        {
            Debug.LogError("fileContentPanel or its parent is not assigned!");
            return;
        }
        fileContentPanel.transform.parent.gameObject.SetActive(true);
        Debug.Log("File scroll view set to active");

        foreach (Transform child in fileContentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (downloadingMessage != null && loadingGif != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Fetching files for {patientName}...";
            loadingGif.SetActive(true);
        }
        else
        {
            Debug.LogError("downloadingMessage or loadingGif is not assigned!");
        }

        StartCoroutine(GetFilesFromAPI(patientId, patientName));
    }

    IEnumerator GetFilesFromAPI(int patientId, string patientName)
    {
        string url = $"{fileApiUrl}{patientId}";
        Debug.Log($"Fetching files from URL: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"File list load failed for patient {patientName}: {request.error}");
                UpdateDownloadUI(false, $"Failed to load files for {patientName}");
                yield break;
            }

            string json = request.downloadHandler.text;
            Debug.Log($"File JSON: {json}");

            try
            {
                FileResponse fileData = JsonUtility.FromJson<FileResponse>(json);
                if (fileData == null || fileData.files == null)
                {
                    Debug.LogError("Failed to parse file JSON or files is null");
                    UpdateDownloadUI(false, $"Failed to load files for {patientName}");
                    yield break;
                }

                Debug.Log($"Found {fileData.files.Length} files for patient {patientName}");
                if (fileData.files.Length == 0)
                {
                    Debug.LogWarning($"No files found for patient {patientName}");
                    GameObject messageGO = Instantiate(modelButtonPrefab, fileContentPanel.transform);
                    TextMeshProUGUI label = messageGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = "No files found";
                    }
                    Button button = messageGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.interactable = false;
                    }
                    UpdateDownloadUI(true, $"No files found for {patientName}");
                    yield break;
                }

                foreach (var file in fileData.files)
                {
                    GameObject prefabToUse = null;
                    switch (file.file_type)
                    {
                        case "model":
                            prefabToUse = modelButtonPrefab;
                            break;
                        case "image":
                            prefabToUse = imageButtonPrefab;
                            break;
                        case "video":
                            prefabToUse = videoButtonPrefab;
                            break;
                        default:
                            Debug.LogWarning($"Unknown file type: {file.file_type}");
                            continue;
                    }

                    if (prefabToUse == null)
                    {
                        Debug.LogError($"Prefab for {file.file_type} is not assigned!");
                        continue;
                    }

                    GameObject fileGO = Instantiate(prefabToUse, fileContentPanel.transform);
                    Debug.Log($"Instantiated file: {file.name} (ID: {file.id}, Type: {file.file_type})");

                    TextMeshProUGUI label = fileGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = file.name;
                    }
                    else
                    {
                        Debug.LogError($"TextMeshProUGUI not found in fileGO for {file.name}");
                    }

                    Button button = fileGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() =>
                        {
                            Debug.Log($"File clicked: {file.name} (Type: {file.file_type})");
                            DownloadFile(file.id.ToString(), file.name, file.file_type, file.description, file.is_transparent);
                           
                        });
                    }
                    else
                    {
                        Debug.LogError($"Button component not found in fileGO for {file.name}");
                    }
                }
                photonView.RPC("RPC_SyncFileList", RpcTarget.OthersBuffered, patientId, json);
                UpdateDownloadUI(true, $"Files for {patientName} loaded");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"File JSON parsing error: {e.Message}");
                UpdateDownloadUI(false, $"Failed to load files for {patientName}");
            }
        }
    }

    [PunRPC]
    void RPC_SyncFileList(int patientId, string json)
    {
        try
        {
            FileResponse fileData = JsonUtility.FromJson<FileResponse>(json);
            if (fileData.success)
            {
                foreach (Transform child in fileContentPanel.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (var file in fileData.files)
                {
                    GameObject prefabToUse = null;
                    switch (file.file_type)
                    {
                        case "model":
                            prefabToUse = modelButtonPrefab;
                            break;
                        case "image":
                            prefabToUse = imageButtonPrefab;
                            break;
                        case "video":
                            prefabToUse = videoButtonPrefab;
                            break;
                    }

                    if (prefabToUse == null)
                    {
                        Debug.LogError($"Prefab for {file.file_type} is not assigned!");
                        continue;
                    }

                    GameObject fileGO = Instantiate(prefabToUse, fileContentPanel.transform);
                    TextMeshProUGUI label = fileGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = file.name;
                    }
                    Button button = fileGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => DownloadFile(file.id.ToString(), file.name, file.file_type, file.description, file.is_transparent));
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Sync file list error: {e.Message}");
        }
    }

    void DownloadFile(string fileId, string fileName, string fileType, string description, int isTransparent)
    {
        if (downloadingMessage == null || loadingGif == null)
        {
            Debug.LogError("downloadingMessage or loadingGif is not assigned!");
            return;
        }

        downloadingMessage.gameObject.SetActive(true);
        downloadingMessage.text = $"Downloading {fileName}...";
        loadingGif.SetActive(true);

        try
        {
            switch (fileType)
            {
                case "model":
                    if (modelSample != null)
                        modelSample.LoadModel(fileId, fileName, isTransparent == 1);
                    else
                        Debug.LogError("ModelSample script not assigned.");
                    break;
                case "image":
                    if (imageSample != null)
                        imageSample.LoadImage(fileId, fileName);
                    else
                        Debug.LogError("ImageSample script not assigned.");
                    break;
                case "video":
                    if (videoSample != null)
                        videoSample.LoadVideo(fileId, fileName);
                    else
                        Debug.LogError("VideoSample script not assigned.");
                    break;
                default:
                    Debug.LogError($"Unknown file type: {fileType}");
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Download error for {fileName}: {e.Message}");
            UpdateDownloadUI(false, $"Failed to download {fileName}");
        }
        UpdateDownloadUI(true, $"{fileName} downloading...");
    }

    void UpdateDownloadUI(bool success, string message)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = message;
        }
        if (loadingGif != null)
        {
            loadingGif.SetActive(false);
        }
        StartCoroutine(HideDownloadUI());
    }

    IEnumerator HideDownloadUI()
    {
        yield return new WaitForSeconds(2f);
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(false);
        }
    }
}

*/



using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

[System.Serializable]
public class Patient
{
    public int id;
    public string name;
    public string model_ids;
    public string image_ids;
    public string video_ids;
    public int category_id;
    public string category;
}

[System.Serializable]
public class PatientResponse
{
    public bool success;
    public string message;
    public Patient[] patients;
}

[System.Serializable]
public class FileData
{
    public int id;
    public string name;
    public string description;
    public string file_type;
    public int is_transparent;
    public int is_child;       ////////////////////////////////////////////////////////////
}

[System.Serializable]
public class FileResponse
{
    public bool success;
    public string message;
    public FileData[] files;
}

[System.Serializable]
public class Category
{
    public int id;
    public string category;
    public string created_at;
}

[System.Serializable]
public class CategoryResponse
{
    public bool success;
    public string message;
    public Category[] categories;
}

public class PatientFileManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchPatientsButton;
    public GameObject categoryContentPanel; // New panel for categories
    public GameObject categoryContentPanel1;
    public GameObject patientContentPanel;
    public GameObject patientContentPanel1;
    public GameObject fileContentPanel;
    public GameObject fileContentPanel1;
    public GameObject Back1;
    public GameObject Back2;
  
    public GameObject folderButtonPrefab; // For patients
    public GameObject categoryButtonPrefab; // For categories
    public GameObject modelButtonPrefab;
    public GameObject imageButtonPrefab;
    public GameObject videoButtonPrefab;
    public GameObject loadingGif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Script References")]
    public ImageSample imageSample;
    public VideoSample videoSample;
    public ModelSample modelSample;

    private PhotonView photonView;
       private string categoryApiUrl = "https://medispherexr.com/api/src/api/category/view_categories.php";
        private string patientApiUrl = "https://medispherexr.com/api/src/api/patient/view_patient.php";
        private string fileApiUrl = "https://medispherexr.com/api/src/api/patient/get_patient_files_list.php?patient_id=";

 /*   private string categoryApiUrl = "http://192.168.1.26/medisphere_api/src/api/category/view_categories.php";
    private string patientApiUrl = "http://192.168.1.26/medisphere_api/src/api/patient/view_patient.php";
    private string fileApiUrl = "http://192.168.1.26/medisphere_api/src/api/patient/get_patient_files_list.php?patient_id=";*/

    private const int maxRetries = 2;
    private int selectedCategoryId = -1; // Track selected category

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        // Initialize UI
        if (categoryContentPanel != null)
        {
            categoryContentPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("categoryContentPanel is not assigned!");
        }

        if (patientContentPanel != null && patientContentPanel1 != null)
        {
            patientContentPanel1.SetActive(false);
        }
        else
        {
            Debug.LogError("patientContentPanel or patientContentPanel1 is not assigned!");
        }

        if (fileContentPanel != null && fileContentPanel.transform.parent != null && fileContentPanel1 != null)
        {
            fileContentPanel.transform.parent.gameObject.SetActive(false);
            fileContentPanel1.SetActive(false);
        }
        else
        {
            Debug.LogError("fileContentPanel, fileContentPanel1, or its parent is not assigned!");
        }

     /*   if (Back != null)
        {
            Back.SetActive(false);
            Button backButton = Back.GetComponent<Button>();
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }
        else
        {
            Debug.LogError("Back button is not assigned!");
        }*/

        if (fetchPatientsButton != null)
        {
            fetchPatientsButton.onClick.AddListener(LoadCategories);
        }
        else
        {
            Debug.LogError("fetchPatientsButton is not assigned!");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("PatientRoom", new RoomOptions { MaxPlayers = 50 }, TypedLobby.Default);
    }

   public void LoadCategories()
    {
        if (categoryContentPanel == null)
        {
            Debug.LogError("categoryContentPanel is not assigned!");
            return;
        }
        foreach (Transform child in categoryContentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(GetCategoriesFromAPI());
    }

    IEnumerator GetCategoriesFromAPI()
    {
        string url = categoryApiUrl;
        Debug.Log($"Fetching categories from URL: {url}");

        int retryCount = 0;
        while (retryCount <= maxRetries)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMsg = $"Category load failed: {request.error}";
                    Debug.LogError($"{errorMsg}\nResponse: {request.downloadHandler?.text}\nURL: {url}");
                    if (retryCount < maxRetries && request.responseCode == 500)
                    {
                        retryCount++;
                        Debug.Log($"Retrying ({retryCount}/{maxRetries})...");
                        yield return new WaitForSeconds(1f);
                        continue;
                    }
                    UpdateDownloadUI(false, "Failed to load categories. Please try again.");
                    yield break;
                }

                string json = request.downloadHandler.text;
                Debug.Log($"Category JSON: {json}");

                try
                {
                    CategoryResponse categoryData = JsonUtility.FromJson<CategoryResponse>(json);
                    if (categoryData == null || categoryData.categories == null)
                    {
                        Debug.LogError($"Failed to parse category JSON or categories is null\nJSON: {json}");
                        UpdateDownloadUI(false, "Failed to load categories.");
                        yield break;
                    }

                    if (!categoryData.success)
                    {
                        Debug.LogError($"Category API returned success=false: {categoryData.message}");
                        UpdateDownloadUI(false, $"Error: {categoryData.message}");
                        yield break;
                    }

                    Debug.Log($"Found {categoryData.categories.Length} categories");
                    foreach (var category in categoryData.categories)
                    {
                        if (categoryButtonPrefab == null)
                        {
                            Debug.LogError("categoryButtonPrefab is not assigned!");
                            continue;
                        }

                        GameObject categoryGO = Instantiate(categoryButtonPrefab, categoryContentPanel.transform);
                        Debug.Log($"Instantiated category: {category.category} (ID: {category.id})");

                        TextMeshProUGUI label = categoryGO.GetComponentInChildren<TextMeshProUGUI>();
                        if (label != null)
                        {
                            label.text = category.category;
                        }
                        else
                        {
                            Debug.LogError($"TextMeshProUGUI not found in categoryGO for {category.category}");
                        }

                        Button button = categoryGO.GetComponent<Button>();
                        if (button != null)
                        {
                            button.onClick.AddListener(() =>
                            {
                                Debug.Log($"Category clicked: {category.category} (ID: {category.id})");
                                selectedCategoryId = category.id;
                                LoadPatients();
                                categoryContentPanel1.SetActive(false);
                                patientContentPanel1.SetActive(true);
                                Back1.SetActive(true);
                            });
                        }
                        else
                        {
                            Debug.LogError($"Button component not found in categoryGO for {category.category}");
                        }
                    }
                    photonView.RPC("RPC_SyncCategoryList", RpcTarget.OthersBuffered, json);
                    UpdateDownloadUI(true, "Categories loaded");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Category JSON parsing error: {e.Message}\nJSON: {json}");
                    UpdateDownloadUI(false, "Failed to load categories.");
                }
                yield break;
            }
        }
    }

    [PunRPC]
    void RPC_SyncCategoryList(string json)
    {
        try
        {
            CategoryResponse categoryData = JsonUtility.FromJson<CategoryResponse>(json);
            if (categoryData.success)
            {
                foreach (Transform child in categoryContentPanel.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (var category in categoryData.categories)
                {
                    GameObject categoryGO = Instantiate(categoryButtonPrefab, categoryContentPanel.transform);
                    TextMeshProUGUI label = categoryGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = category.category;
                    }
                    Button button = categoryGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() =>
                        {
                            selectedCategoryId = category.id;
                            LoadPatients();
                            categoryContentPanel1.SetActive(false);
                            patientContentPanel1.SetActive(true);
                            Back1.SetActive(true);
                        });
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Sync category list error: {e.Message}");
        }
    }

    public void LoadPatients()
    {
        if (patientContentPanel == null)
        {
            Debug.LogError("patientContentPanel is not assigned!");
            return;
        }
        foreach (Transform child in patientContentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(GetPatientsFromAPI());
    }

    IEnumerator GetPatientsFromAPI()
    {
        string url = patientApiUrl;
        Debug.Log($"Fetching patients from URL: {url}");

        int retryCount = 0;
        while (retryCount <= maxRetries)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMsg = $"Patient load error: {request.error}";
                    Debug.LogError($"{errorMsg}\nResponse: {request.downloadHandler?.text}\nURL: {url}");
                    if (retryCount < maxRetries && request.responseCode == 500)
                    {
                        retryCount++;
                        Debug.Log($"Retrying ({retryCount}/{maxRetries})...");
                        yield return new WaitForSeconds(1f);
                        continue;
                    }
                    UpdateDownloadUI(false, "Failed to load patients. Please try again.");
                    yield break;
                }

                string json = request.downloadHandler.text;
                Debug.Log($"Patient JSON: {json}");

                try
                {
                    PatientResponse patientData = JsonUtility.FromJson<PatientResponse>(json);
                    if (patientData == null || patientData.patients == null)
                    {
                        Debug.LogError($"Failed to parse patient JSON or patients is null\nJSON: {json}");
                        UpdateDownloadUI(false, "Failed to load patients.");
                        yield break;
                    }

                    if (!patientData.success)
                    {
                        Debug.LogError($"Patient API returned success=false: {patientData.message}");
                        UpdateDownloadUI(false, $"Error: {patientData.message}");
                        yield break;
                    }

                    Debug.Log($"Found {patientData.patients.Length} patients");
                    int patientCount = 0;
                    foreach (var patient in patientData.patients)
                    {
                        if (selectedCategoryId != -1 && patient.category_id != selectedCategoryId)
                        {
                            continue; // Skip patients not in selected category
                        }

                        if (folderButtonPrefab == null)
                        {
                            Debug.LogError("folderButtonPrefab is not assigned!");
                            continue;
                        }

                        GameObject patientGO = Instantiate(folderButtonPrefab, patientContentPanel.transform);
                        Debug.Log($"Instantiated patient: {patient.name} (ID: {patient.id}, Category ID: {patient.category_id})");

                        TextMeshProUGUI label = patientGO.GetComponentInChildren<TextMeshProUGUI>();
                        if (label != null)
                        {
                            label.text = patient.name;
                        }
                        else
                        {
                            Debug.LogError($"TextMeshProUGUI not found in patientGO for {patient.name}");
                        }

                        Button button = patientGO.GetComponent<Button>();
                        if (button != null)
                        {
                            button.onClick.AddListener(() =>
                            {
                                Debug.Log($"Patient clicked: {patient.name} (ID: {patient.id})");
                                LoadFilesInPatient(patient.id, patient.name);
                                patientContentPanel1.SetActive(false);
                                fileContentPanel1.SetActive(true);
                                Back2.SetActive(true);
                            });
                        }
                        else
                        {
                            Debug.LogError($"Button component not found in patientGO for {patient.name}");
                        }
                        patientCount++;
                    }
                    Debug.Log($"Displayed {patientCount} patients for category ID {selectedCategoryId}");
                    if (patientCount == 0)
                    {
                        GameObject messageGO = Instantiate(folderButtonPrefab, patientContentPanel.transform);
                        TextMeshProUGUI label = messageGO.GetComponentInChildren<TextMeshProUGUI>();
                        if (label != null)
                        {
                            label.text = "No patients found";
                        }
                        Button button = messageGO.GetComponent<Button>();
                        if (button != null)
                        {
                            button.interactable = false;
                        }
                        UpdateDownloadUI(true, "No patients found for this category");
                    }
                    photonView.RPC("RPC_SyncPatientList", RpcTarget.OthersBuffered, json, selectedCategoryId);
                    UpdateDownloadUI(true, "Patients loaded");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Patient JSON parsing error: {e.Message}\nJSON: {json}");
                    UpdateDownloadUI(false, "Failed to load patients.");
                }
                yield break;
            }
        }
    }

    [PunRPC]
    void RPC_SyncPatientList(string json, int categoryId)
    {
        try
        {
            PatientResponse patientData = JsonUtility.FromJson<PatientResponse>(json);
            if (patientData.success)
            {
                foreach (Transform child in patientContentPanel.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (var patient in patientData.patients)
                {
                    if (categoryId != -1 && patient.category_id != categoryId)
                    {
                        continue;
                    }
                    GameObject patientGO = Instantiate(folderButtonPrefab, patientContentPanel.transform);
                    TextMeshProUGUI label = patientGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = patient.name;
                    }
                    Button button = patientGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() =>
                        {
                            LoadFilesInPatient(patient.id, patient.name);
                            patientContentPanel1.SetActive(false);
                            fileContentPanel1.SetActive(true);
                            Back2.SetActive(true);
                        });
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Sync patient list error: {e.Message}");
        }
    }

    void LoadFilesInPatient(int patientId, string patientName)
    {
        Debug.Log($"Loading files for patient: {patientName} (ID: {patientId})");

        if (fileContentPanel == null || fileContentPanel.transform.parent == null)
        {
            Debug.LogError("fileContentPanel or its parent is not assigned!");
            return;
        }
        fileContentPanel.transform.parent.gameObject.SetActive(true);
        Debug.Log("File scroll view set to active");

        foreach (Transform child in fileContentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (downloadingMessage != null && loadingGif != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Fetching files for {patientName}...";
            loadingGif.SetActive(true);
        }
        else
        {
            Debug.LogError("downloadingMessage or loadingGif is not assigned!");
        }

        StartCoroutine(GetFilesFromAPI(patientId, patientName));
    }

    IEnumerator GetFilesFromAPI(int patientId, string patientName)
    {
        string url = $"{fileApiUrl}{patientId}";
        Debug.Log($"Fetching files from URL: {url}");

        int retryCount = 0;
        while (retryCount <= maxRetries)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMsg = $"File list load failed for patient {patientName}: {request.error}";
                    Debug.LogError($"{errorMsg}\nResponse: {request.downloadHandler?.text}\nURL: {url}");
                    if (retryCount < maxRetries && request.responseCode == 500)
                    {
                        retryCount++;
                        Debug.Log($"Retrying ({retryCount}/{maxRetries})...");
                        yield return new WaitForSeconds(1f);
                        continue;
                    }
                    UpdateDownloadUI(false, $"Failed to load files for {patientName}. Please try again.");
                    GameObject messageGO = Instantiate(modelButtonPrefab, fileContentPanel.transform);
                    TextMeshProUGUI label = messageGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = "Error loading files";
                    }
                    Button button = messageGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.interactable = false;
                    }
                    yield break;
                }

                string json = request.downloadHandler.text;
                Debug.Log($"File JSON: {json}");

                try
                {
                    FileResponse fileData = JsonUtility.FromJson<FileResponse>(json);
                    if (fileData == null || fileData.files == null)
                    {
                        Debug.LogError($"Failed to parse file JSON or files is null\nJSON: {json}");
                        UpdateDownloadUI(false, $"Failed to load files for {patientName}");
                        yield break;
                    }

                    if (!fileData.success)
                    {
                        Debug.LogError($"File API returned success=false: {fileData.message}");
                        UpdateDownloadUI(false, $"Error: {fileData.message}");
                        yield break;
                    }

                    Debug.Log($"Found {fileData.files.Length} files for patient {patientName}");
                    if (fileData.files.Length == 0)
                    {
                        Debug.LogWarning($"No files found for patient {patientName}");
                        GameObject messageGO = Instantiate(modelButtonPrefab, fileContentPanel.transform);
                        TextMeshProUGUI label = messageGO.GetComponentInChildren<TextMeshProUGUI>();
                        if (label != null)
                        {
                            label.text = "No files found";
                        }
                        Button button = messageGO.GetComponent<Button>();
                        if (button != null)
                        {
                            button.interactable = false;
                        }
                        UpdateDownloadUI(true, $"No files found for {patientName}");
                        yield break;
                    }

                    foreach (var file in fileData.files)
                    {
                        GameObject prefabToUse = null;
                        switch (file.file_type)
                        {
                            case "model":
                                prefabToUse = modelButtonPrefab;
                                break;
                            case "image":
                                prefabToUse = imageButtonPrefab;
                                break;
                            case "video":
                                prefabToUse = videoButtonPrefab;
                                break;
                            default:
                                Debug.LogWarning($"Unknown file type: {file.file_type}");
                                continue;
                        }

                        if (prefabToUse == null)
                        {
                            Debug.LogError($"Prefab for {file.file_type} is not assigned!");
                            continue;
                        }

                        GameObject fileGO = Instantiate(prefabToUse, fileContentPanel.transform);
                        Debug.Log($"Instantiated file: {file.name} (ID: {file.id}, Type: {file.file_type})");

                        TextMeshProUGUI label = fileGO.GetComponentInChildren<TextMeshProUGUI>();
                        if (label != null)
                        {
                            label.text = file.name;
                        }
                        else
                        {
                            Debug.LogError($"TextMeshProUGUI not found in fileGO for {file.name}");
                        }

                        Button button = fileGO.GetComponent<Button>();
                        if (button != null)
                        {
                            button.onClick.AddListener(() =>
                            {
                                Debug.Log($"File clicked: {file.name} (Type: {file.file_type})");
                                DownloadFile(file.id.ToString(), file.name, file.file_type, file.description, file.is_transparent , file.is_child);
                            });
                        }
                        else
                        {
                            Debug.LogError($"Button component not found in fileGO for {file.name}");
                        }
                    }
                    photonView.RPC("RPC_SyncFileList", RpcTarget.OthersBuffered, patientId, json);
                    UpdateDownloadUI(true, $"Files for {patientName} loaded");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"File JSON parsing error: {e.Message}\nJSON: {json}");
                    UpdateDownloadUI(false, $"Failed to load files for {patientName}");
                }
                yield break;
            }
        }
    }

    [PunRPC]
    void RPC_SyncFileList(int patientId, string json)
    {
        try
        {
            FileResponse fileData = JsonUtility.FromJson<FileResponse>(json);
            if (fileData.success)
            {
                foreach (Transform child in fileContentPanel.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (var file in fileData.files)
                {
                    GameObject prefabToUse = null;
                    switch (file.file_type)
                    {
                        case "model":
                            prefabToUse = modelButtonPrefab;
                            break;
                        case "image":
                            prefabToUse = imageButtonPrefab;
                            break;
                        case "video":
                            prefabToUse = videoButtonPrefab;
                            break;
                    }

                    if (prefabToUse == null)
                    {
                        Debug.LogError($"Prefab for {file.file_type} is not assigned!");
                        continue;
                    }

                    GameObject fileGO = Instantiate(prefabToUse, fileContentPanel.transform);
                    TextMeshProUGUI label = fileGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.text = file.name;
                    }
                    Button button = fileGO.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => DownloadFile(file.id.ToString(), file.name, file.file_type, file.description, file.is_transparent, file.is_child));
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Sync file list error: {e.Message}");
        }
    }
    /*
        void DownloadFile(string fileId, string fileName, string fileType, string description, int isTransparent)
        {
            if (downloadingMessage == null || loadingGif == null)
            {
                Debug.LogError("downloadingMessage or loadingGif is not assigned!");
                return;
            }

            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {fileName}...";
            loadingGif.SetActive(true);

            try
            {
                switch (fileType)
                {
                    case "model":
                        if (modelSample != null)
                            modelSample.LoadModel(fileId, fileName, isTransparent == 1);
                        else
                            Debug.LogError("ModelSample script not assigned.");
                        break;
                    case "image":
                        if (imageSample != null)
                            imageSample.LoadImage(fileId, fileName);
                        else
                            Debug.LogError("ImageSample script not assigned.");
                        break;
                    case "video":
                        if (videoSample != null)
                            videoSample.LoadVideo(fileId, fileName);
                        else
                            Debug.LogError("VideoSample script not assigned.");
                        break;
                    default:
                        Debug.LogError($"Unknown file type: {fileType}");
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Download error for {fileName}: {e.Message}");
                UpdateDownloadUI(false, $"Failed to download {fileName}");
            }
            UpdateDownloadUI(true, $"{fileName} downloading...");
        }*/

    void DownloadFile(string fileId, string fileName, string fileType, string description, int isTransparent, int isChild)
    {
        if (downloadingMessage == null || loadingGif == null)
        {
            Debug.LogError("downloadingMessage or loadingGif is not assigned!");
            return;
        }

        downloadingMessage.gameObject.SetActive(true);
        downloadingMessage.text = $"Downloading {fileName}...";
        loadingGif.SetActive(true);

        try
        {
            switch (fileType)
            {
                case "model":
                    if (modelSample != null)
                        modelSample.LoadModel(fileId, fileName, isTransparent == 1, isChild == 1);
                    else
                        Debug.LogError("ModelSample script not assigned.");
                    break;
                case "image":
                    if (imageSample != null)
                        imageSample.LoadImage(fileId, fileName, description); // Pass description
                    else
                        Debug.LogError("ImageSample script not assigned.");
                    break;
                case "video":
                    if (videoSample != null)
                        videoSample.LoadVideo(fileId, fileName);
                    else
                        Debug.LogError("VideoSample script not assigned.");
                    break;
                default:
                    Debug.LogError($"Unknown file type: {fileType}");
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Download error for {fileName}: {e.Message}");
            UpdateDownloadUI(false, $"Failed to download {fileName}");
        }
        UpdateDownloadUI(true, $"{fileName} downloading...");
    }
    void UpdateDownloadUI(bool success, string message)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = message;
        }
        if (loadingGif != null)
        {
            loadingGif.SetActive(false);
        }
        StartCoroutine(HideDownloadUI());
    }

    IEnumerator HideDownloadUI()
    {
        yield return new WaitForSeconds(2f);
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(false);
        }
    }

    void OnBackClicked()
    {
        Debug.Log("Back button clicked");
        if (fileContentPanel1.activeSelf)
        {
            fileContentPanel1.SetActive(false);
            fileContentPanel.transform.parent.gameObject.SetActive(false);
            patientContentPanel1.SetActive(true);
            Back2.SetActive(true);
        }
        else if (patientContentPanel1.activeSelf)
        {
            patientContentPanel1.SetActive(false);
            categoryContentPanel.SetActive(true);
            Back1.SetActive(false);
            selectedCategoryId = -1; // Reset category
        }
    }
}