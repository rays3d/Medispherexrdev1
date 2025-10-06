using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public abstract class Tool : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed;
    public InputActionProperty deleteButton;
    public virtual void DeleteTool()
    {
        HapticManager.Instance.ActivateHapticRight(.3f, .5f);

      //  NetworkManager.Destroy(gameObject);
    }
    
    public virtual void OnGrabbed()
    {
        isGrabbed = true;
        XRRightHandController.Instance.SetGrabbedItem(this.gameObject);
        XRLeftHandController.Instance.SetGrabbedItem(this.gameObject);
    }

    public virtual void OnGrabReleased()
    {
        isGrabbed = false;
        XRRightHandController.Instance.SetGrabbedItem(null);
        XRLeftHandController.Instance.SetGrabbedItem(null);
    }
}
