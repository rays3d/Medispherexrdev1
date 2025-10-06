// DeletionSyncUtility.cs
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public static class DeletionSyncUtility
{
    private static HashSet<string> deletedIDs = new HashSet<string>();

    public static bool IsModelDeletedGlobally(string modelID)
    {
        if (deletedIDs.Contains(modelID))
            return true;

        if (PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("deletedModels", out object deletedObj) &&
            deletedObj is Hashtable deletedModels &&
            deletedModels.ContainsKey(modelID))
        {
            deletedIDs.Add(modelID);
            return true;
        }

        return false;
    }

    public static void AddDeletedModelID(string modelID)
    {
        if (deletedIDs.Contains(modelID))
            return;

        deletedIDs.Add(modelID);

        if (!PhotonNetwork.InRoom) return;

        Hashtable deletedModels = new Hashtable();

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("deletedModels", out object deletedObj) &&
            deletedObj is Hashtable existing)
        {
            foreach (System.Collections.DictionaryEntry entry in existing)
                deletedModels[entry.Key] = entry.Value;
        }

        if (!deletedModels.ContainsKey(modelID))
        {
            deletedModels[modelID] = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "deletedModels", deletedModels } });
        }
    }
}
