using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHandRayWithAutoDot : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    private GameObject pointerDot;
    public float dotSize = 0.01f;
    public float fallbackDistance = 10f;

    void Start()
    {
        if (rayInteractor == null)
        {
            rayInteractor = GetComponent<XRRayInteractor>();
        }

        // Create the dot
        pointerDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointerDot.name = "PointerDot";
        pointerDot.transform.localScale = Vector3.one * dotSize;
        Destroy(pointerDot.GetComponent<Collider>());

        // Create URP-compatible material
        Material dotMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Color dotColor;
        ColorUtility.TryParseHtmlString("#00A0FF", out dotColor);
        dotMat.color = dotColor;
        dotMat.EnableKeyword("_EMISSION");
        dotMat.SetColor("_EmissionColor", dotColor);

        pointerDot.GetComponent<Renderer>().material = dotMat;
        pointerDot.SetActive(false);
    }

    void Update()
    {
        if (rayInteractor == null || pointerDot == null) return;

        // Show only when hovering or selecting
        bool isHovering = rayInteractor.interactablesHovered.Count > 0;
        bool isSelecting = rayInteractor.interactablesSelected.Count > 0;

        if ((isHovering || isSelecting) && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            pointerDot.transform.position = hit.point;
            pointerDot.SetActive(true);
        }
        else
        {
            pointerDot.SetActive(false);
        }
    }
}
