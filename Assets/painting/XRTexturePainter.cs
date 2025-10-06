using UnityEngine;

public class XRTexturePainter : MonoBehaviour
{
    public Transform xrController;       // Reference to the XR controller
    public Camera paintingCamera;        // Camera that renders to the RenderTexture
    public Material modelMaterial;       // Material of the 3D model
    public Material brushMaterial;       // Material for the brush
    public RenderTexture renderTexture;  // RenderTexture where painting happens
    public float brushSize = 0.05f;       // Size of the brush
    public Color brushColor = Color.red; // Color of the brush

    void Start()
    {
        // Check if RenderTexture and Material are set
        if (renderTexture != null)
        {
            modelMaterial.mainTexture = renderTexture;
            paintingCamera.targetTexture = renderTexture;
        }
        if (brushMaterial != null)
        {
            brushMaterial.SetColor("_BrushColor", brushColor);
            brushMaterial.SetFloat("_BrushSize", brushSize);
        }
    }

    void Update()
    {
        // Perform raycast from the XR controller to detect hits on the model
        RaycastHit hit;
        if (Physics.Raycast(xrController.position, xrController.forward, out hit))
        {
            if (hit.collider != null)
            {
                Vector2 uvCoord = hit.textureCoord;
                PaintOnTexture(uvCoord);
            }
        }
    }

    void PaintOnTexture(Vector2 uv)
    {
        if (brushMaterial != null && paintingCamera != null)
        {
            brushMaterial.SetVector("_UVCoord", new Vector4(uv.x, uv.y, 0, 0));
            brushMaterial.SetFloat("_BrushSize", brushSize);
            brushMaterial.SetColor("_BrushColor", brushColor);
            paintingCamera.Render();
        }
    }

    public void SetBrushSize(float newSize)
    {
        brushSize = newSize;
        if (brushMaterial != null)
        {
            brushMaterial.SetFloat("_BrushSize", brushSize);
        }
    }

    public void SetBrushColor(Color newColor)
    {
        brushColor = newColor;
        if (brushMaterial != null)
        {
            brushMaterial.SetColor("_BrushColor", brushColor);
        }
    }
}
