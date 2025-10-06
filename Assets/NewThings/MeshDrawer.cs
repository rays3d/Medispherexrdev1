/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshDrawer : MonoBehaviour
{
    public ActionBasedController controller;
    public GameObject handlePrefab;
    public float pointDistance = 0.05f;
    public float extrusionHeight = 0.05f;

    private List<Vector3> points = new List<Vector3>();
    private Mesh mesh;
    private MeshFilter meshFilter;
    private bool isDrawing = false;
    private bool finalized = false;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        if (finalized) return;

        float trigger = controller.selectAction.action.ReadValue<float>();

        if (trigger > 0.1f)
        {
            if (!isDrawing)
            {
                isDrawing = true;
                points.Clear();
            }

            Vector3 localPos = transform.InverseTransformPoint(controller.transform.position);
            if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], localPos) > pointDistance)
            {
                points.Add(localPos);
                UpdateLiveMesh(points);
            }
        }
        else if (isDrawing)
        {
            isDrawing = false;
            FinalizeMesh(points);
        }
    }

    void UpdateLiveMesh(List<Vector3> rawPoints)
    {
        if (rawPoints.Count < 3) return;

        Vector3 center = GetCenter(rawPoints);
        List<Vector3> sorted = SortPointsByAngle(rawPoints, center);

        BuildExtrudedMesh(sorted, center);
    }

    void FinalizeMesh(List<Vector3> rawPoints)
    {
        finalized = true;

        Vector3 center = GetCenter(rawPoints);
        List<Vector3> sorted = SortPointsByAngle(rawPoints, center);

        BuildExtrudedMesh(sorted, center);

        for (int i = 0; i < sorted.Count; i++)
        {
            GameObject handle = Instantiate(handlePrefab, transform);
            handle.transform.localPosition = sorted[i] - center;

            Handle h = handle.GetComponent<Handle>();
            int calculatedIndex = i * 2 + 2;
            if (calculatedIndex < mesh.vertices.Length)
                h.vertexIndex = calculatedIndex;
            else
                Debug.LogWarning("Handle vertex index out of bounds: " + calculatedIndex);

            h.meshRef = this;
        }
    }

    void BuildExtrudedMesh(List<Vector3> sortedPoints, Vector3 center)
    {
        int count = sortedPoints.Count;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 offset = -center;

        // Bottom ring
        for (int i = 0; i < count; i++)
        {
            vertices.Add(sortedPoints[i] + offset);
        }

        // Top ring
        for (int i = 0; i < count; i++)
        {
            vertices.Add(sortedPoints[i] + offset + Vector3.up * extrusionHeight);
        }

        // Sides
        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;

            int bl = i; // bottom left
            int br = next;
            int tl = i + count; // top left
            int tr = next + count;

            triangles.Add(bl);
            triangles.Add(tl);
            triangles.Add(tr);

            triangles.Add(bl);
            triangles.Add(tr);
            triangles.Add(br);
        }

        // Bottom cap (optional)
        for (int i = 1; i < count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // Top cap
        for (int i = 1; i < count - 1; i++)
        {
            triangles.Add(count);
            triangles.Add(count + i + 1);
            triangles.Add(count + i);
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    Vector3 GetCenter(List<Vector3> pts)
    {
        Vector3 c = Vector3.zero;
        foreach (var p in pts) c += p;
        return c / pts.Count;
    }

    List<Vector3> SortPointsByAngle(List<Vector3> pts, Vector3 center)
    {
        List<Vector3> sorted = new List<Vector3>(pts);
        sorted.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(a.z - center.z, a.x - center.x);
            float angleB = Mathf.Atan2(b.z - center.z, b.x - center.x);
            return angleA.CompareTo(angleB);
        });
        return sorted;
    }

    public void UpdateVertex(int index, Vector3 newLocalPos)
    {
        Vector3[] verts = mesh.vertices;
        verts[index] = newLocalPos;
        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }
}
*/











