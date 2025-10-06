/*using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelObject : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed;
   // public InputActionProperty deleteButton;

    public PhotonView photonView;
    public virtual void OnGrabbed()
    {
        isGrabbed = true;
        SelectionManager.Instance.SelectModel(this);
        photonView.RPC(nameof(SelectModel), RpcTarget.Others);
        XRRightHandController.Instance.SetGrabbedItem(this.gameObject);
        XRLeftHandController.Instance.SetGrabbedItem(this.gameObject);
    }

    [PunRPC]
    public void SelectModel()
    {
        SelectionManager.Instance.SelectModel(this);
    }
    public virtual void OnGrabReleased()
    {
        isGrabbed = false;
        XRRightHandController.Instance.SetGrabbedItem(null);
        XRLeftHandController.Instance.SetGrabbedItem(null);
    }
    public void DestroyOverNetwork()
    {
        photonView.RPC(nameof(DestroyInAll), RpcTarget.All);
    }

    public virtual List<Material> GetMaterial()
    {
        List<Material> materials = new List<Material>();
        if (gameObject.TryGetComponent(out Renderer renderer))
        {
            materials.Add(renderer.material);
            return materials;
        }
        else
        {
            Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();

            foreach (var rend in rends)
            {
                materials.Add(rend.material);
            }
            return materials;
        }

    }
 

    [PunRPC]
    void DestroyInAll()
    {
        Destroy(gameObject);
    }
}
*/
/*using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ModelObject : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed;
    public PhotonView photonView; // PhotonView reference

    private void Awake()
    {
        // Ensure photonView is assigned, especially for API-loaded models
        if (photonView == null)
        {
            photonView = GetComponent<PhotonView>();
        }
    }

    public virtual void OnGrabbed()
    {
        isGrabbed = true;

        // Check if the photonView is available and ownership is correct
        if (photonView != null && !photonView.IsMine)
        {
            photonView.RequestOwnership(); // Request ownership of the PhotonView
        }

        // Log grab action for debugging
        Debug.Log("Model grabbed: " + gameObject.name);

        // Select the model locally and update the SelectionManager
        SelectionManager.Instance.SelectModel(this);

        // If this model is networked, notify others
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(SelectModel), RpcTarget.Others);
        }

        // Handle XR controller interactions
        XRRightHandController.Instance.SetGrabbedItem(this.gameObject);
        XRLeftHandController.Instance.SetGrabbedItem(this.gameObject);
    }

    [PunRPC]
    public void SelectModel()
    {
        // Log selection event for debugging
        Debug.Log("Model selected: " + gameObject.name);

        // Select the model locally and update the SelectionManager
        SelectionManager.Instance.SelectModel(this);
    }

    public virtual void OnGrabReleased()
    {
        isGrabbed = false;

        // Log release action for debugging
        Debug.Log("Model released: " + gameObject.name);

        // Reset XR controller interactions
        XRRightHandController.Instance.SetGrabbedItem(null);
        XRLeftHandController.Instance.SetGrabbedItem(null);
    }

    public void DestroyOverNetwork()
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(DestroyInAll), RpcTarget.All);
        }
    }

    public virtual List<Material> GetMaterial()
    {
        List<Material> materials = new List<Material>();

        if (gameObject.TryGetComponent(out Renderer renderer))
        {
            materials.Add(renderer.material);
        }
        else
        {
            Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var rend in rends)
            {
                materials.Add(rend.material);
            }
        }

        return materials;
    }

    [PunRPC]
    void DestroyInAll()
    {
        // Log destruction event for debugging
        Debug.Log("Model destroyed across network: " + gameObject.name);
        Destroy(gameObject);
    }
}
*/
///////////////////////////this is new to keep color state same
/*using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelObject : MonoBehaviourPunCallbacks
{
    [HideInInspector] public bool isGrabbed;
    public PhotonView photonView;

  public virtual void OnGrabbed()
    {
        // Check if the current client is the master client
        if (PhotonNetwork.IsMasterClient)
        {
            isGrabbed = true;
            SelectionManager.Instance.SelectModel(this);
            photonView.RPC(nameof(SelectModel), RpcTarget.Others);
            XRRightHandController.Instance.SetGrabbedItem(this.gameObject);
            XRLeftHandController.Instance.SetGrabbedItem(this.gameObject);
        }
        else
        {
            // Optionally, you can show a message to non-master clients
            Debug.Log("Only the master client can grab this model.");
        }
    }


    [PunRPC]
    public void SelectModel()
    {
        SelectionManager.Instance.SelectModel(this);
    }

    public virtual void OnGrabReleased()
    {
        isGrabbed = false;
        XRRightHandController.Instance.SetGrabbedItem(null);
        XRLeftHandController.Instance.SetGrabbedItem(null);
    }

    public void DestroyOverNetwork()
    {
        photonView.RPC(nameof(DestroyInAll), RpcTarget.All);
    }

    public virtual List<Material> GetMaterial()
    {
        List<Material> materials = new List<Material>();
        if (gameObject.TryGetComponent(out Renderer renderer))
        {
            materials.Add(renderer.material);
            return materials;
        }
        else
        {
            Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var rend in rends)
            {
                materials.Add(rend.material);
            }
            return materials;
        }
    }

    [PunRPC]
    void DestroyInAll()
    {
        Destroy(gameObject);
    }
}


*/





