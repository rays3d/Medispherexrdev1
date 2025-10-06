using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class HeadsetDetector : MonoBehaviour
{
    void Start()
    {
        Camera mainCamera = GetComponent<Camera>();
        string deviceModel = GetXRDeviceModel();
        Debug.Log("Detected Device Model: " + deviceModel);

        if (deviceModel.ToLower().Contains("quest2") || deviceModel.ToLower().Contains("quest 2"))
        {
            // Set color for Quest 2 (blue = 255)
            mainCamera.backgroundColor = new Color(0, 0, 1); // RGB(0,0,255)
            Debug.Log("Quest 2 detected - Setting blue to 255");
        }
        else if (deviceModel.ToLower().Contains("quest3") || deviceModel.ToLower().Contains("quest 3"))
        {
            // Set color for Quest 3 (blue = 0)
            mainCamera.backgroundColor = new Color(0, 0, 0); // RGB(0,0,0)
            Debug.Log("Quest 3 detected - Setting blue to 0");
        }
    }

    private string GetXRDeviceModel()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        foreach (var device in devices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
            {
                Debug.Log($"Device found: {device.name} ({device.manufacturer})");
                return device.name;
            }
        }
        return "Unknown Device";
    }
}