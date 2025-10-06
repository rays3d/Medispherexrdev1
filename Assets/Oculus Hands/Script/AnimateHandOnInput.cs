using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
public class AnimateHandOnInput : MonoBehaviour
{

    [SerializeField] Photon.Pun.PhotonView photonView;
    public InputActionProperty pinchAction;
    public InputActionProperty gripAction;




    Animator handAnimator;
    private void Start()
    {
        handAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (photonView == null)
        {
            handAnimator.SetFloat("Trigger", pinchAction.action.ReadValue<float>());
            handAnimator.SetFloat("Grip", gripAction.action.ReadValue<float>());
            return;
        }

        if (!photonView.IsMine) return;
        handAnimator.SetFloat("Trigger", pinchAction.action.ReadValue<float>());
        handAnimator.SetFloat("Grip", gripAction.action.ReadValue<float>());
    }

}
