/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class VRLoginManager : MonoBehaviour
{
    public TMP_InputField userIDInput;
    public Button loginButton;
    public string loginAPIUrl = "http://192.168.1.26/storage_new/src/auth/vr_login.php";

    public GameObject n1;
    public GameObject n2;
    public GameObject n3;
    public GameObject n4;

    private void Start()
    {
        loginButton.onClick.AddListener(Login);
    }

    public void Login()
    {
        string userId = userIDInput.text.Trim();

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("User ID is empty!");
            return;
        }

        StartCoroutine(LoginRequest(userId));
    }

    private IEnumerator LoginRequest(string userId)
    {
        string jsonBody = JsonUtility.ToJson(new SimpleIdWrapper { simple_id = userId });

        UnityWebRequest www = new UnityWebRequest(loginAPIUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
        if (www.isNetworkError || www.isHttpError)
#endif
        {
            Debug.LogError("Login Error: " + www.error + "\n" + www.downloadHandler.text);
        }
        else
        {
            string responseText = www.downloadHandler.text;
            Debug.Log("Login Response: " + responseText);

            ApiResponse result = JsonUtility.FromJson<ApiResponse>(responseText);

            if (result != null && result.success && !string.IsNullOrEmpty(result.access_token))
            {
                PlayerPrefs.SetString("user_id", result.simple_id);
                PlayerPrefs.SetString("access_token", result.access_token);
                PlayerPrefs.SetString("refresh_token", result.refresh_token);
                PlayerPrefs.Save();

                Debug.Log("Login successful. Access Token: " + result.access_token);

                n4.SetActive(false);
                n1.SetActive(true);
                n2.SetActive(true);
                n3.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Login failed or malformed response: " + responseText);
            }
        }
    }

    [System.Serializable]
    private class SimpleIdWrapper
    {
        public string simple_id;
    }

    [System.Serializable]
    private class ApiResponse
    {
        public bool success;
        public string message;
        public string access_token;
        public string refresh_token;
        public string simple_id;
    }
}
*/


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class VRLoginManager : MonoBehaviour
{
    public TMP_InputField userIDInput;
    public Button loginButton;
    public TextMeshProUGUI messageText;

    public string loginAPIUrl = "https://medispherexr.com/api/src/api/license/vr_login.php";
    public string checkLoginAPIUrl = "https://medispherexr.com/api/src/api/license/protected_user_data.php";

    public GameObject n1; // content after login
    public GameObject n2;
    public GameObject n3;
    public GameObject n4; // login UI

    private void Start()
    {
        messageText.gameObject.SetActive(false); // Hide message on start
        loginButton.onClick.AddListener(Login);
        StartCoroutine(CheckIfLoggedIn());
    }

    public void Login()
    {
        messageText.gameObject.SetActive(false); // Clear old message
        string userId = userIDInput.text.Trim();

        if (string.IsNullOrEmpty(userId))
        {
            StartCoroutine(ShowMessage("User ID cannot be empty!", 3f));
            return;
        }

        StartCoroutine(LoginRequest(userId));
    }

    private IEnumerator LoginRequest(string userId)
    {
        string jsonBody = JsonUtility.ToJson(new SimpleIdWrapper { simple_id = userId });

        UnityWebRequest www = new UnityWebRequest(loginAPIUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
        if (www.isNetworkError || www.isHttpError)
#endif
        {
            Debug.LogError("Login Error: " + www.error + "\n" + www.downloadHandler.text);
            StartCoroutine(ShowMessage("Login failed! Please try again.", 3f));
        }
        else
        {
            string responseText = www.downloadHandler.text;
            Debug.Log("Login Response: " + responseText);

            ApiResponse result = JsonUtility.FromJson<ApiResponse>(responseText);

            if (result != null && result.success && !string.IsNullOrEmpty(result.access_token))
            {
                PlayerPrefs.SetString("user_id", result.simple_id);
                PlayerPrefs.SetString("access_token", result.access_token);
                PlayerPrefs.SetString("refresh_token", result.refresh_token);
                PlayerPrefs.Save();

                Debug.Log("Login successful. Access Token: " + result.access_token);

                ShowLoggedInUI();
                StartCoroutine(ShowMessage("Logged in successfully!", 3f));

                LicenseManager.Instance.StartLicenseCheck();
            }
            else
            {
                Debug.LogWarning("Login failed or malformed response: " + responseText);
                StartCoroutine(ShowMessage("Login failed! Please try again.", 3f));
            }
        }
    }

    private IEnumerator CheckIfLoggedIn()
    {
        string token = PlayerPrefs.GetString("access_token", "");
        if (string.IsNullOrEmpty(token))
        {
            ShowLoginUI();
            yield break;
        }

        n4.SetActive(false);

        UnityWebRequest www = UnityWebRequest.Get(checkLoginAPIUrl);
        www.SetRequestHeader("Authorization", "Bearer " + token);

        yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
        if (www.isNetworkError || www.isHttpError)
#endif
        {
            Debug.LogWarning("Token invalid or expired. Logging out...");
            PlayerPrefs.DeleteKey("access_token");
            PlayerPrefs.DeleteKey("refresh_token");
            PlayerPrefs.DeleteKey("user_id");
            PlayerPrefs.Save();
            ShowLoginUI();
        }
        else
        {
            string responseText = www.downloadHandler.text;
            Debug.Log("Check Login Response: " + responseText);

            CheckLoginResponse check = JsonUtility.FromJson<CheckLoginResponse>(responseText);
            if (check != null && check.success)
            {
                Debug.Log("User already logged in: " + check.user.email);
                ShowLoggedInUI();
            }
            else
            {
                ShowLoginUI();
            }
        }
    }

    private void ShowLoginUI()
    {
        n1.SetActive(false);
        n2.SetActive(false);
        n3.SetActive(false);
        n4.SetActive(true);
    }

    private void ShowLoggedInUI()
    {
        n4.SetActive(false);
        n1.SetActive(true);
        n2.SetActive(true);
        n3.SetActive(true);
    }

    private IEnumerator ShowMessage(string message, float duration)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        messageText.gameObject.SetActive(false);
    }

    [System.Serializable]
    private class SimpleIdWrapper
    {
        public string simple_id;
    }

    [System.Serializable]
    private class ApiResponse
    {
        public bool success;
        public string message;
        public string access_token;
        public string refresh_token;
        public string simple_id;
    }

    [System.Serializable]
    private class CheckLoginResponse
    {
        public bool success;
        public string message;
        public UserData user;
    }

    [System.Serializable]
    private class UserData
    {
        public int id;
        public string name;
        public string email;
        public string role;
        public int is_verified;
        public int is_active;
        public string simple_id;
    }
}
