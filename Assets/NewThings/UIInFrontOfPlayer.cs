using UnityEngine;

/// <summary>
/// This script positions a UI canvas in front of the XR camera at a specified distance.
/// Attach this to your Canvas or UI parent object and assign the XR camera.
/// </summary>
public class UIInFrontOfPlayer : MonoBehaviour
{
    [Header("Camera Reference")]
    [Tooltip("Drag in the XR Rig's main camera (usually the center eye).")]
    public Transform xrCamera;

    [Header("UI Position Settings")]
    [Tooltip("Distance in meters in front of the camera.")]
    public float distanceFromCamera = 2f;

    [Tooltip("Additional offset if you want to tweak the position manually.")]
    public Vector3 offset = Vector3.zero;

    void Start()
    {
        if (xrCamera == null)
        {
            Debug.LogWarning("XR Camera not assigned. Trying to find the MainCamera tag...");
            Camera mainCam = Camera.main;
            if (mainCam != null)
                xrCamera = mainCam.transform;
        }

        PositionUI();
    }

    /// <summary>
    /// Positions the UI canvas in front of the XR camera.
    /// </summary>
    public void PositionUI()
    {
        if (xrCamera == null)
        {
            Debug.LogError("XR Camera is not assigned!");
            return;
        }

        // Calculate forward direction (optional: ignore Y for horizontal positioning)
        Vector3 forward = xrCamera.forward;
        forward.y = 0;
        forward.Normalize();

        // Set position in front of camera + offset
        transform.position = xrCamera.position + forward * distanceFromCamera + offset;

        // Make UI face the camera
        transform.LookAt(xrCamera);
        transform.Rotate(0, 180, 0); // Flip to face the player properly
    }
}
