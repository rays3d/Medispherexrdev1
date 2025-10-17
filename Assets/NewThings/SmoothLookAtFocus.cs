using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToLookAt : MonoBehaviour
{
    [Header("Camera Settings")]
    public float moveSpeed = 5f;          // How fast the camera moves to target
    public float rotateSpeed = 5f;        // How fast the camera rotates to face target
    public float focusDistance = 5f;      // Distance from the target object
    public Vector3 focusOffset = Vector3.up * 0.5f; // Offset for nicer framing
    public LayerMask clickableLayers = ~0;          // Which layers are clickable

    private Camera cam;
    private Transform target;
    private Vector3 defaultPos;
    private Quaternion defaultRot;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        defaultPos = transform.position;
        defaultRot = transform.rotation;
    }

    void Update()
    {
        HandleClick();

        if (target != null)
        {
            FocusOnTarget();
        }
        else
        {
            ReturnToDefault();
        }
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Ignore UI clicks
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f, clickableLayers))
            {
                target = hit.transform;
            }
            else
            {
                // Reset if clicked on empty space
                target = null;
            }
        }
    }

    void FocusOnTarget()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + focusOffset;
        Quaternion desiredRot = Quaternion.LookRotation(targetPos - transform.position);

        Vector3 desiredPos = targetPos - desiredRot * Vector3.forward * focusDistance;

        transform.position = Vector3.Lerp(transform.position, desiredPos, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotateSpeed * Time.deltaTime);
    }

    void ReturnToDefault()
    {
        transform.position = Vector3.Lerp(transform.position, defaultPos, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, defaultRot, rotateSpeed * Time.deltaTime);
    }
}
