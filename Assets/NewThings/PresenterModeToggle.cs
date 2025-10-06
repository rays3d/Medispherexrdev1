/*using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PresenterModeToggle : MonoBehaviourPunCallbacks
{
    [SerializeField] private Toggle presenterToggle;
    public bool isPresenterMode = false;

    private const string PRESENTER_MODE_KEY = "presenterMode";

    private void Start()
    {
        if (presenterToggle == null)
        {
            Debug.LogError("[PresenterModeToggle] PresenterToggle not assigned!");
            return;
        }

        presenterToggle.onValueChanged.AddListener(OnPresenterToggleChanged);
        presenterToggle.interactable = PhotonNetwork.IsMasterClient;

        // If master, use local state. If not, wait for property update
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(isPresenterMode);
        }
        else
        {
            presenterToggle.interactable = false;
        }
    }

    private void OnPresenterToggleChanged(bool isOn)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            presenterToggle.isOn = false;
            presenterToggle.interactable = false;
            Debug.LogWarning("[PresenterModeToggle] Non-master client attempted to toggle. Denied.");
            return;
        }

        SetPresenterMode(isOn); // Master updates the property
    }

    private void SetPresenterMode(bool mode)
    {
        isPresenterMode = mode;
        presenterToggle.isOn = mode;

        // Set room property — this automatically syncs to all clients, including new ones
        Hashtable props = new Hashtable { { PRESENTER_MODE_KEY, mode } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool mode = (bool)propertiesThatChanged[PRESENTER_MODE_KEY];
            ApplyPresenterMode(mode);
        }
    }

    private void ApplyPresenterMode(bool mode)
    {
        isPresenterMode = mode;
        presenterToggle.isOn = mode;
        presenterToggle.interactable = PhotonNetwork.IsMasterClient;

        var toggleController = FindObjectOfType<ToggleGroupController>();
        var leftHandController = FindObjectOfType<XRLeftHandController>();
        var rightHandController = FindObjectOfType<XRRightHandController>();
        var grabInteractables = FindObjectsOfType<XRGrabNetworkInteractable>();

        if (toggleController != null)
            toggleController.masterClientOnly = mode;

        if (leftHandController != null)
            leftHandController.masterClientOnly = mode;

        if (rightHandController != null)
            rightHandController.masterClientOnly = mode;

        foreach (var interactable in grabInteractables)
        {
            if (interactable != null)
                interactable.masterClientOnly = mode;
        }

        Debug.Log($"[PresenterModeToggle] Applied presenter mode: {mode}");
    }
}
*/



/*using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PresenterModeToggle : MonoBehaviourPunCallbacks
{
    [SerializeField] private Toggle presenterToggle;
    public bool isPresenterMode = false;
    private const string PRESENTER_MODE_KEY = "presenterMode";

    [Header("Debug Info")]
    [SerializeField] private bool showDebugLogs = true;

    private void Start()
    {
        if (presenterToggle == null)
        {
            Debug.LogError("[PresenterModeToggle] PresenterToggle not assigned!");
            return;
        }

        presenterToggle.onValueChanged.AddListener(OnPresenterToggleChanged);

        // Wait a frame to ensure room is fully loaded
        StartCoroutine(DelayedInitialization());
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        yield return null; // Wait one frame

        // Check if this is a new room or existing room
        bool isNewRoom = PhotonNetwork.CurrentRoom.PlayerCount <= 1;

        if (isNewRoom)
        {
            // NEW ROOM: Default to presenter mode OFF
            isPresenterMode = false;
            if (PhotonNetwork.IsMasterClient)
            {
                SetPresenterMode(false);
                if (showDebugLogs)
                    Debug.Log("[PresenterModeToggle] New room - Presenter mode set to OFF by default");
            }
        }
        else
        {
            // EXISTING ROOM: Sync from room properties
            SyncPresenterStateFromRoom();
            if (showDebugLogs)
                Debug.Log("[PresenterModeToggle] Existing room - Syncing presenter mode from room");
        }

        // Set initial interactability
        UpdateToggleInteractability();
    }

    /// <summary>
    /// Syncs presenter state from room properties - called for existing rooms
    /// </summary>
    private void SyncPresenterStateFromRoom()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool roomPresenterMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Syncing from room properties: {roomPresenterMode}");

            ApplyPresenterMode(roomPresenterMode);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            // Master client initializes the room property if it doesn't exist
            SetPresenterMode(isPresenterMode);
        }
    }

    private void UpdateToggleInteractability()
    {
        bool canInteract = PhotonNetwork.IsMasterClient;
        presenterToggle.interactable = canInteract;

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Toggle interactable: {canInteract}");
    }

    private void OnPresenterToggleChanged(bool isOn)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // Revert unauthorized change
            presenterToggle.isOn = isPresenterMode;

            if (showDebugLogs)
                Debug.LogWarning("[PresenterModeToggle] Non-master client attempted to toggle. Reverted.");
            return;
        }

        SetPresenterMode(isOn);
    }

    private void SetPresenterMode(bool mode)
    {
        isPresenterMode = mode;
        presenterToggle.isOn = mode;

        // Set room property - THIS SYNCS TO ALL CLIENTS INCLUDING FUTURE JOINERS
        Hashtable props = new Hashtable { { PRESENTER_MODE_KEY, mode } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master set presenter mode: {mode}");
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool mode = (bool)propertiesThatChanged[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Received room property update: {mode}");

            ApplyPresenterMode(mode);
        }
    }

    private void ApplyPresenterMode(bool mode)
    {
        isPresenterMode = mode;
        presenterToggle.isOn = mode;

        // Update toggle interactability
        UpdateToggleInteractability();

        // Apply to all controlled components
        ApplyToControlledComponents(mode);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Applied presenter mode: {mode} | IsMaster: {PhotonNetwork.IsMasterClient}");
    }

    private void ApplyToControlledComponents(bool presenterMode)
    {
        // Apply to ToggleGroupController
        var toggleController = FindObjectOfType<ToggleGroupController>();
        if (toggleController != null)
        {
            toggleController.masterClientOnly = presenterMode;

            // If presenter mode is being turned ON in an existing room, mark it as existing
            if (presenterMode)
            {
                toggleController.SetRoomAsExisting();
            }

            toggleController.UpdateToggleInteractability();
        }

        // Apply to XR Controllers
        var leftHandController = FindObjectOfType<XRLeftHandController>();
        var rightHandController = FindObjectOfType<XRRightHandController>();

        if (leftHandController != null)
            leftHandController.masterClientOnly = presenterMode;
        if (rightHandController != null)
            rightHandController.masterClientOnly = presenterMode;

        // Apply to Grab Interactables
        var grabInteractables = FindObjectsOfType<XRGrabNetworkInteractable>();
        foreach (var interactable in grabInteractables)
        {
            if (interactable != null)
                interactable.masterClientOnly = presenterMode;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master client switched to: {newMasterClient.NickName}");

        // Update UI when master client changes
        UpdateToggleInteractability();

        // If we became the new master, ensure we have the correct state
        if (PhotonNetwork.IsMasterClient)
        {
            SyncPresenterStateFromRoom();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Player {newPlayer.NickName} joined room");

        // Notify ToggleGroupController that someone joined
        var toggleController = FindObjectOfType<ToggleGroupController>();
        if (toggleController != null)
        {
            toggleController.OnPlayerJoinedRoom();
        }

        // If presenter mode is on, ensure new player gets the correct restrictions
        if (PhotonNetwork.IsMasterClient && isPresenterMode)
        {
            // Force a property refresh to ensure new player gets presenter mode state
            Hashtable props = new Hashtable { { PRESENTER_MODE_KEY, isPresenterMode } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    // Public methods for external access
    public bool IsPresenterModeActive() => isPresenterMode;

    public void ForceSync()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(isPresenterMode);
        }
        else
        {
            SyncPresenterStateFromRoom();
        }
    }
}*/

