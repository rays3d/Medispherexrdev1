/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class MeasurmentInk : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform canvas;
    [SerializeField] TMP_Text lengthText;
    [Header("Points :-")]
    [SerializeField] GameObject point1;
    [SerializeField] GameObject point2;



    public void SetLineAndMesurments(Vector3 startPos, Vector3 endPos)
    {

        photonView.RPC(nameof(SetLineAndMesurmentsRPC), RpcTarget.All, startPos, endPos);
    }
    [PunRPC]
    public void SetLineAndMesurmentsRPC(Vector3 startPos, Vector3 endPos)
    {
        lineRenderer.startWidth = lineRenderer.endWidth = .001f;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        point1.transform.position = startPos;
        lineRenderer.SetPosition(1, endPos);
        point2.transform.position = endPos;
        float distanceInM = Vector3.Distance(startPos, endPos);

        Vector3 mid = new((startPos.x + endPos.x) / 2, (startPos.y + endPos.y) / 2, (startPos.z + endPos.z) / 2);
        canvas.position = mid;
        lengthText.text = $"{(distanceInM * 1000):0.00} mm";

        Transform head = FindObjectOfType<XRRigMapper>().headTarget;
        canvas.LookAt(head);
        //transform.forward *= -1;


        gameObject.AddComponent<Deletable>();
        gameObject.AddComponent<BoxCollider>().isTrigger = true;
        if (SelectionManager.Instance.GetSelectedModel())
            transform.parent = SelectionManager.Instance.GetSelectedModel().transform;
    }

    public void SetPointOne(Vector3 pos)
    {
        point1.transform.position = pos;

    }
}*/

// above is correct

/*using UnityEngine;
using TMPro;
using Photon.Pun;

public class MeasurmentInk : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform canvas;
    [SerializeField] TMP_Text lengthText;
    [Header("Points")]
    [SerializeField] GameObject point1;
    [SerializeField] GameObject point2;

    public void SetLineAndMesurments(Vector3 startPos, Vector3 endPos)
    {
        photonView.RPC(nameof(SetLineAndMesurmentsRPC), RpcTarget.All, startPos, endPos);
    }

    [PunRPC]
    public void SetLineAndMesurmentsRPC(Vector3 startPos, Vector3 endPos)
    {
        // Set line properties
        lineRenderer.startWidth = lineRenderer.endWidth = .001f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Set point positions
        point1.transform.position = startPos;
        point2.transform.position = endPos;

        // Calculate distance in world space to ensure scale-independence
        float distanceInWorldSpace = Vector3.Distance(startPos, endPos);

        // Convert to millimeters with scaling adjustment
        float scaleFactor = 0.3f; // Adjustment factor to match other software
        float distanceInMM = distanceInWorldSpace * 1000f * scaleFactor;

        // Position measurement text
        Vector3 mid = (startPos + endPos) / 2f;
        canvas.position = mid;
        lengthText.text = $"{distanceInMM:0.00} mm";

        // Make text face the head
        Transform head = FindObjectOfType<XRRigMapper>().headTarget;
        if (head != null)
        {
            canvas.LookAt(head);
        }

        // Add components if needed
        if (GetComponent<Deletable>() == null)
            gameObject.AddComponent<Deletable>();

        if (GetComponent<BoxCollider>() == null)
        {
            var collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        // Parent to selected model if one exists
        if (SelectionManager.Instance.GetSelectedModel() && transform.parent == null)
            transform.parent = SelectionManager.Instance.GetSelectedModel().transform;
    }

    public void SetPointOne(Vector3 pos)
    {
        point1.transform.position = pos;
    }
}*/

/// above for checking...
/// 

/////////////////////////////////////////////////////////////// look like working.........................................................
/*using UnityEngine;
using TMPro;
using Photon.Pun;

public class MeasurmentInk : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform canvas;
    [SerializeField] TMP_Text lengthText;

    [Header("Points")]
    [SerializeField] GameObject point1;
    [SerializeField] GameObject point2;

    private float originalMeasurement; // Store the original measurement value

    public void SetLineAndMesurments(Vector3 startPos, Vector3 endPos)
    {
        photonView.RPC(nameof(SetLineAndMesurmentsRPC), RpcTarget.All, startPos, endPos);
    }

    [PunRPC]
    public void SetLineAndMesurmentsRPC(Vector3 startPos, Vector3 endPos)
    {
        // Set line properties
        lineRenderer.startWidth = lineRenderer.endWidth = .001f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Set point positions
        point1.transform.position = startPos;
        point2.transform.position = endPos;

        // Get the parent's scale (assuming uniform scale)
        float parentScale = transform.parent ? transform.parent.lossyScale.x : 1f;

        // Calculate distance in world space and divide by the scale to get original distance
        float distanceInWorldSpace = Vector3.Distance(startPos, endPos) / parentScale;

        // Convert to millimeters with scaling adjustment
        float scaleFactor = 0.3f; // Adjustment factor to match other software
        float distanceInMM = distanceInWorldSpace * 1000f * scaleFactor;

        // Store original measurement
        if (originalMeasurement == 0)
        {
            originalMeasurement = distanceInMM;
        }

        // Position measurement text
        Vector3 mid = (startPos + endPos) / 2f;
        canvas.position = mid;
        lengthText.text = $"{originalMeasurement:0.00} mm";

        // Make text face the head
        Transform head = FindObjectOfType<XRRigMapper>().headTarget;
        if (head != null)
        {
            canvas.LookAt(head);
            // Rotate text 180 degrees around Y-axis if it's facing away from the camera
            Vector3 directionToHead = head.position - canvas.position;
            float angle = Vector3.Angle(canvas.forward, directionToHead);
            if (angle > 90)
            {
                canvas.Rotate(0, 180, 0);
            }
        }
        
        gameObject.AddComponent<Deletable>();
        gameObject.AddComponent<BoxCollider>().isTrigger = true;
        if (SelectionManager.Instance.GetSelectedModel())
            transform.parent = SelectionManager.Instance.GetSelectedModel().transform;

    }

    public void SetPointOne(Vector3 pos)
    {
        point1.transform.position = pos;
    }

    public void SetPointTwo(Vector3 pos)
    {
        point2.transform.position = pos;
    }

    public float GetOriginalMeasurement()
    {
        return originalMeasurement;
    }

    // Optional: Add this method if you need to manually set the original measurement
    public void SetOriginalMeasurement(float measurement)
    {
        originalMeasurement = measurement;
        if (lengthText != null)
        {
            lengthText.text = $"{originalMeasurement:0.00} mm";
        }
    }
}
*/

