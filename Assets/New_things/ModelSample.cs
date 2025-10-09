/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using TriLibCore;
using TriLibCore.General;

public class ModelSample : MonoBehaviour
{
    public string modelUrl = "http://192.168.1.26/3d_models/apis/download_model_unity.php?model_id=105";
    public string textureUrl = "http://yourserver.com/yourimage.png"; // Update with actual texture URL
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;

    private string modelFilePath;
    private string fileExtension = "";
    private GameObject instantiatedPrefab;
    private Material targetMaterial;

    void Awake()
    {
        Debug.Log("Model Loader Ready.");
    }

    public void DownloadAndLoadModelOnClick()
    {
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = modelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
        }

        yield return new WaitForSeconds(1);
        LoadModel();
    }

    void LoadModel()
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            return;
        }

        // Instantiate the prefab
        instantiatedPrefab = Instantiate(prefabWithMeshFilter, spawnPoint.position, Quaternion.identity);
        if (instantiatedPrefab == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            Destroy(loadedModel);
            return;
        }

        // Assign the downloaded mesh to the prefab's MeshFilter
        MeshFilter prefabMeshFilter = instantiatedPrefab.GetComponent<MeshFilter>();
        if (prefabMeshFilter == null)
        {
            Debug.LogError("Prefab does not have a MeshFilter.");
            Destroy(instantiatedPrefab);
            Destroy(loadedModel);
            return;
        }

        prefabMeshFilter.mesh = Instantiate(downloadedMeshFilter.sharedMesh);
        Debug.Log("Applied downloaded MeshFilter to prefab.");

        // Find the material named "cross section"
        Renderer renderer = instantiatedPrefab.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("No Renderer found on instantiated prefab!");
            Destroy(instantiatedPrefab);
            return;
        }

        Material[] materials = renderer.materials;
        foreach (Material mat in materials)
        {
            if (mat.name.Contains("Cross-Section-Material")) // Ensure you match the material name
            {
                targetMaterial = mat;
                break;
            }
        }

        if (targetMaterial != null)
        {
            Debug.Log("Found 'cross section' material! Starting texture download...");
            StartCoroutine(DownloadAndApplyTexture());
        }
        else
        {
            Debug.LogError("Could not find 'cross section' material in prefab.");
        }

        // Destroy the original downloaded model
        Destroy(loadedModel);
    }

    IEnumerator DownloadAndApplyTexture()
    {
        Debug.Log("Downloading texture...");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            // Apply to the "_Texture2D" property of the material
            if (targetMaterial != null)
            {
                targetMaterial.SetTexture("_Texture2D", downloadedTexture);
                Debug.Log("? Texture applied to '_Texture2D' property of 'cross section' material!");
            }
            else
            {
                Debug.LogError("? Target material not found, texture not applied.");
            }
        }
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
    }
}
*/
////////////////////////////////////////////////above is woking
/*
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using TriLibCore;
using TriLibCore.General;

public class ModelSample : MonoBehaviour
{
    public string modelUrl = "http://192.168.1.26/3d_models/apis/download_model_unity.php?model_id=105";
    public string textureUrl = "http://yourserver.com/yourimage.png"; // Update with actual texture URL
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;

    private string modelFilePath;
    private string fileExtension = "";
    private GameObject instantiatedPrefab;
    private Material targetMaterial;

    void Awake()
    {
        Debug.Log("Model Loader Ready.");
    }

    public void DownloadAndLoadModelOnClick()
    {
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = modelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
        }

        yield return new WaitForSeconds(1);
        LoadModel();
    }

    void LoadModel()
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            return;
        }

        // Instantiate the prefab
        instantiatedPrefab = Instantiate(prefabWithMeshFilter, spawnPoint.position, Quaternion.identity);
        if (instantiatedPrefab == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            Destroy(loadedModel);
            return;
        }

        // Assign the downloaded mesh to the prefab's MeshFilter
        MeshFilter prefabMeshFilter = instantiatedPrefab.GetComponent<MeshFilter>();
        if (prefabMeshFilter == null)
        {
            Debug.LogError("Prefab does not have a MeshFilter.");
            Destroy(instantiatedPrefab);
            Destroy(loadedModel);
            return;
        }

        // Clone and resize the mesh
        Mesh resizedMesh = Instantiate(downloadedMeshFilter.sharedMesh);
        ResizeMeshToFit(resizedMesh, 0.5f); // Ensure model fits within a 1x1x1 unit cube

        // Apply resized mesh
        prefabMeshFilter.mesh = resizedMesh;
        AdjustColliderToMesh(instantiatedPrefab, resizedMesh);

        Debug.Log("Applied resized MeshFilter to prefab.");

        // Find the material named "cross section"
        Renderer renderer = instantiatedPrefab.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("No Renderer found on instantiated prefab!");
            Destroy(instantiatedPrefab);
            return;
        }

        Material[] materials = renderer.materials;
        foreach (Material mat in materials)
        {
            if (mat.name.Contains("Cross-Section-Material")) // Ensure you match the material name
            {
                targetMaterial = mat;
                break;
            }
        }

        if (targetMaterial != null)
        {
            Debug.Log("Found 'cross section' material! Starting texture download...");
            StartCoroutine(DownloadAndApplyTexture());
        }
        else
        {
            Debug.LogError("Could not find 'cross section' material in prefab.");
        }

        // Destroy the original downloaded model
        Destroy(loadedModel);
    }

    IEnumerator DownloadAndApplyTexture()
    {
        Debug.Log("Downloading texture...");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            if (targetMaterial != null)
            {
                targetMaterial.SetTexture("_Texture2D", downloadedTexture);
                Debug.Log("Texture applied to 'cross section' material!");
            }
            else
            {
                Debug.LogError("Target material not found, texture not applied.");
            }
        }
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
    }
}
*/
//////////////////////above is working with size and collider


//////////////////////BELOW IS WORKING IN MULTIPLAYER

/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TriLibCore;
using TriLibCore.General;

public class ModelSample : MonoBehaviourPunCallbacks
{
    public string modelUrl = "http://192.168.1.26/3d_models/apis/download_model_unity.php?model_id=105";
    public string textureUrl = "http://yourserver.com/yourimage.png"; // Update with actual texture URL
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;

    private string modelFilePath;
    private string fileExtension = "";
    private GameObject instantiatedPrefab;
    private Material targetMaterial;

    void Awake()
    {
        Debug.Log("Model Loader Ready.");
    }

    public void DownloadAndLoadModelOnClick()
    {
        photonView.RPC("RPC_DownloadAndLoadModel", RpcTarget.All, modelUrl);
    }

    [PunRPC]
    void RPC_DownloadAndLoadModel(string url)
    {
        modelUrl = url;
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = modelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
        }

        yield return new WaitForSeconds(1);
        LoadModel();
    }

    void LoadModel()
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            return;
        }

        instantiatedPrefab = Instantiate(prefabWithMeshFilter, spawnPoint.position, Quaternion.identity);
        if (instantiatedPrefab == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            Destroy(loadedModel);
            return;
        }

        MeshFilter prefabMeshFilter = instantiatedPrefab.GetComponent<MeshFilter>();
        if (prefabMeshFilter == null)
        {
            Debug.LogError("Prefab does not have a MeshFilter.");
            Destroy(instantiatedPrefab);
            Destroy(loadedModel);
            return;
        }

        Mesh resizedMesh = Instantiate(downloadedMeshFilter.sharedMesh);
        ResizeMeshToFit(resizedMesh, 0.5f); // Ensure model fits within a 1x1x1 unit cube

        prefabMeshFilter.mesh = resizedMesh;
        AdjustColliderToMesh(instantiatedPrefab, resizedMesh);

        Debug.Log("Applied resized MeshFilter to prefab.");

        Renderer renderer = instantiatedPrefab.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("No Renderer found on instantiated prefab!");
            Destroy(instantiatedPrefab);
            return;
        }

        Material[] materials = renderer.materials;
        foreach (Material mat in materials)
        {
            if (mat.name.Contains("Cross-Section-Material"))
            {
                targetMaterial = mat;
                break;
            }
        }

        if (targetMaterial != null)
        {
            Debug.Log("Found 'cross section' material! Starting texture download...");
            photonView.RPC("RPC_DownloadAndApplyTexture", RpcTarget.All, textureUrl);
        }
        else
        {
            Debug.LogError("Could not find 'cross section' material in prefab.");
        }

        Destroy(loadedModel);
    }

    [PunRPC]
    void RPC_DownloadAndApplyTexture(string url)
    {
        textureUrl = url;
        StartCoroutine(DownloadAndApplyTexture());
    }

    IEnumerator DownloadAndApplyTexture()
    {
        Debug.Log("Downloading texture...");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            if (targetMaterial != null)
            {
                targetMaterial.SetTexture("_Texture2D", downloadedTexture);
                Debug.Log("Texture applied to 'cross section' material!");
            }
            else
            {
                Debug.LogError("Target material not found, texture not applied.");
            }
        }
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
    }
}*/






/*using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using System.Collections;
using System.IO;
using System;
using TriLibCore;
using TriLibCore.General;
using UnityEngine.UIElements;

public class ModelSample : MonoBehaviourPunCallbacks
{
    public string modelUrl = "http://192.168.1.26/3d_models/apis/download_model_unity.php?model_id=105";
    public string textureUrl = "http://yourserver.com/yourimage.png"; // Update with actual texture URL
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;

    private string modelFilePath;
    private string fileExtension = "";
    private GameObject instantiatedPrefab;
    private Material targetMaterial;
    private Mesh downloadedMesh;

    void Awake()
    {
        Debug.Log("Model Loader Ready.");
    }

    public void DownloadAndLoadModelOnClick()
    {
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = modelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
        }

        yield return new WaitForSeconds(1);
        LoadModel();
    }

    void LoadModel()
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            return;
        }

        instantiatedPrefab = PhotonNetwork.Instantiate(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);
        if (instantiatedPrefab == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            Destroy(loadedModel);
            return;
        }

        MeshFilter prefabMeshFilter = instantiatedPrefab.GetComponent<MeshFilter>();
        if (prefabMeshFilter == null)
        {
            Debug.LogError("Prefab does not have a MeshFilter.");
            Destroy(instantiatedPrefab);
            Destroy(loadedModel);
            return;
        }

        downloadedMesh = Instantiate(downloadedMeshFilter.sharedMesh);
        ResizeMeshToFit(downloadedMesh, 0.5f);
        prefabMeshFilter.mesh = downloadedMesh;
        AdjustColliderToMesh(instantiatedPrefab, downloadedMesh);

        Renderer renderer = instantiatedPrefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            targetMaterial = renderer.material;
            StartCoroutine(DownloadAndApplyTexture());
        }

        byte[] meshData = MeshToByteArray(downloadedMesh);
        photonView.RPC("SyncModel", RpcTarget.Others, meshData);

        Destroy(loadedModel);
    }

    [PunRPC]
    private void SyncModel(byte[] meshData)
    {
        instantiatedPrefab = PhotonNetwork.Instantiate(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);
        MeshFilter prefabMeshFilter = instantiatedPrefab.GetComponent<MeshFilter>();

        if (meshData != null && prefabMeshFilter != null)
        {
            Mesh receivedMesh = ByteArrayToMesh(meshData);
            prefabMeshFilter.mesh = receivedMesh;
            AdjustColliderToMesh(instantiatedPrefab, receivedMesh);
        }
    }
    IEnumerator DownloadAndApplyTexture()
    {
        Debug.Log("Downloading texture...");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            if (targetMaterial != null)
            {
                targetMaterial.SetTexture("_Texture2D", downloadedTexture);
                Debug.Log("Texture applied to 'cross section' material!");
            }
            else
            {
                Debug.LogError("Target material not found, texture not applied.");
            }
        }
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
{
    if (mesh == null) return;

    Bounds bounds = mesh.bounds;
    float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
    float scaleFactor = desiredModelSize / maxDimension;

    Vector3[] vertices = mesh.vertices;
    for (int i = 0; i < vertices.Length; i++)
    {
        vertices[i] *= scaleFactor; // Correctly modifying vertices[i]
    }

    mesh.vertices = vertices;
    mesh.RecalculateBounds();
    mesh.RecalculateNormals();
}


    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
    }

    private byte[] MeshToByteArray(Mesh mesh)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(mesh.vertices.Length);

            foreach (Vector3 vertex in mesh.vertices)
            {
                writer.Write(vertex.x);
                writer.Write(vertex.y);
                writer.Write(vertex.z);
            }

            writer.Write(mesh.triangles.Length);
            foreach (int triangle in mesh.triangles)
            {
                writer.Write(triangle);
            }

            return stream.ToArray();
        }
    }


    private Mesh ByteArrayToMesh(byte[] data)
    {
        Mesh mesh = new Mesh();
        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(stream);
            Vector3[] vertices = new Vector3[reader.ReadInt32()];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            int[] triangles = new int[reader.ReadInt32()];
            for (int i = 0; i < triangles.Length; i++)
                triangles[i] = reader.ReadInt32();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
        }
        return mesh;
    }
}*/