//////////////////////////////////////////////////////////////////////////////////////////////////main working/////////////////////////////////////////


/*using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PresenterModeToggle : MonoBehaviourPunCallbacks
{
    [SerializeField] private Toggle presenterToggle;
    public bool isPresenterMode = false;
    private const string PRESENTER_MODE_KEY = "presenterMode";

    [Header("Debug Info")]
    [SerializeField] private bool showDebugLogs = true;

    private void Start()
    {
        if (presenterToggle == null)
        {
            Debug.LogError("[PresenterModeToggle] PresenterToggle not assigned!");
            return;
        }

        presenterToggle.onValueChanged.AddListener(OnPresenterToggleChanged);

        // Wait a frame to ensure room is fully loaded
        StartCoroutine(DelayedInitialization());
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        yield return null; // Wait one frame

        // Check if this is a new room or existing room
        bool isNewRoom = PhotonNetwork.CurrentRoom.PlayerCount <= 1;

        if (isNewRoom)
        {
            // NEW ROOM: Default to presenter mode OFF
            isPresenterMode = false;
            if (PhotonNetwork.IsMasterClient)
            {
                SetPresenterMode(false);
                if (showDebugLogs)
                    Debug.Log("[PresenterModeToggle] New room - Presenter mode set to OFF by default");
            }
        }
        else
        {
            // EXISTING ROOM: Sync from room properties
            SyncPresenterStateFromRoom();
            if (showDebugLogs)
                Debug.Log("[PresenterModeToggle] Existing room - Syncing presenter mode from room");
        }

        // Set initial interactability
        UpdateToggleInteractability();
    }

    /// <summary>
    /// Syncs presenter state from room properties - called for existing rooms
    /// </summary>
    private void SyncPresenterStateFromRoom()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool roomPresenterMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Syncing from room properties: {roomPresenterMode}");

            ApplyPresenterMode(roomPresenterMode);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            // Master client initializes the room property if it doesn't exist
            SetPresenterMode(isPresenterMode);
        }
    }

    private void UpdateToggleInteractability()
    {
        bool canInteract = PhotonNetwork.IsMasterClient;
        presenterToggle.interactable = canInteract;

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Toggle interactable: {canInteract}");
    }

    private void OnPresenterToggleChanged(bool isOn)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // Revert unauthorized change
            presenterToggle.isOn = isPresenterMode;

            if (showDebugLogs)
                Debug.LogWarning("[PresenterModeToggle] Non-master client attempted to toggle. Reverted.");
            return;
        }

        SetPresenterMode(isOn);
    }

    private void SetPresenterMode(bool mode)
    {
        isPresenterMode = mode;
        presenterToggle.isOn = mode;

        // Set room property - THIS SYNCS TO ALL CLIENTS INCLUDING FUTURE JOINERS
        Hashtable props = new Hashtable { { PRESENTER_MODE_KEY, mode } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master set presenter mode: {mode}");
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool mode = (bool)propertiesThatChanged[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Received room property update: {mode}");

            ApplyPresenterMode(mode);
        }
    }

    private void ApplyPresenterMode(bool mode)
    {
        isPresenterMode = mode;
        presenterToggle.isOn = mode;

        // Update toggle interactability
        UpdateToggleInteractability();

        // Apply to all controlled components
        ApplyToControlledComponents(mode);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Applied presenter mode: {mode} | IsMaster: {PhotonNetwork.IsMasterClient}");
    }

    private void ApplyToControlledComponents(bool presenterMode)
    {
        // Apply to ToggleGroupController
        var toggleController = FindObjectOfType<ToggleGroupController>();
        if (toggleController != null)
        {
            toggleController.masterClientOnly = presenterMode;
        }

        // Apply to XR Controllers
        var leftHandController = FindObjectOfType<XRLeftHandController>();
        var rightHandController = FindObjectOfType<XRRightHandController>();

        if (leftHandController != null)
            leftHandController.masterClientOnly = presenterMode;
        if (rightHandController != null)
            rightHandController.masterClientOnly = presenterMode;

        // Apply to Grab Interactables
        var grabInteractables = FindObjectsOfType<XRGrabNetworkInteractable>();
        foreach (var interactable in grabInteractables)
        {
            if (interactable != null)
                interactable.masterClientOnly = presenterMode;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master client switched to: {newMasterClient.NickName}");

        // Update UI when master client changes
        UpdateToggleInteractability();

        // If we became the new master, ensure we have the correct state
        if (PhotonNetwork.IsMasterClient)
        {
            SyncPresenterStateFromRoom();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Player {newPlayer.NickName} joined room");

        // If presenter mode is on, ensure new player gets the correct restrictions
        if (PhotonNetwork.IsMasterClient && isPresenterMode)
        {
            // Force a property refresh to ensure new player gets presenter mode state
            Hashtable props = new Hashtable { { PRESENTER_MODE_KEY, isPresenterMode } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    // Public methods for external access
    public bool IsPresenterModeActive() => isPresenterMode;

    public void ForceSync()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(isPresenterMode);
        }
        else
        {
            SyncPresenterStateFromRoom();
        }
    }
}*/


