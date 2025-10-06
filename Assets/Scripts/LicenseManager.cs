/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LicenseManager : MonoBehaviourSingletonPersistent<LicenseManager>
{
    public bool isPurchased;
    DateTime purchaseTime;
    [SerializeField] int licenseTimeInDays = 7;
    [SerializeField] TMP_Text licenseTimeText;

    private void Start()
    {

        if (!PlayerPrefs.HasKey("Purchaed"))
        {
            // new one
            purchaseTime = DateTime.Now.AddDays(licenseTimeInDays);
            PlayerPrefs.SetInt("Purchaed", 1);
            isPurchased = true;
           // licenseTimeText.text = "License Time : " + (purchaseTime.Subtract(DateTime.Now)).ToString("dd") + "Days";
            PlayerPrefs.SetString("PurchasedDate", purchaseTime.Ticks.ToString());
        }
        else
        {
            purchaseTime = new DateTime(long.Parse(PlayerPrefs.GetString("PurchasedDate")));
            if (purchaseTime.Subtract(DateTime.Now).Days > 0)
            {
                isPurchased = true;
               // licenseTimeText.text = "License Time : " + (purchaseTime.Subtract(DateTime.Now)).ToString("dd") + "Days";
            }
            else
            {
                licenseTimeText.text = $"License Expired!\n Please Contact RaysXRLabs.";
                PlayerPrefs.SetInt("Purchaed", 0);
                isPurchased = false;
            }

        }

    }
}
*/
/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LicenseManager : MonoBehaviourSingletonPersistent<LicenseManager>
{
    public bool isPurchased;
    DateTime purchaseTime;

    // Set license time to 1 hour for testing
    [SerializeField] int licenseTimeInHours = 1;

    [SerializeField] TMP_Text licenseTimeText;

    private void Start()
    {
        // Check if the license has been purchased before
        if (!PlayerPrefs.HasKey("Purchased"))
        {
            // If not purchased, set the new license expiry time
            purchaseTime = DateTime.Now.AddHours(licenseTimeInHours);
            PlayerPrefs.SetInt("Purchased", 1);
            isPurchased = true;

            // Save the purchase date as ticks (long value)
            PlayerPrefs.SetString("PurchasedDate", purchaseTime.Ticks.ToString());

            // Update the license time display
            UpdateLicenseText(purchaseTime.Subtract(DateTime.Now));
        }
        else
        {
            // Retrieve the previously saved purchase date
            purchaseTime = new DateTime(long.Parse(PlayerPrefs.GetString("PurchasedDate")));

            // Check if the license is still valid
            TimeSpan timeRemaining = purchaseTime.Subtract(DateTime.Now);
            if (timeRemaining.TotalHours > 0)
            {
                isPurchased = true;

                // Update the license time display
                UpdateLicenseText(timeRemaining);
            }
            else
            {
                // License has expired
                licenseTimeText.text = $"License Expired!\n Please Contact RaysXRLabs.";
                PlayerPrefs.SetInt("Purchased", 0);
                isPurchased = false;
            }
        }
    }

    // Helper function to update the remaining license time display
    private void UpdateLicenseText(TimeSpan remainingTime)
    {
        licenseTimeText.text = $"License Time: {remainingTime.Hours} Hours";
    }
}*/




////////////////////////////////////////////////////////working   i ma using////////////////////


/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LicenseManager : MonoBehaviourSingletonPersistent<LicenseManager>
{
    public bool isPurchased;
    DateTime purchaseTime;

    // Set license time to 14 days
    [SerializeField] int licenseTimeInDays = 15;

    [SerializeField] TMP_Text licenseTimeText;

    private void Start()
    {
        // Check if the license has been purchased before
        if (!PlayerPrefs.HasKey("Purchased"))
        {
            // If not purchased, set the new license expiry time
            purchaseTime = DateTime.Now.AddDays(licenseTimeInDays);
            PlayerPrefs.SetInt("Purchased", 1);
            isPurchased = true;

            // Save the purchase date as ticks (long value)
            PlayerPrefs.SetString("PurchasedDate", purchaseTime.Ticks.ToString());

            // Update the license time display
            UpdateLicenseText(purchaseTime.Subtract(DateTime.Now));
        }
        else
        {
            // Retrieve the previously saved purchase date
            purchaseTime = new DateTime(long.Parse(PlayerPrefs.GetString("PurchasedDate")));

            // Check if the license is still valid
            TimeSpan timeRemaining = purchaseTime.Subtract(DateTime.Now);
            if (timeRemaining.TotalDays > 0)
            {
                isPurchased = true;

                // Update the license time display
                UpdateLicenseText(timeRemaining);
            }
            else
            {
                // License has expired
                licenseTimeText.text = $"License Expired!\n Please Contact RaysXRLabs.";
                PlayerPrefs.SetInt("Purchased", 0);
                isPurchased = false;
            }
        }
    }

    // Helper function to update the remaining license time display
    private void UpdateLicenseText(TimeSpan remainingTime)
    {
        licenseTimeText.text = $"License Time: {remainingTime.Days} Days";
    }
}
*/




