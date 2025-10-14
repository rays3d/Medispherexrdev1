/*using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ModelPart : ModelObject, IColorChangable, IDeletable
{
    public int partID;
    public Color[] colors = new Color[8] { Color.white, Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.yellow, Color.cyan };
    public InputActionProperty changeColor;

    private int currentColorIndex;
    public InputActionProperty deleteButton;
    private void Start()
    {
        XRGrabNetworkInteractable xRGrabNetworkInteractable = GetComponent<XRGrabNetworkInteractable>();
        xRGrabNetworkInteractable.selectEntered.AddListener(OnSelect);
        xRGrabNetworkInteractable.selectExited.AddListener(OnExit);

        photonView = GetComponent<PhotonView>();

        changeColor.action.performed += (S) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };
        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                DeleteTool();
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);

            }
        };
    }
    [PunRPC]
    private void OnExit(SelectExitEventArgs arg0)
    {
        OnGrabReleased();
    }

    private void OnSelect(SelectEnterEventArgs arg0)
    {
        OnGrabbed();
    }

    public Color GetCurrentColor()
    {
        return colors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        throw new System.NotImplementedException();
    }

    [PunRPC]

    private void SwitchColor()
    {
        if (currentColorIndex == colors.Length - 1)
        {
            currentColorIndex = 0;
        }
        else
        {
            currentColorIndex++;
        }
        //GetComponent<Renderer>().material.color = colors[currentColorIndex];

        if (TryGetComponent(out Renderer rend))
        {
            // rend.material.color = colors[currentColorIndex];
            rend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
        else
        {
            if (transform.GetChild(0).TryGetComponent(out Renderer rendinChild))
            {
                rendinChild.material.SetColor("_BaseColor", colors[currentColorIndex]);
                // rendinChild.material.color = colors[currentColorIndex];
            }
        }
    }

    public void Reset()
    {
        photonView.RequestOwnership();

        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;

        float scale = transform.parent.localScale.x;
        transform.localScale = Vector3.one;// * scale;
    }

    public void DeleteTool()
    {
        HapticManager.Instance.ActivateHapticRight(.3f, .5f);
       // Destroy(gameObject);
    }
}

*/

///////////////////////////////////above is working properly////////////////
///



/*using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ModelPart : ModelObject, IColorChangable, IDeletable, IPunObservable
{
    public int partID;
    public Color[] colors = new Color[8] { Color.white, Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.yellow, Color.cyan };
    public InputActionProperty changeColor;
    [SerializeField]
    private int currentColorIndex;
    public InputActionProperty deleteButton;
    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();
    private void Start()
    {
        XRGrabNetworkInteractable xRGrabNetworkInteractable = GetComponent<XRGrabNetworkInteractable>();
        xRGrabNetworkInteractable.selectEntered.AddListener(OnSelect);
        xRGrabNetworkInteractable.selectExited.AddListener(OnExit);
        photonView = GetComponent<PhotonView>();
        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))

        {

            Destroy(gameObject);

            return;

        }
        // Apply current color on start (important for late joiners)
        ApplyCurrentColor();

        changeColor.action.performed += (S) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };

        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                DeleteTool();
                destroyedObjectIDs.Add(photonView.ViewID.ToString());
                // photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Implement OnPhotonSerializeView to sync color index
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(currentColorIndex);
        }
        else
        {
            // Network player, receive data
            currentColorIndex = (int)stream.ReceiveNext();
            ApplyCurrentColor();
        }
    }

    [PunRPC]
    private void OnExit(SelectExitEventArgs arg0)
    {
        OnGrabReleased();
    }

    private void OnSelect(SelectEnterEventArgs arg0)
    {
        OnGrabbed();
    }

    public Color GetCurrentColor()
    {
        return colors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        throw new System.NotImplementedException();
    }

    [PunRPC]
    private void SwitchColor()
    {
        if (currentColorIndex == colors.Length - 1)
        {
            currentColorIndex = 0;
        }
        else
        {
            currentColorIndex++;
        }
        ApplyCurrentColor();
    }

    private void ApplyCurrentColor()
    {
        if (TryGetComponent(out Renderer rend))
        {
            rend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
        else
        {
            if (transform.GetChild(0).TryGetComponent(out Renderer rendinChild))
            {
                rendinChild.material.SetColor("_BaseColor", colors[currentColorIndex]);
            }
        }
    }

    public void Reset()
    {
        photonView.RequestOwnership();
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        float scale = transform.parent.localScale.x;
        transform.localScale = Vector3.one;
    }

    public void DeleteTool()
    {
        HapticManager.Instance.ActivateHapticRight(.3f, .5f);
    }

}*/


