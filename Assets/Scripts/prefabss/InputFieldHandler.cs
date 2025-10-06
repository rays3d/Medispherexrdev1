using UnityEngine;

public class InputFieldHandler : MonoBehaviour
{
    public GameObject customKeyboard; // Assign your keyboard prefab here

    public void OnInputFieldClick()
    {
        customKeyboard.SetActive(true); // Show the keyboard
    }
}