/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshDrawer : MonoBehaviour
{
    public ActionBasedController controller;
    public GameObject handlePrefab;
    public float pointDistance = 0.02f;
    public float extrusionHeight = 0.05f;

    private List<Vector3> rawPoints = new List<Vector3>();
    private Mesh mesh;
    private MeshFilter meshFilter;
    private bool isDrawing = false;
    private bool finalized = false;
    private LineRenderer lineRenderer;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = false;
    }

    void Update()
    {
        if (finalized) return;

        float trigger = controller.selectAction.action.ReadValue<float>();

        if (trigger > 0.1f)
        {
            if (!isDrawing)
            {
                isDrawing = true;
                rawPoints.Clear();
                lineRenderer.positionCount = 0;
            }

            Vector3 localPos = transform.InverseTransformPoint(controller.transform.position);

            if (rawPoints.Count == 0 || Vector3.Distance(rawPoints[rawPoints.Count - 1], localPos) > pointDistance)
            {
                rawPoints.Add(localPos);
                lineRenderer.positionCount = rawPoints.Count;
                lineRenderer.SetPositions(rawPoints.ToArray());
            }
        }
        else if (isDrawing)
        {
            isDrawing = false;
            FinalizeMesh();
        }
    }

    void FinalizeMesh()
    {
        finalized = true;
        lineRenderer.enabled = false;

        List<Vector3> resampled = ResamplePoints(rawPoints, pointDistance);
        if (resampled.Count < 3) return;

        // FLATTEN all points to Y = 0 (for a flat base)
        for (int i = 0; i < resampled.Count; i++)
        {
            Vector3 p = resampled[i];
            resampled[i] = new Vector3(p.x, 0f, p.z);
        }

        Vector3 center = GetCenter(resampled);
        List<Vector3> sorted = SortPointsByAngle(resampled, center);
        BuildExtrudedMesh(sorted, center);
    }


    void BuildExtrudedMesh(List<Vector3> points, Vector3 center)
    {
        int count = points.Count;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Dictionary<int, Vector3> topVertexHandles = new Dictionary<int, Vector3>();
        float thickness = 0.05f; // Thickness for the 3D mesh

        Vector3 offset = -center;

        // Add top face vertices (at Y=0)
        for (int i = 0; i < count; i++)
        {
            vertices.Add(points[i] + offset);
        }

        // Add bottom face vertices (at Y=-thickness)
        for (int i = 0; i < count; i++)
        {
            Vector3 bottom = points[i] + offset + Vector3.down * thickness;
            vertices.Add(bottom);
            topVertexHandles[i + count] = vertices[i]; // Use top vertices for handles
        }

        // Create top face triangles
        for (int i = 1; i < count - 1; i++)
        {
            triangles.Add(0); triangles.Add(i); triangles.Add(i + 1);
        }

        // Create bottom face triangles (reverse winding)
        for (int i = 1; i < count - 1; i++)
        {
            triangles.Add(count); triangles.Add(count + i + 1); triangles.Add(count + i);
        }

        // Create side triangles
        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;
            int tl = i; // Top left
            int tr = next; // Top right
            int bl = i + count; // Bottom left
            int br = next + count; // Bottom right

            // Two triangles per side
            triangles.Add(tl); triangles.Add(bl); triangles.Add(tr);
            triangles.Add(tr); triangles.Add(bl); triangles.Add(br);
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        foreach (var kvp in topVertexHandles)
        {
            GameObject handle = Instantiate(handlePrefab, transform);
            handle.transform.localPosition = kvp.Value;
            Handle h = handle.GetComponent<Handle>();
            h.vertexIndex = kvp.Key;
            h.meshRef = this;
        }
    }

    Vector3 GetCenter(List<Vector3> pts)
    {
        Vector3 sum = Vector3.zero;
        foreach (var p in pts) sum += p;
        return sum / pts.Count;
    }

    List<Vector3> SortPointsByAngle(List<Vector3> pts, Vector3 center)
    {
        pts.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(a.z - center.z, a.x - center.x);
            float angleB = Mathf.Atan2(b.z - center.z, b.x - center.x);
            return angleA.CompareTo(angleB);
        });
        return pts;
    }

    List<Vector3> ResamplePoints(List<Vector3> input, float spacing)
    {
        List<Vector3> result = new List<Vector3>();
        if (input.Count < 2) return input;

        result.Add(input[0]);
        float accDist = 0f;
        for (int i = 1; i < input.Count; i++)
        {
            accDist += Vector3.Distance(input[i - 1], input[i]);
            if (accDist >= spacing)
            {
                result.Add(input[i]);
                accDist = 0f;
            }
        }
        return result;
    }

    public void UpdateVertex(int index, Vector3 newLocalPos)
    {
        Vector3[] verts = mesh.vertices;
        if (index < verts.Length)
        {
            verts[index] = newLocalPos;
            mesh.vertices = verts;
            mesh.RecalculateNormals();
        }
    }
}*/