/////////////////////////////////////////////////////above is working




////////////////////////////////////below is for curved and straightline




/*using UnityEngine;
using TMPro;
using Photon.Pun;

public class MeasurmentInk : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform canvas;
    [SerializeField] TMP_Text lengthText;

    [Header("Points")]
    [SerializeField] GameObject point1;
    [SerializeField] GameObject point2;

    private float originalMeasurement; // Store the original measurement value

    public void SetLineAndMesurments(Vector3[] points, bool isStraight)
    {
        photonView.RPC(nameof(SetLineAndMesurmentsRPC), RpcTarget.All, points, isStraight);
    }

    [PunRPC]
    public void SetLineAndMesurmentsRPC(Vector3[] points, bool isStraight)
    {
        if (points == null || points.Length < 2) return;

        // Set line properties
        lineRenderer.startWidth = lineRenderer.endWidth = 0.001f;
        if (isStraight && points.Length >= 2)
        {
            // For straight line, use only first and last points
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, points[0]);
            lineRenderer.SetPosition(1, points[points.Length - 1]);
        }
        else
        {
            // For curved, use all points
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }

        // Set point positions (for first and last points, if needed)
        if (point1 != null) point1.transform.position = points[0];
        if (point2 != null) point2.transform.position = points[points.Length - 1];

        // Get the parent's scale (assuming uniform scale)
        float parentScale = transform.parent ? transform.parent.lossyScale.x : 1f;

        // Calculate total distance
        float totalDistance = 0f;
        if (isStraight && points.Length >= 2)
        {
            // Straight-line mode: direct distance between first and last points
            totalDistance = Vector3.Distance(points[0], points[points.Length - 1]);
        }
        else
        {
            // Curved mode: sum of distances between consecutive points
            for (int i = 0; i < points.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(points[i], points[i + 1]);
            }
        }

        // Convert to millimeters with scaling adjustment
        float scaleFactor = 0.3f; // Adjustment factor to match other software
        float distanceInMM = (totalDistance / parentScale) * 1000f * scaleFactor;

        // Store original measurement
        if (originalMeasurement == 0)
        {
            originalMeasurement = distanceInMM;
        }

        // Position measurement text
        Vector3 mid;
        if (isStraight && points.Length >= 2)
        {
            // For straight line, use midpoint of first and last points
            mid = (points[0] + points[points.Length - 1]) / 2f;
        }
        else
        {
            // For curved line, use average of all points
            mid = Vector3.zero;
            for (int i = 0; i < points.Length; i++)
            {
                mid += points[i];
            }
            mid /= points.Length;
        }
        // Offset text above the line
        canvas.position = mid + Vector3.up * 0.02f; // Small upward offset (adjust as needed)
        lengthText.text = $"{originalMeasurement:0.00} mm";

        // Make text face the head
        Transform head = FindObjectOfType<XRRigMapper>().headTarget;
        if (head != null)
        {
            canvas.LookAt(head);
            Vector3 directionToHead = head.position - canvas.position;
            float angle = Vector3.Angle(canvas.forward, directionToHead);
            if (angle > 90)
            {
                canvas.Rotate(0, 180, 0);
            }
        }


        gameObject.AddComponent<Deletable>();
        gameObject.AddComponent<BoxCollider>().isTrigger = true;
        if (SelectionManager.Instance.GetSelectedModel())
            transform.parent = SelectionManager.Instance.GetSelectedModel().transform;

    }

    public void SetPointOne(Vector3 pos)
    {
        if (point1 != null) point1.transform.position = pos;
    }

    public void SetPointTwo(Vector3 pos)
    {
        if (point2 != null) point2.transform.position = pos;
    }

    public float GetOriginalMeasurement()
    {
        return originalMeasurement;
    }

    public void SetOriginalMeasurement(float measurement)
    {
        originalMeasurement = measurement;
        if (lengthText != null)
        {
            lengthText.text = $"{originalMeasurement:0.00} mm";
        }
    }
}*/