/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////this is original from old
/*
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ModelObject : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed;
    public PhotonView photonView; // PhotonView reference


    [HideInInspector] public Vector3 instantiationPosition; // Store position at instantiation

    [HideInInspector] public Quaternion instantiationRotation; // Store rotation at instantiation
    [HideInInspector] public Vector3 instantiationScale;

    private void Awake()
    {
        // Ensure photonView is assigned, especially for API-loaded models
        if (photonView == null)
        {
            photonView = GetComponent<PhotonView>();
        }

        instantiationPosition = transform.position;

        instantiationRotation = transform.rotation;

        instantiationScale = transform.localScale;

        Debug.Log($"ModelObject {gameObject.name} instantiated at position: {instantiationPosition}, rotation: {instantiationRotation}, scale: {instantiationScale}");
    }

    public virtual void OnGrabbed()
    {
        isGrabbed = true;

        // Check if the photonView is available and ownership is correct
        if (photonView != null && !photonView.IsMine)
        {
            photonView.RequestOwnership(); // Request ownership of the PhotonView
        }

        // Log grab action for debugging
        Debug.Log("Model grabbed: " + gameObject.name);

        // Select the model locally and update the SelectionManager
        SelectionManager.Instance.SelectModel(this);

        // If this model is networked, notify others
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(SelectModel), RpcTarget.Others);
        }

        // Handle XR controller interactions
        XRRightHandController.Instance.SetGrabbedItem(this.gameObject);
        XRLeftHandController.Instance.SetGrabbedItem(this.gameObject);
    }

    [PunRPC]
    public void SelectModel()
    {
        // Log selection event for debugging
        Debug.Log("Model selected: " + gameObject.name);

        // Select the model locally and update the SelectionManager
        SelectionManager.Instance.SelectModel(this);
    }

    public virtual void OnGrabReleased()
    {
        isGrabbed = false;

        // Log release action for debugging
        Debug.Log("Model released: " + gameObject.name);

        // Reset XR controller interactions
        XRRightHandController.Instance.SetGrabbedItem(null);
        XRLeftHandController.Instance.SetGrabbedItem(null);
    }

    public void DestroyOverNetwork()
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(DestroyInAll), RpcTarget.All);
        }
    }

    public virtual List<Material> GetMaterial()
    {
        List<Material> materials = new List<Material>();

        if (gameObject.TryGetComponent(out Renderer renderer))
        {
            materials.Add(renderer.material);
        }
        else
        {
            Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var rend in rends)
            {
                materials.Add(rend.material);
            }
        }

        return materials;
    }

    [PunRPC]
    void DestroyInAll()
    {
        // Log destruction event for debugging
        Debug.Log("Model destroyed across network: " + gameObject.name);
        Destroy(gameObject);

    }
}*/
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///



