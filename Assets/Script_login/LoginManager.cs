/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI messageText;

    private string apiUrl = "http://192.168.1.26/storage/api/auth/login.php";

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        // Debugging input values
        Debug.Log("Email: " + email);
        Debug.Log("Password: " + password);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Email and password must be filled.";
            return;
        }

        StartCoroutine(LoginCoroutine(email, password));
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        // Create an instance of the LoginData class
        LoginData loginData = new LoginData
        {
            email = email,
            password = password
        };

        // Convert the LoginData instance to JSON
        string jsonData = JsonUtility.ToJson(loginData);

        // Log the JSON data being sent
        Debug.Log("Sending data: " + jsonData);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new BypassCertificateHandler(); // Allow insecure connections

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);
                messageText.text = responseData.message;
            }
            else
            {
                messageText.text = "Login failed: " + www.error;
            }
        }
    }

    [System.Serializable]
    public class LoginData
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class ResponseData
    {
        public string message;
    }
}

*//*public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // Bypass certificate validation
    }
}
*/

/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI messageText;

    private string apiUrl = "https://medispherexr.com/api/apis/auth/login.php";

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        // Debugging input values
        Debug.Log("Email: " + email);
        Debug.Log("Password: " + password);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Email and password must be filled.";
            return;
        }

        StartCoroutine(LoginCoroutine(email, password));
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        // Create an instance of the LoginData class
        LoginData loginData = new LoginData
        {
            email = email,
            password = password
        };

        // Convert the LoginData instance to JSON
        string jsonData = JsonUtility.ToJson(loginData);

        // Log the JSON data being sent
        Debug.Log("Sending data: " + jsonData);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new BypassCertificateHandler(); // Allow insecure connections

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Server response: " + jsonResponse);

                ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

                if (responseData.success)
                {
                    PlayerPrefs.SetString("access_token", responseData.access_token);
                    PlayerPrefs.SetString("refresh_token", responseData.refresh_token);
                    Debug.Log("sucess data");
                    PlayerPrefs.Save();

                    Debug.Log("Tokens saved successfully.");
                    Debug.Log("Proceeding to Scene1.");

                    // Proceed to the next scene
                    SceneManager.LoadScene(1);
                }
                else
                {
                    messageText.text = responseData.message;
                    Debug.Log("next");
                    SceneManager.LoadScene(1);
                }
            }
            else
            {
                messageText.text = "Login failed: " + www.error;
            }
        }
    }

    [System.Serializable]
    public class LoginData
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class ResponseData
    {
        public bool success;
        public string access_token;
        public string refresh_token;
        public string message;
    }
}
*/
/*public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // Bypass certificate validation
    }
}*/


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    // Public fields to connect UI components in the Inspector
    public TMP_InputField emailInput;        // Input field for email
    public TMP_InputField passwordInput;     // Input field for password
    public Button loginButton;                // Button to trigger login
    public TextMeshProUGUI messageText;      // Text area for displaying messages

    // API URL for the login endpoint
    private string apiUrl = "https://medispherexr.com/api/apis/auth/login.php";

    void Start()
    {
        // Add listener to the login button to trigger the login process
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;         // Get email input
        string password = passwordInput.text;   // Get password input

        // Log input values for debugging
        Debug.Log("Email: " + email);
        Debug.Log("Password: " + password);

        // Validate inputs
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Email and password must be filled.";
            return;
        }

        // Start the login coroutine
        StartCoroutine(LoginCoroutine(email, password));
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        // Create an instance of the LoginData class
        LoginData loginData = new LoginData
        {
            email = email,
            password = password
        };

        // Convert the LoginData instance to JSON
        string jsonData = JsonUtility.ToJson(loginData);

        // Log the JSON data being sent
        Debug.Log("Sending data: " + jsonData);

        // Create a UnityWebRequest for the API call
        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            // Convert the JSON data to bytes
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new BypassCertificateHandler(); // Allow insecure connections for testing

            // Send the request and wait for a response
            yield return www.SendWebRequest();

            // Check for network errors
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                messageText.text = "Login failed: " + www.error;
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // Get the response from the server
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Server response: " + jsonResponse);

                // Deserialize the response
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

                // Check if the login was successful
                if (responseData.success)
                {
                    // Save tokens in PlayerPrefs
                    PlayerPrefs.SetString("access_token", responseData.access_token);
                    PlayerPrefs.SetString("refresh_token", responseData.refresh_token);
                    PlayerPrefs.Save();

                    Debug.Log("Tokens saved successfully. Proceeding to next scene.");

                    // Load the next scene only if login was successful
                    SceneManager.LoadScene("Splash"); // Replace with the actual name of your next scene
                }
                else
                {
                    // Display the error message from the response
                    messageText.text = responseData.message;
                    Debug.Log("Login failed: " + responseData.message);
                    SceneManager.LoadScene("Splash");
                }
            }
        }
    }

    // Class to hold login data
    [System.Serializable]
    public class LoginData
    {
        public string email;
        public string password;
    }

    // Class to hold the response data from the API
    [System.Serializable]
    public class ResponseData
    {
        public bool success;
        public string access_token;
        public string refresh_token;
        public string message;
    }
}




// Class to bypass SSL certificate verification (not recommended for production)
/*public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // Bypass certificate validation for testing
    }
}*/


// Class to bypass SSL certificate verification (not recommended for production)
//public class BypassCertificateHandler : CertificateHandler
//{
//  protected override bool ValidateCertificate(byte[] certificateData)
//  {
//      return true; // Bypass certificate validation for testing
//  }
//}