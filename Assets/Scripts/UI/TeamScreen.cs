/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class TeamScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text roomCode;
    [SerializeField] TMP_InputField joinCode;
    [SerializeField] Button joinButton;
    void Update()
    {
        roomCode.text = NetworkManager.instance.roomName;
    }


    public void OnJoinRoomButtonPress()
    {
        NetworkManager.instance.roomName = joinCode.text;
        joinCode.text = "";
        PhotonNetwork.LeaveRoom();
        joinButton.interactable = false;

    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.LogError("Left Room");
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        joinButton.interactable = true;

    }
}

*/
/////////////////////////////////      MAIN WORKINGGGGG BELOWW /////////////////////////////////////////////

/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class TeamScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text roomCode; // Displays the room code
   
    [SerializeField] TMP_InputField joinCode; // Input field for room code
    [SerializeField] TMP_InputField playerNameInput; // Input field for player's name
    [SerializeField] Button joinButton; // Button to join room


    void Update()
    {
        // Show the current room name from the network manager
        roomCode.text = NetworkManager.instance.roomName;
    }

    public void OnJoinRoomButtonPress()
    {
        // Ensure the player has entered a name
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            Debug.LogError("Player name is empty! Please enter a name.");
            return;
        }

        // Additional name validation
        if (playerNameInput.text.Length > 20) // Limit player name length
        {
            Debug.LogError("Player name is too long! Please use a shorter name.");
            return;
        }

        // Set the player's name in Photon before joining a room
        PhotonNetwork.NickName = playerNameInput.text;

        // Join room using the code provided in the input field
        NetworkManager.instance.roomName = joinCode.text;

        // Clear the join code input field
        joinCode.text = "";

        // Check if already in a room, then leave
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        joinButton.interactable = false; // Disable button while processing
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Left Room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        joinButton.interactable = true; // Re-enable the button once joined
        Debug.Log("Joined Room as " + PhotonNetwork.NickName);
       
        // Instantiate player prefab
        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Set your spawn position
        GameObject playerObject = PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity);
        TMP_Text playerNameText = playerObject.GetComponentInChildren<TMP_Text>();
        playerNameText.text = PhotonNetwork.NickName; // Set displayed name
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
        joinButton.interactable = true; // Re-enable the button for retry
    }
}
*/


//////////////////////////////////////////////////////////////////////////////main////////////////////////////////


/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

public class TeamScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text roomCode; // Displays the room code
    [SerializeField] TMP_InputField joinCode; // Input field for room code
    [SerializeField] TMP_InputField playerNameInput; // Input field for player's name
    [SerializeField] Button joinButton; // Button to join room
    [SerializeField] TMP_Text statusMessage; // Text to display status
    [SerializeField] float messageDuration = 3f; // How long to show status messages

    private Coroutine messageCoroutine;

    void Update()
    {
        // Continuously show the current room name from the network manager
        roomCode.text = NetworkManager.instance.roomName;
    }

    public void OnJoinRoomButtonPress()
    {
        // Input validation
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            ShowStatusMessage("Please enter name.");
            return;
        }

        if (string.IsNullOrEmpty(joinCode.text))
        {
            ShowStatusMessage("Please enter room code.");
            return;
        }

        if (playerNameInput.text.Length > 20)
        {
            ShowStatusMessage("Name too long! Max 20 characters.");
            return;
        }

        PhotonNetwork.NickName = playerNameInput.text;
        NetworkManager.instance.roomName = joinCode.text;
        joinCode.text = "";
        joinButton.interactable = false;

        // Leave current room if in one, otherwise try to join directly
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.JoinRoom(NetworkManager.instance.roomName);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.JoinRoom(NetworkManager.instance.roomName);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        joinButton.interactable = true;
        ShowStatusMessage("Joined team successfully!");
        InstantiatePlayer();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinButton.interactable = true;
       // ShowStatusMessage("Failed to join team.");
    }
    private void InstantiatePlayer()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Default spawn position
        GameObject playerObject = PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity);
        TMP_Text playerNameText = playerObject.GetComponentInChildren<TMP_Text>();
        playerNameText.text = PhotonNetwork.NickName;
    }

    private void ShowStatusMessage(string message)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(DisplayMessage(message));
    }

    private IEnumerator DisplayMessage(string message)
    {
        statusMessage.text = message;
        yield return new WaitForSeconds(messageDuration);
        statusMessage.text = "";
    }
}*/


