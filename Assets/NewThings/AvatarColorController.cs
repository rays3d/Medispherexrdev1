/*using UnityEngine;
using Photon.Pun;

public class AvatarColorController : MonoBehaviour
{
    public Renderer avatarRenderer;  // Renderer 1 (Assign in Inspector)
    public Renderer avatarRenderer1; // Renderer 2
    public Renderer avatarRenderer2; // Renderer 3
    public Renderer avatarRenderer3; // Renderer 4

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>(); // Get the PhotonView component

        // Ensure renderers are assigned (If missing, try to get from children)
        if (avatarRenderer == null)
            avatarRenderer = GetComponentInChildren<Renderer>();

        if (avatarRenderer1 == null)
            avatarRenderer1 = GetComponentInChildren<Renderer>();

        if (avatarRenderer2 == null)
            avatarRenderer2 = GetComponentInChildren<Renderer>();

        if (avatarRenderer3 == null)
            avatarRenderer3 = GetComponentInChildren<Renderer>();

        // Apply the correct color based on the player type
        ApplyAvatarColor();
    }

    void ApplyAvatarColor()
    {
        // Define colors (Master - Yellow, Others - Custom Pink)
        Color masterColor = Color.yellow;
        Color otherPlayersColor;
        ColorUtility.TryParseHtmlString("#FF00DA", out otherPlayersColor);

        // Get the Master Client's ActorNumber
        int masterClientID = PhotonNetwork.MasterClient.ActorNumber;

        // Assign color: If this player is the master, use Yellow; otherwise, use Pink
        Color assignedColor = (photonView.Owner.ActorNumber == masterClientID) ? masterColor : otherPlayersColor;

        // Apply the assigned color
        ApplyColorToRenderer(avatarRenderer, assignedColor);
        ApplyColorToRenderer(avatarRenderer1, assignedColor);
        ApplyColorToRenderer(avatarRenderer2, assignedColor);
        ApplyColorToRenderer(avatarRenderer3, assignedColor);
    }

    // Function to safely apply color to a renderer
    void ApplyColorToRenderer(Renderer renderer, Color color)
    {
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetColor("_BaseColor", color); // URP Shader Property
            }
        }
    }
}
*/


using UnityEngine;
using Photon.Pun;

public class AvatarColorController : MonoBehaviour
{
    public Renderer avatarRenderer;  // Renderer 1 (Assign in Inspector)
    public Renderer avatarRenderer1; // Renderer 2
    public Renderer avatarRenderer2; // Renderer 3
    public Renderer avatarRenderer3; // Renderer 4

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>(); // Get the PhotonView component

        // Ensure renderers are assigned (If missing, try to get from children)
        if (avatarRenderer == null)
            avatarRenderer = GetComponentInChildren<Renderer>();

        if (avatarRenderer1 == null)
            avatarRenderer1 = GetComponentInChildren<Renderer>();

        if (avatarRenderer2 == null)
            avatarRenderer2 = GetComponentInChildren<Renderer>();

        if (avatarRenderer3 == null)
            avatarRenderer3 = GetComponentInChildren<Renderer>();

        // Apply the correct color based on the player type
        ApplyAvatarColor();
    }

    void ApplyAvatarColor()
    {
        // Define colors (Master - Light Blue with 50% Transparency, Others - Custom Pink)
        Color masterColor;
        Color otherPlayersColor;
        ColorUtility.TryParseHtmlString("#5ACAED", out masterColor); // Light Blue
        ColorUtility.TryParseHtmlString("#FF00DA", out otherPlayersColor); // Custom Pink

        // Apply transparency (50%)
        masterColor.a = 0.4f;
        otherPlayersColor.a = 0.4f; // Keep pink fully opaque

        // Get the Master Client's ActorNumber
        int masterClientID = PhotonNetwork.MasterClient.ActorNumber;

        // Assign color: If this player is the master, use Light Blue; otherwise, use Pink
        Color assignedColor = (photonView.Owner.ActorNumber == masterClientID) ? masterColor : otherPlayersColor;

        // Apply the assigned color
        ApplyColorToRenderer(avatarRenderer, assignedColor);
        ApplyColorToRenderer(avatarRenderer1, assignedColor);
        ApplyColorToRenderer(avatarRenderer2, assignedColor);
        ApplyColorToRenderer(avatarRenderer3, assignedColor);
    }

    // Function to safely apply color to a renderer
    void ApplyColorToRenderer(Renderer renderer, Color color)
    {
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetColor("_BaseColor", color); // URP Shader Property
            }
        }
    }
}
