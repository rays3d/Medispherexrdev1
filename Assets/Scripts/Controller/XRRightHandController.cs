/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRRightHandController : MonoBehaviourSingleton<XRRightHandController>
{
    GameObject grabedItem;

    [SerializeField] ColorChangeIndicator changeColorIndicator;
    [SerializeField] DeleteItemIndicator deleteItemIndicator;
    [SerializeField] RotateIndicator rotateIndicator;
    [SerializeField] ResetIndicator resetIndicator;

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

            if (grabedItem.TryGetComponent(out IDeletable deletable))
            {
                deleteItemIndicator.Show();
            }
            else
            {
                deleteItemIndicator.hide();

            }

            if (grabedItem.TryGetComponent(out IColorChangable colorChangeable))
            {
                changeColorIndicator.Show();
            }
            else
            {
                changeColorIndicator.hide();
            }

            rotateIndicator.hide();
            resetIndicator.hide();

        }
        else
        {
            // OnRelease
            deleteItemIndicator.hide();
            changeColorIndicator.hide();
            rotateIndicator.Show();
            resetIndicator.Show();
        }
    }
}
*/
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRRightHandController : MonoBehaviourSingleton<XRRightHandController>
{
    GameObject grabbedItem;

    [SerializeField] ColorChangeIndicator changeColorIndicator;
    [SerializeField] DeleteItemIndicator deleteItemIndicator;
    [SerializeField] RotateIndicator rotateIndicator;
    [SerializeField] ResetIndicator resetIndicator;
    [SerializeField] public bool masterClientOnly = false; // Flag to control grab access

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
            if (grabbedItem.TryGetComponent(out IDeletable deletable))
            {
                deleteItemIndicator.Show();
            }
            else
            {
                deleteItemIndicator.hide();
            }

            if (grabbedItem.TryGetComponent(out IColorChangable colorChangeable))
            {
                changeColorIndicator.Show();
            }
            else
            {
                changeColorIndicator.hide();
            }

            rotateIndicator.hide();
            resetIndicator.hide();
        }
        else
        {
            deleteItemIndicator.hide();
            changeColorIndicator.hide();
            rotateIndicator.Show();
            resetIndicator.Show();
        }
    }

    // New method to attempt grabbing a model
    public void AttemptGrab(GameObject targetModel)
    {
        if (masterClientOnly && !PhotonNetwork.IsMasterClient)
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
