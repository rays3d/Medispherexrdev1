/*using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class ModelData
{
    public string modelPath;       // "Models/Skull" for local, "ModelPrefab" for server
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool isServerModel;     // true for server models
}

[Serializable]
public class SessionData
{
    public List<ModelData> models = new List<ModelData>();
}

public class SessionManager : MonoBehaviour
{
    private const string modelTag = "ModelPart";

    // Cached server meshes and materials
    private Dictionary<string, Mesh> serverMeshes = new Dictionary<string, Mesh>();
    private Dictionary<string, Material> serverMaterials = new Dictionary<string, Material>();

    // -----------------
    // Import Local Model
    // -----------------
    public GameObject ImportLocalModel(string prefabName)
    {
        string path = "Models/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Local prefab not found: " + path);
            return null;
        }

        GameObject model = Instantiate(prefab);
        model.name = prefab.name; // remove (Clone)
        model.tag = modelTag;
        return model;
    }

    // -----------------
    // Import Server Model
    // -----------------
    public GameObject ImportServerModel(string prefabName, Mesh downloadedMesh, Material downloadedMaterial)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName); // prefab directly under Resources
        if (prefab == null)
        {
            Debug.LogError("Server prefab not found: " + prefabName);
            return null;
        }

        GameObject model = Instantiate(prefab);
        model.name = prefab.name;
        model.tag = modelTag;

        // Assign downloaded mesh and material
        MeshFilter mf = model.GetComponent<MeshFilter>();
        if (mf != null) mf.mesh = downloadedMesh;

        Renderer rend = model.GetComponent<Renderer>();
        if (rend != null) rend.material = downloadedMaterial;

        // Cache mesh/material for reload
        serverMeshes[prefabName] = downloadedMesh;
        serverMaterials[prefabName] = downloadedMaterial;

        return model;
    }

    // -----------------
    // Save Session
    // -----------------
    public void SaveSession(string saveFileName)
    {
        SessionData session = new SessionData();
        GameObject[] models = GameObject.FindGameObjectsWithTag(modelTag);

        foreach (var model in models)
        {
            string prefabName = model.name.Replace("(Clone)", "");
            bool isServer = serverMeshes.ContainsKey(prefabName);
            string path = isServer ? prefabName : "Models/" + prefabName;

            ModelData data = new ModelData
            {
                modelPath = path,
                position = model.transform.position,
                rotation = model.transform.rotation,
                scale = model.transform.localScale,
                tag = model.tag,
                isServerModel = isServer
            };

            session.models.Add(data);
        }

        string json = JsonUtility.ToJson(session, true);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName + ".json", json);
        Debug.Log("Session Saved: " + Application.persistentDataPath + "/" + saveFileName + ".json");
    }

    // -----------------
    // Load Session
    // -----------------
    public void LoadSession(string saveFileName)
    {
        string path = Application.persistentDataPath + "/" + saveFileName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        SessionData session = JsonUtility.FromJson<SessionData>(json);

        // Destroy existing models
        GameObject[] existingModels = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in existingModels)
            Destroy(model);

        // Load each model
        foreach (var modelData in session.models)
        {
            GameObject prefab = Resources.Load<GameObject>(modelData.modelPath);
            GameObject model;

            if (prefab != null)
            {
                model = Instantiate(prefab);
                model.name = prefab.name;
            }
            else
            {
                model = GameObject.CreatePrimitive(PrimitiveType.Cube);
                model.name = modelData.modelPath;
                Debug.LogWarning("Prefab not found: " + modelData.modelPath);
            }

            model.transform.position = modelData.position;
            model.transform.rotation = modelData.rotation;
            model.transform.localScale = modelData.scale;
            model.tag = modelData.tag;

            // Restore server mesh/material
            if (modelData.isServerModel && serverMeshes.ContainsKey(model.name))
            {
                MeshFilter mf = model.GetComponent<MeshFilter>();
                if (mf != null) mf.mesh = serverMeshes[model.name];

                Renderer rend = model.GetComponent<Renderer>();
                if (rend != null) rend.material = serverMaterials[model.name];
            }
        }

        Debug.Log("Session Loaded: " + session.models.Count + " models");
    }
}


*/

////////////////////////////////////////////////////////////////////////////////runtime works//////////////////////////////////////////////

/*using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
[Serializable]
public class ModelData
{
    public string modelPath; // "Models/Skull" for local, "ModelPrefab" for server
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool isServerModel; // true for server models
    public string modelId; // Store the server model ID for re-downloading
    public string modelName; // Store the model name
    public bool isTransparent; // Store transparency state
    public bool isChild; // Store child state
}
[Serializable]
public class SessionData
{
    public List<ModelData> models = new List<ModelData>();
}
public class SessionManager : MonoBehaviour
{
    private const string modelTag = "ModelPart";
    [Header("References")]
    public ModelSample modelSample; // Reference to your ModelSample script
    // Simple runtime cache (exactly like your original working system)
    private Dictionary<string, ServerModelCache> serverModelCache = new Dictionary<string, ServerModelCache>();
    private class ServerModelCache
    {
        public Mesh mesh;
        public Material material;
        public string modelId;
        public string modelName;
        public bool isTransparent;
        public bool isChild;
    }
    void Start()
    {
        if (modelSample == null)
        {
            modelSample = FindObjectOfType<ModelSample>();
        }
    }
    // -----------------
    // Import Local Model (unchanged)
    // -----------------
    public GameObject ImportLocalModel(string prefabName)
    {
        string path = "Models/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Local prefab not found: " + path);
            return null;
        }
        GameObject model = Instantiate(prefab);
        model.name = prefab.name; // remove (Clone)
        model.tag = modelTag;
        return model;
    }
    // -----------------
    // Cache Server Model (unchanged - keeps your working system)
    // -----------------
    public void RegisterServerModel(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild)
    {
        if (model == null) return;
        string modelKey = model.name.Replace("(Clone)", "");
        MeshFilter mf = model.GetComponent<MeshFilter>();
        Renderer rend = model.GetComponent<Renderer>();
        if (mf?.mesh == null || rend?.material == null) return;
        ServerModelCache cache = new ServerModelCache
        {
            mesh = mf.mesh,
            material = rend.material,
            modelId = modelId,
            modelName = modelName,
            isTransparent = isTransparent,
            isChild = isChild
        };
        serverModelCache[modelKey] = cache;
        Debug.Log($"Registered server model: {modelKey} with ID: {modelId}");
    }
    // -----------------
    // Save Session (enhanced to save server model info)
    // -----------------
    public void SaveSession(string saveFileName)
    {
        SessionData session = new SessionData();
        GameObject[] models = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in models)
        {
            string modelKey = model.name.Replace("(Clone)", "");
            bool isServer = serverModelCache.ContainsKey(modelKey);
            ModelData data = new ModelData
            {
                position = model.transform.position,
                rotation = model.transform.rotation,
                scale = model.transform.localScale,
                tag = model.tag,
                isServerModel = isServer,
                modelPath = isServer ? modelKey : "Models/" + modelKey
            };
            if (isServer)
            {
                ServerModelCache cache = serverModelCache[modelKey];
                data.modelId = cache.modelId;
                data.modelName = cache.modelName;
                data.isTransparent = cache.isTransparent;
                data.isChild = cache.isChild;
                Debug.Log($"Saving server model: {data.modelName}, ID: {data.modelId}, Key: {modelKey}");
            }
            else
            {
                data.modelId = "";
                data.modelName = modelKey;
                data.isTransparent = false;
                data.isChild = false;
                Debug.Log($"Saving local model: {data.modelName}");
            }
            session.models.Add(data);
        }
        string json = JsonUtility.ToJson(session, true);
        string filePath = Application.persistentDataPath + "/" + saveFileName + ".json";
        File.WriteAllText(filePath, json);
        Debug.Log($"Session saved: {filePath} with {session.models.Count} models");
    }
    // -----------------
    // Load Session (the key fix - handles missing cache by re-downloading)
    // -----------------
    public void LoadSession(string saveFileName)
    {
        string path = Application.persistentDataPath + "/" + saveFileName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found: " + path);
            return;
        }
        string json = File.ReadAllText(path);
        SessionData session = JsonUtility.FromJson<SessionData>(json);
        // Destroy existing models
        GameObject[] existingModels = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in existingModels)
        {
            Destroy(model);
        }
        Debug.Log($"Loading session with {session.models.Count} models...");
        StartCoroutine(LoadModelsCoroutine(session.models));
    }
    private IEnumerator LoadModelsCoroutine(List<ModelData> models)
    {
        yield return null; // Wait for cleanup
        foreach (var modelData in models)
        {
            if (modelData.isServerModel)
            {
                string modelKey = modelData.modelPath;
                // Check if we have cached data (works if not quit)
                if (serverModelCache.ContainsKey(modelKey))
                {
                    // Load from cache - your original working system
                    LoadServerModelFromCache(modelData, modelKey);
                }
                else
                {
                    // Re-download if cache is missing (after quit)
                    yield return StartCoroutine(RedownloadServerModel(modelData));
                }
            }
            else
            {
                // Load local model - unchanged
                LoadLocalModel(modelData);
            }
            yield return new WaitForSeconds(0.1f); // Small delay between models
        }
        Debug.Log("Session loading completed");
    }
    private void LoadLocalModel(ModelData modelData)
    {
        GameObject prefab = Resources.Load<GameObject>(modelData.modelPath);
        if (prefab != null)
        {
            GameObject model = Instantiate(prefab);
            model.name = prefab.name;
            ApplyTransform(model, modelData);
            Debug.Log($"Loaded local model: {model.name}");
        }
        else
        {
            Debug.LogWarning($"Local prefab not found: {modelData.modelPath}");
        }
    }
    private void LoadServerModelFromCache(ModelData modelData, string modelKey)
    {
        ServerModelCache cache = serverModelCache[modelKey];
        // Load base prefab
        GameObject prefab = Resources.Load<GameObject>(modelKey);
        if (prefab == null)
        {
            Debug.LogError($"Base prefab not found: {modelKey}");
            return;
        }
        GameObject model = Instantiate(prefab);
        model.name = prefab.name;
        model.tag = modelTag;
        // Apply cached mesh and material (your original working method)
        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = cache.mesh;
        }
        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = cache.material;
        }
        ApplyTransform(model, modelData);
        Debug.Log($"Loaded server model from cache: {model.name}");
    }
    private IEnumerator RedownloadServerModel(ModelData modelData)
    {
        Debug.Log($"=== REDOWNLOAD DEBUG ===");
        Debug.Log($"ModelSample null? {modelSample == null}");
        Debug.Log($"Model ID: '{modelData.modelId}'");
        Debug.Log($"Model Name: '{modelData.modelName}'");
        Debug.Log($"Is Transparent: {modelData.isTransparent}");
        Debug.Log($"Is Child: {modelData.isChild}");
        if (modelSample == null || string.IsNullOrEmpty(modelData.modelId))
        {
            Debug.LogError($"Cannot redownload server model: ModelSample={modelSample}, ModelId='{modelData.modelId}'");
            yield break;
        }
        Debug.Log($"Calling ModelSample.LoadModel with ID: {modelData.modelId}");
        // Track expected model for positioning
        var expectedModel = modelData;
        // Use your existing ModelSample download system
        modelSample.LoadModel(modelData.modelId, modelData.modelName, modelData.isTransparent, modelData.isChild);
        // Wait for download to complete
        float timeout = 20f;
        float elapsed = 0f;
        GameObject foundModel = null;
        Debug.Log($"Waiting for model download... Looking for modelPath: {modelData.modelPath}");
        while (elapsed < timeout && foundModel == null)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
            Debug.Log($"Searching... Elapsed: {elapsed}s");
            // Look for the newly downloaded model
            GameObject[] currentModels = GameObject.FindGameObjectsWithTag(modelTag);
            Debug.Log($"Found {currentModels.Length} models with tag '{modelTag}'");
            foreach (var model in currentModels)
            {
                string modelKey = model.name.Replace("(Clone)", "");
                Debug.Log($"Checking model: {model.name} -> Key: {modelKey} (Looking for: {modelData.modelPath})");
                if (modelKey == modelData.modelPath)
                {
                    MeshFilter mf = model.GetComponent<MeshFilter>();
                    Debug.Log($"Found matching model! MeshFilter: {mf != null}, Mesh: {mf?.mesh != null}, Vertices: {mf?.mesh?.vertexCount}");
                    if (mf != null && mf.mesh != null && mf.mesh.vertexCount > 0)
                    {
                        foundModel = model;
                        Debug.Log("? Model found with valid mesh!");
                        break;
                    }
                }
            }
        }
        if (foundModel != null)
        {
            // Apply saved transform
            ApplyTransform(foundModel, expectedModel);
            Debug.Log($"? Successfully redownloaded and positioned: {foundModel.name}");
            // Cache it for this session
            RegisterServerModel(foundModel, modelData.modelId, modelData.modelName,
modelData.isTransparent, modelData.isChild);
        }
        else
        {
            Debug.LogError($"? Failed to redownload server model: {modelData.modelName} after {elapsed}s");
        }
    }
    private void ApplyTransform(GameObject model, ModelData data)
    {
        model.transform.position = data.position;
        model.transform.rotation = data.rotation;
        model.transform.localScale = data.scale;
        model.tag = data.tag;
    }
    // -----------------
    // Public Interface (unchanged)
    // -----------------
    public void OnServerModelLoaded(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild)
    {
        RegisterServerModel(model, modelId, modelName, isTransparent, isChild);
    }
    public void SaveCurrentSession()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        SaveSession($"Session_{timestamp}");
    }
    public void ClearCache()
    {
        serverModelCache.Clear();
        Debug.Log("Server model cache cleared");
    }
    public bool HasCachedModel(string modelKey)
    {
        return serverModelCache.ContainsKey(modelKey);
    }
}




*/

//////////////////////////////redownloading///////////////////////////
/*using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

[Serializable]
public class ModelData
{
    public string modelPath;       // "Models/Skull" for local, prefab name for server
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool isServerModel;     // true for server models
    public string modelId;         // Server model ID for re-downloading
    public string modelName;       // Model name
    public bool isTransparent;     // Transparency state
    public bool isChild;           // Child state
    public int photonViewID;       // Store PhotonView ID if available

    // Additional fields for server models
    public string modelUrl;        // Full download URL
    public string textureUrl;      // Texture download URL
    public string meshDataPath;    // Path to saved mesh data file
    public string materialDataPath; // Path to saved material data file
}

[Serializable]
public class SessionData
{
    public List<ModelData> models = new List<ModelData>();
}

// Serializable classes for mesh and material data
[Serializable]
public class SerializableMesh
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uv;
    public string name;
}

[Serializable]
public class SerializableMaterial
{
    public string name;
    public Color color;
    public string texturePath; // Path to saved texture file
    public bool isTransparent;
    public string shaderName; // Store shader name
    public Dictionary<string, string> textureProperties; // Store all texture properties
}

public class SessionManager : MonoBehaviour
{
    private const string modelTag = "ModelPart";

    [Header("References")]
    public ModelSample modelSample; // Reference to your ModelSample script

    // Cache for server models - matches your ModelSample approach
    private Dictionary<string, ServerModelCache> serverModelCache = new Dictionary<string, ServerModelCache>();

    [Serializable]
    private class ServerModelCache
    {
        public Mesh mesh;
        public Material material;
        public string modelId;
        public string modelName;
        public bool isTransparent;
        public bool isChild;
        public string modelUrl;
        public string textureUrl;
        public GameObject prefab; // Store the base prefab reference
        public Dictionary<string, Texture2D> cachedTextures; // Cache all textures
    }

    void Start()
    {
        // Auto-find ModelSample if not assigned
        if (modelSample == null)
        {
            modelSample = FindObjectOfType<ModelSample>();
            if (modelSample == null)
            {
                Debug.LogError("ModelSample not found! Please assign it in the inspector.");
            }
        }
    }

    // -----------------
    // Import Local Model
    // -----------------
    public GameObject ImportLocalModel(string prefabName)
    {
        string path = "Models/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Local prefab not found: " + path);
            return null;
        }

        GameObject model = Instantiate(prefab);
        model.name = prefab.name; // remove (Clone)
        model.tag = modelTag;
        return model;
    }

    // -----------------
    // Cache Server Model (enhanced version with proper texture handling)
    // -----------------
    public void CacheServerModel(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild, string modelUrl, string textureUrl)
    {
        if (model == null)
        {
            Debug.LogError("Cannot cache null model");
            return;
        }

        string cacheKey = model.name.Replace("(Clone)", "");

        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        Renderer renderer = model.GetComponent<Renderer>();

        if (meshFilter == null || renderer == null)
        {
            Debug.LogError($"Model {cacheKey} missing MeshFilter or Renderer components");
            return;
        }

        // Find the base prefab
        GameObject basePrefab = Resources.Load<GameObject>(cacheKey);

        // Cache all textures from the material
        Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
        if (renderer.material != null)
        {
            CacheAllMaterialTextures(renderer.material, cachedTextures);
        }

        ServerModelCache cache = new ServerModelCache
        {
            mesh = meshFilter.mesh,
            material = renderer.material,
            modelId = modelId,  // This should be the actual API model ID (like "199")
            modelName = modelName,
            isTransparent = isTransparent,
            isChild = isChild,
            modelUrl = modelUrl,
            textureUrl = textureUrl,
            prefab = basePrefab,
            cachedTextures = cachedTextures
        };

        serverModelCache[cacheKey] = cache;
        Debug.Log($"Cached server model: {cacheKey} with ID: {modelId}, name: {modelName}");
    }

    // Helper method to cache all textures from a material
    private void CacheAllMaterialTextures(Material material, Dictionary<string, Texture2D> textureCache)
    {
        if (material == null || material.shader == null) return;

        // Try common texture property names since ShaderUtil might not be available in runtime
        string[] commonTextureProperties = {
            "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap", "_Texture", "_Tex", "_Texture2D",
            "_EmissionMap", "_BumpMap", "_NormalMap", "_OcclusionMap", "_MetallicGlossMap"
        };

        foreach (string propName in commonTextureProperties)
        {
            if (material.HasProperty(propName))
            {
                Texture texture = material.GetTexture(propName);
                if (texture != null && texture is Texture2D)
                {
                    textureCache[propName] = texture as Texture2D;
                    Debug.Log($"Cached texture property: {propName}");
                }
            }
        }
    }

    // -----------------
    // Save mesh and material data to files
    // -----------------
    private string SaveMeshData(Mesh mesh, string modelKey)
    {
        if (mesh == null) return "";

        SerializableMesh serializableMesh = new SerializableMesh
        {
            vertices = mesh.vertices,
            triangles = mesh.triangles,
            normals = mesh.normals,
            uv = mesh.uv,
            name = mesh.name
        };

        string fileName = $"mesh_{modelKey}_{DateTime.Now.Ticks}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(serializableMesh);
        File.WriteAllText(filePath, json);

        Debug.Log($"Saved mesh data to: {filePath}");
        return fileName;
    }

    private string SaveMaterialData(Material material, string modelKey)
    {
        if (material == null) return "";

        // Save all texture properties
        Dictionary<string, string> textureFiles = new Dictionary<string, string>();
        if (serverModelCache.ContainsKey(modelKey) && serverModelCache[modelKey].cachedTextures != null)
        {
            foreach (var kvp in serverModelCache[modelKey].cachedTextures)
            {
                string textureFileName = SaveTextureData(kvp.Value, $"{modelKey}_{kvp.Key}");
                if (!string.IsNullOrEmpty(textureFileName))
                {
                    textureFiles[kvp.Key] = textureFileName;
                }
            }
        }

        // Get color safely - check if material has color property
        Color materialColor = Color.white; // default
        if (material.HasProperty("_Color"))
        {
            materialColor = material.color;
        }
        else if (material.HasProperty("_BaseColor"))
        {
            materialColor = material.GetColor("_BaseColor");
        }
        else if (material.HasProperty("_MainColor"))
        {
            materialColor = material.GetColor("_MainColor");
        }

        SerializableMaterial serializableMaterial = new SerializableMaterial
        {
            name = material.name,
            color = materialColor,
            texturePath = textureFiles.ContainsKey("_MainTex") ? textureFiles["_MainTex"] :
                         textureFiles.ContainsKey("_Texture2D") ? textureFiles["_Texture2D"] :
                         (textureFiles.Count > 0 ? textureFiles.First().Value : ""), // Use first texture as fallback
            isTransparent = material.name.Contains("Transparent"),
            shaderName = material.shader != null ? material.shader.name : "",
            textureProperties = textureFiles
        };

        string fileName = $"material_{modelKey}_{DateTime.Now.Ticks}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(serializableMaterial);
        File.WriteAllText(filePath, json);

        Debug.Log($"Saved material data to: {filePath}");
        return fileName;
    }

    private string SaveTextureData(Texture2D texture, string modelKey)
    {
        if (texture == null) return "";

        try
        {
            // Make texture readable if it isn't
            Texture2D readableTexture = texture;
            if (!texture.isReadable)
            {
                readableTexture = MakeTextureReadable(texture);
            }

            byte[] textureData = readableTexture.EncodeToPNG();
            string fileName = $"texture_{modelKey}_{DateTime.Now.Ticks}.png";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, textureData);

            Debug.Log($"Saved texture data to: {filePath}");
            return fileName;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save texture data: {e.Message}");
            return "";
        }
    }

    // Helper method to make texture readable
    private Texture2D MakeTextureReadable(Texture2D original)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(original.width, original.height);
        Graphics.Blit(original, renderTexture);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D readableTexture = new Texture2D(original.width, original.height);
        readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return readableTexture;
    }

    // -----------------
    // Enhanced Save Session
    // -----------------
    public void SaveSession(string saveFileName)
    {
        SessionData session = new SessionData();
        GameObject[] models = GameObject.FindGameObjectsWithTag(modelTag);

        Debug.Log($"Saving session with {models.Length} models");

        foreach (var model in models)
        {
            string modelKey = model.name.Replace("(Clone)", "");
            bool isServer = serverModelCache.ContainsKey(modelKey);

            ModelData data = new ModelData
            {
                position = model.transform.position,
                rotation = model.transform.rotation,
                scale = model.transform.localScale,
                tag = model.tag,
                isServerModel = isServer,
                modelPath = isServer ? modelKey : "Models/" + modelKey
            };

            // Store PhotonView ID if available
            var photonView = model.GetComponent<Photon.Pun.PhotonView>();
            if (photonView != null)
            {
                data.photonViewID = photonView.ViewID;
            }

            if (isServer && serverModelCache.ContainsKey(modelKey))
            {
                ServerModelCache cache = serverModelCache[modelKey];
                data.modelId = cache.modelId;
                data.modelName = cache.modelName;
                data.isTransparent = cache.isTransparent;
                data.isChild = cache.isChild;
                data.modelUrl = cache.modelUrl;
                data.textureUrl = cache.textureUrl;

                // Save mesh and material data to files
                data.meshDataPath = SaveMeshData(cache.mesh, modelKey);
                data.materialDataPath = SaveMaterialData(cache.material, modelKey);
            }
            else
            {
                data.modelId = "";
                data.modelName = modelKey;
                data.isTransparent = false;
                data.isChild = false;
            }

            session.models.Add(data);
            Debug.Log($"Saved model: {data.modelName} (Server: {isServer})");
        }

        string json = JsonUtility.ToJson(session, true);
        string filePath = Application.persistentDataPath + "/" + saveFileName + ".json";
        File.WriteAllText(filePath, json);
        Debug.Log($"Session saved to: {filePath} with {session.models.Count} models");
    }

    // -----------------
    // Enhanced Load Session - Use ModelSample for Server Models
    // -----------------
    public void LoadSession(string saveFileName)
    {
        string path = Application.persistentDataPath + "/" + saveFileName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        SessionData session = JsonUtility.FromJson<SessionData>(json);

        Debug.Log($"Loading session with {session.models.Count} models");

        // Destroy existing models tagged as ModelPart
        GameObject[] existingModels = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in existingModels)
        {
            Debug.Log($"Destroying existing model: {model.name}");
            Destroy(model);
        }

        // Clear the cache to avoid conflicts
        serverModelCache.Clear();

        // Wait a frame for cleanup
        StartCoroutine(LoadModelsAfterCleanup(session.models));
    }

    private IEnumerator LoadModelsAfterCleanup(List<ModelData> models)
    {
        yield return null; // Wait one frame for cleanup

        foreach (var modelData in models)
        {
            if (modelData.isServerModel)
            {
                // For server models, try ModelSample first, then fallback to saved data
                if (modelSample != null && !string.IsNullOrEmpty(modelData.modelId))
                {
                    Debug.Log($"Loading server model via ModelSample: {modelData.modelName} (ID: {modelData.modelId})");
                    modelSample.LoadModel(modelData.modelId, modelData.modelName, modelData.isTransparent, modelData.isChild);

                    // Wait for the model to load, with fallback to saved data if download fails
                    yield return StartCoroutine(WaitAndApplyTransformWithFallback(modelData));
                }
                else
                {
                    Debug.LogError($"Cannot load server model: ModelSample is null or modelId is empty for {modelData.modelName}");
                    CreatePlaceholderModel(modelData);
                }
            }
            else
            {
                LoadLocalModel(modelData);
            }

            yield return new WaitForSeconds(0.5f); // Delay between model loads
        }

        Debug.Log("Session loading completed");
    }

    private IEnumerator WaitAndApplyTransformWithFallback(ModelData modelData)
    {
        // Wait for the model to be created by ModelSample
        float timeout = 10f;
        float elapsed = 0f;
        GameObject foundModel = null;

        string expectedName = modelData.modelPath; // This should be the prefab name without "Models/"

        while (elapsed < timeout && foundModel == null)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;

            // Look for the newly created model
            GameObject[] currentModels = GameObject.FindGameObjectsWithTag(modelTag);
            foreach (var model in currentModels)
            {
                string modelKey = model.name.Replace("(Clone)", "");
                if (modelKey == expectedName)
                {
                    // Check if it has a mesh (meaning it's fully loaded)
                    MeshFilter mf = model.GetComponent<MeshFilter>();
                    if (mf != null && mf.mesh != null)
                    {
                        foundModel = model;
                        break;
                    }
                }
            }
        }

        if (foundModel != null)
        {
            // Apply the saved transform
            foundModel.transform.position = modelData.position;
            foundModel.transform.rotation = modelData.rotation;
            foundModel.transform.localScale = modelData.scale;

            Debug.Log($"Applied saved transform to server model: {foundModel.name}");

            // Cache the model for future saves
            CacheServerModel(foundModel, modelData.modelId, modelData.modelName,
                           modelData.isTransparent, modelData.isChild,
                           modelData.modelUrl, modelData.textureUrl);
        }
        else
        {
            Debug.LogWarning($"ModelSample download failed for {modelData.modelName}, trying fallback to saved data...");

            // Fallback: Try to load from saved mesh/material data
            if (!string.IsNullOrEmpty(modelData.meshDataPath) && !string.IsNullOrEmpty(modelData.materialDataPath))
            {
                yield return StartCoroutine(LoadModelFromSavedFiles(modelData));
            }
            else
            {
                Debug.LogError($"No saved data available for fallback. Creating placeholder for: {modelData.modelName}");
                CreatePlaceholderModel(modelData);
            }
        }
    }

    // Add the LoadModelFromSavedFiles method back for fallback
    private IEnumerator LoadModelFromSavedFiles(ModelData modelData)
    {
        string modelKey = modelData.modelPath;

        // Load base prefab
        GameObject basePrefab = Resources.Load<GameObject>(modelKey);
        if (basePrefab == null)
        {
            Debug.LogError($"Base prefab not found for {modelKey}");
            CreatePlaceholderModel(modelData);
            yield break;
        }

        // Create model instance
        GameObject model = Instantiate(basePrefab);
        model.name = basePrefab.name;
        model.tag = modelTag;

        // Load mesh data
        string meshPath = Path.Combine(Application.persistentDataPath, modelData.meshDataPath);
        if (File.Exists(meshPath))
        {
            string meshJson = File.ReadAllText(meshPath);
            SerializableMesh serializableMesh = JsonUtility.FromJson<SerializableMesh>(meshJson);

            // Create mesh from saved data
            Mesh mesh = new Mesh
            {
                vertices = serializableMesh.vertices,
                triangles = serializableMesh.triangles,
                normals = serializableMesh.normals,
                uv = serializableMesh.uv,
                name = serializableMesh.name
            };

            MeshFilter meshFilter = model.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = mesh;
                Debug.Log($"Applied saved mesh to {model.name}");
            }
        }

        // Load material data
        string materialPath = Path.Combine(Application.persistentDataPath, modelData.materialDataPath);
        if (File.Exists(materialPath))
        {
            yield return StartCoroutine(LoadAndApplyMaterial(model, materialPath, modelData));
        }

        ApplyModelTransform(model, modelData);

        // Re-cache the loaded model
        CacheServerModel(model, modelData.modelId, modelData.modelName,
                        modelData.isTransparent, modelData.isChild,
                        modelData.modelUrl, modelData.textureUrl);

        Debug.Log($"Successfully loaded server model from saved data fallback: {model.name}");
    }

    private IEnumerator LoadAndApplyMaterial(GameObject model, string materialPath, ModelData modelData)
    {
        string materialJson = File.ReadAllText(materialPath);
        SerializableMaterial serializableMaterial = JsonUtility.FromJson<SerializableMaterial>(materialJson);

        // Load the appropriate base material
        Material baseMaterial = serializableMaterial.isTransparent
            ? Resources.Load<Material>("Cross-Section-Material_Transparent")
            : Resources.Load<Material>("Cross-Section-Material");

        if (baseMaterial == null)
        {
            Debug.LogError("Base material not found in Resources");
            yield break;
        }

        // Create new material instance
        Material material = new Material(baseMaterial);
        material.name = serializableMaterial.name;

        // Apply color safely
        if (material.HasProperty("_Color"))
        {
            material.color = serializableMaterial.color;
        }
        else if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", serializableMaterial.color);
        }

        // Load all saved textures
        if (serializableMaterial.textureProperties != null)
        {
            foreach (var kvp in serializableMaterial.textureProperties)
            {
                string propertyName = kvp.Key;
                string textureFileName = kvp.Value;

                if (!string.IsNullOrEmpty(textureFileName))
                {
                    string texturePath = Path.Combine(Application.persistentDataPath, textureFileName);
                    if (File.Exists(texturePath))
                    {
                        byte[] textureData = File.ReadAllBytes(texturePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(textureData))
                        {
                            // Check if the material has this property before setting it
                            if (material.HasProperty(propertyName))
                            {
                                material.SetTexture(propertyName, texture);
                                Debug.Log($"Applied saved texture to property {propertyName} on {model.name}");
                            }
                            else
                            {
                                Debug.LogWarning($"Material doesn't have property {propertyName}, skipping texture assignment");
                            }
                        }
                    }
                }
            }
        }

        // Apply material to renderer
        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
            Debug.Log($"Applied saved material to {model.name}");
        }

        yield return null;
    }

    private void LoadLocalModel(ModelData modelData)
    {
        GameObject prefab = Resources.Load<GameObject>(modelData.modelPath);
        GameObject model;

        if (prefab != null)
        {
            model = Instantiate(prefab);
            model.name = prefab.name;
        }
        else
        {
            model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            model.name = modelData.modelPath.Replace("Models/", "");
            Debug.LogWarning("Local prefab not found: " + modelData.modelPath);
        }

        ApplyModelTransform(model, modelData);
        Debug.Log($"Loaded local model: {model.name}");
    }

    private void ApplyModelTransform(GameObject model, ModelData modelData)
    {
        model.transform.position = modelData.position;
        model.transform.rotation = modelData.rotation;
        model.transform.localScale = modelData.scale;
        model.tag = modelData.tag;
    }

    private void CreatePlaceholderModel(ModelData modelData)
    {
        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.name = modelData.modelName + "_Placeholder";

        ApplyModelTransform(model, modelData);

        // Make it distinctive
        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        Debug.LogWarning($"Created placeholder for: {modelData.modelName}");
    }

    // -----------------
    // Public utility methods
    // -----------------
    public void OnServerModelLoaded(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild)
    {
        if (model != null)
        {
            // Get the URLs from ModelSample if available
            string modelUrl = "";
            string textureUrl = "";

            if (modelSample != null)
            {
                modelUrl = modelSample.CurrentModelUrl;
                textureUrl = modelSample.CurrentTextureUrl;
            }

            // Extract the actual model ID from the URL if modelId is actually the name
            string actualModelId = modelId;
            if (!string.IsNullOrEmpty(modelUrl) && modelUrl.Contains("model_id="))
            {
                // Extract model ID from URL using simple string manipulation
                int startIndex = modelUrl.IndexOf("model_id=") + "model_id=".Length;
                int endIndex = modelUrl.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = modelUrl.Length;

                string extractedId = modelUrl.Substring(startIndex, endIndex - startIndex);
                if (!string.IsNullOrEmpty(extractedId))
                {
                    actualModelId = extractedId;
                    Debug.Log($"Extracted model ID: {actualModelId} from URL");
                }
            }

            CacheServerModel(model, actualModelId, modelName, isTransparent, isChild, modelUrl, textureUrl);
        }
    }

    public void ClearCache()
    {
        serverModelCache.Clear();
        Debug.Log("Server model cache cleared");
    }

    public void ClearSavedData()
    {
        string[] meshFiles = Directory.GetFiles(Application.persistentDataPath, "mesh_*.json");
        string[] materialFiles = Directory.GetFiles(Application.persistentDataPath, "material_*.json");
        string[] textureFiles = Directory.GetFiles(Application.persistentDataPath, "texture_*.png");

        foreach (string file in meshFiles.Concat(materialFiles).Concat(textureFiles))
        {
            File.Delete(file);
        }

        Debug.Log("Cleared all saved model data files");
    }

    public bool HasCachedModel(string modelKey)
    {
        return serverModelCache.ContainsKey(modelKey);
    }

    public void SaveCurrentSession()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        SaveSession("Session_" + timestamp);
    }
}*/
























