/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;              // Reference to the button in the scene
    public GameObject contentPanel;         // Reference to the Content GameObject in the ScrollView
    public GameObject textPrefab;           // Reference to the TextMeshProUGUI prefab (template for each model name)

    private PhotonView photonView;           // Reference to PhotonView component

    private void Start()
    {
        // Initialize PhotonView
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not attached to the GameObject.");
        }

        // Connect to Photon and join or create a room
        PhotonNetwork.ConnectUsingSettings();

        // Assign the FetchModelNames function to the button's onClick event
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room successfully");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null)
        {
            Debug.LogError("contentPanel is not assigned.");
            return;
        }

        if (textPrefab == null)
        {
            Debug.LogError("textPrefab is not assigned.");
            return;
        }

        // Start the coroutine to fetch the model names
        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/3d_model_store/api/view_test.php";  // Replace with your API URL
        Debug.Log("Fetching data from URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Received JSON: " + jsonResult);

            // Parse the JSON into a list of model objects
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>("{\"models\":" + jsonResult + "}");

            if (modelListResponse != null && modelListResponse.models != null)
            {
                Debug.Log("Models count: " + modelListResponse.models.Length);

                // Clear previous content
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                // Log and add each model name to the UI
                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        Debug.Log("Model name: " + model.name);
                        photonView.RPC("AddModelNameToUI", RpcTarget.All, model.name);
                    }
                    else
                    {
                        Debug.LogError("Model is null or name is empty.");
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    // This method is called on all clients to update the UI with a new model name
    [PunRPC]
    private void AddModelNameToUI(string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            RectTransform rectTransform = newText.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                // Optionally set the position or size if needed
                rectTransform.anchoredPosition = Vector2.zero; // Position will be managed by layout group
                // Optionally, adjust the sizeDelta if needed (e.g., width, height)
            }

            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = modelName;
                Debug.Log("Model name added to UI: " + modelName);
            }
            else
            {
                Debug.LogError("The textPrefab does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    // Class to match the JSON structure of the API response
    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
        public ModelData data; // 'data' field is now a ModelData object
    }

    // Wrapper class for deserializing the array of models
    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models; // This should match the root element of the JSON
    }

    // Class for additional model data
    [System.Serializable]
    public class ModelData
    {
        public string color;
        public string capacity;
        public float? price; // Use nullable types for optional fields
        public string generation;
        public string strapColour;
        public string caseSize;
        public string description;
        public int? year;
        public string cpuModel;
        public string hardDiskSize;
        public float? screenSize;
    }
}*/

