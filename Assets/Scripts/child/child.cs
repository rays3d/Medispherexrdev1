using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component attached to this object
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody component found on this object.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ModelPart"))
        {
            // Log collision details for debugging
            Debug.Log("Collision detected with: " + collision.gameObject.name);
            Debug.Log("Child Transform Before: " + transform.position);
            Debug.Log("Parent Transform: " + collision.transform.position);

            // Set the current object as a child of the collided object
            transform.SetParent(collision.transform);

            // Set the local position of the child object to zero to keep it in the same spot relative to the new parent
            transform.localPosition = Vector3.zero;

            // Set Rigidbody to kinematic
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Optionally, log the child transform after parenting
            Debug.Log("Child Transform After: " + transform.localPosition);
        }
    }
}