////////////////////////////////////////////////////////////////////////////////////////////////switch mode working//////////////////


/*using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;

public class PresenterModeToggle : MonoBehaviourPunCallbacks
{
    [SerializeField] private Toggle presenterToggle;
    public bool isPresenterMode = false;
    private const string PRESENTER_MODE_KEY = "presenterMode";

    [Header("Debug Info")]
    [SerializeField] private bool showDebugLogs = true;

    private void Awake()
    {
        if (presenterToggle != null)
        {
            presenterToggle.isOn = false; // Reset to default before any logic
        }
    }

    private void Start()
    {
        if (presenterToggle == null)
        {
            Debug.LogError("[PresenterModeToggle] PresenterToggle not assigned!");
            return;
        }

        // Force initial state to OFF before adding listeners
        presenterToggle.isOn = false;
        presenterToggle.onValueChanged.AddListener(OnPresenterToggleChanged);

        // Proceed with delayed initialization
        StartCoroutine(DelayedInitialization());
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        yield return null; // Wait one frame

        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null; // Wait until connected
        }

        // Check if this is a new room or existing room
        bool isNewRoom = PhotonNetwork.CurrentRoom?.PlayerCount <= 1;

        if (isNewRoom)
        {
            isPresenterMode = false;
            if (PhotonNetwork.IsMasterClient)
            {
                SetPresenterMode(false);
                if (showDebugLogs)
                    Debug.Log("[PresenterModeToggle] New room - Presenter mode set to OFF by default");
            }
        }
        else
        {
            // EXISTING ROOM: Sync from room properties
            SyncPresenterStateFromRoom();
            if (showDebugLogs)
                Debug.Log("[PresenterModeToggle] Existing room - Syncing presenter mode from room");
        }

        // Set initial interactability
        UpdateToggleInteractability();
    }

    /// <summary>
    /// Syncs presenter state from room properties - called for existing rooms
    /// </summary>
    private void SyncPresenterStateFromRoom()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool roomPresenterMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Syncing from room properties: {roomPresenterMode}");

            ApplyPresenterMode(roomPresenterMode);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            // Master client initializes the room property if it doesn't exist
            SetPresenterMode(isPresenterMode);
        }
    }

    private void UpdateToggleInteractability()
    {
        bool canInteract = PhotonNetwork.IsMasterClient;
        presenterToggle.interactable = canInteract;

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Toggle interactable: {canInteract}");
    }

    private void OnPresenterToggleChanged(bool isOn)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // Revert unauthorized change
            presenterToggle.isOn = isPresenterMode;

            if (showDebugLogs)
                Debug.LogWarning("[PresenterModeToggle] Non-master client attempted to toggle. Reverted.");
            return;
        }

        SetPresenterMode(isOn);
    }

    private void SetPresenterMode(bool mode)
    {
        isPresenterMode = mode;
        presenterToggle.isOn = mode;

        // Set room property - THIS SYNCS TO ALL CLIENTS INCLUDING FUTURE JOINERS
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { PRESENTER_MODE_KEY, mode } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master set presenter mode: {mode}");
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool mode = (bool)propertiesThatChanged[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Received room property update: {mode}");

            ApplyPresenterMode(mode);
        }
    }

    private void ApplyPresenterMode(bool mode)
    {
        isPresenterMode = mode;
        presenterToggle.isOn = mode;

        // Update toggle interactability
        UpdateToggleInteractability();

        // Apply to all controlled components
        ApplyToControlledComponents(mode);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Applied presenter mode: {mode} | IsMaster: {PhotonNetwork.IsMasterClient}");
    }

    private void ApplyToControlledComponents(bool presenterMode)
    {
        // Apply to ToggleGroupController
        var toggleController = FindObjectOfType<ToggleGroupController>();
        if (toggleController != null)
        {
            toggleController.masterClientOnly = presenterMode;
        }

        // Apply to XR Controllers
        var leftHandController = FindObjectOfType<XRLeftHandController>();
        var rightHandController = FindObjectOfType<XRRightHandController>();

        if (leftHandController != null)
            leftHandController.masterClientOnly = presenterMode;
        if (rightHandController != null)
            rightHandController.masterClientOnly = presenterMode;

        // Apply to Grab Interactables
        var grabInteractables = FindObjectsOfType<XRGrabNetworkInteractable>();
        foreach (var interactable in grabInteractables)
        {
            if (interactable != null)
                interactable.masterClientOnly = presenterMode;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master client switched to: {newMasterClient.NickName}");

        // Update UI when master client changes
        UpdateToggleInteractability();

        // If we became the new master, ensure we have the correct state
        if (PhotonNetwork.IsMasterClient)
        {
            SyncPresenterStateFromRoom();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Player {newPlayer.NickName} joined room");

        // If presenter mode is on, ensure new player gets the correct restrictions
        if (PhotonNetwork.IsMasterClient && isPresenterMode)
        {
            // Force a property refresh to ensure new player gets presenter mode state
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { PRESENTER_MODE_KEY, isPresenterMode } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    // Public methods for external access
    public bool IsPresenterModeActive() => isPresenterMode;

    public void ForceSync()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(isPresenterMode);
        }
        else
        {
            SyncPresenterStateFromRoom();
        }
    }
}

*/








