using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;


/// <summary>
/// Handles XR ray interaction for selecting and scaling objects (spheres) 
/// using a UI slider. It provides visual feedback for hover and selection states.
/// </summary>
public class SphereScaler : MonoBehaviour
{
    // --- Editor-Visible Configuration ---
    #region Configuration Fields
    [Header("XR Input")]
    [SerializeField] private InputActionProperty selectAction; // Input action for selection (e.g., trigger press)
    [SerializeField] private XRRayInteractor leftRayInteractor; // Ray interactor for the left hand
    [SerializeField] private XRRayInteractor rightRayInteractor; // Ray interactor for the right hand
    [SerializeField] private bool useBothHands = false; // Toggle to check for hits from both ray interactors

    [Header("UI Elements")]
    [SerializeField] private Slider scaleSlider; // UI Slider to control the scaling factor
    [SerializeField] private Color hoverColor = Color.yellow; // Color to apply when an object is hovered
    [SerializeField] private Color selectedColor = Color.green; // Color to apply when an object is selected
    #endregion

    // --- State Variables ---
    #region Internal State
    private GameObject _hoveredSphere; // The object currently being hovered by a ray
    private GameObject _selectedSphere; // The object currently selected for scaling
    private Renderer _hoveredRenderer; // Renderer component of the hovered object
    private Renderer _selectedRenderer; // Renderer component of the selected object
    private Vector3 _originalSelectedScale; // The scale of the selected object when it was first selected

    // Dictionary to store the original color of any object whose color was changed by this script
    private Dictionary<Renderer, Color> _originalColors = new Dictionary<Renderer, Color>();
    #endregion

    // --- Unity Lifecycle Methods ---
    #region Unity Lifecycle
    private void Awake()
    {
        // Initialize the slider settings and listener
        if (scaleSlider != null)
        {
            scaleSlider.minValue = 0.5f; // Minimum scale factor
            scaleSlider.maxValue = 2f;  // Maximum scale factor
            scaleSlider.value = 1f;     // Default scale factor (original size)
            // Register the scaling method to the slider's value change event
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        }
    }

    private void OnEnable()
    {
        // Enable the input action and subscribe to its 'performed' event
        selectAction.action.Enable();
        selectAction.action.performed += OnSelectPressed;
    }

    private void OnDisable()
    {
        // Unsubscribe from the input action and disable it
        selectAction.action.performed -= OnSelectPressed;
        selectAction.action.Disable();
    }

    private void Update()
    {
        // Continuously check for raycast hits to handle hover visual feedback
        HoverDetection();
    }
    #endregion

    // --- Interaction Logic ---
    #region Interaction Methods

    /// <summary>
    /// Checks for raycast hits from the XR Interactors and updates the hover state
    /// and visual feedback (color). Prioritizes the right interactor if both are used.
    /// </summary>
    private void HoverDetection()
    {
        GameObject hovered = null;

        // Check Left Ray Interactor hit
        if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitLeft))
            hovered = hitLeft.collider.gameObject;

        // If using both hands, check Right Ray Interactor and override if it hits something
        if (useBothHands && rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitRight))
            hovered = hitRight.collider.gameObject;

        // 1. Reset color of the previously hovered object (if it wasn't the selected object)
        if (_hoveredRenderer != null && _hoveredSphere != _selectedSphere)
        {
            if (_originalColors.ContainsKey(_hoveredRenderer))
                _hoveredRenderer.material.color = _originalColors[_hoveredRenderer];
        }

        // 2. Apply hover color to the currently hovered object (if it's not the selected object)
        if (hovered != null && hovered != _selectedSphere)
        {
            Renderer rend = hovered.GetComponent<Renderer>();
            if (rend != null)
            {
                // Save original color if not already recorded
                if (!_originalColors.ContainsKey(rend))
                    _originalColors[rend] = rend.material.color;

                rend.material.color = hoverColor;
                _hoveredRenderer = rend;
                _hoveredSphere = hovered;
            }
        }
        else
        {
            // No object is currently hovered
            _hoveredRenderer = null;
            _hoveredSphere = null;
        }
    }

    /// <summary>
    /// Called when the select input action is performed (e.g., trigger pressed). 
    /// Handles selecting and deselecting objects.
    /// </summary>
    private void OnSelectPressed(InputAction.CallbackContext context)
    {
        GameObject selected = null;

        // Check for hit from the left interactor
        if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitLeft))
            selected = hitLeft.collider.gameObject;

        // Check for hit from the right interactor (overrides left if both hit)
        if (useBothHands && rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitRight))
            selected = hitRight.collider.gameObject;

        // Exit if no object was hit by the ray
        if (selected == null)
            return;

        Renderer rend = selected.GetComponent<Renderer>();
        if (rend == null)
            return; // Only process objects with a renderer

        // --- Deselection Logic ---
        if (_selectedSphere == selected)
        {
            // Restore original color
            if (_originalColors.ContainsKey(rend))
                rend.material.color = _originalColors[rend];

            // Re-enable XR Grab Interactable on the selected object/parent (assuming it was disabled for scaling)
            XRGrabInteractable grab = selected.GetComponentInParent<XRGrabInteractable>();
            if (grab != null)
                grab.enabled = true; // Re-enable grab interaction

            // Reset state
            _selectedSphere = null;
            _selectedRenderer = null;
            _originalSelectedScale = Vector3.one;

            // Reset slider value
            if (scaleSlider != null)
                scaleSlider.value = 1f;

            return;
        }

        // --- Selection Logic ---

        // 1. Deselect previous object if one existed
        if (_selectedSphere != null && _selectedRenderer != null && _originalColors.ContainsKey(_selectedRenderer))
            _selectedRenderer.material.color = _originalColors[_selectedRenderer];

        // 2. Select new sphere and update state
        _selectedSphere = selected;
        _selectedRenderer = rend;

        // Save original color of the new selection if needed
        if (rend != null && !_originalColors.ContainsKey(rend))
            _originalColors[rend] = rend.material.color;

        // Apply selected color
        rend.material.color = selectedColor;

        // Store the current scale as the base scale for scaling operations
        _originalSelectedScale = _selectedSphere.transform.localScale;

        // Temporarily disable XR Grab Interactable to prevent movement while scaling is active
        XRGrabInteractable grabInteractable = _selectedSphere.GetComponentInParent<XRGrabInteractable>();
        if (grabInteractable != null)
            grabInteractable.enabled = false;

        // Reset slider to the default (1.0) for the newly selected object
        if (scaleSlider != null)
            scaleSlider.value = 1f;
    }

    /// <summary>
    /// Listener function for the UI Slider's OnValueChanged event.
    /// Applies the scale factor to the currently selected object.
    /// </summary>
    /// <param name="value">The new scale multiplier value from the slider.</param>
    private void OnScaleChanged(float value)
    {
        if (_selectedSphere != null)
            // Apply the new scale factor (value) to the original scale
            _selectedSphere.transform.localScale = _originalSelectedScale * value;
    }
    #endregion
}