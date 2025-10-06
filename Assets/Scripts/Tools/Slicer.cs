/*using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Slicer : Tool, IPunObservable, IDeletable
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

    private void OnApplicationQuit()

    {

        destroyedObjectIDs.Clear();

    }
}
*/
//////////////////////////above is recent 

/*using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Slicer : Tool, IPunObservable, IDeletable
{
    private PhotonView photonView;

    // ? Static list to track destroyed object IDs
    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        // ? If this object has been marked as destroyed already, destroy it locally
        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))
        {
            Destroy(gameObject);
            return;
        }

        // ? Set up delete action (called when button is pressed)
        deleteButton.action.performed += (ctx) =>
        {
            if (isGrabbed)
            {
                DeleteTool();

                // Add ID to list and call RPC to destroy across network
                destroyedObjectIDs.Add(photonView.ViewID.ToString());
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };
    }

    [PunRPC]
    void DestroyOverNetwork()
    {
        // ? Add ID and destroy locally when RPC is received
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

    // ? Optional: Clean up the list when the app quits (not necessary unless you use play mode multiple times in editor)
    private void OnApplicationQuit()
    {
        destroyedObjectIDs.Clear();
    }
}*/

////////////////////////////overall delete
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon;

public class Slicer : Tool, IPunObservable, IDeletable
{
    private PhotonView photonView;
    private string modelID;

    private const string DeletedObjectsKey = "DeletedObjects";

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // Get modelID from instantiation data
        if (photonView.InstantiationData != null && photonView.InstantiationData.Length > 0)
        {
            modelID = photonView.InstantiationData[0] as string;
        }
        else
        {
            Debug.LogWarning("Slicer missing modelID in InstantiationData.");
            modelID = "UNKNOWN_MODEL_ID_" + photonView.ViewID; // fallback
        }
    }

    private void Start()
    {
        // Destroy if this modelID was previously deleted
        if (IsModelDeleted(modelID))
        {
            Destroy(gameObject);
            return;
        }

        deleteButton.action.performed += (ctx) =>
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

    // --- Helper Methods for Deletion Persistence ---
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

    private void OnApplicationQuit()
    {
        // Optional: only useful for editor sessions
        // Destroyed ID list is now stored in room properties
        
    }
}

