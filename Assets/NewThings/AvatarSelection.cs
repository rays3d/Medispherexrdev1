using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarSelection : MonoBehaviour
{
    [SerializeField] Button[] avatarButtons; // Array of buttons for avatar selection
    [SerializeField] Button nextButton;      // Button to proceed to team screen
    [SerializeField] GameObject avatarPanel; // The panel with avatar buttons
    [SerializeField] GameObject teamPanel;   // The panel to activate after choosing avatar

    public static int selectedAvatarIndex = -1; // Static variable to store selected avatar

    void Start()
    {
        nextButton.interactable = false; // Disable next button until an avatar is selected

        // Add listeners to avatar buttons
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            int index = i; // Capture index for the lambda
            avatarButtons[i].onClick.AddListener(() => SelectAvatar(index));
        }

        nextButton.onClick.AddListener(() => LoadTeamScreen());
    }

    void SelectAvatar(int index)
    {
        selectedAvatarIndex = index;
        nextButton.interactable = true; // Enable next button

        // Highlight by disabling other buttons
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            avatarButtons[i].interactable = (i != index);
        }
    }

    void LoadTeamScreen()
    {
        // Disable avatar panel and show team panel
        avatarPanel.SetActive(false);
        teamPanel.SetActive(true);
    }
}
