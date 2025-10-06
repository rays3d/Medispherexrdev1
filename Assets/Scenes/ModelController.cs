using UnityEngine;
using Photon.Pun;

public class ModelController : MonoBehaviourPun
{
    private void Start()
    {
        // Ensure PhotonView is set up correctly
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not attached to this GameObject.");
            return;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return; // Only allow the owner to control the model

        // Basic movement input for the model
        float moveSpeed = 5f;
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        transform.Translate(moveDirection);
    }

    // Optional RPC to sync position and rotation with other players
    [PunRPC]
    private void SyncModelPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}
