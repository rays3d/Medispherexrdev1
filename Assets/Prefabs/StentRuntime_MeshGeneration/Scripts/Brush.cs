using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon; // Contains the Hashtable class

/// <summary>
/// A serializable class to hold all necessary data for a persistent brush stroke.
/// Used for saving and loading strokes via JSON serialization (in Photon Room Properties).
/// </summary>
[System.Serializable]
public class StrokeData
{
    public string strokeId;
    public List<Vector3> positions;
    public List<Quaternion> rotations;
    public float size;
    public string parentBrushName;
}

/// <summary>
/// The main Brush component responsible for handling VR input,
/// starting, continuing, and ending brush strokes, synchronizing state
/// across the network using Photon PUN, and managing stroke persistence.
/// </summary>
public class Brush : MonoBehaviour, IPunObservable
{
    #region Configuration Fields
    /// <summary>
    /// Configuration settings for the drawing functionality.
    /// </summary>
    [Header("Drawing Setup")]
    [Tooltip("The prefab instantiated for a new brush stroke.")]
    [SerializeField] private GameObject _brushStrokePrefab = null;
    
    private enum Hand { LeftHand, RightHand };
    [Tooltip("Specifies which VR controller hand this brush corresponds to.")]
    [SerializeField] private Hand _hand = Hand.RightHand;

    /// <summary>
    /// Transforms used to determine where the drawing originates.
    /// </summary>
    [Header("Brush Tip Transform")]
    [Tooltip("The actual point where the stroke originates (e.g., tip of the physical brush model).")]
    [SerializeField] private Transform _brushTipTransform = null;
    [Tooltip("The preview sphere that visually represents the current drawing size.")]
    [SerializeField] private Transform _brushTipSphere = null;

    /// <summary>
    /// Settings for the new Unity XR Input System.
    /// </summary>
    [Header("XR Input System")]
    [Tooltip("The action used to trigger drawing (e.g., trigger button).")]
    [SerializeField] private InputActionProperty drawAction;
    [Tooltip("The action used to increase the brush size.")]
    [SerializeField] private InputActionProperty sizeIncreaseAction;
    [Tooltip("The action used to decrease the brush size.")]
    [SerializeField] private InputActionProperty sizeDecreaseAction;

    /// <summary>
    /// Settings for brush size modification.
    /// </summary>
    [Header("Size Adjustment Settings")]
    [Tooltip("The minimum allowed size for drawing.")]
    [SerializeField] private float minSize = 0.5f;
    [Tooltip("The maximum allowed size for drawing.")]
    [SerializeField] private float maxSize = 3.0f;
    [Tooltip("The speed at which the size changes when adjusting.")]
    [SerializeField] private float sizeAdjustSpeed = 1.0f;
    [Tooltip("Multiplier to scale the brush tip sphere for preview.")]
    [SerializeField] private float previewScaleMultiplier = 0.02f;

    /// <summary>
    /// Settings for limiting the stroke length.
    /// </summary>
    [Header("Drawing Length Limit")]
    [Tooltip("The maximum allowed length for a single continuous stroke.")]
    [SerializeField] private float maxDrawingLength = 5.0f;
    [Tooltip("Whether to enforce the drawing length limit.")]
    [SerializeField] private bool limitDrawingLength = true;
    #endregion

    //-------------------------------------------------------------------------
    
    #region Internal State
    private Vector3 _handPosition;
    private Quaternion _handRotation;
    private BrushStroke _activeBrushStroke; // The stroke currently being drawn.
    private float _currentDrawingSize = 1.0f;
    private Vector3 _initialSphereScale;
    private float _currentStrokeLength = 0f;
    private Vector3 _lastDrawPosition;
    #endregion

    //-------------------------------------------------------------------------

    #region Multiplayer Variables
    private PhotonView photonView;
    private string brushID; // Unique ID for persistence/deletion tracking.
    private bool isGrabbed = false; // State for grabbing/holding (synced via OnPhotonSerializeView).
    
    // Temporary lists to cache stroke data before saving it to room properties.
    private string currentStrokeId;
    private List<Vector3> currentStrokePositions = new List<Vector3>();
    private List<Quaternion> currentStrokeRotations = new List<Quaternion>();