/*
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class ModelNamesScrollView : MonoBehaviour
{
    public Button fetchButton;              // Reference to the button in the scene
    public GameObject contentPanel;         // Reference to the Content GameObject in the ScrollView
    public GameObject textPrefab;           // Reference to the TextMeshProUGUI prefab (template for each model name)

    private void Start()
    {
        // Assign the FetchModelNames function to the button's onClick event
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }
    }

    public void FetchModelNames()
    {
        if (contentPanel == null)
        {
            Debug.LogError("contentPanel is not assigned.");
            return;
        }

        if (textPrefab == null)
        {
            Debug.LogError("textPrefab is not assigned.");
            return;
        }

        // Start the coroutine to fetch the model names
        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage/api/model_info_unity.php";  // Replace with your API URL
        Debug.Log("Fetching data from URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Received JSON: " + jsonResult);

            // Parse the JSON into a list of model objects
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                Debug.Log("Models count: " + modelListResponse.models.Length);

                // Clear previous content
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                // Log and add each model name to the UI
                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        Debug.Log("Model name: " + model.name);
                        AddModelNameToUI(model.name, model.file_path);
                    }
                    else
                    {
                        Debug.LogError("Model is null or name is empty.");
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelName, string modelFilePath)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = modelName;
                Debug.Log("Model name added to UI: " + modelName);

                // Add click event listener
                Button button = newText.GetComponent<Button>();
                if (button != null)
                {
                    // Attach the button click event
                    button.onClick.AddListener(() => OnModelNameClicked(modelFilePath));
                }
                else
                {
                    Debug.LogError("The textPrefab does not have a Button component.");
                }
            }
            else
            {
                Debug.LogError("The textPrefab does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    private void OnModelNameClicked(string modelFilePath)
    {
        Debug.Log("Loading model from: " + modelFilePath);
        StartCoroutine(LoadModelCoroutine(modelFilePath));
    }

    private IEnumerator LoadModelCoroutine(string modelFilePath)
    {
        // Convert file path to a URL format
        string url = "file:///" + modelFilePath.Replace("\\", "/"); // Handle local file path

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Model data received.");

            // Here you would use your model loader to process the .obj file
            // For example, use a third-party library or custom code to load .obj files
            // This is a placeholder:
            GameObject model = LoadModelFromObjData(request.downloadHandler.data); // Example function

            if (model != null)
            {
                model.transform.SetParent(contentPanel.transform);
                model.transform.localPosition = Vector3.zero; // Adjust position if necessary
                Debug.Log("Model loaded and displayed.");
            }
            else
            {
                Debug.LogError("Failed to load model.");
            }
        }
        else
        {
            Debug.LogError("Error loading model: " + request.error);
        }
    }

    // Placeholder function for loading model
    private GameObject LoadModelFromObjData(byte[] objData)
    {
        // Implement actual model loading logic here
        // This is a placeholder to illustrate
        GameObject model = new GameObject(); // Replace with actual model loading
        return model;
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
        public string file_path; // URL or path to the 3D model
        public ModelData data; // Optional additional data
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models; // This should match the root element of the JSON
    }

    [System.Serializable]
    public class ModelData
    {
        public string color;
        public string description;
    }
}
*/
/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using TriLibCore.Samples; // Ensure this namespace is correct based on your setup
using Photon.Pun;
using Photon.Realtime;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;              // Reference to the button in the scene
    public GameObject contentPanel;         // Reference to the Content GameObject in the ScrollView
    public GameObject textPrefab;           // Reference to the TextMeshProUGUI prefab (template for each model name)

    private PhotonView photonView;           // Reference to PhotonView component

    private void Start()
    {
        // Initialize PhotonView
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not attached to the GameObject.");
        }

        // Connect to Photon and join or create a room
        PhotonNetwork.ConnectUsingSettings();

        // Assign the FetchModelNames function to the button's onClick event
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room successfully");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null)
        {
            Debug.LogError("contentPanel is not assigned.");
            return;
        }

        if (textPrefab == null)
        {
            Debug.LogError("textPrefab is not assigned.");
            return;
        }

        // Start the coroutine to fetch the model names
        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage/api/model_info_unity.php";  // Replace with your API URL
        Debug.Log("Fetching data from URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Received JSON: " + jsonResult);

            // Parse the JSON into a list of model objects
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                Debug.Log("Models count: " + modelListResponse.models.Length);

                // Clear previous content
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                // Log and add each model name to the UI
                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        Debug.Log("Model name: " + model.name);
                        AddModelNameToUI(model.name, model.file_path);
                    }
                    else
                    {
                        Debug.LogError("Model is null or name is empty.");
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelName, string modelFilePath)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = modelName;
                Debug.Log("Model name added to UI: " + modelName);

                // Add click event listener
                Button button = newText.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnModelNameClicked(modelFilePath));
                }
                else
                {
                    Debug.LogError("The textPrefab does not have a Button component.");
                }
            }
            else
            {
                Debug.LogError("The textPrefab does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    private void OnModelNameClicked(string modelFilePath)
    {
        Debug.Log("Loading model from: " + modelFilePath);
        photonView.RPC("LoadModelFromUrl", RpcTarget.All, modelFilePath);
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelFilePath)
    {
        string url = "http://192.168.1.26/storage/models/" + modelFilePath; // Adjust URL format as needed
        LoadModelFromURLSample loadModelFromURLSample = gameObject.AddComponent<LoadModelFromURLSample>();
        loadModelFromURLSample.ModelURL = url;
        loadModelFromURLSample.LoadModel();
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
        public string file_path; // URL or path to the 3D model
        public ModelData data; // Optional additional data
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models; // This should match the root element of the JSON
    }

    [System.Serializable]
    public class ModelData
    {
        public string color;
        public string description;
    }
}*/


/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Dummiesman;
using System.IO;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;              // Reference to the button in the scene
    public GameObject contentPanel;         // Reference to the Content GameObject in the ScrollView
    public GameObject textPrefab;           // Reference to the TextMeshProUGUI prefab (template for each model name)

    private PhotonView photonView;           // Reference to PhotonView component

    private void Start()
    {
        // Initialize PhotonView
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not attached to the GameObject.");
        }

        // Connect to Photon and join or create a room
        PhotonNetwork.ConnectUsingSettings();

        // Assign the FetchModelNames function to the button's onClick event
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room successfully");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null)
        {
            Debug.LogError("contentPanel is not assigned.");
            return;
        }

        if (textPrefab == null)
        {
            Debug.LogError("textPrefab is not assigned.");
            return;
        }

        // Start the coroutine to fetch the model names
        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage/api/model_info_unity.php";  // Replace with your API URL
        Debug.Log("Fetching data from URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Received JSON: " + jsonResult);

            // Parse the JSON into a list of model objects
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                Debug.Log("Models count: " + modelListResponse.models.Length);

                // Clear previous content
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                // Log and add each model name to the UI
                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        Debug.Log("Model name: " + model.name);
                        AddModelNameToUI(model.id, model.name); // Pass model ID and name
                    }
                    else
                    {
                        Debug.LogError("Model is null or name is empty.");
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = modelName;
                Debug.Log("Model name added to UI: " + modelName);

                // Add click event listener
                Button button = newText.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnModelNameClicked(modelId));
                }
                else
                {
                    Debug.LogError("The textPrefab does not have a Button component.");
                }
            }
            else
            {
                Debug.LogError("The textPrefab does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    private void OnModelNameClicked(string modelId)
    {
        Debug.Log("Loading model with ID: " + modelId);
        photonView.RPC("LoadModelFromUrl", RpcTarget.All, modelId);
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId)
    {
        string url = "http://192.168.1.26/storage/api/download_model_unity.php?model_id=" + modelId; // URL with model ID parameter
        StartCoroutine(DownloadAndLoadOBJ(url));
    }

    private IEnumerator DownloadAndLoadOBJ(string url)
    {
        string tempFilePath = Path.Combine(Application.temporaryCachePath, "model.obj");
        Debug.Log("Downloading OBJ file from: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes(tempFilePath, request.downloadHandler.data);
            Debug.Log("OBJ file downloaded successfully.");

            // Load the OBJ file
            OBJLoader objLoader = new OBJLoader();
            GameObject model = objLoader.Load(tempFilePath);

            if (model != null)
            {
                model.transform.position = Vector3.zero; // Adjust the position as needed
                model.transform.rotation = Quaternion.identity; // Adjust the rotation as needed
                Debug.Log("Model loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to load the model from the OBJ file.");
            }
        }
        else
        {
            Debug.LogError("Error downloading the OBJ model: " + request.error);
        }
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
        public string file_path; // URL or path to the 3D model
        public ModelData data; // Optional additional data
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models; // This should match the root element of the JSON
    }

    [System.Serializable]
    public class ModelData
    {
        public string color;
        public string description;
    }
}

*/
/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using Dummiesman;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;              // Reference to the button in the scene
    public GameObject contentPanel;         // Reference to the Content GameObject in the ScrollView
    public GameObject textPrefab;           // Reference to the TextMeshProUGUI prefab (template for each model name)

    private PhotonView photonView;           // Reference to PhotonView component

    private void Start()
    {
        // Initialize PhotonView
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not attached to the GameObject.");
        }

        // Connect to Photon and join or create a room
        PhotonNetwork.ConnectUsingSettings();

        // Assign the FetchModelNames function to the button's onClick event
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room successfully");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null)
        {
            Debug.LogError("contentPanel is not assigned.");
            return;
        }

        if (textPrefab == null)
        {
            Debug.LogError("textPrefab is not assigned.");
            return;
        }

        // Start the coroutine to fetch the model names
        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage/api/model_info_unity.php";  // Replace with your API URL
        Debug.Log("Fetching data from URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Received JSON: " + jsonResult);

            // Parse the JSON into a list of model objects
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                Debug.Log("Models count: " + modelListResponse.models.Length);

                // Clear previous content
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                // Log and add each model name to the UI
                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        Debug.Log("Model name: " + model.name);
                        AddModelNameToUI(model.id, model.name); // Pass model ID and name
                    }
                    else
                    {
                        Debug.LogError("Model is null or name is empty.");
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = modelName;
                Debug.Log("Model name added to UI: " + modelName);

                // Add click event listener
                Button button = newText.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnModelNameClicked(modelId));
                }
                else
                {
                    Debug.LogError("The textPrefab does not have a Button component.");
                }
            }
            else
            {
                Debug.LogError("The textPrefab does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogError("textPrefab is not assigned.");
        }
    }

    private void OnModelNameClicked(string modelId)
    {
        Debug.Log("Loading model with ID: " + modelId);
        photonView.RPC("LoadModelFromUrl", RpcTarget.All, modelId);
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId)
    {
        string url = "http://192.168.1.26/storage/api/download_model_unity.php?model_id=" + modelId; // URL with model ID parameter
        StartCoroutine(DownloadAndLoadOBJ(url));
    }

    private IEnumerator DownloadAndLoadOBJ(string url)
    {
        string tempObjFilePath = Path.Combine(Application.temporaryCachePath, "model.obj");
        string tempMtlFilePath = Path.Combine(Application.temporaryCachePath, "model.mtl");
        Debug.Log("Downloading OBJ file from: " + url);

        using (UnityWebRequest objRequest = UnityWebRequest.Get(url))
        {
            yield return objRequest.SendWebRequest();

            if (objRequest.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(tempObjFilePath, objRequest.downloadHandler.data);
                Debug.Log("OBJ file downloaded successfully.");

                // Download the MTL file
                string mtlUrl = url.Replace(".obj", ".mtl");
                Debug.Log("Downloading MTL file from: " + mtlUrl);
                using (UnityWebRequest mtlRequest = UnityWebRequest.Get(mtlUrl))
                {
                    yield return mtlRequest.SendWebRequest();

                    if (mtlRequest.result == UnityWebRequest.Result.Success)
                    {
                        File.WriteAllBytes(tempMtlFilePath, mtlRequest.downloadHandler.data);
                        Debug.Log("MTL file downloaded successfully.");

                        // Load materials from the MTL file
                        MTLLoader mtlLoader = new MTLLoader();
                        mtlLoader.SearchPaths.Add(Path.GetDirectoryName(tempObjFilePath)); // Add the directory of the OBJ file to the search paths
                        Dictionary<string, Material> materials = mtlLoader.Load(tempMtlFilePath);

                        // Load the OBJ file with materials
                        OBJLoader objLoader = new OBJLoader();
                        GameObject model = objLoader.Load(tempObjFilePath, materials);

                        if (model != null)
                        {
                            model.transform.position = Vector3.zero; // Adjust the position as needed
                            model.transform.rotation = Quaternion.identity; // Adjust the rotation as needed
                            Debug.Log("Model loaded successfully.");
                        }
                        else
                        {
                            Debug.LogError("Failed to load the model from the OBJ file.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Error downloading the MTL file: " + mtlRequest.error);
                    }
                }
            }
            else
            {
                Debug.LogError("Error downloading the OBJ model: " + objRequest.error);
            }
        }
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
        public string file_path; // URL or path to the 3D model
        public ModelData data; // Optional additional data
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models; // This should match the root element of the JSON
    }

    [System.Serializable]
    public class ModelData
    {
        public string color;
        public string description;
    }
}

