using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EditableInputfield : MonoBehaviour
{
    [SerializeField] FlatNumberPad flatNumberPad;
    void Start()
    {
        flatNumberPad.SetInputfield(GetComponent<TMP_InputField>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