///////////////////////////////////workinggg


/*using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LicenseManager : MonoBehaviourSingletonPersistent<LicenseManager>
{
    public bool isPurchased;
    [SerializeField] private TMP_Text licenseTimeText;
    private string accessToken;
    private const string LOGIN_URL = "http://192.168.1.26/storage_new/src/api/license/vr_login.php";
    private const string VERIFY_URL = "http://192.168.1.26/storage_new/src/api/license/verify_license.php";
    // Remove hardcoded SIMPLE_ID - will get it from logged-in user

    private void Start()
    {
        // Add delay to ensure VRLoginManager has completed login process
        StartCoroutine(DelayedLicenseCheck());
    }

    private IEnumerator DelayedLicenseCheck()
    {
        // Wait 2 seconds to allow VRLoginManager to complete login
        yield return new WaitForSeconds(1f);
        StartCoroutine(CheckAndVerifyLicense());
    }

    // Public method to manually refresh license - useful for testing
    public void RefreshLicense()
    {
        StartCoroutine(CheckAndVerifyLicense());
    }

    // Method to force UI update - call this if UI seems stuck
    public void ForceUpdateUI()
    {
        if (licenseTimeText != null)
        {
            licenseTimeText.SetText(licenseTimeText.text); // Force text mesh pro to update
            Canvas.ForceUpdateCanvases(); // Force canvas update
        }
    }

    private IEnumerator CheckAndVerifyLicense()
    {
        // First, try to use existing access token from VRLoginManager
        string existingToken = PlayerPrefs.GetString("access_token", "");

        if (!string.IsNullOrEmpty(existingToken))
        {
            Debug.Log("Using existing access token from login");
            accessToken = existingToken;
            yield return StartCoroutine(VerifyLicense());
        }
        else
        {
            Debug.Log("No existing token found, performing fresh login");
            yield return StartCoroutine(LoginAndVerifyLicense());
        }
    }

    private IEnumerator LoginAndVerifyLicense()
    {
        // Get the logged-in user's ID from PlayerPrefs (set by VRLoginManager)
        string loggedInUserId = PlayerPrefs.GetString("user_id", "");

        if (string.IsNullOrEmpty(loggedInUserId))
        {
            licenseTimeText.text = "No user logged in!";
            Debug.LogError("No user ID found in PlayerPrefs. User must login first.");
            yield break;
        }

        // Use the actual logged-in user's ID instead of hardcoded value
        string loginJson = $"{{\"simple_id\": \"{loggedInUserId}\"}}";
        using (UnityWebRequest loginRequest = UnityWebRequest.Post(LOGIN_URL, loginJson, "application/json"))
        {
            yield return loginRequest.SendWebRequest();

            if (loginRequest.result == UnityWebRequest.Result.Success)
            {
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(loginRequest.downloadHandler.text);
                if (loginResponse.success)
                {
                    accessToken = loginResponse.access_token;
                    StartCoroutine(VerifyLicense());
                }
                else
                {
                    licenseTimeText.text = "Login Failed: " + loginResponse.message;
                    Debug.LogError($"Login Failed: {loginResponse.message}, Response: {loginRequest.downloadHandler.text}");
                }
            }
            else
            {
                licenseTimeText.text = "Login Error: " + loginRequest.error;
                Debug.LogError($"Login Error: {loginRequest.error}, Response Code: {loginRequest.responseCode}, Response: {loginRequest.downloadHandler.text}");
            }
        }
    }

    private IEnumerator VerifyLicense()
    {
        Debug.Log($"Starting license verification with token: {accessToken}");

        // License verification - only Bearer token needed, no simple_id required
        using (UnityWebRequest verifyRequest = UnityWebRequest.Post(VERIFY_URL, "{}", "application/json"))
        {
            verifyRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            yield return verifyRequest.SendWebRequest();

            Debug.Log($"Verification Response Code: {verifyRequest.responseCode}");
            Debug.Log($"Verification Response: {verifyRequest.downloadHandler.text}");

            if (verifyRequest.result == UnityWebRequest.Result.Success)
            {
                LicenseResponse licenseResponse = JsonUtility.FromJson<LicenseResponse>(verifyRequest.downloadHandler.text);
                Debug.Log($"License Status: {licenseResponse.license_status}, Expiry: {licenseResponse.expiry_date}");

                if (licenseResponse.success && licenseResponse.license_status == "active")
                {
                    isPurchased = true;
                    DateTime expiryDate = DateTime.Parse(licenseResponse.expiry_date);
                    DateTime currentTime = DateTime.Now;
                    TimeSpan timeRemaining = expiryDate - currentTime;

                    Debug.Log($"Current Time: {currentTime}, Expiry Date: {expiryDate}, Days Remaining: {timeRemaining.TotalDays}");

                    if (timeRemaining.TotalDays >= 0)
                    {
                        int daysRemaining = Mathf.CeilToInt((float)timeRemaining.TotalDays);
                        string statusText = $"License Status: {licenseResponse.license_status}\nLicense Time: {daysRemaining} Days";
                        licenseTimeText.text = statusText;
                        Debug.Log($"Updated license text: {statusText}");
                    }
                    else
                    {
                        licenseTimeText.text = "License Expired!\nPlease Contact RaysXRLabs.";
                        isPurchased = false;
                        Debug.Log("License expired - negative time remaining");
                    }
                }
                else
                {
                    licenseTimeText.text = "License Expired!\nPlease Contact RaysXRLabs.";
                    isPurchased = false;
                    Debug.LogError($"Verification Failed: {licenseResponse.message}, Response: {verifyRequest.downloadHandler.text}");
                }
            }
            else
            {
                licenseTimeText.text = "Verification Error: " + verifyRequest.error;
                isPurchased = false;
                Debug.LogError($"Verification Error: {verifyRequest.error}, Response Code: {verifyRequest.responseCode}, Response: {verifyRequest.downloadHandler.text}");
            }
        }
    }

    [System.Serializable]
    private class LoginResponse
    {
        public bool success;
        public string message;
        public string access_token;
        public string refresh_token;
        public string simple_id; // Keep this as the API might return it
    }

    [System.Serializable]
    private class LicenseResponse
    {
        public bool success;
        public string message;
        public int user_id;
        public string license_status;
        public string license_key;
        public string activation_date;
        public string expiry_date;
    }
}*/