/*using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;

public class MeasurmentInk : MonoBehaviour, IPunObservable
{
    [SerializeField] PhotonView photonView;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform canvas;
    [SerializeField] TMP_Text lengthText;

    [Header("Points")]
    [SerializeField] GameObject point1;
    [SerializeField] GameObject point2;

    private float originalMeasurement;
    private Vector3[] storedPoints;
    private bool isStraightLine;
    private bool isInitialized = false;
    private Vector3 canvasLocalPosition;
    private Vector3 point1LocalPosition;
    private Vector3 point2LocalPosition;

    // Model attachment data
    private bool isAttachedToModel = false;
    private string attachedModelID = "";
    private Transform attachedModel;

    // Deletion tracking - Use PhotonView ID as consistent identifier
    private bool isDeleted = false;
    private static HashSet<int> deletedMeasurements = new HashSet<int>();
    private int measurementID;

    private void Start()
    {
        // Use PhotonView ID as the consistent measurement ID
        measurementID = photonView.ViewID;

        // Check if this measurement was already deleted
        if (deletedMeasurements.Contains(measurementID))
        {
            Destroy(gameObject);
            return;
        }

        // For late joiners, request the current state from master client
        if (!PhotonNetwork.IsMasterClient && !isInitialized)
        {
            photonView.RPC(nameof(RequestMeasurementData), RpcTarget.MasterClient);
        }
    }

    private void Update()
    {
        // Don't update if deleted
        if (isDeleted) return;

        // Update line renderer positions every frame if attached to a model
        if (isAttachedToModel && attachedModel != null && isInitialized && storedPoints != null)
        {
            UpdateLineRendererPositions();
        }
    }

    private void UpdateLineRendererPositions()
    {
        if (storedPoints == null || storedPoints.Length < 2) return;

        // Convert local points to world space
        Vector3[] worldPoints = new Vector3[storedPoints.Length];
        for (int i = 0; i < storedPoints.Length; i++)
        {
            worldPoints[i] = attachedModel.TransformPoint(storedPoints[i]);
        }

        // Update line renderer positions
        if (isStraightLine && worldPoints.Length >= 2)
        {
            lineRenderer.SetPosition(0, worldPoints[0]);
            lineRenderer.SetPosition(1, worldPoints[worldPoints.Length - 1]);
        }
        else
        {
            lineRenderer.SetPositions(worldPoints);
        }
    }

    [PunRPC]
    void RequestMeasurementData()
    {
        // Don't send data if deleted
        if (isDeleted || deletedMeasurements.Contains(measurementID))
            return;

        // Master client sends current state to the requester
        if (isInitialized && storedPoints != null)
        {
            photonView.RPC(nameof(ReceiveMeasurementData), RpcTarget.Others,
            storedPoints, isStraightLine, originalMeasurement,
            canvasLocalPosition, point1LocalPosition, point2LocalPosition,
            isAttachedToModel, attachedModelID, measurementID);
        }
    }

    [PunRPC]
    void ReceiveMeasurementData(Vector3[] points, bool isStraight, float measurement,
    Vector3 canvasLocalPos, Vector3 p1LocalPos, Vector3 p2LocalPos,
    bool attachedToModel, string modelID, int measID)
    {
        // Check if this measurement was deleted
        if (deletedMeasurements.Contains(measID))
        {
            Destroy(gameObject);
            return;
        }

        if (isInitialized) return; // Already initialized

        measurementID = measID;
        storedPoints = points;
        isStraightLine = isStraight;
        originalMeasurement = measurement;
        canvasLocalPosition = canvasLocalPos;
        point1LocalPosition = p1LocalPos;
        point2LocalPosition = p2LocalPos;
        isAttachedToModel = attachedToModel;
        attachedModelID = modelID;
        isInitialized = true;

        // Find and attach to model if needed
        if (isAttachedToModel && !string.IsNullOrEmpty(attachedModelID))
        {
            FindAndAttachToModel();
        }

        ApplyCompleteVisualization();
    }

      private void FindAndAttachToModel()
        {
            // Try to find the model by ID
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                // Check if this object has the model ID we're looking for
                if (obj.name.Contains(attachedModelID) ||
                (obj.GetComponent<PhotonView>() && obj.GetComponent<PhotonView>().ViewID.ToString() == attachedModelID))
                {
                    attachedModel = obj.transform;
                    transform.parent = attachedModel;
                    break;
                }
            }

            // If model not found, try SelectionManager as fallback
            if (attachedModel == null)
            {
                ModelObject selectedModelObject = SelectionManager.Instance.GetSelectedModel();
                if (selectedModelObject != null)
                {
                    attachedModel = selectedModelObject.transform;
                    transform.parent = attachedModel;
                }
            }
        }


    public void SetLineAndMesurments(Vector3[] points, bool isStraight)
    {
        if (isDeleted) return;
        photonView.RPC(nameof(SetLineAndMesurmentsRPC), RpcTarget.All, points, isStraight);
    }

    [PunRPC]
    public void SetLineAndMesurmentsRPC(Vector3[] points, bool isStraight)
    {
        if (points == null || points.Length < 2 || isDeleted) return;

        // Store all data for network synchronization
        storedPoints = points;
        isStraightLine = isStraight;
        isInitialized = true;

        // Check for model attachment
        CheckAndSetModelAttachment();

        ApplyMeasurement(points, isStraight);
    }

    private void CheckAndSetModelAttachment()
    {
        // Check if we should attach to a model
        ModelObject selectedModelObject = SelectionManager.Instance.GetSelectedModel();
        if (selectedModelObject != null)
        {
            isAttachedToModel = true;
            attachedModel = selectedModelObject.transform;
            attachedModelID = GetModelIdentifier(attachedModel);

            // Convert world positions to local positions relative to the model
            ConvertToLocalPositions();

            // Attach to the model
            transform.parent = attachedModel;
        }
        else
        {
            isAttachedToModel = false;
            attachedModelID = "";
            attachedModel = null;
        }
    }

    private string GetModelIdentifier(Transform model)
    {
        // Try to get PhotonView ID first
        PhotonView pv = model.GetComponent<PhotonView>();
        if (pv != null)
        {
            return pv.ViewID.ToString();
        }

        // Fallback to name
        return model.name;
    }

    private void ConvertToLocalPositions()
    {
        if (attachedModel == null) return;

        // Convert stored points to local space for storage and network sync
        if (storedPoints != null)
        {
            Vector3[] localPoints = new Vector3[storedPoints.Length];
            for (int i = 0; i < storedPoints.Length; i++)
            {
                localPoints[i] = attachedModel.InverseTransformPoint(storedPoints[i]);
            }
            storedPoints = localPoints;
        }

        // Convert other positions to local space for storage
        if (canvas != null)
        {
            canvasLocalPosition = attachedModel.InverseTransformPoint(canvas.position);
        }

        if (point1 != null)
        {
            point1LocalPosition = attachedModel.InverseTransformPoint(point1.transform.position);
        }

        if (point2 != null)
        {
            point2LocalPosition = attachedModel.InverseTransformPoint(point2.transform.position);
        }
    }

    private void ConvertFromLocalPositions()
    {
        if (attachedModel == null || storedPoints == null) return;

        // Convert stored points from local space to world space for rendering
        Vector3[] worldPoints = new Vector3[storedPoints.Length];
        for (int i = 0; i < storedPoints.Length; i++)
        {
            worldPoints[i] = attachedModel.TransformPoint(storedPoints[i]);
        }

        // Configure line renderer with world positions
        ConfigureLineRenderer(worldPoints, isStraightLine);

        // Set point positions in world space
        if (point1 != null && point1LocalPosition != Vector3.zero)
        {
            point1.transform.position = attachedModel.TransformPoint(point1LocalPosition);
        }

        if (point2 != null && point2LocalPosition != Vector3.zero)
        {
            point2.transform.position = attachedModel.TransformPoint(point2LocalPosition);
        }

        // Set canvas position in world space
        if (canvas != null && canvasLocalPosition != Vector3.zero)
        {
            canvas.position = attachedModel.TransformPoint(canvasLocalPosition);
        }
    }

    private void ApplyMeasurement(Vector3[] points, bool isStraight)
    {
        // Store the original world points for calculation
        Vector3[] worldPoints = new Vector3[points.Length];
        System.Array.Copy(points, worldPoints, points.Length);

        // Configure line renderer with world points
        ConfigureLineRenderer(worldPoints, isStraight);

        // Set point positions with world points
        SetPointPositions(worldPoints);

        // Calculate measurement only if not already set
        if (originalMeasurement == 0)
        {
            CalculateAndSetMeasurement(worldPoints, isStraight);
        }

        // Position and configure canvas with world points
        PositionCanvas(worldPoints, isStraight);

        // Store positions for network sync AFTER everything is positioned
        StorePositionsForSync();

        // Add interaction components
        AddInteractionComponents();
    }

    private void ApplyCompleteVisualization()
    {
        if (storedPoints == null || storedPoints.Length < 2) return;

        if (isAttachedToModel && attachedModel != null)
        {
            // Use local positions converted to world space
            ConvertFromLocalPositions();
        }
        else
        {
            // Use world positions directly
            ConfigureLineRenderer(storedPoints, isStraightLine);

            if (point1 != null && point1LocalPosition != Vector3.zero)
            {
                point1.transform.position = point1LocalPosition;
            }

            if (point2 != null && point2LocalPosition != Vector3.zero)
            {
                point2.transform.position = point2LocalPosition;
            }

            if (canvas != null && canvasLocalPosition != Vector3.zero)
            {
                canvas.position = canvasLocalPosition;
            }
        }

        // Set measurement text
        lengthText.text = $"{originalMeasurement:0.00} mm";

        // Orient canvas to face local player's head
        OrientCanvasToHead();

        // Add interaction components
        AddInteractionComponents();
    }

    private void ConfigureLineRenderer(Vector3[] points, bool isStraight)
    {
        lineRenderer.startWidth = lineRenderer.endWidth = 0.001f;

        // Always use world space for LineRenderer when dealing with dynamic positions
        lineRenderer.useWorldSpace = true;

        if (isStraight && points.Length >= 2)
        {
            // For straight line, use only first and last points
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, points[0]);
            lineRenderer.SetPosition(1, points[points.Length - 1]);
        }
        else
        {
            // For curved, use all points
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }

    private void SetPointPositions(Vector3[] points)
    {
        if (point1 != null) point1.transform.position = points[0];
        if (point2 != null) point2.transform.position = points[points.Length - 1];
    }

    private void CalculateAndSetMeasurement(Vector3[] points, bool isStraight)
    {
        // Get the parent's scale (assuming uniform scale)
        float parentScale = transform.parent ? transform.parent.lossyScale.x : 1f;

        // Calculate total distance
        float totalDistance = 0f;
        if (isStraight && points.Length >= 2)
        {
            totalDistance = Vector3.Distance(points[0], points[points.Length - 1]);
        }
        else
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(points[i], points[i + 1]);
            }
        }

        // Convert to millimeters with scaling adjustment
        float scaleFactor = 0.3f;
        originalMeasurement = (totalDistance / parentScale) * 1000f * scaleFactor;

        lengthText.text = $"{originalMeasurement:0.00} mm";
    }

    private void PositionCanvas(Vector3[] points, bool isStraight)
    {
        // Calculate midpoint for canvas positioning
        Vector3 mid;
        if (isStraight && points.Length >= 2)
        {
            mid = (points[0] + points[points.Length - 1]) / 2f;
        }
        else
        {
            mid = Vector3.zero;
            for (int i = 0; i < points.Length; i++)
            {
                mid += points[i];
            }
            mid /= points.Length;
        }

        canvas.position = mid + Vector3.up * 0.02f;
        OrientCanvasToHead();
    }

    private void OrientCanvasToHead()
    {
        Transform head = FindObjectOfType<XRRigMapper>()?.headTarget;
        if (head != null)
        {
            canvas.LookAt(head);
            Vector3 directionToHead = head.position - canvas.position;
            float angle = Vector3.Angle(canvas.forward, directionToHead);
            if (angle > 90)
            {
                canvas.Rotate(0, 180, 0);
            }
        }
    }

    private void StorePositionsForSync()
    {
        if (isAttachedToModel && attachedModel != null)
        {
            // Store as local positions relative to the model
            canvasLocalPosition = attachedModel.InverseTransformPoint(canvas.position);
            point1LocalPosition = point1 != null ? attachedModel.InverseTransformPoint(point1.transform.position) : Vector3.zero;
            point2LocalPosition = point2 != null ? attachedModel.InverseTransformPoint(point2.transform.position) : Vector3.zero;
        }
        else
        {
            // Store as world positions
            canvasLocalPosition = canvas.position;
            point1LocalPosition = point1 != null ? point1.transform.position : Vector3.zero;
            point2LocalPosition = point2 != null ? point2.transform.position : Vector3.zero;
        }
    }

    private void AddInteractionComponents()
    {
        Deletable deletableComponent = gameObject.GetComponent<Deletable>();
        if (!deletableComponent)
        {
            deletableComponent = gameObject.AddComponent<Deletable>();
        }

        if (!gameObject.GetComponent<BoxCollider>())
        {
            gameObject.AddComponent<BoxCollider>().isTrigger = true;
        }
    }

    // Permanent deletion methods
    [PunRPC]
    public void DeleteMeasurementPermanently()
    {
        isDeleted = true;
        deletedMeasurements.Add(measurementID);

        // Also destroy the PhotonView to prevent recreation
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DeleteMeasurement()
    {
        // Call this method from your Deletable component or delete button
        photonView.RPC(nameof(DeleteMeasurementPermanently), RpcTarget.All);
    }

    public void SetPointOne(Vector3 pos)
    {
        if (point1 != null && !isDeleted) point1.transform.position = pos;
    }

    public void SetPointTwo(Vector3 pos)
    {
        if (point2 != null && !isDeleted) point2.transform.position = pos;
    }

    public float GetOriginalMeasurement()
    {
        return originalMeasurement;
    }

    public void SetOriginalMeasurement(float measurement)
    {
        if (isDeleted) return;

        originalMeasurement = measurement;
        if (lengthText != null)
        {
            lengthText.text = $"{originalMeasurement:0.00} mm";
        }
    }

    // IPunObservable implementation for continuous synchronization
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send deletion state first
            stream.SendNext(isDeleted);

            if (isDeleted)
            {
                stream.SendNext(measurementID);
                return; // Don't send any other data if deleted
            }

            // Send current state only if not deleted
            stream.SendNext(isInitialized);
            stream.SendNext(originalMeasurement);
            stream.SendNext(isAttachedToModel);
            stream.SendNext(attachedModelID);
            stream.SendNext(measurementID);

            if (isInitialized && storedPoints != null)
            {
                stream.SendNext(storedPoints.Length);
                for (int i = 0; i < storedPoints.Length; i++)
                {
                    stream.SendNext(storedPoints[i]);
                }
                stream.SendNext(isStraightLine);
                stream.SendNext(canvasLocalPosition);
                stream.SendNext(point1LocalPosition);
                stream.SendNext(point2LocalPosition);
            }
            else
            {
                stream.SendNext(0); // No data to send
            }
        }
        else
        {
            // Receive deletion state first
            bool receivedDeleted = (bool)stream.ReceiveNext();

            if (receivedDeleted)
            {
                int deletedID = (int)stream.ReceiveNext();
                deletedMeasurements.Add(deletedID);
                if (!isDeleted)
                {
                    isDeleted = true;
                    Destroy(gameObject);
                }
                return;
            }

            // Receive state from other players only if not deleted
            bool receivedInitialized = (bool)stream.ReceiveNext();
            float receivedMeasurement = (float)stream.ReceiveNext();
            bool receivedAttachedToModel = (bool)stream.ReceiveNext();
            string receivedModelID = (string)stream.ReceiveNext();
            int receivedMeasurementID = (int)stream.ReceiveNext();

            // Check if this measurement was deleted
            if (deletedMeasurements.Contains(receivedMeasurementID))
            {
                Destroy(gameObject);
                return;
            }

            // Set the measurement ID if not set
            if (measurementID == 0)
            {
                measurementID = receivedMeasurementID;
            }

            int pointCount = (int)stream.ReceiveNext();

            if (pointCount > 0 && !isInitialized && receivedInitialized)
            {
                Vector3[] receivedPoints = new Vector3[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    receivedPoints[i] = (Vector3)stream.ReceiveNext();
                }
                bool receivedStraightMode = (bool)stream.ReceiveNext();
                Vector3 receivedCanvasPos = (Vector3)stream.ReceiveNext();
                Vector3 receivedP1Pos = (Vector3)stream.ReceiveNext();
                Vector3 receivedP2Pos = (Vector3)stream.ReceiveNext();

                // Apply received complete state
                storedPoints = receivedPoints;
                isStraightLine = receivedStraightMode;
                originalMeasurement = receivedMeasurement;
                canvasLocalPosition = receivedCanvasPos;
                point1LocalPosition = receivedP1Pos;
                point2LocalPosition = receivedP2Pos;
                isAttachedToModel = receivedAttachedToModel;
                attachedModelID = receivedModelID;
                isInitialized = true;

                // Find and attach to model if needed
                if (isAttachedToModel && !string.IsNullOrEmpty(attachedModelID))
                {
                    FindAndAttachToModel();
                }

                ApplyCompleteVisualization();
            }
        }
    }
}*/
using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;

