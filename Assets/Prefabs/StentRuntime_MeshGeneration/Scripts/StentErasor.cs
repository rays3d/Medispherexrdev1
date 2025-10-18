using Photon.Pun;
using UnityEngine;

public class StentErasor : MonoBehaviour
{

private PhotonView rootPV;
public GameObject rootReference { get; private set; }

// Call this from another script after parenting is finalized
    public void SetRootReference(GameObject root)
    {
        if (root == null) return;

        rootReference = root;
        rootPV = root.GetComponent<PhotonView>();

        if (rootPV == null)
            Debug.LogWarning("⚠ Assigned root has no PhotonView! Deletion may not sync over network.");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Eraser"))
        {
            if (rootReference != null)
            {
                if (rootPV != null && rootPV.IsMine)
                    PhotonNetwork.Destroy(rootReference);
                else if (rootPV != null)
                    rootPV.RPC(nameof(RPC_DestroyStent), RpcTarget.AllBuffered);
            }

            Destroy(gameObject); // Destroy the eraser locally
        }
    }

    [PunRPC]
    private void RPC_DestroyStent()
    {
        if (rootReference != null)
            Destroy(rootReference);
    }

    private void OnDestroy()
{
    // Just in case the root still exists locally, destroy it (owner only)
    if (rootPV != null && rootPV.IsMine && rootReference != null)
    {
        PhotonNetwork.Destroy(rootReference);
    }

    // Clear cached references
    rootReference = null;
    rootPV = null;
}

}