*/
/*using System.Collections;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;
using Dummiesman;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public string modelPrefabName;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not attached to this GameObject.");
            return;
        }

        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage/api/model_info_unity.php"; // Replace with your API URL
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        AddModelNameToUI(model.id, model.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId));
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

    private void OnModelNameClicked(string modelId)
    {
        if (photonView != null)
        {
            Debug.Log("Attempting to call LoadModelFromUrl RPC with modelId: " + modelId);
            photonView.RPC("LoadModelFromUrl", RpcTarget.All, modelId);
        }
        else
        {
            Debug.LogError("PhotonView is not initialized.");
        }
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId)
    {
        Debug.Log("RPC LoadModelFromUrl received with modelId: " + modelId);
        StartCoroutine(DownloadAndInstantiateModel(modelId));
    }

    private IEnumerator DownloadAndInstantiateModel(string modelId)
    {
        string url = "http://192.168.1.26/storage/api/download_model_unity.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);

                Debug.Log("OBJ file downloaded to: " + tempObjFilePath);

                // Use OBJLoader to load the model
                OBJLoader loader = new OBJLoader();
                GameObject model = loader.Load(tempObjFilePath);

                if (model != null)
                {
                    model.transform.position = Vector3.zero;
                    model.transform.rotation = Quaternion.identity;
                    model.transform.SetParent(transform);

                    Debug.Log("Model instantiated successfully.");
                }
                else
                {
                    Debug.LogError("Model failed to load.");
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
            }
        }
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models;
    }
}*/
//this is i am usimng ----------------------------------------------------------------------------------------------------------------------------------------------
/*using System.Collections;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;
using Dummiesman;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public string modelPrefabName;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not attached to this GameObject.");
            return;
        }

        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage/api/model_info_unity.php"; // Replace with your API URL
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        AddModelNameToUI(model.id, model.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId));
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

    private void OnModelNameClicked(string modelId)
    {
        if (photonView != null)
        {
            Debug.Log("Attempting to call LoadModelFromUrl RPC with modelId: " + modelId);
            photonView.RPC("LoadModelFromUrl", RpcTarget.All, modelId);
        }
        else
        {
            Debug.LogError("PhotonView is not initialized.");
        }
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId)
    {
        Debug.Log("RPC LoadModelFromUrl received with modelId: " + modelId);
        StartCoroutine(DownloadAndInstantiateModel(modelId));
    }

    private IEnumerator DownloadAndInstantiateModel(string modelId)
    {
        string url = "http://192.168.1.26/storage/api/download_model_unity.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);

                Debug.Log("OBJ file downloaded to: " + tempObjFilePath);

                // Use OBJLoader to load the model
                OBJLoader loader = new OBJLoader();
                GameObject model = loader.Load(tempObjFilePath);

                if (model != null)
                {
                    *//*// Set the model's position, rotation, and scale
                    model.transform.position = Vector3.zero;
                    model.transform.rotation = Quaternion.identity;*//*
                    Transform playerTransform = Camera.main.transform; // Assuming the main camera represents the player's position
                    Vector3 spawnPosition = playerTransform.position + playerTransform.forward * 1.5f; // 2 units in front of the player
                    model.transform.position = spawnPosition;

                    // Set the rotation to face the same direction as the player
                    model.transform.rotation = playerTransform.rotation;
                    // Set the desired scale (adjust the scale factor as needed)
                    float scaleFactor = 0.02f; // Reduce the size to 10% of the original size
                    model.transform.localScale = Vector3.one * scaleFactor;

                    model.transform.SetParent(transform);

                    Debug.Log("Model instantiated successfully with scale: " + scaleFactor);
                }
                else
                {
                    Debug.LogError("Model failed to load.");
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
            }
        }
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models;
    }
}


/*using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Dummiesman;
using Photon.Realtime;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;                   // Button to fetch models
    public GameObject contentPanel;              // Panel to display model names
    public GameObject textPrefab;                 // Prefab for UI text
    public string apiUrl = "http://192.168.1.26/storage/api/model_info_unity.php"; // Your API URL
    public string prefabName = "YourPrefabName";  // Name of the prefab in Resources

    private Dictionary<string, string> modelDownloadUrls = new Dictionary<string, string>();

    private void Start()
    {
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();      // Connect to Photon
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                // Clear previous model names
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                // Add model names to UI and prepare download URLs
                foreach (Model model in modelListResponse.models)
                {
                    Debug.Log($"Model ID: {model.id}, Name: {model.name}");
                    modelDownloadUrls[model.id] = $"http://192.168.1.26/storage/api/download_model_unity.php?model_id={model.id}"; // Prepare download URL
                    AddModelNameToUI(model.id, model.name);
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId));
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

    private void OnModelNameClicked(string modelId)
    {
        if (modelDownloadUrls.TryGetValue(modelId, out string downloadUrl))
        {
            StartCoroutine(DownloadAndInstantiateModel(downloadUrl, modelId));
        }
        else
        {
            Debug.LogError($"No download URL found for model ID: {modelId}");
        }
    }

    private IEnumerator DownloadAndInstantiateModel(string downloadUrl, string modelId)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(downloadUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);

                Debug.Log("OBJ file downloaded to: " + tempObjFilePath);

                // Use OBJLoader to load the model
                OBJLoader loader = new OBJLoader();
                GameObject downloadedModel = loader.Load(tempObjFilePath);

                if (downloadedModel != null)
                {
                    SetupModel(downloadedModel);
                }
                else
                {
                    Debug.LogError("Model failed to load.");
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
            }
        }
    }

    private void SetupModel(GameObject downloadedModel)
    {
        // Load the prefab that already has the desired components
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            // Instantiate the prefab
            GameObject instantiatedPrefab = Instantiate(prefab);

            // Find the downloaded model's MeshFilter
            MeshFilter downloadedMeshFilter = downloadedModel.GetComponent<MeshFilter>();
            if (downloadedMeshFilter != null)
            {
                // Set the downloaded model's transform as a child of the instantiated prefab
                downloadedModel.transform.SetParent(instantiatedPrefab.transform);
                downloadedModel.transform.localPosition = Vector3.zero; // Center it
                downloadedModel.transform.localRotation = Quaternion.identity; // Reset rotation

                // If needed, copy over the materials from the downloaded model
                MeshRenderer downloadedRenderer = downloadedModel.GetComponent<MeshRenderer>();
                if (downloadedRenderer != null)
                {
                    MeshRenderer prefabRenderer = instantiatedPrefab.GetComponent<MeshRenderer>();
                    if (prefabRenderer != null)
                    {
                        prefabRenderer.materials = downloadedRenderer.materials; // Use downloaded materials
                    }
                }

                // Adjust Rigidbody properties
                Rigidbody rb = instantiatedPrefab.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;  // Set isKinematic to true
                    rb.useGravity = false;   // Disable gravity
                }

                Debug.Log("Model instantiated successfully as a child.");
            }
            else
            {
                Debug.LogError("Downloaded model has no MeshFilter.");
            }

            // Destroy the downloaded model to avoid clutter if it’s not needed anymore
            Destroy(downloadedModel);
        }
        else
        {
            Debug.LogError("Prefab not found.");
        }
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models;
    }
}*/
/*using System.Collections;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Dummiesman;
using Photon.Realtime;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public string modelPrefabName; // Name of the prefab to instantiate
    public GameObject modelsParent; // Reference to the parent GameObject where models will be instantiated

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage/api/model_info_unity.php"; // Your API URL
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        AddModelNameToUI(model.id, model.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId));
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

    private void OnModelNameClicked(string modelId)
    {
        Debug.Log("Attempting to call LoadModelFromUrl RPC with modelId: " + modelId);
        photonView.RPC("LoadModelFromUrl", RpcTarget.All, modelId);
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId)
    {
        StartCoroutine(DownloadAndInstantiateModel(modelId));
    }

    private IEnumerator DownloadAndInstantiateModel(string modelId)
    {
        string url = "http://192.168.1.26/storage/api/download_model_unity.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);
                Debug.Log("OBJ file downloaded to: " + tempObjFilePath);

                // Load the model using OBJLoader
                OBJLoader loader = new OBJLoader();
                GameObject loadedModel = loader.Load(tempObjFilePath);
                LogHierarchy(loadedModel.transform);  // Log the hierarchy to see where the MeshFilter is
                LogModelComponents(loadedModel);  // Log the components of the loaded model

                if (loadedModel != null)
                {
                    // Instantiate the model prefab
                    GameObject modelInstance = PhotonNetwork.Instantiate(modelPrefabName, Vector3.zero, Quaternion.identity, 0);

                    // Assign the downloaded mesh to the instantiated prefab
                    AssignMeshToPrefab(loadedModel, modelInstance);

                    // Clean up the loaded model if it's no longer needed
                    Destroy(loadedModel);
                }
                else
                {
                    Debug.LogError("Failed to load the model from OBJ.");
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
            }
        }
    }

    private void AssignMeshToPrefab(GameObject sourceModel, GameObject targetModel)
    {
        // Search for MeshFilter in the source model and its children
        MeshFilter sourceMeshFilter = sourceModel.GetComponentInChildren<MeshFilter>();
        MeshRenderer sourceMeshRenderer = sourceModel.GetComponentInChildren<MeshRenderer>();
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        MeshRenderer targetMeshRenderer = targetModel.GetComponent<MeshRenderer>();

        if (sourceMeshFilter != null && targetMeshFilter != null)
        {
            // Copy the mesh from the source model to the target prefab
            targetMeshFilter.mesh = sourceMeshFilter.mesh;

            if (sourceMeshRenderer != null && targetMeshRenderer != null)
            {
                // Copy the materials from the source model to the target prefab
                targetMeshRenderer.materials = sourceMeshRenderer.materials;
            }
            else
            {
                Debug.LogError("Source model does not have a MeshRenderer.");
            }
        }
        else
        {
            Debug.LogError("Source model does not have a MeshFilter.");
        }

        // Clean up source model after copying, if necessary
        Destroy(sourceModel);
    }

    private void LogHierarchy(Transform parent, int level = 0)
    {
        string indent = new string(' ', level * 2);
        Debug.Log(indent + parent.name);
        foreach (Transform child in parent)
        {
            LogHierarchy(child, level + 1);
        }
    }

    private void LogModelComponents(GameObject model)
    {
        var meshFilters = model.GetComponentsInChildren<MeshFilter>();
        var meshRenderers = model.GetComponentsInChildren<MeshRenderer>();

        Debug.Log("Mesh Filters:");
        foreach (var mf in meshFilters)
        {
            Debug.Log("MeshFilter found in: " + mf.gameObject.name);
        }

        Debug.Log("Mesh Renderers:");
        foreach (var mr in meshRenderers)
        {
            Debug.Log("MeshRenderer found in: " + mr.gameObject.name);
        }
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models;
    }
}*/
/*using System.Collections;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Dummiesman;
using Photon.Realtime;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public string modelPrefabName; // Name of the prefab to instantiate
    public GameObject modelsParent; // Reference to the parent GameObject where models will be instantiated

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "http://192.168.1.26/storage/api/model_info_unity.php"; // Your API URL
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (Model model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        AddModelNameToUI(model.id, model.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId));
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

    private void OnModelNameClicked(string modelId)
    {
        Debug.Log("Attempting to call LoadModelFromUrl RPC with modelId: " + modelId);
        photonView.RPC("LoadModelFromUrl", RpcTarget.All, modelId);
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId)
    {
        StartCoroutine(DownloadAndInstantiateModel(modelId));
    }

    private IEnumerator DownloadAndInstantiateModel(string modelId)
    {
        string url = "http://192.168.1.26/storage/api/download_model_unity.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);
                Debug.Log("OBJ file downloaded to: " + tempObjFilePath);

                // Load the model using OBJLoader
                OBJLoader loader = new OBJLoader();
                GameObject loadedModel = loader.Load(tempObjFilePath);
                LogHierarchy(loadedModel.transform);  // Log the hierarchy to see where the MeshFilter is
                LogModelComponents(loadedModel);  // Log the components of the loaded model

                if (loadedModel != null)
                {
                    // Instantiate the model prefab with PhotonNetwork
                    GameObject modelInstance = PhotonNetwork.Instantiate(modelPrefabName, Vector3.zero, Quaternion.identity, 0);

                    // Assign the downloaded mesh to the instantiated prefab
                    AssignMeshToPrefab(loadedModel, modelInstance);

                    // Clean up the loaded model if it's no longer needed
                    Destroy(loadedModel);
                }
                else
                {
                    Debug.LogError("Failed to load the model from OBJ.");
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
            }
        }
    }

    private void AssignMeshToPrefab(GameObject sourceModel, GameObject targetModel)
    {
        // Search for MeshFilter in the source model and its children
        MeshFilter sourceMeshFilter = sourceModel.GetComponentInChildren<MeshFilter>();
        MeshRenderer sourceMeshRenderer = sourceModel.GetComponentInChildren<MeshRenderer>();
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        MeshRenderer targetMeshRenderer = targetModel.GetComponent<MeshRenderer>();

        if (sourceMeshFilter != null && targetMeshFilter != null)
        {
            // Copy the mesh from the source model to the target prefab
            targetMeshFilter.mesh = sourceMeshFilter.mesh;

            if (sourceMeshRenderer != null && targetMeshRenderer != null)
            {
                // Apply the "Cross-Section-Material" instead of the original materials
                Material crossSectionMaterial = Resources.Load<Material>("Cross-Section-Material");
                if (crossSectionMaterial != null)
                {
                    targetMeshRenderer.material = crossSectionMaterial;
                    Debug.Log("Cross-Section-Material applied to the model.");
                }
                else
                {
                    // If the "Cross-Section-Material" can't be found, use the source model's material
                    targetMeshRenderer.materials = sourceMeshRenderer.materials;
                    Debug.LogWarning("Cross-Section-Material not found. Using original materials.");
                }
            }
            else
            {
                Debug.LogError("Source model does not have a MeshRenderer.");
            }
        }
        else
        {
            Debug.LogError("Source model does not have a MeshFilter.");
        }

        // Clean up source model after copying, if necessary
        Destroy(sourceModel);
    }

    private void LogHierarchy(Transform parent, int level = 0)
    {
        string indent = new string(' ', level * 2);
        Debug.Log(indent + parent.name);
        foreach (Transform child in parent)
        {
            LogHierarchy(child, level + 1);
        }
    }

    private void LogModelComponents(GameObject model)
    {
        var meshFilters = model.GetComponentsInChildren<MeshFilter>();
        var meshRenderers = model.GetComponentsInChildren<MeshRenderer>();

        Debug.Log("Mesh Filters:");
        foreach (var mf in meshFilters)
        {
            Debug.Log("MeshFilter found in: " + mf.gameObject.name);
        }

        Debug.Log("Mesh Renderers:");
        foreach (var mr in meshRenderers)
        {
            Debug.Log("MeshRenderer found in: " + mr.gameObject.name);
        }
    }

    // New Function to Move Models
    public void MoveModel(GameObject modelInstance, Vector3 newPosition)
    {
        PhotonView modelPhotonView = modelInstance.GetComponent<PhotonView>();

        // Check if the local player is the owner of the object
        if (!modelPhotonView.IsMine)
        {
            // Request ownership if the player doesn't own the model
            modelPhotonView.RequestOwnership();
        }

        // Ensure ownership before moving the model
        if (modelPhotonView.IsMine)
        {
            photonView.RPC("RPCMoveModel", RpcTarget.AllBuffered, modelPhotonView.ViewID, newPosition);
        }
        else
        {
            Debug.LogWarning("Ownership transfer pending. Waiting for ownership.");
        }
    }

    [PunRPC]
    public void RPCMoveModel(int viewID, Vector3 newPosition)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView != null)
        {
            modelView.transform.position = newPosition;
        }
        else
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
        }
    }

    [System.Serializable]
    public class Model
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public Model[] models;
    }
}
*/
///////////////////////////////////////////////////////////////////////////////////////////////////////////////// i am using....................................................
/*using System.Collections;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Dummiesman;
using Photon.Realtime;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public string modelPrefabName;
    public GameObject modelsParent;
    public TextMeshProUGUI downloadingMessage;
    public float desiredModelSize = 0.5f; // Adjust this in the Inspector to control model size

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "https://medispherexr.com/api/apis/model_info_unity.php";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (ModelData model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        AddModelNameToUI(model.id, model.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId));
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

    private void OnModelNameClicked(string modelId)
    {
        Debug.Log("Attempting to call LoadModelFromUrl RPC with modelId: " + modelId);
        photonView.RPC("LoadModelFromUrl", RpcTarget.AllBuffered, modelId);
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId, PhotonMessageInfo info)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        StartCoroutine(DownloadAndInstantiateModel(modelId));
    }

    private IEnumerator DownloadAndInstantiateModel(string modelId)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = "Model is downloading...";
        }

        string url = "https://medispherexr.com/api/apis/download_model_unity.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);
                Debug.Log("OBJ file downloaded to: " + tempObjFilePath);

                OBJLoader loader = new OBJLoader();
                GameObject loadedModel = loader.Load(tempObjFilePath);

                if (loadedModel != null)
                {
                    Camera mainCamera = Camera.main;
                    if (mainCamera != null)
                    {
                        Vector3 fixedPosition = mainCamera.transform.position + mainCamera.transform.forward * 2f;

                        GameObject modelInstance = PhotonNetwork.Instantiate(modelPrefabName, fixedPosition, Quaternion.identity, 0);

                        AssignMeshToPrefab(loadedModel, modelInstance);

                        PhotonTransformView photonTransformView = modelInstance.GetComponent<PhotonTransformView>();
                        if (photonTransformView != null)
                        {
                            photonTransformView.m_SynchronizePosition = true;
                            photonTransformView.m_SynchronizeRotation = true;
                            photonTransformView.m_SynchronizeScale = true;
                        }

                        photonView.RPC("SyncMeshData", RpcTarget.AllBuffered, modelInstance.GetComponent<PhotonView>().ViewID, modelId);

                        Destroy(loadedModel);
                    }
                    else
                    {
                        Debug.LogError("Main Camera not found.");
                    }

                    if (downloadingMessage != null)
                    {
                        downloadingMessage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError("Failed to load the model from OBJ.");
                    if (downloadingMessage != null)
                    {
                        downloadingMessage.text = "Failed to load model.";
                    }
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
                if (downloadingMessage != null)
                {
                    downloadingMessage.text = "Error downloading model: " + request.error;
                }
            }
        }
    }

    [PunRPC]
    private void SyncMeshData(int viewID, string modelId)
    {
        StartCoroutine(DownloadAndApplyMesh(viewID, modelId));
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string modelId)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        string url = "https://medispherexr.com/api/apis/download_model_unity.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);

                OBJLoader loader = new OBJLoader();
                GameObject loadedModel = loader.Load(tempObjFilePath);

                if (loadedModel != null)
                {
                    AssignMeshToPrefab(loadedModel, modelView.gameObject);
                    Destroy(loadedModel);
                }
                else
                {
                    Debug.LogError("Failed to load the model from OBJ.");
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
            }
        }
    }

    private void AssignMeshToPrefab(GameObject sourceModel, GameObject targetModel)
    {
        MeshFilter sourceMeshFilter = sourceModel.GetComponentInChildren<MeshFilter>();
        MeshRenderer sourceMeshRenderer = sourceModel.GetComponentInChildren<MeshRenderer>();
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        MeshRenderer targetMeshRenderer = targetModel.GetComponent<MeshRenderer>();

        if (sourceMeshFilter != null && targetMeshFilter != null)
        {
            Mesh mesh = sourceMeshFilter.sharedMesh;

            // Calculate the bounding box of the mesh
            Bounds bounds = mesh.bounds;

            // Calculate the scale factor to fit the model in the desired cube size
            float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            float scaleFactor = desiredModelSize / maxDimension;

            // Create a new scaled mesh
            Mesh scaledMesh = Instantiate(mesh);
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] *= scaleFactor;
            }
            scaledMesh.vertices = vertices;
            scaledMesh.RecalculateBounds();

            // Assign the scaled mesh to the target
            targetMeshFilter.mesh = scaledMesh;

            // Set the position to a fixed point (e.g., slightly in front of the camera)
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 fixedPosition = mainCamera.transform.position + mainCamera.transform.forward * 1f;
                targetModel.transform.position = fixedPosition;
            }

            if (sourceMeshRenderer != null && targetMeshRenderer != null)
            {
                Material crossSectionMaterial = Resources.Load<Material>("Cross-Section-Material");
                if (crossSectionMaterial != null)
                {
                    targetMeshRenderer.material = crossSectionMaterial;
                    Debug.Log("Cross-Section-Material applied to the model.");
                }
                else
                {
                    targetMeshRenderer.materials = sourceMeshRenderer.materials;
                    Debug.LogWarning("Cross-Section-Material not found. Using original materials.");
                }
            }
            else
            {
                Debug.LogError("Source model does not have a MeshRenderer.");
            }

            // Adjust the collider to fit the new mesh
            AdjustColliderToMesh(targetModel, scaledMesh);
        }
        else
        {
            Debug.LogError("Source model does not have a MeshFilter.");
        }

        Destroy(sourceModel);
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        // Set the collider's center and size to match the mesh bounds
        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    public void MoveModel(GameObject modelInstance, Vector3 newPosition)
    {
        PhotonView modelPhotonView = modelInstance.GetComponent<PhotonView>();

        if (!modelPhotonView.IsMine)
        {
            modelPhotonView.RequestOwnership();
        }

        if (modelPhotonView.IsMine)
        {
            modelInstance.transform.position = newPosition;
            photonView.RPC("SyncModelPosition", RpcTarget.OthersBuffered, modelPhotonView.ViewID, newPosition);
        }
    }

    [PunRPC]
    private void SyncModelPosition(int viewID, Vector3 newPosition)
    {
        PhotonView modelPhotonView = PhotonView.Find(viewID);

        if (modelPhotonView != null && !modelPhotonView.IsMine)
        {
            modelPhotonView.transform.position = newPosition;
        }
    }

    [System.Serializable]
    public class ModelData
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public ModelData[] models;
    }
}*/
/*using System.Collections;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Dummiesman;
using Photon.Realtime;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public string modelPrefabName;
    public GameObject modelsParent;
    public TextMeshProUGUI downloadingMessage;
    public float desiredModelSize = 0.5f;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "https://medispherexr.com/api/apis/model_info_unity.php";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;
                ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

                if (modelListResponse != null && modelListResponse.models != null)
                {
                    // Clear existing content
                    foreach (Transform child in contentPanel.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    // Add new model names
                    foreach (ModelData model in modelListResponse.models)
                    {
                        if (model != null && !string.IsNullOrEmpty(model.name))
                        {
                            AddModelNameToUI(model.id, model.name);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Model list response is null or empty.");
                }
            }
            else
            {
                Debug.LogError("Error fetching model names: " + request.error);
            }
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId));
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

    private void OnModelNameClicked(string modelId)
    {
        Debug.Log("Attempting to call LoadModelFromUrl RPC with modelId: " + modelId);
        photonView.RPC("LoadModelFromUrl", RpcTarget.AllBuffered, modelId);
    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId, PhotonMessageInfo info)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        StartCoroutine(DownloadAndInstantiateModel(modelId));
    }

    private IEnumerator DownloadAndInstantiateModel(string modelId)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = "Model is downloading...";
        }

        string url = "https://medispherexr.com/api/apis/download_model_unity.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);
                Debug.Log("OBJ file downloaded to: " + tempObjFilePath);

                try
                {
                    OBJLoader loader = new OBJLoader();
                    GameObject loadedModel = loader.Load(tempObjFilePath);

                    if (loadedModel != null)
                    {
                        Camera mainCamera = Camera.main;
                        if (mainCamera != null)
                        {
                            Vector3 fixedPosition = mainCamera.transform.position + mainCamera.transform.forward * 2f;
                            GameObject modelInstance = PhotonNetwork.Instantiate(modelPrefabName, fixedPosition, Quaternion.identity, 0);

                            // Process and assign the mesh with mirroring fix
                            AssignMeshToPrefab(loadedModel, modelInstance);

                            // Setup Photon components
                            PhotonTransformView photonTransformView = modelInstance.GetComponent<PhotonTransformView>();
                            if (photonTransformView != null)
                            {
                                photonTransformView.m_SynchronizePosition = true;
                                photonTransformView.m_SynchronizeRotation = true;
                                photonTransformView.m_SynchronizeScale = true;
                            }

                            photonView.RPC("SyncMeshData", RpcTarget.AllBuffered, modelInstance.GetComponent<PhotonView>().ViewID, modelId);

                            // Cleanup
                            Destroy(loadedModel);

                            if (downloadingMessage != null)
                            {
                                downloadingMessage.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            Debug.LogError("Main Camera not found.");
                            if (downloadingMessage != null)
                            {
                                downloadingMessage.text = "Error: Camera not found.";
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to load the model from OBJ.");
                        if (downloadingMessage != null)
                        {
                            downloadingMessage.text = "Failed to load model.";
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error processing model: " + e.Message);
                    if (downloadingMessage != null)
                    {
                        downloadingMessage.text = "Error processing model.";
                    }
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
                if (downloadingMessage != null)
                {
                    downloadingMessage.text = "Error downloading model: " + request.error;
                }
            }
        }
    }

    [PunRPC]
    private void SyncMeshData(int viewID, string modelId)
    {
        StartCoroutine(DownloadAndApplyMesh(viewID, modelId));
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string modelId)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        string url = "https://medispherexr.com/api/apis/download_model_unity.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);

                try
                {
                    OBJLoader loader = new OBJLoader();
                    GameObject loadedModel = loader.Load(tempObjFilePath);

                    if (loadedModel != null)
                    {
                        AssignMeshToPrefab(loadedModel, modelView.gameObject);
                        Destroy(loadedModel);
                    }
                    else
                    {
                        Debug.LogError("Failed to load the model from OBJ during sync.");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error processing model during sync: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Error downloading the model during sync: " + request.error);
            }
        }
    }

    private void AssignMeshToPrefab(GameObject sourceModel, GameObject targetModel)
    {
        MeshFilter sourceMeshFilter = sourceModel.GetComponentInChildren<MeshFilter>();
        MeshRenderer sourceMeshRenderer = sourceModel.GetComponentInChildren<MeshRenderer>();
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        MeshRenderer targetMeshRenderer = targetModel.GetComponent<MeshRenderer>();

        if (sourceMeshFilter != null && targetMeshFilter != null)
        {
            Mesh originalMesh = sourceMeshFilter.sharedMesh;
            Bounds bounds = originalMesh.bounds;

            // Create a new mesh instance
            Mesh processedMesh = new Mesh();

            // Copy the original mesh data
            Vector3[] vertices = originalMesh.vertices;
            Vector3[] normals = originalMesh.normals;
            Vector2[] uvs = originalMesh.uv;
            int[] triangles = originalMesh.triangles;

            // Calculate scale factor
            float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            float scaleFactor = desiredModelSize / maxDimension;

            // Process vertices and normals to fix mirroring and apply scaling
            Vector3[] processedVertices = new Vector3[vertices.Length];
            Vector3[] processedNormals = new Vector3[normals.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                // Fix mirroring by negating X coordinate and apply scaling
                processedVertices[i] = new Vector3(
                    -vertices[i].x * scaleFactor,
                    vertices[i].y * scaleFactor,
                    vertices[i].z * scaleFactor
                );

                // Adjust normals accordingly
                if (i < normals.Length)
                {
                    processedNormals[i] = new Vector3(
                        -normals[i].x,
                        normals[i].y,
                        normals[i].z
                    );
                }
            }

            // Fix triangle winding order to maintain proper face orientation
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i + 1];
                triangles[i + 1] = triangles[i + 2];
                triangles[i + 2] = temp;
            }

            // Assign processed data to the new mesh
            processedMesh.vertices = processedVertices;
            processedMesh.normals = processedNormals;
            processedMesh.uv = uvs;
            processedMesh.triangles = triangles;

            // Recalculate mesh properties
            processedMesh.RecalculateBounds();
            processedMesh.RecalculateTangents();
            processedMesh.RecalculateNormals();

            // Assign the processed mesh to the target
            targetMeshFilter.mesh = processedMesh;

            // Handle materials
            if (sourceMeshRenderer != null && targetMeshRenderer != null)
            {
                Material crossSectionMaterial = Resources.Load<Material>("Cross-Section-Material");
                if (crossSectionMaterial != null)
                {
                    targetMeshRenderer.material = crossSectionMaterial;
                    Debug.Log("Cross-Section-Material applied successfully.");
                }
                else
                {
                    targetMeshRenderer.materials = sourceMeshRenderer.materials;
                    Debug.LogWarning("Cross-Section-Material not found, using original materials.");
                }
            }

            // Update collider
            AdjustColliderToMesh(targetModel, processedMesh);

            // Set initial position relative to camera
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 fixedPosition = mainCamera.transform.position + mainCamera.transform.forward * 2f;
                targetModel.transform.position = fixedPosition;
                targetModel.transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            Debug.LogError("Required mesh components not found on source or target model.");
        }
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        // Set the collider's center and size to match the mesh bounds
        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    public void MoveModel(GameObject modelInstance, Vector3 newPosition)
    {
        PhotonView modelPhotonView = modelInstance.GetComponent<PhotonView>();

        if (!modelPhotonView.IsMine)
        {
            modelPhotonView.RequestOwnership();
        }

        if (modelPhotonView.IsMine)
        {
            modelInstance.transform.position = newPosition;
            photonView.RPC("SyncModelPosition", RpcTarget.OthersBuffered, modelPhotonView.ViewID, newPosition);
        }
    }

    [PunRPC]
    private void SyncModelPosition(int viewID, Vector3 newPosition)
    {
        PhotonView modelPhotonView = PhotonView.Find(viewID);
        if (modelPhotonView != null && !modelPhotonView.IsMine)
        {
            modelPhotonView.transform.position = newPosition;
        }
    }

    [System.Serializable]
    public class ModelData
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public ModelData[] models;
    }
}*/
using System.Collections;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Dummiesman;
using Photon.Realtime;