////////////////////////////////////////////////////////////////////////////////mainnnnnnnnnnnnn




/*
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;

public class PresenterModeToggle : MonoBehaviourPunCallbacks
{
    [Header("Button Setup")]
    [SerializeField] private Button collaboratorButton;
    [SerializeField] private Button presenterButton;

    public bool isPresenterMode = false;
    private const string PRESENTER_MODE_KEY = "presenterMode";

    [Header("UI Text Elements (Optional)")]
    [SerializeField] private TextMeshProUGUI statusTextBox1;
    [SerializeField] private TextMeshProUGUI statusTextBox2;
    [SerializeField] private string presenterModeText = "Presenter Mode";
    [SerializeField] private string collaboratorModeText = "Collaborator Mode";

    [Header("Visual Feedback")]
    [SerializeField] private Color selectedColor = new Color(0.5f, 0f, 0.5f, 1f); // Violet
    [SerializeField] private Color unselectedColor = Color.white;

    [Header("Debug Info")]
    [SerializeField] private bool showDebugLogs = true;

    private void Awake()
    {
        // Initialize buttons to default state - Collaborator is active by default
        UpdateUI(false);
    }

    private void Start()
    {
        if (collaboratorButton == null || presenterButton == null)
        {
            Debug.LogError("[PresenterModeToggle] Collaborator or Presenter Button not assigned!");
            return;
        }

        // Add button click listeners
        collaboratorButton.onClick.AddListener(OnCollaboratorButtonClicked);
        presenterButton.onClick.AddListener(OnPresenterButtonClicked);

        // Proceed with delayed initialization
        StartCoroutine(DelayedInitialization());
    }

      private System.Collections.IEnumerator DelayedInitialization()
        {
            yield return null; // Wait one frame

            while (!PhotonNetwork.IsConnectedAndReady)
            {
                yield return null; // Wait until connected
            }

            // Check if this is a new room or existing room
            bool isNewRoom = PhotonNetwork.CurrentRoom?.PlayerCount <= 1;

            if (isNewRoom)
            {
                isPresenterMode = false;
                if (PhotonNetwork.IsMasterClient)
                {
                    SetPresenterMode(false);
                    if (showDebugLogs)
                        Debug.Log("[PresenterModeToggle] New room - Set to Collaborator mode by default");
                }
            }
            else
            {
                // EXISTING ROOM: Sync from room properties
                SyncPresenterStateFromRoom();
                if (showDebugLogs)
                    Debug.Log("[PresenterModeToggle] Existing room - Syncing mode from room");
            }

            // Set initial interactability
            UpdateButtonInteractability();
        }


    



    private void OnCollaboratorButtonClicked()
    {
        if (showDebugLogs)
            Debug.Log("[PresenterModeToggle] Collaborator button clicked");

        if (!PhotonNetwork.IsMasterClient)
        {
            if (showDebugLogs)
                Debug.LogWarning("[PresenterModeToggle] Non-master client attempted to change mode. Ignored.");
            return;
        }

        // Only switch if not already in collaborator mode
        if (isPresenterMode)
        {
            SetPresenterMode(false);
        }
    }

    private void OnPresenterButtonClicked()
    {
        if (showDebugLogs)
            Debug.Log("[PresenterModeToggle] Presenter button clicked");

        if (!PhotonNetwork.IsMasterClient)
        {
            if (showDebugLogs)
                Debug.LogWarning("[PresenterModeToggle] Non-master client attempted to change mode. Ignored.");
            return;
        }

        // Only switch if not already in presenter mode
        if (!isPresenterMode)
        {
            SetPresenterMode(true);
        }
    }

    /// <summary>
    /// Updates both text boxes and visual feedback based on current mode
    /// </summary>
    private void UpdateUI(bool presenterModeActive)
    {
        // Update text boxes
        string displayText = presenterModeActive ? presenterModeText : collaboratorModeText;

        if (statusTextBox1 != null)
        {
            statusTextBox1.text = displayText;
        }

        if (statusTextBox2 != null)
        {
            statusTextBox2.text = displayText;
        }

        // Update button visual feedback
        UpdateButtonColors(presenterModeActive);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Updated UI to: {displayText}");
    }

    private void UpdateButtonColors(bool presenterModeActive)
    {
        if (collaboratorButton != null && presenterButton != null)
        {
            // Get the Image components (button backgrounds)
            Image collaboratorImage = collaboratorButton.GetComponent<Image>();
            Image presenterImage = presenterButton.GetComponent<Image>();

            if (collaboratorImage != null && presenterImage != null)
            {
                if (presenterModeActive)
                {
                    // Presenter mode: Presenter button is violet, Collaborator is white
                    collaboratorImage.color = unselectedColor;
                    presenterImage.color = selectedColor;
                }
                else
                {
                    // Collaborator mode: Collaborator button is violet, Presenter is white
                    collaboratorImage.color = selectedColor;
                    presenterImage.color = unselectedColor;
                }
            }
        }
    }

    /// <summary>
    /// Syncs presenter state from room properties - called for existing rooms
    /// </summary>
    private void SyncPresenterStateFromRoom()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool roomPresenterMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Syncing from room properties: {roomPresenterMode}");

            ApplyPresenterMode(roomPresenterMode);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            // Master client initializes the room property if it doesn't exist
            SetPresenterMode(isPresenterMode);
        }
    }

    private void UpdateButtonInteractability()
    {
        bool canInteract = PhotonNetwork.IsMasterClient;

        if (collaboratorButton != null)
            collaboratorButton.interactable = canInteract;
        if (presenterButton != null)
            presenterButton.interactable = canInteract;

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Buttons interactable: {canInteract}");
    }

    private void SetPresenterMode(bool mode)
    {
        isPresenterMode = mode;

        // Update UI
        UpdateUI(mode);

        // Set room property - THIS SYNCS TO ALL CLIENTS INCLUDING FUTURE JOINERS
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { PRESENTER_MODE_KEY, mode } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master set mode: {(mode ? "Presenter" : "Collaborator")}");
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool mode = (bool)propertiesThatChanged[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Received room property update: {(mode ? "Presenter" : "Collaborator")}");

            ApplyPresenterMode(mode);
        }
    }

    private void ApplyPresenterMode(bool mode)
    {
        isPresenterMode = mode;

        // Update UI
        UpdateUI(mode);

        // Update button interactability
        UpdateButtonInteractability();

        // Apply to all controlled components
        ApplyToControlledComponents(mode);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Applied mode: {(mode ? "Presenter" : "Collaborator")} | IsMaster: {PhotonNetwork.IsMasterClient}");
    }

    private void ApplyToControlledComponents(bool presenterMode)
    {
        // Apply to ToggleGroupController
        var toggleController = FindObjectOfType<ToggleGroupController>();
        if (toggleController != null)
        {
            toggleController.masterClientOnly = presenterMode;
        }

        // Apply to XR Controllers
        var leftHandController = FindObjectOfType<XRLeftHandController>();
        var rightHandController = FindObjectOfType<XRRightHandController>();

        if (leftHandController != null)
            leftHandController.masterClientOnly = presenterMode;
        if (rightHandController != null)
            rightHandController.masterClientOnly = presenterMode;

        // Apply to Grab Interactables
        var grabInteractables = FindObjectsOfType<XRGrabNetworkInteractable>();
        foreach (var interactable in grabInteractables)
        {
            if (interactable != null)
                interactable.masterClientOnly = presenterMode;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master client switched to: {newMasterClient.NickName}");

        // Update UI when master client changes
        UpdateButtonInteractability();

        // If we became the new master, ensure we have the correct state
        if (PhotonNetwork.IsMasterClient)
        {
            SyncPresenterStateFromRoom();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Player {newPlayer.NickName} joined room");

        // If presenter mode is on, ensure new player gets the correct restrictions
        if (PhotonNetwork.IsMasterClient)
        {
            // Force a property refresh to ensure new player gets current mode state
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { PRESENTER_MODE_KEY, isPresenterMode } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    // Public methods for external access
    public bool IsPresenterModeActive() => isPresenterMode;
    public bool IsCollaboratorModeActive() => !isPresenterMode;

    public void ForceSync()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(isPresenterMode);
        }
        else
        {
            SyncPresenterStateFromRoom();
        }
    }

    // Public method to manually update UI (if needed from external scripts)
    public void RefreshUI()
    {
        UpdateUI(isPresenterMode);
    }

    // Public methods to change the text dynamically
    public void SetPresenterModeText(string newText)
    {
        presenterModeText = newText;
        if (isPresenterMode)
            UpdateUI(true);
    }

    public void SetCollaboratorModeText(string newText)
    {
        collaboratorModeText = newText;
        if (!isPresenterMode)
            UpdateUI(false);
    }

    // Public methods to programmatically switch modes (only works for master client)
    public void SwitchToPresenterMode()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(true);
        }
    }

    public void SwitchToCollaboratorMode()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(false);
        }
    }
}*/



