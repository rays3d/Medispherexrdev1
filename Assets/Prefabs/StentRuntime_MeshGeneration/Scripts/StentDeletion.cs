
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class StentDeletion : MonoBehaviour
{ private PhotonView photonView;
    public InputActionProperty deleteButton;

    private void Start()
    {
        // Find the PhotonView on the root parent
        GameObject rootObj = GetRootDeletableParent();
        photonView = rootObj.GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.LogError("PhotonView is missing on root object: " + rootObj.name);
            return;
        }

        string modelID = photonView.ViewID.ToString();
        if (DeletionSyncUtility.IsModelDeletedGlobally(modelID))
        {
            Destroy(rootObj);
            return;
        }

        deleteButton.action.performed += OnDeleteButtonPressed;
        deleteButton.action.Enable();
    }

    private void OnDestroy()
    {
        deleteButton.action.performed -= OnDeleteButtonPressed;
    }

    private void OnDeleteButtonPressed(InputAction.CallbackContext context)
    {
        // Only delete when right hand trigger/button is pressed
        if (context.control.device.name.Contains("RightHand"))
        {
            DeleteModel();
        }
    }

    public void DeleteModel()
    {
        GameObject rootObj = GetRootDeletableParent();
        PhotonView rootPV = rootObj.GetComponent<PhotonView>();

        if (rootPV != null && rootPV.IsMine)
        {
            string modelID = rootPV.ViewID.ToString();
            DeletionSyncUtility.AddDeletedModelID(modelID);
            MeasurmentInk.OnModelDeleted(modelID);

            // RPC to destroy on all clients
            rootPV.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void DestroyOverNetwork()
    {
        GameObject rootObj = GetRootDeletableParent();
        PhotonView rootPV = rootObj.GetComponent<PhotonView>();

        if (rootPV != null)
        {
            string modelID = rootPV.ViewID.ToString();
            DeletionSyncUtility.AddDeletedModelID(modelID);
            MeasurmentInk.OnModelDeleted(modelID);

            Destroy(rootObj);
        }
    }

    /// <summary>
    /// Finds the top-most parent that should be deleted.
    /// </summary>
    /// <returns>The root GameObject with a PhotonView</returns>
    private GameObject GetRootDeletableParent()
    {
        Transform current = transform;
        Transform lastPhotonView = null;

        // Traverse up the hierarchy until we find the first parent with a PhotonView
        while (current != null)
        {
            if (current.GetComponent<PhotonView>() != null)
                lastPhotonView = current;

            current = current.parent;
        }

        return lastPhotonView != null ? lastPhotonView.gameObject : gameObject;
    }
}
