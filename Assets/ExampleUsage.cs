/*using UnityEngine;
using TMPro;

public class ExampleUsage : MonoBehaviour
{
    [SerializeField] FlatNumberPad numberPad; // Reference to the FlatNumberPad script
    [SerializeField] TMP_InputField inputField1; // First input field
    [SerializeField] TMP_InputField inputField2; // Second input field
    [SerializeField] TMP_InputField inputField3;
    [SerializeField] TMP_InputField inputField4;
    [SerializeField] TMP_InputField inputField5;

    void Start()
    {
        // Assign the first input field
        inputField1.onSelect.AddListener((_) => numberPad.SetInputfield(inputField1));

        // Assign the second input field
        inputField2.onSelect.AddListener((_) => numberPad.SetInputfield(inputField2));
        inputField3.onSelect.AddListener((_) => numberPad.SetInputfield(inputField3));
        inputField4.onSelect.AddListener((_) => numberPad.SetInputfield(inputField4));
        inputField5.onSelect.AddListener((_) => numberPad.SetInputfield(inputField5));

    }
}
*/
using UnityEngine;
using TMPro;

public class ExampleUsage : MonoBehaviour
{
    [SerializeField] FlatNumberPad numberPad; // Reference to the FlatNumberPad script
    [SerializeField] TMP_InputField inputField1; // First input field
    [SerializeField] TMP_InputField inputField2; // Second input field
    [SerializeField] TMP_InputField inputField3; // Third input field
    [SerializeField] TMP_InputField inputField4; // Fourth input field
    [SerializeField] TMP_InputField inputField5; // Fifth input field

    TMP_InputField currentInputField; // Track the currently selected input field

    void Start()
    {
        // Set up listeners for each input field
        inputField1.onSelect.AddListener((_) => SetCurrentInputField(inputField1));
        inputField2.onSelect.AddListener((_) => SetCurrentInputField(inputField2));
        inputField3.onSelect.AddListener((_) => SetCurrentInputField(inputField3));
        inputField4.onSelect.AddListener((_) => SetCurrentInputField(inputField4));
        inputField5.onSelect.AddListener((_) => SetCurrentInputField(inputField5));
    }

    // Method to set the current input field
    void SetCurrentInputField(TMP_InputField inputField)
    {
        // Optional: Handle deselection logic for the previous input field
        if (currentInputField != null && currentInputField != inputField)
        {
            currentInputField.onDeselect.Invoke(""); // Optional: Invoke deselect logic
        }

        currentInputField = inputField; // Update the currently selected input field
        numberPad.SetInputfield(currentInputField); // Set the input field in numberPad
    }
}
