/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class ScaleItem : MonoBehaviour
{
    [Header("Input:-")]
    public InputActionProperty scaleAction;
    public InputActionProperty zoomLeft;
    public InputActionProperty zoomRight;

    Transform targetObject;
    [Header("Scale Rate")]
    [SerializeField] float scaleValue = .1f;
    [Header("Min & Max Scale")]
    [SerializeField] private float minScale = .1f;
    [SerializeField] private float maxScale = 5f;

    float preDistance;
    void Start()
    {
        SelectionManager.Instance.OnSelectedObject += OnSelectingObject;

        XRRigMapper xRRigMapper = FindObjectOfType<XRRigMapper>();

        scaleAction.action.performed += _ =>
        {
            if (targetObject == null) return;
            float inputY = _.ReadValue<Vector2>().y;


            if (inputY > .5f)
            {
                targetObject.localScale += Vector3.one * scaleValue;
            }
            else if (inputY < -.5f)
            {
                targetObject.localScale -= Vector3.one * scaleValue;
            }
        };

        zoomLeft.action.performed += _ =>
        {
            if (targetObject == null) return;
        };
        zoomRight.action.performed += _ =>
        {
            if (targetObject == null) return;
            float distance = Vector3.Distance(xRRigMapper.rightHandTarget.position, xRRigMapper.leftHandTarget.position);

            if (Mathf.Abs(distance - preDistance) < .01) return;
            if (!targetObject.GetComponent<Photon.Pun.PhotonView>().IsMine)
            {
                targetObject.GetComponent<Photon.Pun.PhotonView>().RequestOwnership();
            }
            float yScale = 0; ;
            if (targetObject.TryGetComponent(out Renderer render))
            {
                yScale = render.bounds.center.y / 2;
            }
            if (distance < preDistance)
            {
                targetObject.localScale -= Vector3.one * scaleValue;
            }
            else
            {
                targetObject.localScale += Vector3.one * scaleValue;
            }
            preDistance = distance;
        };
    }

    private void OnSelectingObject()
    {
        targetObject = SelectionManager.Instance.GetSelectedModel().transform;
    }

    private void Update()
    {

        if (targetObject == null) return;

        if (targetObject.localScale.x < minScale)
        {
            targetObject.localScale = Vector3.one * minScale;
        }
        else if (targetObject.localScale.x > maxScale)
        {
            targetObject.localScale = Vector3.one * maxScale;
        }
    }

}
*/


/////////////////////////////////////////////////////////////this workingg///////////////////////////////////////////////////

/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class ScaleItem : MonoBehaviour
{
    [Header("Input:-")]
    public InputActionProperty scaleAction;
    public InputActionProperty zoomLeft;
    public InputActionProperty zoomRight;

    private Transform targetObject;

    [Header("Scale Rate")]
    [SerializeField] private float scaleValue = .1f;

    [Header("Min & Max Scale")]
    [SerializeField] private float minScale = .1f;
    [SerializeField] private float maxScale = 5f;

    private float preDistance;

    private void Start()
    {
        SelectionManager.Instance.OnSelectedObject += OnSelectingObject;

        XRRigMapper xRRigMapper = FindObjectOfType<XRRigMapper>();

        scaleAction.action.performed += _ =>
        {
            if (targetObject == null || !PhotonNetwork.IsMasterClient) return;

            float inputY = _.ReadValue<Vector2>().y;

            if (inputY > .5f)
            {
                targetObject.localScale += Vector3.one * scaleValue;
            }
            else if (inputY < -.5f)
            {
                targetObject.localScale -= Vector3.one * scaleValue;
            }
        };

        zoomLeft.action.performed += _ =>
        {
            if (targetObject == null) return;
        };

        zoomRight.action.performed += _ =>
        {
            if (targetObject == null || !PhotonNetwork.IsMasterClient) return;

            float distance = Vector3.Distance(xRRigMapper.rightHandTarget.position, xRRigMapper.leftHandTarget.position);

            if (Mathf.Abs(distance - preDistance) < .01) return;

            // Request ownership if not owned
            var photonView = targetObject.GetComponent<PhotonView>();
            if (!photonView.IsMine)
            {
                photonView.RequestOwnership();
            }

            if (distance < preDistance)
            {
                targetObject.localScale -= Vector3.one * scaleValue;
            }
            else
            {
                targetObject.localScale += Vector3.one * scaleValue;
            }

            preDistance = distance;
        };
    }

    private void OnSelectingObject()
    {
        targetObject = SelectionManager.Instance.GetSelectedModel()?.transform;
    }

    private void Update()
    {
        if (targetObject == null) return;

        // Clamping the scale to min and max values
        float currentScale = targetObject.localScale.x;
        if (currentScale < minScale)
        {
            targetObject.localScale = Vector3.one * minScale;
        }
        else if (currentScale > maxScale)
        {
            targetObject.localScale = Vector3.one * maxScale;
        }
    }
}*/



////////////////////////////////without masterclient/////////////////////////////////



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class ScaleItem : MonoBehaviour
{
    [Header("Input:-")]
    public InputActionProperty scaleAction;
    public InputActionProperty zoomLeft;
    public InputActionProperty zoomRight;
    private Transform targetObject;
    [Header("Scale Rate")]
    [SerializeField] private float scaleValue = .1f;
    [Header("Min & Max Scale")]
    [SerializeField] private float minScale = .1f;
    [SerializeField] private float maxScale = 5f;
    private float preDistance;

    private void Start()
    {
        SelectionManager.Instance.OnSelectedObject += OnSelectingObject;
        XRRigMapper xRRigMapper = FindObjectOfType<XRRigMapper>();
        scaleAction.action.performed += _ =>
        {
            if (targetObject == null) return;
            float inputY = _.ReadValue<Vector2>().y;
            if (inputY > .5f)
            {
                targetObject.localScale += Vector3.one * scaleValue;
            }
            else if (inputY < -.5f)
            {
                targetObject.localScale -= Vector3.one * scaleValue;
            }
        };
        zoomLeft.action.performed += _ =>
        {
            if (targetObject == null) return;
        };
        zoomRight.action.performed += _ =>
        {
            if (targetObject == null) return;
            float distance = Vector3.Distance(xRRigMapper.rightHandTarget.position, xRRigMapper.leftHandTarget.position);
            if (Mathf.Abs(distance - preDistance) < .01) return;
            var photonView = targetObject.GetComponent<PhotonView>();
            if (!photonView.IsMine)
            {
                photonView.RequestOwnership();
            }
            if (distance < preDistance)
            {
                targetObject.localScale -= Vector3.one * scaleValue;
            }
            else
            {
                targetObject.localScale += Vector3.one * scaleValue;
            }
            preDistance = distance;
        };
    }

    private void OnSelectingObject()
    {
        targetObject = SelectionManager.Instance.GetSelectedModel()?.transform;
    }

    private void Update()
    {
        if (targetObject == null) return;
        float currentScale = targetObject.localScale.x;
        if (currentScale < minScale)
        {
            targetObject.localScale = Vector3.one * minScale;
        }
        else if (currentScale > maxScale)
        {
            targetObject.localScale = Vector3.one * maxScale;
        }
    }
}