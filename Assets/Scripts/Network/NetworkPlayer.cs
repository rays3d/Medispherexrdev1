/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayer : MonoBehaviour
{
    
    PhotonView photonView;
    XRRigMapper mapper;

    [SerializeField] TMP_Text nameText;
    [SerializeField] Transform head;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;

    void Start()
    {

        photonView = GetComponent<PhotonView>();

        int i = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (photonView.Owner == player)
            {
                int playerNumber = i + 1;
                photonView.Controller.NickName = "user " + (playerNumber);
            }
            i++;
        }
        nameText.text = photonView.Controller.NickName;

        mapper = FindObjectOfType<XRRigMapper>();

        if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }
    }

    public void LoadModelInNetwork(string url)
    {
        photonView.RPC(nameof(LoadModelRPC),RpcTarget.Others, url);
    }
    [PunRPC]

    void LoadModelRPC(string url)
    {
        LoadModelFromURL.Instance.LoadModel(url, null);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(head, mapper.headTarget);
            MapPosition(rightHand, mapper.rightHandTarget);
            MapPosition(leftHand, mapper.leftHandTarget);
        }
    }

    void MapPosition(Transform target, Transform rig)
    {
        target.position = rig.position;
        target.rotation = rig.rotation;
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayer : MonoBehaviour
{
    PhotonView photonView;
    XRRigMapper mapper;

    [SerializeField] TMP_Text nameText;
    [SerializeField] Transform head;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        mapper = FindObjectOfType<XRRigMapper>();

        // Set the player's nickname to the UI text element
        if (photonView.IsMine)
        {
            // If this player is mine, use the name they entered
            nameText.text = PhotonNetwork.NickName;

            // Disable rendering for local player
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }
        else
        {
            // For other players, use their Photon nickname
            nameText.text = photonView.Owner.NickName;
        }
    }

    public void LoadModelInNetwork(string url)
    {
        photonView.RPC(nameof(LoadModelRPC), RpcTarget.Others, url);
    }

    [PunRPC]
    void LoadModelRPC(string url)
    {
        LoadModelFromURL.Instance.LoadModel(url, null);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(head, mapper.headTarget);
            MapPosition(rightHand, mapper.rightHandTarget);
            MapPosition(leftHand, mapper.leftHandTarget);
        }
    }

    void MapPosition(Transform target, Transform rig)
    {
        target.position = rig.position;
        target.rotation = rig.rotation;
    }
}