public class MeasurmentInk : MonoBehaviour, IPunObservable
{
    [SerializeField] PhotonView photonView;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform canvas;
    [SerializeField] TMP_Text lengthText;

    [Header("Points")]
    [SerializeField] GameObject point1;
    [SerializeField] GameObject point2;

    private float originalMeasurement;
    private Vector3[] storedPoints;
    private bool isStraightLine;
    private bool isInitialized = false;
    private Vector3 canvasLocalPosition;
    private Vector3 point1LocalPosition;
    private Vector3 point2LocalPosition;

    // Model attachment data
    private bool isAttachedToModel = false;
    private string attachedModelID = "";
    private Transform attachedModel;

    // Deletion tracking - Use PhotonView ID as consistent identifier
    private bool isDeleted = false;
    private static HashSet<int> deletedMeasurements = new HashSet<int>();
    private static HashSet<string> deletedModels = new HashSet<string>(); // Track deleted models
    private int measurementID;

    // Add this method to be called when a model is deleted
    public static void OnModelDeleted(string modelID)
    {
        deletedModels.Add(modelID);

        // Find and delete all measurements attached to this model
        MeasurmentInk[] allMeasurements = FindObjectsOfType<MeasurmentInk>();
        foreach (MeasurmentInk measurement in allMeasurements)
        {
            if (measurement.isAttachedToModel && measurement.attachedModelID == modelID)
            {
                measurement.DeleteMeasurement();
            }
        }
    }