/*using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using System.Collections;
using System.IO;
using System;
using TriLibCore;
using TriLibCore.General;
using UnityEngine.UIElements;

public class ModelSample : MonoBehaviourPunCallbacks
{
    public string modelUrl = "http://192.168.1.26/3d_models/apis/download_model_unity.php?model_id=105";
    public string textureUrl = "http://yourserver.com/yourimage.png"; // Update with actual texture URL
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;

    private string modelFilePath;
    private string fileExtension = "";
    private GameObject instantiatedPrefab;
    private Material targetMaterial;
    private Mesh downloadedMesh;

    void Awake()
    {
        Debug.Log("Model Loader Ready.");
    }

    public void DownloadAndLoadModelOnClick()
    {
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = modelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
        }

        yield return new WaitForSeconds(1);
        LoadModel();
    }

    void LoadModel()
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            return;
        }

        instantiatedPrefab = PhotonNetwork.Instantiate(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);
        if (instantiatedPrefab == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            Destroy(loadedModel);
            return;
        }

        MeshFilter prefabMeshFilter = instantiatedPrefab.GetComponent<MeshFilter>();
        if (prefabMeshFilter == null)
        {
            Debug.LogError("Prefab does not have a MeshFilter.");
            Destroy(instantiatedPrefab);
            Destroy(loadedModel);
            return;
        }

        downloadedMesh = Instantiate(downloadedMeshFilter.sharedMesh);
        ResizeMeshToFit(downloadedMesh, 0.5f);
        prefabMeshFilter.mesh = downloadedMesh;
        AdjustColliderToMesh(instantiatedPrefab, downloadedMesh);

        Renderer renderer = instantiatedPrefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            targetMaterial = renderer.material;
            StartCoroutine(DownloadAndApplyTexture());
        }

        byte[] meshData = MeshToByteArray(downloadedMesh);
        photonView.RPC("SyncModel", RpcTarget.Others, meshData);

        Destroy(loadedModel);
    }

    [PunRPC]
    private void SyncModel(byte[] meshData)
    {
        // Instantiate the prefab for the new player
        instantiatedPrefab = PhotonNetwork.Instantiate(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);

        if (instantiatedPrefab == null || meshData == null)
        {
            Debug.LogError("Prefab instantiation or mesh data is null.");
            return;
        }

        MeshFilter prefabMeshFilter = instantiatedPrefab.GetComponent<MeshFilter>();

        if (prefabMeshFilter != null)
        {
            Mesh receivedMesh = ByteArrayToMesh(meshData);

            if (receivedMesh != null)
            {
                prefabMeshFilter.mesh = receivedMesh;
                AdjustColliderToMesh(instantiatedPrefab, receivedMesh); // Adjust collider based on the received mesh
                Debug.Log("Mesh successfully applied to prefab.");
            }
            else
            {
                Debug.LogError("Failed to create mesh from byte array.");
            }
        }
        else
        {
            Debug.LogError("MeshFilter component not found on instantiated prefab.");
        }
    }
    IEnumerator DownloadAndApplyTexture()
    {
        Debug.Log("Downloading texture...");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            if (targetMaterial != null)
            {
                targetMaterial.SetTexture("_Texture2D", downloadedTexture);
                Debug.Log("Texture applied to 'cross section' material!");
            }
            else
            {
                Debug.LogError("Target material not found, texture not applied.");
            }
        }
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor; // Correctly modifying vertices[i]
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }


    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
    }

    private byte[] MeshToByteArray(Mesh mesh)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(mesh.vertices.Length);

            foreach (Vector3 vertex in mesh.vertices)
            {
                writer.Write(vertex.x);
                writer.Write(vertex.y);
                writer.Write(vertex.z);
            }

            writer.Write(mesh.triangles.Length);
            foreach (int triangle in mesh.triangles)
            {
                writer.Write(triangle);
            }

            return stream.ToArray();
        }
    }


    private Mesh ByteArrayToMesh(byte[] data)
    {
        Mesh mesh = new Mesh();
        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(stream);
            Vector3[] vertices = new Vector3[reader.ReadInt32()];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            int[] triangles = new int[reader.ReadInt32()];
            for (int i = 0; i < triangles.Length; i++)
                triangles[i] = reader.ReadInt32();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
        }
        return mesh;
    }
}

*/









///// model came for both without sync


/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TriLibCore;
using TriLibCore.General;

public class ModelSample : MonoBehaviourPunCallbacks
{
    public string modelUrl = "http://192.168.1.26/3d_models/apis/download_model_unity.php?model_id=105";
    public string textureUrl = "http://yourserver.com/yourimage.png"; // Update with actual texture URL
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;

    private string modelFilePath;
    private string fileExtension = "";
    private GameObject instantiatedPrefab;
    private Material targetMaterial;

    void Awake()
    {
        Debug.Log("Model Loader Ready.");
    }

    public void DownloadAndLoadModelOnClick()
    {
        photonView.RPC("RPC_DownloadAndLoadModel", RpcTarget.All, modelUrl);
    }

    [PunRPC]
    void RPC_DownloadAndLoadModel(string url)
    {
        modelUrl = url;
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = modelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
        }

        yield return new WaitForSeconds(1);
        LoadModel();
    }

    void LoadModel()
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            return;
        }

        instantiatedPrefab = Instantiate(prefabWithMeshFilter, spawnPoint.position, Quaternion.identity);
        if (instantiatedPrefab == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            Destroy(loadedModel);
            return;
        }

        // Add PhotonTransformView for syncing position/rotation/scale
        PhotonTransformView photonTransformView = instantiatedPrefab.AddComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            Debug.LogError("Failed to add PhotonTransformView to the prefab.");
            Destroy(instantiatedPrefab);
            Destroy(loadedModel);
            return;
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        MeshFilter prefabMeshFilter = instantiatedPrefab.GetComponent<MeshFilter>();
        if (prefabMeshFilter == null)
        {
            Debug.LogError("Prefab does not have a MeshFilter.");
            Destroy(instantiatedPrefab);
            Destroy(loadedModel);
            return;
        }

        Mesh resizedMesh = Instantiate(downloadedMeshFilter.sharedMesh);
        ResizeMeshToFit(resizedMesh, 0.5f); // Ensure model fits within a 1x1x1 unit cube

        prefabMeshFilter.mesh = resizedMesh;
        AdjustColliderToMesh(instantiatedPrefab, resizedMesh);

        Debug.Log("Applied resized MeshFilter to prefab.");

        Renderer renderer = instantiatedPrefab.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("No Renderer found on instantiated prefab!");
            Destroy(instantiatedPrefab);
            return;
        }

        Material[] materials = renderer.materials;
        foreach (Material mat in materials)
        {
            if (mat.name.Contains("Cross-Section-Material"))
            {
                targetMaterial = mat;
                break;
            }
        }

        if (targetMaterial != null)
        {
            Debug.Log("Found 'cross section' material! Starting texture download...");
            photonView.RPC("RPC_DownloadAndApplyTexture", RpcTarget.All, textureUrl);
        }
        else
        {
            Debug.LogError("Could not find 'cross section' material in prefab.");
        }

        Destroy(loadedModel);
    }

    [PunRPC]
    void RPC_DownloadAndApplyTexture(string url)
    {
        textureUrl = url;
        StartCoroutine(DownloadAndApplyTexture());
    }

    IEnumerator DownloadAndApplyTexture()
    {
        Debug.Log("Downloading texture...");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            if (targetMaterial != null)
            {
                targetMaterial.SetTexture("_Texture2D", downloadedTexture);
                Debug.Log("Texture applied to 'cross section' material!");
            }
            else
            {
                Debug.LogError("Target material not found, texture not applied.");
            }
        }
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
    }
}*/

///////////////////////////////////////////////////////////////////////////////////////////

////////////////////////// working with sync

/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TriLibCore;
using TriLibCore.General;

public class ModelSample : MonoBehaviourPunCallbacks
{
    public string modelUrl = "http://192.168.1.26/3d_models/apis/download_model_unity.php?model_id=105";
    public string textureUrl = "http://yourserver.com/yourimage.png"; // Update with actual texture URL
    public GameObject prefabWithMeshFilter; // Ensure this prefab is in a Resources folder
    public Transform spawnPoint;
    public float desiredModelSize = 0.5f; // Ensure model fits within a specific unit cube

    private string modelFilePath;
    private string fileExtension = "";
    private Material targetMaterial;

    void Awake()
    {
        Debug.Log("Model Loader Ready.");
    }

    public void DownloadAndLoadModelOnClick()
    {
        // Only the local player initiates the download
        StartCoroutine(DownloadAndLoadModel());

        // Synchronize the model download with all other users
        photonView.RPC("RPC_LoadModelFromUrl", RpcTarget.OthersBuffered, modelUrl);
    }

    [PunRPC]
    void RPC_LoadModelFromUrl(string url)
    {
        modelUrl = url;
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = modelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
        }

