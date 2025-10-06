using UnityEngine;
using UnityEngine.UI;

public class PlaySoundOnToggle : MonoBehaviour
{
    public Toggle toggle;           // Reference to the Toggle
    public AudioSource audioSource; // Reference to the AudioSource

    void Start()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

   public void OnToggleChanged(bool isOn)
    {
        if (isOn && audioSource != null)
        {
            audioSource.Play();
        }
    }
}
