/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public NetworkPlayer player;
    public string roomName;
    public static NetworkManager instance;

    List<RoomInfo> roomList = new List<RoomInfo>();


    bool joinRoomWithCode;

    public static event Action JoinedRoom;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SetNetworkPlayer(NetworkPlayer _player)
    {
        player = _player;
    }
    public NetworkPlayer GetNetworkPlayer()
    {
        return player;
    }
     void Start()
    {
       
        Connect();
    }
    public void Connect()
    {
        Debug.Log("up");
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("inside");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("outside");
            OnConnectedToMaster();
           
        }
    }



    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }


    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom(string roomName, int playerCount)
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = playerCount }, TypedLobby.Default);
    }

    public List<string> GetRoomList()
    {
        List<string> list = new List<string>();

        foreach (var roomInfo in roomList)
        {
            list.Add(roomInfo.Name);
            // Debug.Log(roomInfo.Name);
        }

        return list;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
        GetRoomList();
    }

    public override void OnConnectedToMaster()
    {
        
        base.OnConnectedToMaster();
        //Debug.Log("OnConnected");
        if (roomName == "")
        {
           // if (!LicenseManager.Instance.isPurchased) return;
            roomName = UnityEngine.Random.Range(10000, 99999).ToString();
            //roomName = "99156";
            JoinRoom(roomName);
            joinRoomWithCode = true;
           // CreateRoom(roomName, 25);
            SceneManager.LoadScene(2);
        }
        else
        {
            JoinRoom(roomName);
            joinRoomWithCode = true;
        }
        GetRoomList();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        GetRoomList();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        //  Debug.Log($"<color=red>Disconnected</color>: <color=yellow>{cause}</color>");
        Application.Quit();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.LogError("Join Random Room Failed");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //  Debug.LogError("Join Room Failed");
        if (joinRoomWithCode)
        {
            joinRoomWithCode = false;

            CreateRoom(roomName, 25);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // Debug.LogError("Creating Room Failed");

        if (joinRoomWithCode)
        {
            joinRoomWithCode = false;
            JoinRoom(roomName);
        }

    }
    public override void OnConnected()
    {
        // Debug.Log("<color=green>Connected</color>");

    }
    public override void OnJoinedRoom()
    {
        //  Debug.Log("<color=green>JoinedRoom </color>" + PhotonNetwork.CurrentRoom.PlayerCount);
        JoinedRoom?.Invoke();
        roomName = PhotonNetwork.CurrentRoom.Name;
        //  Debug.Log(roomName);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //  Debug.Log("Entered " + PhotonNetwork.CurrentRoom.PlayerCount);
        LoadModelFromURL.Instance.OnOtherJoinRoom();

    }
    public override void OnCreatedRoom()
    {
        // Debug.Log("RoomCreated");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Disconnect();
    }
}
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using UnityEditor.XR;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public NetworkPlayer player;
    public string roomName;
    public static NetworkManager instance;
    private PunVoiceClient voiceClient;
    private Recorder recorder;
    private Speaker speaker;

    List<RoomInfo> roomList = new List<RoomInfo>();
    bool joinRoomWithCode;

    public static event Action JoinedRoom;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        InitializeVoice();
    }

    private void InitializeVoice()
    {
        voiceClient = PunVoiceClient.Instance;
        if (voiceClient == null)
        {
            Debug.LogError("PunVoiceClient instance not found!");
            return;
        }

        recorder = FindObjectOfType<Recorder>();
        speaker = FindObjectOfType<Speaker>();

        Debug.Log("Voice components initialized");
    }

    public void SetNetworkPlayer(NetworkPlayer _player)
    {
        player = _player;
    }

    public NetworkPlayer GetNetworkPlayer()
    {
        return player;
    }

    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        Debug.Log("Attempting connection...");
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Photon with fixed region...");

            // Set the fixed region to 'asia' (or your preferred region)
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";

            // Connect using the updated settings
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Already connected");
            OnConnectedToMaster();
        }
    }

    /*  public void Connect()
      {
          Debug.Log("Attempting connection...");
          if (!PhotonNetwork.IsConnected)
          {
              Debug.Log("Connecting to Photon...");                      ///////////////changing main
              PhotonNetwork.ConnectUsingSettings();
          }
          else
          {
              Debug.Log("Already connected");
              OnConnectedToMaster();
          }
      }*/

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (voiceClient != null)
        {
            voiceClient.Disconnect();
        }
    }

    public void Disconnect()
    {
        if (voiceClient != null)
        {
            voiceClient.Disconnect();
        }
        PhotonNetwork.Disconnect();
    }

    public void JoinRoom(string roomName)
    {
        bool roomExists = roomList.Exists(room => room.Name == roomName);

        if (roomExists)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogWarning("Room does not exist, creating a new one.");
            CreateRoom(roomName, 60); // Ensure max players does not exceed 20
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom(string roomName, int playerCount)
    {
        int maxPlayers = Mathf.Min(playerCount, 60); // Photon limit is 20 players max

        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = (byte)maxPlayers,
            PublishUserId = true
        };
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public List<string> GetRoomList()
    {
        List<string> list = new List<string>();
        foreach (var roomInfo in roomList)
        {
            list.Add(roomInfo.Name);
        }
        return list;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
        Debug.Log("Updated room list:");
        foreach (var roomInfo in roomList)
        {
            Debug.Log($"Room Name: {roomInfo.Name}, Players: {roomInfo.PlayerCount}/{roomInfo.MaxPlayers}");
        }
        GetRoomList();
    }

    /*  public override void OnConnectedToMaster()
      {
          base.OnConnectedToMaster();
          Debug.Log("Connected to Master");
          if (!LicenseManager.Instance.isPurchased) return;

          if (string.IsNullOrEmpty(roomName))
          {
              roomName = UnityEngine.Random.Range(10000, 99999).ToString();

              CreateRoom(roomName, 60); // Ensure max players does not exceed 20
              joinRoomWithCode = true;
              SceneManager.LoadScene(1);
          }
          else
          {
              PhotonNetwork.JoinRoom(roomName); // Try to join specified room
              joinRoomWithCode = true;
          }
          GetRoomList();
      }*/





    /* public override void OnConnectedToMaster()
     {
         base.OnConnectedToMaster();
         Debug.Log("Connected to Master");
         // Wait for license check to complete before proceeding
         StartCoroutine(WaitForLicenseAndProceed());
     }

     private IEnumerator WaitForLicenseAndProceed()
     {
         // Wait for license check to complete
         yield return StartCoroutine(LicenseManager.Instance.WaitForLicenseCheck());
         Debug.Log($"License check complete. isPurchased: {LicenseManager.Instance.isPurchased}");

        if (!LicenseManager.Instance.isPurchased)
         {
             Debug.LogWarning("License validation failed - access denied");
             // Show error message to user
             yield break; // Use yield break instead of return
         }

         // License is valid, proceed with room operations
         if (string.IsNullOrEmpty(roomName))
         {
             roomName = UnityEngine.Random.Range(10000, 99999).ToString();
             CreateRoom(roomName, 60);
             joinRoomWithCode = true;
             SceneManager.LoadScene(1);
         }
         else
         {
             PhotonNetwork.JoinRoom(roomName);
             joinRoomWithCode = true;
         }
         GetRoomList();
     }



 */



    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby(); // ? This is required before matchmaking
    }




    private IEnumerator WaitForLicenseAndProceed()
    {
        yield return StartCoroutine(LicenseManager.Instance.WaitForLicenseCheck());
        Debug.Log($"License check complete. isPurchased: {LicenseManager.Instance.isPurchased}");

        if (!LicenseManager.Instance.isPurchased)
        {
            Debug.LogWarning("License validation failed - access denied");
            yield break;
        }

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = UnityEngine.Random.Range(10000, 99999).ToString();
            CreateRoom(roomName, 60);
            joinRoomWithCode = true;
            SceneManager.LoadScene(1);
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
            joinRoomWithCode = true;
        }

        GetRoomList();
    }



    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Lobby");
        StartCoroutine(WaitForLicenseAndProceed());
        GetRoomList();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected: {cause}");
        Application.Quit();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Random room join failed: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room join failed: {message}");

        if (joinRoomWithCode)
        {
            joinRoomWithCode = false;
            Debug.Log("Attempting to create room...");
            CreateRoom(roomName, 60); // Ensure max players does not exceed 20
        }
        else
        {
            Debug.LogError("Failed to join or create the room.");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed: {message}");
        if (joinRoomWithCode)
        {
            joinRoomWithCode = false;
            JoinRoom(roomName);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        JoinedRoom?.Invoke();
        roomName = PhotonNetwork.CurrentRoom.Name;

        // Connect the voice client only after joining the room
        if (voiceClient != null && !voiceClient.Client.IsConnected)
        {
            Debug.Log("Connecting voice client to the room...");
            voiceClient.ConnectAndJoinRoom();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entered: {newPlayer.NickName}");
        if (LoadModelFromURL.Instance != null)
        {
            LoadModelFromURL.Instance.OnOtherJoinRoom();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left: {otherPlayer.NickName}");
    }
}



