using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ModelAccessController : MonoBehaviour
{
    [Header("Lock/Unlock Mesh Objects")]
    public GameObject lockedMesh;     // Assign lock model in inspector
    public GameObject unlockedMesh;   // Assign unlock model in inspector

    [Header("Models")]
    public List<GameObject> parentModels = new List<GameObject>();

    [Header("Input Action")]
    public InputActionProperty toggleAction; // Secondary button on left hand

    private bool isAccessMode = true;
    private void Start()
    {
        SetAccessMode();        // Enable colliders at start
        UpdateMeshVisibility(); // Show unlock mesh at start
    }

    private void OnEnable()
    {
        toggleAction.action.Enable();
        toggleAction.action.performed += ToggleMode;
    }

    private void OnDisable()
    {
        toggleAction.action.performed -= ToggleMode;
        toggleAction.action.Disable();
    }

    private void ToggleMode(InputAction.CallbackContext context)
    {
        if (isAccessMode)
            SetNonAccessMode();
        else
            SetAccessMode();
    }

    public void SetParentModel(GameObject model)
    {
        if (!parentModels.Contains(model))
        {
            parentModels.Add(model);
            Debug.Log($"Model added: {model.name}");
        }
    }

    public void SetAccessMode()
    {
        foreach (GameObject parent in parentModels)
        {
            if (parent == null) continue;
            BoxCollider[] colliders = parent.GetComponentsInChildren<BoxCollider>(true);
            foreach (BoxCollider col in colliders)
            {
                if (col.gameObject != parent)
                    col.enabled = true;
            }
        }

        isAccessMode = true;
        UpdateMeshVisibility();
        Debug.Log("Access Mode Enabled");
    }

    public void SetNonAccessMode()
    {
        foreach (GameObject parent in parentModels)
        {
            if (parent == null) continue;
            BoxCollider[] colliders = parent.GetComponentsInChildren<BoxCollider>(true);
            foreach (BoxCollider col in colliders)
            {
                if (col.gameObject != parent)
                    col.enabled = false;
            }
        }

        isAccessMode = false;
        UpdateMeshVisibility();
        Debug.Log("Non-Access Mode Enabled");
    }

    private void UpdateMeshVisibility()
    {
        if (lockedMesh != null) lockedMesh.SetActive(!isAccessMode);
        if (unlockedMesh != null) unlockedMesh.SetActive(isAccessMode);
    }
}