///////////////////////////from local 


/*using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

[Serializable]
public class ModelData
{
    public string modelPath;       // "Models/Skull" for local, prefab name for server
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool isServerModel;     // true for server models
    public string modelId;         // Server model ID for re-downloading
    public string modelName;       // Model name
    public bool isTransparent;     // Transparency state
    public bool isChild;           // Child state
    public int photonViewID;       // Store PhotonView ID if available
    public string uniqueInstanceId; // NEW: Unique identifier for each model instance

    // Additional fields for server models
    public string modelUrl;        // Full download URL
    public string textureUrl;      // Texture download URL
    public string meshDataPath;    // Path to saved mesh data file
    public string materialDataPath; // Path to saved material data file
}

[Serializable]
public class SessionData
{
    public List<ModelData> models = new List<ModelData>();
}

// Serializable classes for mesh and material data
[Serializable]
public class SerializableMesh
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uv;
    public string name;
}

[Serializable]
public class SerializableMaterial
{
    public string name;
    public Color color;
    public string texturePath; // Path to saved texture file
    public bool isTransparent;
    public string shaderName; // Store shader name
    public Dictionary<string, string> textureProperties; // Store all texture properties
}

public class SessionManager : MonoBehaviour
{
    private const string modelTag = "ModelPart";

    [Header("References")]
    public ModelSample modelSample; // Reference to your ModelSample script

    [Header("UI References")]
    public GameObject loadingUI; // Reference to loading/downloading UI panel

    // Cache for server models - with unique instance IDs
    private Dictionary<string, ServerModelCache> serverModelCache = new Dictionary<string, ServerModelCache>();

    // Track loaded instances to prevent overwriting
    private Dictionary<string, GameObject> loadedInstances = new Dictionary<string, GameObject>();

    [Serializable]
    private class ServerModelCache
    {
        public Mesh mesh;
        public Material material;
        public string modelId;
        public string modelName;
        public bool isTransparent;
        public bool isChild;
        public string modelUrl;
        public string textureUrl;
        public GameObject prefab; // Store the base prefab reference
        public Dictionary<string, Texture2D> cachedTextures; // Cache all textures
        public string uniqueInstanceId; // NEW: Track instance
    }

    void Start()
    {
        // Auto-find ModelSample if not assigned
        if (modelSample == null)
        {
            modelSample = FindObjectOfType<ModelSample>();
            if (modelSample == null)
            {
                Debug.LogError("ModelSample not found! Please assign it in the inspector.");
            }
        }
    }

    // -----------------
    // Import Local Model
    // -----------------
    public GameObject ImportLocalModel(string prefabName)
    {
        string path = "Models/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Local prefab not found: " + path);
            return null;
        }

        GameObject model = Instantiate(prefab);
        model.name = prefab.name; // remove (Clone)
        model.tag = modelTag;
        return model;
    }

    // -----------------
    // Cache Server Model (enhanced with unique instance tracking)
    // -----------------
    public void CacheServerModel(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild, string modelUrl, string textureUrl)
    {
        if (model == null)
        {
            Debug.LogError("Cannot cache null model");
            return;
        }

        // Generate unique instance ID based on model and instance ID
        string uniqueInstanceId = model.GetInstanceID().ToString();
        string cacheKey = $"{model.name.Replace("(Clone)", "")}_{uniqueInstanceId}";

        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        Renderer renderer = model.GetComponent<Renderer>();

        if (meshFilter == null || renderer == null)
        {
            Debug.LogError($"Model {cacheKey} missing MeshFilter or Renderer components");
            return;
        }

        // Find the base prefab
        GameObject basePrefab = Resources.Load<GameObject>(model.name.Replace("(Clone)", ""));

        // Cache all textures from the material
        Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
        if (renderer.material != null)
        {
            CacheAllMaterialTextures(renderer.material, cachedTextures);
        }

        ServerModelCache cache = new ServerModelCache
        {
            mesh = meshFilter.mesh,
            material = renderer.material,
            modelId = modelId,
            modelName = modelName,
            isTransparent = isTransparent,
            isChild = isChild,
            modelUrl = modelUrl,
            textureUrl = textureUrl,
            prefab = basePrefab,
            cachedTextures = cachedTextures,
            uniqueInstanceId = uniqueInstanceId
        };

        serverModelCache[cacheKey] = cache;
        loadedInstances[uniqueInstanceId] = model;

        Debug.Log($"Cached server model: {cacheKey} with ID: {modelId}, name: {modelName}");
    }

    // Helper method to cache all textures from a material
    private void CacheAllMaterialTextures(Material material, Dictionary<string, Texture2D> textureCache)
    {
        if (material == null || material.shader == null) return;

        string[] commonTextureProperties = {
            "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap", "_Texture", "_Tex", "_Texture2D",
            "_EmissionMap", "_BumpMap", "_NormalMap", "_OcclusionMap", "_MetallicGlossMap"
        };

        foreach (string propName in commonTextureProperties)
        {
            if (material.HasProperty(propName))
            {
                Texture texture = material.GetTexture(propName);
                if (texture != null && texture is Texture2D)
                {
                    textureCache[propName] = texture as Texture2D;
                    Debug.Log($"Cached texture property: {propName}");
                }
            }
        }
    }

    // -----------------
    // Save mesh and material data to files
    // -----------------
    private string SaveMeshData(Mesh mesh, string modelKey)
    {
        if (mesh == null) return "";

        SerializableMesh serializableMesh = new SerializableMesh
        {
            vertices = mesh.vertices,
            triangles = mesh.triangles,
            normals = mesh.normals,
            uv = mesh.uv,
            name = mesh.name
        };

        string fileName = $"mesh_{modelKey}_{DateTime.Now.Ticks}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(serializableMesh);
        File.WriteAllText(filePath, json);

        Debug.Log($"Saved mesh data to: {filePath}");
        return fileName;
    }

    private string SaveMaterialData(Material material, string modelKey)
    {
        if (material == null) return "";

        // Save all texture properties
        Dictionary<string, string> textureFiles = new Dictionary<string, string>();

        // Find the cache entry that matches this material
        foreach (var cacheEntry in serverModelCache)
        {
            if (cacheEntry.Value.material == material && cacheEntry.Value.cachedTextures != null)
            {
                foreach (var kvp in cacheEntry.Value.cachedTextures)
                {
                    string textureFileName = SaveTextureData(kvp.Value, $"{modelKey}_{kvp.Key}");
                    if (!string.IsNullOrEmpty(textureFileName))
                    {
                        textureFiles[kvp.Key] = textureFileName;
                    }
                }
                break;
            }
        }

        Color materialColor = Color.white;
        if (material.HasProperty("_Color"))
        {
            materialColor = material.color;
        }
        else if (material.HasProperty("_BaseColor"))
        {
            materialColor = material.GetColor("_BaseColor");
        }
        else if (material.HasProperty("_MainColor"))
        {
            materialColor = material.GetColor("_MainColor");
        }

        SerializableMaterial serializableMaterial = new SerializableMaterial
        {
            name = material.name,
            color = materialColor,
            texturePath = textureFiles.ContainsKey("_MainTex") ? textureFiles["_MainTex"] :
                         textureFiles.ContainsKey("_Texture2D") ? textureFiles["_Texture2D"] :
                         (textureFiles.Count > 0 ? textureFiles.First().Value : ""),
            isTransparent = material.name.Contains("Transparent"),
            shaderName = material.shader != null ? material.shader.name : "",
            textureProperties = textureFiles
        };

        string fileName = $"material_{modelKey}_{DateTime.Now.Ticks}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(serializableMaterial);
        File.WriteAllText(filePath, json);

        Debug.Log($"Saved material data to: {filePath}");
        return fileName;
    }

    private string SaveTextureData(Texture2D texture, string modelKey)
    {
        if (texture == null) return "";

        try
        {
            Texture2D readableTexture = texture;
            if (!texture.isReadable)
            {
                readableTexture = MakeTextureReadable(texture);
            }

            byte[] textureData = readableTexture.EncodeToPNG();
            string fileName = $"texture_{modelKey}_{DateTime.Now.Ticks}.png";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, textureData);

            Debug.Log($"Saved texture data to: {filePath}");
            return fileName;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save texture data: {e.Message}");
            return "";
        }
    }

    private Texture2D MakeTextureReadable(Texture2D original)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(original.width, original.height);
        Graphics.Blit(original, renderTexture);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D readableTexture = new Texture2D(original.width, original.height);
        readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return readableTexture;
    }

    // -----------------
    // Enhanced Save Session
    // -----------------
    public void SaveSession(string saveFileName)
    {
        SessionData session = new SessionData();
        GameObject[] models = GameObject.FindGameObjectsWithTag(modelTag);

        Debug.Log($"Saving session with {models.Length} models");

        foreach (var model in models)
        {
            string modelKey = model.name.Replace("(Clone)", "");
            string instanceId = model.GetInstanceID().ToString();
            string cacheKey = $"{modelKey}_{instanceId}";

            bool isServer = serverModelCache.ContainsKey(cacheKey);

            ModelData data = new ModelData
            {
                position = model.transform.position,
                rotation = model.transform.rotation,
                scale = model.transform.localScale,
                tag = model.tag,
                isServerModel = isServer,
                modelPath = isServer ? modelKey : "Models/" + modelKey,
                uniqueInstanceId = instanceId // Store unique instance ID
            };

            var photonView = model.GetComponent<Photon.Pun.PhotonView>();
            if (photonView != null)
            {
                data.photonViewID = photonView.ViewID;
            }

            if (isServer && serverModelCache.ContainsKey(cacheKey))
            {
                ServerModelCache cache = serverModelCache[cacheKey];
                data.modelId = cache.modelId;
                data.modelName = cache.modelName;
                data.isTransparent = cache.isTransparent;
                data.isChild = cache.isChild;
                data.modelUrl = cache.modelUrl;
                data.textureUrl = cache.textureUrl;

                data.meshDataPath = SaveMeshData(cache.mesh, cacheKey);
                data.materialDataPath = SaveMaterialData(cache.material, cacheKey);
            }
            else
            {
                data.modelId = "";
                data.modelName = modelKey;
                data.isTransparent = false;
                data.isChild = false;
            }

            session.models.Add(data);
            Debug.Log($"Saved model: {data.modelName} (Server: {isServer}, Instance: {instanceId})");
        }

        string json = JsonUtility.ToJson(session, true);
        string filePath = Application.persistentDataPath + "/" + saveFileName + ".json";
        File.WriteAllText(filePath, json);
        Debug.Log($"Session saved to: {filePath} with {session.models.Count} models");
    }

    // -----------------
    // Enhanced Load Session
    // -----------------
    public void LoadSession(string saveFileName)
    {
        string path = Application.persistentDataPath + "/" + saveFileName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        SessionData session = JsonUtility.FromJson<SessionData>(json);

        Debug.Log($"Loading session with {session.models.Count} models");

        // Show "Reloading" UI
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
            // If your loading UI has a text component, update it
            var loadingText = loadingUI.GetComponentInChildren<UnityEngine.UI.Text>();
            if (loadingText != null)
            {
                loadingText.text = "Reloading Session...";
            }
        }

        // Destroy existing models
        GameObject[] existingModels = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in existingModels)
        {
            Debug.Log($"Destroying existing model: {model.name}");
            Destroy(model);
        }

        // Clear caches
        serverModelCache.Clear();
        loadedInstances.Clear();

        StartCoroutine(LoadModelsAfterCleanup(session.models));
    }

    private IEnumerator LoadModelsAfterCleanup(List<ModelData> models)
    {
        yield return null; // Wait one frame for cleanup

        int loadedCount = 0;
        int totalModels = models.Count;

        foreach (var modelData in models)
        {
            loadedCount++;

            // Update loading UI progress
            if (loadingUI != null)
            {
                var loadingText = loadingUI.GetComponentInChildren<UnityEngine.UI.Text>();
                if (loadingText != null)
                {
                    loadingText.text = $"Reloading Session... ({loadedCount}/{totalModels})";
                }
            }

            if (modelData.isServerModel)
            {
                // Load from saved data first (offline mode)
                if (!string.IsNullOrEmpty(modelData.meshDataPath) && !string.IsNullOrEmpty(modelData.materialDataPath))
                {
                    Debug.Log($"Loading server model from saved data: {modelData.modelName}");
                    yield return StartCoroutine(LoadModelFromSavedFiles(modelData));
                }
                else if (modelSample != null && !string.IsNullOrEmpty(modelData.modelId))
                {
                    Debug.Log($"Loading server model via ModelSample: {modelData.modelName} (ID: {modelData.modelId})");
                    modelSample.LoadModel(modelData.modelId, modelData.modelName, modelData.isTransparent, modelData.isChild);
                    yield return StartCoroutine(WaitAndApplyTransform(modelData));
                }
                else
                {
                    Debug.LogError($"Cannot load server model: No saved data or ModelSample unavailable for {modelData.modelName}");
                    CreatePlaceholderModel(modelData);
                }
            }
            else
            {
                LoadLocalModel(modelData);
            }

            yield return new WaitForSeconds(0.1f); // Small delay between loads
        }

        // Hide loading UI when done
        if (loadingUI != null)
        {
            loadingUI.SetActive(false);
        }

        Debug.Log("Session loading completed");
    }

    private IEnumerator WaitAndApplyTransform(ModelData modelData)
    {
        float timeout = 10f;
        float elapsed = 0f;
        GameObject foundModel = null;
        string expectedName = modelData.modelPath;

        while (elapsed < timeout && foundModel == null)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;

            GameObject[] currentModels = GameObject.FindGameObjectsWithTag(modelTag);
            foreach (var model in currentModels)
            {
                string modelKey = model.name.Replace("(Clone)", "");
                if (modelKey == expectedName)
                {
                    MeshFilter mf = model.GetComponent<MeshFilter>();
                    if (mf != null && mf.mesh != null)
                    {
                        // Check if this instance hasn't been processed yet
                        string instanceId = model.GetInstanceID().ToString();
                        if (!loadedInstances.ContainsKey(instanceId))
                        {
                            foundModel = model;
                            break;
                        }
                    }
                }
            }
        }

        if (foundModel != null)
        {
            foundModel.transform.position = modelData.position;
            foundModel.transform.rotation = modelData.rotation;
            foundModel.transform.localScale = modelData.scale;

            Debug.Log($"Applied saved transform to server model: {foundModel.name}");

            CacheServerModel(foundModel, modelData.modelId, modelData.modelName,
                           modelData.isTransparent, modelData.isChild,
                           modelData.modelUrl, modelData.textureUrl);
        }
        else
        {
            Debug.LogWarning($"ModelSample download failed for {modelData.modelName}, trying fallback...");

            if (!string.IsNullOrEmpty(modelData.meshDataPath) && !string.IsNullOrEmpty(modelData.materialDataPath))
            {
                yield return StartCoroutine(LoadModelFromSavedFiles(modelData));
            }
            else
            {
                CreatePlaceholderModel(modelData);
            }
        }
    }

    private IEnumerator LoadModelFromSavedFiles(ModelData modelData)
    {
        string modelKey = modelData.modelPath;

        GameObject basePrefab = Resources.Load<GameObject>(modelKey);
        if (basePrefab == null)
        {
            Debug.LogError($"Base prefab not found for {modelKey}");
            CreatePlaceholderModel(modelData);
            yield break;
        }

        GameObject model = Instantiate(basePrefab);
        model.name = basePrefab.name;
        model.tag = modelTag;

        // Load mesh data
        string meshPath = Path.Combine(Application.persistentDataPath, modelData.meshDataPath);
        if (File.Exists(meshPath))
        {
            string meshJson = File.ReadAllText(meshPath);
            SerializableMesh serializableMesh = JsonUtility.FromJson<SerializableMesh>(meshJson);

            Mesh mesh = new Mesh
            {
                vertices = serializableMesh.vertices,
                triangles = serializableMesh.triangles,
                normals = serializableMesh.normals,
                uv = serializableMesh.uv,
                name = serializableMesh.name
            };

            MeshFilter meshFilter = model.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = mesh;
                Debug.Log($"Applied saved mesh to {model.name}");
            }
        }

        // Load material data
        string materialPath = Path.Combine(Application.persistentDataPath, modelData.materialDataPath);
        if (File.Exists(materialPath))
        {
            yield return StartCoroutine(LoadAndApplyMaterial(model, materialPath, modelData));
        }

        ApplyModelTransform(model, modelData);

        CacheServerModel(model, modelData.modelId, modelData.modelName,
                        modelData.isTransparent, modelData.isChild,
                        modelData.modelUrl, modelData.textureUrl);

        Debug.Log($"Successfully loaded server model from saved data: {model.name}");
    }

    private IEnumerator LoadAndApplyMaterial(GameObject model, string materialPath, ModelData modelData)
    {
        string materialJson = File.ReadAllText(materialPath);
        SerializableMaterial serializableMaterial = JsonUtility.FromJson<SerializableMaterial>(materialJson);

        Material baseMaterial = serializableMaterial.isTransparent
            ? Resources.Load<Material>("Cross-Section-Material_Transparent")
            : Resources.Load<Material>("Cross-Section-Material");

        if (baseMaterial == null)
        {
            Debug.LogError("Base material not found in Resources");
            yield break;
        }

        Material material = new Material(baseMaterial);
        material.name = serializableMaterial.name;

        if (material.HasProperty("_Color"))
        {
            material.color = serializableMaterial.color;
        }
        else if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", serializableMaterial.color);
        }

        if (serializableMaterial.textureProperties != null)
        {
            foreach (var kvp in serializableMaterial.textureProperties)
            {
                string propertyName = kvp.Key;
                string textureFileName = kvp.Value;

                if (!string.IsNullOrEmpty(textureFileName))
                {
                    string texturePath = Path.Combine(Application.persistentDataPath, textureFileName);
                    if (File.Exists(texturePath))
                    {
                        byte[] textureData = File.ReadAllBytes(texturePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(textureData))
                        {
                            if (material.HasProperty(propertyName))
                            {
                                material.SetTexture(propertyName, texture);
                                Debug.Log($"Applied saved texture to property {propertyName}");
                            }
                        }
                    }
                }
            }
        }

        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
            Debug.Log($"Applied saved material to {model.name}");
        }

        yield return null;
    }

    private void LoadLocalModel(ModelData modelData)
    {
        GameObject prefab = Resources.Load<GameObject>(modelData.modelPath);
        GameObject model;

        if (prefab != null)
        {
            model = Instantiate(prefab);
            model.name = prefab.name;
        }
        else
        {
            model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            model.name = modelData.modelPath.Replace("Models/", "");
            Debug.LogWarning("Local prefab not found: " + modelData.modelPath);
        }

        ApplyModelTransform(model, modelData);
        Debug.Log($"Loaded local model: {model.name}");
    }

    private void ApplyModelTransform(GameObject model, ModelData modelData)
    {
        model.transform.position = modelData.position;
        model.transform.rotation = modelData.rotation;
        model.transform.localScale = modelData.scale;
        model.tag = modelData.tag;
    }

    private void CreatePlaceholderModel(ModelData modelData)
    {
        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.name = modelData.modelName + "_Placeholder";

        ApplyModelTransform(model, modelData);

        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        Debug.LogWarning($"Created placeholder for: {modelData.modelName}");
    }

    // -----------------
    // Public utility methods
    // -----------------
    public void OnServerModelLoaded(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild)
    {
        if (model != null)
        {
            string modelUrl = "";
            string textureUrl = "";

            if (modelSample != null)
            {
                modelUrl = modelSample.CurrentModelUrl;
                textureUrl = modelSample.CurrentTextureUrl;
            }

            string actualModelId = modelId;
            if (!string.IsNullOrEmpty(modelUrl) && modelUrl.Contains("model_id="))
            {
                int startIndex = modelUrl.IndexOf("model_id=") + "model_id=".Length;
                int endIndex = modelUrl.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = modelUrl.Length;

                string extractedId = modelUrl.Substring(startIndex, endIndex - startIndex);
                if (!string.IsNullOrEmpty(extractedId))
                {
                    actualModelId = extractedId;
                    Debug.Log($"Extracted model ID: {actualModelId} from URL");
                }
            }

            CacheServerModel(model, actualModelId, modelName, isTransparent, isChild, modelUrl, textureUrl);
        }
    }

    public void ClearCache()
    {
        serverModelCache.Clear();
        loadedInstances.Clear();
        Debug.Log("Server model cache cleared");
    }

    public void ClearSavedData()
    {
        string[] meshFiles = Directory.GetFiles(Application.persistentDataPath, "mesh_*.json");
        string[] materialFiles = Directory.GetFiles(Application.persistentDataPath, "material_*.json");
        string[] textureFiles = Directory.GetFiles(Application.persistentDataPath, "texture_*.png");

        foreach (string file in meshFiles.Concat(materialFiles).Concat(textureFiles))
        {
            File.Delete(file);
        }

        Debug.Log("Cleared all saved model data files");
    }

    public bool HasCachedModel(string modelKey)
    {
        return serverModelCache.ContainsKey(modelKey);
    }

    public void SaveCurrentSession()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        SaveSession("Session_" + timestamp);
    }
}*/









