using UnityEngine;

public class MouseLookCamera : MonoBehaviour
{
    [Header("Look Settings")]
    public float sensitivity = 2f;
    public float smoothSpeed = 10f;

    [Header("Move Settings")]
    public float moveSpeed = 5f;
    public float scrollSpeed = 50f;
    public float verticalMoveSpeed = 5f; // for Q/E up-down movement

    private float rotationY; // only horizontal rotation
    private Vector3 currentVelocity;

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(0)) // Hold Left Mouse Button
        {
            rotationY += Input.GetAxis("Mouse X") * sensitivity;
            Quaternion targetRotation = Quaternion.Euler(0f, rotationY, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }

    void HandleMovement()
    {
        // Horizontal plane movement (WASD)
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        Vector3 move = (transform.forward * vertical + transform.right * horizontal).normalized;

        // Vertical movement (Q/E)
        float upDown = 0f;
        if (Input.GetKey(KeyCode.E)) upDown += 1f;
        if (Input.GetKey(KeyCode.Q)) upDown -= 1f;
        Vector3 verticalMove = Vector3.up * upDown * verticalMoveSpeed;

        // Scroll forward/back
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 scrollMove = transform.forward * scroll * scrollSpeed;

        // Combine all movement
        Vector3 targetVelocity = (move * moveSpeed) + verticalMove + scrollMove;

        // Smooth movement
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * smoothSpeed);
        transform.position += currentVelocity * Time.deltaTime;
    }
}