    // Add this method to check if a model was deleted (call this from your model script when models are loaded)
    public static bool IsModelDeleted(string modelID)
    {
        return deletedModels.Contains(modelID);
    }

    private void Start()
    {
        // Use PhotonView ID as the consistent measurement ID
        measurementID = photonView.ViewID;

        // Check if this measurement was already deleted
        if (deletedMeasurements.Contains(measurementID))
        {
            Destroy(gameObject);
            return;
        }

        // Check if the attached model was deleted
        if (isAttachedToModel && !string.IsNullOrEmpty(attachedModelID) && deletedModels.Contains(attachedModelID))
        {
            DeleteMeasurement();
            return;
        }

        // For late joiners, request the current state from master client
        if (!PhotonNetwork.IsMasterClient && !isInitialized)
        {
            photonView.RPC(nameof(RequestMeasurementData), RpcTarget.MasterClient);
        }
    }

    private void Update()
    {
        // Don't update if deleted
        if (isDeleted) return;

        // Check if attached model still exists
        if (isAttachedToModel && !string.IsNullOrEmpty(attachedModelID))
        {
            // If model was deleted, delete this measurement
            if (deletedModels.Contains(attachedModelID))
            {
                DeleteMeasurement();
                return;
            }

            // If model reference is lost, try to find it again
            if (attachedModel == null)
            {
                FindAndAttachToModel();
                // If still can't find it, it might have been deleted
                if (attachedModel == null)
                {
                    DeleteMeasurement();
                    return;
                }
            }
        }

        // Update line renderer positions every frame if attached to a model
        if (isAttachedToModel && attachedModel != null && isInitialized && storedPoints != null)
        {
            UpdateLineRendererPositions();
        }
    }