//////////////////////////////////////currently working 



/*using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;

[Serializable]
public class ModelData
{
    public string modelPath;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool isServerModel;
    public string modelId;
    public string modelName;
    public bool isTransparent;
    public bool isChild;
    public int photonViewID;
    public string modelInstanceId;
    public string modelUrl;
    public string textureUrl;
    public string meshDataPath;
    public string materialDataPath;
}

[Serializable]
public class MeasurementData
{
    public Vector3[] storedPoints;
    public bool isStraightLine;
    public float originalMeasurement;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector3 canvasLocalPosition;
    public Vector3 point1LocalPosition;
    public Vector3 point2LocalPosition;
    public bool isAttachedToModel;
    public string attachedModelID;
    public float lineWidth;
    public Color lineColor;
}

[Serializable]
public class DrawingSessionData
{
    public Vector3[] points;
    public int colorIndex;
    public float width;
    public Color lineColor;
    public string drawingId;
    public bool useWorldSpace;
    public Vector3 parentPosition;
    public Quaternion parentRotation;
    public bool isAttachedToModel;
    public string attachedModelID;
}

[Serializable]
public class SessionData
{
    public List<ModelData> models = new List<ModelData>();
    public List<MeasurementData> measurements = new List<MeasurementData>();
    public List<DrawingSessionData> drawings = new List<DrawingSessionData>();
}

[Serializable]
public class SerializableMesh
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uv;
    public string name;
}

[Serializable]
public class SerializableMaterial
{
    public string name;
    public Color color;
    public string texturePath;
    public bool isTransparent;
    public string shaderName;
    public Dictionary<string, string> textureProperties;
}

public class SessionManager : MonoBehaviour
{
    private const string modelTag = "ModelPart";

    [Header("References")]
    public ModelSample modelSample;
    public Material drawingMaterial; // Reference to the pen drawing material
    [SerializeField] private LayerMask drawingLayerMask;

    private Dictionary<string, ServerModelCache> serverModelCache = new Dictionary<string, ServerModelCache>();
    private Dictionary<string, GameObject> modelInstanceMap = new Dictionary<string, GameObject>();

    [Serializable]
    private class ServerModelCache
    {
        public Mesh mesh;
        public Material material;
        public string modelId;
        public string modelName;
        public bool isTransparent;
        public bool isChild;
        public string modelUrl;
        public string textureUrl;
        public GameObject prefab;
        public Dictionary<string, Texture2D> cachedTextures;
    }

    void Start()
    {
        if (modelSample == null)
        {
            modelSample = FindObjectOfType<ModelSample>();
            if (modelSample == null)
            {
                Debug.LogError("ModelSample not found! Please assign it in the inspector.");
            }
        }

        if (drawingMaterial == null)
        {
            Debug.LogWarning("Drawing material not assigned! Attempting to find it...");
            drawingMaterial = Resources.Load<Material>("DrawingMaterial");
            if (drawingMaterial == null)
            {
                Debug.LogError("Drawing material not found in Resources! Drawings may not load correctly.");
            }
        }
    }

    private string GenerateModelInstanceId()
    {
        return System.Guid.NewGuid().ToString();
    }

    private void SetModelInstanceId(GameObject model, string instanceId)
    {
        var idComponent = model.GetComponent<ModelInstanceId>();
        if (idComponent == null)
        {
            idComponent = model.AddComponent<ModelInstanceId>();
        }
        idComponent.instanceId = instanceId;
        modelInstanceMap[instanceId] = model;
    }

    private string GetModelInstanceId(GameObject model)
    {
        var idComponent = model.GetComponent<ModelInstanceId>();
        return idComponent != null ? idComponent.instanceId : null;
    }

    public GameObject ImportLocalModel(string prefabName)
    {
        string path = "Models/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Local prefab not found: " + path);
            return null;
        }

        GameObject model = Instantiate(prefab);
        model.name = prefab.name;
        model.tag = modelTag;
        SetModelInstanceId(model, GenerateModelInstanceId());
        return model;
    }

    public void CacheServerModel(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild, string modelUrl, string textureUrl)
    {
        if (model == null)
        {
            Debug.LogError("Cannot cache null model");
            return;
        }

        if (GetModelInstanceId(model) == null)
        {
            SetModelInstanceId(model, GenerateModelInstanceId());
        }

        string cacheKey = model.name.Replace("(Clone)", "");

        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        Renderer renderer = model.GetComponent<Renderer>();

        if (meshFilter == null || renderer == null)
        {
            Debug.LogError($"Model {cacheKey} missing MeshFilter or Renderer components");
            return;
        }

        GameObject basePrefab = Resources.Load<GameObject>(cacheKey);
        Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

        if (renderer.material != null)
        {
            CacheAllMaterialTextures(renderer.material, cachedTextures);
        }

        ServerModelCache cache = new ServerModelCache
        {
            mesh = meshFilter.mesh,
            material = renderer.material,
            modelId = modelId,
            modelName = modelName,
            isTransparent = isTransparent,
            isChild = isChild,
            modelUrl = modelUrl,
            textureUrl = textureUrl,
            prefab = basePrefab,
            cachedTextures = cachedTextures
        };

        serverModelCache[cacheKey] = cache;
        Debug.Log($"Cached server model: {cacheKey} with ID: {modelId}, name: {modelName}");
    }

    private void CacheAllMaterialTextures(Material material, Dictionary<string, Texture2D> textureCache)
    {
        if (material == null || material.shader == null) return;

        string[] commonTextureProperties = {
            "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap", "_Texture", "_Tex", "_Texture2D",
            "_EmissionMap", "_BumpMap", "_NormalMap", "_OcclusionMap", "_MetallicGlossMap"
        };

        foreach (string propName in commonTextureProperties)
        {
            if (material.HasProperty(propName))
            {
                Texture texture = material.GetTexture(propName);
                if (texture != null && texture is Texture2D)
                {
                    textureCache[propName] = texture as Texture2D;
                }
            }
        }
    }

    private string SaveMeshData(Mesh mesh, string modelKey)
    {
        if (mesh == null) return "";

        SerializableMesh serializableMesh = new SerializableMesh
        {
            vertices = mesh.vertices,
            triangles = mesh.triangles,
            normals = mesh.normals,
            uv = mesh.uv,
            name = mesh.name
        };

        string fileName = $"mesh_{modelKey}_{DateTime.Now.Ticks}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(serializableMesh);
        File.WriteAllText(filePath, json);

        return fileName;
    }

    private string SaveMaterialData(Material material, string modelKey)
    {
        if (material == null) return "";

        Dictionary<string, string> textureFiles = new Dictionary<string, string>();
        if (serverModelCache.ContainsKey(modelKey) && serverModelCache[modelKey].cachedTextures != null)
        {
            foreach (var kvp in serverModelCache[modelKey].cachedTextures)
            {
                string textureFileName = SaveTextureData(kvp.Value, $"{modelKey}_{kvp.Key}");
                if (!string.IsNullOrEmpty(textureFileName))
                {
                    textureFiles[kvp.Key] = textureFileName;
                }
            }
        }

        Color materialColor = Color.white;
        if (material.HasProperty("_Color"))
            materialColor = material.color;
        else if (material.HasProperty("_BaseColor"))
            materialColor = material.GetColor("_BaseColor");
        else if (material.HasProperty("_MainColor"))
            materialColor = material.GetColor("_MainColor");

        SerializableMaterial serializableMaterial = new SerializableMaterial
        {
            name = material.name,
            color = materialColor,
            texturePath = textureFiles.ContainsKey("_MainTex") ? textureFiles["_MainTex"] :
                         textureFiles.ContainsKey("_Texture2D") ? textureFiles["_Texture2D"] :
                         (textureFiles.Count > 0 ? textureFiles.First().Value : ""),
            isTransparent = material.name.Contains("Transparent"),
            shaderName = material.shader != null ? material.shader.name : "",
            textureProperties = textureFiles
        };

        string fileName = $"material_{modelKey}_{DateTime.Now.Ticks}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(serializableMaterial);
        File.WriteAllText(filePath, json);

        return fileName;
    }

    private string SaveTextureData(Texture2D texture, string modelKey)
    {
        if (texture == null) return "";

        try
        {
            Texture2D readableTexture = texture;
            if (!texture.isReadable)
            {
                readableTexture = MakeTextureReadable(texture);
            }

            byte[] textureData = readableTexture.EncodeToPNG();
            string fileName = $"texture_{modelKey}_{DateTime.Now.Ticks}.png";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, textureData);

            return fileName;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save texture data: {e.Message}");
            return "";
        }
    }

    private Texture2D MakeTextureReadable(Texture2D original)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(original.width, original.height);
        Graphics.Blit(original, renderTexture);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D readableTexture = new Texture2D(original.width, original.height);
        readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return readableTexture;
    }

    // ===========================================
    // SAVE SESSION WITH MEASUREMENTS AND DRAWINGS
    // ===========================================
    public void SaveSession(string saveFileName)
    {
        SessionData session = new SessionData();
        GameObject[] models = GameObject.FindGameObjectsWithTag(modelTag);

        Debug.Log($"=== SAVING SESSION: {models.Length} models ===");

        // Save models
        foreach (var model in models)
        {
            string modelKey = model.name.Replace("(Clone)", "");
            bool isServer = serverModelCache.ContainsKey(modelKey);
            string instanceId = GetModelInstanceId(model);

            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = GenerateModelInstanceId();
                SetModelInstanceId(model, instanceId);
            }

            ModelData data = new ModelData
            {
                position = model.transform.position,
                rotation = model.transform.rotation,
                scale = model.transform.localScale,
                tag = model.tag,
                isServerModel = isServer,
                modelPath = isServer ? modelKey : "Models/" + modelKey,
                modelInstanceId = instanceId
            };

            var photonView = model.GetComponent<Photon.Pun.PhotonView>();
            if (photonView != null)
            {
                data.photonViewID = photonView.ViewID;
            }

            if (isServer && serverModelCache.ContainsKey(modelKey))
            {
                ServerModelCache cache = serverModelCache[modelKey];
                data.modelId = cache.modelId;
                data.modelName = cache.modelName;
                data.isTransparent = cache.isTransparent;
                data.isChild = cache.isChild;
                data.modelUrl = cache.modelUrl;
                data.textureUrl = cache.textureUrl;
                data.meshDataPath = SaveMeshData(cache.mesh, modelKey);
                data.materialDataPath = SaveMaterialData(cache.material, modelKey);
            }
            else
            {
                data.modelId = "";
                data.modelName = modelKey;
            }

            session.models.Add(data);
            Debug.Log($"? Saved model: {data.modelName} (ID: {instanceId})");
        }

        // Save measurements
        MeasurmentInk[] allMeasurements = FindObjectsOfType<MeasurmentInk>();
        Debug.Log($"=== SAVING {allMeasurements.Length} MEASUREMENTS ===");

        foreach (var measurement in allMeasurements)
        {
            LineRenderer lineRenderer = measurement.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogWarning($"Measurement missing LineRenderer: {measurement.name}");
                continue;
            }

            var measurementType = typeof(MeasurmentInk);

            var storedPointsField = measurementType.GetField("storedPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isStraightLineField = measurementType.GetField("isStraightLine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var originalMeasurementField = measurementType.GetField("originalMeasurement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var canvasLocalPositionField = measurementType.GetField("canvasLocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var point1LocalPositionField = measurementType.GetField("point1LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var point2LocalPositionField = measurementType.GetField("point2LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isAttachedToModelField = measurementType.GetField("isAttachedToModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var attachedModelIDField = measurementType.GetField("attachedModelID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Vector3[] storedPoints = storedPointsField?.GetValue(measurement) as Vector3[];
            if (storedPoints == null || storedPoints.Length < 2)
            {
                Debug.LogWarning($"Measurement has no points: {measurement.name}");
                continue;
            }

            MeasurementData measureData = new MeasurementData
            {
                storedPoints = storedPoints,
                isStraightLine = (bool)(isStraightLineField?.GetValue(measurement) ?? false),
                originalMeasurement = (float)(originalMeasurementField?.GetValue(measurement) ?? 0f),
                canvasLocalPosition = (Vector3)(canvasLocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                point1LocalPosition = (Vector3)(point1LocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                point2LocalPosition = (Vector3)(point2LocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                isAttachedToModel = (bool)(isAttachedToModelField?.GetValue(measurement) ?? false),
                attachedModelID = (string)(attachedModelIDField?.GetValue(measurement) ?? ""),
                position = measurement.transform.position,
                rotation = measurement.transform.rotation,
                scale = measurement.transform.localScale,
                lineWidth = lineRenderer.startWidth
            };

            if (lineRenderer.material != null && lineRenderer.material.HasProperty("_Color"))
            {
                measureData.lineColor = lineRenderer.material.color;
            }

            if (measureData.isAttachedToModel && measurement.transform.parent != null)
            {
                string parentInstanceId = GetModelInstanceId(measurement.transform.parent.gameObject);
                if (!string.IsNullOrEmpty(parentInstanceId))
                {
                    measureData.attachedModelID = parentInstanceId;
                    Debug.Log($"  - Measurement attached to parent with instanceId: {parentInstanceId}");
                }
                else
                {
                    Debug.LogWarning($"  - Has parent but no instanceId found, saving as independent");
                    measureData.isAttachedToModel = false;
                    measureData.attachedModelID = "";
                }
            }
            else if (measureData.isAttachedToModel && string.IsNullOrEmpty(measureData.attachedModelID))
            {
                Debug.LogWarning($"  - Marked as attached but no parent found, saving as independent");
                measureData.isAttachedToModel = false;
            }

            session.measurements.Add(measureData);

            string pointsPreview = measureData.storedPoints.Length > 0 ?
                $"First: {measureData.storedPoints[0]}, Last: {measureData.storedPoints[measureData.storedPoints.Length - 1]}" :
                "No points";
            Debug.Log($"? Saved measurement #{session.measurements.Count}:\n" +
                     $"  - Points: {measureData.storedPoints.Length} ({pointsPreview})\n" +
                     $"  - Mode: {(measureData.isStraightLine ? "Straight" : "Curved")}\n" +
                     $"  - Attached: {measureData.isAttachedToModel}\n" +
                     $"  - Model ID: {measureData.attachedModelID}");
        }

        // Save drawings
        LineRenderer[] allDrawings = FindObjectsOfType<LineRenderer>();
        Debug.Log($"=== SAVING DRAWINGS ===");
        int drawingCount = 0;

        foreach (var drawing in allDrawings)
        {
            // Skip if this is a measurement line (has MeasurmentInk component)
            if (drawing.GetComponent<MeasurmentInk>() != null)
                continue;

            // Check if this is a pen drawing (has DrawingTracker component)
            DrawingTracker tracker = drawing.GetComponent<DrawingTracker>();
            if (tracker == null)
                continue;

            // Get all points from the line renderer
            Vector3[] points = new Vector3[drawing.positionCount];
            for (int i = 0; i < drawing.positionCount; i++)
            {
                points[i] = drawing.GetPosition(i);
            }

            if (points.Length < 2)
            {
                Debug.LogWarning($"Drawing has insufficient points: {drawing.name}");
                continue;
            }

            DrawingSessionData drawingData = new DrawingSessionData
            {
                points = points,
                width = drawing.startWidth,
                drawingId = tracker.drawingId,
                useWorldSpace = drawing.useWorldSpace
            };

            // Get line color
            if (drawing.material != null && drawing.material.HasProperty("_Color"))
            {
                drawingData.lineColor = drawing.material.color;
            }
            else
            {
                drawingData.lineColor = Color.white;
            }

            // Default color index
            drawingData.colorIndex = 0;

            // Check if drawing is attached to a model
            if (drawing.transform.parent != null)
            {
                string parentInstanceId = GetModelInstanceId(drawing.transform.parent.gameObject);
                if (!string.IsNullOrEmpty(parentInstanceId))
                {
                    drawingData.isAttachedToModel = true;
                    drawingData.attachedModelID = parentInstanceId;
                    drawingData.parentPosition = drawing.transform.parent.position;
                    drawingData.parentRotation = drawing.transform.parent.rotation;
                    Debug.Log($"  - Drawing attached to model: {parentInstanceId} (parent: {drawing.transform.parent.name})");
                }
                else
                {
                    drawingData.isAttachedToModel = false;
                }
            }
            else
            {
                drawingData.isAttachedToModel = false;
            }

            session.drawings.Add(drawingData);
            drawingCount++;

            string drawingPointsPreview = points.Length > 0 ?
                $"First: {points[0]}, Last: {points[points.Length - 1]}" :
                "No points";
            Debug.Log($"? Saved drawing #{drawingCount}:\n" +
                     $"  - Points: {points.Length} ({drawingPointsPreview})\n" +
                     $"  - Width: {drawingData.width}\n" +
                     $"  - Color: {drawingData.lineColor}\n" +
                     $"  - Attached: {drawingData.isAttachedToModel}\n" +
                     $"  - Model ID: {drawingData.attachedModelID}\n" +
                     $"  - UseWorldSpace: {drawingData.useWorldSpace}");
        }

        string json = JsonUtility.ToJson(session, true);
        string filePath = Application.persistentDataPath + "/" + saveFileName + ".json";
        File.WriteAllText(filePath, json);
        Debug.Log($"=== SESSION SAVED: {filePath} ===\nModels: {session.models.Count}, Measurements: {session.measurements.Count}, Drawings: {session.drawings.Count}");
    }

    // ===========================================
    // LOAD SESSION WITH MEASUREMENTS AND DRAWINGS
    // ===========================================
    public void LoadSession(string saveFileName)
    {
        string path = Application.persistentDataPath + "/" + saveFileName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        SessionData session = JsonUtility.FromJson<SessionData>(json);

        Debug.Log($"=== LOADING SESSION ===\nModels: {session.models.Count}, Measurements: {session.measurements.Count}, Drawings: {session.drawings.Count}");

        // Destroy existing models
        GameObject[] existingModels = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in existingModels)
        {
            Destroy(model);
        }

        // Destroy existing measurements
        MeasurmentInk[] existingMeasurements = FindObjectsOfType<MeasurmentInk>();
        foreach (var measurement in existingMeasurements)
        {
            Destroy(measurement.gameObject);
        }

        // Destroy existing drawings
        DrawingTracker[] existingDrawings = FindObjectsOfType<DrawingTracker>();
        foreach (var drawing in existingDrawings)
        {
            Destroy(drawing.gameObject);
        }

        serverModelCache.Clear();
        modelInstanceMap.Clear();

        StartCoroutine(LoadModelsAndMeasurementsAndDrawingsAfterCleanup(session.models, session.measurements, session.drawings));
    }

    private IEnumerator LoadModelsAndMeasurementsAndDrawingsAfterCleanup(List<ModelData> models, List<MeasurementData> measurements, List<DrawingSessionData> drawings)
    {
        yield return null;

        // Load all models first
        Debug.Log("=== LOADING MODELS ===");
        foreach (var modelData in models)
        {
            if (modelData.isServerModel)
            {
                if (modelSample != null && !string.IsNullOrEmpty(modelData.modelId))
                {
                    modelSample.LoadModel(modelData.modelId, modelData.modelName, modelData.isTransparent, modelData.isChild);
                    yield return StartCoroutine(WaitAndApplyTransformWithFallback(modelData));
                }
                else
                {
                    CreatePlaceholderModel(modelData);
                }
            }
            else
            {
                LoadLocalModel(modelData);
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Wait for all models to be ready
        yield return new WaitForSeconds(1f);

        // Now load measurements
        Debug.Log($"=== LOADING {measurements.Count} MEASUREMENTS ===");
        foreach (var measurementData in measurements)
        {
            LoadMeasurement(measurementData);
            yield return new WaitForSeconds(0.2f);
        }

        // Load drawings
        Debug.Log($"=== LOADING {drawings.Count} DRAWINGS ===");
        foreach (var drawingData in drawings)
        {
            LoadDrawing(drawingData);
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("=== SESSION LOADING COMPLETE ===");
    }

    private void LoadDrawing(DrawingSessionData drawingData)
    {
        // Create a new GameObject for the drawing
        GameObject drawingObj = new GameObject("LoadedDrawing_" + drawingData.drawingId);
        LineRenderer lineRenderer = drawingObj.AddComponent<LineRenderer>();

        if (drawingMaterial != null)
        {
            lineRenderer.material = new Material(drawingMaterial);
        }
        else
        {
            // Fallback if material not assigned
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            Debug.LogWarning("Drawing material not assigned, using default shader");
        }

        // CRITICAL: Find and parent FIRST, before setting any positions
        GameObject parentModel = null;
        if (drawingData.isAttachedToModel && !string.IsNullOrEmpty(drawingData.attachedModelID))
        {
            if (modelInstanceMap.TryGetValue(drawingData.attachedModelID, out parentModel))
            {
                // Parent the drawing BEFORE setting coordinate space
                drawingObj.transform.SetParent(parentModel.transform, false);
                drawingObj.transform.localPosition = Vector3.zero;
                drawingObj.transform.localRotation = Quaternion.identity;
                drawingObj.transform.localScale = Vector3.one;

                // Use LOCAL space when parented (matches Pen.cs behavior)
                lineRenderer.useWorldSpace = false;

                Debug.Log($"? Drawing parented to model: {parentModel.name}");
            }
            else
            {
                Debug.LogWarning($"? Parent model not found for drawing (ID: {drawingData.attachedModelID})");
                drawingData.isAttachedToModel = false;
                // Use WORLD space if parent not found
                lineRenderer.useWorldSpace = true;
            }
        }
        else
        {
            // Independent drawing - use world space
            lineRenderer.useWorldSpace = true;
        }

        // Set line width
        lineRenderer.startWidth = drawingData.width;
        lineRenderer.endWidth = drawingData.width;

        // Set color
        if (drawingData.lineColor != Color.clear)
        {
            lineRenderer.startColor = drawingData.lineColor;
            lineRenderer.endColor = drawingData.lineColor;
            if (lineRenderer.material.HasProperty("_Color"))
            {
                lineRenderer.material.color = drawingData.lineColor;
            }
        }

        // Set line positions based on coordinate space
        lineRenderer.positionCount = drawingData.points.Length;

        if (drawingData.isAttachedToModel && parentModel != null)
        {
            // Drawing is attached - points are stored in LOCAL space
            // Since useWorldSpace = false, we can set them directly
            lineRenderer.SetPositions(drawingData.points);

            Debug.Log($"? Set {drawingData.points.Length} LOCAL space points for attached drawing");
            if (drawingData.points.Length > 0)
            {
                Debug.Log($"  First local point: {drawingData.points[0]}, Last: {drawingData.points[drawingData.points.Length - 1]}");
            }
        }
        else if (!drawingData.useWorldSpace && drawingData.isAttachedToModel)
        {
            // Parent is missing but drawing was in local space
            // Convert to world space using stored parent transform
            lineRenderer.useWorldSpace = true;
            Matrix4x4 parentMatrix = Matrix4x4.TRS(drawingData.parentPosition, drawingData.parentRotation, Vector3.one);

            for (int i = 0; i < drawingData.points.Length; i++)
            {
                Vector3 worldPoint = parentMatrix.MultiplyPoint3x4(drawingData.points[i]);
                lineRenderer.SetPosition(i, worldPoint);
            }

            Debug.Log($"? Converted {drawingData.points.Length} points from local to world space (parent missing)");
        }
        else
        {
            // Independent drawing - points are in world space
            lineRenderer.SetPositions(drawingData.points);

            Debug.Log($"? Set {drawingData.points.Length} WORLD space points for independent drawing");
        }

        // Add tracking component
        DrawingTracker tracker = drawingObj.AddComponent<DrawingTracker>();
        tracker.drawingId = drawingData.drawingId;

        // Add Deletable component
        drawingObj.AddComponent<Deletable>();

        // Set layer
        if (drawingLayerMask != 0)
        {
            drawingObj.layer = (int)Mathf.Log(drawingLayerMask.value, 2);
        }

        // Add collider - handle both world and local space
        BoxCollider collider = drawingObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        if (lineRenderer.useWorldSpace)
        {
            // World space - use bounds
            if (lineRenderer.bounds.size.magnitude > 0)
            {
                collider.center = lineRenderer.bounds.center;
                collider.size = lineRenderer.bounds.size;
            }
            else
            {
                collider.size = Vector3.one * 0.1f;
            }
        }
        else
        {
            // Local space - calculate bounds manually
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 point = lineRenderer.GetPosition(i);
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;

            // Ensure minimum size
            size.x = Mathf.Max(size.x, 0.01f);
            size.y = Mathf.Max(size.y, 0.01f);
            size.z = Mathf.Max(size.z, 0.01f);

            collider.center = center;
            collider.size = size;
        }

        Debug.Log($"? Loaded drawing: {drawingData.points.Length} points, attached: {drawingData.isAttachedToModel}, width: {drawingData.width}, useWorldSpace: {lineRenderer.useWorldSpace}");
    }


    ///new abovee

    private void LoadMeasurement(MeasurementData measurementData)
    {
        GameObject measurementPrefab = Resources.Load<GameObject>("MesurmentInk");
        if (measurementPrefab == null)
        {
            Debug.LogError("MesurmentInk prefab not found in Resources!");
            return;
        }

        GameObject measurementObj = Instantiate(measurementPrefab, Vector3.zero, Quaternion.identity);
        measurementObj.name = "LoadedMeasurement";

        MeasurmentInk inkComponent = measurementObj.GetComponent<MeasurmentInk>();
        LineRenderer lineRenderer = measurementObj.GetComponent<LineRenderer>();

        if (inkComponent == null || lineRenderer == null)
        {
            Debug.LogError("MesurmentInk or LineRenderer component missing!");
            Destroy(measurementObj);
            return;
        }

        var measurementType = typeof(MeasurmentInk);

        var storedPointsField = measurementType.GetField("storedPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isStraightLineField = measurementType.GetField("isStraightLine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var originalMeasurementField = measurementType.GetField("originalMeasurement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasLocalPositionField = measurementType.GetField("canvasLocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point1LocalPositionField = measurementType.GetField("point1LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point2LocalPositionField = measurementType.GetField("point2LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isAttachedToModelField = measurementType.GetField("isAttachedToModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attachedModelIDField = measurementType.GetField("attachedModelID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isInitializedField = measurementType.GetField("isInitialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attachedModelField = measurementType.GetField("attachedModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasField = measurementType.GetField("canvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point1Field = measurementType.GetField("point1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point2Field = measurementType.GetField("point2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Transform canvas = canvasField?.GetValue(inkComponent) as Transform;
        GameObject point1 = point1Field?.GetValue(inkComponent) as GameObject;
        GameObject point2 = point2Field?.GetValue(inkComponent) as GameObject;

        GameObject parentModel = null;
        if (measurementData.isAttachedToModel && !string.IsNullOrEmpty(measurementData.attachedModelID))
        {
            if (modelInstanceMap.TryGetValue(measurementData.attachedModelID, out parentModel))
            {
                // IMPORTANT: Parent BEFORE setting any positions
                measurementObj.transform.SetParent(parentModel.transform, false);
                measurementObj.transform.localPosition = Vector3.zero;
                measurementObj.transform.localRotation = Quaternion.identity;
                measurementObj.transform.localScale = Vector3.one;

                attachedModelField?.SetValue(inkComponent, parentModel.transform);

                Debug.Log($"? Measurement parented to model: {parentModel.name}");
            }
            else
            {
                Debug.LogWarning($"? Parent model not found (ID: {measurementData.attachedModelID})");
                measurementData.isAttachedToModel = false;
            }
        }

        // CRITICAL: For attached measurements, storedPoints must be in LOCAL space
        // The measurement script expects local coordinates when attached to a model
        Vector3[] pointsToStore;
        if (measurementData.isAttachedToModel && parentModel != null)
        {
            // Points are already in local space from save, use them directly
            pointsToStore = measurementData.storedPoints;
        }
        else
        {
            // Independent measurement - points are in world space
            pointsToStore = measurementData.storedPoints;
        }

        // Set component fields
        storedPointsField?.SetValue(inkComponent, pointsToStore);
        isStraightLineField?.SetValue(inkComponent, measurementData.isStraightLine);
        originalMeasurementField?.SetValue(inkComponent, measurementData.originalMeasurement);
        canvasLocalPositionField?.SetValue(inkComponent, measurementData.canvasLocalPosition);
        point1LocalPositionField?.SetValue(inkComponent, measurementData.point1LocalPosition);
        point2LocalPositionField?.SetValue(inkComponent, measurementData.point2LocalPosition);
        isAttachedToModelField?.SetValue(inkComponent, measurementData.isAttachedToModel);
        attachedModelIDField?.SetValue(inkComponent, measurementData.attachedModelID);
        isInitializedField?.SetValue(inkComponent, true);

        // Set line properties
        lineRenderer.startWidth = lineRenderer.endWidth = measurementData.lineWidth;
        lineRenderer.useWorldSpace = true;

        if (measurementData.lineColor != Color.clear && lineRenderer.material != null)
        {
            lineRenderer.material.color = measurementData.lineColor;
        }

        // KEY FIX: Set line positions correctly based on attachment
        if (measurementData.isAttachedToModel && parentModel != null)
        {
            // Points are stored in LOCAL space relative to the parent model
            // We need to transform them to WORLD space for the LineRenderer (which uses world space)

            if (measurementData.isStraightLine && measurementData.storedPoints.Length >= 2)
            {
                lineRenderer.positionCount = 2;

                // Transform local points to world space
                Vector3 worldPoint1 = parentModel.transform.TransformPoint(measurementData.storedPoints[0]);
                Vector3 worldPoint2 = parentModel.transform.TransformPoint(measurementData.storedPoints[measurementData.storedPoints.Length - 1]);

                lineRenderer.SetPosition(0, worldPoint1);
                lineRenderer.SetPosition(1, worldPoint2);

                Debug.Log($"  Line points (world): Start={worldPoint1}, End={worldPoint2}");
            }
            else
            {
                // Curved line - transform all points
                lineRenderer.positionCount = measurementData.storedPoints.Length;
                Vector3[] worldPoints = new Vector3[measurementData.storedPoints.Length];

                for (int i = 0; i < measurementData.storedPoints.Length; i++)
                {
                    worldPoints[i] = parentModel.transform.TransformPoint(measurementData.storedPoints[i]);
                }

                lineRenderer.SetPositions(worldPoints);

                if (worldPoints.Length > 0)
                {
                    Debug.Log($"  Curved line points (world): Start={worldPoints[0]}, End={worldPoints[worldPoints.Length - 1]}");
                }
            }

            // Position the canvas in world space
            if (canvas != null && measurementData.canvasLocalPosition != Vector3.zero)
            {
                Vector3 worldCanvasPos = parentModel.transform.TransformPoint(measurementData.canvasLocalPosition);
                canvas.position = worldCanvasPos;
                Debug.Log($"  Canvas world pos: {worldCanvasPos}");
            }

            // Position point1 in world space
            if (point1 != null && measurementData.point1LocalPosition != Vector3.zero)
            {
                Vector3 worldPoint1Pos = parentModel.transform.TransformPoint(measurementData.point1LocalPosition);
                point1.transform.position = worldPoint1Pos;
                Debug.Log($"  Point1 world pos: {worldPoint1Pos}");
            }

            // Position point2 in world space
            if (point2 != null && measurementData.point2LocalPosition != Vector3.zero)
            {
                Vector3 worldPoint2Pos = parentModel.transform.TransformPoint(measurementData.point2LocalPosition);
                point2.transform.position = worldPoint2Pos;
                Debug.Log($"  Point2 world pos: {worldPoint2Pos}");
            }
        }
        else
        {
            // Independent measurement - use saved world positions directly
            measurementObj.transform.position = measurementData.position;
            measurementObj.transform.rotation = measurementData.rotation;
            measurementObj.transform.localScale = measurementData.scale;

            if (measurementData.isStraightLine && measurementData.storedPoints.Length >= 2)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, measurementData.storedPoints[0]);
                lineRenderer.SetPosition(1, measurementData.storedPoints[measurementData.storedPoints.Length - 1]);
            }
            else
            {
                lineRenderer.positionCount = measurementData.storedPoints.Length;
                lineRenderer.SetPositions(measurementData.storedPoints);
            }

            if (canvas != null && measurementData.canvasLocalPosition != Vector3.zero)
            {
                canvas.position = measurementData.canvasLocalPosition;
            }

            if (point1 != null && measurementData.point1LocalPosition != Vector3.zero)
            {
                point1.transform.position = measurementData.point1LocalPosition;
            }

            if (point2 != null && measurementData.point2LocalPosition != Vector3.zero)
            {
                point2.transform.position = measurementData.point2LocalPosition;
            }

            Debug.Log("? Created independent measurement");
        }

        // Update the measurement text
        var lengthTextField = measurementType.GetField("lengthText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lengthText = lengthTextField?.GetValue(inkComponent) as TMPro.TMP_Text;
        if (lengthText != null)
        {
            lengthText.text = $"{measurementData.originalMeasurement:0.00} mm";
        }

        // Orient canvas to camera
        if (canvas != null)
        {
            var orientMethod = measurementType.GetMethod("OrientCanvasToHead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            orientMethod?.Invoke(inkComponent, null);
        }

        Debug.Log($"? Loaded measurement: {measurementData.storedPoints.Length} points, mode: {(measurementData.isStraightLine ? "Straight" : "Curved")}, value: {measurementData.originalMeasurement:F2}mm, attached: {measurementData.isAttachedToModel}");
    }


    ///newww aboovee

    private IEnumerator WaitAndApplyTransformWithFallback(ModelData modelData)
    {
        float timeout = 10f;
        float elapsed = 0f;
        GameObject foundModel = null;
        string expectedName = modelData.modelPath;

        while (elapsed < timeout && foundModel == null)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;

            GameObject[] currentModels = GameObject.FindGameObjectsWithTag(modelTag);
            foreach (var model in currentModels)
            {
                string modelKey = model.name.Replace("(Clone)", "");
                if (modelKey == expectedName)
                {
                    MeshFilter mf = model.GetComponent<MeshFilter>();
                    if (mf != null && mf.mesh != null)
                    {
                        foundModel = model;
                        break;
                    }
                }
            }
        }

        if (foundModel != null)
        {
            foundModel.transform.position = modelData.position;
            foundModel.transform.rotation = modelData.rotation;
            foundModel.transform.localScale = modelData.scale;

            SetModelInstanceId(foundModel, modelData.modelInstanceId);

            Debug.Log($"? Model loaded: {foundModel.name} (ID: {modelData.modelInstanceId})");

            CacheServerModel(foundModel, modelData.modelId, modelData.modelName,
                           modelData.isTransparent, modelData.isChild,
                           modelData.modelUrl, modelData.textureUrl);
        }
        else
        {
            Debug.LogWarning($"? Download failed for {modelData.modelName}, trying fallback...");

            if (!string.IsNullOrEmpty(modelData.meshDataPath) && !string.IsNullOrEmpty(modelData.materialDataPath))
            {
                yield return StartCoroutine(LoadModelFromSavedFiles(modelData));
            }
            else
            {
                CreatePlaceholderModel(modelData);
            }
        }
    }

    private IEnumerator LoadModelFromSavedFiles(ModelData modelData)
    {
        string modelKey = modelData.modelPath;
        GameObject basePrefab = Resources.Load<GameObject>(modelKey);

        if (basePrefab == null)
        {
            CreatePlaceholderModel(modelData);
            yield break;
        }

        GameObject model = Instantiate(basePrefab);
        model.name = basePrefab.name;
        model.tag = modelTag;

        SetModelInstanceId(model, modelData.modelInstanceId);

        string meshPath = Path.Combine(Application.persistentDataPath, modelData.meshDataPath);
        if (File.Exists(meshPath))
        {
            string meshJson = File.ReadAllText(meshPath);
            SerializableMesh serializableMesh = JsonUtility.FromJson<SerializableMesh>(meshJson);

            Mesh mesh = new Mesh
            {
                vertices = serializableMesh.vertices,
                triangles = serializableMesh.triangles,
                normals = serializableMesh.normals,
                uv = serializableMesh.uv,
                name = serializableMesh.name
            };

            MeshFilter meshFilter = model.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = mesh;
            }
        }

        string materialPath = Path.Combine(Application.persistentDataPath, modelData.materialDataPath);
        if (File.Exists(materialPath))
        {
            yield return StartCoroutine(LoadAndApplyMaterial(model, materialPath, modelData));
        }

        ApplyModelTransform(model, modelData);

        CacheServerModel(model, modelData.modelId, modelData.modelName,
                        modelData.isTransparent, modelData.isChild,
                        modelData.modelUrl, modelData.textureUrl);
    }

    private IEnumerator LoadAndApplyMaterial(GameObject model, string materialPath, ModelData modelData)
    {
        string materialJson = File.ReadAllText(materialPath);
        SerializableMaterial serializableMaterial = JsonUtility.FromJson<SerializableMaterial>(materialJson);

        Material baseMaterial = serializableMaterial.isTransparent
            ? Resources.Load<Material>("Cross-Section-Material_Transparent")
            : Resources.Load<Material>("Cross-Section-Material");

        if (baseMaterial == null)
        {
            yield break;
        }

        Material material = new Material(baseMaterial);
        material.name = serializableMaterial.name;

        if (material.HasProperty("_Color"))
        {
            material.color = serializableMaterial.color;
        }
        else if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", serializableMaterial.color);
        }

        if (serializableMaterial.textureProperties != null)
        {
            foreach (var kvp in serializableMaterial.textureProperties)
            {
                string propertyName = kvp.Key;
                string textureFileName = kvp.Value;

                if (!string.IsNullOrEmpty(textureFileName))
                {
                    string texturePath = Path.Combine(Application.persistentDataPath, textureFileName);
                    if (File.Exists(texturePath))
                    {
                        byte[] textureData = File.ReadAllBytes(texturePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(textureData))
                        {
                            if (material.HasProperty(propertyName))
                            {
                                material.SetTexture(propertyName, texture);
                            }
                        }
                    }
                }
            }
        }

        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        yield return null;
    }

    private void LoadLocalModel(ModelData modelData)
    {
        GameObject prefab = Resources.Load<GameObject>(modelData.modelPath);
        GameObject model;

        if (prefab != null)
        {
            model = Instantiate(prefab);
            model.name = prefab.name;
        }
        else
        {
            model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            model.name = modelData.modelPath.Replace("Models/", "");
        }

        SetModelInstanceId(model, modelData.modelInstanceId);
        ApplyModelTransform(model, modelData);
        Debug.Log($"? Loaded local model: {model.name}");
    }

    private void ApplyModelTransform(GameObject model, ModelData modelData)
    {
        model.transform.position = modelData.position;
        model.transform.rotation = modelData.rotation;
        model.transform.localScale = modelData.scale;
        model.tag = modelData.tag;
    }

    private void CreatePlaceholderModel(ModelData modelData)
    {
        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.name = modelData.modelName + "_Placeholder";

        SetModelInstanceId(model, modelData.modelInstanceId);
        ApplyModelTransform(model, modelData);

        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        Debug.LogWarning($"? Created placeholder for: {modelData.modelName}");
    }

    public void OnServerModelLoaded(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild)
    {
        if (model != null)
        {
            string modelUrl = "";
            string textureUrl = "";

            if (modelSample != null)
            {
                modelUrl = modelSample.CurrentModelUrl;
                textureUrl = modelSample.CurrentTextureUrl;
            }

            string actualModelId = modelId;
            if (!string.IsNullOrEmpty(modelUrl) && modelUrl.Contains("model_id="))
            {
                int startIndex = modelUrl.IndexOf("model_id=") + "model_id=".Length;
                int endIndex = modelUrl.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = modelUrl.Length;

                string extractedId = modelUrl.Substring(startIndex, endIndex - startIndex);
                if (!string.IsNullOrEmpty(extractedId))
                {
                    actualModelId = extractedId;
                }
            }

            CacheServerModel(model, actualModelId, modelName, isTransparent, isChild, modelUrl, textureUrl);
        }
    }

    public void ClearCache()
    {
        serverModelCache.Clear();
        modelInstanceMap.Clear();
        Debug.Log("Cache cleared");
    }

    public void ClearSavedData()
    {
        string[] meshFiles = Directory.GetFiles(Application.persistentDataPath, "mesh_*.json");
        string[] materialFiles = Directory.GetFiles(Application.persistentDataPath, "material_*.json");
        string[] textureFiles = Directory.GetFiles(Application.persistentDataPath, "texture_*.png");

        foreach (string file in meshFiles.Concat(materialFiles).Concat(textureFiles))
        {
            File.Delete(file);
        }

        Debug.Log("Cleared all saved data");
    }

    public bool HasCachedModel(string modelKey)
    {
        return serverModelCache.ContainsKey(modelKey);
    }

    public void SaveCurrentSession()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        SaveSession("Session_" + timestamp);
    }
}

// Helper component to store unique instance IDs on models
public class ModelInstanceId : MonoBehaviour
{
    public string instanceId;
}

// Note: DrawingTracker is already defined in Pen.cs - don't duplicate it here*/






