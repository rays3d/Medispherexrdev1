using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates and manages a 3D mesh representing a brush stroke, which is composed of a series of interconnected "cross-rings" (toroidal shapes).
/// It supports dynamic insertion of points, mesh modification via movable/scalable "control points," and overall scaling.
/// </summary>
public class BrushStrokeMesh : MonoBehaviour
{
    // --- Editor-Visible Configuration Parameters ---
    #region Configuration Fields
    [Header("Ring and Tube Parameters")]
    [SerializeField] private float _ringRadius = 0.02f;              // Radius of the main circle of the ring (torus major radius)
    [SerializeField] private float _tubeRadius = 0.003f;             // Radius of the tube along the ring (torus minor radius)
    [SerializeField] private int _ringSegments = 16;                 // Number of segments around the main ring circumference
    [SerializeField] private int _tubeSegments = 8;                  // Number of segments around the tube circumference
    [SerializeField] private float _minDistance = 0.02f;             // Minimum distance between successive ribbon points to create a new ring

    [Header("Brush Zigzag and Connection")]
    [SerializeField] private float _zigzagAngle = 45f;               // Angle offset for the second ring in the "cross-ring" pair, creating a zigzag pattern
    [SerializeField] private float _connectionRingRadius = 0.015f;   // Ring radius for the connecting rings between cross-rings
    [SerializeField] private int _connectionRingSegments = 6;        // Ring segments for connecting rings
    [SerializeField] private int _connectionTubeSegments = 6;        // Tube segments for connecting rings

    [Header("Control Point Interaction")]
    [SerializeField] private Material _controlPointMat = null;       // Material for the control point spheres
    [SerializeField] private GameObject _controlPointPrefab = null;   // Prefab for the interactive control points
    [SerializeField] private float _controlPointScale = 0.01f;       // Initial scale for control point GameObjects
    [SerializeField] private int _adjacentInfluenceCount = 4;        // Number of adjacent rings a control point's scale change will influence
    [SerializeField] private bool _ignoreControllerRotation = true;   // If true, the ring rotation is derived from the stroke direction, ignoring input rotation

    [Header("Size Scaling")]
    [SerializeField] private float _sizeMultiplier = 1.0f;           // Global scale multiplier for all geometric elements
    #endregion

    // --- Mesh Data Storage ---
    #region Mesh Data
    private Mesh _mesh;                                              // The generated Unity Mesh
    private List<Vector3> _vertices = new List<Vector3>();           // List of all mesh vertices
    private List<int> _triangles = new List<int>();                 // List of all mesh triangle indices
    private List<Vector3> _normals = new List<Vector3>();           // List of all mesh normals
    #endregion

    // --- Stroke Tracking State ---
    #region Stroke State
    private Vector3 _lastRingPosition;                               // Position of the last successfully created ring
    private Quaternion _lastRingRotation = Quaternion.identity;      // Rotation of the last successfully created ring
    private bool _hasLastPosition = false;                           // Flag to track if the first point has been recorded
    private int _dotCount = 0;                                       // Counter for the number of successful cross-rings created (used for indexing)

    private List<Vector3> _connectionPoints = new List<Vector3>();   // Points on the last cross-ring used to start connection rings
    private List<Vector3> _connectionDirections = new List<Vector3>(); // Directions corresponding to connectionPoints
    #endregion

    // --- Control Point Management and Deformation Data ---
    #region Control Point and Deformation
    private List<ControlPoint> _controlPoints = new List<ControlPoint>(); // List of all created ControlPoint structures
    private Dictionary<int, float> _accumulatedScales = new Dictionary<int, float>(); // Stores the maximum accumulated scale factor for each control point index

    // Flag to skip the last point in the ribbon (currently unused, but defined for API)
    private bool _skipLastRibbonPoint = false;
    public bool skipLastRibbonPoint
    {
        get => _skipLastRibbonPoint;
        set => _skipLastRibbonPoint = value;
    }
    #endregion

    // --- Public Properties ---
    #region Public Properties
    /// <summary>
    /// Global scale multiplier for the brush stroke geometry. Clamped to a minimum of 0.1f.
    /// </summary>
    public float sizeMultiplier
    {
        get => _sizeMultiplier;
        set => _sizeMultiplier = Mathf.Max(0.1f, value);
    }
    #endregion

