using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class ServerSessionRequest
{
    public string session_name;
    public string session_data;
    public string user_id;
}

[Serializable]
public class ServerSessionResponse
{
    public bool success;
    public string message;
    public int id;
    public string session_name;
    public SessionData session_data; // ?? Changed: This should be SessionData object, not string
    public string created_at;
    public string updated_at;
}

// ?? NEW: Wrapper for session_data when it comes as nested JSON string
[Serializable]
public class ServerSessionResponseRaw
{
    public bool success;
    public string message;
    public int id;
    public string session_name;
    public string session_data; // Raw JSON string
    public string created_at;
    public string updated_at;
}

public class ServerSessionHandler : MonoBehaviour
{
    [Header("Server Configuration")]
    [SerializeField] private string serverBaseUrl = "http://192.168.1.125/medisphere_api/src/api/session";

    [Header("UI Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [Header("Load Settings")]
    [SerializeField] private int sessionIdToLoad = 15; // ? NEW: Set the session ID you want to load

    [Header("User Settings")]
    [SerializeField] private string currentUserId = "user_123";

    private const string SAVE_ENDPOINT = "/save_session.php";
    private const string RETRIEVE_ENDPOINT = "/retrieve_session.php";

    public event Action<int> OnSessionSaved;
    public event Action<string> OnSessionLoaded;
    public event Action<string> OnError;

    private SessionManager sessionManager;
    private int lastSavedSessionId = 0;

    void Start()
    {
        sessionManager = FindObjectOfType<SessionManager>();
        if (sessionManager == null)
        {
            Debug.LogError("? SessionManager not found!");
        }

        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButtonClicked);

        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadButtonClicked);
    }

    // ===============================
    // BUTTON CALLBACKS
    // ===============================

    private void OnSaveButtonClicked()
    {
        string sessionName = "Session_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        Debug.Log($"?? Saving session '{sessionName}'...");

        if (sessionManager != null)
            sessionManager.SaveSession(sessionName, currentUserId);
        else
            Debug.LogError("? SessionManager not found!");
    }

    private void OnLoadButtonClicked()
    {
        // ? FIX: Use sessionIdToLoad from Inspector instead of lastSavedSessionId
        int idToLoad = lastSavedSessionId > 0 ? lastSavedSessionId : sessionIdToLoad;

        if (idToLoad == 0)
        {
            Debug.LogWarning("?? No session ID specified. Set 'Session Id To Load' in Inspector or save a session first.");
            return;
        }

        Debug.Log($"?? Loading session ID {idToLoad}...");
        if (sessionManager != null)
            sessionManager.LoadSession(idToLoad);
        else
            Debug.LogError("? SessionManager not found!");
    }

    // ===============================
    // PUBLIC METHODS
    // ===============================

    /// <summary>
    /// Set which session ID to load (call this from UI or other scripts)
    /// </summary>
    public void SetSessionIdToLoad(int sessionId)
    {
        sessionIdToLoad = sessionId;
        Debug.Log($"?? Session ID to load set to: {sessionId}");
    }

    // ===============================
    // SERVER METHODS
    // ===============================

    public void SaveSessionToServer(string sessionName, SessionData sessionData, string userId = "")
    {
        string jsonData = JsonUtility.ToJson(sessionData, true);
        StartCoroutine(SaveSessionCoroutine(sessionName, jsonData, userId));
    }

    public void LoadSessionFromServer(int sessionId)
    {
        StartCoroutine(LoadSessionCoroutine(sessionId));
    }