/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////workingggg





/*using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ModelData
{
    public string modelPath;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool isServerModel;
    public string modelId;
    public string modelName;
    public bool isTransparent;
    public bool isChild;
    public string modelInstanceId;
    public string modelUrl;
    public string textureUrl;
    public string meshDataPath;
    public string materialDataPath;
}

[Serializable]
public class MeasurementData
{
    public Vector3[] storedPoints;
    public bool isStraightLine;
    public float originalMeasurement;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector3 canvasLocalPosition;
    public Vector3 point1LocalPosition;
    public Vector3 point2LocalPosition;
    public bool isAttachedToModel;
    public string attachedModelID;
    public float lineWidth;
    public Color lineColor;
}

[Serializable]
public class DrawingSessionData
{
    public Vector3[] points;
    public int colorIndex;
    public float width;
    public Color lineColor;
    public string drawingId;
    public bool useWorldSpace;
    public Vector3 parentPosition;
    public Quaternion parentRotation;
    public bool isAttachedToModel;
    public string attachedModelID;
}

[Serializable]
public class SessionData
{
    public List<ModelData> models = new List<ModelData>();
    public List<MeasurementData> measurements = new List<MeasurementData>();
    public List<DrawingSessionData> drawings = new List<DrawingSessionData>();
}

[Serializable]
public class SerializableMesh
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uv;
    public string name;
}

[Serializable]
public class SerializableMaterial
{
    public string name;
    public Color color;
    public string texturePath;
    public bool isTransparent;
    public string shaderName;
    public Dictionary<string, string> textureProperties;
}

public class SessionManager : MonoBehaviour
{
    private const string modelTag = "ModelPart";

    [Header("References")]
    public ModelSample modelSample;
    public Material drawingMaterial;
    [SerializeField] private LayerMask drawingLayerMask;

    private Dictionary<string, ServerModelCache> serverModelCache = new Dictionary<string, ServerModelCache>();
    private Dictionary<string, GameObject> modelInstanceMap = new Dictionary<string, GameObject>();
 

    [Serializable]
    private class ServerModelCache
    {
        public Mesh mesh;
        public Material material;
        public string modelId;
        public string modelName;
        public bool isTransparent;
        public bool isChild;
        public string modelUrl;
        public string textureUrl;
        public GameObject prefab;
        public Dictionary<string, Texture2D> cachedTextures;
    }

    void Start()
    {
        if (modelSample == null)
        {
            modelSample = FindObjectOfType<ModelSample>();
            if (modelSample == null)
            {
                Debug.LogError("ModelSample not found! Please assign it in the inspector.");
            }
        }

        if (drawingMaterial == null)
        {
            Debug.LogWarning("Drawing material not assigned! Attempting to find it...");
            drawingMaterial = Resources.Load<Material>("DrawingMaterial");
            if (drawingMaterial == null)
            {
                Debug.LogError("Drawing material not found in Resources! Drawings may not load correctly.");
            }
        }
    }

    private string GenerateModelInstanceId()
    {
        return System.Guid.NewGuid().ToString();
    }

    private void SetModelInstanceId(GameObject model, string instanceId)
    {
        var idComponent = model.GetComponent<ModelInstanceId>();
        if (idComponent == null)
        {
            idComponent = model.AddComponent<ModelInstanceId>();
        }
        idComponent.instanceId = instanceId;
        modelInstanceMap[instanceId] = model;
    }

    private string GetModelInstanceId(GameObject model)
    {
        var idComponent = model.GetComponent<ModelInstanceId>();
        return idComponent != null ? idComponent.instanceId : null;
    }

    public GameObject ImportLocalModel(string prefabName)
    {
        string path = "Models/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Local prefab not found: " + path);
            return null;
        }

        GameObject model = Instantiate(prefab);
        model.name = prefab.name;
        model.tag = modelTag;
        SetModelInstanceId(model, GenerateModelInstanceId());
        return model;
    }




    public void CacheServerModel(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild, string modelUrl, string textureUrl)
    {
        if (model == null)
        {
            Debug.LogError("Cannot cache null model");
            return;
        }

        // Assign instance ID if not exists
        string existingInstanceId = GetModelInstanceId(model);
        if (string.IsNullOrEmpty(existingInstanceId))
        {
            existingInstanceId = GenerateModelInstanceId();
            SetModelInstanceId(model, existingInstanceId);
        }


        // ? FIX: Use modelId + instanceId as cache key to prevent collisions
        string cacheKey;
        if (!string.IsNullOrEmpty(modelId))
        {
            cacheKey = modelId + "_" + existingInstanceId;  // UNIQUE KEY
        }
        else
        {
            cacheKey = model.name.Replace("(Clone)", "") + "_" + existingInstanceId;
        }

        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        Renderer renderer = model.GetComponent<Renderer>();

        if (meshFilter == null || renderer == null)
        {
            Debug.LogError($"Model {cacheKey} missing MeshFilter or Renderer components");
            return;
        }

        GameObject basePrefab = Resources.Load<GameObject>(model.name.Replace("(Clone)", ""));
        Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

        if (renderer.material != null)
        {
            CacheAllMaterialTextures(renderer.material, cachedTextures);
        }

        ServerModelCache cache = new ServerModelCache
        {
            mesh = meshFilter.mesh,
            material = renderer.material,
            modelId = modelId,
            modelName = modelName,
            isTransparent = isTransparent,
            isChild = isChild,
            modelUrl = modelUrl,
            textureUrl = textureUrl,
            prefab = basePrefab,
            cachedTextures = cachedTextures
        };

        serverModelCache[cacheKey] = cache;
        Debug.Log($"? Cached with KEY: {cacheKey}, modelId: {modelId}, name: {modelName}, instanceId: {existingInstanceId}");
    }





    private void CacheAllMaterialTextures(Material material, Dictionary<string, Texture2D> textureCache)
    {
        if (material == null || material.shader == null) return;

        string[] commonTextureProperties = {
            "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap", "_Texture", "_Tex", "_Texture2D",
            "_EmissionMap", "_BumpMap", "_NormalMap", "_OcclusionMap", "_MetallicGlossMap"
        };

        foreach (string propName in commonTextureProperties)
        {
            if (material.HasProperty(propName))
            {
                Texture texture = material.GetTexture(propName);
                if (texture != null && texture is Texture2D)
                {
                    textureCache[propName] = texture as Texture2D;
                }
            }
        }
    }

    private string SaveMeshData(Mesh mesh, string modelKey)
    {
        if (mesh == null) return "";

        SerializableMesh serializableMesh = new SerializableMesh
        {
            vertices = mesh.vertices,
            triangles = mesh.triangles,
            normals = mesh.normals,
            uv = mesh.uv,
            name = mesh.name
        };

        string fileName = $"mesh_{modelKey}_{DateTime.Now.Ticks}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(serializableMesh);
        File.WriteAllText(filePath, json);

        return fileName;
    }

    private string SaveMaterialData(Material material, string modelKey)
    {
        if (material == null) return "";

        Dictionary<string, string> textureFiles = new Dictionary<string, string>();
        if (serverModelCache.ContainsKey(modelKey) && serverModelCache[modelKey].cachedTextures != null)
        {
            foreach (var kvp in serverModelCache[modelKey].cachedTextures)
            {
                string textureFileName = SaveTextureData(kvp.Value, $"{modelKey}_{kvp.Key}");
                if (!string.IsNullOrEmpty(textureFileName))
                {
                    textureFiles[kvp.Key] = textureFileName;
                }
            }
        }

        Color materialColor = Color.white;
        if (material.HasProperty("_Color"))
            materialColor = material.color;
        else if (material.HasProperty("_BaseColor"))
            materialColor = material.GetColor("_BaseColor");
        else if (material.HasProperty("_MainColor"))
            materialColor = material.GetColor("_MainColor");

        SerializableMaterial serializableMaterial = new SerializableMaterial
        {
            name = material.name,
            color = materialColor,
            texturePath = textureFiles.ContainsKey("_MainTex") ? textureFiles["_MainTex"] :
                         textureFiles.ContainsKey("_Texture2D") ? textureFiles["_Texture2D"] :
                         (textureFiles.Count > 0 ? textureFiles.First().Value : ""),
            isTransparent = material.name.Contains("Transparent"),
            shaderName = material.shader != null ? material.shader.name : "",
            textureProperties = textureFiles
        };

        string fileName = $"material_{modelKey}_{DateTime.Now.Ticks}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(serializableMaterial);
        File.WriteAllText(filePath, json);

        return fileName;
    }

    private string SaveTextureData(Texture2D texture, string modelKey)
    {
        if (texture == null) return "";

        try
        {
            Texture2D readableTexture = texture;
            if (!texture.isReadable)
            {
                readableTexture = MakeTextureReadable(texture);
            }

            byte[] textureData = readableTexture.EncodeToPNG();
            string fileName = $"texture_{modelKey}_{DateTime.Now.Ticks}.png";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, textureData);

            return fileName;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save texture data: {e.Message}");
            return "";
        }
    }

    private Texture2D MakeTextureReadable(Texture2D original)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(original.width, original.height);
        Graphics.Blit(original, renderTexture);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D readableTexture = new Texture2D(original.width, original.height);
        readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return readableTexture;
    }

    // ===========================================
    // SAVE SESSION WITH MEASUREMENTS AND DRAWINGS
    // ===========================================
    public void SaveSession(string saveFileName)
    {
        SessionData session = new SessionData();
        GameObject[] models = GameObject.FindGameObjectsWithTag(modelTag);

        Debug.Log($"=== SAVING SESSION: {models.Length} models ===");

         // Save models
    


        foreach (var model in models)
        {
            string instanceId = GetModelInstanceId(model);

            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = GenerateModelInstanceId();
                SetModelInstanceId(model, instanceId);
            }

            // ? Find the correct cache key for this model instance
            string cacheKey = null;
            foreach (var kvp in serverModelCache)
            {
                if (kvp.Key.EndsWith("_" + instanceId))
                {
                    cacheKey = kvp.Key;
                    break;
                }
            }

            bool isServer = !string.IsNullOrEmpty(cacheKey);

            ModelData data = new ModelData
            {
                position = model.transform.position,
                rotation = model.transform.rotation,
                scale = model.transform.localScale,
                tag = model.tag,
                isServerModel = isServer,
                modelPath = cacheKey ?? ("Models/" + model.name.Replace("(Clone)", "")),
                modelInstanceId = instanceId
            };

            if (isServer && serverModelCache.ContainsKey(cacheKey))
            {
                ServerModelCache cache = serverModelCache[cacheKey];
                data.modelId = cache.modelId;
                data.modelName = cache.modelName;
                data.isTransparent = cache.isTransparent;
                data.isChild = cache.isChild;
                data.modelUrl = cache.modelUrl;
                data.textureUrl = cache.textureUrl;
                data.meshDataPath = SaveMeshData(cache.mesh, cacheKey);
                data.materialDataPath = SaveMaterialData(cache.material, cacheKey);
            }
            else
            {
                data.modelId = "";
                data.modelName = model.name.Replace("(Clone)", "");
            }

            session.models.Add(data);
        }


        // Save measurements
        MeasurmentInk[] allMeasurements = FindObjectsOfType<MeasurmentInk>();
        Debug.Log($"=== SAVING {allMeasurements.Length} MEASUREMENTS ===");

        foreach (var measurement in allMeasurements)
        {
            LineRenderer lineRenderer = measurement.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogWarning($"Measurement missing LineRenderer: {measurement.name}");
                continue;
            }

            var measurementType = typeof(MeasurmentInk);

            var storedPointsField = measurementType.GetField("storedPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isStraightLineField = measurementType.GetField("isStraightLine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var originalMeasurementField = measurementType.GetField("originalMeasurement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var canvasLocalPositionField = measurementType.GetField("canvasLocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var point1LocalPositionField = measurementType.GetField("point1LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var point2LocalPositionField = measurementType.GetField("point2LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isAttachedToModelField = measurementType.GetField("isAttachedToModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var attachedModelIDField = measurementType.GetField("attachedModelID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Vector3[] storedPoints = storedPointsField?.GetValue(measurement) as Vector3[];
            if (storedPoints == null || storedPoints.Length < 2)
            {
                Debug.LogWarning($"Measurement has no points: {measurement.name}");
                continue;
            }

            MeasurementData measureData = new MeasurementData
            {
                storedPoints = storedPoints,
                isStraightLine = (bool)(isStraightLineField?.GetValue(measurement) ?? false),
                originalMeasurement = (float)(originalMeasurementField?.GetValue(measurement) ?? 0f),
                canvasLocalPosition = (Vector3)(canvasLocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                point1LocalPosition = (Vector3)(point1LocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                point2LocalPosition = (Vector3)(point2LocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                isAttachedToModel = (bool)(isAttachedToModelField?.GetValue(measurement) ?? false),
                attachedModelID = (string)(attachedModelIDField?.GetValue(measurement) ?? ""),
                position = measurement.transform.position,
                rotation = measurement.transform.rotation,
                scale = measurement.transform.localScale,
                lineWidth = lineRenderer.startWidth
            };

            if (lineRenderer.material != null && lineRenderer.material.HasProperty("_Color"))
            {
                measureData.lineColor = lineRenderer.material.color;
            }

                       if (measureData.isAttachedToModel && measurement.transform.parent != null)
                        {
                            string parentInstanceId = GetModelInstanceId(measurement.transform.parent.gameObject);
                            if (!string.IsNullOrEmpty(parentInstanceId))
                            {
                                measureData.attachedModelID = parentInstanceId;
                                Debug.Log($"  - Measurement attached to parent with instanceId: {parentInstanceId}");
                            }


          
                else
                {
                    Debug.LogWarning($"  - Has parent but no instanceId found, saving as independent");
                    measureData.isAttachedToModel = false;
                    measureData.attachedModelID = "";
                }
            }
            else if (measureData.isAttachedToModel && string.IsNullOrEmpty(measureData.attachedModelID))
            {
                Debug.LogWarning($"  - Marked as attached but no parent found, saving as independent");
                measureData.isAttachedToModel = false;
            }

            session.measurements.Add(measureData);

            string pointsPreview = measureData.storedPoints.Length > 0 ?
                $"First: {measureData.storedPoints[0]}, Last: {measureData.storedPoints[measureData.storedPoints.Length - 1]}" :
                "No points";
            Debug.Log($"? Saved measurement #{session.measurements.Count}:\n" +
                     $"  - Points: {measureData.storedPoints.Length} ({pointsPreview})\n" +
                     $"  - Mode: {(measureData.isStraightLine ? "Straight" : "Curved")}\n" +
                     $"  - Attached: {measureData.isAttachedToModel}\n" +
                     $"  - Model ID: {measureData.attachedModelID}");
        }

        // Save drawings
        LineRenderer[] allDrawings = FindObjectsOfType<LineRenderer>();
        Debug.Log($"=== SAVING DRAWINGS ===");
        int drawingCount = 0;

        foreach (var drawing in allDrawings)
        {
            if (drawing.GetComponent<MeasurmentInk>() != null)
                continue;

            DrawingTracker tracker = drawing.GetComponent<DrawingTracker>();
            if (tracker == null)
                continue;

            Vector3[] points = new Vector3[drawing.positionCount];
            for (int i = 0; i < drawing.positionCount; i++)
            {
                points[i] = drawing.GetPosition(i);
            }

            if (points.Length < 2)
            {
                Debug.LogWarning($"Drawing has insufficient points: {drawing.name}");
                continue;
            }

            DrawingSessionData drawingData = new DrawingSessionData
            {
                points = points,
                width = drawing.startWidth,
                drawingId = tracker.drawingId,
                useWorldSpace = drawing.useWorldSpace
            };

            if (drawing.material != null && drawing.material.HasProperty("_Color"))
            {
                drawingData.lineColor = drawing.material.color;
            }
            else
            {
                drawingData.lineColor = Color.white;
            }

            drawingData.colorIndex = 0;

                        if (drawing.transform.parent != null)
                        {
                            string parentInstanceId = GetModelInstanceId(drawing.transform.parent.gameObject);
                            if (!string.IsNullOrEmpty(parentInstanceId))
                            {
                                drawingData.isAttachedToModel = true;
                                drawingData.attachedModelID = parentInstanceId;
                                drawingData.parentPosition = drawing.transform.parent.position;
                                drawingData.parentRotation = drawing.transform.parent.rotation;
                                Debug.Log($"  - Drawing attached to model: {parentInstanceId} (parent: {drawing.transform.parent.name})");
                            }

          


                else
                {
                    drawingData.isAttachedToModel = false;
                }
            }
            else
            {
                drawingData.isAttachedToModel = false;
            }

            session.drawings.Add(drawingData);
            drawingCount++;

            string drawingPointsPreview = points.Length > 0 ?
                $"First: {points[0]}, Last: {points[points.Length - 1]}" :
                "No points";
            Debug.Log($"? Saved drawing #{drawingCount}:\n" +
                     $"  - Points: {points.Length} ({drawingPointsPreview})\n" +
                     $"  - Width: {drawingData.width}\n" +
                     $"  - Color: {drawingData.lineColor}\n" +
                     $"  - Attached: {drawingData.isAttachedToModel}\n" +
                     $"  - Model ID: {drawingData.attachedModelID}\n" +
                     $"  - UseWorldSpace: {drawingData.useWorldSpace}");
        }

        string json = JsonUtility.ToJson(session, true);
        string filePath = Application.persistentDataPath + "/" + saveFileName + ".json";
        File.WriteAllText(filePath, json);
        Debug.Log($"=== SESSION SAVED: {filePath} ===\nModels: {session.models.Count}, Measurements: {session.measurements.Count}, Drawings: {session.drawings.Count}");
    }

    // ===========================================
    // LOAD SESSION WITH MEASUREMENTS AND DRAWINGS
    // ===========================================
    public void LoadSession(string saveFileName)
    {
        string path = Application.persistentDataPath + "/" + saveFileName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        SessionData session = JsonUtility.FromJson<SessionData>(json);

        Debug.Log($"=== LOADING SESSION ===\nModels: {session.models.Count}, Measurements: {session.measurements.Count}, Drawings: {session.drawings.Count}");

        // Destroy existing models
        GameObject[] existingModels = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in existingModels)
        {
            Destroy(model);
        }

        // Destroy existing measurements
        MeasurmentInk[] existingMeasurements = FindObjectsOfType<MeasurmentInk>();
        foreach (var measurement in existingMeasurements)
        {
            Destroy(measurement.gameObject);
        }

        // Destroy existing drawings
        DrawingTracker[] existingDrawings = FindObjectsOfType<DrawingTracker>();
        foreach (var drawing in existingDrawings)
        {
            Destroy(drawing.gameObject);
        }

        serverModelCache.Clear();
        modelInstanceMap.Clear();
    
        StartCoroutine(LoadModelsAndMeasurementsAndDrawingsAfterCleanup(session.models, session.measurements, session.drawings));
    }

        private IEnumerator LoadModelsAndMeasurementsAndDrawingsAfterCleanup(List<ModelData> models, List<MeasurementData> measurements, List<DrawingSessionData> drawings)
              {
                  yield return null;

                  // Load all models first
                  Debug.Log("=== LOADING MODELS ===");
                  foreach (var modelData in models)
                  {
                      if (modelData.isServerModel)
                      {
                          if (modelSample != null && !string.IsNullOrEmpty(modelData.modelId))
                          {
                              modelSample.LoadModel(modelData.modelId, modelData.modelName, modelData.isTransparent, modelData.isChild);
                              yield return StartCoroutine(WaitAndApplyTransformWithFallback(modelData));
                          }
                          else
                          {
                              CreatePlaceholderModel(modelData);
                          }
                      }
                      else
                      {
                          LoadLocalModel(modelData);
                      }

                      yield return new WaitForSeconds(0.5f);
                  }

                  // Wait for all models to be ready
                  yield return new WaitForSeconds(1f);

                  // Now load measurements
                  Debug.Log($"=== LOADING {measurements.Count} MEASUREMENTS ===");
                  foreach (var measurementData in measurements)
                  {
                      LoadMeasurement(measurementData);
                      yield return new WaitForSeconds(0.2f);
                  }

                  // Load drawings
                  Debug.Log($"=== LOADING {drawings.Count} DRAWINGS ===");
                  foreach (var drawingData in drawings)
                  {
                      LoadDrawing(drawingData);
                      yield return new WaitForSeconds(0.1f);
                  }

                  Debug.Log("=== SESSION LOADING COMPLETE ===");
              }












    private void LoadDrawing(DrawingSessionData drawingData)
    {
        // Create a new GameObject for the drawing
        GameObject drawingObj = new GameObject("LoadedDrawing_" + drawingData.drawingId);
        LineRenderer lineRenderer = drawingObj.AddComponent<LineRenderer>();

        if (drawingMaterial != null)
        {
            lineRenderer.material = new Material(drawingMaterial);
        }
        else
        {
            // Fallback if material not assigned
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            Debug.LogWarning("Drawing material not assigned, using default shader");
        }

        // CRITICAL: Find and parent FIRST, before setting any positions
        GameObject parentModel = null;
        if (drawingData.isAttachedToModel && !string.IsNullOrEmpty(drawingData.attachedModelID))
        {
            if (modelInstanceMap.TryGetValue(drawingData.attachedModelID, out parentModel))
            {
                // Parent the drawing BEFORE setting coordinate space
                drawingObj.transform.SetParent(parentModel.transform, false);


  


                drawingObj.transform.localPosition = Vector3.zero;
                drawingObj.transform.localRotation = Quaternion.identity;
                drawingObj.transform.localScale = Vector3.one;

                // Use LOCAL space when parented (matches Pen.cs behavior)
                lineRenderer.useWorldSpace = false;

                Debug.Log($"? Drawing parented to model: {parentModel.name}");
            }
            else
            {
                Debug.LogWarning($"? Parent model not found for drawing (ID: {drawingData.attachedModelID})");
                drawingData.isAttachedToModel = false;
                // Use WORLD space if parent not found
                lineRenderer.useWorldSpace = true;
            }
        }
        else
        {
            // Independent drawing - use world space
            lineRenderer.useWorldSpace = true;
        }

        // Set line width
        lineRenderer.startWidth = drawingData.width;
        lineRenderer.endWidth = drawingData.width;

        // Set color
        if (drawingData.lineColor != Color.clear)
        {
            lineRenderer.startColor = drawingData.lineColor;
            lineRenderer.endColor = drawingData.lineColor;
            if (lineRenderer.material.HasProperty("_Color"))
            {
                lineRenderer.material.color = drawingData.lineColor;
            }
        }

        // Set line positions based on coordinate space
        lineRenderer.positionCount = drawingData.points.Length;

        if (drawingData.isAttachedToModel && parentModel != null)
        {
            // Drawing is attached - points are stored in LOCAL space
            // Since useWorldSpace = false, we can set them directly
            lineRenderer.SetPositions(drawingData.points);

            Debug.Log($"? Set {drawingData.points.Length} LOCAL space points for attached drawing");
            if (drawingData.points.Length > 0)
            {
                Debug.Log($"  First local point: {drawingData.points[0]}, Last: {drawingData.points[drawingData.points.Length - 1]}");
            }
        }
        else if (!drawingData.useWorldSpace && drawingData.isAttachedToModel)
        {
            // Parent is missing but drawing was in local space
            // Convert to world space using stored parent transform
            lineRenderer.useWorldSpace = true;
            Matrix4x4 parentMatrix = Matrix4x4.TRS(drawingData.parentPosition, drawingData.parentRotation, Vector3.one);

            for (int i = 0; i < drawingData.points.Length; i++)
            {
                Vector3 worldPoint = parentMatrix.MultiplyPoint3x4(drawingData.points[i]);
                lineRenderer.SetPosition(i, worldPoint);
            }

            Debug.Log($"? Converted {drawingData.points.Length} points from local to world space (parent missing)");
        }
        else
        {
            // Independent drawing - points are in world space
            lineRenderer.SetPositions(drawingData.points);

            Debug.Log($"? Set {drawingData.points.Length} WORLD space points for independent drawing");
        }

        // Add tracking component
        DrawingTracker tracker = drawingObj.AddComponent<DrawingTracker>();
        tracker.drawingId = drawingData.drawingId;

        // Add Deletable component
        drawingObj.AddComponent<Deletable>();

        // Set layer
        if (drawingLayerMask != 0)
        {
            drawingObj.layer = (int)Mathf.Log(drawingLayerMask.value, 2);
        }

        // Add collider - handle both world and local space
        BoxCollider collider = drawingObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        if (lineRenderer.useWorldSpace)
        {
            // World space - use bounds
            if (lineRenderer.bounds.size.magnitude > 0)
            {
                collider.center = lineRenderer.bounds.center;
                collider.size = lineRenderer.bounds.size;
            }
            else
            {
                collider.size = Vector3.one * 0.1f;
            }
        }
        else
        {
            // Local space - calculate bounds manually
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 point = lineRenderer.GetPosition(i);
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;

            // Ensure minimum size
            size.x = Mathf.Max(size.x, 0.01f);
            size.y = Mathf.Max(size.y, 0.01f);
            size.z = Mathf.Max(size.z, 0.01f);

            collider.center = center;
            collider.size = size;
        }

        Debug.Log($"? Loaded drawing: {drawingData.points.Length} points, attached: {drawingData.isAttachedToModel}, width: {drawingData.width}, useWorldSpace: {lineRenderer.useWorldSpace}");
    }

    private void LoadMeasurement(MeasurementData measurementData)
    {
        GameObject measurementPrefab = Resources.Load<GameObject>("MesurmentInk");
        if (measurementPrefab == null)
        {
            Debug.LogError("MesurmentInk prefab not found in Resources!");
            return;
        }

        GameObject measurementObj = Instantiate(measurementPrefab, Vector3.zero, Quaternion.identity);
        measurementObj.name = "LoadedMeasurement";

        MeasurmentInk inkComponent = measurementObj.GetComponent<MeasurmentInk>();
        LineRenderer lineRenderer = measurementObj.GetComponent<LineRenderer>();

        if (inkComponent == null || lineRenderer == null)
        {
            Debug.LogError("MesurmentInk or LineRenderer component missing!");
            Destroy(measurementObj);
            return;
        }

        var measurementType = typeof(MeasurmentInk);

        var storedPointsField = measurementType.GetField("storedPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isStraightLineField = measurementType.GetField("isStraightLine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var originalMeasurementField = measurementType.GetField("originalMeasurement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasLocalPositionField = measurementType.GetField("canvasLocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point1LocalPositionField = measurementType.GetField("point1LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point2LocalPositionField = measurementType.GetField("point2LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isAttachedToModelField = measurementType.GetField("isAttachedToModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attachedModelIDField = measurementType.GetField("attachedModelID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isInitializedField = measurementType.GetField("isInitialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attachedModelField = measurementType.GetField("attachedModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasField = measurementType.GetField("canvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point1Field = measurementType.GetField("point1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point2Field = measurementType.GetField("point2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Transform canvas = canvasField?.GetValue(inkComponent) as Transform;
        GameObject point1 = point1Field?.GetValue(inkComponent) as GameObject;
        GameObject point2 = point2Field?.GetValue(inkComponent) as GameObject;

               GameObject parentModel = null;
                if (measurementData.isAttachedToModel && !string.IsNullOrEmpty(measurementData.attachedModelID))
                {
                    if (modelInstanceMap.TryGetValue(measurementData.attachedModelID, out parentModel))
                    {
                        // IMPORTANT: Parent BEFORE setting any positions
                        measurementObj.transform.SetParent(parentModel.transform, false);



                measurementObj.transform.localPosition = Vector3.zero;
                measurementObj.transform.localRotation = Quaternion.identity;
                measurementObj.transform.localScale = Vector3.one;

                attachedModelField?.SetValue(inkComponent, parentModel.transform);

                Debug.Log($"? Measurement parented to model: {parentModel.name}");
            }
            else
            {
                Debug.LogWarning($"? Parent model not found (ID: {measurementData.attachedModelID})");
                measurementData.isAttachedToModel = false;
            }
        }

        // CRITICAL: For attached measurements, storedPoints must be in LOCAL space
        // The measurement script expects local coordinates when attached to a model
        Vector3[] pointsToStore;
        if (measurementData.isAttachedToModel && parentModel != null)
        {
            // Points are already in local space from save, use them directly
            pointsToStore = measurementData.storedPoints;
        }
        else
        {
            // Independent measurement - points are in world space
            pointsToStore = measurementData.storedPoints;
        }

        // Set component fields
        storedPointsField?.SetValue(inkComponent, pointsToStore);
        isStraightLineField?.SetValue(inkComponent, measurementData.isStraightLine);
        originalMeasurementField?.SetValue(inkComponent, measurementData.originalMeasurement);
        canvasLocalPositionField?.SetValue(inkComponent, measurementData.canvasLocalPosition);
        point1LocalPositionField?.SetValue(inkComponent, measurementData.point1LocalPosition);
        point2LocalPositionField?.SetValue(inkComponent, measurementData.point2LocalPosition);
        isAttachedToModelField?.SetValue(inkComponent, measurementData.isAttachedToModel);
        attachedModelIDField?.SetValue(inkComponent, measurementData.attachedModelID);
        isInitializedField?.SetValue(inkComponent, true);

        // Set line properties
        lineRenderer.startWidth = lineRenderer.endWidth = measurementData.lineWidth;
        lineRenderer.useWorldSpace = true;

        if (measurementData.lineColor != Color.clear && lineRenderer.material != null)
        {
            lineRenderer.material.color = measurementData.lineColor;
        }

        // KEY FIX: Set line positions correctly based on attachment
        if (measurementData.isAttachedToModel && parentModel != null)
        {
            // Points are stored in LOCAL space relative to the parent model
            // We need to transform them to WORLD space for the LineRenderer (which uses world space)

            if (measurementData.isStraightLine && measurementData.storedPoints.Length >= 2)
            {
                lineRenderer.positionCount = 2;

                // Transform local points to world space
                Vector3 worldPoint1 = parentModel.transform.TransformPoint(measurementData.storedPoints[0]);
                Vector3 worldPoint2 = parentModel.transform.TransformPoint(measurementData.storedPoints[measurementData.storedPoints.Length - 1]);

                lineRenderer.SetPosition(0, worldPoint1);
                lineRenderer.SetPosition(1, worldPoint2);

                Debug.Log($"  Line points (world): Start={worldPoint1}, End={worldPoint2}");
            }
            else
            {
                // Curved line - transform all points
                lineRenderer.positionCount = measurementData.storedPoints.Length;
                Vector3[] worldPoints = new Vector3[measurementData.storedPoints.Length];

                for (int i = 0; i < measurementData.storedPoints.Length; i++)
                {
                    worldPoints[i] = parentModel.transform.TransformPoint(measurementData.storedPoints[i]);
                }

                lineRenderer.SetPositions(worldPoints);

                if (worldPoints.Length > 0)
                {
                    Debug.Log($"  Curved line points (world): Start={worldPoints[0]}, End={worldPoints[worldPoints.Length - 1]}");
                }
            }

            // Position the canvas in world space
            if (canvas != null && measurementData.canvasLocalPosition != Vector3.zero)
            {
                Vector3 worldCanvasPos = parentModel.transform.TransformPoint(measurementData.canvasLocalPosition);
                canvas.position = worldCanvasPos;
                Debug.Log($"  Canvas world pos: {worldCanvasPos}");
            }

            // Position point1 in world space
            if (point1 != null && measurementData.point1LocalPosition != Vector3.zero)
            {
                Vector3 worldPoint1Pos = parentModel.transform.TransformPoint(measurementData.point1LocalPosition);
                point1.transform.position = worldPoint1Pos;
                Debug.Log($"  Point1 world pos: {worldPoint1Pos}");
            }

            // Position point2 in world space
            if (point2 != null && measurementData.point2LocalPosition != Vector3.zero)
            {
                Vector3 worldPoint2Pos = parentModel.transform.TransformPoint(measurementData.point2LocalPosition);
                point2.transform.position = worldPoint2Pos;
                Debug.Log($"  Point2 world pos: {worldPoint2Pos}");
            }
        }
        else
        {
            // Independent measurement - use saved world positions directly
            measurementObj.transform.position = measurementData.position;
            measurementObj.transform.rotation = measurementData.rotation;
            measurementObj.transform.localScale = measurementData.scale;

            if (measurementData.isStraightLine && measurementData.storedPoints.Length >= 2)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, measurementData.storedPoints[0]);
                lineRenderer.SetPosition(1, measurementData.storedPoints[measurementData.storedPoints.Length - 1]);
            }
            else
            {
                lineRenderer.positionCount = measurementData.storedPoints.Length;
                lineRenderer.SetPositions(measurementData.storedPoints);
            }

            if (canvas != null && measurementData.canvasLocalPosition != Vector3.zero)
            {
                canvas.position = measurementData.canvasLocalPosition;
            }

            if (point1 != null && measurementData.point1LocalPosition != Vector3.zero)
            {
                point1.transform.position = measurementData.point1LocalPosition;
            }

            if (point2 != null && measurementData.point2LocalPosition != Vector3.zero)
            {
                point2.transform.position = measurementData.point2LocalPosition;
            }

            Debug.Log("? Created independent measurement");
        }

        // Update the measurement text
        var lengthTextField = measurementType.GetField("lengthText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lengthText = lengthTextField?.GetValue(inkComponent) as TMPro.TMP_Text;
        if (lengthText != null)
        {
            lengthText.text = $"{measurementData.originalMeasurement:0.00} mm";
        }

        // Orient canvas to camera
        if (canvas != null)
        {
            var orientMethod = measurementType.GetMethod("OrientCanvasToHead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            orientMethod?.Invoke(inkComponent, null);
        }

        Debug.Log($"? Loaded measurement: {measurementData.storedPoints.Length} points, mode: {(measurementData.isStraightLine ? "Straight" : "Curved")}, value: {measurementData.originalMeasurement:F2}mm, attached: {measurementData.isAttachedToModel}");
    }


    
        private IEnumerator WaitAndApplyTransformWithFallback(ModelData modelData)
         {
             float timeout = 10f;
             float elapsed = 0f;
             GameObject foundModel = null;

             while (elapsed < timeout && foundModel == null)
             {
                 yield return new WaitForSeconds(0.5f);
                 elapsed += 0.5f;

                 GameObject[] currentModels = GameObject.FindGameObjectsWithTag(modelTag);
                 foreach (var model in currentModels)
                 {
                     // ? Match by modelId, not by name
                     string existingInstanceId = GetModelInstanceId(model);
                     if (string.IsNullOrEmpty(existingInstanceId))
                         continue;

                     // Check if this is a newly loaded model without cache yet
                     MeshFilter mf = model.GetComponent<MeshFilter>();
                     if (mf != null && mf.mesh != null)
                     {
                         // This might be our model - check by comparing it to expected modelId
                         bool isMatch = false;

                         // If modelData has modelId, try to match
                         if (!string.IsNullOrEmpty(modelData.modelId))
                         {
                             // Check against model's actual ID from server
                             // This requires checking the ModelSample component or similar
                             isMatch = true; // Assume match for now, will be verified
                         }

                         if (isMatch)
                         {
                             foundModel = model;
                             break;
                         }
                     }
                 }
             }

             if (foundModel != null)
             {
                            foundModel.transform.position = modelData.position;
                             foundModel.transform.rotation = modelData.rotation;
                             foundModel.transform.localScale = modelData.scale;

                             SetModelInstanceId(foundModel, modelData.modelInstanceId);

                             Debug.Log($"? Model loaded: {foundModel.name} (ID: {modelData.modelInstanceId})");





                CacheServerModel(foundModel, modelData.modelId, modelData.modelName,
                                modelData.isTransparent, modelData.isChild,
                                modelData.modelUrl, modelData.textureUrl);
             }
             else
             {
                 Debug.LogWarning($"? Download failed for {modelData.modelName}, trying fallback...");

                 if (!string.IsNullOrEmpty(modelData.meshDataPath) && !string.IsNullOrEmpty(modelData.materialDataPath))
                 {
                     yield return StartCoroutine(LoadModelFromSavedFiles(modelData));
                 }
                 else
                 {
                     CreatePlaceholderModel(modelData);
                 }
             }
         }
    








    private IEnumerator LoadModelFromSavedFiles(ModelData modelData)
    {
        string modelKey = modelData.modelPath;
        GameObject basePrefab = Resources.Load<GameObject>(modelKey);

        if (basePrefab == null)
        {
            CreatePlaceholderModel(modelData);
            yield break;
        }

        GameObject model = Instantiate(basePrefab);
        model.name = basePrefab.name;
        model.tag = modelTag;

        SetModelInstanceId(model, modelData.modelInstanceId);

        string meshPath = Path.Combine(Application.persistentDataPath, modelData.meshDataPath);
        if (File.Exists(meshPath))
        {
            string meshJson = File.ReadAllText(meshPath);
            SerializableMesh serializableMesh = JsonUtility.FromJson<SerializableMesh>(meshJson);

            Mesh mesh = new Mesh
            {
                vertices = serializableMesh.vertices,
                triangles = serializableMesh.triangles,
                normals = serializableMesh.normals,
                uv = serializableMesh.uv,
                name = serializableMesh.name
            };

            MeshFilter meshFilter = model.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = mesh;
            }
        }

        string materialPath = Path.Combine(Application.persistentDataPath, modelData.materialDataPath);
        if (File.Exists(materialPath))
        {
            yield return StartCoroutine(LoadAndApplyMaterial(model, materialPath, modelData));
        }

        ApplyModelTransform(model, modelData);

        CacheServerModel(model, modelData.modelId, modelData.modelName,
                        modelData.isTransparent, modelData.isChild,
                        modelData.modelUrl, modelData.textureUrl);
    }

    private IEnumerator LoadAndApplyMaterial(GameObject model, string materialPath, ModelData modelData)
    {
        string materialJson = File.ReadAllText(materialPath);
        SerializableMaterial serializableMaterial = JsonUtility.FromJson<SerializableMaterial>(materialJson);

        Material baseMaterial = serializableMaterial.isTransparent
            ? Resources.Load<Material>("Cross-Section-Material_Transparent")
            : Resources.Load<Material>("Cross-Section-Material");

        if (baseMaterial == null)
        {
            yield break;
        }

        Material material = new Material(baseMaterial);
        material.name = serializableMaterial.name;

        if (material.HasProperty("_Color"))
        {
            material.color = serializableMaterial.color;
        }
        else if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", serializableMaterial.color);
        }

        if (serializableMaterial.textureProperties != null)
        {
            foreach (var kvp in serializableMaterial.textureProperties)
            {
                string propertyName = kvp.Key;
                string textureFileName = kvp.Value;

                if (!string.IsNullOrEmpty(textureFileName))
                {
                    string texturePath = Path.Combine(Application.persistentDataPath, textureFileName);
                    if (File.Exists(texturePath))
                    {
                        byte[] textureData = File.ReadAllBytes(texturePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(textureData))
                        {
                            if (material.HasProperty(propertyName))
                            {
                                material.SetTexture(propertyName, texture);
                            }
                        }
                    }
                }
            }
        }

        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        yield return null;
    }

    private void LoadLocalModel(ModelData modelData)
    {
        GameObject prefab = Resources.Load<GameObject>(modelData.modelPath);
        GameObject model;

        if (prefab != null)
        {
            model = Instantiate(prefab);
            model.name = prefab.name;
        }
        else
        {
            model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            model.name = modelData.modelPath.Replace("Models/", "");
        }

        SetModelInstanceId(model, modelData.modelInstanceId);
        ApplyModelTransform(model, modelData);
        Debug.Log($"? Loaded local model: {model.name}");
    }

    private void ApplyModelTransform(GameObject model, ModelData modelData)
    {
        model.transform.position = modelData.position;
        model.transform.rotation = modelData.rotation;
        model.transform.localScale = modelData.scale;
        model.tag = modelData.tag;
    }

    private void CreatePlaceholderModel(ModelData modelData)
    {
        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.name = modelData.modelName + "_Placeholder";

        SetModelInstanceId(model, modelData.modelInstanceId);
        ApplyModelTransform(model, modelData);

        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        Debug.LogWarning($"? Created placeholder for: {modelData.modelName}");
    }

    public void OnServerModelLoaded(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild)
    {
        if (model != null)
        {
            string modelUrl = "";
            string textureUrl = "";

            if (modelSample != null)
            {
                modelUrl = modelSample.CurrentModelUrl;
                textureUrl = modelSample.CurrentTextureUrl;
            }

            string actualModelId = modelId;
            if (!string.IsNullOrEmpty(modelUrl) && modelUrl.Contains("model_id="))
            {
                int startIndex = modelUrl.IndexOf("model_id=") + "model_id=".Length;
                int endIndex = modelUrl.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = modelUrl.Length;

                string extractedId = modelUrl.Substring(startIndex, endIndex - startIndex);
                if (!string.IsNullOrEmpty(extractedId))
                {
                    actualModelId = extractedId;
                }
            }

            CacheServerModel(model, actualModelId, modelName, isTransparent, isChild, modelUrl, textureUrl);
        }
    }

    // ===========================================
    // UTILITY METHODS
    // ===========================================

    public void ClearCache()
    {
        serverModelCache.Clear();
        modelInstanceMap.Clear();
     
        Debug.Log("Cache cleared");
    }

    public void ClearSavedData()
    {
        string[] meshFiles = Directory.GetFiles(Application.persistentDataPath, "mesh_*.json");
        string[] materialFiles = Directory.GetFiles(Application.persistentDataPath, "material_*.json");
        string[] textureFiles = Directory.GetFiles(Application.persistentDataPath, "texture_*.png");

        foreach (string file in meshFiles.Concat(materialFiles).Concat(textureFiles))
        {
            File.Delete(file);
        }

        Debug.Log("Cleared all saved data");
    }

    public bool HasCachedModel(string modelKey)
    {
        return serverModelCache.ContainsKey(modelKey);
    }

    public void SaveCurrentSession()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        SaveSession("Session_" + timestamp);
    }

    public string[] GetAvailableSessions()
    {
        string[] sessionFiles = Directory.GetFiles(Application.persistentDataPath, "Session_*.json");
        List<string> sessionNames = new List<string>();

        foreach (string file in sessionFiles)
        {
            sessionNames.Add(Path.GetFileNameWithoutExtension(file));
        }

        return sessionNames.ToArray();
    }


}

// Helper component to store unique instance IDs on models
public class ModelInstanceId : MonoBehaviour
{
    public string instanceId;
}


*/


