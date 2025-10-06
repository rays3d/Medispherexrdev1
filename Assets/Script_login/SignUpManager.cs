using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class SignUpManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button signUpButton;
    public TextMeshProUGUI messageText;

    private string apiUrl = "https://medispherexr.com/api/apis/auth/signup.php";

    void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpButtonClicked);
    }

    public void OnSignUpButtonClicked()
    {
        string name = nameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        // Debugging input values
        Debug.Log("Name: " + name);
        Debug.Log("Email: " + email);
        Debug.Log("Password: " + password);

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "All fields must be filled.";
            return;
        }

        StartCoroutine(SignUpCoroutine(name, email, password));
    }

    IEnumerator SignUpCoroutine(string name, string email, string password)
    {
        // Create an instance of the SignUpData class with role set to "user"
        SignUpData signUpData = new SignUpData
        {
            name = name,
            email = email,
            password = password,
            role = "user" // Set role to "user" by default
        };

        // Convert the SignUpData instance to JSON
        string jsonData = JsonUtility.ToJson(signUpData);

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
                messageText.text = "Sign up failed: " + www.error;
            }
        }
    }

    [System.Serializable]
    public class SignUpData
    {
        public string name;
        public string email;
        public string password;
        public string role; // Role is included but set to "user" in code
    }

    [System.Serializable]
    public class ResponseData
    {
        public string message;
    }
}

public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // Bypass certificate validation
    }
}
