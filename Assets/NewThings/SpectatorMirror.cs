using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpectatorMirror : MonoBehaviourPunCallbacks
{
    [Header("Follow Settings")]
    public float smoothSpeed = 5f;        // Smooth following
    public Transform masterCameraTransform; // Assign dynamically

    void LateUpdate()
    {
        if (masterCameraTransform == null) return;

        // Mirror master camera position and rotation
        transform.position = Vector3.Lerp(transform.position, masterCameraTransform.position, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, masterCameraTransform.rotation, Time.deltaTime * smoothSpeed);
    }

    // Called automatically when master client changes
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Spectator: Master changed, reassigning camera.");
        masterCameraTransform = null;
        InvokeRepeating(nameof(FindMasterCamera), 0.5f, 1f);
    }

    void Start()
    {
        InvokeRepeating(nameof(FindMasterCamera), 1f, 2f);
    }

    void FindMasterCamera()
    {
        foreach (var view in FindObjectsOfType<PhotonView>())
        {
            if (view.Owner != null && view.Owner.IsMasterClient)
            {
                var cam = view.GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    masterCameraTransform = cam.transform;
                    Debug.Log("Spectator: Found master camera " + cam.name);
                    CancelInvoke(nameof(FindMasterCamera));
                    break;
                }
            }
        }
    }
}