////////////////////////////////////////////////////////////////////////////////for servermodel


/*using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ModelData
{
    public string modelPath;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool isServerModel;
    public string modelId;
    public string modelName;
    public bool isTransparent;
    public bool isChild;
    public string modelInstanceId;
    public string modelUrl;
    public string textureUrl;
}

[Serializable]
public class MeasurementData
{
    public Vector3[] storedPoints;
    public bool isStraightLine;
    public float originalMeasurement;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector3 canvasLocalPosition;
    public Vector3 point1LocalPosition;
    public Vector3 point2LocalPosition;
    public bool isAttachedToModel;
    public string attachedModelID;
    public float lineWidth;
    public Color lineColor;
}

[Serializable]
public class DrawingSessionData
{
    public Vector3[] points;
    public int colorIndex;
    public float width;
    public Color lineColor;
    public string drawingId;
    public bool useWorldSpace;
    public Vector3 parentPosition;
    public Quaternion parentRotation;
    public bool isAttachedToModel;
    public string attachedModelID;
}

[Serializable]
public class SessionData
{
    public List<ModelData> models = new List<ModelData>();
    public List<MeasurementData> measurements = new List<MeasurementData>();
    public List<DrawingSessionData> drawings = new List<DrawingSessionData>();
}

public class SessionManager : MonoBehaviour
{
    private const string modelTag = "ModelPart";

    [Header("References")]
    public ModelSample modelSample;
    public Material drawingMaterial;
    [SerializeField] private LayerMask drawingLayerMask;
    [SerializeField] private ServerSessionHandler serverSessionHandler;

    private Dictionary<string, ServerModelCache> serverModelCache = new Dictionary<string, ServerModelCache>();
    private Dictionary<string, GameObject> modelInstanceMap = new Dictionary<string, GameObject>();

    [Serializable]
    private class ServerModelCache
    {
        public Mesh mesh;
        public Material material;
        public string modelId;
        public string modelName;
        public bool isTransparent;
        public bool isChild;
        public string modelUrl;
        public string textureUrl;
        public GameObject prefab;
        public Dictionary<string, Texture2D> cachedTextures;
    }

    void Start()
    {
        if (modelSample == null)
        {
            modelSample = FindObjectOfType<ModelSample>();
            if (modelSample == null)
            {
                Debug.LogError("ModelSample not found! Please assign it in the inspector.");
            }
        }

        if (drawingMaterial == null)
        {
            Debug.LogWarning("Drawing material not assigned! Attempting to find it...");
            drawingMaterial = Resources.Load<Material>("DrawingMaterial");
            if (drawingMaterial == null)
            {
                Debug.LogError("Drawing material not found in Resources! Drawings may not load correctly.");
            }
        }

        // Setup server session handler
        if (serverSessionHandler == null)
        {
            GameObject handlerObj = new GameObject("ServerSessionHandler");
            handlerObj.transform.SetParent(transform);
            serverSessionHandler = handlerObj.AddComponent<ServerSessionHandler>();
        }

        // Subscribe to server events
        serverSessionHandler.OnSessionSaved += OnServerSessionSaved;
        serverSessionHandler.OnSessionLoaded += OnServerSessionLoaded;
        serverSessionHandler.OnError += OnServerError;
    }

    private string GenerateModelInstanceId()
    {
        return System.Guid.NewGuid().ToString();
    }

    private void SetModelInstanceId(GameObject model, string instanceId)
    {
        var idComponent = model.GetComponent<ModelInstanceId>();
        if (idComponent == null)
        {
            idComponent = model.AddComponent<ModelInstanceId>();
        }
        idComponent.instanceId = instanceId;
        modelInstanceMap[instanceId] = model;
    }

    private string GetModelInstanceId(GameObject model)
    {
        var idComponent = model.GetComponent<ModelInstanceId>();
        return idComponent != null ? idComponent.instanceId : null;
    }

    public GameObject ImportLocalModel(string prefabName)
    {
        string path = "Models/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Local prefab not found: " + path);
            return null;
        }

        GameObject model = Instantiate(prefab);
        model.name = prefab.name;
        model.tag = modelTag;
        SetModelInstanceId(model, GenerateModelInstanceId());
        return model;
    }

    public void CacheServerModel(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild, string modelUrl, string textureUrl)
    {
        if (model == null)
        {
            Debug.LogError("Cannot cache null model");
            return;
        }

        string existingInstanceId = GetModelInstanceId(model);
        if (string.IsNullOrEmpty(existingInstanceId))
        {
            existingInstanceId = GenerateModelInstanceId();
            SetModelInstanceId(model, existingInstanceId);
        }

        string cacheKey;
        if (!string.IsNullOrEmpty(modelId))
        {
            cacheKey = modelId + "_" + existingInstanceId;
        }
        else
        {
            cacheKey = model.name.Replace("(Clone)", "") + "_" + existingInstanceId;
        }

        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        Renderer renderer = model.GetComponent<Renderer>();

        if (meshFilter == null || renderer == null)
        {
            Debug.LogError($"Model {cacheKey} missing MeshFilter or Renderer components");
            return;
        }

        GameObject basePrefab = Resources.Load<GameObject>(model.name.Replace("(Clone)", ""));
        Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

        if (renderer.material != null)
        {
            CacheAllMaterialTextures(renderer.material, cachedTextures);
        }

        ServerModelCache cache = new ServerModelCache
        {
            mesh = meshFilter.mesh,
            material = renderer.material,
            modelId = modelId,
            modelName = modelName,
            isTransparent = isTransparent,
            isChild = isChild,
            modelUrl = modelUrl,
            textureUrl = textureUrl,
            prefab = basePrefab,
            cachedTextures = cachedTextures
        };

        serverModelCache[cacheKey] = cache;
        Debug.Log($"? Cached with KEY: {cacheKey}, modelId: {modelId}, name: {modelName}, instanceId: {existingInstanceId}");
    }

    private void CacheAllMaterialTextures(Material material, Dictionary<string, Texture2D> textureCache)
    {
        if (material == null || material.shader == null) return;

        string[] commonTextureProperties = {
            "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap", "_Texture", "_Tex", "_Texture2D",
            "_EmissionMap", "_BumpMap", "_NormalMap", "_OcclusionMap", "_MetallicGlossMap"
        };

        foreach (string propName in commonTextureProperties)
        {
            if (material.HasProperty(propName))
            {
                Texture texture = material.GetTexture(propName);
                if (texture != null && texture is Texture2D)
                {
                    textureCache[propName] = texture as Texture2D;
                }
            }
        }
    }

    // ===========================================
    // COLLECT SESSION DATA
    // ===========================================
    private SessionData CollectSessionData()
    {
        SessionData session = new SessionData();
        GameObject[] models = GameObject.FindGameObjectsWithTag(modelTag);

        Debug.Log($"=== COLLECTING SESSION DATA: {models.Length} models ===");

        // Collect models
        foreach (var model in models)
        {
            string instanceId = GetModelInstanceId(model);

            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = GenerateModelInstanceId();
                SetModelInstanceId(model, instanceId);
            }

            string cacheKey = null;
            foreach (var kvp in serverModelCache)
            {
                if (kvp.Key.EndsWith("_" + instanceId))
                {
                    cacheKey = kvp.Key;
                    break;
                }
            }

            bool isServer = !string.IsNullOrEmpty(cacheKey);

            ModelData data = new ModelData
            {
                position = model.transform.position,
                rotation = model.transform.rotation,
                scale = model.transform.localScale,
                tag = model.tag,
                isServerModel = isServer,
                modelPath = cacheKey ?? ("Models/" + model.name.Replace("(Clone)", "")),
                modelInstanceId = instanceId
            };

            if (isServer && serverModelCache.ContainsKey(cacheKey))
            {
                ServerModelCache cache = serverModelCache[cacheKey];
                data.modelId = cache.modelId;
                data.modelName = cache.modelName;
                data.isTransparent = cache.isTransparent;
                data.isChild = cache.isChild;
                data.modelUrl = cache.modelUrl;
                data.textureUrl = cache.textureUrl;
            }
            else
            {
                data.modelId = "";
                data.modelName = model.name.Replace("(Clone)", "");
            }

            session.models.Add(data);
        }

        // Collect measurements
        MeasurmentInk[] allMeasurements = FindObjectsOfType<MeasurmentInk>();
        Debug.Log($"=== COLLECTING {allMeasurements.Length} MEASUREMENTS ===");

        foreach (var measurement in allMeasurements)
        {
            LineRenderer lineRenderer = measurement.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogWarning($"Measurement missing LineRenderer: {measurement.name}");
                continue;
            }

            var measurementType = typeof(MeasurmentInk);

            var storedPointsField = measurementType.GetField("storedPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isStraightLineField = measurementType.GetField("isStraightLine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var originalMeasurementField = measurementType.GetField("originalMeasurement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var canvasLocalPositionField = measurementType.GetField("canvasLocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var point1LocalPositionField = measurementType.GetField("point1LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var point2LocalPositionField = measurementType.GetField("point2LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isAttachedToModelField = measurementType.GetField("isAttachedToModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var attachedModelIDField = measurementType.GetField("attachedModelID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Vector3[] storedPoints = storedPointsField?.GetValue(measurement) as Vector3[];
            if (storedPoints == null || storedPoints.Length < 2)
            {
                Debug.LogWarning($"Measurement has no points: {measurement.name}");
                continue;
            }

            MeasurementData measureData = new MeasurementData
            {
                storedPoints = storedPoints,
                isStraightLine = (bool)(isStraightLineField?.GetValue(measurement) ?? false),
                originalMeasurement = (float)(originalMeasurementField?.GetValue(measurement) ?? 0f),
                canvasLocalPosition = (Vector3)(canvasLocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                point1LocalPosition = (Vector3)(point1LocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                point2LocalPosition = (Vector3)(point2LocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                isAttachedToModel = (bool)(isAttachedToModelField?.GetValue(measurement) ?? false),
                attachedModelID = (string)(attachedModelIDField?.GetValue(measurement) ?? ""),
                position = measurement.transform.position,
                rotation = measurement.transform.rotation,
                scale = measurement.transform.localScale,
                lineWidth = lineRenderer.startWidth
            };

            if (lineRenderer.material != null && lineRenderer.material.HasProperty("_Color"))
            {
                measureData.lineColor = lineRenderer.material.color;
            }

            if (measureData.isAttachedToModel && measurement.transform.parent != null)
            {
                string parentInstanceId = GetModelInstanceId(measurement.transform.parent.gameObject);
                if (!string.IsNullOrEmpty(parentInstanceId))
                {
                    measureData.attachedModelID = parentInstanceId;
                    Debug.Log($"  - Measurement attached to parent with instanceId: {parentInstanceId}");
                }
                else
                {
                    Debug.LogWarning($"  - Has parent but no instanceId found, saving as independent");
                    measureData.isAttachedToModel = false;
                    measureData.attachedModelID = "";
                }
            }
            else if (measureData.isAttachedToModel && string.IsNullOrEmpty(measureData.attachedModelID))
            {
                Debug.LogWarning($"  - Marked as attached but no parent found, saving as independent");
                measureData.isAttachedToModel = false;
            }

            session.measurements.Add(measureData);
        }

        // Collect drawings
        LineRenderer[] allDrawings = FindObjectsOfType<LineRenderer>();
        int drawingCount = 0;

        foreach (var drawing in allDrawings)
        {
            if (drawing.GetComponent<MeasurmentInk>() != null)
                continue;

            DrawingTracker tracker = drawing.GetComponent<DrawingTracker>();
            if (tracker == null)
                continue;

            Vector3[] points = new Vector3[drawing.positionCount];
            for (int i = 0; i < drawing.positionCount; i++)
            {
                points[i] = drawing.GetPosition(i);
            }

            if (points.Length < 2)
            {
                Debug.LogWarning($"Drawing has insufficient points: {drawing.name}");
                continue;
            }

            DrawingSessionData drawingData = new DrawingSessionData
            {
                points = points,
                width = drawing.startWidth,
                drawingId = tracker.drawingId,
                useWorldSpace = drawing.useWorldSpace
            };

            if (drawing.material != null && drawing.material.HasProperty("_Color"))
            {
                drawingData.lineColor = drawing.material.color;
            }
            else
            {
                drawingData.lineColor = Color.white;
            }

            drawingData.colorIndex = 0;

            if (drawing.transform.parent != null)
            {
                string parentInstanceId = GetModelInstanceId(drawing.transform.parent.gameObject);
                if (!string.IsNullOrEmpty(parentInstanceId))
                {
                    drawingData.isAttachedToModel = true;
                    drawingData.attachedModelID = parentInstanceId;
                    drawingData.parentPosition = drawing.transform.parent.position;
                    drawingData.parentRotation = drawing.transform.parent.rotation;
                }
                else
                {
                    drawingData.isAttachedToModel = false;
                }
            }
            else
            {
                drawingData.isAttachedToModel = false;
            }

            session.drawings.Add(drawingData);
            drawingCount++;
        }

        Debug.Log($"=== SESSION DATA COLLECTED ===\nModels: {session.models.Count}, Measurements: {session.measurements.Count}, Drawings: {session.drawings.Count}");
        return session;
    }

    // ===========================================
    // SAVE SESSION TO SERVER (ONLY METHOD)
    // ===========================================
    public void SaveSession(string sessionName, string userId = "")
    {
        SessionData session = CollectSessionData();

        if (serverSessionHandler != null)
        {
            Debug.Log($"?? Saving session '{sessionName}' to server...");
            serverSessionHandler.SaveSessionToServer(sessionName, session, userId);
        }
        else
        {
            Debug.LogError("? ServerSessionHandler not initialized!");
        }
    }

    // ===========================================
    // LOAD SESSION FROM SERVER (ONLY METHOD)
    // ===========================================
    public void LoadSession(int sessionId)
    {
        if (serverSessionHandler != null)
        {
            Debug.Log($"?? Loading session ID {sessionId} from server...");
            serverSessionHandler.LoadSessionFromServer(sessionId);
        }
        else
        {
            Debug.LogError("? ServerSessionHandler not initialized!");
        }
    }

    // ===========================================
    // APPLY LOADED SESSION
    // ===========================================
    private void ApplyLoadedSession(SessionData session)
    {
        Debug.Log($"=== APPLYING LOADED SESSION ===\nModels: {session.models.Count}, Measurements: {session.measurements.Count}, Drawings: {session.drawings.Count}");

        // Destroy existing models
        GameObject[] existingModels = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in existingModels)
        {
            Destroy(model);
        }

        // Destroy existing measurements
        MeasurmentInk[] existingMeasurements = FindObjectsOfType<MeasurmentInk>();
        foreach (var measurement in existingMeasurements)
        {
            Destroy(measurement.gameObject);
        }

        // Destroy existing drawings
        DrawingTracker[] existingDrawings = FindObjectsOfType<DrawingTracker>();
        foreach (var drawing in existingDrawings)
        {
            Destroy(drawing.gameObject);
        }

        serverModelCache.Clear();
        modelInstanceMap.Clear();

        StartCoroutine(LoadModelsAndMeasurementsAndDrawingsAfterCleanup(session.models, session.measurements, session.drawings));
    }

    private IEnumerator LoadModelsAndMeasurementsAndDrawingsAfterCleanup(List<ModelData> models, List<MeasurementData> measurements, List<DrawingSessionData> drawings)
    {
        yield return null;

        // Load all models first
        Debug.Log("=== LOADING MODELS ===");
        foreach (var modelData in models)
        {
            if (modelData.isServerModel)
            {
                if (modelSample != null && !string.IsNullOrEmpty(modelData.modelId))
                {
                    modelSample.LoadModel(modelData.modelId, modelData.modelName, modelData.isTransparent, modelData.isChild);
                    yield return StartCoroutine(WaitAndApplyTransform(modelData));
                }
                else
                {
                    CreatePlaceholderModel(modelData);
                }
            }
            else
            {
                LoadLocalModel(modelData);
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Wait for all models to be ready
        yield return new WaitForSeconds(1f);

        // Now load measurements
        Debug.Log($"=== LOADING {measurements.Count} MEASUREMENTS ===");
        foreach (var measurementData in measurements)
        {
            LoadMeasurement(measurementData);
            yield return new WaitForSeconds(0.2f);
        }

        // Load drawings
        Debug.Log($"=== LOADING {drawings.Count} DRAWINGS ===");
        foreach (var drawingData in drawings)
        {
            LoadDrawing(drawingData);
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("? SESSION LOADING COMPLETE ===");
    }

    private void LoadDrawing(DrawingSessionData drawingData)
    {
        GameObject drawingObj = new GameObject("LoadedDrawing_" + drawingData.drawingId);
        LineRenderer lineRenderer = drawingObj.AddComponent<LineRenderer>();

        if (drawingMaterial != null)
        {
            lineRenderer.material = new Material(drawingMaterial);
        }
        else
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            Debug.LogWarning("Drawing material not assigned, using default shader");
        }

        GameObject parentModel = null;
        if (drawingData.isAttachedToModel && !string.IsNullOrEmpty(drawingData.attachedModelID))
        {
            if (modelInstanceMap.TryGetValue(drawingData.attachedModelID, out parentModel))
            {
                drawingObj.transform.SetParent(parentModel.transform, false);
                drawingObj.transform.localPosition = Vector3.zero;
                drawingObj.transform.localRotation = Quaternion.identity;
                drawingObj.transform.localScale = Vector3.one;
                lineRenderer.useWorldSpace = false;
                Debug.Log($"? Drawing parented to model: {parentModel.name}");
            }
            else
            {
                Debug.LogWarning($"? Parent model not found for drawing (ID: {drawingData.attachedModelID})");
                drawingData.isAttachedToModel = false;
                lineRenderer.useWorldSpace = true;
            }
        }
        else
        {
            lineRenderer.useWorldSpace = true;
        }

        lineRenderer.startWidth = drawingData.width;
        lineRenderer.endWidth = drawingData.width;

        if (drawingData.lineColor != Color.clear)
        {
            lineRenderer.startColor = drawingData.lineColor;
            lineRenderer.endColor = drawingData.lineColor;
            if (lineRenderer.material.HasProperty("_Color"))
            {
                lineRenderer.material.color = drawingData.lineColor;
            }
        }

        lineRenderer.positionCount = drawingData.points.Length;

        if (drawingData.isAttachedToModel && parentModel != null)
        {
            lineRenderer.SetPositions(drawingData.points);
        }
        else if (!drawingData.useWorldSpace && drawingData.isAttachedToModel)
        {
            lineRenderer.useWorldSpace = true;
            Matrix4x4 parentMatrix = Matrix4x4.TRS(drawingData.parentPosition, drawingData.parentRotation, Vector3.one);

            for (int i = 0; i < drawingData.points.Length; i++)
            {
                Vector3 worldPoint = parentMatrix.MultiplyPoint3x4(drawingData.points[i]);
                lineRenderer.SetPosition(i, worldPoint);
            }
        }
        else
        {
            lineRenderer.SetPositions(drawingData.points);
        }

        DrawingTracker tracker = drawingObj.AddComponent<DrawingTracker>();
        tracker.drawingId = drawingData.drawingId;
        drawingObj.AddComponent<Deletable>();

        if (drawingLayerMask != 0)
        {
            drawingObj.layer = (int)Mathf.Log(drawingLayerMask.value, 2);
        }

        BoxCollider collider = drawingObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        if (lineRenderer.useWorldSpace)
        {
            if (lineRenderer.bounds.size.magnitude > 0)
            {
                collider.center = lineRenderer.bounds.center;
                collider.size = lineRenderer.bounds.size;
            }
            else
            {
                collider.size = Vector3.one * 0.1f;
            }
        }
        else
        {
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 point = lineRenderer.GetPosition(i);
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;

            size.x = Mathf.Max(size.x, 0.01f);
            size.y = Mathf.Max(size.y, 0.01f);
            size.z = Mathf.Max(size.z, 0.01f);

            collider.center = center;
            collider.size = size;
        }
    }

    private void LoadMeasurement(MeasurementData measurementData)
    {
        GameObject measurementPrefab = Resources.Load<GameObject>("MesurmentInk");
        if (measurementPrefab == null)
        {
            Debug.LogError("MesurmentInk prefab not found in Resources!");
            return;
        }

        GameObject measurementObj = Instantiate(measurementPrefab, Vector3.zero, Quaternion.identity);
        measurementObj.name = "LoadedMeasurement";

        MeasurmentInk inkComponent = measurementObj.GetComponent<MeasurmentInk>();
        LineRenderer lineRenderer = measurementObj.GetComponent<LineRenderer>();

        if (inkComponent == null || lineRenderer == null)
        {
            Debug.LogError("MesurmentInk or LineRenderer component missing!");
            Destroy(measurementObj);
            return;
        }

        var measurementType = typeof(MeasurmentInk);

        var storedPointsField = measurementType.GetField("storedPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isStraightLineField = measurementType.GetField("isStraightLine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var originalMeasurementField = measurementType.GetField("originalMeasurement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasLocalPositionField = measurementType.GetField("canvasLocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point1LocalPositionField = measurementType.GetField("point1LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point2LocalPositionField = measurementType.GetField("point2LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isAttachedToModelField = measurementType.GetField("isAttachedToModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attachedModelIDField = measurementType.GetField("attachedModelID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isInitializedField = measurementType.GetField("isInitialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attachedModelField = measurementType.GetField("attachedModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasField = measurementType.GetField("canvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point1Field = measurementType.GetField("point1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point2Field = measurementType.GetField("point2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Transform canvas = canvasField?.GetValue(inkComponent) as Transform;
        GameObject point1 = point1Field?.GetValue(inkComponent) as GameObject;
        GameObject point2 = point2Field?.GetValue(inkComponent) as GameObject;

        GameObject parentModel = null;
        if (measurementData.isAttachedToModel && !string.IsNullOrEmpty(measurementData.attachedModelID))
        {
            if (modelInstanceMap.TryGetValue(measurementData.attachedModelID, out parentModel))
            {
                measurementObj.transform.SetParent(parentModel.transform, false);
                measurementObj.transform.localPosition = Vector3.zero;
                measurementObj.transform.localRotation = Quaternion.identity;
                measurementObj.transform.localScale = Vector3.one;
                attachedModelField?.SetValue(inkComponent, parentModel.transform);
                Debug.Log($"? Measurement parented to model: {parentModel.name}");
            }
            else
            {
                Debug.LogWarning($"? Parent model not found (ID: {measurementData.attachedModelID})");
                measurementData.isAttachedToModel = false;
            }
        }

        Vector3[] pointsToStore = measurementData.storedPoints;

        storedPointsField?.SetValue(inkComponent, pointsToStore);
        isStraightLineField?.SetValue(inkComponent, measurementData.isStraightLine);
        originalMeasurementField?.SetValue(inkComponent, measurementData.originalMeasurement);
        canvasLocalPositionField?.SetValue(inkComponent, measurementData.canvasLocalPosition);
        point1LocalPositionField?.SetValue(inkComponent, measurementData.point1LocalPosition);
        point2LocalPositionField?.SetValue(inkComponent, measurementData.point2LocalPosition);
        isAttachedToModelField?.SetValue(inkComponent, measurementData.isAttachedToModel);
        attachedModelIDField?.SetValue(inkComponent, measurementData.attachedModelID);
        isInitializedField?.SetValue(inkComponent, true);

        lineRenderer.startWidth = lineRenderer.endWidth = measurementData.lineWidth;
        lineRenderer.useWorldSpace = true;

        if (measurementData.lineColor != Color.clear && lineRenderer.material != null)
        {
            lineRenderer.material.color = measurementData.lineColor;
        }

        if (measurementData.isAttachedToModel && parentModel != null)
        {
            if (measurementData.isStraightLine && measurementData.storedPoints.Length >= 2)
            {
                lineRenderer.positionCount = 2;
                Vector3 worldPoint1 = parentModel.transform.TransformPoint(measurementData.storedPoints[0]);
                Vector3 worldPoint2 = parentModel.transform.TransformPoint(measurementData.storedPoints[measurementData.storedPoints.Length - 1]);
                lineRenderer.SetPosition(0, worldPoint1);
                lineRenderer.SetPosition(1, worldPoint2);
            }
            else
            {
                lineRenderer.positionCount = measurementData.storedPoints.Length;
                Vector3[] worldPoints = new Vector3[measurementData.storedPoints.Length];

                for (int i = 0; i < measurementData.storedPoints.Length; i++)
                {
                    worldPoints[i] = parentModel.transform.TransformPoint(measurementData.storedPoints[i]);
                }

                lineRenderer.SetPositions(worldPoints);
            }

            if (canvas != null && measurementData.canvasLocalPosition != Vector3.zero)
            {
                Vector3 worldCanvasPos = parentModel.transform.TransformPoint(measurementData.canvasLocalPosition);
                canvas.position = worldCanvasPos;
            }

            if (point1 != null && measurementData.point1LocalPosition != Vector3.zero)
            {
                Vector3 worldPoint1Pos = parentModel.transform.TransformPoint(measurementData.point1LocalPosition);
                point1.transform.position = worldPoint1Pos;
            }

            if (point2 != null && measurementData.point2LocalPosition != Vector3.zero)
            {
                Vector3 worldPoint2Pos = parentModel.transform.TransformPoint(measurementData.point2LocalPosition);
                point2.transform.position = worldPoint2Pos;
            }
        }
        else
        {
            measurementObj.transform.position = measurementData.position;
            measurementObj.transform.rotation = measurementData.rotation;
            measurementObj.transform.localScale = measurementData.scale;

            if (measurementData.isStraightLine && measurementData.storedPoints.Length >= 2)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, measurementData.storedPoints[0]);
                lineRenderer.SetPosition(1, measurementData.storedPoints[measurementData.storedPoints.Length - 1]);
            }
            else
            {
                lineRenderer.positionCount = measurementData.storedPoints.Length;
                lineRenderer.SetPositions(measurementData.storedPoints);
            }

            if (canvas != null && measurementData.canvasLocalPosition != Vector3.zero)
            {
                canvas.position = measurementData.canvasLocalPosition;
            }

            if (point1 != null && measurementData.point1LocalPosition != Vector3.zero)
            {
                point1.transform.position = measurementData.point1LocalPosition;
            }

            if (point2 != null && measurementData.point2LocalPosition != Vector3.zero)
            {
                point2.transform.position = measurementData.point2LocalPosition;
            }
        }

        var lengthTextField = measurementType.GetField("lengthText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lengthText = lengthTextField?.GetValue(inkComponent) as TMPro.TMP_Text;
        if (lengthText != null)
        {
            lengthText.text = $"{measurementData.originalMeasurement:0.00} mm";
        }

        if (canvas != null)
        {
            var orientMethod = measurementType.GetMethod("OrientCanvasToHead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            orientMethod?.Invoke(inkComponent, null);
        }
    }

    private IEnumerator WaitAndApplyTransform(ModelData modelData)
    {
        float timeout = 10f;
        float elapsed = 0f;
        GameObject foundModel = null;

        while (elapsed < timeout && foundModel == null)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;

            GameObject[] currentModels = GameObject.FindGameObjectsWithTag(modelTag);
            foreach (var model in currentModels)
            {
                string existingInstanceId = GetModelInstanceId(model);
                if (string.IsNullOrEmpty(existingInstanceId))
                    continue;

                MeshFilter mf = model.GetComponent<MeshFilter>();
                if (mf != null && mf.mesh != null)
                {
                    bool isMatch = false;

                    if (!string.IsNullOrEmpty(modelData.modelId))
                    {
                        isMatch = true;
                    }

                    if (isMatch)
                    {
                        foundModel = model;
                        break;
                    }
                }
            }
        }

        if (foundModel != null)
        {
            foundModel.transform.position = modelData.position;
            foundModel.transform.rotation = modelData.rotation;
            foundModel.transform.localScale = modelData.scale;

            SetModelInstanceId(foundModel, modelData.modelInstanceId);

            Debug.Log($"? Model loaded: {foundModel.name} (ID: {modelData.modelInstanceId})");

            CacheServerModel(foundModel, modelData.modelId, modelData.modelName,
                            modelData.isTransparent, modelData.isChild,
                            modelData.modelUrl, modelData.textureUrl);
        }
        else
        {
            Debug.LogWarning($"? Download failed for {modelData.modelName}, creating placeholder...");
            CreatePlaceholderModel(modelData);
        }
    }

    private void LoadLocalModel(ModelData modelData)
    {
        GameObject prefab = Resources.Load<GameObject>(modelData.modelPath);
        GameObject model;

        if (prefab != null)
        {
            model = Instantiate(prefab);
            model.name = prefab.name;
        }
        else
        {
            model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            model.name = modelData.modelPath.Replace("Models/", "");
        }

        SetModelInstanceId(model, modelData.modelInstanceId);
        ApplyModelTransform(model, modelData);
        Debug.Log($"? Loaded local model: {model.name}");
    }

    private void ApplyModelTransform(GameObject model, ModelData modelData)
    {
        model.transform.position = modelData.position;
        model.transform.rotation = modelData.rotation;
        model.transform.localScale = modelData.scale;
        model.tag = modelData.tag;
    }

    private void CreatePlaceholderModel(ModelData modelData)
    {
        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.name = modelData.modelName + "_Placeholder";

        SetModelInstanceId(model, modelData.modelInstanceId);
        ApplyModelTransform(model, modelData);

        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        Debug.LogWarning($"? Created placeholder for: {modelData.modelName}");
    }

    public void OnServerModelLoaded(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild)
    {
        if (model != null)
        {
            string modelUrl = "";
            string textureUrl = "";

            if (modelSample != null)
            {
                modelUrl = modelSample.CurrentModelUrl;
                textureUrl = modelSample.CurrentTextureUrl;
            }

            string actualModelId = modelId;
            if (!string.IsNullOrEmpty(modelUrl) && modelUrl.Contains("model_id="))
            {
                int startIndex = modelUrl.IndexOf("model_id=") + "model_id=".Length;
                int endIndex = modelUrl.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = modelUrl.Length;

                string extractedId = modelUrl.Substring(startIndex, endIndex - startIndex);
                if (!string.IsNullOrEmpty(extractedId))
                {
                    actualModelId = extractedId;
                }
            }

            CacheServerModel(model, actualModelId, modelName, isTransparent, isChild, modelUrl, textureUrl);
        }
    }

    // ===========================================
    // SERVER EVENT CALLBACKS
    // ===========================================
    private void OnServerSessionSaved(int sessionId)
    {
        Debug.Log($"? Session saved to server with ID: {sessionId}");
        // You can add UI notification here
    }

    private void OnServerSessionLoaded(string sessionDataJson)
    {
        Debug.Log($"? Session data loaded from server, applying...");

        try
        {
            SessionData session = JsonUtility.FromJson<SessionData>(sessionDataJson);
            ApplyLoadedSession(session);
        }
        catch (Exception e)
        {
            Debug.LogError($"? Failed to parse loaded session: {e.Message}");
        }
    }

    private void OnServerError(string errorMessage)
    {
        Debug.LogError($"? Server error: {errorMessage}");
        // You can add UI error notification here
    }

    // ===========================================
    // UTILITY METHODS
    // ===========================================
    public void ClearCache()
    {
        serverModelCache.Clear();
        modelInstanceMap.Clear();
        Debug.Log("Cache cleared");
    }

    public bool HasCachedModel(string modelKey)
    {
        return serverModelCache.ContainsKey(modelKey);
    }

    public void SaveCurrentSession()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        SaveSession("Session_" + timestamp);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (serverSessionHandler != null)
        {
            serverSessionHandler.OnSessionSaved -= OnServerSessionSaved;
            serverSessionHandler.OnSessionLoaded -= OnServerSessionLoaded;
            serverSessionHandler.OnError -= OnServerError;
        }
    }
}

// Helper component to store unique instance IDs on models
public class ModelInstanceId : MonoBehaviour
{
    public string instanceId;
}*/




