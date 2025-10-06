using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticManager : MonoBehaviourSingleton<HapticManager>
{

    [SerializeField] XRBaseController leftController;
    [SerializeField] XRBaseController rightController;
    public void ActivateHapticLeft(float inten, float dur)
    {
        if (inten > 0)
        {
            leftController.SendHapticImpulse(inten, dur);
        }
    }
    public void ActivateHapticRight(float inten, float dur)
    {
        if (inten > 0)
        {
            rightController.SendHapticImpulse(inten, dur);
        }
    }
}