//////////////////////////////////////////////////////////////new for avatar/////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

public class TeamScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text roomCode;
    [SerializeField] TMP_InputField joinCode;
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_Text statusMessage;
    [SerializeField] float messageDuration = 3f;

    private Coroutine messageCoroutine;
    private bool playerInstantiated = false;
    private GameObject currentPlayerObject; // Track the current player instance

    void Update()
    {
        roomCode.text = NetworkManager.instance.roomName;
    }

    public void OnJoinRoomButtonPress()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            ShowStatusMessage("Please enter name.");
            return;
        }

        if (playerNameInput.text.Length > 20)
        {
            ShowStatusMessage("Name too long! Max 20 characters.");
            return;
        }

        PhotonNetwork.NickName = playerNameInput.text;
        joinCode.text = joinCode.text.Trim(); // Clean input

        if (string.IsNullOrEmpty(joinCode.text))
        {
            // Join a random room if no team code is entered
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // Join or create a room with the team code
            NetworkManager.instance.roomName = joinCode.text;
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                PhotonNetwork.JoinRoom(NetworkManager.instance.roomName);
            }
        }
        joinButton.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        joinButton.interactable = true;
        ShowStatusMessage("Joined team successfully!");

        if (!playerInstantiated)
        {
            if (string.IsNullOrEmpty(NetworkManager.instance.roomName) || AvatarSelection.selectedAvatarIndex < 0)
            {
                // Instantiate default avatar for random room
                InstantiateDefaultPlayer();
            }
            else
            {
                // Instantiate selected avatar for new room with team code
                InstantiateSelectedPlayer();
            }
            playerInstantiated = true;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // If no random room is available, create one
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        // Destroy the current player object when leaving the room
        if (currentPlayerObject != null)
        {
            PhotonNetwork.Destroy(currentPlayerObject);
            currentPlayerObject = null;
            playerInstantiated = false; // Allow reinstantiation in the new room
        }
        // Join the new room with the team code if set
        if (!string.IsNullOrEmpty(NetworkManager.instance.roomName))
        {
            PhotonNetwork.JoinRoom(NetworkManager.instance.roomName);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinButton.interactable = true;
        ShowStatusMessage("Failed to join team.");
    }

    private void InstantiateDefaultPlayer()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0);
        currentPlayerObject = PhotonNetwork.Instantiate("Network Player", spawnPosition, Quaternion.identity);
        TMP_Text playerNameText = currentPlayerObject.GetComponentInChildren<TMP_Text>();
        playerNameText.text = PhotonNetwork.NickName;
        NetworkManager.instance.SetNetworkPlayer(currentPlayerObject.GetComponent<NetworkPlayer>());
    }

    private void InstantiateSelectedPlayer()
    {
        // Destroy any existing default avatar
        PhotonView[] existingPlayers = FindObjectsOfType<PhotonView>();
        foreach (PhotonView pv in existingPlayers)
        {
            if (pv.IsMine && pv.gameObject.name.Contains("Avatar"))
            {
                PhotonNetwork.Destroy(pv.gameObject);
            }
        }

        Vector3 spawnPosition = new Vector3(0, 0, 0);
        string avatarPrefabName = "Avatar" + AvatarSelection.selectedAvatarIndex.ToString();
        currentPlayerObject = PhotonNetwork.Instantiate(avatarPrefabName, spawnPosition, Quaternion.identity);
        TMP_Text playerNameText = currentPlayerObject.GetComponentInChildren<TMP_Text>();
        playerNameText.text = PhotonNetwork.NickName;
        NetworkManager.instance.SetNetworkPlayer(currentPlayerObject.GetComponent<NetworkPlayer>());
    }

    private void ShowStatusMessage(string message)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(DisplayMessage(message));
    }

    private IEnumerator DisplayMessage(string message)
    {
        statusMessage.text = message;
        yield return new WaitForSeconds(messageDuration);
        statusMessage.text = "";
    }
}