/*
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ModelPart : ModelObject, IColorChangable, IDeletable, IPunObservable
{
    public int partID;
    public Color[] colors = new Color[8] { Color.white, Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.yellow, Color.cyan };

    public InputActionProperty changeColor;
    public InputActionProperty deleteButton;

    [SerializeField]
    private int currentColorIndex;

    private PhotonView photonView;

    private bool isGrabbed;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        string modelID = photonView.ViewID.ToString();

        // Destroy if already deleted globally
        if (DeletionSyncUtility.IsModelDeletedGlobally(modelID))
        {
            Destroy(gameObject);
            return;
        }

        ApplyCurrentColor();

        // Grab interaction setup
        var grab = GetComponent<XRGrabNetworkInteractable>();
        grab.selectEntered.AddListener(OnSelect);
        grab.selectExited.AddListener(OnExit);

        // Change color handler
        changeColor.action.performed += (ctx) =>
        {
            if (isGrabbed && photonView.IsMine)
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
        };

        // Delete handler
        deleteButton.action.performed += (ctx) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                string id = photonView.ViewID.ToString();
                DeletionSyncUtility.AddDeletedModelID(id);
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };
    }

    [PunRPC]
    private void DestroyOverNetwork()
    {
        string modelID = photonView.ViewID.ToString();
        DeletionSyncUtility.AddDeletedModelID(modelID);
        DeleteTool();
        Destroy(gameObject);
    }

    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        ApplyCurrentColor();
    }

    private void ApplyCurrentColor()
    {
        if (TryGetComponent(out Renderer rend))
        {
            rend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
        else if (transform.childCount > 0 && transform.GetChild(0).TryGetComponent(out Renderer childRend))
        {
            childRend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
    }

    private void OnSelect(SelectEnterEventArgs args) => isGrabbed = true;
     private void OnExit(SelectExitEventArgs args) => isGrabbed = false;

    *//*    private void OnSelect(SelectEnterEventArgs args)
        {
            isGrabbed = true;
            SelectionManager.Instance?.SelectModel(this); // <- This is enough
        }

        private void OnExit(SelectExitEventArgs args)
        {
            isGrabbed = false;
            SelectionManager.Instance?.ClearSelection(); // <- Clears the selected model
        }
    *//*

    // In ModelPart.cs



    public Color GetCurrentColor()
    {
        return colors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        // Not used but required by interface
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        photonView.RequestOwnership();
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public void DeleteTool()
    {
        HapticManager.Instance?.ActivateHapticRight(.3f, .5f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentColorIndex);
        }
        else
        {
            currentColorIndex = (int)stream.ReceiveNext();
            ApplyCurrentColor();
        }
    }
}













/*
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ModelPart : ModelObject, IColorChangable, IDeletable
{
    public int partID;
    public Color[] colors = new Color[8] { Color.white, Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.yellow, Color.cyan };
    public InputActionProperty changeColor;

    private int currentColorIndex;
    public InputActionProperty deleteButton;
    private void Start()
    {
        XRGrabNetworkInteractable xRGrabNetworkInteractable = GetComponent<XRGrabNetworkInteractable>();
        xRGrabNetworkInteractable.selectEntered.AddListener(OnSelect);
        xRGrabNetworkInteractable.selectExited.AddListener(OnExit);

        photonView = GetComponent<PhotonView>();

        changeColor.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };
        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);

            }
        };
    }
    [PunRPC]
    private void OnExit(SelectExitEventArgs arg0)
    {
        OnGrabReleased();
    }

    private void OnSelect(SelectEnterEventArgs arg0)
    {
        OnGrabbed();
    }

    public Color GetCurrentColor()
    {
        return colors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        throw new System.NotImplementedException();
    }

    [PunRPC]

    private void SwitchColor()
    {
        if (currentColorIndex == colors.Length - 1)
        {
            currentColorIndex = 0;
        }
        else
        {
            currentColorIndex++;
        }
        //GetComponent<Renderer>().material.color = colors[currentColorIndex];

        if (TryGetComponent(out Renderer rend))
        {
            // rend.material.color = colors[currentColorIndex];
            rend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
        else
        {
            if (transform.GetChild(0).TryGetComponent(out Renderer rendinChild))
            {
                rendinChild.material.SetColor("_BaseColor", colors[currentColorIndex]);
                // rendinChild.material.color = colors[currentColorIndex];
            }
        }
    }

    public void Reset()
    {
        photonView.RequestOwnership();

        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;

        float scale = transform.parent.localScale.x;
        transform.localScale = Vector3.one;// * scale;
    }

    public void DeleteTool()
    {
        HapticManager.Instance.ActivateHapticRight(.3f, .5f);
        // Destroy(gameObject);
    }
}
*/








