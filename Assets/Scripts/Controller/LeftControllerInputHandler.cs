/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftControllerInputHandler : MonoBehaviour
{
    [Header("Menu Button")]
    [SerializeField] XRControllerButton menuButton;
    public GameObject menu;


    private void Start()
    {
        menuButton.button.action.performed += _ =>
        {
            menu.SetActive(!menu.activeSelf);
            menuButton.buttonRenderer.material.color = menuButton.pressedColor;
            HapticManager.Instance.ActivateHapticLeft(.25f,.2f);
        };
        menuButton.button.action.canceled += _ =>
        {
            menuButton.buttonRenderer.material.color = menuButton.normalColor;

        };
    }
}

[Serializable]
public class XRControllerButton
{
    [Header("Input: ")]
    public InputActionProperty button;

    [Header("Visual: ")]
    public Renderer buttonRenderer;
    public Animator buttonAnimator;

    [Header("Color: ")]
    public Color normalColor;
    public Color pressedColor;

    public void OnPressed()
    {
        buttonAnimator.SetBool("Pressed", true);
    }

    public void OnReleased()
    {
        buttonAnimator.SetBool("Pressed", false);
    }

}
*/
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftControllerInputHandler : MonoBehaviour
{
    [Header("Menu Button")]
    [SerializeField] XRControllerButton menuButton;
    public GameObject menu;

    private void Start()
    {
        // Check for null references at the start
        if (menu == null)
        {
            Debug.LogError("Menu GameObject is not assigned or has been destroyed.");
            return;
        }

        if (menuButton == null || menuButton.buttonRenderer == null)
        {
            Debug.LogError("Menu Button or Button Renderer is not assigned.");
            return;
        }

        // Subscribe to input action events
        menuButton.button.action.performed += HandleMenuToggle;
        menuButton.button.action.canceled += HandleMenuRelease;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the input action events to prevent dangling references
        if (menuButton.button.action != null)
        {
            menuButton.button.action.performed -= HandleMenuToggle;
            menuButton.button.action.canceled -= HandleMenuRelease;
        }
    }

    private void HandleMenuToggle(InputAction.CallbackContext context)
    {
        if (menu != null)
        {
            menu.SetActive(!menu.activeSelf);
            menuButton.buttonRenderer.material.color = menuButton.pressedColor;
            HapticManager.Instance.ActivateHapticLeft(.25f, .2f);
        }
        else
        {
            Debug.LogWarning("Menu GameObject has been destroyed when trying to toggle visibility.");
        }
    }

    private void HandleMenuRelease(InputAction.CallbackContext context)
    {
        if (menuButton.buttonRenderer != null)
        {
            menuButton.buttonRenderer.material.color = menuButton.normalColor;
        }
        else
        {
            Debug.LogWarning("Button Renderer has been destroyed when trying to change color.");
        }
    }
}

[Serializable]
public class XRControllerButton
{
    [Header("Input: ")]
    public InputActionProperty button;

    [Header("Visual: ")]
    public Renderer buttonRenderer;
    public Animator buttonAnimator;

    [Header("Color: ")]
    public Color normalColor;
    public Color pressedColor;

    public void OnPressed()
    {
        buttonAnimator.SetBool("Pressed", true);
    }

    public void OnReleased()
    {
        buttonAnimator.SetBool("Pressed", false);
    }
}
