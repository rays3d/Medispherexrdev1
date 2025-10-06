using UnityEngine;
using UnityEngine.UI;

public class KeyboardButton : MonoBehaviour
{
    public string keyValue; // Set this to the value of the key (e.g., "A", "B", "1")

    private InputField activeInputField;

    private void Update()
    {
        // Find the currently selected InputField
        activeInputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<InputField>();
    }

    public void OnButtonClick()
    {
        if (activeInputField != null)
        {
            // Add the key value to the active InputField's text
            activeInputField.text += keyValue;

            // Optionally, set the InputField to be focused
            activeInputField.Select();
            activeInputField.ActivateInputField();
        }
    }
}
