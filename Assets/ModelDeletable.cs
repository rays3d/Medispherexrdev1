/*using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class ModelDeletable : MonoBehaviour
{
    private PhotonView photonView;

    // Reference to the delete button action property
    public InputActionProperty deleteButton;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing!");
        }

        // Subscribe to the performed event using InputActionProperty
        deleteButton.action.performed += OnDeleteButtonPressed;

        // Enable the action
        deleteButton.action.Enable();
    }

    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        deleteButton.action.performed -= OnDeleteButtonPressed;
    }

    private void OnDeleteButtonPressed(InputAction.CallbackContext context)
    {
        // Call the DeleteModel method when the delete button is pressed
        DeleteModel();
    }

    public void DeleteModel()
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
        }
    }

    [PunRPC]
    private void DestroyOverNetwork()
    {
        Destroy(gameObject);
    }
}
*/






/*using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelDeletable : MonoBehaviour
{
    private PhotonView photonView;
    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();
    // Reference to the delete button action property, assigned to the right controller
    public InputActionProperty deleteButton;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))

        {

            Destroy(gameObject);

            return;

        }
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing!");
        }

        // Subscribe to the performed event using InputActionProperty for the right controller
        deleteButton.action.performed += OnDeleteButtonPressed;

        // Enable the action
        deleteButton.action.Enable();
    }

    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        deleteButton.action.performed -= OnDeleteButtonPressed;
    }

    private void OnDeleteButtonPressed(InputAction.CallbackContext context)
    {
        // Ensure it's only from the right controller by checking the action path (optional)
        if (IsRightController(context))
        {
            // Call the DeleteModel method when the delete button is pressed
            DeleteModel();
            destroyedObjectIDs.Add(photonView.ViewID.ToString());
            photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
        }
    }

    private bool IsRightController(InputAction.CallbackContext context)
    {
        // Optionally check the control path to confirm the input is from the right controller
        return context.control.device.name.Contains("RightHand");
    }

    public void DeleteModel()
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
        }
    }

    [PunRPC]
    private void DestroyOverNetwork()
    {
        Destroy(gameObject);
        destroyedObjectIDs.Add(photonView.ViewID.ToString());
    }
    private void OnApplicationQuit()

    {

        destroyedObjectIDs.Clear();

    }
}*/
////////////////////////////////////////////////////////////////above is correct which i am using
/*using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelDeletable : MonoBehaviour
{
    private PhotonView photonView;
    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();

    // Reference to the delete button action property, assigned to the right controller
    public InputActionProperty deleteButton;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))
        {
            Destroy(gameObject);
            return;
        }

        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing!");
        }

        // Subscribe to the performed event using InputActionProperty for the right controller
        deleteButton.action.performed += OnDeleteButtonPressed;
        // Enable the action
        deleteButton.action.Enable();
    }

    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        deleteButton.action.performed -= OnDeleteButtonPressed;
    }

    private void OnDeleteButtonPressed(InputAction.CallbackContext context)
    {
        // Ensure it's only from the right controller by checking the action path (optional)
        if (IsRightController(context))
        {
            // Call the DeleteModel method when the delete button is pressed
            DeleteModel();
        }
    }

    private bool IsRightController(InputAction.CallbackContext context)
    {
        // Optionally check the control path to confirm the input is from the right controller
        return context.control.device.name.Contains("RightHand");
    }

    public void DeleteModel()
    {
        if (photonView != null && photonView.IsMine)
        {
            string modelID = photonView.ViewID.ToString();

            // Add to destroyed objects list before RPC
            destroyedObjectIDs.Add(modelID);

            // Notify all measurements that this model is being deleted
            MeasurmentInk.OnModelDeleted(modelID);

            // Send RPC to all clients to destroy the model
            photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void DestroyOverNetwork()
    {
        string modelID = photonView.ViewID.ToString();

        // Add to destroyed objects list
        destroyedObjectIDs.Add(modelID);

        // Notify all measurements on this client that this model is being deleted
        MeasurmentInk.OnModelDeleted(modelID);

        // Destroy the model
        Destroy(gameObject);
    }

    private void OnApplicationQuit()
    {
        destroyedObjectIDs.Clear();
    }

    // Method to check if a model was deleted (for late joiners)
    public static bool IsModelDeleted(string modelID)
    {
        return destroyedObjectIDs.Contains(modelID);
    }

    // Method to get all deleted model IDs (useful for synchronization)
    public static HashSet<string> GetDeletedModelIDs()
    {
        return new HashSet<string>(destroyedObjectIDs);
    }
}*/


