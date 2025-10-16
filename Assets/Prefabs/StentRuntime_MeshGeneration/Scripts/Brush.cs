using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; // Required for accessing XRNode and InputTracking
using UnityEngine.InputSystem; // Required for using the new Input System actions
using Photon.Pun;

/// <summary>
/// Manages the XR drawing functionality. It handles input for starting/ending a brush stroke,
/// continuous size adjustment, and limits the maximum length of a single stroke.
/// This script relies on the 'BrushStroke' component (not included here) to handle the
/// actual mesh generation of the stroke.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class Brush : MonoBehaviour
{
    // --- Editor Configuration ---
    #region Configuration Fields
    [Header("Drawing Setup")]
    [SerializeField] private GameObject _brushStrokePrefab = null; // Prefab containing the BrushStroke component

    private enum Hand { LeftHand, RightHand };
    [SerializeField] private Hand _hand = Hand.RightHand; // The hand/controller to track for drawing

    [Header("Brush Tip Transform")]
    [SerializeField] private Transform _brushTipTransform = null; // The exact world position/rotation for drawing
    [SerializeField] private Transform _brushTipSphere = null; // The visual indicator (sphere) that shows the current brush size

    [Header("XR Input System")]
    [SerializeField] private InputActionProperty drawAction; // Input for drawing (e.g., Trigger button)
    [SerializeField] private InputActionProperty sizeIncreaseAction; // Input to increase size (e.g., Primary button)
    [SerializeField] private InputActionProperty sizeDecreaseAction; // Input to decrease size (e.g., Secondary button)

    [Header("Size Adjustment Settings")]
    [SerializeField] private float minSize = 0.5f; // Minimum allowed size multiplier
    [SerializeField] private float maxSize = 3.0f; // Maximum allowed size multiplier
    [SerializeField] private float sizeAdjustSpeed = 1.0f; // Speed of size change per second
    [SerializeField] private float previewScaleMultiplier = 0.02f; // Scale factor applied to _brushTipSphere for visual preview

    [Header("Drawing Length Limit")]
    [SerializeField] private float maxDrawingLength = 5.0f; // Maximum length of a single stroke in meters
    [SerializeField] private bool limitDrawingLength = true; // Toggle to enable/disable length limiting
    #endregion

    // --- Private State Variables ---
    #region Internal State
    private Vector3 _handPosition; // Last recorded hand position
    private Quaternion _handRotation; // Last recorded hand rotation
    private BrushStroke _activeBrushStroke; // Reference to the currently active stroke object
    private float _currentDrawingSize = 1.0f; // The current size multiplier for the stroke
    private Vector3 _initialSphereScale; // Original local scale of the _brushTipSphere
    private float _currentStrokeLength = 0f; // Accumulated length of the current stroke
    private Vector3 _lastDrawPosition; // The position of the brush tip in the previous frame
    #endregion

    #region Networking Setup
    public PhotonView _photonView;
    #endregion

    // --- Input System Setup ---
    #region Input Setup
    private void OnEnable()
    {
        // Enable all necessary input actions when the script becomes active
        drawAction.action.Enable();
        sizeIncreaseAction.action.Enable();
        sizeDecreaseAction.action.Enable();
    }

    private void OnDisable()
    {
        // Disable all input actions when the script is disabled
        drawAction.action.Disable();
        sizeIncreaseAction.action.Disable();
        sizeDecreaseAction.action.Disable();
    }
    #endregion

    // --- Initialization and Update Loop ---
    #region Core Logic
    private void Start()
    {
        if(_photonView == null)
        {
            _photonView = GetComponent<PhotonView>();
        }
        // Store the initial scale of the brush tip sphere for relative scaling later
        if (_brushTipSphere != null)
        {
            _initialSphereScale = _brushTipSphere.localScale;
        }
    }

    private void Update()
    {
        if (!PhotonNetwork.IsConnected)
            return;
         
        // 1. Get Hand Pose: Determine which XRNode to use and get its latest pose data
        XRNode node = _hand == Hand.LeftHand ? XRNode.LeftHand : XRNode.RightHand;
        bool handIsTracking = UpdatePose(node, ref _handPosition, ref _handRotation);

        // 2. Read Input Values
        bool triggerPressed = drawAction.action.ReadValue<float>() > 0.1f; // Check for drawing input (e.g., trigger pull)
        bool increasePressed = sizeIncreaseAction.action.ReadValue<float>() > 0.1f;
        bool decreasePressed = sizeDecreaseAction.action.ReadValue<float>() > 0.1f;

        // Force stop drawing if controller tracking is lost
        if (!handIsTracking)
            triggerPressed = false;

        // 3. Handle Size Adjustment
        HandleSizeAdjustment(increasePressed, decreasePressed, triggerPressed);

        // 4. Update Brush Tip Visual
        UpdateBrushTipSphereSize();

        // 5. Determine Draw Position
        Vector3 drawPosition = _brushTipTransform != null ? _brushTipTransform.position : _handPosition;
        Quaternion drawRotation = _brushTipTransform != null ? _brushTipTransform.rotation : _handRotation;

        // 6. Check for Forced Stroke End (Length Limit)
        bool forcedEnd = false;
        if (limitDrawingLength && _activeBrushStroke != null && _currentStrokeLength >= maxDrawingLength)
        {
            forcedEnd = true;
        }

        // 7. Start New Stroke
        if (triggerPressed && _activeBrushStroke == null)
        {
            // Instantiate the prefab and get the BrushStroke component
            GameObject brushStrokeGO = PhotonNetwork.Instantiate("Tools/BrushStroke", Vector3.zero, Quaternion.identity);

            _activeBrushStroke = brushStrokeGO.GetComponent<BrushStroke>();
            
            // Apply the current drawing size to the new stroke
            SetBrushStrokeSize(_activeBrushStroke, _currentDrawingSize);
            
            // Tell the stroke to start at the current tip point
            _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(drawPosition, drawRotation);
            
            // Reset and initialize length tracking
            _currentStrokeLength = 0f;
            _lastDrawPosition = drawPosition;
        }

        // 8. Continue Stroke
        if (triggerPressed && _activeBrushStroke != null && !forcedEnd)
        {
            // Calculate distance moved and update total stroke length
            float distanceMoved = Vector3.Distance(drawPosition, _lastDrawPosition);
            _currentStrokeLength += distanceMoved;
            _lastDrawPosition = drawPosition;

            // Update the brush stroke with the new point
            _activeBrushStroke.MoveBrushTipToPoint(drawPosition, drawRotation);

            // Check if movement just crossed the limit
            if (limitDrawingLength && _currentStrokeLength >= maxDrawingLength)
            {
                forcedEnd = true;
            }
        }

        // 9. End Stroke
        if ((!triggerPressed || forcedEnd) && _activeBrushStroke != null)
        {
            // Finalize the brush stroke
            _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(drawPosition, drawRotation);
            
            // Clear the active reference and reset length
            _activeBrushStroke = null;
            _currentStrokeLength = 0f;
        }
    }
    #endregion

    // --- Helper Methods ---
    #region Private Helpers
    /// <summary>
    /// Adjusts the current drawing size based on input and time. Size adjustment is paused while drawing.
    /// </summary>
    private void HandleSizeAdjustment(bool increase, bool decrease, bool isDrawing)
    {
        // Only allow size adjustment when the user is NOT actively drawing
        if (isDrawing) return;

        if (increase)
        {
            // Increase size, clamped between min and max
            _currentDrawingSize += sizeAdjustSpeed * Time.deltaTime;
            _currentDrawingSize = Mathf.Clamp(_currentDrawingSize, minSize, maxSize);
        }
        else if (decrease)
        {
            // Decrease size, clamped between min and max
            _currentDrawingSize -= sizeAdjustSpeed * Time.deltaTime;
            _currentDrawingSize = Mathf.Clamp(_currentDrawingSize, minSize, maxSize);
        }
    }

    /// <summary>
    /// Updates the local scale of the visual brush tip sphere to reflect the current drawing size.
    /// </summary>
    private void UpdateBrushTipSphereSize()
    {
        if (_brushTipSphere == null) return;

        // Calculate the scale factor by multiplying current size by a visual multiplier
        float sizeScale = _currentDrawingSize * previewScaleMultiplier;
        
        // Apply the scale factor to the original scale
        _brushTipSphere.localScale = _initialSphereScale * sizeScale;
    }

    /// <summary>
    /// Finds the BrushStrokeMesh component on the stroke prefab and sets its size multiplier.
    /// </summary>
    private void SetBrushStrokeSize(BrushStroke brushStroke, float size)
    {
        if (brushStroke == null) return;

        // Assumes BrushStrokeMesh is a child/component of the BrushStroke
        BrushStrokeMesh mesh = brushStroke.GetComponentInChildren<BrushStrokeMesh>();
        if (mesh != null)
        {
            // Set the size multiplier, which scales the geometry internally
            mesh.sizeMultiplier = size;
        }
        // Note: The outer transform scale of the brush stroke is NOT changed here.
    }

    /// <summary>
    /// Attempts to retrieve the position and rotation data for a specified XRNode.
    /// This uses the deprecated InputTracking API, which is common in older XR templates.
    /// </summary>
    /// <returns>True if the node's position could be successfully retrieved (i.e., tracking).</returns>
    private static bool UpdatePose(XRNode node, ref Vector3 position, ref Quaternion rotation)
    {
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == node)
            {
                Vector3 nodePosition;
                Quaternion nodeRotation;
                bool gotPosition = nodeState.TryGetPosition(out nodePosition);
                bool gotRotation = nodeState.TryGetRotation(out nodeRotation);

                if (gotPosition) position = nodePosition;
                if (gotRotation) rotation = nodeRotation;

                // Return true if position data was successfully retrieved (indicating tracking)
                return gotPosition;
            }
        }
        return false;
    }
    #endregion

    // --- Public Accessors ---
    #region Public API
    public float GetCurrentDrawingSize()
    {
        return _currentDrawingSize;
    }

    public void SetDrawingSize(float size)
    {
        // Public method to set the size externally, ensuring it remains clamped
        _currentDrawingSize = Mathf.Clamp(size, minSize, maxSize);
    }

    public float GetCurrentStrokeLength()
    {
        return _currentStrokeLength;
    }

    public float GetMaxDrawingLength()
    {
        return maxDrawingLength;
    }
    #endregion
}