using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class LaserPointer : Tool, IPunObservable, IDeletable, IColorChangable
{
    public InputActionProperty laserButton;  // Trigger button to activate the laser
    public InputActionProperty changeColor;

    [SerializeField] private GameObject laserPrefab; // Prefab for the laser (a GameObject with a LineRenderer)
    [SerializeField] private GameObject glowPrefab;  // Prefab for the glowing circular endpoint (e.g., a sprite or particle system)

    [Header("Laser Properties")]
    public Transform laserOrigin; // Point where the laser originates
    public Material laserMaterial;
    public float laserLength = 10.0f; // Maximum length of the laser
    public Color[] laserColors;

    private LineRenderer laser;
    private int currentColorIndex;
    private bool isHolding;
    private PhotonView photonView;

    [SerializeField] private LayerMask layerMask; // To detect what the laser points to
    private GameObject glowInstance; // The instance of the glowing circular endpoint

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        currentColorIndex = 0;

        // Set the initial color of the laser
        laserMaterial.color = laserColors[currentColorIndex];

        // Subscribe to the trigger button action
        laserButton.action.performed += (S) => { isHolding = true; };
        laserButton.action.canceled += (S) => { isHolding = false; };

        // Subscribe to the color change action
        changeColor.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };

        // Subscribe to the delete action (if applicable)
        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
            }
        };
    }

    [PunRPC]
    void DestroyOverNetwork()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        bool isFiringLaser = isGrabbed && isHolding;

        if (isFiringLaser)
        {
            photonView.RPC(nameof(FireLaser), RpcTarget.All);
        }
        else if (laser != null)
        {
            DisableLaser();
        }
    }

    [PunRPC]
    private void FireLaser()
    {
        // Initialize the laser if it hasn't been instantiated
        if (laser == null)
        {
            // Instantiate the laser prefab at the laser origin position and rotation
            GameObject laserObject = Instantiate(laserPrefab, laserOrigin.position, laserOrigin.rotation);
            laserObject.transform.SetParent(laserOrigin); // Set the parent to laserOrigin to keep it attached

            laser = laserObject.GetComponent<LineRenderer>(); // Get the LineRenderer from the instantiated laser
            laser.material = laserMaterial; // Apply material
            laser.startColor = laserColors[currentColorIndex];
            laser.endColor = laserColors[currentColorIndex];
            laser.positionCount = 2; // Laser has two points (start and end)
        }

        // Set the starting point of the laser
        laser.SetPosition(0, laserOrigin.position);

        // Cast a ray from the laser origin
        // Declare the RaycastHit variable
        // Declare the RaycastHit variable
        RaycastHit hit;

        // Cast a ray from the laser origin
        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, laserLength, layerMask))
        {
            // Set the endpoint of the laser at the hit point
            laser.SetPosition(1, hit.point);

            // Handle the glowing circular endpoint at the hit point
            if (glowInstance == null)
            {
                // Create the glowing object at the hit point with a slight offset to avoid z-fighting
                Vector3 offset = hit.normal * 0.01f; // Ensure offset is calculated after hit
                glowInstance = Instantiate(glowPrefab, hit.point + offset, Quaternion.identity);
            }
            else
            {
                // Move the glowing object to the new hit point with the offset
                Vector3 offset = hit.normal * 0.01f; // Calculate offset here again
                glowInstance.transform.position = hit.point + offset;
            }

            // Optional: Adjust the rotation of the glow to face outward from the hit point
            glowInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            // Set the endpoint of the laser to its maximum length if no hit
            laser.SetPosition(1, laserOrigin.position + laserOrigin.forward * laserLength);

            // Move the glowing object to the max length position (no offset needed in this case)
            if (glowInstance == null)
            {
                glowInstance = Instantiate(glowPrefab, laser.GetPosition(1), Quaternion.identity);
            }
            else
            {
                glowInstance.transform.position = laser.GetPosition(1);
            }
        }

        

    }

    private void DisableLaser()
    {
        if (laser != null)
        {
            Destroy(laser.gameObject); // Destroy the laser GameObject
            laser = null; // Clear the reference
        }

        // Destroy the glow instance if it exists
        if (glowInstance != null)
        {
            Destroy(glowInstance);
            glowInstance = null;
        }
    }

    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % laserColors.Length; // Cycle through colors

        
        if (laser != null)
        {
            laser.startColor = laserColors[currentColorIndex];
            laser.endColor = laserColors[currentColorIndex];
        }

        laserMaterial.color = laserColors[currentColorIndex]; 
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isHolding);
            stream.SendNext(isGrabbed);
        }
        else
        {
            isHolding = (bool)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }

    public Color GetCurrentColor()
    {
        return laserColors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        
    }
}
