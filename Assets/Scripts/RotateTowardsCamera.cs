using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCamera : MonoBehaviour
{
    void Update()
    {
        Transform head = FindObjectOfType<XRRigMapper>().headTarget;
        transform.LookAt(head);
        transform.forward *= -1;
    }
}
