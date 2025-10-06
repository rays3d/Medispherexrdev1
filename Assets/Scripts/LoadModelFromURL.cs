
using TriLibCore;
using UnityEngine;
using System;

public class LoadModelFromURL : MonoBehaviourSingleton<LoadModelFromURL>
{
    public string ModelURL;
    public GameObject loadedObject;
    [SerializeField] XRCompactableManager xrCompatableManager;
    public static Action modelLoaded;


    private void Start()//test
    {
      /*  if (FirebaseManager.Instance.modelDatabase.Count > 0)
        {
            ModelURL = FirebaseManager.Instance.modelDatabase[0].downloadURL;
            NetworkManager.instance.GetNetworkPlayer().LoadModelInNetwork(ModelURL);
            LoadModel();
        }*/
        FirebaseManager.OnDataLoaded += () =>
        {
           
        };
    }

    public void OnOtherJoinRoom()
    {
        ModelURL = FirebaseManager.Instance.modelDatabase[0].downloadURL;
        NetworkManager.instance.GetNetworkPlayer().LoadModelInNetwork(ModelURL);
        LoadModel();
    }
    private void LoadModel()
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        var webRequest = AssetDownloader.CreateWebRequest(ModelURL);
        AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
    }
    public void LoadModel(string url, Action<AssetLoaderContext, float> OnProgress)
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        var webRequest = AssetDownloader.CreateWebRequest(url);
        AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
    }
    private void OnError(IContextualizedError obj)
    {
        Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
    }

    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        Debug.Log($"Loading Model. Progress: {progress:P}");
    }

    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Materials loaded. Model fully loaded.");
        xrCompatableManager.MakeModelXRCompactable(loadedObject);

    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model loaded. Loading materials.");
        loadedObject = assetLoaderContext.RootGameObject;
        modelLoaded?.Invoke();
    }

    public void MakeItXR( GameObject netWorkObject)
    {
        xrCompatableManager.MakeModelXRCompactable(loadedObject, netWorkObject);

    }
}
