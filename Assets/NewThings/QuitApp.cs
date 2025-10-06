using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;


public class QuitApp : MonoBehaviour
{
    // Call this method from your UI Button
    public void QuitApplication()
    {
        Debug.Log("Application Quit triggered");

        // Works in a built app
        Application.Quit();

        // This line is just for testing in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
