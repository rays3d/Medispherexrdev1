using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{

    GameObject playerObject;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Vector3 pos = new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y, transform.position.z);
        playerObject = PhotonNetwork.Instantiate("Avatar", pos, transform.rotation);
        NetworkManager.instance.SetNetworkPlayer(playerObject.GetComponent<NetworkPlayer>());
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(playerObject);
    }
}