using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[Serializable]
public class ModelData
{
    public string modelPath;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string tag;
    public bool isServerModel;
    public string modelId;
    public string modelName;
    public bool isTransparent;
    public bool isChild;
    public string modelInstanceId;
    public string modelUrl;
    public string textureUrl;
}

[Serializable]
public class MeasurementData
{
    public Vector3[] storedPoints;
    public bool isStraightLine;
    public float originalMeasurement;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector3 canvasLocalPosition;
    public Vector3 point1LocalPosition;
    public Vector3 point2LocalPosition;
    public bool isAttachedToModel;
    public string attachedModelID;
    public float lineWidth;
    public Color lineColor;
}

[Serializable]
public class DrawingSessionData
{
    public Vector3[] points;
    public int colorIndex;
    public float width;
    public Color lineColor;
    public string drawingId;
    public bool useWorldSpace;
    public Vector3 parentPosition;
    public Quaternion parentRotation;
    public bool isAttachedToModel;
    public string attachedModelID;
}

[Serializable]
public class SessionData
{
    public List<ModelData> models = new List<ModelData>();
    public List<MeasurementData> measurements = new List<MeasurementData>();
    public List<DrawingSessionData> drawings = new List<DrawingSessionData>();
}

public class SessionManager : MonoBehaviourPunCallbacks
{
    private const string modelTag = "ModelPart";
    private const byte LoadSessionEventCode = 1;
    private const byte SessionDataEventCode = 2;