///////////////////////////////////////////////////////////////////////////////////////////////////workingggggggggggggggggggggggggggggggggg////////

/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshDrawer : MonoBehaviour
{
    public ActionBasedController controller;
    public GameObject handlePrefab;
    public float pointDistance = 0.02f;
    public float patchThickness = 0.01f;
    public int smoothingPasses = 3;
    public float smoothingStrength = 0.5f;

    private List<Vector3> rawPoints = new List<Vector3>();
    private Mesh mesh;
    private MeshFilter meshFilter;
    private bool isDrawing = false;
    private bool finalized = false;
    private bool hasStartHandle = false;
    private GameObject startHandle;
    private Vector3 startHandlePosition;
    private bool triggerPressed = false;
    private List<Vector3> smoothedPoints;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        if (finalized) return;

        float trigger = controller.selectAction.action.ReadValue<float>();
        bool triggerCurrentlyPressed = trigger > 0.1f;

        if (triggerCurrentlyPressed && !triggerPressed)
        {
            OnTriggerPressed();
        }
        else if (!triggerCurrentlyPressed && triggerPressed)
        {
            OnTriggerReleased();
        }

        triggerPressed = triggerCurrentlyPressed;

        if (triggerPressed && isDrawing)
        {
            ContinueDrawing();
        }
    }

    void OnTriggerPressed()
    {
        if (!hasStartHandle)
        {
            CreateStartHandle();
        }
        else if (!isDrawing)
        {
            StartDrawing();
        }
    }

    void OnTriggerReleased()
    {
        if (isDrawing)
        {
            FinalizeMesh();
        }
    }

    void CreateStartHandle()
    {
        Vector3 localPos = transform.InverseTransformPoint(controller.transform.position);
        startHandlePosition = localPos;

        startHandle = Instantiate(handlePrefab, transform);
        startHandle.transform.localPosition = startHandlePosition;

        hasStartHandle = true;
        Debug.Log("Start handle created. Click trigger again to start drawing.");
    }

    void StartDrawing()
    {
        isDrawing = true;
        rawPoints.Clear();
        rawPoints.Add(startHandlePosition);
        Debug.Log("Started drawing patch. Release trigger to finalize.");
    }

    void ContinueDrawing()
    {
        Vector3 localPos = transform.InverseTransformPoint(controller.transform.position);

        if (rawPoints.Count == 0 || Vector3.Distance(rawPoints[rawPoints.Count - 1], localPos) > pointDistance)
        {
            rawPoints.Add(localPos);

            if (rawPoints.Count >= 3)
            {
                UpdatePatchMesh();
            }
        }
    }

    void UpdatePatchMesh()
    {
        smoothedPoints = SmoothPoints(rawPoints);
        BuildSmoothPatch(smoothedPoints);
    }

    List<Vector3> SmoothPoints(List<Vector3> points)
    {
        if (points.Count < 3) return points;

        List<Vector3> smoothed = new List<Vector3>(points);

        for (int pass = 0; pass < smoothingPasses; pass++)
        {
            List<Vector3> newSmoothed = new List<Vector3>();
            newSmoothed.Add(smoothed[0]);

            for (int i = 1; i < smoothed.Count - 1; i++)
            {
                Vector3 avg = (smoothed[i - 1] + smoothed[i] + smoothed[i + 1]) / 3f;
                Vector3 blended = Vector3.Lerp(smoothed[i], avg, smoothingStrength);
                newSmoothed.Add(blended);
            }

            newSmoothed.Add(smoothed[smoothed.Count - 1]);
            smoothed = newSmoothed;
        }

        return smoothed;
    }

    void BuildSmoothPatch(List<Vector3> points)
    {
        if (points.Count < 3) return;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Add boundary points as vertices (top surface)
        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(points[i]);
        }

        // Simple triangulation for top surface
        for (int i = 1; i < points.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // Add bottom surface for thickness
        int bottomStartIndex = vertices.Count;
        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(points[i] + Vector3.down * patchThickness);
        }

        // Bottom surface triangles (reverse winding)
        for (int i = 1; i < points.Count - 1; i++)
        {
            triangles.Add(bottomStartIndex);
            triangles.Add(bottomStartIndex + i + 1);
            triangles.Add(bottomStartIndex + i);
        }

        // Create side walls
        for (int i = 0; i < points.Count; i++)
        {
            int current = i;
            int next = (i + 1) % points.Count;
            int bottomCurrent = bottomStartIndex + i;
            int bottomNext = bottomStartIndex + (i + 1) % points.Count;

            triangles.Add(current);
            triangles.Add(bottomCurrent);
            triangles.Add(next);

            triangles.Add(next);
            triangles.Add(bottomCurrent);
            triangles.Add(bottomNext);
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void FinalizeMesh()
    {
        finalized = true;
        isDrawing = false;

        if (startHandle != null)
        {
            DestroyImmediate(startHandle);
        }

        if (rawPoints.Count < 3)
        {
            Debug.LogWarning("Not enough points to create patch.");
            return;
        }

        smoothedPoints = SmoothPoints(rawPoints);
        BuildSmoothPatch(smoothedPoints);
        CreateEditingHandles(smoothedPoints);

        Debug.Log("Smooth patch finalized with editing handles!");
    }

    void CreateEditingHandles(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            GameObject handle = Instantiate(handlePrefab, transform);
            handle.transform.localPosition = points[i];

            Handle handleScript = handle.GetComponent<Handle>();
            if (handleScript != null)
            {
                // Assign vertexIndex to match the top surface vertex index (0 to points.Count-1)
                handleScript.vertexIndex = i;
                handleScript.meshRef = this;
            }
        }
    }

    public void Reset()
    {
        finalized = false;
        hasStartHandle = false;
        isDrawing = false;
        rawPoints.Clear();

        if (startHandle != null)
        {
            DestroyImmediate(startHandle);
        }

        mesh.Clear();

        Handle[] existingHandles = GetComponentsInChildren<Handle>();
        for (int i = 0; i < existingHandles.Length; i++)
        {
            DestroyImmediate(existingHandles[i].gameObject);
        }
    }

    public void UpdateVertex(int index, Vector3 newLocalPos)
    {
        Vector3[] verts = mesh.vertices;
        if (index < verts.Length)
        {
            verts[index] = newLocalPos;

            int bottomIndex = (verts.Length / 2) + index;
            if (bottomIndex < verts.Length)
            {
                verts[bottomIndex] = newLocalPos + Vector3.down * patchThickness;
            }

            mesh.vertices = verts;
            mesh.RecalculateNormals();
        }
    }
}*/

