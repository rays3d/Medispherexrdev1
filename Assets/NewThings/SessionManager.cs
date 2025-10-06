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






using UnityEngine;
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
}























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