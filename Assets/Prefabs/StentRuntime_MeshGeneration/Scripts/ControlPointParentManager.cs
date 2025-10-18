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
        // 1️⃣ Collect control points
        List<Transform> controlPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("ControlPoint_"))
                controlPoints.Add(child);
        }

        if (controlPoints.Count == 0) return;

        // 2️⃣ Calculate center
        Vector3 centerPosition = CalculateCenterPosition(controlPoints);

        // 3️⃣ Instantiate parent over network (only once - owner only)
        GameObject parent = PhotonNetwork.Instantiate(
            "Tools/ControlPointParent",
            centerPosition,
            Quaternion.identity
        );

        _controlPointsParent = parent;

        // 4️⃣ Send parent ViewID to all clients via RPC
        PhotonView parentView = parent.GetComponent<PhotonView>();
        PhotonView thisView = GetComponent<PhotonView>();

        thisView.RPC("RPC_CreateControlPointsParent", RpcTarget.AllBuffered, parentView.ViewID);

    }

    
[PunRPC]
private void RPC_CreateControlPointsParent(int viewID)
{
    PhotonView pv = PhotonView.Find(viewID);
    if (pv == null) return;

    GameObject obj = pv.gameObject;
    obj.transform.SetParent(this.transform, true);

        // 🔁 Reparent existing control points
        foreach (Transform child in transform.Cast<Transform>().ToArray())
        {
            if (child.name.StartsWith("ControlPoint_"))
                child.SetParent(obj.transform, true);
        }
    #region  Add Interaction

    // 🧱 Add Rigidbody (Network Safe)
    Rigidbody rb = obj.GetComponent<Rigidbody>();
    if (rb == null)
        rb = obj.AddComponent<Rigidbody>();
    rb.useGravity = false;
    rb.isKinematic = true;

    // 🤲 Add XRGrabInteractable
    XRGrabNetworkInteractable grab = obj.GetComponent<XRGrabNetworkInteractable>();
    if (grab == null)
        grab = obj.AddComponent<XRGrabNetworkInteractable>();

    // 🎯 Make sure it uses all child colliders
    Collider[] childColliders = obj.GetComponentsInChildren<Collider>();
    grab.colliders.Clear();

    foreach (Collider col in childColliders)
    {
        if (!grab.colliders.Contains(col))
            grab.colliders.Add(col);
    }

        grab.throwOnDetach = false; // Optional: prevent throwing
    #endregion
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
