using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FoldUnfoldController : MonoBehaviour
{
    public GameObject[] parts; // Array of parts that make up the model
    public GameObject foldAnchor; // Central anchor (an empty GameObject with Rigidbody)
    public bool isFolded = true; // State toggle: folded or unfolded

    private List<FixedJoint> joints = new List<FixedJoint>();
    private XRGrabInteractable foldAnchorGrabInteractable;

    void Start()
    {
        // Get XR Grab Interactable on foldAnchor
        foldAnchorGrabInteractable = foldAnchor.GetComponent<XRGrabInteractable>();
        if (foldAnchorGrabInteractable == null)
        {
            foldAnchorGrabInteractable = foldAnchor.AddComponent<XRGrabInteractable>();
        }

        FoldParts(); // Start in folded state
    }

    // Method to fold parts into one unit (attach all parts to the foldAnchor)
    public void FoldParts()
    {
        isFolded = true;

        // Attach all parts to the foldAnchor using FixedJoint so they move as one
        foreach (GameObject part in parts)
        {
            Rigidbody partRb = part.GetComponent<Rigidbody>();
            if (partRb == null)
            {
                partRb = part.AddComponent<Rigidbody>(); // Ensure all parts have Rigidbodies
            }

            // Create a FixedJoint to attach each part to the foldAnchor
            FixedJoint joint = part.AddComponent<FixedJoint>();
            joint.connectedBody = foldAnchor.GetComponent<Rigidbody>(); // Connect to the anchor
            joints.Add(joint);

            // Disable XR Grab Interactable on child parts
            XRGrabInteractable grabInteractable = part.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false; // Disable interaction on child parts
            }
        }

        // Enable XR Grab Interactable on foldAnchor (move entire model as one)
        if (foldAnchorGrabInteractable != null)
        {
            foldAnchorGrabInteractable.enabled = true; // Enable the fold anchor grabbing
        }
    }

    // Method to unfold parts (detach all parts from the foldAnchor)
    public void UnfoldParts()
    {
        isFolded = false;

        // Detach all parts from the anchor by removing the FixedJoints
        foreach (FixedJoint joint in joints)
        {
            Destroy(joint); // Remove the FixedJoint to make parts independent
        }
        joints.Clear();

        // Disable XR Grab Interactable on foldAnchor (not grabbable in unfolded state)
        if (foldAnchorGrabInteractable != null)
        {
            foldAnchorGrabInteractable.enabled = false; // Disable fold anchor grabbing
        }

        // Enable XR Grab Interactable on child parts (make each part grabbable independently)
        foreach (GameObject part in parts)
        {
            XRGrabInteractable grabInteractable = part.GetComponent<XRGrabInteractable>();
            if (grabInteractable == null)
            {
                grabInteractable = part.AddComponent<XRGrabInteractable>();
            }
            grabInteractable.enabled = true; // Enable interaction on child parts
        }
    }

    // Grabbing logic: in folded state, grab the whole model by moving the foldAnchor
    public void Grab(GameObject grabbedPart)
    {
        if (isFolded)
        {
            // In the folded state, move the foldAnchor (moves all connected parts)
            Debug.Log("Grabbing the entire folded model.");
            MoveObject(foldAnchor); // Move the whole group through the foldAnchor
        }
        else
        {
            // In the unfolded state, move just the grabbed part
            Debug.Log("Grabbing individual part: " + grabbedPart.name + " in unfolded state.");
            MoveObject(grabbedPart); // Move the part independently
        }
    }

    // Example method to move objects (implement movement logic here)
    private void MoveObject(GameObject obj)
    {
        // Example movement: Move the object to the mouse position (or VR controller position)
        obj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Replace this with your actual grab/movement logic based on your input system
    }
}