/*using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ModelDeletable : MonoBehaviour
{
    private PhotonView photonView;
    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();
    // Reference to the delete button action property, assigned to the right controller
    public InputActionProperty deleteButton;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))
        {
            Destroy(gameObject);
            return;
        }
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing!");
        }
        // Subscribe to the performed event using InputActionProperty for the right controller
        deleteButton.action.performed += OnDeleteButtonPressed;
        // Enable the action
        deleteButton.action.Enable();
    }
    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        deleteButton.action.performed -= OnDeleteButtonPressed;
    }
    private void OnDeleteButtonPressed(InputAction.CallbackContext context)
    {
        // Ensure it's only from the right controller by checking the action path (optional)
        if (IsRightController(context))
        {
            // Call the DeleteModel method when the delete button is pressed
            DeleteModel();
        }
    }
    private bool IsRightController(InputAction.CallbackContext context)
    {
        // Optionally check the control path to confirm the input is from the right controller
        return context.control.device.name.Contains("RightHand");
    }
    public void DeleteModel()
    {
        if (photonView != null && photonView.IsMine)
        {
            string modelID = photonView.ViewID.ToString();
            // Add to destroyed objects list before RPC
            destroyedObjectIDs.Add(modelID);
            // Notify all measurements that this model is being deleted
            MeasurmentInk.OnModelDeleted(modelID);
            // Send RPC to all clients to destroy the model
            photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    private void DestroyOverNetwork()
    {
        string modelID = photonView.ViewID.ToString();
        // Add to destroyed objects list
        destroyedObjectIDs.Add(modelID);
        // Notify all measurements on this client that this model is being deleted
        MeasurmentInk.OnModelDeleted(modelID);
        // Destroy the model
        Destroy(gameObject);
    }
    private void OnApplicationQuit()
    {
        destroyedObjectIDs.Clear();
    }
    // Method to check if a model was deleted (for late joiners)
    public static bool IsModelDeleted(string modelID)
    {
        return destroyedObjectIDs.Contains(modelID);
    }
    // Method to get all deleted model IDs (useful for synchronization)
    public static HashSet<string> GetDeletedModelIDs()
    {
        return new HashSet<string>(destroyedObjectIDs);
    }
}*/

using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelDeletable : MonoBehaviour
{
    private PhotonView photonView;
    public InputActionProperty deleteButton;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        string modelID = photonView.ViewID.ToString();

        if (DeletionSyncUtility.IsModelDeletedGlobally(modelID))
        {
            Destroy(gameObject);
            return;
        }

        if (photonView == null)
        {
            Debug.LogError("PhotonView is missing on " + gameObject.name);
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
        if (context.control.device.name.Contains("RightHand"))
        {
            DeleteModel();
        }
    }

    public void DeleteModel()
    {
        if (photonView != null && photonView.IsMine)
        {
            string modelID = photonView.ViewID.ToString();
            DeletionSyncUtility.AddDeletedModelID(modelID);
            MeasurmentInk.OnModelDeleted(modelID);
            photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void DestroyOverNetwork()
    {
        string modelID = photonView.ViewID.ToString();
        DeletionSyncUtility.AddDeletedModelID(modelID);
        MeasurmentInk.OnModelDeleted(modelID);
        Destroy(gameObject);
    }
}

