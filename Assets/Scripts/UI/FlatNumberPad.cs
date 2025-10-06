/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class FlatNumberPad : MonoBehaviour
{
    TMP_InputField targetInputField;
    [SerializeField] GameObject keyboard;

    float activeTime;

    public void SetInputfield(TMP_InputField _field)
    {
        targetInputField = _field;
        targetInputField.onSelect.AddListener(ActivateKeyboard);
    }

    private void ActivateKeyboard(string arg0)
    {
        keyboard.SetActive(true);
        activeTime = 0;
    }
    private void Deactivate(string arg0)
    {
        keyboard.SetActive(false);
        activeTime = 0;
    }

    public void TypeChar(string c)
    {
        targetInputField.text += c;
        activeTime = 0;
    }

    public void ClearAChar()
    {
        activeTime = 0;

        if (targetInputField == null || targetInputField.text.Length <= 0) return;
        string currentString = targetInputField.text;

        targetInputField.text = currentString.Remove(currentString.Length - 1, 1);
    }

    private void Update()
    {
        activeTime += Time.deltaTime;
        if (activeTime >= 5)
        {
            keyboard.SetActive(false);
        }
    }
}


*/

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlatNumberPad : MonoBehaviour
{
    private TMP_InputField targetInputField;
    [SerializeField] private GameObject keyboard; // Reference to the keyboard GameObject
    private float activeTime;

    // Method to set the input field and manage listeners
    public void SetInputfield(TMP_InputField _field)
    {
        // Remove listener from previous input field
        if (targetInputField != null)
        {
            targetInputField.onSelect.RemoveListener(ActivateKeyboard);
            targetInputField.onDeselect.RemoveListener(Deactivate);
        }

        targetInputField = _field; // Set the new target input field
        targetInputField.onSelect.AddListener(ActivateKeyboard);
        targetInputField.onDeselect.AddListener(Deactivate);
    }

    // Activate the keyboard when an input field is selected
    private void ActivateKeyboard(string arg0)
    {
        keyboard.SetActive(true);
        activeTime = 0; // Reset active time
    }

    // Deactivate the keyboard when the input field is deselected
    private void Deactivate(string arg0)
    {
        // Optionally: only deactivate if there's no current input or if you want it to behave differently
        // keyboard.SetActive(false);
        // activeTime = 0; // Reset active time
        // targetInputField = null; // Clear the reference to the input field
    }

    // Type a character into the target input field
    public void TypeChar(string c)
    {
        if (targetInputField != null)
        {
            targetInputField.text += c; // Append the character
            // activeTime = 0; // Keep the keyboard active, don't reset time or deactivate
        }
    }

    // Clear the last character from the target input field
    public void ClearAChar()
    {
        if (targetInputField == null || targetInputField.text.Length <= 0) return;

        string currentString = targetInputField.text;
        targetInputField.text = currentString.Remove(currentString.Length - 1, 1); // Remove the last character
    }
    // Add a space to the input field
    public void AddSpace()
    {
        if (targetInputField != null)
        {
            targetInputField.text += " ";
        }
    }

    // Trigger enter functionality
    public void Enter()
    {
        if (targetInputField != null)
        {
            Debug.Log("Entered text: " + targetInputField.text);
            keyboard.SetActive(false); // Hide keyboard on Enter
            targetInputField.DeactivateInputField(); // Deselect input
            targetInputField = null;
        }
    }

    private void Update()
    {
        activeTime += Time.deltaTime; // Increment active time
        if (activeTime >= 5) // Auto deactivate after 5 seconds of inactivity
        {
            keyboard.SetActive(false);
            targetInputField = null; // Clear target if keyboard auto-deactivates
        }
    }
}

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FlatNumberPad : MonoBehaviour
{
    private TMP_InputField targetInputField;
    [SerializeField] private GameObject keyboard; // The keyboard UI panel
    private float activeTime;

    public void SetInputfield(TMP_InputField _field)
    {
        targetInputField = _field;
        keyboard.SetActive(true);
        activeTime = 0;
    }

 /*   private void Update()
    {
        activeTime += Time.deltaTime;
        if (activeTime >= 5f)
        {
            keyboard.SetActive(false);
            targetInputField = null;
        }
    }*/

    public void TypeChar(string c)
    {
        if (targetInputField != null)
        {
            targetInputField.text += c;
            activeTime = 0;
        }
    }

    public void ClearAChar()
    {
        if (targetInputField == null || targetInputField.text.Length <= 0) return;

        string currentString = targetInputField.text;
        targetInputField.text = currentString.Remove(currentString.Length - 1, 1);
        activeTime = 0;
    }

    public void AddSpace()
    {
        if (targetInputField != null)
        {
            targetInputField.text += " ";
            activeTime = 0;
        }
    }

    public void Enter()
    {
        if (targetInputField != null)
        {
            Debug.Log("Entered text: " + targetInputField.text);

            // Find the next selectable UI element
            Selectable next = targetInputField.FindSelectableOnDown();

            // Deactivate current input field
            targetInputField.DeactivateInputField();

            // If the next selectable is an input field, activate it
            if (next != null && next is TMP_InputField nextInputField)
            {
                nextInputField.ActivateInputField();  // Focus next input field
                SetInputfield(nextInputField);        // Set it as target
            }
            else
            {
                // If no next input field, close the keyboard
                //keyboard.SetActive(false);
                targetInputField = null;
            }
        }
    }

}