using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LicenseManager : MonoBehaviourSingletonPersistent<LicenseManager>
{
    public bool isPurchased = false; // Initialize to false by default
    public bool licenseCheckComplete = false; // Add this flag
    [SerializeField] private bool bypassLicenseInEditor = true; // For development testing
    [SerializeField] private TMP_Text licenseTimeText;
    private string accessToken;
    private const string LOGIN_URL = "https://medispherexr.com/api/src/api/license/vr_login.php";
    private const string VERIFY_URL = "https://medispherexr.com/api/src/api/license/verify_license.php";
    // Remove hardcoded SIMPLE_ID - will get it from logged-in user

    private void Start()
    {
        // Add delay to ensure VRLoginManager has completed login process
        StartCoroutine(DelayedLicenseCheck());
    }
    public void StartLicenseCheck()
    {
        StartCoroutine(CheckAndVerifyLicense()); // ?? Retry license check after login
    }
    private IEnumerator DelayedLicenseCheck()
    {
        // Wait 2 seconds to allow VRLoginManager to complete login
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(CheckAndVerifyLicense());
    }

    // Public method to manually refresh license - useful for testing
    public void RefreshLicense()
    {
        StartCoroutine(CheckAndVerifyLicense());
    }

    // Method to check if license is valid (with editor bypass option)
    public bool IsLicenseValid()
    {
#if UNITY_EDITOR
        if (bypassLicenseInEditor)
        {
            Debug.Log("License check bypassed in editor for development");
            return true;
        }
#endif
        return isPurchased && licenseCheckComplete;
    }

    // Method to wait for license check to complete
    public IEnumerator WaitForLicenseCheck()
    {
        while (!licenseCheckComplete)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Method to force UI update - call this if UI seems stuck
    public void ForceUpdateUI()
    {
        if (licenseTimeText != null)
        {
            licenseTimeText.SetText(licenseTimeText.text); // Force text mesh pro to update
            Canvas.ForceUpdateCanvases(); // Force canvas update
        }
    }

    private IEnumerator CheckAndVerifyLicense()
    {
        // First, try to use existing access token from VRLoginManager
        string existingToken = PlayerPrefs.GetString("access_token", "");

        if (!string.IsNullOrEmpty(existingToken))
        {
            Debug.Log("Using existing access token from login");
            accessToken = existingToken;
            yield return StartCoroutine(VerifyLicense());
        }
        else
        {
            Debug.Log("No existing token found, performing fresh login");
            yield return StartCoroutine(LoginAndVerifyLicense());
        }
    }

    private IEnumerator LoginAndVerifyLicense()
    {
        // Get the logged-in user's ID from PlayerPrefs (set by VRLoginManager)
        string loggedInUserId = PlayerPrefs.GetString("user_id", "");

        if (string.IsNullOrEmpty(loggedInUserId))
        {
            licenseTimeText.text = "No user logged in!";
            Debug.LogError("No user ID found in PlayerPrefs. User must login first.");
            yield break;
        }

        // Use the actual logged-in user's ID instead of hardcoded value
        string loginJson = $"{{\"simple_id\": \"{loggedInUserId}\"}}";
        using (UnityWebRequest loginRequest = UnityWebRequest.Post(LOGIN_URL, loginJson, "application/json"))
        {
            yield return loginRequest.SendWebRequest();

            if (loginRequest.result == UnityWebRequest.Result.Success)
            {
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(loginRequest.downloadHandler.text);
                if (loginResponse.success)
                {
                    accessToken = loginResponse.access_token;
                    StartCoroutine(VerifyLicense());
                }
                else
                {
                    licenseTimeText.text = "Login Failed: " + loginResponse.message;
                    licenseCheckComplete = true; // Mark as complete even on login failure
                    Debug.LogError($"Login Failed: {loginResponse.message}, Response: {loginRequest.downloadHandler.text}");
                }
            }
            else
            {
                licenseTimeText.text = "Login Error: " + loginRequest.error;
                licenseCheckComplete = true; // Mark as complete even on error
                Debug.LogError($"Login Error: {loginRequest.error}, Response Code: {loginRequest.responseCode}, Response: {loginRequest.downloadHandler.text}");
            }
        }
    }

    private IEnumerator VerifyLicense()
    {
        Debug.Log($"Starting license verification with token: {accessToken}");

        // License verification - only Bearer token needed, no simple_id required
        using (UnityWebRequest verifyRequest = UnityWebRequest.Post(VERIFY_URL, "{}", "application/json"))
        {
            verifyRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            yield return verifyRequest.SendWebRequest();

            Debug.Log($"Verification Response Code: {verifyRequest.responseCode}");
            Debug.Log($"Verification Response: {verifyRequest.downloadHandler.text}");

            if (verifyRequest.result == UnityWebRequest.Result.Success)
            {
                LicenseResponse licenseResponse = JsonUtility.FromJson<LicenseResponse>(verifyRequest.downloadHandler.text);
                Debug.Log($"License Status: {licenseResponse.license_status}, Expiry: {licenseResponse.expiry_date}");

                if (licenseResponse.success && licenseResponse.license_status == "active")
                {
                    isPurchased = true;
                    DateTime expiryDate = DateTime.Parse(licenseResponse.expiry_date);
                    DateTime currentTime = DateTime.Now;
                    TimeSpan timeRemaining = expiryDate - currentTime;

                    Debug.Log($"Current Time: {currentTime}, Expiry Date: {expiryDate}, Days Remaining: {timeRemaining.TotalDays}");

                    if (timeRemaining.TotalDays >= 0)
                    {
                        int daysRemaining = Mathf.CeilToInt((float)timeRemaining.TotalDays);
                        string statusText = $"License Status: {licenseResponse.license_status}\nLicense Time: {daysRemaining} Days";
                        licenseTimeText.text = statusText;
                        Debug.Log($"Updated license text: {statusText}");
                        licenseCheckComplete = true; // Mark as complete
                    }
                    else
                    {
                        licenseTimeText.text = "License Expired!\nPlease Contact RaysXRLabs.";
                        isPurchased = false;
                        licenseCheckComplete = true; // Mark as complete even if expired
                        Debug.Log("License expired - negative time remaining");
                    }
                }
                else
                {
                    licenseTimeText.text = "License Expired!\nPlease Contact MedisphereXR.";
                    isPurchased = false;
                    licenseCheckComplete = true; // Mark as complete
                    Debug.LogError($"Verification Failed: {licenseResponse.message}, Response: {verifyRequest.downloadHandler.text}");
                }
            }
            else
            {
                licenseTimeText.text = "Verification Error: " + verifyRequest.error;
                isPurchased = false;
                licenseCheckComplete = true; // Mark as complete even on error
                Debug.LogError($"Verification Error: {verifyRequest.error}, Response Code: {verifyRequest.responseCode}, Response: {verifyRequest.downloadHandler.text}");
            }
        }
    }

    [System.Serializable]
    private class LoginResponse
    {
        public bool success;
        public string message;
        public string access_token;
        public string refresh_token;
        public string simple_id; // Keep this as the API might return it
    }

    [System.Serializable]
    private class LicenseResponse
    {
        public bool success;
        public string message;
        public int user_id;
        public string license_status;
        public string license_key;
        public string activation_date;
        public string expiry_date;
    }
}