    // --- Internal Control Point Data Structure ---
    #region ControlPoint Structure
    /// <summary>
    /// Data structure to hold information about an interactive control point.
    /// </summary>
    private class ControlPoint
    {
        public GameObject gameObject;                     // The Unity GameObject (e.g., sphere) for manipulation
        public Vector3 originalPosition;                 // Position when the point was created
        public Quaternion originalRotation;               // Rotation when the point was created
        public Vector3 initialScale;                     // Initial local scale of the GameObject
        public int crossRingIndex;                       // Index of the brush stroke segment this point corresponds to
        public List<int> affectedVertexIndices = new List<int>(); // Indices of vertices belonging to this segment
        public List<Vector3> originalVertexOffsets = new List<Vector3>(); // Local offset from the control point's center to each affected vertex
    }
    #endregion

    // --- Unity Lifecycle Methods ---
    #region Unity Lifecycle
    private void Awake()
    {
        // Get or add MeshFilter component and initialize the Mesh object
        MeshFilter filter = GetComponent<MeshFilter>();
        if (!filter) filter = gameObject.AddComponent<MeshFilter>();
        _mesh = filter.mesh;
        if (!_mesh) _mesh = new Mesh();

        // Ensure the mesh starts empty
        ClearRibbon();
    }

    private void Update()
    {
        // Periodically check if any control points have been moved/scaled and update the mesh accordingly
        UpdateMeshFromControlPoints();
    }
    #endregion

    // --- Ribbon Creation Methods ---
    #region Ribbon Creation
    /// <summary>
    /// Inserts a new point into the brush stroke, generating mesh geometry if the distance threshold is met.
    /// </summary>
    /// <param name="position">The world position of the new point.</param>
    /// <param name="rotation">The world rotation of the controller/input source.</param>
    public void InsertRibbonPoint(Vector3 position, Quaternion rotation)
    {
        Quaternion finalRotation = rotation;

        // Determine the actual ring rotation based on the stroke direction if ignoring controller rotation
        if (_ignoreControllerRotation)
        {
            if (!_hasLastPosition)
            {
                // First point: use forward direction from controller/input
                finalRotation = Quaternion.LookRotation(rotation * Vector3.forward, Vector3.up);
            }
            else
            {
                // Subsequent points: align with the direction of movement
                Vector3 direction = (position - _lastRingPosition).normalized;
                if (direction.magnitude > 0.01f)
                    finalRotation = Quaternion.LookRotation(direction, Vector3.up);
                else
                    finalRotation = _lastRingRotation; // Maintain previous rotation if not moving
            }
        }

        // FIRST CALL: Only store initial state, do not generate geometry
        if (!_hasLastPosition)
        {
            _lastRingPosition = position;
            _lastRingRotation = finalRotation;
            _hasLastPosition = true;
            return;
        }

        float distance = Vector3.Distance(position, _lastRingPosition);
        float scaledMinDistance = _minDistance * _sizeMultiplier;

        // Check if the distance threshold has been reached
        if (distance >= scaledMinDistance)
        {
            List<Vector3> currentPoints;
            List<Vector3> currentDirections;
            int startVertexIndex = _vertices.Count;

            // 1. Create the new cross-ring geometry
            CreateCrossRings(position, finalRotation, out currentPoints, out currentDirections);

            // 2. Connect the previous cross-ring to the new one
            if (_connectionPoints.Count == currentPoints.Count)
            {
                for (int i = 0; i < _connectionPoints.Count; i++)
                {
                    // Create a small connecting ring between corresponding points on the rings
                    CreateConnectionRing(_connectionPoints[i], currentPoints[i], _connectionDirections[i]);
                }
            }

            // 3. Create a control point associated with this new segment
            CreateControlPoint(position, finalRotation, _dotCount, startVertexIndex, _vertices.Count);

            // 4. Update state variables for the next point
            _connectionPoints = currentPoints;
            _connectionDirections = currentDirections;
            _lastRingPosition = position;
            _lastRingRotation = finalRotation;
            _dotCount++;

            // 5. Apply the changes to the actual mesh component
            UpdateGeometry();
        }
    }

