using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // Assign the object you want to toggle

    public void Toggle()
    {
        if (targetObject != null)
        {
            bool isActive = targetObject.activeSelf;
            targetObject.SetActive(!isActive);
        }
    }
}
