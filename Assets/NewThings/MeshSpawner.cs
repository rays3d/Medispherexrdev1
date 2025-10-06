/*using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MeshSpawner : MonoBehaviour
{
    public GameObject meshPrefab;
    public ActionBasedController controller;

    private GameObject spawnedMesh;

    void Update()
    {
        if (controller.selectAction.action.ReadValue<float>() > 0.1f && spawnedMesh == null)
        {
            //  spawnedMesh = Instantiate(meshPrefab, controller.transform.position + controller.transform.forward * 0.5f, Quaternion.identity);
            spawnedMesh = Instantiate(meshPrefab, controller.transform.position + controller.transform.forward * 0.5f, Quaternion.identity);
            MeshDrawer drawer = spawnedMesh.GetComponent<MeshDrawer>();
            drawer.controller = controller;
        }
    }
}

*/


/*using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MeshSpawner : MonoBehaviour
{
    public GameObject meshPrefab;
    public ActionBasedController controller;

    private GameObject spawnedMesh;

    void Update()
    {
        if (controller.selectAction.action.ReadValue<float>() > 0.1f && spawnedMesh == null)
        {
            *//*        spawnedMesh = Instantiate(meshPrefab, controller.transform.position + controller.transform.forward * 0.5f, Quaternion.identity);
                    var dynamicMesh = spawnedMesh.GetComponent<MeshDrawer>();
                    dynamicMesh.controller = controller;*//*

            spawnedMesh = Instantiate(meshPrefab, controller.transform.position + controller.transform.forward * 0.3f, Quaternion.identity);
            var drawer = spawnedMesh.GetComponent<BaffleMeshCreator>();
            drawer.controller = controller;
        }
    }
}
*/
///////////////////////////////// i am working//////////////////////////////////////////////////////////

/*using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MeshSpawner : MonoBehaviour
{
    public GameObject meshPrefab;
    public ActionBasedController controller;

    private GameObject spawnedMesh;

    void Update()
    {
   *//*     if (controller.selectAction.action.ReadValue<float>() > 0.1f && spawnedMesh == null)
        {
            spawnedMesh = Instantiate(meshPrefab, controller.transform.position + controller.transform.forward * 0.5f, Quaternion.identity);
            var dynamicMesh = spawnedMesh.GetComponent<MeshDrawer>();
            dynamicMesh.controller = controller;
        }*//*
        if (controller.activateAction.action.ReadValue<float>() > 0.1f && spawnedMesh == null)
        {
            spawnedMesh = Instantiate(meshPrefab, controller.transform.position + controller.transform.forward * 0.5f, Quaternion.identity);
            var dynamicMesh = spawnedMesh.GetComponent<MeshDrawer>();
            dynamicMesh.controller = controller;
        }
    }
}*/


///////////////////////////////////////cureenlty usingggg///////////

/*using UnityEngine;
using UnityEngine.UI; // Required for UI Button
using UnityEngine.XR.Interaction.Toolkit;

public class MeshSpawner : MonoBehaviour
{
    public GameObject meshPrefab;
    public ActionBasedController controller;
    public Button baffleButton; // Reference to the Baffle button
    private bool isBaffleButtonPressed = false; // Flag to track button state
    private GameObject spawnedMesh;

    void Start()
    {
        // Ensure the button has a listener for the click event
        if (baffleButton != null)
        {
            baffleButton.onClick.AddListener(OnBaffleButtonPressed);
        }
        else
        {
            Debug.LogWarning("Baffle button not assigned in the Inspector!");
        }
    }

    void OnBaffleButtonPressed()
    {
        isBaffleButtonPressed = true;
        Debug.Log("Baffle button pressed. Drawing enabled.");
    }

    void Update()
    {
        // Only spawn mesh if the baffle button is pressed and no mesh exists
        if (isBaffleButtonPressed && controller.activateAction.action.ReadValue<float>() > 0.1f && spawnedMesh == null)
        {
            spawnedMesh = Instantiate(meshPrefab, controller.transform.position + controller.transform.forward * 0.5f, Quaternion.identity);
            var dynamicMesh = spawnedMesh.GetComponent<MeshDrawer>();
            dynamicMesh.controller = controller;
        }
    }
}*/






using UnityEngine;
using UnityEngine.UI; // Required for UI Button
using UnityEngine.XR.Interaction.Toolkit;

public class MeshSpawner : MonoBehaviour
{
    public GameObject meshPrefab;
    public ActionBasedController controller;
    public Button baffleButton; // Reference to the Baffle button
    private bool isBaffleButtonPressed = false; // Flag to track button state
    private GameObject spawnedMesh;
    private MeshDrawer currentMeshDrawer; // Reference to current mesh drawer

    void Start()
    {
        // Ensure the button has a listener for the click event
        if (baffleButton != null)
        {
            baffleButton.onClick.AddListener(OnBaffleButtonPressed);
        }
        else
        {
            Debug.LogWarning("Baffle button not assigned in the Inspector!");
        }
    }

    void OnBaffleButtonPressed()
    {
        isBaffleButtonPressed = true;
        Debug.Log("Baffle button pressed. Drawing enabled.");
    }

    void Update()
    {
        // Check if current mesh has been deleted
        if (spawnedMesh == null && currentMeshDrawer != null)
        {
            // Mesh was deleted, reset the baffle button state
            isBaffleButtonPressed = false;
            currentMeshDrawer = null;
            Debug.Log("Mesh deleted. Click baffle button to draw again.");
        }

        // Only spawn mesh if the baffle button is pressed and no mesh exists
        if (isBaffleButtonPressed && controller.activateAction.action.ReadValue<float>() > 0.1f && spawnedMesh == null)
        {
            spawnedMesh = Instantiate(meshPrefab, controller.transform.position + controller.transform.forward * 0.5f, Quaternion.identity);
            currentMeshDrawer = spawnedMesh.GetComponent<MeshDrawer>();
            currentMeshDrawer.controller = controller;

            // Reset the baffle button state after spawning
            isBaffleButtonPressed = false;
            Debug.Log("Mesh spawned. Baffle button reset - click again for next mesh.");
        }
    }
}