///////////////////////////lastest i am usinggg

/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshDrawer : MonoBehaviour
{
    public ActionBasedController controller;
    public GameObject handlePrefab;
    public float pointDistance = 0.02f;
    public float patchThickness = 0.01f;
    public int smoothingPasses = 3;
    public float smoothingStrength = 0.5f;

    private List<Vector3> rawPoints = new List<Vector3>();
    private Mesh mesh;
    private MeshFilter meshFilter;
    private bool isDrawing = false;
    private bool finalized = false;
    private bool hasStartHandle = false;
    private GameObject startHandle;
    private Vector3 startHandlePosition;
    private bool triggerPressed = false;
    private List<Vector3> smoothedPoints;
    private Vector3 centerPoint;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        if (finalized) return;

        float trigger = controller.activateAction.action.ReadValue<float>();
        bool triggerCurrentlyPressed = trigger > 0.1f;

        if (triggerCurrentlyPressed && !triggerPressed)
        {
            OnTriggerPressed();
        }
        else if (!triggerCurrentlyPressed && triggerPressed)
        {
            OnTriggerReleased();
        }

        triggerPressed = triggerCurrentlyPressed;

        if (triggerPressed && isDrawing)
        {
            ContinueDrawing();
        }
    }

    void OnTriggerPressed()
    {
        if (!hasStartHandle)
        {
            CreateStartHandle();
        }
        else if (!isDrawing)
        {
            StartDrawing();
        }
    }

    void OnTriggerReleased()
    {
        if (isDrawing)
        {
            FinalizeMesh();
        }
    }

    void CreateStartHandle()
    {
        Vector3 localPos = transform.InverseTransformPoint(controller.transform.position);
        startHandlePosition = localPos;

        startHandle = Instantiate(handlePrefab, transform);
        startHandle.transform.localPosition = startHandlePosition;

        hasStartHandle = true;
        Debug.Log("Start handle created. Click trigger again to start drawing.");
    }

    void StartDrawing()
    {
        isDrawing = true;
        rawPoints.Clear();
        rawPoints.Add(startHandlePosition);
        Debug.Log("Started drawing patch. Release trigger to finalize.");
    }

    void ContinueDrawing()
    {
        Vector3 localPos = transform.InverseTransformPoint(controller.transform.position);

        if (rawPoints.Count == 0 || Vector3.Distance(rawPoints[rawPoints.Count - 1], localPos) > pointDistance)
        {
            rawPoints.Add(localPos);

            if (rawPoints.Count >= 3)
            {
                UpdatePatchMesh();
            }
        }
    }

    void UpdatePatchMesh()
    {
        smoothedPoints = SmoothPoints(rawPoints);
        BuildSmoothPatch(smoothedPoints);
        UpdateBoxCollider(); // Update collider to match mesh
    }

    List<Vector3> SmoothPoints(List<Vector3> points)
    {
        if (points.Count < 3) return points;

        List<Vector3> smoothed = new List<Vector3>(points);

        for (int pass = 0; pass < smoothingPasses; pass++)
        {
            List<Vector3> newSmoothed = new List<Vector3>();
            newSmoothed.Add(smoothed[0]);

            for (int i = 1; i < smoothed.Count - 1; i++)
            {
                Vector3 avg = (smoothed[i - 1] + smoothed[i] + smoothed[i + 1]) / 3f;
                Vector3 blended = Vector3.Lerp(smoothed[i], avg, smoothingStrength);
                newSmoothed.Add(blended);
            }

            newSmoothed.Add(smoothed[smoothed.Count - 1]);
            smoothed = newSmoothed;
        }

        return smoothed;
    }

    void BuildSmoothPatch(List<Vector3> points)
    {
        if (points.Count < 3) return;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // --- Top surface ---
        centerPoint = Vector3.zero;
        foreach (var p in points) centerPoint += p;
        centerPoint /= points.Count;

        int centerTopIndex = vertices.Count;
        vertices.Add(centerPoint); // Top center

        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(points[i]); // Top edge
        }

        for (int i = 0; i < points.Count; i++)
        {
            int current = centerTopIndex + 1 + i;
            int next = centerTopIndex + 1 + (i + 1) % points.Count;
            triangles.Add(centerTopIndex);
            triangles.Add(current);
            triangles.Add(next);
        }

        // --- Bottom surface ---
        int centerBottomIndex = vertices.Count;
        vertices.Add(centerPoint + Vector3.down * patchThickness); // Bottom center

        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(points[i] + Vector3.down * patchThickness); // Bottom edge
        }

        for (int i = 0; i < points.Count; i++)
        {
            int current = centerBottomIndex + 1 + i;
            int next = centerBottomIndex + 1 + (i + 1) % points.Count;
            triangles.Add(centerBottomIndex);
            triangles.Add(next);
            triangles.Add(current);
        }

        // --- Side walls ---
        for (int i = 0; i < points.Count; i++)
        {
            int top = centerTopIndex + 1 + i;
            int topNext = centerTopIndex + 1 + (i + 1) % points.Count;
            int bottom = centerBottomIndex + 1 + i;
            int bottomNext = centerBottomIndex + 1 + (i + 1) % points.Count;

            triangles.Add(top);
            triangles.Add(bottom);
            triangles.Add(topNext);

            triangles.Add(topNext);
            triangles.Add(bottom);
            triangles.Add(bottomNext);
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void FinalizeMesh()
    {
        finalized = true;
        isDrawing = false;

        if (startHandle != null)
        {
            DestroyImmediate(startHandle);
        }

        if (rawPoints.Count < 3)
        {
            Debug.LogWarning("Not enough points to create patch.");
            return;
        }

        smoothedPoints = SmoothPoints(rawPoints);
        BuildSmoothPatch(smoothedPoints);
        UpdateBoxCollider(); // Update collider to match mesh
        CreateEditingHandles(smoothedPoints);
        CreateCenterHandle(centerPoint);

        Debug.Log("Smooth patch finalized with center and editing handles!");
    }

    void CreateEditingHandles(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i += 2)
        {
            GameObject handle = Instantiate(handlePrefab, transform);
            handle.transform.localPosition = points[i];

            Handle handleScript = handle.GetComponent<Handle>();
            if (handleScript != null)
            {
                handleScript.vertexIndex = i + 1; // Offset due to center point
                handleScript.meshRef = this;
            }
        }
    }

    void CreateCenterHandle(Vector3 center)
    {
        GameObject centerHandle = Instantiate(handlePrefab, transform);
        centerHandle.transform.localPosition = center;

        Handle handleScript = centerHandle.GetComponent<Handle>();
        if (handleScript != null)
        {
            handleScript.vertexIndex = 0; // Center is at index 0
            handleScript.meshRef = this;
        }
    }

    public void Reset()
    {
        finalized = false;
        hasStartHandle = false;
        isDrawing = false;
        rawPoints.Clear();

        if (startHandle != null)
        {
            DestroyImmediate(startHandle);
        }

        mesh.Clear();

        Handle[] existingHandles = GetComponentsInChildren<Handle>();
        for (int i = 0; i < existingHandles.Length; i++)
        {
            DestroyImmediate(existingHandles[i].gameObject);
        }

        UpdateBoxCollider(); // Reset collider
    }

    public void UpdateVertex(int index, Vector3 newLocalPos)
    {
        Vector3[] verts = mesh.vertices;

        if (index >= verts.Length) return;

        verts[index] = newLocalPos;

        int centerBottomIndex = verts.Length / 2;

        if (index == 0)
        {
            // Update bottom center too
            if (centerBottomIndex < verts.Length)
                verts[centerBottomIndex] = newLocalPos + Vector3.down * patchThickness;
        }
        else
        {
            int outerIndex = index - 1;
            int bottomIndex = centerBottomIndex + 1 + outerIndex;
            if (bottomIndex < verts.Length)
                verts[bottomIndex] = newLocalPos + Vector3.down * patchThickness;
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        UpdateBoxCollider(); // Update collider to match mesh
    }

    void UpdateBoxCollider()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }

        // Get the mesh bounds
        Bounds bounds = mesh.bounds;

        // Set the collider's center and size to match the mesh bounds
        collider.center = bounds.center;
        collider.size = bounds.size;
    }
}
*/




