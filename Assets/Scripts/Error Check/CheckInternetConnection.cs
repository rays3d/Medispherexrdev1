using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInternetConnection : MonoBehaviour
{
    [SerializeField] GameObject retryCanvas;
    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            retryCanvas.SetActive(true);

        }
        else
        {
            retryCanvas.SetActive(false);
        }
    }
}
