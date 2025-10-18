using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Manages the organization of individual brush stroke control points into a single,
/// network-instantiated, and interactable parent object once the stroke is complete.
/// </summary>
public class ControlPointParentManager : MonoBehaviour
{
    #region Configuration Fields
    [Header("Parent Object Settings")]
    [Tooltip("The name prefix used for the control point parent GameObject.")]
    [SerializeField] private bool createParentOnStrokeEnd = true;
    [Tooltip("If true, draws a gizmo at the parent's center for visualization.")]
    [SerializeField] private bool showParentGizmo = true;
    [Tooltip("Color of the debug gizmo.")]
    [SerializeField] private Color gizmoColor = Color.yellow;
    [Tooltip("Size of the debug gizmo sphere and line.")]
    [SerializeField] private float gizmoSize = 0.02f;
    #endregion

    // Event to notify listeners that hierarchy is ready


    #region Internal State
    private GameObject _controlPointsParent = null; // The instantiated parent object
    private bool _strokeCompleted = false; // Tracks if the stroke has finished
    #endregion


    #region Networking Setup
    [Tooltip("Reference to the PhotonView on this GameObject for sending RPCs.")]
    private PhotonView _photonView;

    private void Awake()
    {
        // Ensure the PhotonView reference is set
        _photonView = GetComponent<PhotonView>();
        if (_photonView == null)
        {
            Debug.LogError("ControlPointParentManager requires a PhotonView on its GameObject.");
        }
    }
    #endregion

    #region Core Logic - Organization
    /// <summary>
    /// Call this when the stroke is completed to organize control points.
    /// This is typically called by the BrushStroke component.
    /// </summary>
    public void OnStrokeCompleted()
    {
        // Only run this logic once per stroke
        if (_strokeCompleted) return;

        if (createParentOnStrokeEnd)
        {
            OrganizeControlPoints();
        }

        _strokeCompleted = true;
    }

    /// <summary>
    /// Manually triggers the organization process.
    /// </summary>
    public void ManualOrganize() => OrganizeControlPoints();

