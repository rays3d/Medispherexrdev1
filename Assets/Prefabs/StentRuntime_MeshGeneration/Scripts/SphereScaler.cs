using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// Handles XR ray interaction for selecting and scaling objects (spheres) 
/// using a UI slider with multiplayer synchronization.
/// </summary>
public class SphereScaler : MonoBehaviourPun
{
    #region Configuration Fields
    [Header("XR Input")]
    [SerializeField] private InputActionProperty selectAction;
    [SerializeField] private XRRayInteractor leftRayInteractor;
    [SerializeField] private XRRayInteractor rightRayInteractor;
    [SerializeField] private bool useBothHands = false;

    [Header("UI Elements")]
    [SerializeField] private Slider scaleSlider;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.green;
    #endregion

    #region Internal State
    private GameObject _hoveredSphere;
    private GameObject _selectedSphere;
    private Renderer _hoveredRenderer;
    private Renderer _selectedRenderer;
    private Vector3 _originalSelectedScale;
    private Dictionary<Renderer, Color> _originalColors = new Dictionary<Renderer, Color>();
    private PhotonView _selectedPhotonView;
    #endregion

    private void Awake()
    {
        if (scaleSlider != null)
        {
            scaleSlider.minValue = 0.5f;
            scaleSlider.maxValue = 2f;
            scaleSlider.value = 1f;
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        }
    }

    private void OnEnable()
    {
        selectAction.action.Enable();
        selectAction.action.performed += OnSelectPressed;
    }

    private void OnDisable()
    {
        selectAction.action.performed -= OnSelectPressed;
        selectAction.action.Disable();
    }

    private void Update()
    {
        HoverDetection();
    }

    /// <summary>
    /// Checks for raycast hits from the XR Interactors and updates the hover state
    /// </summary>
    private void HoverDetection()
    {
        GameObject hovered = null;

        if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitLeft))
            hovered = hitLeft.collider.gameObject;

        if (useBothHands && rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitRight))
            hovered = hitRight.collider.gameObject;

        // Reset color of the previously hovered object
        if (_hoveredRenderer != null && _hoveredSphere != _selectedSphere)
        {
            if (_originalColors.ContainsKey(_hoveredRenderer))
                _hoveredRenderer.material.color = _originalColors[_hoveredRenderer];
        }

        // Apply hover color to the currently hovered object
        if (hovered != null && hovered != _selectedSphere)
        {
            Renderer rend = hovered.GetComponent<Renderer>();
            if (rend != null)
            {
                if (!_originalColors.ContainsKey(rend))
                    _originalColors[rend] = rend.material.color;

                rend.material.color = hoverColor;
                _hoveredRenderer = rend;
                _hoveredSphere = hovered;
            }
        }
        else
        {
            _hoveredRenderer = null;
            _hoveredSphere = null;
        }
    }

    /// <summary>
    /// Called when the select input action is performed
    /// </summary>
    private void OnSelectPressed(InputAction.CallbackContext context)
    {
        GameObject selected = null;

        if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitLeft))
            selected = hitLeft.collider.gameObject;

        if (useBothHands && rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitRight))
            selected = hitRight.collider.gameObject;

        if (selected == null)
            return;

        Renderer rend = selected.GetComponent<Renderer>();
        if (rend == null)
            return;

        // Get PhotonView from parent (control points parent)
        PhotonView pv = selected.GetComponentInParent<PhotonView>();

        // --- Deselection Logic ---
        if (_selectedSphere == selected)
        {
            if (_originalColors.ContainsKey(rend))
                rend.material.color = _originalColors[rend];

            XRGrabInteractable grab = selected.GetComponentInParent<XRGrabInteractable>();
            if (grab != null)
                grab.enabled = true;

            _selectedSphere = null;
            _selectedRenderer = null;
            _selectedPhotonView = null;
            _originalSelectedScale = Vector3.one;

            if (scaleSlider != null)
                scaleSlider.value = 1f;

            return;
        }

        // --- Selection Logic ---

        // Deselect previous object
        if (_selectedSphere != null && _selectedRenderer != null && _originalColors.ContainsKey(_selectedRenderer))
            _selectedRenderer.material.color = _originalColors[_selectedRenderer];

        // Select new sphere
        _selectedSphere = selected;
        _selectedRenderer = rend;
        _selectedPhotonView = pv;

        if (rend != null && !_originalColors.ContainsKey(rend))
            _originalColors[rend] = rend.material.color;

        rend.material.color = selectedColor;

        _originalSelectedScale = _selectedSphere.transform.localScale;

        // Request ownership if networked
        if (_selectedPhotonView != null && !_selectedPhotonView.IsMine)
        {
            _selectedPhotonView.RequestOwnership();
        }

        XRGrabInteractable grabInteractable = _selectedSphere.GetComponentInParent<XRGrabInteractable>();
        if (grabInteractable != null)
            grabInteractable.enabled = false;

        if (scaleSlider != null)
            scaleSlider.value = 1f;
    }

    /// <summary>
    /// Listener function for the UI Slider's OnValueChanged event with network sync
    /// </summary>
    private void OnScaleChanged(float value)
    {
        if (_selectedSphere != null)
        {
            Vector3 newScale = _originalSelectedScale * value;
            _selectedSphere.transform.localScale = newScale;

            // Sync scale across network
            if (_selectedPhotonView != null && _selectedPhotonView.IsMine)
            {
                photonView.RPC(nameof(RPC_SyncScale), RpcTarget.Others, 
                    _selectedSphere.GetInstanceID(), value, _originalSelectedScale);
            }
        }
    }

    [PunRPC]
    private void RPC_SyncScale(int instanceID, float value, Vector3 originalScale)
    {
        // Find the GameObject by instance ID
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.GetInstanceID() == instanceID)
            {
                obj.transform.localScale = originalScale * value;
                break;
            }
        }
    }
}