    /// <summary>
    /// Placeholder method for updating the position/rotation of the last inserted ribbon point.
    /// (Currently empty, suggesting functionality for dynamically updating the stroke end is not implemented).
    /// </summary>
    public void UpdateLastRibbonPoint(Vector3 position, Quaternion rotation) { }

    /// <summary>
    /// Clears all stored data, destroys control points, and resets the mesh.
    /// </summary>
    public void ClearRibbon()
    {
        _vertices.Clear();
        _normals.Clear();
        _triangles.Clear();
        _hasLastPosition = false;
        _dotCount = 0;
        _connectionPoints.Clear();
        _connectionDirections.Clear();
        _accumulatedScales.Clear();
        _lastRingRotation = Quaternion.identity;

        // Destroy all control point GameObjects
        foreach (var cp in _controlPoints)
        {
            if (cp.gameObject != null)
                Destroy(cp.gameObject);
        }
        _controlPoints.Clear();

        // Clear the actual Unity mesh data
        if (_mesh != null)
        {
            _mesh.Clear();
        }
    }
    #endregion

    // --- Control Point Methods ---
    #region Control Point Management
    /// <summary>
    /// Creates a visual control point GameObject and stores its associated data.
    /// </summary>
    private void CreateControlPoint(Vector3 position, Quaternion rotation, int index, int startVertexIndex, int endVertexIndex)
    {
        GameObject cpObject = null;
        float scaledControlPointSize = _controlPointScale * _sizeMultiplier;

        // Instantiate prefab or create a default sphere primitive
        if (_controlPointPrefab != null)
        {
            cpObject = Instantiate(_controlPointPrefab, position, rotation, transform);
        }
        else
        {
            cpObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cpObject.transform.position = position;
            cpObject.transform.rotation = rotation;
            cpObject.transform.parent = transform;
            cpObject.transform.GetComponent<Renderer>().material = _controlPointMat;
        }

        cpObject.name = "ControlPoint_" + index;
        cpObject.transform.localScale = Vector3.one * scaledControlPointSize;
      

        // Store the ControlPoint data
        ControlPoint cp = new ControlPoint
        {
            gameObject = cpObject,
            originalPosition = position,
            originalRotation = rotation,
            initialScale = cpObject.transform.localScale,
            crossRingIndex = index
        };

        // Record which vertices belong to this segment and their original offsets
        for (int i = startVertexIndex; i < endVertexIndex; i++)
        {
            cp.affectedVertexIndices.Add(i);
            cp.originalVertexOffsets.Add(_vertices[i] - position);
        }
        _controlPoints.Add(cp);
    }

