/*using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleGroupController : MonoBehaviour
{
    [System.Serializable]
    public class TogglePanelGroup
    {
        public Toggle toggle;

        [Header("Panels to Enable When This Toggle Is ON")]
        public List<GameObject> panelsToEnable;

        [Header("Panels to Disable When This Toggle Is ON")]
        public List<GameObject> panelsToDisable;
    }

    [Header("Toggle Panel Settings")]
    [SerializeField] private List<TogglePanelGroup> toggleGroups = new List<TogglePanelGroup>();

    private void Start()
    {
        foreach (var group in toggleGroups)
        {
            group.toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(group, isOn));
        }
    }

    private void OnToggleChanged(TogglePanelGroup group, bool isOn)
    {
        if (!isOn) return;

        // Enable panels assigned to this toggle
        foreach (var panel in group.panelsToEnable)
        {
            if (panel != null)
                panel.SetActive(true);
        }

        // Disable panels assigned to this toggle
        foreach (var panel in group.panelsToDisable)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }
}
*/



///////////// master client





/*using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
public class ToggleGroupController : MonoBehaviourPun
{
    [System.Serializable]
    public class TogglePanelGroup
    {
        public Toggle toggle;
        [Header("Panels to Enable When This Toggle Is ON")]
        public List<GameObject> panelsToEnable;
        [Header("Panels to Disable When This Toggle Is ON")]
        public List<GameObject> panelsToDisable;
    }
    [Header("Toggle Panel Settings")]
    [SerializeField] private List<TogglePanelGroup> toggleGroups = new List<TogglePanelGroup>();
    [SerializeField] public bool masterClientOnly = false; // Flag to control access for elements beyond index 1
    private void Start()
    {
        foreach (var group in toggleGroups)
        {
            group.toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(group, isOn));
        }
    }
    private void OnToggleChanged(TogglePanelGroup group, bool isOn)
    {
        // Allow elements 0 and 1 for everyone, others restricted to master client if masterClientOnly is true
        int groupIndex = toggleGroups.IndexOf(group);
        if (masterClientOnly && groupIndex > 1 && !PhotonNetwork.IsMasterClient) return;
        if (!isOn) return;
        // Enable panels assigned to this toggle
        foreach (var panel in group.panelsToEnable)
        {
            if (panel != null)
                panel.SetActive(true);
        }
        // Disable panels assigned to this toggle
        foreach (var panel in group.panelsToDisable)
        {
            if (panel != null)
                panel.SetActive(false);
        }
        // Sync changes to all clients
        photonView.RPC("SyncToggleState", RpcTarget.All, groupIndex, isOn);
    }
    [PunRPC]
    private void SyncToggleState(int groupIndex, bool isOn)
    {
        if (!isOn) return;
        var group = toggleGroups[groupIndex];
        // Enable panels
        foreach (var panel in group.panelsToEnable)
        {
            if (panel != null)
                panel.SetActive(true);
        }
        // Disable panels
        foreach (var panel in group.panelsToDisable)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }
}*/


/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[System.Serializable]
public class TogglePanelGroup
{
    public Toggle toggle;
    public GameObject[] panelsToEnable;
    public GameObject[] panelsToDisable;
}

public class ToggleGroupController : MonoBehaviour
{
    [Header("Toggle Panel Settings")]
    [SerializeField] private List<TogglePanelGroup> toggleGroups = new List<TogglePanelGroup>();
    [SerializeField] public bool masterClientOnly = false;
    [Header("Access Control")]
    [SerializeField] private int publicAccessCount = 2;
    [Header("Room Behavior")]
    [SerializeField] private bool showDebugLogs = true;
    private bool isNewRoom = false;

