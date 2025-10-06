using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using UnityEngine.SceneManagement;



public class connecttoserver : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("inside1");
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinedLobby()
    {
        Debug.Log("inside2");
        SceneManager.LoadScene(2);
    }


}
