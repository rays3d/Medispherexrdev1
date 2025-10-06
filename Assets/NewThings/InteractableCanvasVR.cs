using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Add this if you're using XR Interaction Toolkit

public class InteractableCanvasVR : MonoBehaviour
{
    public Canvas childCanvas;
    [SerializeField] private float displayDuration = 3f; // How long the canvas stays visible

    // Tracking touch state
    private bool isTouched = false;

    private void Start()
    {
        // Make sure canvas is disabled at start
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(false);
        }
        else
        {
            // Try to find the canvas if not assigned
            childCanvas = GetComponentInChildren<Canvas>(true);
            if (childCanvas != null)
            {
                childCanvas.gameObject.SetActive(false);
            }
        }

        // Optional: If using XR Interaction Toolkit
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);

            // For initial touch
            grabInteractable.hoverEntered.AddListener(OnHovered);
        }
    }

    // For direct hand/controller collisions
    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Player") ||
             collision.gameObject.CompareTag("Controller")) &&
            !isTouched)
        {
            isTouched = true;
            ShowCanvas();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("Controller"))
        {
            isTouched = false;
        }
    }

    // For ray-based or direct interaction
    public void OnGrabbed(SelectEnterEventArgs args)
    {
        // Handle grab logic if needed
    }

    public void OnReleased(SelectExitEventArgs args)
    {
        // Handle release logic if needed
    }

    public void OnHovered(HoverEnterEventArgs args)
    {
        ShowCanvas();
    }

    // For mouse-based testing in Unity editor
    private void OnMouseDown()
    {
        ShowCanvas();
    }

    public void ShowCanvas()
    {
        if (childCanvas != null)
        {
            // Stop any previous coroutines to prevent conflicts
            StopAllCoroutines();

            // Show the canvas
            childCanvas.gameObject.SetActive(true);

            // Start the timer to hide it
            StartCoroutine(HideCanvasAfterDelay());
        }
    }

    private IEnumerator HideCanvasAfterDelay()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(displayDuration);

        // Hide the canvas
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(false);
        }
    }
}