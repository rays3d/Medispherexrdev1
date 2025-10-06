/*using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Eraser : Tool, IPunObservable,IDeletable
{
    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
           }
        };
    }


    [PunRPC]

    void DestroyOverNetwork()
    {
        Destroy(gameObject);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGrabbed);
        }
        else
        {
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }

  
}
*/
/*
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Eraser : Tool, IPunObservable, IDeletable
{
    PhotonView photonView;

    // Static list to track destroyed objects
    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Check if this object should already be destroyed
        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))
        {
            Destroy(gameObject);
            return;
        }

        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();

                // Add the ID to the destroyed objects list
                destroyedObjectIDs.Add(photonView.ViewID.ToString());

                // Notify all clients to destroy this object
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };
    }

    [PunRPC]
    void DestroyOverNetwork()
    {
        // Add to destroyed objects list when any client receives this RPC
        destroyedObjectIDs.Add(photonView.ViewID.ToString());
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGrabbed);
        }
        else
        {
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }

    // Optional: Clear the static list when the room is left
    private void OnApplicationQuit()
    {
        destroyedObjectIDs.Clear();
    }
}


*/

////////////////////////////this  is for overall delete
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon;

public class Eraser : Tool, IPunObservable, IDeletable
{
    private PhotonView photonView;
    private string modelID;
    private const string DeletedObjectsKey = "DeletedObjects";

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // Extract custom modelID from instantiation data
        if (photonView.InstantiationData != null && photonView.InstantiationData.Length > 0)
        {
            modelID = photonView.InstantiationData[0] as string;
        }
    }

    private void Start()
    {
        if (IsModelDeleted(modelID))
        {
            Destroy(gameObject);
            return;
        }

        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                MarkModelDeleted(modelID);
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };
    }

    [PunRPC]
    void DestroyOverNetwork()
    {
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGrabbed);
        }
        else
        {
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }

    private void MarkModelDeleted(string id)
    {
        HashSet<string> deleted = GetDeletedIDs();
        if (!deleted.Contains(id))
        {
            deleted.Add(id);
            Hashtable props = new Hashtable
            {
                [DeletedObjectsKey] = SerializeIDSet(deleted)
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    private bool IsModelDeleted(string id)
    {
        return GetDeletedIDs().Contains(id);
    }

    private HashSet<string> GetDeletedIDs()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(DeletedObjectsKey, out object raw))
        {
            return DeserializeIDSet(raw as string);
        }
        return new HashSet<string>();
    }

    private string SerializeIDSet(HashSet<string> set)
    {
        return string.Join(",", set);
    }

    private HashSet<string> DeserializeIDSet(string str)
    {
        if (string.IsNullOrEmpty(str)) return new HashSet<string>();
        return new HashSet<string>(str.Split(','));
    }
}



