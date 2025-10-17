using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
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

    PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

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
                controlPoints.Add(child);
        }

        if (controlPoints.Count == 0) return;

        // Calculate center position
        Vector3 centerPosition = CalculateCenterPosition(controlPoints);

        // Call RPC to create parent and reparent on all clients
        _photonView = GetComponent<PhotonView>();
        if (_photonView != null)
        {
            _photonView.RPC("RPC_CreateControlPointsParent", RpcTarget.AllBuffered, centerPosition);
        }
    }

    [PunRPC]
    private void RPC_CreateControlPointsParent(Vector3 position)
    {
        if (_controlPointsParent != null) return;

        // ✅ Spawn over network
        GameObject parent = PhotonNetwork.Instantiate(
            "Tools/ControlPointParent",
            position,
            Quaternion.identity
        );
        
        _controlPointsParent = parent;

        // Reparent existing control points
        foreach (Transform child in transform.Cast<Transform>().ToArray())
        {
            if (child.name.StartsWith("ControlPoint_"))
                child.SetParent(_controlPointsParent.transform, true);
        }
        AddGrabComponents(parent);
    }

    private void AddGrabComponents(GameObject obj)
    {
        if (obj.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        var grab = obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
        if (grab == null)
        {
            grab = obj.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            grab.trackPosition = true;

            Transform attachPoint = new GameObject("AttachPoint").transform;
            attachPoint.SetParent(obj.transform, false);
            attachPoint.localPosition = Vector3.zero;
            attachPoint.localRotation = Quaternion.identity;
            grab.attachTransform = attachPoint;

            obj.AddComponent<Deletable>();
        }
    }

    private Vector3 CalculateCenterPosition(List<Transform> controlPoints)
    {
        if (controlPoints.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (Transform cp in controlPoints) sum += cp.position;
        return sum / controlPoints.Count;
    }

    public GameObject GetControlPointsParent() => _controlPointsParent;

    public bool IsOrganized() => _controlPointsParent != null;

    public void ResetStrokeState()
    {
        _strokeCompleted = false;
        _controlPointsParent = null;
    }

    public void ManualOrganize() => OrganizeControlPoints();

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