    private IEnumerator SaveSessionCoroutine(string sessionName, string sessionData, string userId)
    {
        string url = serverBaseUrl + SAVE_ENDPOINT;
        string userIdToUse = string.IsNullOrEmpty(userId) ? currentUserId : userId;

        ServerSessionRequest request = new ServerSessionRequest
        {
            session_name = sessionName,
            session_data = sessionData,
            user_id = userIdToUse
        };

        string jsonRequest = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"?? Sending to: {url}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Debug.Log($"?? Server response: {responseText}");

                try
                {
                    ServerSessionResponse response = JsonUtility.FromJson<ServerSessionResponse>(responseText);

                    if (response.success)
                    {
                        Debug.Log($"? Session saved successfully! Session ID: {response.id}");
                        lastSavedSessionId = response.id;
                        sessionIdToLoad = response.id; // ? Also update the load ID
                        OnSessionSaved?.Invoke(response.id);
                    }
                    else
                    {
                        Debug.LogError($"? Save failed: {response.message}");
                        OnError?.Invoke(response.message);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"? Failed to parse response: {e.Message}");
                    OnError?.Invoke(e.Message);
                }
            }
            else
            {
                Debug.LogError($"? Network error: {www.error}");
                OnError?.Invoke(www.error);
            }
        }
    }

    private IEnumerator LoadSessionCoroutine(int sessionId)
    {
        string url = $"{serverBaseUrl}{RETRIEVE_ENDPOINT}?session_id={sessionId}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            Debug.Log($"?? Loading from: {url}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Debug.Log($"?? Raw server response: {responseText}");

                try
                {
                    // ? CRITICAL FIX: Your server returns session_data as a nested JSON object
                    // We need to extract just the session_data part

                    // First, parse to check success
                    var tempResponse = JsonUtility.FromJson<ServerSessionResponseRaw>(responseText);

                    if (!tempResponse.success)
                    {
                        Debug.LogError($"? Server error: {tempResponse.message}");
                        OnError?.Invoke(tempResponse.message);
                        yield break;
                    }

                    Debug.Log($"? Session found! Name: {tempResponse.session_name}, ID: {tempResponse.id}");

                    // ? Extract the session_data object from the response
                    // Find the session_data in the JSON string
                    int startIndex = responseText.IndexOf("\"session_data\":");
                    if (startIndex == -1)
                    {
                        Debug.LogError("? session_data not found in response!");
                        OnError?.Invoke("Invalid response format");
                        yield break;
                    }

                    // Move past "session_data":
                    startIndex += "\"session_data\":".Length;

                    // Skip whitespace
                    while (startIndex < responseText.Length && char.IsWhiteSpace(responseText[startIndex]))
                    {
                        startIndex++;
                    }

                    // Find the matching closing brace for session_data object
                    int braceCount = 0;
                    int endIndex = startIndex;
                    bool inString = false;

                    for (int i = startIndex; i < responseText.Length; i++)
                    {
                        char c = responseText[i];

                        if (c == '"' && (i == 0 || responseText[i - 1] != '\\'))
                        {
                            inString = !inString;
                        }
                        else if (!inString)
                        {
                            if (c == '{') braceCount++;
                            if (c == '}') braceCount--;

                            if (braceCount == 0)
                            {
                                endIndex = i + 1;
                                break;
                            }
                        }
                    }

                    // Extract the session_data JSON
                    string sessionDataJson = responseText.Substring(startIndex, endIndex - startIndex);

                    Debug.Log($"?? Extracted session_data: {sessionDataJson.Substring(0, Mathf.Min(200, sessionDataJson.Length))}...");
                    Debug.Log($"?? Session has {tempResponse.session_name}");

                    OnSessionLoaded?.Invoke(sessionDataJson);
                }
                catch (Exception e)
                {
                    Debug.LogError($"? Failed to parse response: {e.Message}");
                    Debug.LogError($"? Stack: {e.StackTrace}");
                    OnError?.Invoke(e.Message);
                }
            }
            else
            {
                Debug.LogError($"? Network error: {www.error}");
                Debug.LogError($"? Response: {www.downloadHandler.text}");
                OnError?.Invoke(www.error);
            }
        }
    }

    // ===============================
    // UTILITY METHODS
    // ===============================

    public void SetUserId(string userId)
    {
        currentUserId = userId;
        Debug.Log($"?? User ID set to: {currentUserId}");
    }

    public void SetServerUrl(string url)
    {
        serverBaseUrl = url;
        Debug.Log($"?? Server URL set to: {serverBaseUrl}");
    }
}