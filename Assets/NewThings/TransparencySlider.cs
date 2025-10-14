using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TransparencySlider : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider transparencySlider;

    [Header("Target Material Name")]
    [SerializeField] private string targetMaterialName = "Cross-Section-Material_Transparent";

    private Material linkedMaterial;
    private ModelObject currentSelectedModel;
    private int currentModelViewID = -1;

    // Shader property names to try
    private string[] possiblePropertyNames = new string[]
    {
        "_TextureTransparency",
        "TextureTransparency",
        "_Transparency",
        "_Alpha"
    };

    private string activePropertyName = null;

    private void Start()
    {
        if (transparencySlider == null)
        {
            Debug.LogError("[TransparencySlider] ? No Slider assigned!");
            enabled = false;
            return;
        }

        // Set slider range
        transparencySlider.minValue = 0f;
        transparencySlider.maxValue = 1f;
        transparencySlider.value = 1f; // Default fully opaque

        transparencySlider.onValueChanged.AddListener(OnUISliderChanged);

        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.OnSelectedObject += OnSelectedObjectChanged;
        }

        // Initial state
        transparencySlider.interactable = false;
        Debug.Log("[TransparencySlider] Initialized and waiting for model selection...");
    }

    private void OnDestroy()
    {
        if (transparencySlider != null)
            transparencySlider.onValueChanged.RemoveListener(OnUISliderChanged);

        if (SelectionManager.Instance != null)
            SelectionManager.Instance.OnSelectedObject -= OnSelectedObjectChanged;
    }

    private void OnSelectedObjectChanged()
    {
        Debug.Log("[TransparencySlider] ?? Selection changed, re-linking material...");
        TryLinkMaterial();
    }

    private void TryLinkMaterial()
    {
        // Clear previous link
        linkedMaterial = null;
        currentSelectedModel = null;
        currentModelViewID = -1;
        activePropertyName = null;

        // Get the selected ModelObject
        ModelObject selectedModel = SelectionManager.Instance?.GetSelectedModel();

        if (selectedModel == null)
        {
            transparencySlider.interactable = false;
            Debug.Log("[TransparencySlider] ? No model selected - slider disabled.");
            return;
        }

        currentSelectedModel = selectedModel;

        // Get the GameObject from ModelObject
        GameObject selectedGameObject = selectedModel.gameObject;

        // Get PhotonView ID for network sync
        PhotonView photonView = selectedGameObject.GetComponent<PhotonView>();
        if (photonView != null)
        {
            currentModelViewID = photonView.ViewID;
            Debug.Log($"[TransparencySlider] ?? Model has PhotonView ID: {currentModelViewID}");
        }

        // Search for the transparent material
        var renderers = selectedGameObject.GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            // Use materials (creates instance automatically if needed)
            foreach (var mat in renderer.materials)
            {
                if (mat == null) continue;

                // Remove " (Instance)" from material name for comparison
                string cleanMatName = mat.name.Replace(" (Instance)", "");

                if (cleanMatName.Contains(targetMaterialName))
                {
                    // Find which property name exists
                    foreach (string propName in possiblePropertyNames)
                    {
                        if (mat.HasProperty(propName))
                        {
                            linkedMaterial = mat;
                            activePropertyName = propName;

                            float currentValue = mat.GetFloat(propName);

                            // Update slider without triggering callback
                            transparencySlider.SetValueWithoutNotify(currentValue);
                            transparencySlider.interactable = true;

                            Debug.Log($"[TransparencySlider] ? Linked to '{mat.name}' on '{selectedGameObject.name}'");
                            Debug.Log($"[TransparencySlider] ?? Using property: '{propName}' | Current value: {currentValue}");
                            return;
                        }
                    }

                    // Material found but no valid property
                    Debug.LogWarning($"[TransparencySlider] ?? Material '{mat.name}' found but missing transparency property!");
                    Debug.LogWarning($"[TransparencySlider] Tried properties: {string.Join(", ", possiblePropertyNames)}");
                }
            }
        }

        // No matching material found
        transparencySlider.interactable = false;
        Debug.LogWarning($"[TransparencySlider] ?? No material containing '{targetMaterialName}' found in '{selectedGameObject.name}'");

        // Log all materials for debugging
        Debug.Log("[TransparencySlider] ?? Available materials in selected model:");
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat != null)
                {
                    Debug.Log($"  - {mat.name}");
                }
            }
        }
    }

    private void OnUISliderChanged(float value)
    {
        if (linkedMaterial == null || activePropertyName == null)
        {
            Debug.LogWarning("[TransparencySlider] ?? No linked material - cannot change value");
            return;
        }

        if (linkedMaterial.HasProperty(activePropertyName))
        {
            linkedMaterial.SetFloat(activePropertyName, value);
            Debug.Log($"[TransparencySlider] ?? Updated '{linkedMaterial.name}' ? {activePropertyName} = {value}");

            // Optional: Sync over network if using Photon
            if (currentModelViewID != -1 && PhotonNetwork.InRoom)
            {
                PhotonView pv = GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    pv.RPC("SyncTransparency", RpcTarget.OthersBuffered, currentModelViewID, value);
                }
            }
        }
        else
        {
            Debug.LogError($"[TransparencySlider] ? Property '{activePropertyName}' no longer exists on material!");
        }
    }

    // Network sync for multiplayer
    [PunRPC]
    void SyncTransparency(int viewID, float value)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView == null)
        {
            Debug.LogWarning($"[TransparencySlider] Cannot sync - ViewID {viewID} not found");
            return;
        }

        var renderers = targetView.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat != null && mat.name.Contains(targetMaterialName.Replace(" (Instance)", "")))
                {
                    foreach (string propName in possiblePropertyNames)
                    {
                        if (mat.HasProperty(propName))
                        {
                            mat.SetFloat(propName, value);
                            Debug.Log($"[TransparencySlider] ?? Synced transparency to {value} on ViewID {viewID}");
                            return;
                        }
                    }
                }
            }
        }
    }
}