using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ModelPart : ModelObject, IColorChangable, IDeletable, IPunObservable
{
    [Header("Part Configuration")]
    public int partID;
    public Color[] colors = new Color[8] { Color.white, Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.yellow, Color.cyan };

    [Header("Input Actions")]
    public InputActionProperty changeColor;
    public InputActionProperty deleteButton;

    [Header("Private Fields")]
    [SerializeField] private int currentColorIndex;
    private ModelDeletable modelDeletable;             ///newwwww
    
    private PhotonView photonView;
    private bool isGrabbed;

    private void Start()
    {
        InitializeComponents();
        SetupInteractions();
        SetupInputHandlers();
        ApplyCurrentColor();
    }

    private void InitializeComponents()
    {
        photonView = GetComponent<PhotonView>();
        modelDeletable = GetComponent<ModelDeletable>();     /////////////////newwww
        // Check if this object was already deleted globally
        string modelID = photonView.ViewID.ToString();
        if (DeletionSyncUtility.IsModelDeletedGlobally(modelID))
        {
            Destroy(gameObject);
            return;
        }
    }

    private void SetupInteractions()
    {
        XRGrabNetworkInteractable grabInteractable = GetComponent<XRGrabNetworkInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelect);
            grabInteractable.selectExited.AddListener(OnExit);
        }
    }

    private void SetupInputHandlers()
    {
        // Color change input handler
        changeColor.action.performed += (ctx) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };

        // Delete input handler
        deleteButton.action.performed += (ctx) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                DeleteTool();
                string modelID = photonView.ViewID.ToString();
                DeletionSyncUtility.AddDeletedModelID(modelID);
                // photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
                modelDeletable.DeleteModel();
            }
        };
    }

    #region Interaction Events
    private void OnSelect(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        OnGrabbed();


       
    }

    private void OnExit(SelectExitEventArgs args)
    {
        isGrabbed = false;
        OnGrabReleased();

        
    }
    #endregion

    #region Color Management
    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        ApplyCurrentColor();
    }

    private void ApplyCurrentColor()
    {
        if (TryGetComponent(out Renderer rend))
        {
            rend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
        else if (transform.childCount > 0 && transform.GetChild(0).TryGetComponent(out Renderer childRend))
        {
            childRend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
    }

    public Color GetCurrentColor()
    {
        return colors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        // Interface implementation - not used in this context
        throw new System.NotImplementedException();
    }


    [PunRPC]
    private void ApplyCustomColorRPC(float r, float g, float b)
    {
        Color pickedColor = new Color(r, g, b);

        if (TryGetComponent(out Renderer rend))
        {
            rend.material.SetColor("_BaseColor", pickedColor);
        }
        else if (transform.childCount > 0 && transform.GetChild(0).TryGetComponent(out Renderer childRend))
        {
            childRend.material.SetColor("_BaseColor", pickedColor);
        }

        Debug.Log($"?? Custom color applied: {pickedColor}");
    }

    #endregion

    #region Delete Functionality
    public void DeleteTool()
    {
        // Trigger haptic feedback for deletion
        HapticManager.Instance?.ActivateHapticRight(.3f, .5f);
    }

/*    [PunRPC]
    private void DestroyOverNetwork()
    {
        string modelID = photonView.ViewID.ToString();
        DeletionSyncUtility.AddDeletedModelID(modelID);
        DeleteTool();
        Destroy(gameObject);
    }*/
    #endregion

    #region Utility Methods
    public void Reset()
    {
        if (photonView != null)
        {
            photonView.RequestOwnership();
        }

        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    #endregion

    #region Photon Networking
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentColorIndex);
        }
        else
        {
            currentColorIndex = (int)stream.ReceiveNext();
            ApplyCurrentColor();
        }
    }
    #endregion
}




















