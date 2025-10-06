/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    PhotonView photonView;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

   
    void Update()
    {
        
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        photonView.RequestOwnership();
        base.OnSelectEntered(args);
    }
}
*/

//////////////////////////////////////////////////////////////////////////////////////////////////////


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RequestOwnership();
            base.OnSelectEntered(args);
        }
        else
        {
            // Optionally, you can provide feedback to non-master clients
            Debug.Log("Only the master client can grab this object.");
        }
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        return PhotonNetwork.IsMasterClient && base.IsSelectableBy(interactor);
    }
}

*/


////////////////////////////////////////////////////////////////////////////////////////////////
///
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    private PhotonView photonView;
    [SerializeField] public bool masterClientOnly = false; // Flag to control grab access

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError($"[XRGrabNetworkInteractable] {gameObject.name} is missing PhotonView component!");
        }
        // Sync with PresenterModeToggle's initial state
        var presenterToggle = FindObjectOfType<PresenterModeToggle>();
        if (presenterToggle != null)
        {
            masterClientOnly = presenterToggle.GetComponent<PresenterModeToggle>().isPresenterMode;
            Debug.Log($"[XRGrabNetworkInteractable] {gameObject.name} initialized: masterClientOnly={masterClientOnly}");
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"[XRGrabNetworkInteractable] OnSelectEntered for {gameObject.name}, IsMasterClient={PhotonNetwork.IsMasterClient}");
        if (photonView != null && !photonView.IsMine)
        {
            photonView.RequestOwnership();
            Debug.Log($"[XRGrabNetworkInteractable] {gameObject.name} requested ownership.");
        }
        base.OnSelectEntered(args);
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        bool canSelect = (!masterClientOnly || PhotonNetwork.IsMasterClient) && base.IsSelectableBy(interactor);
        Debug.Log($"[XRGrabNetworkInteractable] IsSelectableBy for {gameObject.name}: masterClientOnly={masterClientOnly}, IsMasterClient={PhotonNetwork.IsMasterClient}, CanSelect={canSelect}");
        return canSelect;
    }
}