    // Keys for Photon Room Custom Properties
    private const string DeletedBrushesKey = "DeletedBrushes";
    private const string PersistentStrokesKey = "PersistentStrokes";
    private const string DeletedStrokesKey = "DeletedStrokes";
    #endregion

    //-------------------------------------------------------------------------

    /// <summary>
    /// Initializes PhotonView and sets a unique ID for the brush, 
    /// using InstantiationData if provided by the instantiator.
    /// </summary>
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.InstantiationData != null && photonView.InstantiationData.Length > 0)
        {
            brushID = photonView.InstantiationData[0] as string;
        }
        else
        {
            // Fallback ID if instantiated without data.
            brushID = "BRUSH_" + photonView.ViewID;
        }
    }

    /// <summary>
    /// Runs initial setup, checks for prior deletion, and starts loading persistent strokes.
    /// </summary>
    private void Start()
    {
        // Check if this brush instance was marked for deletion by any client.
        if (IsBrushDeleted(brushID))
        {
            Destroy(gameObject);
            return;
        }

        if (_brushTipSphere != null)
        {
            _initialSphereScale = _brushTipSphere.localScale;
        }

        // Load existing strokes, especially important for late joiners.
        StartCoroutine(LoadExistingStrokes());
    }

    /// <summary>
    /// Delayed coroutine to allow time for the client to fully join the room
    /// and receive the current room custom properties before attempting to load strokes.
    /// </summary>
    private IEnumerator LoadExistingStrokes()
    {
        // Wait briefly to ensure Photon properties are available.
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.CurrentRoom != null && 
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PersistentStrokesKey))
        {
            string strokesData = PhotonNetwork.CurrentRoom.CustomProperties[PersistentStrokesKey] as string;
            if (!string.IsNullOrEmpty(strokesData))
            {
                LoadStrokesFromData(strokesData);
            }
        }
    }

    //-------------------------------------------------------------------------

    #region Input Setup
    /// <summary>
    /// Enables the XR Input Actions when the component becomes active.
    /// </summary>
    private void OnEnable()
    {
        drawAction.action.Enable();
        sizeIncreaseAction.action.Enable();
        sizeDecreaseAction.action.Enable();
    }

    /// <summary>
    /// Disables the XR Input Actions when the component is deactivated.
    /// </summary>
    private void OnDisable()
    {
        drawAction.action.Disable();
        sizeIncreaseAction.action.Disable();
        sizeDecreaseAction.action.Disable();
    }
    #endregion

    //-------------------------------------------------------------------------

    /// <summary>
    /// Main update loop to check input, track hand pose, manage brush size, and draw strokes.
    /// </summary>
    private void Update()
    {
        // 1. Get XR Hand Pose
        XRNode node = _hand == Hand.LeftHand ? XRNode.LeftHand : XRNode.RightHand;
        bool handIsTracking = UpdatePose(node, ref _handPosition, ref _handRotation);

        // 2. Read Input Actions
        bool triggerPressed = drawAction.action.ReadValue<float>() > 0.1f;
        bool increasePressed = sizeIncreaseAction.action.ReadValue<float>() > 0.1f;
        bool decreasePressed = sizeDecreaseAction.action.ReadValue<float>() > 0.1f;

        // Prevent drawing if the hand pose isn't currently tracked.
        if (!handIsTracking)
            triggerPressed = false;

        // 3. Handle Brush Size
        HandleSizeAdjustment(increasePressed, decreasePressed, triggerPressed);
        UpdateBrushTipSphereSize();

        // Determine the drawing location (use tip transform if available, otherwise hand pose).
        Vector3 drawPosition = _brushTipTransform != null ? _brushTipTransform.position : _handPosition;
        Quaternion drawRotation = _brushTipTransform != null ? _brushTipTransform.rotation : _handRotation;

        // 4. Check for length-based termination
        bool forcedEnd = false;
        if (limitDrawingLength && _activeBrushStroke != null && _currentStrokeLength >= maxDrawingLength)
        {
            forcedEnd = true;
        }

        // Only the owner of the PhotonView is allowed to initiate RPCs for drawing.
        if (!photonView.IsMine)
            return;

        // 5. Start New Stroke
        if (triggerPressed && _activeBrushStroke == null)
        {
            // Generate a unique ID for the new stroke.
            currentStrokeId = photonView.ViewID + "_" + System.DateTime.Now.Ticks;
            currentStrokePositions.Clear();
            currentStrokeRotations.Clear();
            
            // Start the stroke on all clients via RPC.
            photonView.RPC(nameof(RPC_StartStroke), RpcTarget.All, drawPosition, drawRotation, currentStrokeId, _currentDrawingSize);
            
            // Cache the initial point locally for persistence.
            currentStrokePositions.Add(drawPosition);
            currentStrokeRotations.Add(drawRotation);
        }

        // 6. Continue Stroke
        if (triggerPressed && _activeBrushStroke != null && !forcedEnd)
        {
            // Update stroke length tracking.
            float distanceMoved = Vector3.Distance(drawPosition, _lastDrawPosition);
            _currentStrokeLength += distanceMoved;
            _lastDrawPosition = drawPosition;

            // Continue the stroke on all clients via RPC.
            photonView.RPC(nameof(RPC_ContinueStroke), RpcTarget.All, drawPosition, drawRotation);
            
            // Cache the current point locally for persistence.
            currentStrokePositions.Add(drawPosition);
            currentStrokeRotations.Add(drawRotation);

            // Re-check for forced end after movement.
            if (limitDrawingLength && _currentStrokeLength >= maxDrawingLength)
            {
                forcedEnd = true;
            }
        }

        // 7. End Stroke
        if ((!triggerPressed || forcedEnd) && _activeBrushStroke != null)
        {
            // End the stroke on all clients via RPC.
            photonView.RPC(nameof(RPC_EndStroke), RpcTarget.All, drawPosition, drawRotation);
            
            // Cache the final point locally for persistence.
            currentStrokePositions.Add(drawPosition);
            currentStrokeRotations.Add(drawRotation);
            
            // Save the completed stroke for late joiners (only the owner performs the save).
            if (photonView.IsMine)
            {
                SaveCurrentStroke();
            }
            
            // Reset local state.
            _activeBrushStroke = null;
            _currentStrokeLength = 0f;
            currentStrokePositions.Clear();
            currentStrokeRotations.Clear();
        }
    }

    //-------------------------------------------------------------------------

    #region RPC Methods
    /// <summary>
    /// [PunRPC] Called on all clients to begin a new brush stroke.
    /// </summary>
    /// <param name="position">The starting position of the stroke.</param>
    /// <param name="rotation">The starting rotation of the stroke.</param>
    /// <param name="strokeId">The unique ID for the stroke.</param>
    /// <param name="size">The size of the brush stroke.</param>
    [PunRPC]
    private void RPC_StartStroke(Vector3 position, Quaternion rotation, string strokeId, float size)
    {
        GameObject brushStrokeGO = Instantiate(_brushStrokePrefab);
        _activeBrushStroke = brushStrokeGO.GetComponent<BrushStroke>();

        // Add a tracker component to identify this stroke later (e.g., for deletion).
        var tracker = brushStrokeGO.AddComponent<StrokeTracker>();
        tracker.strokeId = strokeId;

        SetBrushStrokeSize(_activeBrushStroke, size);
        _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(position, rotation);

        // Initialize tracking variables.
        _currentStrokeLength = 0f;
        _lastDrawPosition = position;
    }

    /// <summary>
    /// [PunRPC] Called on all clients to extend the current brush stroke.
    /// </summary>
    /// <param name="position">The new position of the brush tip.</param>
    /// <param name="rotation">The new rotation of the brush tip.</param>
    [PunRPC]
    private void RPC_ContinueStroke(Vector3 position, Quaternion rotation)
    {
        if (_activeBrushStroke != null)
        {
            _activeBrushStroke.MoveBrushTipToPoint(position, rotation);
        }
    }

    /// <summary>
    /// [PunRPC] Called on all clients to finalize and end the brush stroke.
    /// </summary>
    /// <param name="position">The final position of the brush tip.</param>
    /// <param name="rotation">The final rotation of the brush tip.</param>
    [PunRPC]
    private void RPC_EndStroke(Vector3 position, Quaternion rotation)
    {
        if (_activeBrushStroke != null)
        {
            _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(position, rotation);
            _activeBrushStroke = null;
            _currentStrokeLength = 0f;
        }
    }

    /// <summary>
    /// [PunRPC] Called on non-owning clients to sync the brush size.
    /// </summary>
    /// <param name="size">The current drawing size.</param>
    [PunRPC]
    private void RPC_SyncSize(float size)
    {
        _currentDrawingSize = size;
    }

    /// <summary>
    /// [PunRPC] Called on all clients to destroy this brush object.
    /// </summary>
    [PunRPC]
    private void RPC_DeleteBrush()
    {
        Destroy(gameObject);
    }
    #endregion

    //-------------------------------------------------------------------------

    #region Persistence Methods
    /// <summary>
    /// Packages the current stroke data into a serializable object and saves it.
    /// This method is only called by the stroke's owner.
    /// </summary>
    private void SaveCurrentStroke()
    {
        if (currentStrokePositions.Count == 0) return;

        StrokeData data = new StrokeData
        {
            strokeId = currentStrokeId,
            positions = new List<Vector3>(currentStrokePositions),
            rotations = new List<Quaternion>(currentStrokeRotations),
            size = _currentDrawingSize,
            parentBrushName = gameObject.name
        };

        SaveStrokeToRoomProperties(data);
    }

    /// <summary>
    /// Adds a new stroke to the persistent list and updates Photon Room Custom Properties.
    /// Implements a simple limit to prevent the property data from growing indefinitely.
    /// </summary>
    /// <param name="data">The stroke data to save.</param>
    private void SaveStrokeToRoomProperties(StrokeData data)
    {
        List<StrokeData> allStrokes = GetAllStrokesFromRoom();
        allStrokes.Add(data);

        // Keep a rolling limit on the number of strokes to save (e.g., last 50).
        if (allStrokes.Count > 50)
        {
            allStrokes.RemoveRange(0, allStrokes.Count - 50);
        }

        string serializedData = SerializeStrokes(allStrokes);

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            [PersistentStrokesKey] = serializedData
        };

        // Update the room properties across the network.
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    /// <summary>
    /// Retrieves and deserializes the list of all persistent strokes from the room properties.
    /// </summary>
    /// <returns>A list of all persisted StrokeData objects.</returns>
    private List<StrokeData> GetAllStrokesFromRoom()
    {
        List<StrokeData> strokes = new List<StrokeData>();

        if (PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PersistentStrokesKey))
        {
            string data = PhotonNetwork.CurrentRoom.CustomProperties[PersistentStrokesKey] as string;
            if (!string.IsNullOrEmpty(data))
            {
                strokes = DeserializeStrokes(data);
            }
        }

        return strokes;
    }

    /// <summary>
    /// Deserializes the full stroke data string and creates game objects for each valid stroke.
    /// This runs for new clients joining the room.
    /// </summary>
    /// <param name="data">The JSON string containing all persistent strokes.</param>
    private void LoadStrokesFromData(string data)
    {
        List<StrokeData> strokes = DeserializeStrokes(data);
        HashSet<string> deletedStrokes = GetDeletedStrokeIds();

        foreach (StrokeData stroke in strokes)
        {
            // Only create the stroke if it hasn't been marked as deleted by a client.
            if (!deletedStrokes.Contains(stroke.strokeId))
            {
                CreatePersistedStroke(stroke);
            }
        }
    }

    /// <summary>
    /// Instantiates and rebuilds a single brush stroke based on stored data.
    /// </summary>
    /// <param name="data">The data containing the stroke's points and properties.</param>
    private void CreatePersistedStroke(StrokeData data)
    {
        if (data.positions == null || data.positions.Count == 0) return;

        GameObject brushStrokeGO = Instantiate(_brushStrokePrefab);
        BrushStroke brushStroke = brushStrokeGO.GetComponent<BrushStroke>();

        var tracker = brushStrokeGO.AddComponent<StrokeTracker>();
        tracker.strokeId = data.strokeId;

        SetBrushStrokeSize(brushStroke, data.size);

        // Replay the stroke by iterating through all saved points.
        for (int i = 0; i < data.positions.Count; i++)
        {
            if (i == 0)
            {
                brushStroke.BeginBrushStrokeWithBrushTipPoint(data.positions[i], data.rotations[i]);
            }
            else if (i == data.positions.Count - 1)
            {
                brushStroke.EndBrushStrokeWithBrushTipPoint(data.positions[i], data.rotations[i]);
            }
            else
            {
                brushStroke.MoveBrushTipToPoint(data.positions[i], data.rotations[i]);
            }
        }
    }

    /// <summary>
    /// Static method to mark a stroke ID as deleted in the Photon Room Properties.
    /// This prevents the stroke from being loaded by late joiners.
    /// </summary>
    /// <param name="strokeId">The unique ID of the stroke to mark.</param>
    public static void MarkStrokeAsDeleted(string strokeId)
    {
        if (PhotonNetwork.CurrentRoom == null || string.IsNullOrEmpty(strokeId)) return;

        HashSet<string> deletedStrokes = GetDeletedStrokeIds();
        if (!deletedStrokes.Contains(strokeId))
        {
            deletedStrokes.Add(strokeId);

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                // Joins the set of deleted IDs into a single comma-separated string.
                [DeletedStrokesKey] = string.Join(",", deletedStrokes)
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    /// <summary>
    /// Static method to retrieve the set of all stroke IDs marked for deletion.
    /// </summary>
    /// <returns>A HashSet of deleted stroke IDs.</returns>
    private static HashSet<string> GetDeletedStrokeIds()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties.TryGetValue(DeletedStrokesKey, out object raw) == true)
        {
            string rawStr = raw as string;
            if (!string.IsNullOrEmpty(rawStr))
                return new HashSet<string>(rawStr.Split(','));
        }
        return new HashSet<string>();
    }

    /// <summary>
    /// Converts a list of StrokeData into a JSON string for storage in Room Properties.
    /// </summary>
    private string SerializeStrokes(List<StrokeData> strokes)
    {
        try
        {
            // Use a wrapper class for array serialization with JsonUtility.
            return JsonUtility.ToJson(new StrokeList { strokes = strokes.ToArray() });
        }
        catch
        {
            Debug.LogError("Failed to serialize stroke data.");
            return "";
        }
    }

    /// <summary>
    /// Converts a JSON string back into a list of StrokeData objects.
    /// </summary>
    private List<StrokeData> DeserializeStrokes(string data)
    {
        try
        {
            StrokeList list = JsonUtility.FromJson<StrokeList>(data);
            if (list?.strokes != null)
            {
                return new List<StrokeData>(list.strokes);
            }
            return new List<StrokeData>();
        }
        catch
        {
            Debug.LogError("Failed to deserialize stroke data.");
            return new List<StrokeData>();
        }
    }

    /// <summary>
    /// A simple wrapper class required for Unity's JsonUtility to handle array serialization at the root level.
    /// </summary>
    [System.Serializable]
    private class StrokeList
    {
        public StrokeData[] strokes;
    }
    #endregion

    //-------------------------------------------------------------------------

    #region Deletion Management
    /// <summary>
    /// Marks this specific brush object ID as deleted in the Photon Room Properties.
    /// </summary>
    /// <param name="id">The unique ID of the brush.</param>
    private void MarkBrushDeleted(string id)
    {
        HashSet<string> deleted = GetDeletedBrushIds();
        if (!deleted.Contains(id))
        {
            deleted.Add(id);
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                [DeletedBrushesKey] = string.Join(",", deleted)
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    /// <summary>
    /// Checks if this brush has been marked as deleted in the Room Properties.
    /// </summary>
    /// <param name="id">The unique ID of the brush.</param>
    /// <returns>True if the brush ID is in the deleted list.</returns>
    private bool IsBrushDeleted(string id)
    {
        return GetDeletedBrushIds().Contains(id);
    }

    /// <summary>
    /// Retrieves the set of all brush IDs marked for deletion.
    /// </summary>
    /// <returns>A HashSet of deleted brush IDs.</returns>
    private HashSet<string> GetDeletedBrushIds()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties.TryGetValue(DeletedBrushesKey, out object raw) == true)
        {
            string rawStr = raw as string;
            if (!string.IsNullOrEmpty(rawStr))
                return new HashSet<string>(rawStr.Split(','));
        }
        return new HashSet<string>();
    }
    #endregion

    //-------------------------------------------------------------------------

    #region Helper Methods
    /// <summary>
    /// Handles continuous adjustment of the brush size based on input actions.
    /// The size is clamped within min/max limits and synced across the network.
    /// </summary>
    private void HandleSizeAdjustment(bool increase, bool decrease, bool isDrawing)
    {
        // Don't change size while actively drawing to prevent input conflicts.
        if (isDrawing) return;

        float oldSize = _currentDrawingSize;

        if (increase)
        {
            _currentDrawingSize += sizeAdjustSpeed * Time.deltaTime;
        }
        else if (decrease)
        {
            _currentDrawingSize -= sizeAdjustSpeed * Time.deltaTime;
        }

        _currentDrawingSize = Mathf.Clamp(_currentDrawingSize, minSize, maxSize);

        // Only sync if the size actually changed.
        if (photonView.IsMine && oldSize != _currentDrawingSize)
        {
            // Sync the size to other clients so they see the correct brush preview.
            photonView.RPC(nameof(RPC_SyncSize), RpcTarget.Others, _currentDrawingSize);
        }
    }

    /// <summary>
    /// Updates the visual scale of the brush tip sphere to reflect the current drawing size.
    /// </summary>
    private void UpdateBrushTipSphereSize()
    {
        if (_brushTipSphere == null) return;

        float sizeScale = _currentDrawingSize * previewScaleMultiplier;
        _brushTipSphere.localScale = _initialSphereScale * sizeScale;
    }

    /// <summary>
    /// Sets the drawing size property on the BrushStroke's mesh component.
    /// </summary>
    /// <param name="brushStroke">The target BrushStroke component.</param>
    /// <param name="size">The desired size multiplier.</param>
    private void SetBrushStrokeSize(BrushStroke brushStroke, float size)
    {
        if (brushStroke == null) return;

        // Assumes BrushStrokeMesh is a child component that handles the actual mesh generation.
        BrushStrokeMesh mesh = brushStroke.GetComponentInChildren<BrushStrokeMesh>();
        if (mesh != null)
        {
            mesh.sizeMultiplier = size;
        }
    }

    /// <summary>
    /// Static helper method to retrieve the current position and rotation of an XR Node (controller).
    /// </summary>
    /// <param name="node">The XRNode (e.g., LeftHand, RightHand).</param>
    /// <param name="position">Output for the tracked position.</param>
    /// <param name="rotation">Output for the tracked rotation.</param>
    /// <returns>True if position tracking data was successfully retrieved.</returns>
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

                return gotPosition; // Return tracking status based on position availability.
            }
        }
        return false;
    }
    #endregion

    //-------------------------------------------------------------------------

    #region Photon Callbacks
    /// <summary>
    /// Implementation of IPunObservable. Used to send/receive critical state
    /// variables for non-RPC synchronization.
    /// </summary>
    /// <param name="stream">The Photon stream for reading/writing data.</param>
    /// <param name="info">Information about the sender.</param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Owner sends the current brush size and grab state.
            stream.SendNext(_currentDrawingSize);
            stream.SendNext(isGrabbed);
        }
        else
        {
            // Remote clients receive and update the state.
            _currentDrawingSize = (float)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }
    #endregion

    //-------------------------------------------------------------------------

    #region Public API
    /// <summary>
    /// Gets the current size used for drawing new strokes.
    /// </summary>
    /// <returns>The current drawing size.</returns>
    public float GetCurrentDrawingSize()
    {
        return _currentDrawingSize;
    }

    /// <summary>
    /// Sets the drawing size and clamps it, then syncs the change to other clients.
    /// </summary>
    /// <param name="size">The new size to set.</param>
    public void SetDrawingSize(float size)
    {
        _currentDrawingSize = Mathf.Clamp(size, minSize, maxSize);
        if (photonView.IsMine)
            photonView.RPC(nameof(RPC_SyncSize), RpcTarget.Others, _currentDrawingSize);
    }

    /// <summary>
    /// Gets the length of the stroke currently being drawn.
    /// </summary>
    /// <returns>The length of the active stroke.</returns>
    public float GetCurrentStrokeLength()
    {
        return _currentStrokeLength;
    }

    /// <summary>
    /// Gets the maximum allowed length for a single stroke.
    /// </summary>
    /// <returns>The maximum drawing length.</returns>
    public float GetMaxDrawingLength()
    {
        return maxDrawingLength;
    }
    #endregion
}

//-------------------------------------------------------------------------

/// <summary>
/// Helper component attached to instantiated BrushStroke objects to store their
/// unique ID for persistence, deletion, and retrieval.
/// </summary>
public class StrokeTracker : MonoBehaviour
{
    public string strokeId;
}