        yield return new WaitForSeconds(1);
        LoadModel();
    }

    void LoadModel()
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            return;
        }

        // Instantiate the prefab on the network only if it's the local player's action
        if (photonView.IsMine)
        {
            // Create network instance of the prefab at the spawn point
            GameObject modelInstance = PhotonNetwork.Instantiate(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);

            // Apply mesh to the prefab
            AssignMeshToPrefab(loadedModel, modelInstance, downloadedMeshFilter.sharedMesh);

            // Sync the model with other clients
            photonView.RPC("SyncMeshData", RpcTarget.OthersBuffered, modelInstance.GetComponent<PhotonView>().ViewID, modelUrl);

            // Handle texture if needed
            photonView.RPC("RPC_DownloadAndApplyTexture", RpcTarget.All, textureUrl, modelInstance.GetComponent<PhotonView>().ViewID);
        }

        Destroy(loadedModel);
    }

    [PunRPC]
    void SyncMeshData(int viewID, string url)
    {
        StartCoroutine(DownloadAndApplyMesh(viewID, url));
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string url)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + request.error);
                yield break;
            }

            string uniqueFilename = "model_sync_" + DateTime.Now.Ticks + fileExtension;
            string syncModelPath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(syncModelPath, request.downloadHandler.data);

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(syncModelPath, (context) => {
                GameObject loadedModel = context.RootGameObject;
                if (loadedModel != null)
                {
                    MeshFilter meshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        AssignMeshToPrefab(loadedModel, modelView.gameObject, meshFilter.sharedMesh);
                    }
                    Destroy(loadedModel);
                }
            }, null, null, OnError, null, assetLoaderOptions);
        }
    }

    private void AssignMeshToPrefab(GameObject sourceModel, GameObject targetModel, Mesh sourceMesh)
    {
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        if (targetMeshFilter == null)
        {
            Debug.LogError("Target model does not have a MeshFilter.");
            return;
        }

        // Create a resized copy of the mesh
        Mesh resizedMesh = Instantiate(sourceMesh);
        ResizeMeshToFit(resizedMesh, desiredModelSize);

        // Apply the mesh to the target
        targetMeshFilter.mesh = resizedMesh;

        // Configure PhotonTransformView for synchronization
        PhotonTransformView photonTransformView = targetModel.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = targetModel.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        // Adjust collider to fit the mesh
        AdjustColliderToMesh(targetModel, resizedMesh);

        // Get the renderer and find target material if needed
        Renderer renderer = targetModel.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            foreach (Material mat in materials)
            {
                if (mat.name.Contains("Cross-Section-Material"))
                {
                    targetMaterial = mat;
                    break;
                }
            }
        }
    }

    [PunRPC]
    void RPC_DownloadAndApplyTexture(string url, int viewID)
    {
        textureUrl = url;
        StartCoroutine(DownloadAndApplyTexture(viewID));
    }

    IEnumerator DownloadAndApplyTexture(int viewID)
    {
        Debug.Log("Downloading texture...");
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            PhotonView modelView = PhotonView.Find(viewID);
            if (modelView == null)
            {
                Debug.LogError("Model with ViewID " + viewID + " not found for texture application.");
                yield break;
            }

            Renderer renderer = modelView.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] materials = renderer.materials;
                Material targetMat = null;

                foreach (Material mat in materials)
                {
                    if (mat.name.Contains("Cross-Section-Material"))
                    {
                        targetMat = mat;
                        break;
                    }
                }

                if (targetMat != null)
                {
                    targetMat.SetTexture("_Texture2D", downloadedTexture);
                    Debug.Log("Texture applied to 'cross section' material!");
                }
                else
                {
                    Debug.LogError("Target material not found, texture not applied.");
                }
            }
        }
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
    }
}*/




/////////////////////////////////////////working for latejoin 
/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TriLibCore;
using TriLibCore.General;

public class ModelSample : MonoBehaviourPunCallbacks
{
    public string modelUrl = "http://192.168.1.26/3d_models/apis/download_model_unity.php?model_id=105";
    public string textureUrl = "http://yourserver.com/yourimage.png"; // Update with actual texture URL
    public GameObject prefabWithMeshFilter; // Ensure this prefab is in a Resources folder
    public Transform spawnPoint;
    public float desiredModelSize = 0.5f; // Ensure model fits within a specific unit cube

    private string modelFilePath;
    private string fileExtension = "";
    private Material targetMaterial;
    private int currentModelViewID = -1;

    void Awake()
    {
        Debug.Log("Model Loader Ready.");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined room - requesting existing models");

        // Request model data from the master client
        if (!PhotonNetwork.IsMasterClient && currentModelViewID == -1)
        {
            photonView.RPC("RequestModelData", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void RequestModelData()
    {
        // Only master client should handle this request
        if (PhotonNetwork.IsMasterClient && currentModelViewID != -1)
        {
            Debug.Log("Master client received model data request, sending data for ViewID: " + currentModelViewID);
            // Send current model data to the requesting client
            photonView.RPC("SyncMeshData", RpcTarget.Others, currentModelViewID, modelUrl, textureUrl);
        }
    }

    public void DownloadAndLoadModelOnClick()
    {
        // Only the local player initiates the download
        StartCoroutine(DownloadAndLoadModel());

        // Synchronize the model download with all other users
        photonView.RPC("RPC_LoadModelFromUrl", RpcTarget.OthersBuffered, modelUrl, textureUrl);
    }

    [PunRPC]
    void RPC_LoadModelFromUrl(string url, string texUrl)
    {
        modelUrl = url;
        textureUrl = texUrl;
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = modelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
        }

        yield return new WaitForSeconds(1);
        LoadModel();
    }

    void LoadModel()
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            return;
        }

        // Instantiate the prefab on the network only if it's the local player's action
        if (photonView.IsMine)
        {
            // Create network instance of the prefab at the spawn point
            GameObject modelInstance = PhotonNetwork.Instantiate(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);
            currentModelViewID = modelInstance.GetComponent<PhotonView>().ViewID;

            // Apply mesh to the prefab
            AssignMeshToPrefab(loadedModel, modelInstance, downloadedMeshFilter.sharedMesh);

            // Sync the model with other clients, including texture URL
            photonView.RPC("SyncMeshData", RpcTarget.OthersBuffered, currentModelViewID, modelUrl, textureUrl);

            // Download and apply texture to our own instance
            StartCoroutine(DownloadAndApplyTexture(currentModelViewID));
        }

        Destroy(loadedModel);
    }

    [PunRPC]
    void SyncMeshData(int viewID, string url, string texUrl)
    {
        currentModelViewID = viewID;
        textureUrl = texUrl;
        StartCoroutine(DownloadAndApplyMesh(viewID, url));
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string url)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + request.error);
                yield break;
            }

            string uniqueFilename = "model_sync_" + DateTime.Now.Ticks + fileExtension;
            string syncModelPath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(syncModelPath, request.downloadHandler.data);

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(syncModelPath, (context) => {
                GameObject loadedModel = context.RootGameObject;
                if (loadedModel != null)
                {
                    MeshFilter meshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        AssignMeshToPrefab(loadedModel, modelView.gameObject, meshFilter.sharedMesh);

                        // Apply texture after mesh is assigned
                        StartCoroutine(DownloadAndApplyTexture(viewID));
                    }
                    Destroy(loadedModel);
                }
            }, null, null, OnError, null, assetLoaderOptions);
        }
    }

    private void AssignMeshToPrefab(GameObject sourceModel, GameObject targetModel, Mesh sourceMesh)
    {
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        if (targetMeshFilter == null)
        {
            Debug.LogError("Target model does not have a MeshFilter.");
            return;
        }

        // Create a resized copy of the mesh
        Mesh resizedMesh = Instantiate(sourceMesh);
        ResizeMeshToFit(resizedMesh, desiredModelSize);

        // Apply the mesh to the target
        targetMeshFilter.mesh = resizedMesh;

        // Configure PhotonTransformView for synchronization
        PhotonTransformView photonTransformView = targetModel.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = targetModel.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        // Adjust collider to fit the mesh
        AdjustColliderToMesh(targetModel, resizedMesh);

        // Get the renderer and find target material if needed
        Renderer renderer = targetModel.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            foreach (Material mat in materials)
            {
                if (mat.name.Contains("Cross-Section-Material"))
                {
                    targetMaterial = mat;
                    break;
                }
            }
        }
    }

    IEnumerator DownloadAndApplyTexture(int viewID)
    {
        // Add delay to ensure model is loaded first
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Downloading texture for viewID: " + viewID + " from URL: " + textureUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            PhotonView modelView = PhotonView.Find(viewID);
            if (modelView == null)
            {
                Debug.LogError("Model with ViewID " + viewID + " not found for texture application.");
                yield break;
            }

            Renderer renderer = modelView.GetComponent<Renderer>();
            if (renderer != null)
            {
                Debug.Log("Found renderer on model with materials count: " + renderer.materials.Length);

                bool textureApplied = false;
                Material[] materials = renderer.materials;

                // First try to find a specific material by name
                foreach (Material mat in materials)
                {
                    Debug.Log("Checking material: " + mat.name);

                    if (mat.name.Contains("Cross-Section-Material"))
                    {
                        ApplyTextureToMaterial(mat, downloadedTexture);
                        textureApplied = true;
                        break;
                    }
                }

                // If no specific material found, try applying to all materials
                if (!textureApplied)
                {
                    Debug.Log("No specific material found, trying all materials");
                    foreach (Material mat in materials)
                    {
                        ApplyTextureToMaterial(mat, downloadedTexture);
                    }
                }

                // Save updated materials back to renderer
                renderer.materials = materials;
            }
            else
            {
                Debug.LogError("No renderer found on model with ViewID: " + viewID);
            }
        }
    }

    private void ApplyTextureToMaterial(Material mat, Texture2D texture)
    {
        // Try different common texture property names
        string[] texturePropertyNames = new string[] {
            "_Texture2D", "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap"
        };

        foreach (string propName in texturePropertyNames)
        {
            if (mat.HasProperty(propName))
            {
                mat.SetTexture(propName, texture);
                Debug.Log($"Applied texture to material '{mat.name}' using property '{propName}'");
                return;
            }
        }

        Debug.LogWarning($"Could not find appropriate texture property on material '{mat.name}'");
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
    }
}
*/


////////////////////////////////////working////////////////////////
///
















