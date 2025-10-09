/*using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirebaseManager : MonoBehaviourSingletonPersistent<FirebaseManager>
{
    public string roomCode => NetworkManager.instance.roomName;
   // public string roomCode = "99156";
    const string FILE_DATA_PARENT = "uploads/";


    public List<Data> modelDatabase;
    public static Action OnDataLoaded;
    void Start()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        NetworkManager.JoinedRoom += AddListner;
    }
    void AddListner()
    {
        FirebaseDatabase.DefaultInstance.GetReference(FILE_DATA_PARENT + roomCode).ValueChanged += OnChangeInVideoData;
    }
    public void LoadVideoData()
    {
        HapticManager.Instance.ActivateHapticRight(.3f, .5f);
        FirebaseDatabase.DefaultInstance.GetReference(FILE_DATA_PARENT + roomCode)
         .GetValueAsync().ContinueWithOnMainThread(task =>
         {
             if (task.IsFaulted)
                 Debug.Log("error");
             else if (task.IsCompleted)
             {
                 modelDatabase.Clear();

                 DataSnapshot snapshot = task.Result;
                 foreach (var item in snapshot.Children)
                 {
                     string json = item.GetRawJsonValue();
                     Debug.Log(json);
                     modelDatabase.Add(JsonUtility.FromJson<Data>(json));
                     OnDataLoaded?.Invoke();
                 }
             }
         });
    }
    void OnChangeInVideoData(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        LoadVideoData();
    }
}
[Serializable]
public class Data
{
    public string fileName;
    public string downloadURL;
}*/