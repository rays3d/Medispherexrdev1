using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoutManager : MonoBehaviour
{
    public string logoutAPIUrl = "https://medispherexr.com/api/src/api/auth/logout.php";
    //public string firstSceneName = "Splash"; // Change this to your first scene name

    public void Logout()
    {
        StartCoroutine(LogoutRequest());
    }

    private IEnumerator LogoutRequest()
    {
        string accessToken = PlayerPrefs.GetString("access_token", "");
        string refreshToken = PlayerPrefs.GetString("refresh_token", "");

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            Debug.LogWarning("No access or refresh token found, logging out locally.");
            ClearAndGoToLogin();
            yield break;
        }

        string jsonBody = JsonUtility.ToJson(new RefreshTokenData { refresh_token = refreshToken });

        UnityWebRequest www = new UnityWebRequest(logoutAPIUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
        if (www.isNetworkError || www.isHttpError)
#endif
        {
            Debug.LogWarning("Logout error: " + www.error);
        }
        else
        {
            Debug.Log("Logout success response: " + www.downloadHandler.text);
        }

        // Regardless of success/failure, clear tokens and go back to first scene
        ClearAndGoToLogin();
    }

 
    private void ClearAndGoToLogin()
    {
        PlayerPrefs.DeleteKey("access_token");
        PlayerPrefs.DeleteKey("refresh_token");
        PlayerPrefs.DeleteKey("user_id");
        PlayerPrefs.Save();

        // SceneManager.LoadScene(firstSceneName);
        Debug.Log("Logged out. Exiting application...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif


    }

    [System.Serializable]
    private class RefreshTokenData
    {
        public string refresh_token;
    }
}
