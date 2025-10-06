using UnityEngine;
using TMPro;

public class AssignKeyboardOnSelect : MonoBehaviour
{
    public FlatNumberPad keyboard;

    void Start()
    {
        TMP_InputField field = GetComponent<TMP_InputField>();
        field.onSelect.AddListener((_) => keyboard.SetInputfield(field));
    }
}