    [Header("References")]
    public ModelSample modelSample;
    public Material drawingMaterial;
    [SerializeField] private LayerMask drawingLayerMask;
    [SerializeField] private ServerSessionHandler serverSessionHandler;

    private Dictionary<string, ServerModelCache> serverModelCache = new Dictionary<string, ServerModelCache>();
    private Dictionary<string, GameObject> modelInstanceMap = new Dictionary<string, GameObject>();

    // Track loading state for multiplayer
    private bool isLoadingSession = false;
    private Queue<string> pendingSessionData = new Queue<string>();

    [Serializable]
    private class ServerModelCache
    {
        public Mesh mesh;
        public Material material;
        public string modelId;
        public string modelName;
        public bool isTransparent;
        public bool isChild;
        public string modelUrl;
        public string textureUrl;
        public GameObject prefab;
        public Dictionary<string, Texture2D> cachedTextures;
    }

    void Start()
    {
        if (modelSample == null)
        {
            modelSample = FindObjectOfType<ModelSample>();
            if (modelSample == null)
            {
                Debug.LogError("ModelSample not found! Please assign it in the inspector.");
            }
        }

        if (drawingMaterial == null)
        {
            Debug.LogWarning("Drawing material not assigned! Attempting to find it...");
            drawingMaterial = Resources.Load<Material>("DrawingMaterial");
            if (drawingMaterial == null)
            {
                Debug.LogError("Drawing material not found in Resources! Drawings may not load correctly.");
            }
        }

        // Setup server session handler
        if (serverSessionHandler == null)
        {
            GameObject handlerObj = new GameObject("ServerSessionHandler");
            handlerObj.transform.SetParent(transform);
            serverSessionHandler = handlerObj.AddComponent<ServerSessionHandler>();
        }

        // Subscribe to server events
        serverSessionHandler.OnSessionSaved += OnServerSessionSaved;
        serverSessionHandler.OnSessionLoaded += OnServerSessionLoaded;
        serverSessionHandler.OnError += OnServerError;

        // Register Photon event callbacks
        PhotonNetwork.NetworkingClient.EventReceived += OnPhotonEvent;

        Debug.Log($"[PUN2] SessionManager initialized. Connected: {PhotonNetwork.IsConnected}, In Room: {PhotonNetwork.InRoom}");
    }