using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;

public class PresenterModeToggle : MonoBehaviourPunCallbacks
{
    [Header("Button Setup")]
    [SerializeField] private Button collaboratorButton;
    [SerializeField] private Button presenterButton;

    public bool isPresenterMode = false;
    private const string PRESENTER_MODE_KEY = "presenterMode";

    [Header("UI Text Elements (Optional)")]
    [SerializeField] private TextMeshProUGUI statusTextBox1;
    [SerializeField] private TextMeshProUGUI statusTextBox2;
    [SerializeField] private string presenterModeText = "Presenter Mode";
    [SerializeField] private string collaboratorModeText = "Collaborator Mode";

    [Header("Visual Feedback")]
    [SerializeField] private Color selectedColor = new Color(0.5f, 0f, 0.5f, 1f); // Violet
    [SerializeField] private Color unselectedColor = Color.white;

    [Header("Debug Info")]
    [SerializeField] private bool showDebugLogs = true;

    private void Awake()
    {
        // Initialize buttons to default state - Collaborator is active by default
        UpdateUI(false);
    }

    private void Start()
    {
        if (collaboratorButton == null || presenterButton == null)
        {
            Debug.LogError("[PresenterModeToggle] Collaborator or Presenter Button not assigned!");
            return;
        }

        // Add button click listeners
        collaboratorButton.onClick.AddListener(OnCollaboratorButtonClicked);
        presenterButton.onClick.AddListener(OnPresenterButtonClicked);

        // Proceed with delayed initialization
        StartCoroutine(DelayedInitialization());
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        yield return null; // Wait one frame

        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null; // Wait until connected
        }