/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TriLibCore;
using TriLibCore.General;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class ModelSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Model Instantiation")]
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;
    public float desiredModelSize = 0.5f;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Model Download Variables
    private string currentModelUrl = "";
    private string currentTextureUrl = "";
    private string fileExtension = "";
    private int currentModelViewID = -1;
    private string currentModelName = "";

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Model Loader Ready.");
    }

    void Start()
    {
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
        // Show downloading UI
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {modelName}...";
        }
        if (gif != null)
        {
            gif.SetActive(true);
        }

        // Construct the model and texture URLs
        currentModelUrl = "https://medispherexr.com/api/src/models/download_model.php?model_id=" + modelId;
        currentModelName = modelName;

        // You can modify texture URL if needed
        currentTextureUrl = "https://medispherexr.com/api/src/models/download_texture.php?model_id=" + modelId;

        // Start the download locally for the user initiating the action
        StartCoroutine(DownloadAndLoadModel());

        // Synchronize the model download with all other users
        photonView.RPC("RPC_LoadModelFromUrl", RpcTarget.OthersBuffered, currentModelUrl, currentTextureUrl);
    }

    [PunRPC]
    void RPC_LoadModelFromUrl(string url, string texUrl)
    {
        currentModelUrl = url;
        currentTextureUrl = texUrl;
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = currentModelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                UpdateDownloadUI(false);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            // Determine file extension
            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            string modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);

            yield return new WaitForSeconds(1);
            LoadModel(modelFilePath);
        }
    }

    void LoadModel(string modelFilePath)
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            UpdateDownloadUI(false);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            UpdateDownloadUI(false);
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            UpdateDownloadUI(false);
            return;
        }

        // Instantiate the prefab on the network only if it's the local player's action
        if (photonView.IsMine)
        {
            // Create network instance of the prefab at the spawn point
            GameObject modelInstance = PhotonNetwork.Instantiate(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);
            currentModelViewID = modelInstance.GetComponent<PhotonView>().ViewID;

            // Apply mesh to the prefab
            AssignMeshToPrefab(loadedModel, modelInstance, downloadedMeshFilter.sharedMesh);

            // Sync the model with other clients, including texture URL
            photonView.RPC("SyncMeshData", RpcTarget.OthersBuffered, currentModelViewID, currentModelUrl, currentTextureUrl);

            // Download and apply texture to our own instance
            StartCoroutine(DownloadAndApplyTexture(currentModelViewID));
        }

        Destroy(loadedModel);
        UpdateDownloadUI(true);
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentModelName} downloaded successfully!"
                : $"Failed to download {currentModelName}";

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
    void SyncMeshData(int viewID, string url, string texUrl)
    {
        currentModelViewID = viewID;
        currentModelUrl = url;
        currentTextureUrl = texUrl;
        StartCoroutine(DownloadAndApplyMesh(viewID, url));
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string url)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + request.error);
                yield break;
            }

            string uniqueFilename = "model_sync_" + DateTime.Now.Ticks + fileExtension;
            string syncModelPath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(syncModelPath, request.downloadHandler.data);

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(syncModelPath, (context) => {
                GameObject loadedModel = context.RootGameObject;
                if (loadedModel != null)
                {
                    MeshFilter meshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        AssignMeshToPrefab(loadedModel, modelView.gameObject, meshFilter.sharedMesh);

                        // Apply texture after mesh is assigned
                        StartCoroutine(DownloadAndApplyTexture(viewID));
                    }
                    Destroy(loadedModel);
                }
            }, null, null, OnError, null, assetLoaderOptions);
        }
    }

    private void AssignMeshToPrefab(GameObject sourceModel, GameObject targetModel, Mesh sourceMesh)
    {
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        if (targetMeshFilter == null)
        {
            Debug.LogError("Target model does not have a MeshFilter.");
            return;
        }

        // Create a resized copy of the mesh
        Mesh resizedMesh = Instantiate(sourceMesh);
        ResizeMeshToFit(resizedMesh, desiredModelSize);

        // Apply the mesh to the target
        targetMeshFilter.mesh = resizedMesh;

        // Configure PhotonTransformView for synchronization
        PhotonTransformView photonTransformView = targetModel.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = targetModel.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        // Adjust collider to fit the mesh
        AdjustColliderToMesh(targetModel, resizedMesh);
    }

    private IEnumerator DownloadAndApplyTexture(int viewID)
    {
        // Add delay to ensure model is loaded first
        yield return new WaitForSeconds(0.5f);

        if (string.IsNullOrEmpty(currentTextureUrl))
        {
            Debug.Log("No texture URL provided.");
            yield break;
        }

        Debug.Log("Downloading texture for viewID: " + viewID + " from URL: " + currentTextureUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(currentTextureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            PhotonView modelView = PhotonView.Find(viewID);
            if (modelView == null)
            {
                Debug.LogError("Model with ViewID " + viewID + " not found for texture application.");
                yield break;
            }

            Renderer renderer = modelView.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] materials = renderer.materials;
                foreach (Material mat in materials)
                {
                    ApplyTextureToMaterial(mat, downloadedTexture);
                }
                renderer.materials = materials;
            }
            else
            {
                Debug.LogError("No renderer found on model with ViewID: " + viewID);
            }
        }
    }

    private void ApplyTextureToMaterial(Material mat, Texture2D texture)
    {
        // Try different common texture property names
        string[] texturePropertyNames = new string[] {
            "_Texture2D", "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap"
        };

        foreach (string propName in texturePropertyNames)
        {
            if (mat.HasProperty(propName))
            {
                mat.SetTexture(propName, texture);
                Debug.Log($"Applied texture to material '{mat.name}' using property '{propName}'");
                return;
            }
        }

        Debug.LogWarning($"Could not find appropriate texture property on material '{mat.name}'");
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
        UpdateDownloadUI(false);
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




/////////////////////////////////// transparent shader//////////////////




/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TriLibCore;
using TriLibCore.General;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class ModelSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Model Instantiation")]
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;
    public float desiredModelSize = 0.5f;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Model Download Variables
    private string currentModelUrl = "";
    private string currentTextureUrl = "";
    private string fileExtension = "";
    private int currentModelViewID = -1;
    private string currentModelName = "";
    private bool currentIsTransparent = false; // Store transparency state

    // Materials
    private Material crossSectionMaterial;
    private Material crossSectionTransparentMaterial;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Model Loader Ready.");

        // Load materials from Resources folder
        crossSectionMaterial = Resources.Load<Material>("Cross-Section-Material");
        crossSectionTransparentMaterial = Resources.Load<Material>("Cross-Section-Material_Transparent");

        if (crossSectionMaterial == null || crossSectionTransparentMaterial == null)
        {
            Debug.LogError("One or both materials not found in Resources folder.");
        }
    }

    void Start()
    {
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

                    foreach (ModelData model in modelListResponse.models)
                    {
                        if (model != null && !string.IsNullOrEmpty(model.name))
                        {
                            AddModelNameToUI(model.id, model.name, model.is_transparent);
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

    private void AddModelNameToUI(string modelId, string modelName, bool isTransparent)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId, modelName, isTransparent));
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

    private void OnModelNameClicked(string modelId, string modelName, bool isTransparent)
    {
        // Show downloading UI
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {modelName}...";
        }
        if (gif != null)
        {
            gif.SetActive(true);
        }

        // Construct the model and texture URLs
        currentModelUrl = "https://medispherexr.com/api/src/models/download_model.php?model_id=" + modelId;
        currentTextureUrl = "https://medispherexr.com/api/src/models/download_texture.php?model_id=" + modelId;
        currentModelName = modelName;
        currentIsTransparent = isTransparent;

        // Start the download locally for the user initiating the action
        StartCoroutine(DownloadAndLoadModel());

        // Synchronize the model download with all other users
        photonView.RPC("RPC_LoadModelFromUrl", RpcTarget.OthersBuffered, currentModelUrl, currentTextureUrl, currentIsTransparent);
    }

    [PunRPC]
    void RPC_LoadModelFromUrl(string url, string texUrl, bool isTransparent)
    {
        currentModelUrl = url;
        currentTextureUrl = texUrl;
        currentIsTransparent = isTransparent;
        StartCoroutine(DownloadAndLoadModel());
    }

    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = currentModelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                UpdateDownloadUI(false);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            // Determine file extension
            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            string modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);

            yield return new WaitForSeconds(1);
            LoadModel(modelFilePath);
        }
    }

    void LoadModel(string modelFilePath)
    {
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            UpdateDownloadUI(false);
            return;
        }

        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            UpdateDownloadUI(false);
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            UpdateDownloadUI(false);
            return;
        }

        // Instantiate the prefab on the network only if it's the local player's action
        if (photonView.IsMine)
        {
            // Create network instance of the prefab at the spawn point
            GameObject modelInstance = PhotonNetwork.Instantiate(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);
            currentModelViewID = modelInstance.GetComponent<PhotonView>().ViewID;

            // Apply mesh and material to the prefab
            AssignMeshAndMaterialToPrefab(loadedModel, modelInstance, downloadedMeshFilter.sharedMesh);

            // Sync the model with other clients, including texture URL and transparency
            photonView.RPC("SyncMeshData", RpcTarget.OthersBuffered, currentModelViewID, currentModelUrl, currentTextureUrl, currentIsTransparent);

            // Download and apply texture to our own instance
            StartCoroutine(DownloadAndApplyTexture(currentModelViewID));
        }

        Destroy(loadedModel);
        UpdateDownloadUI(true);
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.text = success
                ? $"{currentModelName} downloaded successfully!"
                : $"Failed to download {currentModelName}";

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
    void SyncMeshData(int viewID, string url, string texUrl, bool isTransparent)
    {
        currentModelViewID = viewID;
        currentModelUrl = url;
        currentTextureUrl = texUrl;
        currentIsTransparent = isTransparent;
        StartCoroutine(DownloadAndApplyMesh(viewID, url));
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string url)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + request.error);
                yield break;
            }

            string uniqueFilename = "model_sync_" + DateTime.Now.Ticks + fileExtension;
            string syncModelPath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(syncModelPath, request.downloadHandler.data);

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(syncModelPath, (context) =>
            {
                GameObject loadedModel = context.RootGameObject;
                if (loadedModel != null)
                {
                    MeshFilter meshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        AssignMeshAndMaterialToPrefab(loadedModel, modelView.gameObject, meshFilter.sharedMesh);

                        // Apply texture after mesh is assigned
                        StartCoroutine(DownloadAndApplyTexture(viewID));
                    }
                    Destroy(loadedModel);
                }
            }, null, null, OnError, null, assetLoaderOptions);
        }
    }

    private void AssignMeshAndMaterialToPrefab(GameObject sourceModel, GameObject targetModel, Mesh sourceMesh)
    {
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        if (targetMeshFilter == null)
        {
            Debug.LogError("Target model does not have a MeshFilter.");
            return;
        }

        // Create a resized copy of the mesh
        Mesh resizedMesh = Instantiate(sourceMesh);
        ResizeMeshToFit(resizedMesh, desiredModelSize);

        // Apply the mesh to the target
        targetMeshFilter.mesh = resizedMesh;

        // Apply the appropriate material based on is_transparent
        MeshRenderer targetRenderer = targetModel.GetComponent<MeshRenderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("Target model does not have a MeshRenderer.");
            return;
        }

        Material selectedMaterial = currentIsTransparent ? crossSectionTransparentMaterial : crossSectionMaterial;
        if (selectedMaterial == null)
        {
            Debug.LogError("Selected material is null.");
            return;
        }

        targetRenderer.material = selectedMaterial;
        Debug.Log($"Applied material: {(currentIsTransparent ? "CrossSection-Transparent" : "CrossSection")} to model.");

        // Configure PhotonTransformView for synchronization
        PhotonTransformView photonTransformView = targetModel.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = targetModel.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        // Adjust collider to fit the mesh
        AdjustColliderToMesh(targetModel, resizedMesh);
    }

    private IEnumerator DownloadAndApplyTexture(int viewID)
    {
        // Add delay to ensure model is loaded first
        yield return new WaitForSeconds(0.5f);

        if (string.IsNullOrEmpty(currentTextureUrl))
        {
            Debug.Log("No texture URL provided.");
            yield break;
        }

        Debug.Log("Downloading texture for viewID: " + viewID + " from URL: " + currentTextureUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(currentTextureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            PhotonView modelView = PhotonView.Find(viewID);
            if (modelView == null)
            {
                Debug.LogError("Model with ViewID " + viewID + " not found for texture application.");
                yield break;
            }

            MeshRenderer renderer = modelView.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material material = renderer.material; // Get the current material (CrossSection or CrossSection-Transparent)
                ApplyTextureToMaterial(material, downloadedTexture);
                renderer.material = material;
            }
            else
            {
                Debug.LogError("No renderer found on model with ViewID: " + viewID);
            }
        }
    }

    private void ApplyTextureToMaterial(Material mat, Texture2D texture)
    {
        // Try different common texture property names
        string[] texturePropertyNames = new string[]
        {
            "_Texture2D", "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap"
        };

        foreach (string propName in texturePropertyNames)
        {
            if (mat.HasProperty(propName))
            {
                mat.SetTexture(propName, texture);
                Debug.Log($"Applied texture to material '{mat.name}' using property '{propName}'");
                return;
            }
        }

        Debug.LogWarning($"Could not find appropriate texture property on material '{mat.name}'");
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
        UpdateDownloadUI(false);
    }

    [System.Serializable]
    public class ModelData
    {
        public string id;
        public string name;
        public bool is_transparent; // Field for transparency
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public ModelData[] models;
    }
}

*/











//////////////////////////////////////////current working//////////////////////////////////////////////////////////////////////////////////////////








