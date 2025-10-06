using UnityEngine;

public class DrawingTracker : MonoBehaviour
{
    [HideInInspector]
    public string drawingId;

    private void OnDestroy()
    {
        // When the drawing is destroyed (erased), mark it as deleted
        if (!string.IsNullOrEmpty(drawingId))
        {
            Pen.MarkDrawingAsDeleted(drawingId);
        }
    }
}