/*using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ModelPart : ModelObject, IColorChangable, IDeletable, IPunObservable
{
    [Header("Part Configuration")]
    public int partID;
    public Color[] colors = new Color[8] { Color.white, Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.yellow, Color.cyan };

    [Header("Input Actions")]
    public InputActionProperty changeColor;
    public InputActionProperty deleteButton;

    [Header("UI Indicators")]
    public ColorChangeIndicator colorChangeIndicator;
    public Indicator deleteIndicator;

    [Header("Private Fields")]
    [SerializeField] private int currentColorIndex;

    private PhotonView photonView;
    private bool isGrabbed;

    private void Start()
    {
        InitializeComponents();
        SetupInteractions();
        SetupInputHandlers();
        ApplyCurrentColor();
        SetIndicatorsVisibility(false); // Hide indicators initially
    }

    private void InitializeComponents()
    {
        photonView = GetComponent<PhotonView>();

        // Auto-find indicators if not assigned
        if (colorChangeIndicator == null || deleteIndicator == null)
        {
            Transform indicatorsParent = transform.Find("Indicators");
            if (indicatorsParent != null)
            {
                if (colorChangeIndicator == null)
                {
                    Transform colorChangeTransform = indicatorsParent.Find("ColorChange");
                    if (colorChangeTransform != null)
                        colorChangeIndicator = colorChangeTransform.GetComponent<ColorChangeIndicator>();
                }

                if (deleteIndicator == null)
                {
                    Transform deleteTransform = indicatorsParent.Find("Delete");
                    if (deleteTransform != null)
                        deleteIndicator = deleteTransform.GetComponent<Indicator>();
                }
            }
        }

        // Check if this object was already deleted globally
        string modelID = photonView.ViewID.ToString();
        if (DeletionSyncUtility.IsModelDeletedGlobally(modelID))
        {
            Destroy(gameObject);
            return;
        }
    }

    private void SetupInteractions()
    {
        XRGrabNetworkInteractable grabInteractable = GetComponent<XRGrabNetworkInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelect);
            grabInteractable.selectExited.AddListener(OnExit);
        }
    }

    private void SetupInputHandlers()
    {
        // Color change input handler
        changeColor.action.performed += (ctx) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };

        // Delete input handler
        deleteButton.action.performed += (ctx) =>
        {
            if (isGrabbed && photonView.IsMine)
            {
                DeleteTool();
                string modelID = photonView.ViewID.ToString();
                DeletionSyncUtility.AddDeletedModelID(modelID);
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };
    }

    #region Interaction Events
    private void OnSelect(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        SetIndicatorsVisibility(true); // Show indicators when grabbed

        // Only call OnGrabbed if it exists and won't cause null reference
        try
        {
            OnGrabbed();
        }
        catch (System.NullReferenceException ex)
        {
            Debug.LogWarning($"OnGrabbed caused null reference: {ex.Message}");
        }
    }

    private void OnExit(SelectExitEventArgs args)
    {
        isGrabbed = false;
        SetIndicatorsVisibility(false); // Hide indicators when released
        OnGrabReleased();
    }

    private void SetIndicatorsVisibility(bool visible)
    {
        if (colorChangeIndicator != null)
        {
            if (visible)
                colorChangeIndicator.Show();
            else
                colorChangeIndicator.hide();
        }

        if (deleteIndicator != null)
        {
            if (visible)
                deleteIndicator.Show();
            else
                deleteIndicator.hide();
        }
    }
    #endregion

    #region Color Management
    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        ApplyCurrentColor();
    }

    private void ApplyCurrentColor()
    {
        if (TryGetComponent(out Renderer rend))
        {
            rend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
        else if (transform.childCount > 0 && transform.GetChild(0).TryGetComponent(out Renderer childRend))
        {
            childRend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
    }

    public Color GetCurrentColor()
    {
        return colors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        // Interface implementation - not used in this context
        throw new System.NotImplementedException();
    }
    #endregion

    #region Delete Functionality
    public void DeleteTool()
    {
        // Trigger haptic feedback for deletion
        HapticManager.Instance?.ActivateHapticRight(.3f, .5f);
    }

    [PunRPC]
    private void DestroyOverNetwork()
    {
        string modelID = photonView.ViewID.ToString();
        DeletionSyncUtility.AddDeletedModelID(modelID);
        DeleteTool();
        Destroy(gameObject);
    }
    #endregion

    #region Utility Methods
    public void Reset()
    {
        if (photonView != null)
        {
            photonView.RequestOwnership();
        }

        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    #endregion

    #region Photon Networking
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentColorIndex);
        }
        else
        {
            currentColorIndex = (int)stream.ReceiveNext();
            ApplyCurrentColor();
        }
    }
    #endregion
}
*/













