using UnityEngine;

public class QuestRecorder : MonoBehaviour
{
    private bool isRecording = false;

    public void ToggleRecording()
    {
        if (!isRecording)
        {
            // Start recording
            //OVRPlugin.Media.StartRecording();
            isRecording = true;
            Debug.Log("Recording started!");
        }
        else
        {
            // Stop recording
          //  OVRPlugin.Media.StopRecording();
            isRecording = false;
            Debug.Log("Recording stopped!");
        }
    }
}