    private void UpdateLineRendererPositions()
    {
        if (storedPoints == null || storedPoints.Length < 2) return;

        // Convert local points to world space
        Vector3[] worldPoints = new Vector3[storedPoints.Length];
        for (int i = 0; i < storedPoints.Length; i++)
        {
            worldPoints[i] = attachedModel.TransformPoint(storedPoints[i]);
        }

        // Update line renderer positions
        if (isStraightLine && worldPoints.Length >= 2)
        {
            lineRenderer.SetPosition(0, worldPoints[0]);
            lineRenderer.SetPosition(1, worldPoints[worldPoints.Length - 1]);
        }
        else
        {
            lineRenderer.SetPositions(worldPoints);
        }
    }

    [PunRPC]
    void RequestMeasurementData()
    {
        // Don't send data if deleted or if attached model was deleted
        if (isDeleted || deletedMeasurements.Contains(measurementID))
            return;

        if (isAttachedToModel && !string.IsNullOrEmpty(attachedModelID) && deletedModels.Contains(attachedModelID))
        {
            // If attached model was deleted, delete this measurement
            photonView.RPC(nameof(DeleteMeasurementPermanently), RpcTarget.All);
            return;
        }

        // Master client sends current state to the requester
        if (isInitialized && storedPoints != null)
        {
            photonView.RPC(nameof(ReceiveMeasurementData), RpcTarget.Others,
            storedPoints, isStraightLine, originalMeasurement,
            canvasLocalPosition, point1LocalPosition, point2LocalPosition,
            isAttachedToModel, attachedModelID, measurementID);
        }
    }

    [PunRPC]
    void ReceiveMeasurementData(Vector3[] points, bool isStraight, float measurement,
    Vector3 canvasLocalPos, Vector3 p1LocalPos, Vector3 p2LocalPos,
    bool attachedToModel, string modelID, int measID)
    {
        // Check if this measurement was deleted
        if (deletedMeasurements.Contains(measID))
        {
            Destroy(gameObject);
            return;
        }

        // Check if attached model was deleted
        if (attachedToModel && !string.IsNullOrEmpty(modelID) && deletedModels.Contains(modelID))
        {
            Destroy(gameObject);
            return;
        }

        if (isInitialized) return; // Already initialized

        measurementID = measID;
        storedPoints = points;
        isStraightLine = isStraight;
        originalMeasurement = measurement;
        canvasLocalPosition = canvasLocalPos;
        point1LocalPosition = p1LocalPos;
        point2LocalPosition = p2LocalPos;
        isAttachedToModel = attachedToModel;
        attachedModelID = modelID;
        isInitialized = true;

        // Find and attach to model if needed
        if (isAttachedToModel && !string.IsNullOrEmpty(attachedModelID))
        {
            FindAndAttachToModel();
            // If model not found, it might have been deleted
            if (attachedModel == null)
            {
                Destroy(gameObject);
                return;
            }
        }

        ApplyCompleteVisualization();
    }

    private void FindAndAttachToModel()
    {
        // Check if model was deleted first
        if (deletedModels.Contains(attachedModelID))
        {
            return; // Don't try to find deleted models
        }

        // Try to find the model by ID
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // Check if this object has the model ID we're looking for
            if (obj.name.Contains(attachedModelID) ||
            (obj.GetComponent<PhotonView>() && obj.GetComponent<PhotonView>().ViewID.ToString() == attachedModelID))
            {
                attachedModel = obj.transform;
                transform.parent = attachedModel;
                return;
            }
        }