public class ModelNamesScrollView : MonoBehaviourPunCallbacks
{
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public string modelPrefabName;
    public GameObject modelsParent;
    public GameObject ModelLabel;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;
    public float desiredModelSize = 0.5f; // Adjust this in the Inspector to control model size

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (fetchButton != null)
        {
            fetchButton.onClick.AddListener(FetchModelNames);
        }
        else
        {
            Debug.LogError("fetchButton is not assigned.");
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
    }

    public void FetchModelNames()
    {
        if (contentPanel == null || textPrefab == null)
        {
            Debug.LogError("contentPanel or textPrefab is not assigned.");
            return;
        }

        StartCoroutine(GetModelNamesFromAPI());
    }

    private IEnumerator GetModelNamesFromAPI()
    {
        string url = "https://medispherexr.com/api/src/models/view_models.php";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            ModelListResponse modelListResponse = JsonUtility.FromJson<ModelListResponse>(jsonResult);

            if (modelListResponse != null && modelListResponse.models != null)
            {
                foreach (Transform child in contentPanel.transform)
                {
                    Destroy(child.gameObject);
                }

                foreach (ModelData model in modelListResponse.models)
                {
                    if (model != null && !string.IsNullOrEmpty(model.name))
                    {
                        AddModelNameToUI(model.id, model.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Model list response is null or empty.");
            }
        }
        else
        {
            Debug.LogError("Error fetching model names: " + request.error);
        }
    }

    private void AddModelNameToUI(string modelId, string modelName)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId, modelName));
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

