using System.Collections.Generic;
using UnityEngine;

public class ControlPointParentManager : MonoBehaviour
{
    [Header("Parent Object Settings")]
    [SerializeField] private string parentObjectName = "ControlPointsParent";
    [SerializeField] private bool createParentOnStrokeEnd = true;
    [SerializeField] private bool showParentGizmo = true;
    [SerializeField] private Color gizmoColor = Color.yellow;
    [SerializeField] private float gizmoSize = 0.02f;

    private GameObject _controlPointsParent = null;
    private bool _strokeCompleted = false;

    /// <summary>
    /// Call this when the stroke is completed to organize control points
    /// </summary>
    public void OnStrokeCompleted()
    {
        if (_strokeCompleted) return;
        
        if (createParentOnStrokeEnd)
        {
            OrganizeControlPoints();
        }
        
        _strokeCompleted = true;
    }

    /// <summary>
    /// Organize all control point spheres under a parent object centered between them
    /// </summary>
    public void OrganizeControlPoints()
    {
        // Find all control point objects (assuming they start with "ControlPoint_")
        List<Transform> controlPoints = new List<Transform>();
        
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("ControlPoint_"))
            {
                controlPoints.Add(child);
            }
        }

        if (controlPoints.Count == 0) return;

        // Calculate center position of all control points
        Vector3 centerPosition = CalculateCenterPosition(controlPoints);

        // Create parent object at center
        _controlPointsParent = new GameObject(parentObjectName);
        _controlPointsParent.transform.position = centerPosition;
        _controlPointsParent.transform.rotation = Quaternion.identity;
        _controlPointsParent.transform.parent = transform;

        // Reparent all control points to the new parent
        foreach (Transform cp in controlPoints)
        {
            cp.SetParent(_controlPointsParent.transform, true);
        }

        //  Add grab functionality in separate method
        AddGrabComponents(_controlPointsParent);
    }



        /// <summary>
        /// Add Collider, Rigidbody and XR Grab to the object
        /// </summary>
    private void AddGrabComponents(GameObject obj)
{
    //  Capsule Collider
    //Collider capCol = obj.AddComponent<MeshCollider>();
    // capCol.center = Vector3.zero;
    // capCol.radius = 0.02f;
    // capCol.height = 0.1f;

    //  Rigidbody
    Rigidbody rb = obj.AddComponent<Rigidbody>();
    rb.useGravity = false;
    rb.isKinematic = true;   //  Object will NOT move on collisions
    // ❗ Don't freeze rotation → allow manual rotation

    //  XR Grab Interactable
    var grab = obj.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
    grab.trackPosition = true;
   // grab.trackRotation = false;   //  No snapping to controller
    //grab.throwOnDetach = false;

    // 🔒 Fixed Attach Point - Prevent snap rotation
    Transform attachPoint = new GameObject("AttachPoint").transform;
    attachPoint.SetParent(obj.transform, false);
    attachPoint.localPosition = Vector3.zero;
    attachPoint.localRotation = Quaternion.identity;
        grab.attachTransform = attachPoint;

        //Delete functionality
        var deletable = obj.AddComponent<Deletable>();
}


    /// <summary>
    /// Calculate the center position of all control points
    /// </summary>
    private Vector3 CalculateCenterPosition(List<Transform> controlPoints)
    {
        if (controlPoints.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (Transform cp in controlPoints)
        {
            sum += cp.position;
        }

        return sum / controlPoints.Count;
    }

    /// <summary>
    /// Get the parent object containing all control points
    /// </summary>
    public GameObject GetControlPointsParent()
    {
        return _controlPointsParent;
    }

    /// <summary>
    /// Check if control points have been organized
    /// </summary>
    public bool IsOrganized()
    {
        return _controlPointsParent != null;
    }

    /// <summary>
    /// Reset the stroke completion state (useful for reusing the component)
    /// </summary>
    public void ResetStrokeState()
    {
        _strokeCompleted = false;
        _controlPointsParent = null;
    }

    /// <summary>
    /// Manually trigger organization of control points
    /// </summary>
    public void ManualOrganize()
    {
        OrganizeControlPoints();
    }

    private void OnDrawGizmos()
    {
        if (!showParentGizmo || _controlPointsParent == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(_controlPointsParent.transform.position, gizmoSize);
        Gizmos.DrawLine(
            _controlPointsParent.transform.position,
            _controlPointsParent.transform.position + Vector3.up * gizmoSize * 2
        );
    }
    
    
}