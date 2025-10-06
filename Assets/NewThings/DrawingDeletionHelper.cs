/*using UnityEngine;

// Simple component to help with drawing deletion - no performance overhead
public class DrawingDeletionHelper : MonoBehaviour
{
    public static void DeleteDrawing(GameObject drawingObject)
    {
        // Get the drawing ID
        DrawingTracker tracker = drawingObject.GetComponent<DrawingTracker>();
        if (tracker != null && !string.IsNullOrEmpty(tracker.drawingId))
        {
            // Mark as deleted in network
            Pen.MarkDrawingAsDeleted(tracker.drawingId);
        }

        // Destroy the drawing object
        Destroy(drawingObject);
    }
}*/