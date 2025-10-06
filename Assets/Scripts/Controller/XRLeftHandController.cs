/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRLeftHandController : MonoBehaviourSingleton<XRLeftHandController>
{
    [SerializeField] RotateIndicator rotateIndicator;
    [SerializeField] ResetIndicator resetIndicator;
    private GameObject grabedItem;

    public void SetRotateIndicatorActive()
    {
        rotateIndicator.Show();
    }

    public void SetResetIndicatorActive()
    {
        resetIndicator.Show();
    }
    public GameObject GetGrabbedItem()
    {
        return grabedItem;
    }
    public void SetGrabbedItem(GameObject _item)
    {
        grabedItem = _item;

        if (grabedItem != null)
        {
            rotateIndicator.hide();
            resetIndicator.hide();
        }
        else
        {
            // OnRelease
            rotateIndicator.Show();
            resetIndicator.Show();
        }
    }
}
*/
/*using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRLeftHandController : MonoBehaviourSingleton<XRLeftHandController>
{
    [SerializeField] RotateIndicator rotateIndicator;
    [SerializeField] ResetIndicator resetIndicator;
    private GameObject grabbedItem;

    public void SetRotateIndicatorActive()
    {
        rotateIndicator.Show();
    }

    public void SetResetIndicatorActive()
    {
        resetIndicator.Show();
    }

    public GameObject GetGrabbedItem()
    {
        return grabbedItem;
    }

    public void SetGrabbedItem(GameObject _item)
    {
        grabbedItem = _item;

        if (grabbedItem != null)
        {
            rotateIndicator.hide();
            resetIndicator.hide();
        }
        else
        {
            rotateIndicator.Show();
            resetIndicator.Show();
        }
    }

    // New method to attempt grabbing a model
    public void AttemptGrab(GameObject targetModel)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can grab objects.");
            return; // Prevent non-master clients from grabbing
        }

        if (targetModel.TryGetComponent<ModelObject>(out ModelObject modelObject))
        {
            modelObject.OnGrabbed(); // Call the OnGrabbed method on the model
            SetGrabbedItem(targetModel); // Keep track of the grabbed model
        }
        else
        {
            Debug.LogWarning("Target model is not a valid ModelObject.");
        }
    }
}
*/


///////////////////master client
///


using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRLeftHandController : MonoBehaviourSingleton<XRLeftHandController>
{
    [SerializeField] RotateIndicator rotateIndicator;
    [SerializeField] ResetIndicator resetIndicator;
    [SerializeField] public bool masterClientOnly = false; // Flag to control grab access
    private GameObject grabbedItem;

    public void SetRotateIndicatorActive()
    {
        rotateIndicator.Show();
    }

    public void SetResetIndicatorActive()
    {
        resetIndicator.Show();
    }

    public GameObject GetGrabbedItem()
    {
        return grabbedItem;
    }

    public void SetGrabbedItem(GameObject _item)
    {
        grabbedItem = _item;
        if (grabbedItem != null)
        {
            rotateIndicator.hide();
            resetIndicator.hide();
        }
        else
        {
            rotateIndicator.Show();
            resetIndicator.Show();
        }
    }

    public void AttemptGrab(GameObject targetModel)
    {
        if (masterClientOnly && !PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can grab objects.");
            return;
        }

        if (targetModel.TryGetComponent<ModelObject>(out ModelObject modelObject))
        {
            modelObject.OnGrabbed();
            SetGrabbedItem(targetModel);
        }
        else
        {
            Debug.LogWarning("Target model is not a valid ModelObject.");
        }
    }
}