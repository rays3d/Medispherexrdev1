using UnityEngine;
using Photon.Pun;   // ✅ Photon Added

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

    // --- Multiplayer ---
    private PhotonView _photonView; // ✅ PhotonView reference for multiplayer stroke ownership

    // --- Internal References ---
    private BrushStrokeMesh _brushStrokeMesh; // Component responsible for generating and deforming the 3D geometry
    private ControlPointParentManager _controlPointParentManager; // Component responsible for organizing and managing control point GameObjects

    // --- Initialization ---
    private void Awake()
    {
        // ✅ Get or Add PhotonView Component
        _photonView = GetComponent<PhotonView>();
        if (_photonView == null)
        {
            _photonView = gameObject.AddComponent<PhotonView>();
        }

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
        _controlPointParentManager = _brushStrokeMesh.GetComponent<ControlPointParentManager>();
        if (_controlPointParentManager == null)
        {
            _controlPointParentManager = _brushStrokeMesh.gameObject.AddComponent<ControlPointParentManager>();
        }
    }

    // --- Public Stroke Drawing Methods ---

    public void BeginBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation)
    {
        if (!_photonView.IsMine) return; // ✅ Only owner modifies stroke

        if (_brushStrokeMesh != null)
        {
            _brushStrokeMesh.ClearRibbon();
            _brushStrokeMesh.InsertRibbonPoint(position, rotation);
        }
    }

    public void MoveBrushTipToPoint(Vector3 position, Quaternion rotation)
    {
        if (!_photonView.IsMine) return;

        if (_brushStrokeMesh != null)
        {
            _brushStrokeMesh.InsertRibbonPoint(position, rotation);
        }
    }

    public void EndBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation)
    {
        if (!_photonView.IsMine) return;

        if (_brushStrokeMesh != null)
        {
            _brushStrokeMesh.InsertRibbonPoint(position, rotation);
        }

        if (_controlPointParentManager != null)
        {
            _controlPointParentManager.OnStrokeCompleted();
        }
    }

    public void UpdateBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation)
    {
        if (!_photonView.IsMine) return;

        if (_brushStrokeMesh != null)
        {
            _brushStrokeMesh.UpdateLastRibbonPoint(position, rotation);
        }
    }

    // --- Public Management Methods ---

    public void OrganizeControlPoints()
    {
        if (_controlPointParentManager != null)
        {
            _controlPointParentManager.ManualOrganize();
        }
    }

    public GameObject GetControlPointsParent()
    {
        if (_controlPointParentManager != null)
        {
            return _controlPointParentManager.GetControlPointsParent();
        }
        return null;
    }

    public bool AreControlPointsOrganized()
    {
        if (_controlPointParentManager != null)
        {
            return _controlPointParentManager.IsOrganized();
        }
        return false;
    }
}
