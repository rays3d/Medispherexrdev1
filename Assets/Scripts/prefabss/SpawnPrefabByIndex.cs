using UnityEngine;

public class SpawnPrefabByIndex : MonoBehaviour
{
    public GameObject[] prefabs;  // Array of prefabs to choose from
    public float distanceFromCamera = 2.0f;  // Distance in front of the camera to spawn the prefab
    public float heightOffset = 1.0f;  // Adjust this based on the prefab's height to place the prefab correctly

    public void SpawnPrefabByIndexm(int index)
    {
        // Make sure the index is within the range of the prefabs array
        if (index < 0 || index >= prefabs.Length)
        {
            Debug.LogError("Prefab index out of range!");
            return;
        }

        // Get the selected prefab
        GameObject prefabToSpawn = prefabs[index];

        // Get the main camera's transform
        Transform cameraTransform = Camera.main.transform;

        // Calculate the spawn position relative to the camera
        Vector3 offset = new Vector3(0, heightOffset, distanceFromCamera);
        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * distanceFromCamera + Vector3.up * heightOffset;

        // Instantiate the prefab at the calculated position with the same rotation as the camera
        Instantiate(prefabToSpawn, spawnPosition, cameraTransform.rotation);
    }
}