/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TriLibCore;
using TriLibCore.General;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class ModelSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Model Instantiation")]
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;
    public float desiredModelSize = 0.5f;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Model Download Variables
    private string currentModelUrl = "";
    private string currentTextureUrl = "";
    private string fileExtension = "";
    private int currentModelViewID = -1;
    private string currentModelName = "";
    private bool currentIsTransparent = false; // Store transparency state
    
    // Materials
    private Material crossSectionMaterial;
    private Material crossSectionTransparentMaterial;

    // Model Tracking for Late Join Sync
    private Dictionary<int, ModelSyncData> activeModels = new Dictionary<int, ModelSyncData>();

    // Class to store model data for synchronization
    [System.Serializable]
    private class ModelSyncData
    {
        public int viewID;
        public string modelUrl;
        public string textureUrl;
        public bool isTransparent;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Mesh cachedMesh; // Store the mesh for direct application
    }

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Model Loader Ready.");

        // Load materials from Resources folder
        crossSectionMaterial = Resources.Load<Material>("Cross-Section-Material");
        crossSectionTransparentMaterial = Resources.Load<Material>("Cross-Section-Material_Transparent");

        if (crossSectionMaterial == null || crossSectionTransparentMaterial == null)
        {
            Debug.LogError("One or both materials not found in Resources folder.");
        }

        // Register event handlers for Photon destruction
        PhotonNetwork.NetworkingClient.EventReceived += OnPhotonEvent;
        
    }

    void OnDestroy()
    {
        // Unregister event handlers when this component is destroyed
        PhotonNetwork.NetworkingClient.EventReceived -= OnPhotonEvent;
    }

    private void OnPhotonEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        const byte EVENT_CODE_DESTROY = 204; // PUN internal: object destroy event

        if (photonEvent.Code == EVENT_CODE_DESTROY)
        {
            int viewID = (int)((object[])photonEvent.CustomData)[0];

            if (activeModels.ContainsKey(viewID))
            {
                Debug.Log($"Removing destroyed model with ViewID: {viewID}");
                activeModels.Remove(viewID);
            }
        }
    }


    void Start()
    {
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

    *//*public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }*//*

    // New method - called when player joins the room
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room - requesting active models from Master Client");

        // Only request model data if we're not the master client and there are other players
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            // Request active models from master client
            photonView.RPC("RequestActiveModels", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    // New method - request active models when joining late
    [PunRPC]
    void RequestActiveModels(int requestingPlayerActorNumber)
    {
        Debug.Log($"Received request for active models from player {requestingPlayerActorNumber}");

        // Master client sends all active model data directly to the requesting player
        foreach (var modelData in activeModels.Values)
        {
            // Use player's actor number to target the specific player
            Player requestingPlayer = PhotonNetwork.CurrentRoom.GetPlayer(requestingPlayerActorNumber);
            if (requestingPlayer != null)
            {
                photonView.RPC("ReceiveModelData", requestingPlayer,
                    modelData.viewID,
                    modelData.modelUrl,
                    modelData.textureUrl,
                    modelData.isTransparent,
                    modelData.position,
                    modelData.rotation,
                    modelData.scale);
            }
        }
    }

    // New method - receive model data for late joiners
    [PunRPC]
    void ReceiveModelData(int viewID, string modelUrl, string textureUrl, bool isTransparent,
                         Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Debug.Log($"Late join: Received model data for viewID: {viewID}");

        // Check if this model already exists in our scene
        PhotonView existingView = PhotonView.Find(viewID);
        if (existingView != null)
        {
            Debug.Log($"Model with viewID {viewID} already exists - updating properties");
            existingView.transform.position = position;
            existingView.transform.rotation = rotation;
            existingView.transform.localScale = scale;

            // Re-download and apply the mesh/texture, as it might be missing
            currentModelUrl = modelUrl;
            currentTextureUrl = textureUrl;
            currentIsTransparent = isTransparent;
            currentModelViewID = viewID;
            StartCoroutine(DownloadAndApplyMesh(viewID, modelUrl));
        }
        else
        {
            Debug.Log($"Creating new model instance for late join with viewID: {viewID}");

            // We need to synchronize the ViewID exactly with what the master is expecting
            // First, create the object through PhotonNetwork to generate the proper viewID
            GameObject modelInstance = PhotonNetwork.InstantiateRoomObject(prefabWithMeshFilter.name, position, rotation);

            // Force the ViewID to match the master's expected ID
            PhotonView newView = modelInstance.GetComponent<PhotonView>();
            if (newView != null)
            {
                // We can't directly set ViewID, so we'll track it in our own system
                Debug.Log($"Created instance with ViewID: {newView.ViewID} (tracking as master's ViewID: {viewID})");

                // Store in a lookup table for later reference
                if (!activeModels.ContainsKey(viewID))
                {
                    ModelSyncData syncData = new ModelSyncData
                    {
                        viewID = viewID,
                        modelUrl = modelUrl,
                        textureUrl = textureUrl,
                        isTransparent = isTransparent,
                        position = position,
                        rotation = rotation,
                        scale = scale
                    };
                    activeModels.Add(viewID, syncData);
                }

                // Set up PhotonTransformView for syncing transforms
                PhotonTransformView transformView = modelInstance.GetComponent<PhotonTransformView>();
                if (transformView == null)
                {
                    transformView = modelInstance.AddComponent<PhotonTransformView>();
                }
                transformView.m_SynchronizePosition = true;
                transformView.m_SynchronizeRotation = true;
                transformView.m_SynchronizeScale = true;

                // Set transform values
                modelInstance.transform.position = position;
                modelInstance.transform.rotation = rotation;
                modelInstance.transform.localScale = scale;

                // Store the viewID for reference during downloads
                currentModelViewID = newView.ViewID; // Use the actual ViewID for download

                // Start mesh download and application
                currentModelUrl = modelUrl;
                currentTextureUrl = textureUrl;
                currentIsTransparent = isTransparent;
                StartCoroutine(DownloadAndApplyMesh(newView.ViewID, modelUrl));
            }
            else
            {
                Debug.LogError("PhotonView component missing on prefab!");
            }
        }
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
        string url = "https://medispherexr.com/api/src/api/model/view_models.php";

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

                    foreach (ModelData model in modelListResponse.models)
                    {
                        if (model != null && !string.IsNullOrEmpty(model.name))
                        {
                            AddModelNameToUI(model.id, model.name, model.is_transparent);
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

    private void AddModelNameToUI(string modelId, string modelName, bool isTransparent)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId, modelName, isTransparent));
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

    private void OnModelNameClicked(string modelId, string modelName, bool isTransparent)
    {
        // Show downloading UI
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {modelName}...";
        }
        if (gif != null)
        {
            gif.SetActive(true);
        }

        // Construct the model and texture URLs
        // currentModelUrl = "https://medispherexr.com/api/src/models/download_model.php?model_id=" + modelId;
        //  currentTextureUrl = "https://medispherexr.com/api/src/models/download_texture.php?model_id=" + modelId;

        currentModelUrl = "https://medispherexr.com/api/src/api/model/download_model.php?model_id=" + modelId;
        currentTextureUrl = "https://medispherexr.com/api/src/api/model/download_texture.php?model_id=" + modelId;
        currentModelName = modelName;
        currentIsTransparent = isTransparent;

        // Start the download locally for the user initiating the action
        StartCoroutine(DownloadAndLoadModel());

        // Synchronize the model download with all other users
        photonView.RPC("RPC_LoadModelFromUrl", RpcTarget.OthersBuffered, currentModelUrl, currentTextureUrl, currentIsTransparent);
    }

    [PunRPC]
    void RPC_LoadModelFromUrl(string url, string texUrl, bool isTransparent)
    {
        currentModelUrl = url;
        currentTextureUrl = texUrl;
        currentIsTransparent = isTransparent;
        StartCoroutine(DownloadAndLoadModel());
    }
    //chnagesssssssss//////////////////////////////

    public void LoadModel(string modelId, string modelName, bool isTransparent)           ///////////newwwww
    {
        OnModelNameClicked(modelId, modelName, isTransparent);
    }
     IEnumerator DownloadAndLoadModel()
     {
         Debug.Log("Starting Model Download...");
         string uniqueModelUrl = currentModelUrl + "&timestamp=" + DateTime.Now.Ticks;

         using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
         {
             request.downloadHandler = new DownloadHandlerBuffer();
             yield return request.SendWebRequest();

             if (request.result != UnityWebRequest.Result.Success)
             {
                 Debug.LogError("Model Download Failed: " + request.error);
                 UpdateDownloadUI(false);
                 yield break;
             }

             Debug.Log("Model Downloaded Successfully!");

             // Determine file extension
             string contentDisposition = request.GetResponseHeader("Content-Disposition");
             if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
             {
                 string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                 fileExtension = Path.GetExtension(fileName);
             }
             else
             {
                 Debug.LogError("Could not determine file extension from response.");
                 yield break;
             }

             string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
             string modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
             // File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
             System.IO.File.WriteAllBytes(modelFilePath, request.downloadHandler.data);         ///////////////change

             yield return new WaitForSeconds(1);
             LoadModel(modelFilePath);
         }
     }
    //chnagesssss////////////////////////////////////////////////////////////////
  


   

    
        void LoadModel(string modelFilePath)
        {
            *//*if (!File.Exists(modelFilePath))
            {
                Debug.LogError("File not found at: " + modelFilePath);
                UpdateDownloadUI(false);
                return;
            }*//*
            if (!System.IO.File.Exists(modelFilePath)) // Fix
            {
                Debug.LogError("File not found at: " + modelFilePath);          /// change
                UpdateDownloadUI(false);
                return;
            }
            Debug.Log("Loading Model from: " + modelFilePath);

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
        }

   

    
        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Model Loaded Successfully!");

            GameObject loadedModel = assetLoaderContext.RootGameObject;
            if (loadedModel == null)
            {
                Debug.LogError("Loaded model is null!");
                UpdateDownloadUI(false);
                return;
            }

            MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
            if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
            {
                Debug.LogError("No valid MeshFilter found in downloaded model.");
                Destroy(loadedModel);
                UpdateDownloadUI(false);
                return;
            }

            // Instantiate the prefab on the network only if it's the local player's action
            if (photonView.IsMine)
            {
                // Create network instance of the prefab at the spawn point
                GameObject modelInstance = PhotonNetwork.InstantiateRoomObject(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.identity);
                currentModelViewID = modelInstance.GetComponent<PhotonView>().ViewID;

                // Apply mesh and material to the prefab
                AssignMeshAndMaterialToPrefab(loadedModel, modelInstance, downloadedMeshFilter.sharedMesh);

                // Store model data for late joiners
                StoreModelData(modelInstance);

                // Broadcast to all clients that a new model has been created
                photonView.RPC("SyncMeshData", RpcTarget.OthersBuffered, currentModelViewID, currentModelUrl, currentTextureUrl, currentIsTransparent);

                // Download and apply texture to our own instance
                StartCoroutine(DownloadAndApplyTexture(currentModelViewID));

                // Log model creation with viewID
                Debug.Log($"Created new model with ViewID: {currentModelViewID}");
            }

            Destroy(loadedModel);
            UpdateDownloadUI(true);
        }

   


    private void StoreModelData(GameObject modelInstance)
    {
        if (modelInstance == null) return;

        PhotonView modelView = modelInstance.GetComponent<PhotonView>();
        if (modelView == null) return;

        int viewID = modelView.ViewID;

        // Get the mesh if possible
        MeshFilter meshFilter = modelInstance.GetComponent<MeshFilter>();
        Mesh cachedMesh = null;
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            cachedMesh = meshFilter.sharedMesh;
        }

        // Create and store model data
        ModelSyncData modelData = new ModelSyncData
        {
            viewID = viewID,
            modelUrl = currentModelUrl,
            textureUrl = currentTextureUrl,
            isTransparent = currentIsTransparent,
            position = modelInstance.transform.position,
            rotation = modelInstance.transform.rotation,
            scale = modelInstance.transform.localScale,
            cachedMesh = cachedMesh
        };

        // Store or update model data
        if (activeModels.ContainsKey(viewID))
        {
            activeModels[viewID] = modelData;
        }
        else
        {
            activeModels.Add(viewID, modelData);
        }

        Debug.Log($"Stored model data for viewID: {viewID}. Total active models: {activeModels.Count}");
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = success
                ? $"{currentModelName} downloaded successfully!"
                : $"Failed to download {currentModelName}";
            Debug.Log($"UI: Set message to '{downloadingMessage.text}'");
            // Reset flag after download completes
          
            StartCoroutine(HideDownloadUI());
        }
        else
        {
            Debug.LogError("downloadingMessage is null in UpdateDownloadUI!");
        }

        if (gif != null)
        {
            gif.SetActive(false);
            Debug.Log("UI: Loading gif deactivated");
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
    void SyncMeshData(int viewID, string url, string texUrl, bool isTransparent)
    {
        Debug.Log($"Syncing mesh data for ViewID {viewID}");
        currentModelViewID = viewID;
        currentModelUrl = url;
        currentTextureUrl = texUrl;
        currentIsTransparent = isTransparent;

        // Try to find existing model
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView != null)
        {
            GameObject modelInstance = modelView.gameObject;

            // Start downloading and applying mesh
            StartCoroutine(DownloadAndApplyMesh(viewID, url));

            // Create and store model data
            ModelSyncData modelData = new ModelSyncData
            {
                viewID = viewID,
                modelUrl = url,
                textureUrl = texUrl,
                isTransparent = isTransparent,
                position = modelInstance.transform.position,
                rotation = modelInstance.transform.rotation,
                scale = modelInstance.transform.localScale,
                cachedMesh = null // Will be populated after download
            };

            // Store or update model data
            if (activeModels.ContainsKey(viewID))
            {
                activeModels[viewID] = modelData;
            }
            else
            {
                activeModels.Add(viewID, modelData);
            }

            Debug.Log($"Added/updated model data for viewID: {viewID}");
        }
        else
        {
            Debug.LogError($"Failed to find PhotonView with ViewID: {viewID}");
        }
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string url)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + request.error);
                yield break;
            }

            string uniqueFilename = "model_sync_" + DateTime.Now.Ticks + fileExtension;
            string syncModelPath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(syncModelPath, request.downloadHandler.data);

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(syncModelPath, (context) =>
            {
                GameObject loadedModel = context.RootGameObject;
                if (loadedModel != null)
                {
                    MeshFilter meshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        AssignMeshAndMaterialToPrefab(loadedModel, modelView.gameObject, meshFilter.sharedMesh);

                        // Apply texture after mesh is assigned
                        StartCoroutine(DownloadAndApplyTexture(viewID));
                    }
                    Destroy(loadedModel);
                }
            }, null, null, OnError, null, assetLoaderOptions);
        }

    }

    private void AssignMeshAndMaterialToPrefab(GameObject sourceModel, GameObject targetModel, Mesh sourceMesh)
    {
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        if (targetMeshFilter == null)
        {
            Debug.LogError("Target model does not have a MeshFilter.");
            return;
        }

        // Create a resized copy of the mesh
        Mesh resizedMesh = Instantiate(sourceMesh);
        ResizeMeshToFit(resizedMesh, desiredModelSize);

        // Apply the mesh to the target
        targetMeshFilter.mesh = resizedMesh;

        // Apply the appropriate material based on is_transparent
        MeshRenderer targetRenderer = targetModel.GetComponent<MeshRenderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("Target model does not have a MeshRenderer.");
            return;
        }

        Material selectedMaterial = currentIsTransparent ? crossSectionTransparentMaterial : crossSectionMaterial;
        if (selectedMaterial == null)
        {
            Debug.LogError("Selected material is null.");
            return;
        }

        targetRenderer.material = selectedMaterial;
        Debug.Log($"Applied material: {(currentIsTransparent ? "CrossSection-Transparent" : "CrossSection")} to model.");

        // Configure PhotonTransformView for synchronization
        PhotonTransformView photonTransformView = targetModel.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = targetModel.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        // Adjust collider to fit the mesh
        AdjustColliderToMesh(targetModel, resizedMesh);
    }

    private IEnumerator DownloadAndApplyTexture(int viewID)
    {
        // Add delay to ensure model is loaded first
        yield return new WaitForSeconds(1f);

        if (string.IsNullOrEmpty(currentTextureUrl))
        {
            Debug.Log("No texture URL provided.");
            yield break;
        }

        Debug.Log("Downloading texture for viewID: " + viewID + " from URL: " + currentTextureUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(currentTextureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            PhotonView modelView = PhotonView.Find(viewID);
            if (modelView == null)
            {
                Debug.LogError("Model with ViewID " + viewID + " not found for texture application.");
                yield break;
            }

            MeshRenderer renderer = modelView.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material material = renderer.material; // Get the current material (CrossSection or CrossSection-Transparent)
                ApplyTextureToMaterial(material, downloadedTexture);
                renderer.material = material;
            }
            else
            {
                Debug.LogError("No renderer found on model with ViewID: " + viewID);
            }
        }
        
    }

    private void ApplyTextureToMaterial(Material mat, Texture2D texture)
    {
        // Try different common texture property names
        string[] texturePropertyNames = new string[]
        {
            "_Texture2D", "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap"
        };

        foreach (string propName in texturePropertyNames)
        {
            if (mat.HasProperty(propName))
            {
                mat.SetTexture(propName, texture);
                Debug.Log($"Applied texture to material '{mat.name}' using property '{propName}'");
                return;
            }
        }

        Debug.LogWarning($"Could not find appropriate texture property on material '{mat.name}'");
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        collider.center = mesh.bounds.center;
        collider.size = mesh.bounds.size;
    }

    // Cleanup when object is destroyed or player leaves
    public override void OnLeftRoom()
    {
        // Clear active models list
        activeModels.Clear();
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
        UpdateDownloadUI(false);
      
    }

    [System.Serializable]
    public class ModelData
    {
        public string id;
        public string name;
        public bool is_transparent; // Field for transparency
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public ModelData[] models;
    }




    [PunRPC]
    void RequestDirectMeshTransfer(int viewID, int requestingActorNumber)
    {
        Debug.Log($"Direct mesh transfer requested for viewID: {viewID}");

        // Check if we have this model with a cached mesh
        if (activeModels.ContainsKey(viewID) && activeModels[viewID].cachedMesh != null)
        {
            // We can't directly send the mesh over RPC, so we'll just notify the player to download it
            PhotonView requesterView = null;

            // Find the requesting player's PhotonView on this ModelSample component
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (player.ActorNumber == requestingActorNumber)
                {
                    photonView.RPC("ForceModelDownload", player, viewID,
                        activeModels[viewID].modelUrl,
                        activeModels[viewID].textureUrl,
                        activeModels[viewID].isTransparent,
                        activeModels[viewID].position,
                        activeModels[viewID].rotation,
                        activeModels[viewID].scale);
                    break;
                }
            }
        }
    }

    [PunRPC]
    void ForceModelDownload(int viewID, string modelUrl, string textureUrl, bool isTransparent,
                           Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Debug.Log($"Force downloading model for viewID: {viewID}");

        // Set current values for download
        currentModelViewID = viewID;
        currentModelUrl = modelUrl;
        currentTextureUrl = textureUrl;
        currentIsTransparent = isTransparent;

        // Get the actual GameObject this refers to
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            // Set transform values directly
            targetView.transform.position = position;
            targetView.transform.rotation = rotation;
            targetView.transform.localScale = scale;

            // Force redownload the mesh and texture
            StartCoroutine(ForceRedownloadMesh(viewID, modelUrl, textureUrl, isTransparent));
        }
        else
        {
            Debug.LogError($"Cannot find PhotonView with ID {viewID} for forced download");
        }
    }

    private IEnumerator ForceRedownloadMesh(int viewID, string url, string texUrl, bool isTransparent)
    {
        Debug.Log($"Starting forced model download for viewID: {viewID}");

        // Add a unique parameter to avoid cache
        string uniqueModelUrl = url + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Force download failed: {request.error}");
                yield break;
            }

            // Get the file extension
            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }

            // Save to disk
            string uniqueFilename = "model_force_" + viewID + "_" + DateTime.Now.Ticks + fileExtension;
            string modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);

            // Load the model
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(modelFilePath, (context) =>
            {
                GameObject loadedModel = context.RootGameObject;
                if (loadedModel != null)
                {
                    MeshFilter meshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        // Find the target view again (in case it changed)
                        PhotonView targetView = PhotonView.Find(viewID);
                        if (targetView != null)
                        {
                            // Apply mesh and material
                            AssignMeshAndMaterialToPrefab(loadedModel, targetView.gameObject, meshFilter.sharedMesh);

                            // Now download and apply texture
                            StartCoroutine(DownloadAndApplyTexture(viewID));

                            Debug.Log($"Successfully force-applied mesh to viewID: {viewID}");
                        }
                    }
                    Destroy(loadedModel);
                }
            }, null, null, OnError, null, assetLoaderOptions);
        }

    }
}