    /// <summary>
    /// Checks all control points for changes in position, rotation, or scale,
    /// calculates their influence on adjacent points, and updates the mesh vertices.
    /// </summary>
    private void UpdateMeshFromControlPoints()
    {
        bool meshChanged = false;
        _accumulatedScales.Clear();

        // Phase 1: Calculate the accumulated scale influence across adjacent control points
        for (int cpIndex = 0; cpIndex < _controlPoints.Count; cpIndex++)
        {
            var cp = _controlPoints[cpIndex];
            if (cp.gameObject == null) continue;

            Vector3 currentScale = cp.gameObject.transform.localScale;
            // Calculate the scaling factor relative to the original size
            float scaleFactor = currentScale.x / cp.initialScale.x;

            if (Mathf.Abs(scaleFactor - 1f) > 0.001f)
            {
                // Distribute the scale change to adjacent control points
                for (int offset = -_adjacentInfluenceCount; offset <= _adjacentInfluenceCount; offset++)
                {
                    int targetIndex = cpIndex + offset;
                    if (targetIndex >= 0 && targetIndex < _controlPoints.Count)
                    {
                        // Linear influence falloff
                        float influence = 1f - (Mathf.Abs(offset) / (float)(_adjacentInfluenceCount + 1));
                        float adjacentScale = 1f + (scaleFactor - 1f) * influence;

                        // Accumulate the maximum scale factor for each affected index
                        if (!_accumulatedScales.ContainsKey(targetIndex))
                            _accumulatedScales[targetIndex] = adjacentScale;
                        else
                            _accumulatedScales[targetIndex] = Mathf.Max(_accumulatedScales[targetIndex], adjacentScale);
                    }
                }
            }
        }

        // Phase 2: Apply position, rotation, and accumulated scale deformation to the vertices
        for (int cpIndex = 0; cpIndex < _controlPoints.Count; cpIndex++)
        {
            var cp = _controlPoints[cpIndex];
            if (cp.gameObject == null) continue;

            Vector3 currentPos = cp.gameObject.transform.position;
            Quaternion currentRot = cp.gameObject.transform.rotation;

            Vector3 positionDelta = currentPos - cp.originalPosition;
            Quaternion rotationDelta = currentRot * Quaternion.Inverse(cp.originalRotation);

            // Get the final scale factor for this point, applying the accumulated maximum
            float finalScaleFactor = _accumulatedScales.ContainsKey(cpIndex) ? _accumulatedScales[cpIndex] : 1f;

            // Check if any deformation has occurred since the last update
            if (positionDelta.magnitude > 0.0001f ||
                Quaternion.Angle(Quaternion.identity, rotationDelta) > 0.01f ||
                Mathf.Abs(finalScaleFactor - 1f) > 0.001f)
            {
                meshChanged = true;

                // Apply deformation to all vertices affected by this control point
                for (int i = 0; i < cp.affectedVertexIndices.Count; i++)
                {
                    int vertexIndex = cp.affectedVertexIndices[i];
                    if (vertexIndex < _vertices.Count)
                    {
                        Vector3 localVertex = cp.originalVertexOffsets[i];
                        // Apply rotation delta
                        localVertex = rotationDelta * localVertex;
                        // Apply scale factor
                        localVertex *= finalScaleFactor;
                        // Calculate new world position: original position + transformed local offset + position delta
                        _vertices[vertexIndex] = cp.originalPosition + localVertex + positionDelta;
                    }
                }
            }
        }

        // Phase 3: Update the actual mesh with the new vertex positions
        if (meshChanged)
            UpdateGeometry();
    }
    #endregion

    // --- Mesh Generation Primitives ---
    #region Mesh Primitives
    /// <summary>
    /// Creates a pair of intersecting rings (a "cross-ring") at a specific location and orientation,
    /// designed to give the brush stroke volume and a specific visual texture.
    /// </summary>
    private void CreateCrossRings(Vector3 center, Quaternion rotation, out List<Vector3> connectionPoints, out List<Vector3> connectionDirections)
    {
        connectionPoints = new List<Vector3>();
        connectionDirections = new List<Vector3>();

        float scaledRingRadius = _ringRadius * _sizeMultiplier;

        // Ring 1: Oriented along the stroke direction (0-degree roll)
        Quaternion rot0 = rotation * Quaternion.Euler(0, 0, 0);
        CreateRing(center, rot0, scaledRingRadius, _ringSegments, _tubeSegments);

        // Store points for connection 1: Top, Bottom, Left, Right points of the ring
        connectionPoints.Add(center + rot0 * new Vector3(0, scaledRingRadius, 0));
        connectionPoints.Add(center + rot0 * new Vector3(0, -scaledRingRadius, 0));
        connectionPoints.Add(center + rot0 * new Vector3(-scaledRingRadius, 0, 0));
        connectionPoints.Add(center + rot0 * new Vector3(scaledRingRadius, 0, 0));

        // Store corresponding directions (outward normals)
        connectionDirections.Add(rot0 * new Vector3(0, 1, 0));
        connectionDirections.Add(rot0 * new Vector3(0, -1, 0));
        connectionDirections.Add(rot0 * new Vector3(-1, 0, 0));
        connectionDirections.Add(rot0 * new Vector3(1, 0, 0));

        // Ring 2: Rotated by the zigzag angle for visual complexity
        Quaternion rot1 = rotation * Quaternion.Euler(0, 0, _zigzagAngle);
        CreateRing(center, rot1, scaledRingRadius, _ringSegments, _tubeSegments);

        // Store points for connection 2
        connectionPoints.Add(center + rot1 * new Vector3(0, scaledRingRadius, 0));
        connectionPoints.Add(center + rot1 * new Vector3(0, -scaledRingRadius, 0));
        connectionPoints.Add(center + rot1 * new Vector3(-scaledRingRadius, 0, 0));
        connectionPoints.Add(center + rot1 * new Vector3(scaledRingRadius, 0, 0));

        // Store corresponding directions
        connectionDirections.Add(rot1 * new Vector3(0, 1, 0));
        connectionDirections.Add(rot1 * new Vector3(0, -1, 0));
        connectionDirections.Add(rot1 * new Vector3(-1, 0, 0));
        connectionDirections.Add(rot1 * new Vector3(1, 0, 0));
    }

