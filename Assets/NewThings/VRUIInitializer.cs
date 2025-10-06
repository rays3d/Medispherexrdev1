using UnityEngine;
using System.Collections;

public class VRUIInitializer : MonoBehaviour
{
    public Transform vrCamera;
    public float distance = 2f;
    public float height = 1.5f;

    void Start()
    {
        StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        // Wait for VR to initialize
        yield return new WaitForSeconds(0.5f);

        if (vrCamera == null)
            vrCamera = Camera.main.transform;

        PositionCanvas();
    }

    void PositionCanvas()
    {
        Vector3 position = vrCamera.position + vrCamera.forward * distance;
        position.y = vrCamera.position.y + height;

        transform.position = position;
        transform.LookAt(vrCamera);
        transform.Rotate(0, 180, 0);
    }
}