*/






using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using Photon.Pun;
using TriLibCore;
using TriLibCore.General;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class ModelSample : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Button fetchButton;
    public GameObject contentPanel;
    public GameObject textPrefab;
    public GameObject gif;
    public TextMeshProUGUI downloadingMessage;

    [Header("Session Management")]
    public SessionManager sessionManager;



    [Header("Model Instantiation")]
    public GameObject prefabWithMeshFilter;
    public Transform spawnPoint;
    public float desiredModelSize = 0.5f;

    [Header("Network Settings")]
    private PhotonView photonView;

    // Model Download Variables
    private string currentModelUrl = "";
    private string currentTextureUrl = "";
    private string fileExtension = "";
    private int currentModelViewID = -1;
    private string currentModelName = "";
    private bool currentIsTransparent = false; // Store transparency state
    private bool currentIsChild = false; // New variable for parent-child relationship

    // Materials
    private Material crossSectionMaterial;
    private Material crossSectionTransparentMaterial;

    // Model Tracking for Late Join Sync
    private Dictionary<int, ModelSyncData> activeModels = new Dictionary<int, ModelSyncData>();

    // Class to store model data for synchronization
    [System.Serializable]
    private class ModelSyncData
    {
        public int viewID;
        public string modelUrl;
        public string textureUrl;
        public bool isTransparent;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Mesh cachedMesh; // Store the mesh for direct application
    }

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Model Loader Ready.");

        // Load materials from Resources folder
        crossSectionMaterial = Resources.Load<Material>("Cross-Section-Material");
        crossSectionTransparentMaterial = Resources.Load<Material>("Cross-Section-Material_Transparent");

        if (crossSectionMaterial == null || crossSectionTransparentMaterial == null)
        {
            Debug.LogError("One or both materials not found in Resources folder.");
        }

        // Register event handlers for Photon destruction
        PhotonNetwork.NetworkingClient.EventReceived += OnPhotonEvent;

    }

    void OnDestroy()
    {
        // Unregister event handlers when this component is destroyed
        PhotonNetwork.NetworkingClient.EventReceived -= OnPhotonEvent;
    }

    private void OnPhotonEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        const byte EVENT_CODE_DESTROY = 204; // PUN internal: object destroy event

        if (photonEvent.Code == EVENT_CODE_DESTROY)
        {
            int viewID = (int)((object[])photonEvent.CustomData)[0];

            if (activeModels.ContainsKey(viewID))
            {
                Debug.Log($"Removing destroyed model with ViewID: {viewID}");
                activeModels.Remove(viewID);
            }
        }
    }


    void Start()
    {
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

    /*public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("ModelRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }*/

    // New method - called when player joins the room
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room - requesting active models from Master Client");

        // Only request model data if we're not the master client and there are other players
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            // Request active models from master client
            photonView.RPC("RequestActiveModels", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    // New method - request active models when joining late
    [PunRPC]
    void RequestActiveModels(int requestingPlayerActorNumber)
    {
        Debug.Log($"Received request for active models from player {requestingPlayerActorNumber}");

        // Master client sends all active model data directly to the requesting player
        foreach (var modelData in activeModels.Values)
        {
            // Use player's actor number to target the specific player
            Player requestingPlayer = PhotonNetwork.CurrentRoom.GetPlayer(requestingPlayerActorNumber);
            if (requestingPlayer != null)
            {
                photonView.RPC("ReceiveModelData", requestingPlayer,
                    modelData.viewID,
                    modelData.modelUrl,
                    modelData.textureUrl,
                    modelData.isTransparent,
                    modelData.position,
                    modelData.rotation,
                    modelData.scale);
            }
        }
    }

    // New method - receive model data for late joiners
    [PunRPC]
    void ReceiveModelData(int viewID, string modelUrl, string textureUrl, bool isTransparent,
                         Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Debug.Log($"Late join: Received model data for viewID: {viewID}");

        // Check if this model already exists in our scene
        PhotonView existingView = PhotonView.Find(viewID);
        if (existingView != null)
        {
            Debug.Log($"Model with viewID {viewID} already exists - updating properties");
            existingView.transform.position = position;
            existingView.transform.rotation = rotation;
            existingView.transform.localScale = scale;

            // Re-download and apply the mesh/texture, as it might be missing
            currentModelUrl = modelUrl;
            currentTextureUrl = textureUrl;
            currentIsTransparent = isTransparent;
            currentModelViewID = viewID;
            StartCoroutine(DownloadAndApplyMesh(viewID, modelUrl));
        }
        else
        {
            Debug.Log($"Creating new model instance for late join with viewID: {viewID}");

            // We need to synchronize the ViewID exactly with what the master is expecting
            // First, create the object through PhotonNetwork to generate the proper viewID
            GameObject modelInstance = PhotonNetwork.InstantiateRoomObject(prefabWithMeshFilter.name, position, rotation);

            // Force the ViewID to match the master's expected ID
            PhotonView newView = modelInstance.GetComponent<PhotonView>();
            if (newView != null)
            {
                // We can't directly set ViewID, so we'll track it in our own system
                Debug.Log($"Created instance with ViewID: {newView.ViewID} (tracking as master's ViewID: {viewID})");

                // Store in a lookup table for later reference
                if (!activeModels.ContainsKey(viewID))
                {
                    ModelSyncData syncData = new ModelSyncData
                    {
                        viewID = viewID,
                        modelUrl = modelUrl,
                        textureUrl = textureUrl,
                        isTransparent = isTransparent,
                        position = position,
                        rotation = rotation,
                        scale = scale
                    };
                    activeModels.Add(viewID, syncData);
                }

                // Set up PhotonTransformView for syncing transforms
                PhotonTransformView transformView = modelInstance.GetComponent<PhotonTransformView>();
                if (transformView == null)
                {
                    transformView = modelInstance.AddComponent<PhotonTransformView>();
                }
                transformView.m_SynchronizePosition = true;
                transformView.m_SynchronizeRotation = true;
                transformView.m_SynchronizeScale = true;

                // Set transform values
                modelInstance.transform.position = position;
                modelInstance.transform.rotation = rotation;
                modelInstance.transform.localScale = scale;

                // Store the viewID for reference during downloads
                currentModelViewID = newView.ViewID; // Use the actual ViewID for download

                // Start mesh download and application
                currentModelUrl = modelUrl;
                currentTextureUrl = textureUrl;
                currentIsTransparent = isTransparent;
                StartCoroutine(DownloadAndApplyMesh(newView.ViewID, modelUrl));
            }
            else
            {
                Debug.LogError("PhotonView component missing on prefab!");
            }
        }
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
        string url = "https://medispherexr.com/api/src/api/model/view_models.php";

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

                    foreach (ModelData model in modelListResponse.models)
                    {
                        if (model != null && !string.IsNullOrEmpty(model.name))
                        {
                            AddModelNameToUI(model.id, model.name, model.is_transparent, model.is_child);
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

    private void AddModelNameToUI(string modelId, string modelName, bool isTransparent, bool isChild)
    {
        if (textPrefab != null)
        {
            GameObject newText = Instantiate(textPrefab, contentPanel.transform);
            TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();
            Button button = newText.GetComponent<Button>();

            if (textComponent != null && button != null)
            {
                textComponent.text = modelName;
                button.onClick.AddListener(() => OnModelNameClicked(modelId, modelName, isTransparent, isChild));
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

    private void OnModelNameClicked(string modelId, string modelName, bool isTransparent, bool isChild)
    {
        // Show downloading UI
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = $"Downloading {modelName}...";
        }
        if (gif != null)
        {
            gif.SetActive(true);
        }

        // Construct the model and texture URLs
        /*currentModelUrl = "http://192.168.1.26/medisphere_api/src/api/model/download_model.php?model_id=" + modelId;
      currentTextureUrl = "http://192.168.1.26/medisphere_api/src/api/model/download_texture.php?model_id=" + modelId;*/

        currentModelUrl = "https://medispherexr.com/api/src/api/model/download_model.php?model_id=" + modelId;
        currentTextureUrl = "https://medispherexr.com/api/src/api/model/download_texture.php?model_id=" + modelId;
        currentModelName = modelName;
        currentIsTransparent = isTransparent;
        currentIsChild = isChild;
        // Start the download locally for the user initiating the action
        StartCoroutine(DownloadAndLoadModel());

        // Synchronize the model download with all other users
        photonView.RPC("RPC_LoadModelFromUrl", RpcTarget.OthersBuffered, currentModelUrl, currentTextureUrl, currentIsTransparent, currentIsChild);
    }

    [PunRPC]
    void RPC_LoadModelFromUrl(string url, string texUrl, bool isTransparent, bool isChild)
    {
        currentModelUrl = url;
        currentTextureUrl = texUrl;
        currentIsTransparent = isTransparent;

        currentIsChild = isChild;
        StartCoroutine(DownloadAndLoadModel());
    }
    //chnagesssssssss//////////////////////////////

    public void LoadModel(string modelId, string modelName, bool isTransparent, bool isChild)           ///////////newwwww
    {
        OnModelNameClicked(modelId, modelName, isTransparent, isChild);
    }
    IEnumerator DownloadAndLoadModel()
    {
        Debug.Log("Starting Model Download...");
        string uniqueModelUrl = currentModelUrl + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Model Download Failed: " + request.error);
                UpdateDownloadUI(false);
                yield break;
            }

            Debug.Log("Model Downloaded Successfully!");

            // Determine file extension
            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }
            else
            {
                Debug.LogError("Could not determine file extension from response.");
                yield break;
            }

            string uniqueFilename = "model_" + DateTime.Now.Ticks + fileExtension;
            string modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            // File.WriteAllBytes(modelFilePath, request.downloadHandler.data);
            System.IO.File.WriteAllBytes(modelFilePath, request.downloadHandler.data);         ///////////////change

            yield return new WaitForSeconds(1);
            LoadModel(modelFilePath);
        }
    }
    //chnagesssss////////////////////////////////////////////////////////////////






    void LoadModel(string modelFilePath)
    {
        /*if (!File.Exists(modelFilePath))
        {
            Debug.LogError("File not found at: " + modelFilePath);
            UpdateDownloadUI(false);
            return;
        }*/
        if (!System.IO.File.Exists(modelFilePath)) // Fix
        {
            Debug.LogError("File not found at: " + modelFilePath);          /// change
            UpdateDownloadUI(false);
            return;
        }
        Debug.Log("Loading Model from: " + modelFilePath);

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);

        AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, null, null, OnError, null, assetLoaderOptions);
    }





    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model Loaded Successfully!");

        GameObject loadedModel = assetLoaderContext.RootGameObject;
        if (loadedModel == null)
        {
            Debug.LogError("Loaded model is null!");
            UpdateDownloadUI(false);
            return;
        }

        MeshFilter downloadedMeshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
        if (downloadedMeshFilter == null || downloadedMeshFilter.sharedMesh == null)
        {
            Debug.LogError("No valid MeshFilter found in downloaded model.");
            Destroy(loadedModel);
            UpdateDownloadUI(false);
            return;
        }

        // Instantiate the prefab on the network only if it's the local player's action
        if (photonView.IsMine)
        {
            // Create network instance of the prefab at the spawn point
            GameObject modelInstance = PhotonNetwork.InstantiateRoomObject(prefabWithMeshFilter.name, spawnPoint.position, Quaternion.Euler(-90f, 0f, 0f));  ///////////
            currentModelViewID = modelInstance.GetComponent<PhotonView>().ViewID;

            // Apply mesh and material to the prefab
            AssignMeshAndMaterialToPrefab(loadedModel, modelInstance, downloadedMeshFilter.sharedMesh);

            // Store model data for late joiners
            StoreModelData(modelInstance);

            if (currentIsChild && SelectionManager.Instance.GetSelectedModel() != null)

            {

                modelInstance.transform.SetParent(SelectionManager.Instance.GetSelectedModel().transform);

                Debug.Log($"Model {currentModelName} set as child of {SelectionManager.Instance.GetSelectedModel().name}");

            }
            // Broadcast to all clients that a new model has been created
            photonView.RPC("SyncMeshData", RpcTarget.OthersBuffered, currentModelViewID, currentModelUrl, currentTextureUrl, currentIsTransparent, currentIsChild);

            // Download and apply texture to our own instance
            StartCoroutine(DownloadAndApplyTexture(currentModelViewID));

            // Log model creation with viewID
            Debug.Log($"Created new model with ViewID: {currentModelViewID}");



            // Add this after line where you store model data (around line 348 in your OnLoad method)
            // Right after: StoreModelData(modelInstance);

            // Add this after StoreModelData(modelInstance); in your OnLoad method
            // Add after StoreModelData(modelInstance);
            // In OnLoad method, after: StoreModelData(modelInstance);
            SessionManager sessionManager = FindObjectOfType<SessionManager>();
            if (sessionManager != null)
            {
                sessionManager.OnServerModelLoaded(modelInstance, currentModelName, currentModelName, currentIsTransparent, currentIsChild);
                Debug.Log($"Notified SessionManager for model: {currentModelName}, ID: {currentModelName}");
            }
            else
            {
                Debug.LogError("SessionManager not found for registering server model!");
            }
        }

        Destroy(loadedModel);
        UpdateDownloadUI(true);
    }




    private void StoreModelData(GameObject modelInstance)
    {
        if (modelInstance == null) return;

        PhotonView modelView = modelInstance.GetComponent<PhotonView>();
        if (modelView == null) return;

        int viewID = modelView.ViewID;

        // Get the mesh if possible
        MeshFilter meshFilter = modelInstance.GetComponent<MeshFilter>();
        Mesh cachedMesh = null;
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            cachedMesh = meshFilter.sharedMesh;
        }

        // Create and store model data
        ModelSyncData modelData = new ModelSyncData
        {
            viewID = viewID,
            modelUrl = currentModelUrl,
            textureUrl = currentTextureUrl,
            isTransparent = currentIsTransparent,
            position = modelInstance.transform.position,
            rotation = modelInstance.transform.rotation,
            scale = modelInstance.transform.localScale,
            cachedMesh = cachedMesh
        };

        // Store or update model data
        if (activeModels.ContainsKey(viewID))
        {
            activeModels[viewID] = modelData;
        }
        else
        {
            activeModels.Add(viewID, modelData);
        }

        Debug.Log($"Stored model data for viewID: {viewID}. Total active models: {activeModels.Count}");
    }

    private void UpdateDownloadUI(bool success)
    {
        if (downloadingMessage != null)
        {
            downloadingMessage.gameObject.SetActive(true);
            downloadingMessage.text = success
                ? $"{currentModelName} downloaded successfully!"
                : $"Failed to download {currentModelName}";
            Debug.Log($"UI: Set message to '{downloadingMessage.text}'");
            // Reset flag after download completes

            StartCoroutine(HideDownloadUI());
        }
        else
        {
            Debug.LogError("downloadingMessage is null in UpdateDownloadUI!");
        }

        if (gif != null)
        {
            gif.SetActive(false);
            Debug.Log("UI: Loading gif deactivated");
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
    void SyncMeshData(int viewID, string url, string texUrl, bool isTransparent, bool isChild)
    {
        Debug.Log($"Syncing mesh data for ViewID {viewID}");
        currentModelViewID = viewID;
        currentModelUrl = url;
        currentTextureUrl = texUrl;
        currentIsTransparent = isTransparent;
        currentIsChild = isChild;

        // Try to find existing model
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView != null)
        {
            GameObject modelInstance = modelView.gameObject;

            // Start downloading and applying mesh
            StartCoroutine(DownloadAndApplyMesh(viewID, url));

            // Create and store model data
            ModelSyncData modelData = new ModelSyncData
            {
                viewID = viewID,
                modelUrl = url,
                textureUrl = texUrl,
                isTransparent = isTransparent,
                position = modelInstance.transform.position,
                rotation = modelInstance.transform.rotation,
                scale = modelInstance.transform.localScale,
                cachedMesh = null // Will be populated after download
            };

            // Store or update model data
            if (activeModels.ContainsKey(viewID))
            {
                activeModels[viewID] = modelData;
            }
            else
            {
                activeModels.Add(viewID, modelData);
            }

            Debug.Log($"Added/updated model data for viewID: {viewID}");
        }
        else
        {
            Debug.LogError($"Failed to find PhotonView with ViewID: {viewID}");
        }
    }

    private IEnumerator DownloadAndApplyMesh(int viewID, string url)
    {
        PhotonView modelView = PhotonView.Find(viewID);
        if (modelView == null)
        {
            Debug.LogError("Model with ViewID " + viewID + " not found.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download model: " + request.error);
                yield break;
            }

            string uniqueFilename = "model_sync_" + DateTime.Now.Ticks + fileExtension;
            string syncModelPath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(syncModelPath, request.downloadHandler.data);

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(syncModelPath, (context) =>
            {
                GameObject loadedModel = context.RootGameObject;
                if (loadedModel != null)
                {
                    MeshFilter meshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        AssignMeshAndMaterialToPrefab(loadedModel, modelView.gameObject, meshFilter.sharedMesh);


                        if (currentIsChild && SelectionManager.Instance.GetSelectedModel() != null)

                        {

                            modelView.gameObject.transform.SetParent(SelectionManager.Instance.GetSelectedModel().transform);

                            Debug.Log($"Synced model set as child of {SelectionManager.Instance.GetSelectedModel().name}");

                        }
                        // Apply texture after mesh is assigned
                        StartCoroutine(DownloadAndApplyTexture(viewID));
                    }
                    Destroy(loadedModel);
                }
            }, null, null, OnError, null, assetLoaderOptions);
        }

    }

    private void AssignMeshAndMaterialToPrefab(GameObject sourceModel, GameObject targetModel, Mesh sourceMesh)
    {
        MeshFilter targetMeshFilter = targetModel.GetComponent<MeshFilter>();
        if (targetMeshFilter == null)
        {
            Debug.LogError("Target model does not have a MeshFilter.");
            return;
        }

        // Create a resized copy of the mesh
        Mesh resizedMesh = Instantiate(sourceMesh);
        ResizeMeshToFit(resizedMesh, desiredModelSize);

        // Apply the mesh to the target
        targetMeshFilter.mesh = resizedMesh;

        // Apply the appropriate material based on is_transparent
        MeshRenderer targetRenderer = targetModel.GetComponent<MeshRenderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("Target model does not have a MeshRenderer.");
            return;
        }

        Material selectedMaterial = currentIsTransparent ? crossSectionTransparentMaterial : crossSectionMaterial;
        if (selectedMaterial == null)
        {
            Debug.LogError("Selected material is null.");
            return;
        }

        targetRenderer.material = selectedMaterial;
        Debug.Log($"Applied material: {(currentIsTransparent ? "CrossSection-Transparent" : "CrossSection")} to model.");

        // Configure PhotonTransformView for synchronization
        PhotonTransformView photonTransformView = targetModel.GetComponent<PhotonTransformView>();
        if (photonTransformView == null)
        {
            photonTransformView = targetModel.AddComponent<PhotonTransformView>();
        }
        photonTransformView.m_SynchronizePosition = true;
        photonTransformView.m_SynchronizeRotation = true;
        photonTransformView.m_SynchronizeScale = true;

        // Adjust collider to fit the mesh
        AdjustColliderToMesh(targetModel, resizedMesh);

        // Add this at the end of AssignMeshAndMaterialToPrefab method
       

    }

    private IEnumerator DownloadAndApplyTexture(int viewID)
    {
        // Add delay to ensure model is loaded first
        yield return new WaitForSeconds(1f);

        if (string.IsNullOrEmpty(currentTextureUrl))
        {
            Debug.Log("No texture URL provided.");
            yield break;
        }

        Debug.Log("Downloading texture for viewID: " + viewID + " from URL: " + currentTextureUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(currentTextureUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download texture: " + request.error);
                yield break;
            }

            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log($"Texture downloaded successfully: {downloadedTexture.width}x{downloadedTexture.height}");

            PhotonView modelView = PhotonView.Find(viewID);
            if (modelView == null)
            {
                Debug.LogError("Model with ViewID " + viewID + " not found for texture application.");
                yield break;
            }

            MeshRenderer renderer = modelView.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material material = renderer.material; // Get the current material (CrossSection or CrossSection-Transparent)
                ApplyTextureToMaterial(material, downloadedTexture);
                renderer.material = material;
            }
            else
            {
                Debug.LogError("No renderer found on model with ViewID: " + viewID);
            }
        }

    }

    private void ApplyTextureToMaterial(Material mat, Texture2D texture)
    {
        // Try different common texture property names
        string[] texturePropertyNames = new string[]
        {
            "_Texture2D", "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap"
        };

        foreach (string propName in texturePropertyNames)
        {
            if (mat.HasProperty(propName))
            {
                mat.SetTexture(propName, texture);
                Debug.Log($"Applied texture to material '{mat.name}' using property '{propName}'");
                return;
            }
        }

        Debug.LogWarning($"Could not find appropriate texture property on material '{mat.name}'");
    }

    private void ResizeMeshToFit(Mesh mesh, float desiredModelSize)
    {
        if (mesh == null) return;

        Bounds bounds = mesh.bounds;
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = desiredModelSize / maxDimension;

        if (currentModelUrl.Contains("199") || currentModelUrl.Contains("253")) // Liver modelId
        {
            scaleFactor *= 0.8f; // Adjust to match combined model's liver size
        }

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void AdjustColliderToMesh(GameObject model, Mesh mesh)
    {
        BoxCollider collider = model.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = model.AddComponent<BoxCollider>();
        }

        // collider.center = mesh.bounds.center;
        //collider.size = mesh.bounds.size;


        if (currentModelUrl.Contains("199") || currentModelUrl.Contains("253"))
        {
            collider.center = new Vector3(0.016418f, -0.118874f, -0.005252f);
            collider.size = new Vector3(0.320679f, 0.1315272f, 0.374281f);
        }
        else
        {
            collider.center = mesh.bounds.center;
            collider.size = mesh.bounds.size;
        }
    }

    // Cleanup when object is destroyed or player leaves
    public override void OnLeftRoom()
    {
        // Clear active models list
        activeModels.Clear();
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        Debug.LogError($"Error Loading Model: {contextualizedError.GetInnerException()}");
        UpdateDownloadUI(false);

    }

    [System.Serializable]
    public class ModelData
    {
        public string id;
        public string name;
        public bool is_transparent; // Field for transparency
        public bool is_child;
    }

    [System.Serializable]
    public class ModelListResponse
    {
        public ModelData[] models;
    }




    [PunRPC]
    void RequestDirectMeshTransfer(int viewID, int requestingActorNumber)
    {
        Debug.Log($"Direct mesh transfer requested for viewID: {viewID}");

        // Check if we have this model with a cached mesh
        if (activeModels.ContainsKey(viewID) && activeModels[viewID].cachedMesh != null)
        {
            // We can't directly send the mesh over RPC, so we'll just notify the player to download it
            PhotonView requesterView = null;

            // Find the requesting player's PhotonView on this ModelSample component
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (player.ActorNumber == requestingActorNumber)
                {
                    photonView.RPC("ForceModelDownload", player, viewID,
                        activeModels[viewID].modelUrl,
                        activeModels[viewID].textureUrl,
                        activeModels[viewID].isTransparent,
                        activeModels[viewID].position,
                        activeModels[viewID].rotation,
                        activeModels[viewID].scale);
                    break;
                }
            }
        }
    }

    [PunRPC]
    void ForceModelDownload(int viewID, string modelUrl, string textureUrl, bool isTransparent,
                           Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Debug.Log($"Force downloading model for viewID: {viewID}");

        // Set current values for download
        currentModelViewID = viewID;
        currentModelUrl = modelUrl;
        currentTextureUrl = textureUrl;
        currentIsTransparent = isTransparent;

        // Get the actual GameObject this refers to
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            // Set transform values directly
            targetView.transform.position = position;
            targetView.transform.rotation = rotation;
            targetView.transform.localScale = scale;

            // Force redownload the mesh and texture
            StartCoroutine(ForceRedownloadMesh(viewID, modelUrl, textureUrl, isTransparent));
        }
        else
        {
            Debug.LogError($"Cannot find PhotonView with ID {viewID} for forced download");
        }
    }

    private IEnumerator ForceRedownloadMesh(int viewID, string url, string texUrl, bool isTransparent)
    {
        Debug.Log($"Starting forced model download for viewID: {viewID}");

        // Add a unique parameter to avoid cache
        string uniqueModelUrl = url + "&timestamp=" + DateTime.Now.Ticks;

        using (UnityWebRequest request = UnityWebRequest.Get(uniqueModelUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Force download failed: {request.error}");
                yield break;
            }

            // Get the file extension
            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("filename="))
            {
                string fileName = contentDisposition.Split(new[] { "filename=" }, StringSplitOptions.None)[1].Trim('"');
                fileExtension = Path.GetExtension(fileName);
            }

            // Save to disk
            string uniqueFilename = "model_force_" + viewID + "_" + DateTime.Now.Ticks + fileExtension;
            string modelFilePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
            File.WriteAllBytes(modelFilePath, request.downloadHandler.data);

            // Load the model
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true, true);
            AssetLoader.LoadModelFromFile(modelFilePath, (context) =>
            {
                GameObject loadedModel = context.RootGameObject;
                if (loadedModel != null)
                {
                    MeshFilter meshFilter = loadedModel.GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        // Find the target view again (in case it changed)
                        PhotonView targetView = PhotonView.Find(viewID);
                        if (targetView != null)
                        {
                            // Apply mesh and material
                            AssignMeshAndMaterialToPrefab(loadedModel, targetView.gameObject, meshFilter.sharedMesh);

                            // Now download and apply texture
                            StartCoroutine(DownloadAndApplyTexture(viewID));

                            Debug.Log($"Successfully force-applied mesh to viewID: {viewID}");
                        }
                    }
                    Destroy(loadedModel);
                }
            }, null, null, OnError, null, assetLoaderOptions);
        }

    }




    public string CurrentModelUrl => currentModelUrl;
    public string CurrentTextureUrl => currentTextureUrl;







}

