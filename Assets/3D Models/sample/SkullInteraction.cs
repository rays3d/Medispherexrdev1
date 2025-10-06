using UnityEngine;

public class SkullInteraction : MonoBehaviour
{
    public GameObject parentObject; // Reference to the parent object (skull)
    public GameObject[] parts; // Array of parts (3 parts)
    private bool isDetached = false; // Track whether parts are detached

    // Call this method to toggle parts from any button or event
    public void TogglePartsAttachment()
    {
        if (isDetached)
        {
            AttachParts();
        }
        else
        {
            DetachParts();
        }
        isDetached = !isDetached; // Toggle state
    }

    private void AttachParts()
    {
        foreach (GameObject part in parts)
        {
            // Attach part back to parent
            part.transform.SetParent(parentObject.transform);
            // Reset part position relative to parent
            part.transform.localPosition = Vector3.zero;

            // Ensure the collider is active
            Collider collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            // Ensure Rigidbody is properly configured
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Enable physics interaction
            }
        }
    }

    private void DetachParts()
    {
        foreach (GameObject part in parts)
        {
            // Detach part from parent
            part.transform.SetParent(null);
            // Optionally move parts to a new position
            part.transform.position = part.transform.position + Vector3.up * 2; // Adjust as needed

            // Ensure the collider is active
            Collider collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            // Ensure Rigidbody is properly configured
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Disable physics interaction if needed
            }
        }
    }
}

/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class SkullInteraction : MonoBehaviour
{
    public XRGrabInteractable skullGrabInteractable; // Reference to the whole skull
    public XRGrabInteractable[] partGrabInteractables; // References to individual parts

    public Button grabButton;       // UI Button to grab the whole model
    public Button grabPartButton;   // UI Button to grab individual parts

    private void Start()
    {
        // Set up listeners for the UI buttons
        if (grabButton != null)
        {
            grabButton.onClick.AddListener(OnGrabButtonPressed);
        }

        if (grabPartButton != null)
        {
            grabPartButton.onClick.AddListener(OnPartGrabButtonPressed);
        }
    }

    // Function to grab the whole skull
    public void OnGrabButtonPressed()
    {
        if (skullGrabInteractable != null)
        {
            // Optionally, trigger grabbing the whole skull or focus on it
            // Ensure the whole skull is set up for grabbing
            skullGrabInteractable.gameObject.SetActive(true);
        }
    }

    // Function to grab individual parts
    public void OnPartGrabButtonPressed()
    {
        foreach (var part in partGrabInteractables)
        {
            if (part != null)
            {
                // Optionally, trigger grabbing each part
                // Ensure each part is set up for grabbing
                part.gameObject.SetActive(true);
            }
        }
    }
}

*/