    /// <summary>
    /// Finds all control point GameObjects (children of this component's transform)
    /// and parents them under a new network-instantiated parent object.
    /// This should only be called by the stroke owner.
    /// </summary>
    public void OrganizeControlPoints()
    {
        // 1️⃣ Collect control points (children named "ControlPoint_...")
        List<Transform> controlPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("ControlPoint_"))
                controlPoints.Add(child);
        }

        if (controlPoints.Count == 0)
        {
            Debug.LogWarning("OrganizeControlPoints called but no control points found.");
            return;
        }

        // 2️⃣ Calculate the average position to center the new parent object
        Vector3 centerPosition = CalculateCenterPosition(controlPoints);

        // 3️⃣ Instantiate the parent over the network (owner only)
        // The prefab path "Tools/ControlPointParent" must exist in a Resources folder for PhotonNetwork.Instantiate.
        GameObject parent = PhotonNetwork.Instantiate(
            "Tools/ControlPointParent",
            centerPosition,
            Quaternion.identity
        );

        // Store the reference locally
        _controlPointsParent = parent;

        // 4️⃣ Send the new parent's ViewID to all clients via RPC
        PhotonView parentView = parent.GetComponent<PhotonView>();
        if (parentView != null && _photonView != null)
        {
            // Use RpcTarget.AllBuffered so late-joining clients also receive the parent ID
            _photonView.RPC("RPC_CreateControlPointsParent", RpcTarget.AllBuffered, parentView.ViewID);
        }
        else
        {
            Debug.LogError("Failed to get PhotonView for either this object or the new parent.");
        }
    }

    /// <summary>
    /// Calculates the average position of a list of Transforms.
    /// </summary>
    private Vector3 CalculateCenterPosition(List<Transform> controlPoints)
    {
        if (controlPoints.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (Transform cp in controlPoints) sum += cp.position;
        return sum / controlPoints.Count;
    }
    #endregion

    #region Public Management Methods
    /// <summary>
    /// Gets the parent GameObject that contains all the organized control points.
    /// </summary>
    /// <returns>The control points parent GameObject or null.</returns>
    public GameObject GetControlPointsParent() => _controlPointsParent;

    /// <summary>
    /// Checks if the control points have been organized into a dedicated parent.
    /// </summary>
    /// <returns>True if the control points are organized, false otherwise.</returns>
    public bool IsOrganized() => _controlPointsParent != null;

    /// <summary>
    /// Resets the internal state, used for preparing the component for a new stroke.
    /// </summary>
    public void ResetStrokeState()
    {
        _strokeCompleted = false;
        _controlPointsParent = null;
    }
    #endregion

    #region Editor Visualization
    /// <summary>
    /// Draws a debug gizmo at the center of the control point parent.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showParentGizmo || _controlPointsParent == null) return;

        Gizmos.color = gizmoColor;
        // Draw a wire sphere to mark the center
        Gizmos.DrawWireSphere(_controlPointsParent.transform.position, gizmoSize);
        // Draw a line sticking up for better visibility
        Gizmos.DrawLine(
            _controlPointsParent.transform.position,
            _controlPointsParent.transform.position + Vector3.up * gizmoSize * 2
        );
    }
    #endregion

    #region RPC Methods
    /// <summary>
    /// RPC executed on all clients to find the newly instantiated parent object,
    /// reparent it under the BrushStrokeMesh, and move the individual control
    /// points under the new parent.
    /// </summary>
    /// <param name="viewID">The PhotonViewID of the newly created control point parent.</param>
    [PunRPC]
    private void RPC_CreateControlPointsParent(int viewID)
    {
        // Find the GameObject via its PhotonViewID
        PhotonView pv = PhotonView.Find(viewID);
        if (pv == null)
        {
            Debug.LogError($"RPC_CreateControlPointsParent: Could not find PhotonView with ID {viewID}");
            return;
        }

        GameObject obj = pv.gameObject;

        // Reparent the new parent object under this component's transform (BrushStrokeMesh)
        // 'true' for 'worldPositionStays' keeps the object at the calculated center
        obj.transform.SetParent(this.transform, true);

        // Store the reference on this client
        _controlPointsParent = obj;

        // 🔁 Reparent existing control points (which are currently children of this component)
        // Use ToArray() to avoid modifying the collection while iterating
        foreach (Transform child in transform.Cast<Transform>().ToArray())
        {
            if (child.name.StartsWith("ControlPoint_"))
                // Set the new parent, keeping the world position/rotation intact (worldPositionStays: true)
                child.SetParent(obj.transform, true);
        }

        #region Add Interaction Components (Local Only)

        // 🧱 Add Rigidbody (Network Safe) - required for XRGrabInteractable
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = obj.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true; // Use kinematic body for networked VR interaction

        // 🤲 Add XRGrabNetworkInteractable
        XRGrabNetworkInteractable grab = obj.GetComponent<XRGrabNetworkInteractable>();
        if (grab == null)
            grab = obj.AddComponent<XRGrabNetworkInteractable>();

        // 🎯 Make sure the grab interactable registers all child colliders (from the spheres)
        // Note: The colliders on the individual control point spheres are what get grabbed/hit
        Collider[] childColliders = obj.GetComponentsInChildren<Collider>();
        grab.colliders.Clear();

        foreach (Collider col in childColliders)
        {
            // Only add colliders if they are not already present (to prevent duplicates)
            if (!grab.colliders.Contains(col))
                grab.colliders.Add(col);
        }

        grab.throwOnDetach = false; // Optional: prevent throwing for precise drawing

        // Notify any listeners that the hierarchy is finalized
        StentErasor stentErasor = _controlPointsParent.GetComponent<StentErasor>();
        stentErasor.SetRootReference(this.transform.parent.gameObject);
        #endregion

        // If model not found, try SelectionManager as fallback
        ModelObject selectedModelObject = SelectionManager.Instance.GetSelectedModel();

        if (selectedModelObject != null)
        {
            obj.GetComponent<XRGrabNetworkInteractable>().enabled = false;
            _controlPointsParent.transform.parent = selectedModelObject.transform;
        }
    }
    
    #endregion
}