    private void Start()
    {
        foreach (var group in toggleGroups)
        {
            group.toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(group, isOn));
        }
        CheckRoomStatus();
        UpdateToggleInteractability();
    }

    private void CheckRoomStatus()
    {
        int playerCount = PhotonNetwork.CurrentRoom?.PlayerCount ?? 1;
        isNewRoom = (playerCount <= 1);
        if (showDebugLogs)
        {
            Debug.Log($"[ToggleGroupController] Room Status - Players: {playerCount}, IsNewRoom: {isNewRoom}, IsMaster: {PhotonNetwork.IsMasterClient}");
        }
        if (isNewRoom && PhotonNetwork.IsMasterClient)
        {
            masterClientOnly = false;
            if (showDebugLogs)
                Debug.Log("[ToggleGroupController] New room detected - Setting presenter mode OFF");
        }
    }

    private void OnToggleChanged(TogglePanelGroup group, bool isOn)
    {
        int groupIndex = toggleGroups.IndexOf(group);
        if (!CanUseToggle(groupIndex))
        {
            group.toggle.isOn = false;
            if (showDebugLogs)
                Debug.LogWarning($"[ToggleGroupController] Access denied for toggle {groupIndex}. Presenter mode is active.");
            return;
        }
        if (isOn)
        {
            foreach (var otherGroup in toggleGroups)
            {
                if (otherGroup != group)
                {
                    foreach (var panel in otherGroup.panelsToEnable)
                    {
                        if (panel != null) panel.SetActive(false);
                    }
                    foreach (var panel in otherGroup.panelsToDisable)
                    {
                        if (panel != null) panel.SetActive(true);
                    }
                }
            }
            ApplyToggleChanges(group, isOn);
        }
        else
        {
            ApplyToggleChanges(group, isOn);
        }
        if (showDebugLogs)
            Debug.Log($"[ToggleGroupController] Toggle {groupIndex} changed to {isOn} by {PhotonNetwork.NickName} (local only)");
    }

    private bool CanUseToggle(int toggleIndex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (showDebugLogs)
                Debug.Log($"[ToggleGroupController] Master client - Full access granted for toggle {toggleIndex}");
            return true;
        }
        if (isNewRoom)
        {
            if (showDebugLogs)
                Debug.Log($"[ToggleGroupController] New room - Full access granted for toggle {toggleIndex}");
            return true;
        }
        if (!masterClientOnly) return true;
        bool publicAccess = toggleIndex == 0 || toggleIndex == 1;
        if (showDebugLogs)
            Debug.Log($"[ToggleGroupController] Non-master client - Toggle {toggleIndex} access: {publicAccess}");
        return publicAccess;
    }

    private void ApplyToggleChanges(TogglePanelGroup group, bool isOn)
    {
        if (isOn)
        {
            foreach (var panel in group.panelsToEnable)
            {
                if (panel != null)
                    panel.SetActive(true);
            }
            foreach (var panel in group.panelsToDisable)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }
        else
        {
            foreach (var panel in group.panelsToEnable)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
            foreach (var panel in group.panelsToDisable)
            {
                if (panel != null)
                    panel.SetActive(true);
            }
        }
    }

    public void UpdateToggleInteractability()
    {
        for (int i = 0; i < toggleGroups.Count; i++)
        {
            var toggle = toggleGroups[i].toggle;
            if (toggle != null)
            {
                bool canUse = CanUseToggle(i);
                toggle.interactable = canUse;
                if (!canUse && toggle.isOn)
                {
                    toggle.isOn = false;
                    ApplyToggleChanges(toggleGroups[i], false);
                }
                if (showDebugLogs)
                    Debug.Log($"[ToggleGroupController] Toggle {i} interactable: {canUse}");
            }
        }
    }

    public void OnPlayerJoinedRoom()
    {
        CheckRoomStatus();
        UpdateToggleInteractability();
        if (showDebugLogs)
            Debug.Log("[ToggleGroupController] Player joined - Updated room status and toggle accessibility");
    }

    public void SetRoomAsExisting()
    {
        isNewRoom = false;
        UpdateToggleInteractability();
        if (showDebugLogs)
            Debug.Log("[ToggleGroupController] Room marked as existing - Presenter mode restrictions now apply");
    }

    public bool HasToggleAccess(int toggleIndex)
    {
        return CanUseToggle(toggleIndex);
    }

    public bool IsNewRoom()
    {
        return isNewRoom;
    }
}*/


/////////////////////////////////////main /////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

public class ToggleGroupController : MonoBehaviourPun
{
    [System.Serializable]
    public class TogglePanelGroup
    {
        public Toggle toggle;
        [Header("Panels to Enable When This Toggle Is ON")]
        public List<GameObject> panelsToEnable;
        [Header("Panels to Disable When This Toggle Is ON")]
        public List<GameObject> panelsToDisable;
    }

    [Header("Toggle Panel Settings")]
    [SerializeField] private List<TogglePanelGroup> toggleGroups = new List<TogglePanelGroup>();
    [SerializeField] public bool masterClientOnly = false;

    private void Start()
    {
        foreach (var group in toggleGroups)
        {
            group.toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(group, isOn));
        }
    }

    private void OnToggleChanged(TogglePanelGroup group, bool isOn)
    {
        int groupIndex = toggleGroups.IndexOf(group);
        bool actualPresenterMode = GetPresenterModeFromRoom();
        if (actualPresenterMode && groupIndex > 1 && !PhotonNetwork.IsMasterClient) return;
        if (!isOn) return;

        foreach (var panel in group.panelsToEnable)
        {
            if (panel != null)
                panel.SetActive(true);
        }

        foreach (var panel in group.panelsToDisable)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }


    private bool GetPresenterModeFromRoom()

    {

        if (PhotonNetwork.CurrentRoom?.CustomProperties != null &&

            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("presenterMode"))

        {

            return (bool)PhotonNetwork.CurrentRoom.CustomProperties["presenterMode"];

        }

        return false; // Default to false if no property exists

    }
}