    private void OnModelNameClicked(string modelId, string modelName)

    {

        photonView.RPC("LoadModelFromUrl", RpcTarget.AllBuffered, modelId, modelName);

    }

    [PunRPC]
    private void LoadModelFromUrl(string modelId, string modelName)

    {

        if (!photonView.IsMine)

        {

            return;

        }

        StartCoroutine(DownloadAndInstantiateModel(modelId, modelName));

    }
    [PunRPC]

    private void AddModelLabelRPC(int viewID, string modelName)

    {

        PhotonView modelView = PhotonView.Find(viewID);

        if (modelView == null)

        {

            Debug.LogError($"Model with ViewID {viewID} not found.");

            return;

        }



        GameObject modelInstance = modelView.gameObject;



        // Load the prefab from Resources

        GameObject labelPrefab = Resources.Load<GameObject>("LabelPrefab");

        if (labelPrefab == null)

        {

            Debug.LogError("LabelPrefab not found in Resources.");

            return;

        }



        // Instantiate the label as a child of the model

        GameObject labelInstance = Instantiate(labelPrefab, modelInstance.transform);

        TextMeshProUGUI labelText = labelInstance.GetComponent<TextMeshProUGUI>();



        if (labelText != null)

        {

            // Set the text to the model's name

            labelText.text = modelName;

        }

        else

        {

            Debug.LogError("LabelPrefab does not have a TextMeshProUGUI component.");

        }



        // Position the label below the model

        Bounds modelBounds = modelInstance.GetComponentInChildren<Renderer>().bounds;

        Vector3 labelPosition = new Vector3(modelBounds.center.x, modelBounds.min.y - 0.1f, modelBounds.center.z);

        labelInstance.transform.position = labelPosition;



        // Optional: Scale the label appropriately

        labelInstance.transform.localScale = Vector3.one * 0.01f; // Adjust scale as needed

    }

