using UnityEngine;

public class AttachToSelectedModel : MonoBehaviour
{
    void Start()
    {
        // Try to find the selected model
        var selectedModel = SelectionManager.Instance?.GetSelectedModel();
        if (selectedModel != null)
        {
            transform.SetParent(selectedModel.transform);
            Debug.Log($"{gameObject.name} attached to {selectedModel.name}");
        }
        else
        {
            Debug.LogWarning("No selected model found. Object will stay unparented.");
        }

        // Add Deletable component if missing
        if (!GetComponent<Deletable>())
        {
            gameObject.AddComponent<Deletable>();
        }

        // Add BoxCollider if missing
        if (!GetComponent<Collider>())
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }
}
