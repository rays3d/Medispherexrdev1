using UnityEngine;
using UnityEngine.InputSystem;

public class XRLogoutTrigger : MonoBehaviour
{
    public InputActionProperty buttonBAction; // Assign this in the Inspector (secondaryButton of LeftHand)
    public LogoutManager logoutManager; // Assign your LogoutManager reference here
    

    void Start()
    {
        if (buttonBAction.action != null)
        {
            buttonBAction.action.Enable(); // Enable the input manually
        }
    }
    void Update()
    {
        if (buttonBAction.action != null && buttonBAction.action.WasPressedThisFrame())
        {
           
            Debug.Log("Button B pressed on Left Controller.");
            if (logoutManager != null)
            {
                logoutManager.Logout();
            }
            else
            {
                Debug.LogWarning("LogoutManager reference is missing!");
            }
        }
    }
}
