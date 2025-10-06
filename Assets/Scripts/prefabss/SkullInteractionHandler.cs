using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SkullInteractionHandler : MonoBehaviourPun
{
    public XRGrabInteractable wholeSkullGrab; // The grab interactable for the whole skull
    public XRGrabInteractable[] partsGrab; // Array of grab interactables for the individual parts
    public Transform[] partsOriginalTransforms; // Array to store original transforms for parts (position, rotation)

    private bool isSeparated = false; // Tracks whether the parts are currently separated

    void Start()
    {
        // Ensure individual part grab is disabled initially
        foreach (var partGrab in partsGrab)
        {
            partGrab.enabled = false; // Disable grab interactables for individual parts
        }
    }

    public void GrabWholeSkull()
    {
        if (!isSeparated)
        {
            // Handle the logic when the whole skull is grabbed
            foreach (var partGrab in partsGrab)
            {
                partGrab.transform.SetParent(this.transform); // Parent parts to the skull
                partGrab.enabled = false; // Disable individual grabbing
            }
        }
    }

    public void SeparateParts()
    {
        if (!isSeparated)
        {
            // Sync separation across the network
            photonView.RPC("RPC_SeparateParts", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_SeparateParts()
    {
        // Separate the parts
        foreach (var partGrab in partsGrab)
        {
            partGrab.enabled = true; // Enable XR Grab Interactable for each part
            partGrab.transform.SetParent(null); // Detach parts from the parent (skull)

            // Ensure Rigidbody settings are correct
            Rigidbody rb = partGrab.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Keep Rigidbody kinematic so it doesn't fly away
                rb.useGravity = false; // Disable gravity
            }

            Collider col = partGrab.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true; // Ensure the collider is enabled
            }

            Debug.Log(partGrab.name + " is separated and kinematic.");
        }

        isSeparated = true; // Mark that the parts are now separated
    }

    public void ReassembleSkull()
    {
        if (isSeparated)
        {
            // Sync reassembly across the network
            photonView.RPC("RPC_ReassembleSkull", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_ReassembleSkull()
    {
        // Reassemble the parts back to the original position
        for (int i = 0; i < partsGrab.Length; i++)
        {
            partsGrab[i].enabled = false; // Disable individual grabbing
            partsGrab[i].transform.position = partsOriginalTransforms[i].position; // Reset position
            partsGrab[i].transform.rotation = partsOriginalTransforms[i].rotation; // Reset rotation
            partsGrab[i].transform.SetParent(this.transform); // Re-parent to skull

            // Keep parts kinematic when reassembled
            Rigidbody rb = partsGrab[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Ensure it's still kinematic
                rb.useGravity = false; // Disable gravity
            }

            Debug.Log(partsGrab[i].name + " is reassembled.");
        }

        isSeparated = false; // Mark that the parts are no longer separated
    }
}
