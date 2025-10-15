using UnityEngine;

/// <summary>
/// The main component representing a single brush stroke.
/// It acts as a facade, managing and coordinating the mesh generation (BrushStrokeMesh) 
/// and the organization of interactive control points (ControlPointParentManager).
/// </summary>
public class BrushStroke : MonoBehaviour
{
    // --- Editor Configuration ---
    [Header("Brush Stroke Settings")]
    [SerializeField] private Material _brushStrokeMaterial = null; // Material to be applied to the generated mesh

    // --- Internal References ---
    private BrushStrokeMesh _brushStrokeMesh; // Component responsible for generating and deforming the 3D geometry
    private ControlPointParentManager _controlPointParentManager; // Component responsible for organizing and managing control point GameObjects

    // --- Initialization ---
    private void Awake()
    {
        // 1. Initialize BrushStrokeMesh
        _brushStrokeMesh = GetComponentInChildren<BrushStrokeMesh>();
        if (_brushStrokeMesh == null)
        {
            // If the mesh component doesn't exist, create a new child GameObject for it
            GameObject meshObject = new GameObject("BrushStrokeMesh");
            meshObject.transform.SetParent(transform);
            _brushStrokeMesh = meshObject.AddComponent<BrushStrokeMesh>();
            
            // Add a MeshRenderer to display the geometry
            MeshRenderer renderer = meshObject.AddComponent<MeshRenderer>();
            
            // Apply the configured material
            if (_brushStrokeMaterial != null)
            {
                renderer.material = _brushStrokeMaterial;
            }
        }

        // 2. Initialize ControlPointParentManager
        // This component is responsible for handling the grouping and cleanup of the control point GameObjects
        _controlPointParentManager = _brushStrokeMesh.GetComponent<ControlPointParentManager>();
        if (_controlPointParentManager == null)
        {
            // Add the manager to the same GameObject as the BrushStrokeMesh for easy reference
            _controlPointParentManager = _brushStrokeMesh.gameObject.AddComponent<ControlPointParentManager>();
        }
    }

    // --- Public Stroke Drawing Methods ---

    /// <summary>
    /// Starts a new brush stroke. Clears any previous data in the mesh and inserts the very first point.
    /// </summary>
    /// <param name="position">The starting world position of the brush tip.</param>
    /// <param name="rotation">The starting world rotation of the brush tip.</param>
    public void BeginBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation)
    {
        if (_brushStrokeMesh != null)
        {
            _brushStrokeMesh.ClearRibbon();
            _brushStrokeMesh.InsertRibbonPoint(position, rotation);
        }
    }

    /// <summary>
    /// Continues the brush stroke by inserting a new point.
    /// The BrushStrokeMesh determines if the distance threshold has been met to create a new ring.
    /// </summary>
    /// <param name="position">The current world position of the brush tip.</param>
    /// <param name="rotation">The current world rotation of the brush tip.</param>
    public void MoveBrushTipToPoint(Vector3 position, Quaternion rotation)
    {
        if (_brushStrokeMesh != null)
        {
            _brushStrokeMesh.InsertRibbonPoint(position, rotation);
        }
    }

    /// <summary>
    /// Finalizes the brush stroke. Inserts the last point and triggers the organization of control points.
    /// </summary>
    /// <param name="position">The ending world position of the brush tip.</param>
    /// <param name="rotation">The ending world rotation of the brush tip.</param>
    public void EndBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation)
    {
        if (_brushStrokeMesh != null)
        {
            // Insert the final point
            _brushStrokeMesh.InsertRibbonPoint(position, rotation);
        }

        // Organize control points when the stroke is completed to group them under a single parent
        if (_controlPointParentManager != null)
        {
            _controlPointParentManager.OnStrokeCompleted();
        }
    }

    /// <summary>
    /// Updates the position and rotation of the last point added to the stroke without adding a new point.
    /// (Currently relies on an unimplemented method in BrushStrokeMesh, typically used for rubber-banding the end of a stroke.)
    /// </summary>
    /// <param name="position">The updated world position.</param>
    /// <param name="rotation">The updated world rotation.</param>
    public void UpdateBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation)
    {
        if (_brushStrokeMesh != null)
        {
            _brushStrokeMesh.UpdateLastRibbonPoint(position, rotation);
        }
    }

    // --- Public Management Methods ---

    /// <summary>
    /// Manually organizes the control points by grouping them under a dedicated parent.
    /// </summary>
    public void OrganizeControlPoints()
    {
        if (_controlPointParentManager != null)
        {
            _controlPointParentManager.ManualOrganize();
        }
    }

    /// <summary>
    /// Gets the parent GameObject that contains all the control points for this stroke.
    /// </summary>
    /// <returns>The control points parent GameObject or null.</returns>
    public GameObject GetControlPointsParent()
    {
        if (_controlPointParentManager != null)
        {
            return _controlPointParentManager.GetControlPointsParent();
        }
        return null;
    }

    /// <summary>
    /// Checks if the control points have been organized into a dedicated parent.
    /// </summary>
    /// <returns>True if the control points are organized, false otherwise.</returns>
    public bool AreControlPointsOrganized()
    {
        if (_controlPointParentManager != null)
        {
            return _controlPointParentManager.IsOrganized();
        }
        return false;
    }
}