        // If model not found, try SelectionManager as fallback
        ModelObject selectedModelObject = SelectionManager.Instance.GetSelectedModel();
        if (selectedModelObject != null && GetModelIdentifier(selectedModelObject.transform) == attachedModelID)
        {
            attachedModel = selectedModelObject.transform;
            transform.parent = attachedModel;
        }
    }

    public void SetLineAndMesurments(Vector3[] points, bool isStraight)
    {
        if (isDeleted) return;
        photonView.RPC(nameof(SetLineAndMesurmentsRPC), RpcTarget.All, points, isStraight);
    }

    [PunRPC]
    public void SetLineAndMesurmentsRPC(Vector3[] points, bool isStraight)
    {
        if (points == null || points.Length < 2 || isDeleted) return;

        // Store all data for network synchronization
        storedPoints = points;
        isStraightLine = isStraight;
        isInitialized = true;

        // Check for model attachment
        CheckAndSetModelAttachment();

        ApplyMeasurement(points, isStraight);
    }

    private void CheckAndSetModelAttachment()
    {
        // Check if we should attach to a model
        ModelObject selectedModelObject = SelectionManager.Instance.GetSelectedModel();
        if (selectedModelObject != null)
        {
            string modelID = GetModelIdentifier(selectedModelObject.transform);

            // Don't attach to deleted models
            if (deletedModels.Contains(modelID))
            {
                isAttachedToModel = false;
                attachedModelID = "";
                attachedModel = null;
                return;
            }

            isAttachedToModel = true;
            attachedModel = selectedModelObject.transform;
            attachedModelID = modelID;

            // Convert world positions to local positions relative to the model
            ConvertToLocalPositions();

            // Attach to the model
            transform.parent = attachedModel;
        }
        else
        {
            isAttachedToModel = false;
            attachedModelID = "";
            attachedModel = null;
        }
    }

    private string GetModelIdentifier(Transform model)
    {
        // Try to get PhotonView ID first
        PhotonView pv = model.GetComponent<PhotonView>();
        if (pv != null)
        {
            return pv.ViewID.ToString();
        }

        // Fallback to name
        return model.name;
    }

    // ... rest of your existing methods remain the same ...

    private void ConvertToLocalPositions()
    {
        if (attachedModel == null) return;

        // Convert stored points to local space for storage and network sync
        if (storedPoints != null)
        {
            Vector3[] localPoints = new Vector3[storedPoints.Length];
            for (int i = 0; i < storedPoints.Length; i++)
            {
                localPoints[i] = attachedModel.InverseTransformPoint(storedPoints[i]);
            }
            storedPoints = localPoints;
        }

        // Convert other positions to local space for storage
        if (canvas != null)
        {
            canvasLocalPosition = attachedModel.InverseTransformPoint(canvas.position);
        }

        if (point1 != null)
        {
            point1LocalPosition = attachedModel.InverseTransformPoint(point1.transform.position);
        }

        if (point2 != null)
        {
            point2LocalPosition = attachedModel.InverseTransformPoint(point2.transform.position);
        }
    }

    private void ConvertFromLocalPositions()
    {
        if (attachedModel == null || storedPoints == null) return;

        // Convert stored points from local space to world space for rendering
        Vector3[] worldPoints = new Vector3[storedPoints.Length];
        for (int i = 0; i < storedPoints.Length; i++)
        {
            worldPoints[i] = attachedModel.TransformPoint(storedPoints[i]);
        }

        // Configure line renderer with world positions
        ConfigureLineRenderer(worldPoints, isStraightLine);

        // Set point positions in world space
        if (point1 != null && point1LocalPosition != Vector3.zero)
        {
            point1.transform.position = attachedModel.TransformPoint(point1LocalPosition);
        }

        if (point2 != null && point2LocalPosition != Vector3.zero)
        {
            point2.transform.position = attachedModel.TransformPoint(point2LocalPosition);
        }

        // Set canvas position in world space
        if (canvas != null && canvasLocalPosition != Vector3.zero)
        {
            canvas.position = attachedModel.TransformPoint(canvasLocalPosition);
        }
    }

    private void ApplyMeasurement(Vector3[] points, bool isStraight)
    {
        // Store the original world points for calculation
        Vector3[] worldPoints = new Vector3[points.Length];
        System.Array.Copy(points, worldPoints, points.Length);

        // Configure line renderer with world points
        ConfigureLineRenderer(worldPoints, isStraight);

        // Set point positions with world points
        SetPointPositions(worldPoints);

        // Calculate measurement only if not already set
        if (originalMeasurement == 0)
        {
            CalculateAndSetMeasurement(worldPoints, isStraight);
        }

        // Position and configure canvas with world points
        PositionCanvas(worldPoints, isStraight);

        // Store positions for network sync AFTER everything is positioned
        StorePositionsForSync();

        // Add interaction components
        AddInteractionComponents();
    }

    private void ApplyCompleteVisualization()
    {
        if (storedPoints == null || storedPoints.Length < 2) return;

        if (isAttachedToModel && attachedModel != null)
        {
            // Use local positions converted to world space
            ConvertFromLocalPositions();
        }
        else
        {
            // Use world positions directly
            ConfigureLineRenderer(storedPoints, isStraightLine);

            if (point1 != null && point1LocalPosition != Vector3.zero)
            {
                point1.transform.position = point1LocalPosition;
            }

            if (point2 != null && point2LocalPosition != Vector3.zero)
            {
                point2.transform.position = point2LocalPosition;
            }

            if (canvas != null && canvasLocalPosition != Vector3.zero)
            {
                canvas.position = canvasLocalPosition;
            }
        }

        // Set measurement text
        lengthText.text = $"{originalMeasurement:0.00} mm";

        // Orient canvas to face local player's head
        OrientCanvasToHead();

        // Add interaction components
        AddInteractionComponents();
    }

    private void ConfigureLineRenderer(Vector3[] points, bool isStraight)
    {
        lineRenderer.startWidth = lineRenderer.endWidth = 0.001f;

        // Always use world space for LineRenderer when dealing with dynamic positions
        lineRenderer.useWorldSpace = true;

        if (isStraight && points.Length >= 2)
        {
            // For straight line, use only first and last points
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, points[0]);
            lineRenderer.SetPosition(1, points[points.Length - 1]);
        }
        else
        {
            // For curved, use all points
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }

    private void SetPointPositions(Vector3[] points)
    {
        if (point1 != null) point1.transform.position = points[0];
        if (point2 != null) point2.transform.position = points[points.Length - 1];
    }

    private void CalculateAndSetMeasurement(Vector3[] points, bool isStraight)
    {
        // Get the parent's scale (assuming uniform scale)
        float parentScale = transform.parent ? transform.parent.lossyScale.x : 1f;

        // Calculate total distance
        float totalDistance = 0f;
        if (isStraight && points.Length >= 2)
        {
            totalDistance = Vector3.Distance(points[0], points[points.Length - 1]);
        }
        else
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(points[i], points[i + 1]);
            }
        }

        // Convert to millimeters with scaling adjustment
        float scaleFactor = 0.3f;
        originalMeasurement = (totalDistance / parentScale) * 1000f * scaleFactor;

        lengthText.text = $"{originalMeasurement:0.00} mm";
    }

    private void PositionCanvas(Vector3[] points, bool isStraight)
    {
        // Calculate midpoint for canvas positioning
        Vector3 mid;
        if (isStraight && points.Length >= 2)
        {
            mid = (points[0] + points[points.Length - 1]) / 2f;
        }
        else
        {
            mid = Vector3.zero;
            for (int i = 0; i < points.Length; i++)
            {
                mid += points[i];
            }
            mid /= points.Length;
        }

        canvas.position = mid + Vector3.up * 0.02f;
        OrientCanvasToHead();
    }

    private void OrientCanvasToHead()
    {
        Transform head = FindObjectOfType<XRRigMapper>()?.headTarget;
        if (head != null)
        {
            canvas.LookAt(head);
            Vector3 directionToHead = head.position - canvas.position;
            float angle = Vector3.Angle(canvas.forward, directionToHead);
            if (angle > 90)
            {
                canvas.Rotate(0, 180, 0);
            }
        }
    }

    private void StorePositionsForSync()
    {
        if (isAttachedToModel && attachedModel != null)
        {
            // Store as local positions relative to the model
            canvasLocalPosition = attachedModel.InverseTransformPoint(canvas.position);
            point1LocalPosition = point1 != null ? attachedModel.InverseTransformPoint(point1.transform.position) : Vector3.zero;
            point2LocalPosition = point2 != null ? attachedModel.InverseTransformPoint(point2.transform.position) : Vector3.zero;
        }
        else
        {
            // Store as world positions
            canvasLocalPosition = canvas.position;
            point1LocalPosition = point1 != null ? point1.transform.position : Vector3.zero;
            point2LocalPosition = point2 != null ? point2.transform.position : Vector3.zero;
        }
    }

    private void AddInteractionComponents()
    {
        Deletable deletableComponent = gameObject.GetComponent<Deletable>();
        if (!deletableComponent)
        {
            deletableComponent = gameObject.AddComponent<Deletable>();
        }

        if (!gameObject.GetComponent<BoxCollider>())
        {
            gameObject.AddComponent<BoxCollider>().isTrigger = true;
        }
    }

    // Permanent deletion methods
    [PunRPC]
    public void DeleteMeasurementPermanently()
    {
        isDeleted = true;
        deletedMeasurements.Add(measurementID);

        // Also destroy the PhotonView to prevent recreation
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DeleteMeasurement()
    {
        // Call this method from your Deletable component or delete button
        photonView.RPC(nameof(DeleteMeasurementPermanently), RpcTarget.All);
    }

    public void SetPointOne(Vector3 pos)
    {
        if (point1 != null && !isDeleted) point1.transform.position = pos;
    }

    public void SetPointTwo(Vector3 pos)
    {
        if (point2 != null && !isDeleted) point2.transform.position = pos;
    }

    public float GetOriginalMeasurement()
    {
        return originalMeasurement;
    }

    public void SetOriginalMeasurement(float measurement)
    {
        if (isDeleted) return;

        originalMeasurement = measurement;
        if (lengthText != null)
        {
            lengthText.text = $"{originalMeasurement:0.00} mm";
        }
    }

    // IPunObservable implementation for continuous synchronization
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send deletion state first
            stream.SendNext(isDeleted);

            if (isDeleted)
            {
                stream.SendNext(measurementID);
                return; // Don't send any other data if deleted
            }

            // Check if attached model was deleted
            if (isAttachedToModel && !string.IsNullOrEmpty(attachedModelID) && deletedModels.Contains(attachedModelID))
            {
                stream.SendNext(true); // Mark as deleted
                stream.SendNext(measurementID);
                DeleteMeasurement(); // Delete this measurement
                return;
            }

            // Send current state only if not deleted
            stream.SendNext(isInitialized);
            stream.SendNext(originalMeasurement);
            stream.SendNext(isAttachedToModel);
            stream.SendNext(attachedModelID);
            stream.SendNext(measurementID);

            if (isInitialized && storedPoints != null)
            {
                stream.SendNext(storedPoints.Length);
                for (int i = 0; i < storedPoints.Length; i++)
                {
                    stream.SendNext(storedPoints[i]);
                }
                stream.SendNext(isStraightLine);
                stream.SendNext(canvasLocalPosition);
                stream.SendNext(point1LocalPosition);
                stream.SendNext(point2LocalPosition);
            }
            else
            {
                stream.SendNext(0); // No data to send
            }
        }
        else
        {
            // Receive deletion state first
            bool receivedDeleted = (bool)stream.ReceiveNext();

            if (receivedDeleted)
            {
                int deletedID = (int)stream.ReceiveNext();
                deletedMeasurements.Add(deletedID);
                if (!isDeleted)
                {
                    isDeleted = true;
                    Destroy(gameObject);
                }
                return;
            }

            // Receive state from other players only if not deleted
            bool receivedInitialized = (bool)stream.ReceiveNext();
            float receivedMeasurement = (float)stream.ReceiveNext();
            bool receivedAttachedToModel = (bool)stream.ReceiveNext();
            string receivedModelID = (string)stream.ReceiveNext();
            int receivedMeasurementID = (int)stream.ReceiveNext();

            // Check if this measurement was deleted or attached model was deleted
            if (deletedMeasurements.Contains(receivedMeasurementID) ||
                (receivedAttachedToModel && !string.IsNullOrEmpty(receivedModelID) && deletedModels.Contains(receivedModelID)))
            {
                Destroy(gameObject);
                return;
            }

            // Set the measurement ID if not set
            if (measurementID == 0)
            {
                measurementID = receivedMeasurementID;
            }

            int pointCount = (int)stream.ReceiveNext();

            if (pointCount > 0 && !isInitialized && receivedInitialized)
            {
                Vector3[] receivedPoints = new Vector3[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    receivedPoints[i] = (Vector3)stream.ReceiveNext();
                }
                bool receivedStraightMode = (bool)stream.ReceiveNext();
                Vector3 receivedCanvasPos = (Vector3)stream.ReceiveNext();
                Vector3 receivedP1Pos = (Vector3)stream.ReceiveNext();
                Vector3 receivedP2Pos = (Vector3)stream.ReceiveNext();

                // Apply received complete state
                storedPoints = receivedPoints;
                isStraightLine = receivedStraightMode;
                originalMeasurement = receivedMeasurement;
                canvasLocalPosition = receivedCanvasPos;
                point1LocalPosition = receivedP1Pos;
                point2LocalPosition = receivedP2Pos;
                isAttachedToModel = receivedAttachedToModel;
                attachedModelID = receivedModelID;
                isInitialized = true;

                // Find and attach to model if needed
                if (isAttachedToModel && !string.IsNullOrEmpty(attachedModelID))
                {
                    FindAndAttachToModel();
                    // If model not found, it might have been deleted
                    if (attachedModel == null)
                    {
                        Destroy(gameObject);
                        return;
                    }
                }

                ApplyCompleteVisualization();
            }
        }
    }
}


