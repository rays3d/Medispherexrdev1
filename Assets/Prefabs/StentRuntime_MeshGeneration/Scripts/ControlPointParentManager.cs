using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

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

    public void OnStrokeCompleted()
    {
        if (_strokeCompleted) return;

        if (createParentOnStrokeEnd)
        {
            OrganizeControlPoints();
        }

        _strokeCompleted = true;
    }

    public void OrganizeControlPoints()
    {
        List<Transform> controlPoints = new List<Transform>();

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("ControlPoint_"))
            {
                controlPoints.Add(child);
            }
        }

        if (controlPoints.Count == 0) return;

        Vector3 centerPosition = CalculateCenterPosition(controlPoints);

        _controlPointsParent = new GameObject(parentObjectName);
        _controlPointsParent.transform.position = centerPosition;
        _controlPointsParent.transform.rotation = Quaternion.identity;
        _controlPointsParent.transform.parent = transform;

        foreach (Transform cp in controlPoints)
        {
            cp.SetParent(_controlPointsParent.transform, true);
        }

        AddNetworkComponents(_controlPointsParent);   // ✅ Network + Grab setup
    }

    /// <summary>
    /// Add Collider, Rigidbody, Grab & Photon
    /// </summary>
    private void AddNetworkComponents(GameObject obj)
    {
        // ✅ PHOTON VIEW
        PhotonView pv = obj.AddComponent<PhotonView>();
        pv.ObservedComponents = new System.Collections.Generic.List<Component>();

        // ✅ TRANSFORM SYNC
        PhotonTransformView ptv = obj.AddComponent<PhotonTransformView>();
        pv.ObservedComponents.Add(ptv);
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;

        // ✅ RIGIDBODY SETUP
        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;

        // ✅ XR GRAB INTERACTABLE
        XRGrabInteractable grab = obj.AddComponent<XRGrabInteractable>();
        grab.trackPosition = true;
        grab.trackRotation = true;
        grab.throwOnDetach = false;
        grab.interactionLayers = InteractionLayerMask.GetMask("Default");

        // ✅ Ownership Transfer on Grab
        grab.selectEntered.AddListener((args) =>
        {
            if (pv && !pv.IsMine) pv.RequestOwnership();
        });

        // Attach Point for stable grabbing
        Transform attachPoint = new GameObject("AttachPoint").transform;
        attachPoint.SetParent(obj.transform, false);
        attachPoint.localPosition = Vector3.zero;
        grab.attachTransform = attachPoint;

        // ✅ Multiplayer Delete
        obj.AddComponent<Deletable>();
    }

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

    public GameObject GetControlPointsParent()
    {
        return _controlPointsParent;
    }

    public bool IsOrganized()
    {
        return _controlPointsParent != null;
    }

    public void ResetStrokeState()
    {
        _strokeCompleted = false;
        _controlPointsParent = null;
    }

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
