using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class StentPart1 : MonoBehaviour, IPunObservable
{
    [Header("Part Configuration")]
    public int partID;
    public Color[] colors = new Color[8] { Color.white, Color.red, Color.blue, Color.green, Color.gray, Color.magenta, Color.yellow, Color.cyan };

    [Header("Input Actions")]
    public InputActionProperty changeColor;
    public InputActionProperty deleteButton;

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
    }

    private void InitializeComponents()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError($"⚠️ PhotonView is missing on {gameObject.name}");
            return;
        }

        string modelID = photonView.ViewID.ToString();
        if (DeletionSyncUtility.IsModelDeletedGlobally(modelID))
        {
            Debug.Log($"⚠️ Object {modelID} is already marked as deleted globally, destroying...");
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
                Debug.Log($"✅ Color change triggered for {gameObject.name}");
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
            else
            {
                Debug.LogWarning($"⚠️ Color change not triggered: isGrabbed={isGrabbed}, photonView.IsMine={photonView.IsMine}");
            }
        };

        // Delete input handler
        deleteButton.action.performed += (ctx) =>
        {
            if (ctx.control.device.name.Contains("RightHand") && isGrabbed && photonView.IsMine)
            {
                Debug.Log($"✅ Delete action triggered for {gameObject.name}");
                GameObject meshParent = FindMeshParent();
                if (meshParent != null)
                {
                    // Verify BrushStroke script
                    BrushStroke brushStroke = meshParent.GetComponent<BrushStroke>();
                    if (brushStroke == null)
                    {
                        Debug.LogWarning($"⚠️ Parent of Mesh {meshParent.name} has no BrushStroke script");
                        return;
                    }

                    PhotonView parentPhotonView = meshParent.GetComponent<PhotonView>();
                    if (parentPhotonView != null)
                    {
                        string modelID = parentPhotonView.ViewID.ToString();
                        if (DeletionSyncUtility.IsModelDeletedGlobally(modelID))
                        {
                            Debug.LogWarning($"⚠️ Object {meshParent.name} (ViewID: {modelID}) already marked as deleted globally");
                            return;
                        }

                        if (!parentPhotonView.IsMine)
                        {
                            Debug.Log($"Requesting ownership of {meshParent.name} (ViewID: {parentPhotonView.ViewID})");
                            StartCoroutine(WaitForOwnershipAndDestroy(parentPhotonView, meshParent));
                        }
                        else
                        {
                            DeletionSyncUtility.AddDeletedModelID(modelID);
                            MeasurmentInk.OnModelDeleted(modelID); // Assuming MeasurmentInk is defined
                            DeleteTool();
                            PhotonNetwork.Destroy(meshParent);
                            Debug.Log($"✅ Deleted parent of Mesh: {meshParent.name} (ViewID: {modelID})");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ Parent of Mesh {meshParent.name} has no PhotonView; cannot delete over network");
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ No parent of Mesh GameObject found to delete");
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ Delete action not triggered: isRightHand={ctx.control.device.name.Contains("RightHand")}, isGrabbed={isGrabbed}, photonView.IsMine={photonView.IsMine}");
            }
        };
    }

    #region Interaction Events
    private void OnSelect(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        Debug.Log($"✅ {gameObject.name} grabbed");
    }

    private void OnExit(SelectExitEventArgs args)
    {
        isGrabbed = false;
        Debug.Log($"✅ {gameObject.name} released");
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
        GameObject meshParent = FindMeshParent();
        if (meshParent != null && meshParent.TryGetComponent(out Renderer rend))
        {
            rend.material.SetColor("_BaseColor", colors[currentColorIndex]);
            Debug.Log($"✅ Color applied to Mesh parent: {colors[currentColorIndex]}");
        }
        else
        {
            Debug.LogWarning($"⚠️ No Mesh parent with Renderer found to apply color");
        }
    }

    public Color GetCurrentColor()
    {
        return colors[currentColorIndex];
    }

    [PunRPC]
    private void ApplyCustomColorRPC(float r, float g, float b)
    {
        Color pickedColor = new Color(r, g, b);
        GameObject meshParent = FindMeshParent();
        if (meshParent != null && meshParent.TryGetComponent(out Renderer rend))
        {
            rend.material.SetColor("_BaseColor", pickedColor);
            Debug.Log($"✅ Custom color applied to Mesh parent: {pickedColor}");
        }
        else
        {
            Debug.LogWarning($"⚠️ No Mesh parent with Renderer found to apply custom color");
        }
    }

    public void ApplyCustomColor(Color color)
    {
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(ApplyCustomColorRPC), RpcTarget.AllBuffered, color.r, color.g, color.b);
        }
    }
    #endregion

    #region Delete Functionality
    public void DeleteTool()
    {
        HapticManager.Instance?.ActivateHapticRight(.3f, .5f);
        Debug.Log("✅ Haptic feedback triggered for deletion");
    }

    private GameObject FindMeshParent()
    {
        Transform current = transform;
        while (current != null)
        {
            if (current.name == "Mesh")
            {
                if (current.parent != null)
                {
                    Debug.Log($"✅ Found Mesh GameObject: {current.name}, Parent: {current.parent.name}");
                    return current.parent.gameObject;
                }
                else
                {
                    Debug.LogWarning($"⚠️ Mesh GameObject {current.name} has no parent");
                    return null;
                }
            }
            current = current.parent;
        }

        // Fallback: Search in this object's children
        Transform meshInSelf = transform.Find("Mesh");
        if (meshInSelf != null)
        {
            if (meshInSelf.parent != null)
            {
                Debug.Log($"✅ Found Mesh GameObject in self: {meshInSelf.name}, Parent: {meshInSelf.parent.name}");
                return meshInSelf.parent.gameObject;
            }
            else
            {
                Debug.LogWarning($"⚠️ Mesh GameObject {meshInSelf.name} has no parent");
                return null;
            }
        }

        Debug.LogWarning("⚠️ No GameObject named 'Mesh' found in parent hierarchy or children");
        return null;
    }

    private IEnumerator WaitForOwnershipAndDestroy(PhotonView pv, GameObject target)
    {
        Debug.Log($"Waiting for ownership of {target.name} (ViewID: {pv.ViewID})");
        int maxAttempts = 100; // Prevent infinite loop
        while (!pv.IsMine && maxAttempts > 0)
        {
            yield return null; // Wait one frame
            maxAttempts--;
        }

        if (pv.IsMine)
        {
            string modelID = pv.ViewID.ToString();
            DeletionSyncUtility.AddDeletedModelID(modelID);
            MeasurmentInk.OnModelDeleted(modelID); // Assuming MeasurmentInk is defined
            DeleteTool();
            PhotonNetwork.Destroy(target);
            Debug.Log($"✅ Delayed deletion of {target.name} after gaining ownership");
        }
        else
        {
            Debug.LogWarning($"⚠️ Failed to gain ownership of {target.name} (ViewID: {pv.ViewID}) after {100 - maxAttempts} attempts");
        }
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