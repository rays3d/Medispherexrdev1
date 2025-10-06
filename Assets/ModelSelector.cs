using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun; // Import Photon PUN namespace
using System.Collections;
using System.IO;
using Dummiesman;

public class ModelSelector : MonoBehaviourPunCallbacks
{
    public Button[] modelButtons; // Assign buttons in the Unity Inspector
    public string[] modelEndpoints; // URLs or API endpoints for the models
    public GameObject objPrefab; // Placeholder prefab for the model

    private string selectedModelUrl;
    private GameObject loadedModel;

    void Start()
    {
        // Assign a button click listener for each button
        for (int i = 0; i < modelButtons.Length; i++)
        {
            int index = i; // Local copy for the lambda expression
            modelButtons[i].onClick.AddListener(() => OnModelSelected(modelEndpoints[index]));
        }
    }

   public void OnModelSelected(string modelUrl)
    {
        selectedModelUrl = modelUrl;
        StartCoroutine(DownloadAndImportModel(selectedModelUrl));
    }

    IEnumerator DownloadAndImportModel(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + uwr.error);
                yield break;
            }

            byte[] modelData = uwr.downloadHandler.data;
            string localPath = SaveToLocalPath(modelData, "downloadedModel.obj");

            LoadAndSynchronizeModel(localPath);
        }
    }

    string SaveToLocalPath(byte[] data, string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, data);
        return path;
    }

    void LoadAndSynchronizeModel(string path)
    {
        if (loadedModel != null)
        {
            PhotonNetwork.Destroy(loadedModel); // Destroy the previous model across the network
        }

        // Load the OBJ model using the Runtime OBJ Importer
        OBJLoader loader = new OBJLoader();
        GameObject model = loader.Load(path);

        if (model != null)
        {
            loadedModel = PhotonNetwork.Instantiate(objPrefab.name, Vector3.zero, Quaternion.identity);
            loadedModel.transform.position = Vector3.zero;

            // Here you might need to replace the placeholder's mesh with the imported model's mesh
            MeshFilter meshFilter = loadedModel.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = model.GetComponent<MeshFilter>().mesh;
            }
        }
    }
}

/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;
using System.Collections;
using System.IO;
using Dummiesman;

public class ModelSelector : MonoBehaviourPunCallbacks
{
    public Button[] modelButtons; // Assign buttons in the Unity Inspector
    public string[] modelEndpoints; // URLs or API endpoints for the models
    public GameObject objPrefab; // Placeholder prefab for the model

    private string selectedModelUrl;
    private GameObject loadedModel;

    void Start()
    {
        // Assign a button click listener for each button
        for (int i = 0; i < modelButtons.Length; i++)
        {
            int index = i; // Local copy for the lambda expression
            modelButtons[i].onClick.AddListener(() => OnModelSelected(modelEndpoints[index]));
        }
    }

    public void OnModelSelected(string modelUrl)
    {
        selectedModelUrl = modelUrl;
        StartCoroutine(DownloadAndImportModel(selectedModelUrl));
    }

    IEnumerator DownloadAndImportModel(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + uwr.error);
                yield break;
            }

            byte[] modelData = uwr.downloadHandler.data;
            string localPath = SaveToLocalPath(modelData, "downloadedModel.obj");

            LoadAndSynchronizeModel(localPath);
        }
    }

    string SaveToLocalPath(byte[] data, string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, data);
        return path;
    }

    void LoadAndSynchronizeModel(string path)
    {
        if (loadedModel != null)
        {
            PhotonNetwork.Destroy(loadedModel); // Destroy the previous model across the network
        }

        // Load the OBJ model using the Runtime OBJ Importer
        StartCoroutine(LoadModelCoroutine(path));
    }

    IEnumerator LoadModelCoroutine(string path)
    {
        // Load the OBJ file from the path
        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            OBJLoader loader = new OBJLoader();
            GameObject model = loader.Load(fileStream, Vector3.zero, Quaternion.identity);

            yield return null; // Ensure we wait until the model is loaded

            if (model != null)
            {
                loadedModel = PhotonNetwork.Instantiate(objPrefab.name, Vector3.zero, Quaternion.identity);
                loadedModel.transform.position = Vector3.zero;

                // Here you might need to replace the placeholder's mesh with the imported model's mesh
                MeshFilter meshFilter = loadedModel.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Mesh modelMesh = model.GetComponent<MeshFilter>().mesh;
                    meshFilter.mesh = modelMesh;
                }
            }
            else
            {
                Debug.LogError("Failed to load the model from the OBJ file.");
            }
        }
    }
}
*/