using Photon.Pun;
using UnityEngine;

public class StentErasor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Eraser"))
        {
            Debug.Log("⚠ Eraser collided with stent part: " + other.gameObject.name);

            GameObject root = GetRootWithPhotonView(transform.gameObject);
            PhotonView pv = root.GetComponent<PhotonView>();

            if (pv != null && pv.IsMine)
            {
                PhotonNetwork.Destroy(root);
            }
            else if (pv != null) // Request owner to destroy
            {
                pv.RPC("RPC_DestroyStent", RpcTarget.AllBuffered);
            }
        }
    }

    // Finds top object that has the PhotonView
    private GameObject GetRootWithPhotonView(GameObject obj)
    {
        Transform current = obj.transform;
        while (current.parent != null)
            current = current.parent;
        return current.gameObject;
    }

    [PunRPC]
    private void RPC_DestroyStent()
    {
        GameObject root = GetRootWithPhotonView(gameObject);
        if (root != null)
            Destroy(root);
    }
}
