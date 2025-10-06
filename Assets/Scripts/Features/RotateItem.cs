/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateItem : MonoBehaviour
{
    [Header("Input:-")]
    public InputActionProperty rotateInputValue;
    public InputActionProperty grabAction;
    [Header("Settings:-")]
    public float rotationSpeed = 10.0f;
    Transform targetObject;

    bool isGrabbing;
    Vector3 rotatePoint;

    private void Start()
    {
        SelectionManager.Instance.OnSelectedObject += OnSelectingObject;


        grabAction.action.performed += (S) =>
        {
            isGrabbing = true;
            SelectionManager.Instance.isGrabbing = true;
        };

        grabAction.action.canceled += (S) =>
        {
            isGrabbing = false;
            SelectionManager.Instance.isGrabbing = false;

        };
    }





    void OnSelectingObject()
    {
        targetObject = SelectionManager.Instance.GetSelectedModel().transform;

    }
    void Update()
    {

        if (isGrabbing) return;

        Vector2 thumbstickInput = rotateInputValue.action.ReadValue<Vector2>();

        float rotationX = 0;
        float rotationY = 0;

        if (thumbstickInput.y > .5f || thumbstickInput.y < -.5f)
            rotationX = thumbstickInput.y * rotationSpeed * Time.deltaTime;
        if (thumbstickInput.x > .5f || thumbstickInput.x < -.5f)
            rotationY = thumbstickInput.x * rotationSpeed * Time.deltaTime;
        if (targetObject == null) return;

        rotatePoint = Vector3.zero;
        if (targetObject.TryGetComponent(out Renderer filter))
        {
            rotatePoint = filter.bounds.center;
        }
        else
        {
            if (targetObject.GetChild(0).TryGetComponent(out Renderer rend))
            {
                rotatePoint = rend.bounds.center;
            }
        }
        targetObject.RotateAround(rotatePoint, Vector3.right, rotationX);
        targetObject.RotateAround(rotatePoint, Vector3.up, -rotationY);
    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class RotateItem : MonoBehaviour
{
    [Header("Input:-")]
    public InputActionProperty rotateInputValue;
    public InputActionProperty grabAction;

    [Header("Settings:-")]
    public float rotationSpeed = 10.0f;

    private Transform targetObject;
    private bool isGrabbing;
    private Vector3 rotatePoint;

    private void Start()
    {
        SelectionManager.Instance.OnSelectedObject += OnSelectingObject;

        grabAction.action.performed += (S) =>
        {
            isGrabbing = true;
            SelectionManager.Instance.isGrabbing = true;
        };

        grabAction.action.canceled += (S) =>
        {
            isGrabbing = false;
            SelectionManager.Instance.isGrabbing = false;
        };
    }

    private void OnSelectingObject()
    {
        targetObject = SelectionManager.Instance.GetSelectedModel()?.transform;
    }

    private void Update()
    {
        // Check if the client is the master before allowing rotation
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can rotate objects.");
            return; // Prevent non-master clients from rotating objects
        }

        if (isGrabbing || targetObject == null) return;

        Vector2 thumbstickInput = rotateInputValue.action.ReadValue<Vector2>();

        float rotationX = 0;
        float rotationY = 0;

        if (Mathf.Abs(thumbstickInput.y) > 0.5f)
            rotationX = thumbstickInput.y * rotationSpeed * Time.deltaTime;

        if (Mathf.Abs(thumbstickInput.x) > 0.5f)
            rotationY = thumbstickInput.x * rotationSpeed * Time.deltaTime;

        if (targetObject.TryGetComponent(out Renderer filter))
        {
            rotatePoint = filter.bounds.center;
        }
        else if (targetObject.childCount > 0 && targetObject.GetChild(0).TryGetComponent(out Renderer rend))
        {
            rotatePoint = rend.bounds.center;
        }
        else
        {
            rotatePoint = targetObject.position;
        }

        targetObject.RotateAround(rotatePoint, Vector3.right, rotationX);
        targetObject.RotateAround(rotatePoint, Vector3.up, -rotationY);
    }
}