///////////////////////////with delete function//////////////////
///


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshDrawer : MonoBehaviour
{
    public ActionBasedController controller;
    public GameObject handlePrefab;
    public float pointDistance = 0.02f;
    public float patchThickness = 0.01f;
    public int smoothingPasses = 3;
    public float smoothingStrength = 0.5f;
    public InputActionProperty deleteButton; // Add this line with other public fields  

    private List<Vector3> rawPoints = new List<Vector3>();
    private Mesh mesh;
    private MeshFilter meshFilter;
    private bool isDrawing = false;
    private bool finalized = false;
    private bool hasStartHandle = false;
    private GameObject startHandle;
    private Vector3 startHandlePosition;
    private bool triggerPressed = false;
    private List<Vector3> smoothedPoints;
    private Vector3 centerPoint;
    private bool deleteButtonPressed = false; // Add this to prevent multiple deletions

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        // Check for delete button press when mesh is finalized
        if (finalized)
        {
            float deleteValue = deleteButton.action.ReadValue<float>();
            bool deleteCurrentlyPressed = deleteValue > 0.1f;

            if (deleteCurrentlyPressed && !deleteButtonPressed)
            {
                DeleteMesh();
                return; // Exit early since object will be destroyed
            }

            deleteButtonPressed = deleteCurrentlyPressed;

            // Return early to prevent any trigger input processing when finalized
            return;
        }

        float trigger = controller.activateAction.action.ReadValue<float>();
        bool triggerCurrentlyPressed = trigger > 0.1f;

        if (triggerCurrentlyPressed && !triggerPressed)
        {
            OnTriggerPressed();
        }
        else if (!triggerCurrentlyPressed && triggerPressed)
        {
            OnTriggerReleased();
        }

        triggerPressed = triggerCurrentlyPressed;

        if (triggerPressed && isDrawing)
        {
            ContinueDrawing();
        }
    }

    void OnTriggerPressed()
    {
        // Don't allow any trigger actions if mesh is finalized
        if (finalized) return;

        if (!hasStartHandle)
        {
            CreateStartHandle();
        }
        else if (!isDrawing)
        {
            StartDrawing();
        }
    }

    void OnTriggerReleased()
    {
        // Don't allow any trigger actions if mesh is finalized
        if (finalized) return;

        if (isDrawing)
        {
            FinalizeMesh();
        }
    }

    void CreateStartHandle()
    {
        Vector3 localPos = transform.InverseTransformPoint(controller.transform.position);
        startHandlePosition = localPos;

        startHandle = Instantiate(handlePrefab, transform);
        startHandle.transform.localPosition = startHandlePosition;

        hasStartHandle = true;
        Debug.Log("Start handle created. Click trigger again to start drawing.");
    }

    void StartDrawing()
    {
        isDrawing = true;
        rawPoints.Clear();
        rawPoints.Add(startHandlePosition);
        Debug.Log("Started drawing patch. Release trigger to finalize.");
    }

    void ContinueDrawing()
    {
        Vector3 localPos = transform.InverseTransformPoint(controller.transform.position);

        if (rawPoints.Count == 0 || Vector3.Distance(rawPoints[rawPoints.Count - 1], localPos) > pointDistance)
        {
            rawPoints.Add(localPos);

            if (rawPoints.Count >= 3)
            {
                UpdatePatchMesh();
            }
        }
    }

    void UpdatePatchMesh()
    {
        smoothedPoints = SmoothPoints(rawPoints);
        BuildSmoothPatch(smoothedPoints);
        UpdateBoxCollider();
    }

    List<Vector3> SmoothPoints(List<Vector3> points)
    {
        if (points.Count < 3) return points;

        List<Vector3> smoothed = new List<Vector3>(points);

        for (int pass = 0; pass < smoothingPasses; pass++)
        {
            List<Vector3> newSmoothed = new List<Vector3>();
            newSmoothed.Add(smoothed[0]);

            for (int i = 1; i < smoothed.Count - 1; i++)
            {
                Vector3 avg = (smoothed[i - 1] + smoothed[i] + smoothed[i + 1]) / 3f;
                Vector3 blended = Vector3.Lerp(smoothed[i], avg, smoothingStrength);
                newSmoothed.Add(blended);
            }

            newSmoothed.Add(smoothed[smoothed.Count - 1]);
            smoothed = newSmoothed;
        }

        return smoothed;
    }

    void BuildSmoothPatch(List<Vector3> points)
    {
        if (points.Count < 3) return;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // --- Top surface ---
        centerPoint = Vector3.zero;
        foreach (var p in points) centerPoint += p;
        centerPoint /= points.Count;

        int centerTopIndex = vertices.Count;
        vertices.Add(centerPoint);

        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(points[i]);
        }

        for (int i = 0; i < points.Count; i++)
        {
            int current = centerTopIndex + 1 + i;
            int next = centerTopIndex + 1 + (i + 1) % points.Count;
            triangles.Add(centerTopIndex);
            triangles.Add(current);
            triangles.Add(next);
        }

        // --- Bottom surface ---
        int centerBottomIndex = vertices.Count;
        vertices.Add(centerPoint + Vector3.down * patchThickness);

        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(points[i] + Vector3.down * patchThickness);
        }

        for (int i = 0; i < points.Count; i++)
        {
            int current = centerBottomIndex + 1 + i;
            int next = centerBottomIndex + 1 + (i + 1) % points.Count;
            triangles.Add(centerBottomIndex);
            triangles.Add(next);
            triangles.Add(current);
        }

        // --- Side walls ---
        for (int i = 0; i < points.Count; i++)
        {
            int top = centerTopIndex + 1 + i;
            int topNext = centerTopIndex + 1 + (i + 1) % points.Count;
            int bottom = centerBottomIndex + 1 + i;
            int bottomNext = centerBottomIndex + 1 + (i + 1) % points.Count;

            triangles.Add(top);
            triangles.Add(bottom);
            triangles.Add(topNext);

            triangles.Add(topNext);
            triangles.Add(bottom);
            triangles.Add(bottomNext);
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void FinalizeMesh()
    {
        finalized = true;
        isDrawing = false;

        if (startHandle != null)
        {
            DestroyImmediate(startHandle);
        }

        if (rawPoints.Count < 3)
        {
            Debug.LogWarning("Not enough points to create patch.");
            return;
        }

        smoothedPoints = SmoothPoints(rawPoints);
        BuildSmoothPatch(smoothedPoints);
        UpdateBoxCollider();
        CreateEditingHandles(smoothedPoints);
        CreateCenterHandle(centerPoint);

        Debug.Log("Smooth patch finalized with center and editing handles! Trigger input disabled.");
    }

    void CreateEditingHandles(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i += 2)
        {
            GameObject handle = Instantiate(handlePrefab, transform);
            handle.transform.localPosition = points[i];

            Handle handleScript = handle.GetComponent<Handle>();
            if (handleScript != null)
            {
                handleScript.vertexIndex = i + 1;
                handleScript.meshRef = this;
            }
        }
    }

    void CreateCenterHandle(Vector3 center)
    {
        GameObject centerHandle = Instantiate(handlePrefab, transform);
        centerHandle.transform.localPosition = center;

        Handle handleScript = centerHandle.GetComponent<Handle>();
        if (handleScript != null)
        {
            handleScript.vertexIndex = 0;
            handleScript.meshRef = this;
        }
    }

    void DeleteMesh()
    {
        Reset();
        Destroy(gameObject);
        Debug.Log("Mesh deleted.");
    }

    public void Reset()
    {
        finalized = false;
        hasStartHandle = false;
        isDrawing = false;
        rawPoints.Clear();

        if (startHandle != null)
        {
            DestroyImmediate(startHandle);
        }

        mesh.Clear();

        Handle[] existingHandles = GetComponentsInChildren<Handle>();
        for (int i = 0; i < existingHandles.Length; i++)
        {
            DestroyImmediate(existingHandles[i].gameObject);
        }

        UpdateBoxCollider();
    }

    public void UpdateVertex(int index, Vector3 newLocalPos)
    {
        Vector3[] verts = mesh.vertices;

        if (index >= verts.Length) return;

        verts[index] = newLocalPos;

        int centerBottomIndex = verts.Length / 2;

        if (index == 0)
        {
            if (centerBottomIndex < verts.Length)
                verts[centerBottomIndex] = newLocalPos + Vector3.down * patchThickness;
        }
        else
        {
            int outerIndex = index - 1;
            int bottomIndex = centerBottomIndex + 1 + outerIndex;
            if (bottomIndex < verts.Length)
                verts[bottomIndex] = newLocalPos + Vector3.down * patchThickness;
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        UpdateBoxCollider();
    }

    void UpdateBoxCollider()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }

        Bounds bounds = mesh.bounds;
        collider.center = bounds.center;
        collider.size = bounds.size;
    }
}

