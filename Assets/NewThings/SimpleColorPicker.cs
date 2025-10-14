using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class SimpleColorPicker : MonoBehaviour
{
    [Header("UI References")]
    public RawImage paletteImage;       // assign your color palette image
    public RectTransform paletteRect;   // same image's RectTransform

    private Texture2D paletteTexture;

    private void Start()
    {
        if (paletteImage != null)
        {
            paletteTexture = paletteImage.texture as Texture2D;
        }

        if (paletteTexture == null)
            Debug.LogError("? Palette texture missing!");
    }

    /// <summary>
    /// Called from UI EventTrigger ? Pointer Click
    /// </summary>
    public void OnClickPalette(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        if (paletteTexture == null)
            return;

        // Convert click/touch/ray position to local rect coords
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            paletteRect,
            pointerData.position,
            pointerData.pressEventCamera,
            out localPoint))
        {
            return;
        }

        Rect rect = paletteRect.rect;
        float uvX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        float uvY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

        // Sample color from palette
        int texX = Mathf.Clamp(Mathf.RoundToInt(uvX * paletteTexture.width), 0, paletteTexture.width - 1);
        int texY = Mathf.Clamp(Mathf.RoundToInt(uvY * paletteTexture.height), 0, paletteTexture.height - 1);

        Color pickedColor = paletteTexture.GetPixel(texX, texY);

        Debug.Log($"?? Picked color: {pickedColor}");

        ApplyColorToSelectedModel(pickedColor);
    }

    private void ApplyColorToSelectedModel(Color pickedColor)
    {
        // Get the selected model from SelectionManager
        ModelObject selectedModel = SelectionManager.Instance?.GetSelectedModel();

        if (selectedModel == null)
        {
            Debug.LogWarning("?? No model selected in SelectionManager.");
            return;
        }

        // Try to get ModelPart component
        ModelPart modelPart = selectedModel.GetComponent<ModelPart>();

        if (modelPart == null)
        {
            Debug.LogWarning("?? Selected model doesn't have ModelPart component.");
            return;
        }

        PhotonView view = modelPart.GetComponent<PhotonView>();

        if (view == null)
        {
            Debug.LogError("? No PhotonView on selected model!");
            return;
        }

        // Request ownership if we don't own it
        if (!view.IsMine)
        {
            view.RequestOwnership();
        }

        // Call RPC to apply color across network
        view.RPC("ApplyCustomColorRPC", RpcTarget.AllBuffered, pickedColor.r, pickedColor.g, pickedColor.b);

        Debug.Log($"? Color applied to selected model: {selectedModel.gameObject.name}");
    }
}