/*using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ModelPart : ModelObject, IColorChangable, IDeletable
{
    public int partID;
    public Color[] colors = new Color[8] { Color.white, Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.yellow, Color.cyan };

    private InputAction changeColor;  // For Right/Change Color action
    private InputAction deleteButton;  // For deletion action

    private int currentColorIndex;
    private PhotonView photonView;

    private void Awake()
    {
        // Load the InputActionAsset from resources or a specific path
        var inputActionAsset = Resources.Load<InputActionAsset>("XRI Default Input Actions");

        // Automatically assign the Right/Change Color and Right/Delete actions
        if (inputActionAsset != null)
        {
            changeColor = inputActionAsset.FindAction("Right/Change Color");
            deleteButton = inputActionAsset.FindAction("Right/Delete");
        }
        else
        {
            Debug.LogError("InputActionAsset not found!");
        }
    }

    private void Start()
    {
        if (changeColor != null && deleteButton != null)
        {
            XRGrabNetworkInteractable xRGrabNetworkInteractable = GetComponent<XRGrabNetworkInteractable>();
            xRGrabNetworkInteractable.selectEntered.AddListener(OnSelect);
            xRGrabNetworkInteractable.selectExited.AddListener(OnExit);

            photonView = GetComponent<PhotonView>();

            changeColor.performed += (S) =>
            {
                if (isGrabbed)
                {
                    photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
                }
            };

            deleteButton.performed += (S) =>
            {
                if (isGrabbed)
                {
                    DeleteTool();
                    photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
                }
            };

            // Enable the actions
            changeColor.Enable();
            deleteButton.Enable();
        }
        else
        {
            Debug.LogError("Actions not assigned. Check Input Action Asset.");
        }
    }

    private void OnDestroy()
    {
        // Disable the actions when the object is destroyed
        changeColor?.Disable();
        deleteButton?.Disable();
    }

    [PunRPC]
    private void OnExit(SelectExitEventArgs arg0)
    {
        OnGrabReleased();
    }

    private void OnSelect(SelectEnterEventArgs arg0)
    {
        OnGrabbed();
    }

    public Color GetCurrentColor()
    {
        return colors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        throw new System.NotImplementedException();
    }

    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;

        if (TryGetComponent(out Renderer rend))
        {
            rend.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
        else if (transform.GetChild(0).TryGetComponent(out Renderer rendinChild))
        {
            rendinChild.material.SetColor("_BaseColor", colors[currentColorIndex]);
        }
    }

    public void Reset()
    {
        photonView.RequestOwnership();

        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one; // Adjust scale if needed
    }

    public void DeleteTool()
    {
        HapticManager.Instance.ActivateHapticRight(.3f, .5f);
        // Destroy(gameObject); // Uncomment if you want to destroy locally
    }
}
*/