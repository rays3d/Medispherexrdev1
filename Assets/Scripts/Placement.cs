using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Placement : MonoBehaviour
{

    private void Start()
    {
        XRSocketInteractor xRSocketInteractor = GetComponent<XRSocketInteractor>();
        xRSocketInteractor.selectEntered.AddListener(OnSelect);
    }

    private void OnSelect(SelectEnterEventArgs arg0)
    {
       /* if (arg0.interactableObject.transform.GetComponent<Tool>().isGrabbed) return;

        if (!arg0.interactableObject.colliders[0].transform.parent)
            arg0.interactableObject.colliders[0].transform.parent.parent = this.transform;
        else
        {
            arg0.interactableObject.colliders[0].transform.parent = this.transform;
        }
        Debug.Log("Doneaaa");*/

    }

    public void OnSelect()
    {
        this.transform.GetChild(0).localScale /= 2;
        Debug.Log("Done");

    }
}