        // Check if this is a new room or existing room
        bool isNewRoom = PhotonNetwork.CurrentRoom?.PlayerCount <= 1;

        if (isNewRoom)
        {
            // NEW ROOM: Only master client sets the default mode
            if (PhotonNetwork.IsMasterClient)
            {
                isPresenterMode = false; // Default to collaborator
                SetPresenterMode(false);
                if (showDebugLogs)
                    Debug.Log("[PresenterModeToggle] ?? NEW ROOM - Master setting default Collaborator mode");
            }
        }
        else
        {
            // EXISTING ROOM: This should have been handled by OnJoinedRoom -> AggressiveRoomSync
            // But let's add a backup check
            if (showDebugLogs)
                Debug.Log("[PresenterModeToggle] ?? EXISTING ROOM - Backup sync check");

            yield return new WaitForSeconds(0.5f); // Wait a bit more

            // Backup sync attempt if AggressiveRoomSync somehow failed
            if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&
                PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
            {
                bool hostMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

                if (showDebugLogs)
                    Debug.Log($"[PresenterModeToggle] ?? BACKUP SYNC - Host mode: {(hostMode ? "PRESENTER" : "COLLABORATOR")}");

                ApplyPresenterMode(hostMode);
            }
        }

        // Set button interactability
        UpdateButtonInteractability();
    }

    /// <summary>
    /// Ensures room property sync happens properly for clients joining existing rooms
    /// </summary>
    private System.Collections.IEnumerator EnsureRoomPropertySync()
    {
        int attempts = 0;
        int maxAttempts = 10; // Try for up to 2 seconds

        while (attempts < maxAttempts)
        {
            if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&
                PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
            {
                bool roomPresenterMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

                if (showDebugLogs)
                    Debug.Log($"[PresenterModeToggle] SUCCESS - Found room property after {attempts} attempts: {roomPresenterMode}");

                // Force apply the room mode
                ApplyPresenterMode(roomPresenterMode);
                yield break; // Exit successfully
            }

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Attempt {attempts + 1}: Room properties not ready yet, waiting...");

            attempts++;
            yield return new WaitForSeconds(0.2f);
        }

        // If we get here, room properties weren't found - set default
        if (showDebugLogs)
            Debug.LogWarning("[PresenterModeToggle] Could not find room properties after max attempts, setting default collaborator mode");

        ApplyPresenterMode(false); // Default to collaborator
    }

    /// <summary>
    /// Forces synchronization from room properties - used when joining existing rooms
    /// </summary>
    private void ForceSyncFromRoomProperties()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool roomPresenterMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] FORCE SYNC - Room mode: {roomPresenterMode}, Local mode before sync: {isPresenterMode}");

            // Force apply the room's mode, overriding local setting
            ApplyPresenterMode(roomPresenterMode);

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] FORCE SYNC SUCCESS - Applied room mode: {roomPresenterMode}");
        }
        else
        {
            if (showDebugLogs)
            {
                if (PhotonNetwork.CurrentRoom?.CustomProperties == null)
                    Debug.Log("[PresenterModeToggle] FORCE SYNC - Room CustomProperties is null");
                else
                    Debug.Log($"[PresenterModeToggle] FORCE SYNC - Room properties exist but no {PRESENTER_MODE_KEY} found");
            }

            // If no room property exists and we're master, set our current mode
            if (PhotonNetwork.IsMasterClient)
            {
                if (showDebugLogs)
                    Debug.Log("[PresenterModeToggle] No room property found, master client setting default");
                SetPresenterMode(false); // Default to collaborator mode
            }
            else
            {
                // If we're not master and no room property exists, wait for master to set it
                if (showDebugLogs)
                    Debug.Log("[PresenterModeToggle] Non-master client waiting for room property");
                ApplyPresenterMode(false); // Default to collaborator while waiting
            }
        }
    }

    private void OnCollaboratorButtonClicked()
    {
        if (showDebugLogs)
            Debug.Log("[PresenterModeToggle] Collaborator button clicked");

        if (!PhotonNetwork.IsMasterClient)
        {
            if (showDebugLogs)
                Debug.LogWarning("[PresenterModeToggle] Non-master client attempted to change mode. Ignored.");
            return;
        }

        // Only switch if not already in collaborator mode
        if (isPresenterMode)
        {
            SetPresenterMode(false);
        }
    }

    private void OnPresenterButtonClicked()
    {
        if (showDebugLogs)
            Debug.Log("[PresenterModeToggle] Presenter button clicked");

        if (!PhotonNetwork.IsMasterClient)
        {
            if (showDebugLogs)
                Debug.LogWarning("[PresenterModeToggle] Non-master client attempted to change mode. Ignored.");
            return;
        }

        // Only switch if not already in presenter mode
        if (!isPresenterMode)
        {
            SetPresenterMode(true);
        }
    }

    /// <summary>
    /// Updates both text boxes and visual feedback based on current mode
    /// </summary>
    private void UpdateUI(bool presenterModeActive)
    {
        // Update text boxes
        string displayText = presenterModeActive ? presenterModeText : collaboratorModeText;

        if (statusTextBox1 != null)
        {
            statusTextBox1.text = displayText;
        }

        if (statusTextBox2 != null)
        {
            statusTextBox2.text = displayText;
        }

        // Update button visual feedback
        UpdateButtonColors(presenterModeActive);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Updated UI to: {displayText}");
    }

    private void UpdateButtonColors(bool presenterModeActive)
    {
        if (collaboratorButton != null && presenterButton != null)
        {
            // Get the Image components (button backgrounds)
            Image collaboratorImage = collaboratorButton.GetComponent<Image>();
            Image presenterImage = presenterButton.GetComponent<Image>();

            if (collaboratorImage != null && presenterImage != null)
            {
                if (presenterModeActive)
                {
                    // Presenter mode: Presenter button is violet, Collaborator is white
                    collaboratorImage.color = unselectedColor;
                    presenterImage.color = selectedColor;
                }
                else
                {
                    // Collaborator mode: Collaborator button is violet, Presenter is white
                    collaboratorImage.color = selectedColor;
                    presenterImage.color = unselectedColor;
                }
            }
        }
    }

    /// <summary>
    /// Syncs presenter state from room properties - called for existing rooms
    /// </summary>
    private void SyncPresenterStateFromRoom()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool roomPresenterMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Syncing from room properties: {roomPresenterMode}");

            ApplyPresenterMode(roomPresenterMode);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            // Master client initializes the room property if it doesn't exist
            SetPresenterMode(isPresenterMode);
        }
    }

    private void UpdateButtonInteractability()
    {
        bool canInteract = PhotonNetwork.IsMasterClient;

        if (collaboratorButton != null)
            collaboratorButton.interactable = canInteract;
        if (presenterButton != null)
            presenterButton.interactable = canInteract;

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Buttons interactable: {canInteract}");
    }

    private void SetPresenterMode(bool mode)
    {
        isPresenterMode = mode;

        // Update UI
        UpdateUI(mode);

        // Set room property - THIS SYNCS TO ALL CLIENTS INCLUDING FUTURE JOINERS
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { PRESENTER_MODE_KEY, mode } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master set mode: {(mode ? "Presenter" : "Collaborator")}");
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PRESENTER_MODE_KEY))
        {
            bool mode = (bool)propertiesThatChanged[PRESENTER_MODE_KEY];

            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] Received room property update: {(mode ? "Presenter" : "Collaborator")}");

            ApplyPresenterMode(mode);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] OnJoinedRoom called - Player count: {PhotonNetwork.CurrentRoom.PlayerCount}");

        // IMMEDIATELY check if this is an existing room with a host
        if (PhotonNetwork.CurrentRoom?.PlayerCount > 1)
        {
            if (showDebugLogs)
                Debug.Log("[PresenterModeToggle] JOINING EXISTING ROOM - Host is already present, syncing to host's mode!");

            // Start aggressive sync process for joining existing room
            StartCoroutine(AggressiveRoomSync());
        }
    }

    /// <summary>
    /// Aggressively tries to sync with room properties when joining existing room
    /// </summary>
    private System.Collections.IEnumerator AggressiveRoomSync()
    {
        int attempts = 0;
        int maxAttempts = 20; // Try for 4 seconds

        while (attempts < maxAttempts)
        {
            // Try to get room properties
            if (PhotonNetwork.CurrentRoom?.CustomProperties != null)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PRESENTER_MODE_KEY))
                {
                    bool hostMode = (bool)PhotonNetwork.CurrentRoom.CustomProperties[PRESENTER_MODE_KEY];

                    if (showDebugLogs)
                        Debug.Log($"[PresenterModeToggle] ?? FOUND HOST MODE: {(hostMode ? "PRESENTER" : "COLLABORATOR")} (attempt {attempts + 1})");

                    // FORCE APPLY HOST'S MODE - Override whatever client had locally
                    isPresenterMode = hostMode;

                    // Update UI to match host's mode
                    UpdateUI(hostMode);
                    UpdateButtonInteractability();
                    ApplyToControlledComponents(hostMode);

                    if (showDebugLogs)
                        Debug.Log($"[PresenterModeToggle] ? CLIENT SYNCED TO HOST MODE: {(hostMode ? "PRESENTER" : "COLLABORATOR")}");

                    yield break; // Success - exit
                }
                else
                {
                    if (showDebugLogs)
                        Debug.Log($"[PresenterModeToggle] Room properties exist but no presenter mode key found (attempt {attempts + 1})");
                }
            }
            else
            {
                if (showDebugLogs)
                    Debug.Log($"[PresenterModeToggle] Room properties not available yet (attempt {attempts + 1})");
            }

            attempts++;
            yield return new WaitForSeconds(0.2f); // Wait 200ms between attempts
        }

        // If we reach here, couldn't find room properties - set default
        if (showDebugLogs)
            Debug.LogWarning("[PresenterModeToggle] ?? Could not sync with host mode, setting default collaborator");

        isPresenterMode = false;
        UpdateUI(false);
        UpdateButtonInteractability();
        ApplyToControlledComponents(false);
    }

    private void ApplyPresenterMode(bool mode)
    {
        isPresenterMode = mode;

        // Update UI
        UpdateUI(mode);

        // Update button interactability
        UpdateButtonInteractability();

        // Apply to all controlled components
        ApplyToControlledComponents(mode);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Applied mode: {(mode ? "Presenter" : "Collaborator")} | IsMaster: {PhotonNetwork.IsMasterClient}");
    }

    private void ApplyToControlledComponents(bool presenterMode)
    {
        // Apply to ToggleGroupController
        var toggleController = FindObjectOfType<ToggleGroupController>();
        if (toggleController != null)
        {
            toggleController.masterClientOnly = presenterMode;
        }

        // Apply to XR Controllers
        var leftHandController = FindObjectOfType<XRLeftHandController>();
        var rightHandController = FindObjectOfType<XRRightHandController>();

        if (leftHandController != null)
            leftHandController.masterClientOnly = presenterMode;
        if (rightHandController != null)
            rightHandController.masterClientOnly = presenterMode;

        // Apply to Grab Interactables
        var grabInteractables = FindObjectsOfType<XRGrabNetworkInteractable>();
        foreach (var interactable in grabInteractables)
        {
            if (interactable != null)
                interactable.masterClientOnly = presenterMode;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] Master client switched to: {newMasterClient.NickName}");

        // Update UI when master client changes
        UpdateButtonInteractability();

        // If we became the new master, ensure we have the correct state
        if (PhotonNetwork.IsMasterClient)
        {
            SyncPresenterStateFromRoom();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (showDebugLogs)
            Debug.Log($"[PresenterModeToggle] ?? Player {newPlayer.NickName} joined room");

        // If we're the master client, IMMEDIATELY refresh room properties for the new player
        if (PhotonNetwork.IsMasterClient)
        {
            if (showDebugLogs)
                Debug.Log($"[PresenterModeToggle] ?? HOST SENDING CURRENT MODE TO NEW PLAYER: {(isPresenterMode ? "PRESENTER" : "COLLABORATOR")}");

            // FORCE send current mode to new player
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { PRESENTER_MODE_KEY, isPresenterMode } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    // Public methods for external access
    public bool IsPresenterModeActive() => isPresenterMode;
    public bool IsCollaboratorModeActive() => !isPresenterMode;

    public void ForceSync()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(isPresenterMode);
        }
        else
        {
            ForceSyncFromRoomProperties();
        }
    }

    // Public method to manually update UI (if needed from external scripts)
    public void RefreshUI()
    {
        UpdateUI(isPresenterMode);
    }

    // Public methods to change the text dynamically
    public void SetPresenterModeText(string newText)
    {
        presenterModeText = newText;
        if (isPresenterMode)
            UpdateUI(true);
    }

    public void SetCollaboratorModeText(string newText)
    {
        collaboratorModeText = newText;
        if (!isPresenterMode)
            UpdateUI(false);
    }

    // Public methods to programmatically switch modes (only works for master client)
    public void SwitchToPresenterMode()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(true);
        }
    }

    public void SwitchToCollaboratorMode()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetPresenterMode(false);
        }
    }
}