    // ===========================================
    // PHOTON EVENT HANDLING
    // ===========================================
    private void OnPhotonEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == LoadSessionEventCode)
        {
            // Any client receives request to load a session
            int sessionId = (int)photonEvent.CustomData;
            Debug.Log($"[PUN2] Received load session request for ID: {sessionId}");

            // Only master client loads from server and broadcasts data
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("[PUN2] I am master, loading from server...");
                LoadSessionFromServerAndBroadcast(sessionId);
            }
            else
            {
                Debug.Log("[PUN2] Waiting for master to broadcast session data...");
            }
        }
        else if (eventCode == SessionDataEventCode)
        {
            // All clients receive session data to apply
            string sessionDataJson = (string)photonEvent.CustomData;
            Debug.Log($"[PUN2] Received session data to apply ({sessionDataJson.Length} chars)");

            // Queue the data if currently loading
            if (isLoadingSession)
            {
                Debug.Log("[PUN2] Already loading, queuing this session data...");
                pendingSessionData.Enqueue(sessionDataJson);
            }
            else
            {
                ApplySessionDataFromNetwork(sessionDataJson);
            }
        }
    }

    // ===========================================
    // MULTIPLAYER SESSION METHODS
    // ===========================================

    /// <summary>
    /// Request session load across all clients - Call this from UI or any client
    /// </summary>
    public void LoadSessionMultiplayer(int sessionId)
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("[PUN2] Not connected to Photon! Cannot load multiplayer session.");
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("[PUN2] Not in a room! Cannot load multiplayer session.");
            return;
        }

        Debug.Log($"[PUN2] Broadcasting load session request for ID: {sessionId}");

        // Send event to all clients including self
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(LoadSessionEventCode, sessionId, raiseEventOptions, sendOptions);
    }

    /// <summary>
    /// Master client loads session from server and broadcasts to all clients
    /// </summary>
    private void LoadSessionFromServerAndBroadcast(int sessionId)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[PUN2] Only master client should load and broadcast session data!");
            return;
        }

        Debug.Log($"[PUN2] Master loading session {sessionId} from server...");

        if (serverSessionHandler != null)
        {
            serverSessionHandler.LoadSessionFromServer(sessionId);
        }
        else
        {
            Debug.LogError("[PUN2] ServerSessionHandler not initialized!");
        }
    }

    /// <summary>
    /// Broadcast session data to all clients (Master only)
    /// </summary>
    private void BroadcastSessionData(string sessionDataJson)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[PUN2] Only master client should broadcast session data!");
            return;
        }

        Debug.Log($"[PUN2] Broadcasting session data to all clients ({sessionDataJson.Length} chars)");

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(SessionDataEventCode, sessionDataJson, raiseEventOptions, sendOptions);
    }

    /// <summary>
    /// Apply session data received from network
    /// </summary>
    private void ApplySessionDataFromNetwork(string sessionDataJson)
    {
        Debug.Log($"[PUN2] Applying session data from network...");

        try
        {
            SessionData session = JsonUtility.FromJson<SessionData>(sessionDataJson);
            ApplyLoadedSession(session);
        }
        catch (Exception e)
        {
            Debug.LogError($"[PUN2] Failed to parse session data: {e.Message}");
        }
    }

    // ===========================================
    // INSTANCE ID MANAGEMENT
    // ===========================================
    private string GenerateModelInstanceId()
    {
        return System.Guid.NewGuid().ToString();
    }

    private void SetModelInstanceId(GameObject model, string instanceId)
    {
        var idComponent = model.GetComponent<ModelInstanceId>();
        if (idComponent == null)
        {
            idComponent = model.AddComponent<ModelInstanceId>();
        }
        idComponent.instanceId = instanceId;
        modelInstanceMap[instanceId] = model;
    }

    private string GetModelInstanceId(GameObject model)
    {
        var idComponent = model.GetComponent<ModelInstanceId>();
        return idComponent != null ? idComponent.instanceId : null;
    }

    // ===========================================
    // MODEL IMPORT AND CACHING
    // ===========================================
    public GameObject ImportLocalModel(string prefabName)
    {
        string path = "Models/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Local prefab not found: " + path);
            return null;
        }

        GameObject model = Instantiate(prefab);
        model.name = prefab.name;
        model.tag = modelTag;
        SetModelInstanceId(model, GenerateModelInstanceId());
        return model;
    }

    public void CacheServerModel(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild, string modelUrl, string textureUrl)
    {
        if (model == null)
        {
            Debug.LogError("Cannot cache null model");
            return;
        }

        string existingInstanceId = GetModelInstanceId(model);
        if (string.IsNullOrEmpty(existingInstanceId))
        {
            existingInstanceId = GenerateModelInstanceId();
            SetModelInstanceId(model, existingInstanceId);
        }

        string cacheKey;
        if (!string.IsNullOrEmpty(modelId))
        {
            cacheKey = modelId + "_" + existingInstanceId;
        }
        else
        {
            cacheKey = model.name.Replace("(Clone)", "") + "_" + existingInstanceId;
        }

        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        Renderer renderer = model.GetComponent<Renderer>();

        if (meshFilter == null || renderer == null)
        {
            Debug.LogError($"Model {cacheKey} missing MeshFilter or Renderer components");
            return;
        }

        GameObject basePrefab = Resources.Load<GameObject>(model.name.Replace("(Clone)", ""));
        Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

        if (renderer.material != null)
        {
            CacheAllMaterialTextures(renderer.material, cachedTextures);
        }

        ServerModelCache cache = new ServerModelCache
        {
            mesh = meshFilter.mesh,
            material = renderer.material,
            modelId = modelId,
            modelName = modelName,
            isTransparent = isTransparent,
            isChild = isChild,
            modelUrl = modelUrl,
            textureUrl = textureUrl,
            prefab = basePrefab,
            cachedTextures = cachedTextures
        };

        serverModelCache[cacheKey] = cache;
        Debug.Log($"?? Cached with KEY: {cacheKey}, modelId: {modelId}, name: {modelName}, instanceId: {existingInstanceId}");
    }

    private void CacheAllMaterialTextures(Material material, Dictionary<string, Texture2D> textureCache)
    {
        if (material == null || material.shader == null) return;

        string[] commonTextureProperties = {
            "_MainTex", "_BaseMap", "_AlbedoMap", "_DiffuseMap", "_Texture", "_Tex", "_Texture2D",
            "_EmissionMap", "_BumpMap", "_NormalMap", "_OcclusionMap", "_MetallicGlossMap"
        };

        foreach (string propName in commonTextureProperties)
        {
            if (material.HasProperty(propName))
            {
                Texture texture = material.GetTexture(propName);
                if (texture != null && texture is Texture2D)
                {
                    textureCache[propName] = texture as Texture2D;
                }
            }
        }
    }

    // ===========================================
    // COLLECT SESSION DATA
    // ===========================================
    private SessionData CollectSessionData()
    {
        SessionData session = new SessionData();
        GameObject[] models = GameObject.FindGameObjectsWithTag(modelTag);

        Debug.Log($"=== COLLECTING SESSION DATA: {models.Length} models ===");

        // Collect models
        foreach (var model in models)
        {
            string instanceId = GetModelInstanceId(model);

            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = GenerateModelInstanceId();
                SetModelInstanceId(model, instanceId);
            }

            string cacheKey = null;
            foreach (var kvp in serverModelCache)
            {
                if (kvp.Key.EndsWith("_" + instanceId))
                {
                    cacheKey = kvp.Key;
                    break;
                }
            }

            bool isServer = !string.IsNullOrEmpty(cacheKey);

            ModelData data = new ModelData
            {
                position = model.transform.position,
                rotation = model.transform.rotation,
                scale = model.transform.localScale,
                tag = model.tag,
                isServerModel = isServer,
                modelPath = cacheKey ?? ("Models/" + model.name.Replace("(Clone)", "")),
                modelInstanceId = instanceId
            };

            if (isServer && serverModelCache.ContainsKey(cacheKey))
            {
                ServerModelCache cache = serverModelCache[cacheKey];
                data.modelId = cache.modelId;
                data.modelName = cache.modelName;
                data.isTransparent = cache.isTransparent;
                data.isChild = cache.isChild;
                data.modelUrl = cache.modelUrl;
                data.textureUrl = cache.textureUrl;
            }
            else
            {
                data.modelId = "";
                data.modelName = model.name.Replace("(Clone)", "");
            }

            session.models.Add(data);
        }

        // Collect measurements
        MeasurmentInk[] allMeasurements = FindObjectsOfType<MeasurmentInk>();
        Debug.Log($"=== COLLECTING {allMeasurements.Length} MEASUREMENTS ===");

        foreach (var measurement in allMeasurements)
        {
            LineRenderer lineRenderer = measurement.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogWarning($"Measurement missing LineRenderer: {measurement.name}");
                continue;
            }

            var measurementType = typeof(MeasurmentInk);

            var storedPointsField = measurementType.GetField("storedPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isStraightLineField = measurementType.GetField("isStraightLine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var originalMeasurementField = measurementType.GetField("originalMeasurement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var canvasLocalPositionField = measurementType.GetField("canvasLocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var point1LocalPositionField = measurementType.GetField("point1LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var point2LocalPositionField = measurementType.GetField("point2LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isAttachedToModelField = measurementType.GetField("isAttachedToModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var attachedModelIDField = measurementType.GetField("attachedModelID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Vector3[] storedPoints = storedPointsField?.GetValue(measurement) as Vector3[];
            if (storedPoints == null || storedPoints.Length < 2)
            {
                Debug.LogWarning($"Measurement has no points: {measurement.name}");
                continue;
            }

            MeasurementData measureData = new MeasurementData
            {
                storedPoints = storedPoints,
                isStraightLine = (bool)(isStraightLineField?.GetValue(measurement) ?? false),
                originalMeasurement = (float)(originalMeasurementField?.GetValue(measurement) ?? 0f),
                canvasLocalPosition = (Vector3)(canvasLocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                point1LocalPosition = (Vector3)(point1LocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                point2LocalPosition = (Vector3)(point2LocalPositionField?.GetValue(measurement) ?? Vector3.zero),
                isAttachedToModel = (bool)(isAttachedToModelField?.GetValue(measurement) ?? false),
                attachedModelID = (string)(attachedModelIDField?.GetValue(measurement) ?? ""),
                position = measurement.transform.position,
                rotation = measurement.transform.rotation,
                scale = measurement.transform.localScale,
                lineWidth = lineRenderer.startWidth
            };

            if (lineRenderer.material != null && lineRenderer.material.HasProperty("_Color"))
            {
                measureData.lineColor = lineRenderer.material.color;
            }

            if (measureData.isAttachedToModel && measurement.transform.parent != null)
            {
                string parentInstanceId = GetModelInstanceId(measurement.transform.parent.gameObject);
                if (!string.IsNullOrEmpty(parentInstanceId))
                {
                    measureData.attachedModelID = parentInstanceId;
                    Debug.Log($"  - Measurement attached to parent with instanceId: {parentInstanceId}");
                }
                else
                {
                    Debug.LogWarning($"  - Has parent but no instanceId found, saving as independent");
                    measureData.isAttachedToModel = false;
                    measureData.attachedModelID = "";
                }
            }
            else if (measureData.isAttachedToModel && string.IsNullOrEmpty(measureData.attachedModelID))
            {
                Debug.LogWarning($"  - Marked as attached but no parent found, saving as independent");
                measureData.isAttachedToModel = false;
            }

            session.measurements.Add(measureData);
        }

        // Collect drawings
        LineRenderer[] allDrawings = FindObjectsOfType<LineRenderer>();
        int drawingCount = 0;

        foreach (var drawing in allDrawings)
        {
            if (drawing.GetComponent<MeasurmentInk>() != null)
                continue;

            DrawingTracker tracker = drawing.GetComponent<DrawingTracker>();
            if (tracker == null)
                continue;

            Vector3[] points = new Vector3[drawing.positionCount];
            for (int i = 0; i < drawing.positionCount; i++)
            {
                points[i] = drawing.GetPosition(i);
            }

            if (points.Length < 2)
            {
                Debug.LogWarning($"Drawing has insufficient points: {drawing.name}");
                continue;
            }

            DrawingSessionData drawingData = new DrawingSessionData
            {
                points = points,
                width = drawing.startWidth,
                drawingId = tracker.drawingId,
                useWorldSpace = drawing.useWorldSpace
            };

            if (drawing.material != null && drawing.material.HasProperty("_Color"))
            {
                drawingData.lineColor = drawing.material.color;
            }
            else
            {
                drawingData.lineColor = Color.white;
            }

            drawingData.colorIndex = 0;

            if (drawing.transform.parent != null)
            {
                string parentInstanceId = GetModelInstanceId(drawing.transform.parent.gameObject);
                if (!string.IsNullOrEmpty(parentInstanceId))
                {
                    drawingData.isAttachedToModel = true;
                    drawingData.attachedModelID = parentInstanceId;
                    drawingData.parentPosition = drawing.transform.parent.position;
                    drawingData.parentRotation = drawing.transform.parent.rotation;
                }
                else
                {
                    drawingData.isAttachedToModel = false;
                }
            }
            else
            {
                drawingData.isAttachedToModel = false;
            }

            session.drawings.Add(drawingData);
            drawingCount++;
        }

        Debug.Log($"=== SESSION DATA COLLECTED ===\nModels: {session.models.Count}, Measurements: {session.measurements.Count}, Drawings: {session.drawings.Count}");
        return session;
    }

    // ===========================================
    // SAVE SESSION TO SERVER (Master Client Only)
    // ===========================================
    public void SaveSession(string sessionName, string userId = "")
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[PUN2] Only master client can save sessions in multiplayer!");
            return;
        }

        SessionData session = CollectSessionData();

        if (serverSessionHandler != null)
        {
            Debug.Log($"?? Saving session '{sessionName}' to server...");
            serverSessionHandler.SaveSessionToServer(sessionName, session, userId);
        }
        else
        {
            Debug.LogError("? ServerSessionHandler not initialized!");
        }
    }

    // ===========================================
    // LOAD SESSION FROM SERVER (Single Player Only)
    // ===========================================
    public void LoadSession(int sessionId)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Debug.LogWarning("[PUN2] Use LoadSessionMultiplayer() for multiplayer sessions!");
            LoadSessionMultiplayer(sessionId);
            return;
        }

        if (serverSessionHandler != null)
        {
            Debug.Log($"?? Loading session ID {sessionId} from server (single player)...");
            serverSessionHandler.LoadSessionFromServer(sessionId);
        }
        else
        {
            Debug.LogError("? ServerSessionHandler not initialized!");
        }
    }

    // ===========================================
    // APPLY LOADED SESSION
    // ===========================================
    private void ApplyLoadedSession(SessionData session)
    {
        if (isLoadingSession)
        {
            Debug.LogWarning("[SessionManager] Already loading a session, queuing this one...");
            return;
        }

        isLoadingSession = true;
        Debug.Log($"=== APPLYING LOADED SESSION ===\nModels: {session.models.Count}, Measurements: {session.measurements.Count}, Drawings: {session.drawings.Count}");

        // Destroy existing models
        GameObject[] existingModels = GameObject.FindGameObjectsWithTag(modelTag);
        foreach (var model in existingModels)
        {
            Destroy(model);
        }

        // Destroy existing measurements
        MeasurmentInk[] existingMeasurements = FindObjectsOfType<MeasurmentInk>();
        foreach (var measurement in existingMeasurements)
        {
            Destroy(measurement.gameObject);
        }

        // Destroy existing drawings
        DrawingTracker[] existingDrawings = FindObjectsOfType<DrawingTracker>();
        foreach (var drawing in existingDrawings)
        {
            Destroy(drawing.gameObject);
        }

        serverModelCache.Clear();
        modelInstanceMap.Clear();

        StartCoroutine(LoadModelsAndMeasurementsAndDrawingsAfterCleanup(session.models, session.measurements, session.drawings));
    }

    private IEnumerator LoadModelsAndMeasurementsAndDrawingsAfterCleanup(List<ModelData> models, List<MeasurementData> measurements, List<DrawingSessionData> drawings)
    {
        yield return null;

        // Load all models first
        Debug.Log("=== LOADING MODELS ===");
        foreach (var modelData in models)
        {
            if (modelData.isServerModel)
            {
                if (modelSample != null && !string.IsNullOrEmpty(modelData.modelId))
                {
                    modelSample.LoadModel(modelData.modelId, modelData.modelName, modelData.isTransparent, modelData.isChild);
                    yield return StartCoroutine(WaitAndApplyTransform(modelData));
                }
                else
                {
                    CreatePlaceholderModel(modelData);
                }
            }
            else
            {
                LoadLocalModel(modelData);
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Wait for all models to be ready
        yield return new WaitForSeconds(1f);

        // Now load measurements
        Debug.Log($"=== LOADING {measurements.Count} MEASUREMENTS ===");
        foreach (var measurementData in measurements)
        {
            LoadMeasurement(measurementData);
            yield return new WaitForSeconds(0.2f);
        }

        // Load drawings
        Debug.Log($"=== LOADING {drawings.Count} DRAWINGS ===");
        foreach (var drawingData in drawings)
        {
            LoadDrawing(drawingData);
            yield return new WaitForSeconds(0.1f);
        }

        isLoadingSession = false;
        Debug.Log("? SESSION LOADING COMPLETE ===");

        // Process any pending session loads
        if (pendingSessionData.Count > 0)
        {
            Debug.Log($"[PUN2] Processing {pendingSessionData.Count} pending session(s)...");
            string nextSession = pendingSessionData.Dequeue();
            ApplySessionDataFromNetwork(nextSession);
        }
    }

    // ===========================================
    // LOAD DRAWING
    // ===========================================
    private void LoadDrawing(DrawingSessionData drawingData)
    {
        GameObject drawingObj = new GameObject("LoadedDrawing_" + drawingData.drawingId);
        LineRenderer lineRenderer = drawingObj.AddComponent<LineRenderer>();

        if (drawingMaterial != null)
        {
            lineRenderer.material = new Material(drawingMaterial);
        }
        else
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            Debug.LogWarning("Drawing material not assigned, using default shader");
        }

        GameObject parentModel = null;
        if (drawingData.isAttachedToModel && !string.IsNullOrEmpty(drawingData.attachedModelID))
        {
            if (modelInstanceMap.TryGetValue(drawingData.attachedModelID, out parentModel))
            {
                drawingObj.transform.SetParent(parentModel.transform, false);
                drawingObj.transform.localPosition = Vector3.zero;
                drawingObj.transform.localRotation = Quaternion.identity;
                drawingObj.transform.localScale = Vector3.one;
                lineRenderer.useWorldSpace = false;
                Debug.Log($"? Drawing parented to model: {parentModel.name}");
            }
            else
            {
                Debug.LogWarning($"?? Parent model not found for drawing (ID: {drawingData.attachedModelID})");
                drawingData.isAttachedToModel = false;
                lineRenderer.useWorldSpace = true;
            }
        }
        else
        {
            lineRenderer.useWorldSpace = true;
        }

        lineRenderer.startWidth = drawingData.width;
        lineRenderer.endWidth = drawingData.width;

        if (drawingData.lineColor != Color.clear)
        {
            lineRenderer.startColor = drawingData.lineColor;
            lineRenderer.endColor = drawingData.lineColor;
            if (lineRenderer.material.HasProperty("_Color"))
            {
                lineRenderer.material.color = drawingData.lineColor;
            }
        }

        lineRenderer.positionCount = drawingData.points.Length;

        if (drawingData.isAttachedToModel && parentModel != null)
        {
            lineRenderer.SetPositions(drawingData.points);
        }
        else if (!drawingData.useWorldSpace && drawingData.isAttachedToModel)
        {
            lineRenderer.useWorldSpace = true;
            Matrix4x4 parentMatrix = Matrix4x4.TRS(drawingData.parentPosition, drawingData.parentRotation, Vector3.one);

            for (int i = 0; i < drawingData.points.Length; i++)
            {
                Vector3 worldPoint = parentMatrix.MultiplyPoint3x4(drawingData.points[i]);
                lineRenderer.SetPosition(i, worldPoint);
            }
        }
        else
        {
            lineRenderer.SetPositions(drawingData.points);
        }

        DrawingTracker tracker = drawingObj.AddComponent<DrawingTracker>();
        tracker.drawingId = drawingData.drawingId;
        drawingObj.AddComponent<Deletable>();

        if (drawingLayerMask != 0)
        {
            drawingObj.layer = (int)Mathf.Log(drawingLayerMask.value, 2);
        }

        BoxCollider collider = drawingObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        if (lineRenderer.useWorldSpace)
        {
            if (lineRenderer.bounds.size.magnitude > 0)
            {
                collider.center = lineRenderer.bounds.center;
                collider.size = lineRenderer.bounds.size;
            }
            else
            {
                collider.size = Vector3.one * 0.1f;
            }
        }
        else
        {
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 point = lineRenderer.GetPosition(i);
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;

            size.x = Mathf.Max(size.x, 0.01f);
            size.y = Mathf.Max(size.y, 0.01f);
            size.z = Mathf.Max(size.z, 0.01f);

            collider.center = center;
            collider.size = size;
        }
    }

    // ===========================================
    // LOAD MEASUREMENT
    // ===========================================
    private void LoadMeasurement(MeasurementData measurementData)
    {
        GameObject measurementPrefab = Resources.Load<GameObject>("MesurmentInk");
        if (measurementPrefab == null)
        {
            Debug.LogError("MesurmentInk prefab not found in Resources!");
            return;
        }

        GameObject measurementObj = Instantiate(measurementPrefab, Vector3.zero, Quaternion.identity);
        measurementObj.name = "LoadedMeasurement";

        MeasurmentInk inkComponent = measurementObj.GetComponent<MeasurmentInk>();
        LineRenderer lineRenderer = measurementObj.GetComponent<LineRenderer>();

        if (inkComponent == null || lineRenderer == null)
        {
            Debug.LogError("MesurmentInk or LineRenderer component missing!");
            Destroy(measurementObj);
            return;
        }

        var measurementType = typeof(MeasurmentInk);

        var storedPointsField = measurementType.GetField("storedPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isStraightLineField = measurementType.GetField("isStraightLine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var originalMeasurementField = measurementType.GetField("originalMeasurement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasLocalPositionField = measurementType.GetField("canvasLocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point1LocalPositionField = measurementType.GetField("point1LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point2LocalPositionField = measurementType.GetField("point2LocalPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isAttachedToModelField = measurementType.GetField("isAttachedToModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attachedModelIDField = measurementType.GetField("attachedModelID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var isInitializedField = measurementType.GetField("isInitialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var attachedModelField = measurementType.GetField("attachedModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasField = measurementType.GetField("canvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point1Field = measurementType.GetField("point1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var point2Field = measurementType.GetField("point2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Transform canvas = canvasField?.GetValue(inkComponent) as Transform;
        GameObject point1 = point1Field?.GetValue(inkComponent) as GameObject;
        GameObject point2 = point2Field?.GetValue(inkComponent) as GameObject;

        GameObject parentModel = null;
        if (measurementData.isAttachedToModel && !string.IsNullOrEmpty(measurementData.attachedModelID))
        {
            if (modelInstanceMap.TryGetValue(measurementData.attachedModelID, out parentModel))
            {
                measurementObj.transform.SetParent(parentModel.transform, false);
                measurementObj.transform.localPosition = Vector3.zero;
                measurementObj.transform.localRotation = Quaternion.identity;
                measurementObj.transform.localScale = Vector3.one;
                attachedModelField?.SetValue(inkComponent, parentModel.transform);
                Debug.Log($"? Measurement parented to model: {parentModel.name}");
            }
            else
            {
                Debug.LogWarning($"?? Parent model not found (ID: {measurementData.attachedModelID})");
                measurementData.isAttachedToModel = false;
            }
        }

        Vector3[] pointsToStore = measurementData.storedPoints;

        storedPointsField?.SetValue(inkComponent, pointsToStore);
        isStraightLineField?.SetValue(inkComponent, measurementData.isStraightLine);
        originalMeasurementField?.SetValue(inkComponent, measurementData.originalMeasurement);
        canvasLocalPositionField?.SetValue(inkComponent, measurementData.canvasLocalPosition);
        point1LocalPositionField?.SetValue(inkComponent, measurementData.point1LocalPosition);
        point2LocalPositionField?.SetValue(inkComponent, measurementData.point2LocalPosition);
        isAttachedToModelField?.SetValue(inkComponent, measurementData.isAttachedToModel);
        attachedModelIDField?.SetValue(inkComponent, measurementData.attachedModelID);
        isInitializedField?.SetValue(inkComponent, true);

        lineRenderer.startWidth = lineRenderer.endWidth = measurementData.lineWidth;
        lineRenderer.useWorldSpace = true;

        if (measurementData.lineColor != Color.clear && lineRenderer.material != null)
        {
            lineRenderer.material.color = measurementData.lineColor;
        }

        if (measurementData.isAttachedToModel && parentModel != null)
        {
            if (measurementData.isStraightLine && measurementData.storedPoints.Length >= 2)
            {
                lineRenderer.positionCount = 2;
                Vector3 worldPoint1 = parentModel.transform.TransformPoint(measurementData.storedPoints[0]);
                Vector3 worldPoint2 = parentModel.transform.TransformPoint(measurementData.storedPoints[measurementData.storedPoints.Length - 1]);
                lineRenderer.SetPosition(0, worldPoint1);
                lineRenderer.SetPosition(1, worldPoint2);
            }
            else
            {
                lineRenderer.positionCount = measurementData.storedPoints.Length;
                Vector3[] worldPoints = new Vector3[measurementData.storedPoints.Length];

                for (int i = 0; i < measurementData.storedPoints.Length; i++)
                {
                    worldPoints[i] = parentModel.transform.TransformPoint(measurementData.storedPoints[i]);
                }

                lineRenderer.SetPositions(worldPoints);
            }

            if (canvas != null && measurementData.canvasLocalPosition != Vector3.zero)
            {
                Vector3 worldCanvasPos = parentModel.transform.TransformPoint(measurementData.canvasLocalPosition);
                canvas.position = worldCanvasPos;
            }

            if (point1 != null && measurementData.point1LocalPosition != Vector3.zero)
            {
                Vector3 worldPoint1Pos = parentModel.transform.TransformPoint(measurementData.point1LocalPosition);
                point1.transform.position = worldPoint1Pos;
            }

            if (point2 != null && measurementData.point2LocalPosition != Vector3.zero)
            {
                Vector3 worldPoint2Pos = parentModel.transform.TransformPoint(measurementData.point2LocalPosition);
                point2.transform.position = worldPoint2Pos;
            }
        }
        else
        {
            measurementObj.transform.position = measurementData.position;
            measurementObj.transform.rotation = measurementData.rotation;
            measurementObj.transform.localScale = measurementData.scale;

            if (measurementData.isStraightLine && measurementData.storedPoints.Length >= 2)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, measurementData.storedPoints[0]);
                lineRenderer.SetPosition(1, measurementData.storedPoints[measurementData.storedPoints.Length - 1]);
            }
            else
            {
                lineRenderer.positionCount = measurementData.storedPoints.Length;
                lineRenderer.SetPositions(measurementData.storedPoints);
            }

            if (canvas != null && measurementData.canvasLocalPosition != Vector3.zero)
            {
                canvas.position = measurementData.canvasLocalPosition;
            }

            if (point1 != null && measurementData.point1LocalPosition != Vector3.zero)
            {
                point1.transform.position = measurementData.point1LocalPosition;
            }

            if (point2 != null && measurementData.point2LocalPosition != Vector3.zero)
            {
                point2.transform.position = measurementData.point2LocalPosition;
            }
        }

        var lengthTextField = measurementType.GetField("lengthText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lengthText = lengthTextField?.GetValue(inkComponent) as TMPro.TMP_Text;
        if (lengthText != null)
        {
            lengthText.text = $"{measurementData.originalMeasurement:0.00} mm";
        }

        if (canvas != null)
        {
            var orientMethod = measurementType.GetMethod("OrientCanvasToHead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            orientMethod?.Invoke(inkComponent, null);
        }
    }

    // ===========================================
    // WAIT AND APPLY TRANSFORM
    // ===========================================
    private IEnumerator WaitAndApplyTransform(ModelData modelData)
    {
        float timeout = 10f;
        float elapsed = 0f;
        GameObject foundModel = null;

        while (elapsed < timeout && foundModel == null)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;

            GameObject[] currentModels = GameObject.FindGameObjectsWithTag(modelTag);
            foreach (var model in currentModels)
            {
                string existingInstanceId = GetModelInstanceId(model);
                if (string.IsNullOrEmpty(existingInstanceId))
                    continue;

                MeshFilter mf = model.GetComponent<MeshFilter>();
                if (mf != null && mf.mesh != null)
                {
                    bool isMatch = false;

                    if (!string.IsNullOrEmpty(modelData.modelId))
                    {
                        isMatch = true;
                    }

                    if (isMatch)
                    {
                        foundModel = model;
                        break;
                    }
                }
            }
        }

        if (foundModel != null)
        {
            foundModel.transform.position = modelData.position;
            foundModel.transform.rotation = modelData.rotation;
            foundModel.transform.localScale = modelData.scale;

            SetModelInstanceId(foundModel, modelData.modelInstanceId);

            Debug.Log($"? Model loaded: {foundModel.name} (ID: {modelData.modelInstanceId})");

            CacheServerModel(foundModel, modelData.modelId, modelData.modelName,
                            modelData.isTransparent, modelData.isChild,
                            modelData.modelUrl, modelData.textureUrl);
        }
        else
        {
            Debug.LogWarning($"?? Download failed for {modelData.modelName}, creating placeholder...");
            CreatePlaceholderModel(modelData);
        }
    }

    // ===========================================
    // LOAD LOCAL MODEL
    // ===========================================
    private void LoadLocalModel(ModelData modelData)
    {
        GameObject prefab = Resources.Load<GameObject>(modelData.modelPath);
        GameObject model;

        if (prefab != null)
        {
            model = Instantiate(prefab);
            model.name = prefab.name;
        }
        else
        {
            model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            model.name = modelData.modelPath.Replace("Models/", "");
        }

        SetModelInstanceId(model, modelData.modelInstanceId);
        ApplyModelTransform(model, modelData);
        Debug.Log($"? Loaded local model: {model.name}");
    }

    // ===========================================
    // APPLY MODEL TRANSFORM
    // ===========================================
    private void ApplyModelTransform(GameObject model, ModelData modelData)
    {
        model.transform.position = modelData.position;
        model.transform.rotation = modelData.rotation;
        model.transform.localScale = modelData.scale;
        model.tag = modelData.tag;
    }

    // ===========================================
    // CREATE PLACEHOLDER MODEL
    // ===========================================
    private void CreatePlaceholderModel(ModelData modelData)
    {
        GameObject model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.name = modelData.modelName + "_Placeholder";

        SetModelInstanceId(model, modelData.modelInstanceId);
        ApplyModelTransform(model, modelData);

        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        Debug.LogWarning($"?? Created placeholder for: {modelData.modelName}");
    }

    // ===========================================
    // ON SERVER MODEL LOADED
    // ===========================================
    public void OnServerModelLoaded(GameObject model, string modelId, string modelName, bool isTransparent, bool isChild)
    {
        if (model != null)
        {
            string modelUrl = "";
            string textureUrl = "";

            if (modelSample != null)
            {
                modelUrl = modelSample.CurrentModelUrl;
                textureUrl = modelSample.CurrentTextureUrl;
            }

            string actualModelId = modelId;
            if (!string.IsNullOrEmpty(modelUrl) && modelUrl.Contains("model_id="))
            {
                int startIndex = modelUrl.IndexOf("model_id=") + "model_id=".Length;
                int endIndex = modelUrl.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = modelUrl.Length;

                string extractedId = modelUrl.Substring(startIndex, endIndex - startIndex);
                if (!string.IsNullOrEmpty(extractedId))
                {
                    actualModelId = extractedId;
                }
            }

            CacheServerModel(model, actualModelId, modelName, isTransparent, isChild, modelUrl, textureUrl);
        }
    }

    // ===========================================
    // SERVER EVENT CALLBACKS
    // ===========================================
    private void OnServerSessionSaved(int sessionId)
    {
        Debug.Log($"? Session saved to server with ID: {sessionId}");
        // You can add UI notification here
    }

    private void OnServerSessionLoaded(string sessionDataJson)
    {
        Debug.Log($"?? Session data loaded from server");

        // If we're the master client in multiplayer, broadcast to all clients
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[PUN2] Master client broadcasting session data to all players...");
            BroadcastSessionData(sessionDataJson);
        }
        else if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            // Single player mode - apply directly
            Debug.Log("[SinglePlayer] Applying session data directly...");
            try
            {
                SessionData session = JsonUtility.FromJson<SessionData>(sessionDataJson);
                ApplyLoadedSession(session);
            }
            catch (Exception e)
            {
                Debug.LogError($"? Failed to parse loaded session: {e.Message}");
            }
        }
        else
        {
            // Non-master clients should not directly load from server
            Debug.LogWarning("[PUN2] Non-master client received server data directly - this shouldn't happen in multiplayer");
        }
    }

    private void OnServerError(string errorMessage)
    {
        Debug.LogError($"? Server error: {errorMessage}");
        // You can add UI error notification here
    }

    // ===========================================
    // UTILITY METHODS
    // ===========================================
    public void ClearCache()
    {
        serverModelCache.Clear();
        modelInstanceMap.Clear();
        Debug.Log("Cache cleared");
    }

    public bool HasCachedModel(string modelKey)
    {
        return serverModelCache.ContainsKey(modelKey);
    }

    public void SaveCurrentSession()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[PUN2] Only master client can save sessions in multiplayer!");
            return;
        }

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        SaveSession("Session_" + timestamp);
    }

    // ===========================================
    // PHOTON CALLBACKS
    // ===========================================
    public override void OnJoinedRoom()
    {
        Debug.Log($"[PUN2] ? Joined room: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"[PUN2] Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}");
        Debug.Log($"[PUN2] I am master: {PhotonNetwork.IsMasterClient}");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("[PUN2] ?? Left room");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"[PUN2] ?? Master client switched to: {newMasterClient.NickName}");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[PUN2] I am now the master client!");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PUN2] ? Player joined: {newPlayer.NickName}");
        Debug.Log($"[PUN2] Total players: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PUN2] ? Player left: {otherPlayer.NickName}");
        Debug.Log($"[PUN2] Total players: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"[PUN2] ?? Disconnected from Photon: {cause}");
    }

    // ===========================================
    // CLEANUP
    // ===========================================
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (serverSessionHandler != null)
        {
            serverSessionHandler.OnSessionSaved -= OnServerSessionSaved;
            serverSessionHandler.OnSessionLoaded -= OnServerSessionLoaded;
            serverSessionHandler.OnError -= OnServerError;
        }

        // Unregister Photon events
        if (PhotonNetwork.NetworkingClient != null)
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnPhotonEvent;
        }
    }
}

// ===========================================
// HELPER COMPONENTS
// ===========================================

/// <summary>
/// Helper component to store unique instance IDs on models
/// </summary>
public class ModelInstanceId : MonoBehaviour
{
    public string instanceId;
}

/// <summary>
/// Helper component for tracking drawings
/// </summary>
/*public class DrawingTracker : MonoBehaviour
{
    public string drawingId;
}

/// <summary>
/// Deletable marker component (if not already defined elsewhere)
/// </summary>
public class Deletable : MonoBehaviour
{
    // Marker component for deletable objects
}*/