/*using UnityEngine;

public class PrefabVisibility : MonoBehaviour
{
    public GameObject[] prefabs; // Array of prefabs to assign in the inspector
    public float distanceInFront = 10f; // Distance in front of the camera

    void Start()
    {
        if (prefabs.Length == 0)
        {
            Debug.LogError("No prefabs assigned.");
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                // Position each prefab in front of the camera at a different offset
                Vector3 positionInFront = mainCamera.transform.position + (mainCamera.transform.forward * distanceInFront) + new Vector3(i * 2f, 0, 0); // Adjust offset as needed
                prefabs[i].transform.position = positionInFront;

                // Ensure each prefab is oriented correctly
                prefabs[i].transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }
        }
        else
        {
            Debug.LogError("Main camera not found.");
        }
    }
}
*/

using UnityEngine;

public class PrefabVisibility : MonoBehaviour
{
    public GameObject[] prefabs; // Array of prefabs to assign in the inspector
    public float distanceInFront = 10f; // Distance in front of the camera

    void Start()
    {
        if (prefabs.Length == 0)
        {
            Debug.LogError("No prefabs assigned.");
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                // Calculate the position in front of the camera with a horizontal offset
                Vector3 positionInFront = mainCamera.transform.position + (mainCamera.transform.forward * distanceInFront) + new Vector3(i * 2f, 0, 0);

                // Set the prefab's position and rotation
                prefabs[i].transform.position = positionInFront;
                prefabs[i].transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }
        }
        else
        {
            Debug.LogError("Main camera not found.");
        }
    }
}