    private IEnumerator DownloadAndInstantiateModel(string modelId, string modelName)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = "Model is downloading";
            gif.SetActive(true);
        }

        string url = "https://medispherexr.com/api/src/models/download_model.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);
                Debug.Log("OBJ file downloaded to: " + tempObjFilePath);

                OBJLoader loader = new OBJLoader();
                GameObject loadedModel = loader.Load(tempObjFilePath);

                if (loadedModel != null)
                {
                    Camera mainCamera = Camera.main;
                    if (mainCamera != null)
                    {
                        Vector3 fixedPosition = mainCamera.transform.position + mainCamera.transform.forward * 2f;

                        GameObject modelInstance = PhotonNetwork.Instantiate(modelPrefabName, fixedPosition, Quaternion.identity, 0);

                        AssignMeshToPrefab(loadedModel, modelInstance);

                        PhotonTransformView photonTransformView = modelInstance.GetComponent<PhotonTransformView>();
                        if (photonTransformView != null)
                        {
                            photonTransformView.m_SynchronizePosition = true;
                            photonTransformView.m_SynchronizeRotation = true;
                            photonTransformView.m_SynchronizeScale = true;
                        }
                        photonView.RPC("AddModelLabelRPC", RpcTarget.AllBuffered, modelInstance.GetComponent<PhotonView>().ViewID, modelName);
                        photonView.RPC("SyncMeshData", RpcTarget.AllBuffered, modelInstance.GetComponent<PhotonView>().ViewID, modelId);

                        Destroy(loadedModel);
                    }
                    else
                    {
                        Debug.LogError("Main Camera not found.");
                    }

                    if (downloadingMessage != null)
                    {
                        downloadingMessage.gameObject.SetActive(false);
                        gif.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError("Failed to load the model from OBJ.");
                    if (downloadingMessage != null)
                    {
                        downloadingMessage.text = "Failed to load model.";
                    }
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
                if (downloadingMessage != null)
                {
                    downloadingMessage.text = "Error downloading model: " + request.error;
                }
            }
        }
    }

    [PunRPC]
    private void SyncMeshData(int viewID, string modelId)
    {
        StartCoroutine(DownloadAndApplyMesh(viewID, modelId));
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string modelId)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        string url = "https://medispherexr.com/api/src/models/download_model.php?model_id=" + modelId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tempObjFilePath = Path.Combine(Application.temporaryCachePath, modelId + ".obj");
                File.WriteAllBytes(tempObjFilePath, request.downloadHandler.data);

                OBJLoader loader = new OBJLoader();
                GameObject loadedModel = loader.Load(tempObjFilePath);

                if (loadedModel != null)
                {
                    AssignMeshToPrefab(loadedModel, modelView.gameObject);
                    Destroy(loadedModel);
                }
                else
                {
                    Debug.LogError("Failed to load the model from OBJ.");
                }
            }
            else
            {
                Debug.LogError("Error downloading the model: " + request.error);
            }
        }
    }

    private void AssignMeshToPrefab(GameObject sourceModel, GameObject targetModel)
    {
        MeshFilter sourceMeshFilter = sourceModel.GetComponentInChildren<MeshFilter>();
        MeshRenderer sourceMeshRenderer = sourceModel.GetComponentInChildren<MeshRenderer>();
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        MeshRenderer targetMeshRenderer = targetModel.GetComponent<MeshRenderer>();

        if (sourceMeshFilter != null && targetMeshFilter != null)
        {
            Mesh mesh = sourceMeshFilter.sharedMesh;

            // Calculate the bounding box of the mesh
            Bounds bounds = mesh.bounds;

            // Calculate the scale factor to fit the model in the desired cube size
            float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            float scaleFactor = desiredModelSize / maxDimension;

            // Create a new scaled mesh and flip it
            Mesh scaledMesh = Instantiate(mesh);
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;

            // Flip the X coordinates of vertices and normals
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(-vertices[i].x * scaleFactor,
                                         vertices[i].y * scaleFactor,
                                         vertices[i].z * scaleFactor);
                normals[i] = new Vector3(-normals[i].x, normals[i].y, normals[i].z);
            }

            scaledMesh.vertices = vertices;
            scaledMesh.normals = normals;

            // Flip triangle order to maintain correct face orientation
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = temp;
            }
            scaledMesh.triangles = triangles;

            scaledMesh.RecalculateBounds();

            // Assign the scaled and flipped mesh to the target
            targetMeshFilter.mesh = scaledMesh;

            // Set the position to a fixed point
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 fixedPosition = mainCamera.transform.position + mainCamera.transform.forward * 1f;
                targetModel.transform.position = fixedPosition;
            }

            if (sourceMeshRenderer != null && targetMeshRenderer != null)
            {
                Material crossSectionMaterial = Resources.Load<Material>("Cross-Section-Material");
                if (crossSectionMaterial != null)
                {
                    targetMeshRenderer.material = crossSectionMaterial;
                    Debug.Log("Cross-Section-Material applied to the model.");
                }
                else
                {
                    targetMeshRenderer.materials = sourceMeshRenderer.materials;
                    Debug.LogWarning("Cross-Section-Material not found. Using original materials.");
                }
            }
            else
            {
                Debug.LogError("Source model does not have a MeshRenderer.");
            }

            // Adjust the collider to fit the new mesh
            AdjustColliderToMesh(targetModel, scaledMesh);
        }
        else
        {
            Debug.LogError("Source model does not have a MeshFilter.");
        }

        Destroy(sourceModel);
    }
    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        // Set the collider's center and size to match the mesh bounds
        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    public void MoveModel(GameObject modelInstance, Vector3 newPosition)
    {
        PhotonView modelPhotonView = modelInstance.GetComponent<PhotonView>();

        if (!modelPhotonView.IsMine)
        {
            modelPhotonView.RequestOwnership();
        }

        if (modelPhotonView.IsMine)
        {
            modelInstance.transform.position = newPosition;
            photonView.RPC("SyncModelPosition", RpcTarget.OthersBuffered, modelPhotonView.ViewID, newPosition);
        }
    }

    [PunRPC]
    private void SyncModelPosition(int viewID, Vector3 newPosition)
    {
        PhotonView modelPhotonView = PhotonView.Find(viewID);

        if (modelPhotonView != null && !modelPhotonView.IsMine)
        {
            modelPhotonView.transform.position = newPosition;
        }
    }

    [System.Serializable]
    public class ModelData
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public ModelData[] models;
    }
}