using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ModelObject : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed;
    public PhotonView photonView; // PhotonView reference
    [HideInInspector] public Vector3 instantiationPosition; // Store position at instantiation
    [HideInInspector] public Quaternion instantiationRotation; // Store rotation at instantiation
    [HideInInspector] public Vector3 instantiationScale;

    private void Awake()
    {
        if (photonView == null)
        {
            photonView = GetComponent<PhotonView>();
            if (photonView == null)
            {
                Debug.LogError($"[ModelObject] {gameObject.name} is missing PhotonView component!");
            }
        }
        instantiationPosition = transform.position;
        instantiationRotation = transform.rotation;
        instantiationScale = transform.localScale;
        Debug.Log($"[ModelObject] {gameObject.name} instantiated at position: {instantiationPosition}, rotation: {instantiationRotation}, scale: {instantiationScale}");
    }

    public virtual void OnGrabbed()
    {
        Debug.Log($"[ModelObject] OnGrabbed called for {gameObject.name}, IsMasterClient={PhotonNetwork.IsMasterClient}, PhotonView.IsMine={photonView?.IsMine}");

        // Check if grabbing is allowed based on masterClientOnly
        var leftController = XRLeftHandController.Instance;
        var rightController = XRRightHandController.Instance;
        if ((leftController.masterClientOnly || rightController.masterClientOnly) && !PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning($"[ModelObject] {gameObject.name} grab attempt blocked: masterClientOnly is true for non-master client.");
            return;
        }

        isGrabbed = true;
        if (photonView != null && !photonView.IsMine)
        {
            photonView.RequestOwnership();
            Debug.Log($"[ModelObject] {gameObject.name} requested ownership for grab.");
        }

        // Select the model locally
        SelectionManager.Instance.SelectModel(this);
        Debug.Log($"[ModelObject] {gameObject.name} selected locally.");

        // Sync grab state to all clients
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(SyncGrabbedState), RpcTarget.All, true);
        }
    }

    [PunRPC]
    public void SyncGrabbedState(bool grabbed)
    {
        isGrabbed = grabbed;
        Debug.Log($"[ModelObject] SyncGrabbedState for {gameObject.name}: isGrabbed={grabbed}, IsMasterClient={PhotonNetwork.IsMasterClient}");

        if (grabbed)
        {
            SelectionManager.Instance.SelectModel(this);
            // Only set grabbed item on the client that initiated the grab
            if (PhotonNetwork.IsMasterClient || (!XRLeftHandController.Instance.masterClientOnly && !XRRightHandController.Instance.masterClientOnly))
            {
                XRRightHandController.Instance.SetGrabbedItem(gameObject);
                XRLeftHandController.Instance.SetGrabbedItem(gameObject);
            }
        }
        else
        {
            XRRightHandController.Instance.SetGrabbedItem(null);
            XRLeftHandController.Instance.SetGrabbedItem(null);
        }
    }

    public virtual void OnGrabReleased()
    {
        Debug.Log($"[ModelObject] OnGrabReleased called for {gameObject.name}, IsMasterClient={PhotonNetwork.IsMasterClient}");
        isGrabbed = false;
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(SyncGrabbedState), RpcTarget.All, false);
        }
    }

    public void DestroyOverNetwork()
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(DestroyInAll), RpcTarget.All);
        }
    }

    public virtual List<Material> GetMaterial()
    {
        List<Material> materials = new List<Material>();
        if (gameObject.TryGetComponent(out Renderer renderer))
        {
            materials.Add(renderer.material);
        }
        else
        {
            Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var rend in rends)
            {
                materials.Add(rend.material);
            }
        }
        return materials;
    }

    [PunRPC]
    void DestroyInAll()
    {
        Debug.Log($"[ModelObject] Destroyed across network: {gameObject.name}");
        Destroy(gameObject);
    }
}