    /// <summary>
    /// Generates the geometry for a single toroidal ring (donut shape) at a specified location.
    /// </summary>
    private void CreateRing(Vector3 center, Quaternion rotation, float ringRadius, int ringSegs, int tubeSegs)
    {
        int startVertexIndex = _vertices.Count;
        float scaledTubeRadius = _tubeRadius * _sizeMultiplier;

        // 1. Generate Vertices and Normals
        for (int i = 0; i <= ringSegs; i++) // Around the main ring
        {
            float ringAngle = 2 * Mathf.PI * i / ringSegs;
            float ringX = ringRadius * Mathf.Cos(ringAngle);
            float ringY = ringRadius * Mathf.Sin(ringAngle);
            Vector3 ringCenter = new Vector3(ringX, ringY, 0); // Center of the tube cross-section in local space

            for (int j = 0; j <= tubeSegs; j++) // Around the tube cross-section
            {
                float tubeAngle = 2 * Mathf.PI * j / tubeSegs;

                // Calculate the direction vector from the ring center to the vertex on the tube surface
                Vector3 tubeDirection = new Vector3(
                    Mathf.Cos(ringAngle) * Mathf.Cos(tubeAngle),
                    Mathf.Sin(ringAngle) * Mathf.Cos(tubeAngle),
                    Mathf.Sin(tubeAngle)
                );

                // Calculate local vertex position (ring center + scaled tube radius * tube direction)
                Vector3 localVertex = ringCenter + tubeDirection * scaledTubeRadius;
                // Transform local vertex to world space
                Vector3 vertex = center + rotation * localVertex;
                
                // The normal is the rotated tube direction (outward from the tube)
                Vector3 normal = rotation * tubeDirection;

                _vertices.Add(vertex);
                _normals.Add(normal);
            }
        }

        // 2. Generate Triangles (Quads made of two triangles)
        for (int i = 0; i < ringSegs; i++)
        {
            for (int j = 0; j < tubeSegs; j++)
            {
                // Indices for the four corners of a quad
                int current = startVertexIndex + i * (tubeSegs + 1) + j;
                int next = current + (tubeSegs + 1); // Next segment along the ring
                int currentNext = current + 1;       // Next segment around the tube
                int nextNext = next + 1;

                // First triangle (current, next, currentNext)
                _triangles.Add(current);
                _triangles.Add(next);
                _triangles.Add(currentNext);

                // Second triangle (currentNext, next, nextNext)
                _triangles.Add(currentNext);
                _triangles.Add(next);
                _triangles.Add(nextNext);
            }
        }
    }

    /// <summary>
    /// Creates a smaller ring that acts as a connector between two points on successive cross-rings.
    /// </summary>
    private void CreateConnectionRing(Vector3 start, Vector3 end, Vector3 perpendicularDirection)
    {
        Vector3 center = (start + end) / 2f;
        Vector3 connectionDirection = (end - start).normalized;

        // Determine the 'up' vector for rotation: must be perpendicular to both the connection direction and the provided 'perpendicularDirection'
        Vector3 up = Vector3.Cross(perpendicularDirection, connectionDirection).normalized;

        // Determine rotation: Look along 'perpendicularDirection', with 'up' as the roll orientation
        Quaternion rotation = Quaternion.LookRotation(perpendicularDirection, up);

        float scaledConnectionRingRadius = _connectionRingRadius * _sizeMultiplier;

        // Create the ring geometry
        CreateRing(center, rotation, scaledConnectionRingRadius, _connectionRingSegments, _connectionTubeSegments);
    }

    /// <summary>
    /// Clears and updates the Mesh component with the current vertex, normal, and triangle data.
    /// </summary>
    private void UpdateGeometry()
    {
        _mesh.Clear();
        _mesh.SetVertices(_vertices);
        _mesh.SetNormals(_normals);
        _mesh.SetTriangles(_triangles, 0);
        _mesh.RecalculateBounds(); // Necessary for proper rendering and culling
    }
    #endregion
}