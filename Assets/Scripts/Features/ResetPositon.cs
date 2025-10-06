/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class ResetPositon : MonoBehaviour
{
    [Header("Input:-")]
    public InputActionProperty resetInput;

    private void Start()
    {
        resetInput.action.performed += OnMenuActionTrigger;
    }

    private void OnMenuActionTrigger(InputAction.CallbackContext obj)
    {
        if (SelectionManager.Instance.GetSelectedModel() == null || SelectionManager.Instance.isGrabbing || SelectionManager.Instance.GetSelectedModel().transform.parent == null) return;
        ResetPos();
    }
    void ResetPos()
    {
        if (SelectionManager.Instance.GetSelectedModel().transform.parent.TryGetComponent(out Model obj))
        {
            obj.ResetAllParts();
            SelectionManager.Instance.SelectModel(obj);

            HapticManager.Instance.ActivateHapticLeft(.25f, .2f);
            HapticManager.Instance.ActivateHapticRight(.25f, .2f);
        }

    }
}
*/
/////////////////////////////////////////////////////////main///////////////////////////////////////////////////////

/*using UnityEngine;
using UnityEngine.InputSystem;

public class ResetPosition : MonoBehaviour
{
    [SerializeField] private InputActionProperty resetInput;

    private void OnEnable()
    {
        if (resetInput.action != null)
        {
            resetInput.action.Enable();
            resetInput.action.performed += OnResetTrigger;
        }
    }

    private void OnDisable()
    {
        if (resetInput.action != null)
        {
            resetInput.action.performed -= OnResetTrigger;
            resetInput.action.Disable();
        }
    }

    private void OnResetTrigger(InputAction.CallbackContext context)
    {
        ModelObject selectedModel = SelectionManager.Instance?.GetSelectedModel();
        if (selectedModel == null)
        {
            Debug.LogWarning("Reset aborted: No model selected.");
            return;
        }

        ResetPos(selectedModel);
    }

    private void ResetPos(ModelObject model)
    {
        if (model.photonView != null && model.photonView.IsMine)
        {
            model.gameObject.transform.position = model.instantiationPosition;
            model.gameObject.transform.rotation = model.instantiationRotation;
            model.gameObject.transform.localScale = model.instantiationScale;
            HapticManager.Instance?.ActivateHapticLeft(0.25f, 0.2f);
            HapticManager.Instance?.ActivateHapticRight(0.25f, 0.2f);
            Debug.Log($"Model {model.gameObject.name} reset to instantiation position: {model.instantiationPosition}, rotation: {model.instantiationRotation}, scale: {model.instantiationScale}");
        }
        else
        {
            Debug.LogWarning("Reset failed: Model not owned by this client.");
        }
    }
}

*/


using UnityEngine;
using UnityEngine.InputSystem;

public class ResetPosition : MonoBehaviour
{
    [SerializeField] private InputActionProperty resetInput;
    [SerializeField] private ResetIndicator resetIndicator; // Reference to the indicator

    private void OnEnable()
    {
        if (resetInput.action != null)
        {
            resetInput.action.Enable();
            resetInput.action.performed += OnResetTrigger;
        }
    }

    private void OnDisable()
    {
        if (resetInput.action != null)
        {
            resetInput.action.performed -= OnResetTrigger;
            resetInput.action.Disable();
        }
    }

    private void OnResetTrigger(InputAction.CallbackContext context)
    {
        // ?? Only allow reset if indicator is visible
        if (resetIndicator == null || !resetIndicator.gameObject.activeSelf)
        {
            Debug.Log("Reset blocked: Indicator is hidden.");
            return;
        }

        ModelObject selectedModel = SelectionManager.Instance?.GetSelectedModel();
        if (selectedModel == null)
        {
            Debug.LogWarning("Reset aborted: No model selected.");
            return;
        }

        ResetPos(selectedModel);
    }

    private void ResetPos(ModelObject model)
    {
        if (model.photonView != null && model.photonView.IsMine)
        {
            model.gameObject.transform.position = model.instantiationPosition;
            model.gameObject.transform.rotation = model.instantiationRotation;
            model.gameObject.transform.localScale = model.instantiationScale;
            HapticManager.Instance?.ActivateHapticLeft(0.25f, 0.2f);
            HapticManager.Instance?.ActivateHapticRight(0.25f, 0.2f);
            Debug.Log($"Model {model.gameObject.name} reset to instantiation position: {model.instantiationPosition}, rotation: {model.instantiationRotation}, scale: {model.instantiationScale}");
        }
        else
        {
            Debug.LogWarning("Reset failed: Model not owned by this client.");
        }
    }
}
