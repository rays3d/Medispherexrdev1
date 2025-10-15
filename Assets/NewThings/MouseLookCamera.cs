using UnityEngine;

public class MouseLookCamera : MonoBehaviour
{
    public float sensitivity = 2f;
    public float moveSpeed = 5f;

    float rotationX = 0f;
    float rotationY = 0f;

    void Update()
    {
        if (Input.GetMouseButton(0)) // Hold right mouse button to look
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp(rotationY, -80f, 80f);
            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(move * moveSpeed * Time.deltaTime);
    }
}
