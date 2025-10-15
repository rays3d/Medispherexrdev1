using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Generates and manages a 3D mesh representing a brush stroke, composed of interconnected "cross-rings".
/// Supports dynamic insertion of points, mesh modification via movable/scalable control points, and overall scaling.
/// </summary>
public class BrushStrokeMesh : MonoBehaviourPun
{
    #region Configuration Fields
    [Header("Ring and Tube Parameters")]
    [SerializeField] private float _ringRadius = 0.02f;
    [SerializeField] private float _tubeRadius = 0.003f;
    [SerializeField] private int _ringSegments = 16;
    [SerializeField] private int _tubeSegments = 8;
    [SerializeField] private float _minDistance = 0.02f;

    [Header("Brush Zigzag and Connection")]
    [SerializeField] private float _zigzagAngle = 45f;
    [SerializeField] private float _connectionRingRadius = 0.015f;
    [SerializeField] private int _connectionRingSegments = 6;
    [SerializeField] private int _connectionTubeSegments = 6;

    [Header("Control Point Interaction")]
    [SerializeField] private Material _controlPointMat = null;
    [SerializeField] private GameObject _controlPointPrefab = null;
    [SerializeField] private float _controlPointScale = 0.01f;
    [SerializeField] private int _adjacentInfluenceCount = 4;
    [SerializeField] private bool _ignoreControllerRotation = true;

    [Header("Size Scaling")]
    [SerializeField] private float _sizeMultiplier = 1.0f;
    #endregion

    #region Mesh Data
    private Mesh _mesh;
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Vector3> _normals = new List<Vector3>();
    #endregion

    #region Stroke State
    private Vector3 _lastRingPosition;
    private Quaternion _lastRingRotation = Quaternion.identity;
    private bool _hasLastPosition = false;
    private int _dotCount = 0;

    private List<Vector3> _connectionPoints = new List<Vector3>();
    private List<Vector3> _connectionDirections = new List<Vector3>();
    #endregion

    #region Control Point and Deformation
    private List<ControlPoint> _controlPoints = new List<ControlPoint>();
    private Dictionary<int, float> _accumulatedScales = new Dictionary<int, float>();

    private bool _skipLastRibbonPoint = false;
    public bool skipLastRibbonPoint
    {
        get => _skipLastRibbonPoint;
        set => _skipLastRibbonPoint = value;
    }
    #endregion

    #region Public Properties
    public float sizeMultiplier
    {
        get => _sizeMultiplier;
        set => _sizeMultiplier = Mathf.Max(0.1f, value);
    }
    #endregion

    #region ControlPoint Structure
    private class ControlPoint
    {
        public GameObject gameObject;
        public Vector3 originalPosition;
        public Quaternion originalRotation;
        public Vector3 initialScale;
        public int crossRingIndex;
        public List<int> affectedVertexIndices = new List<int>();
        public List<Vector3> originalVertexOffsets = new List<Vector3>();
    }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (!filter) filter = gameObject.AddComponent<MeshFilter>();
        _mesh = filter.mesh;
        if (!_mesh) _mesh = new Mesh();
        ClearRibbon();
    }

    private void Update()
    {
        UpdateMeshFromControlPoints();
    }
    #endregion

    #region Ribbon Creation
    public void InsertRibbonPoint(Vector3 position, Quaternion rotation)
    {
        if (!photonView.IsMine) return;

        Quaternion finalRotation = rotation;

        if (_ignoreControllerRotation)
        {
            if (!_hasLastPosition)
                finalRotation = Quaternion.LookRotation(rotation * Vector3.forward, Vector3.up);
            else
            {
                Vector3 direction = (position - _lastRingPosition).normalized;
                finalRotation = direction.magnitude > 0.01f ? Quaternion.LookRotation(direction, Vector3.up) : _lastRingRotation;
            }
        }

        if (!_hasLastPosition)
        {
            _lastRingPosition = position;
            _lastRingRotation = finalRotation;
            _hasLastPosition = true;
            return;
        }

        float distance = Vector3.Distance(position, _lastRingPosition);
        float scaledMinDistance = _minDistance * _sizeMultiplier;

        if (distance >= scaledMinDistance)
        {
            List<Vector3> currentPoints;
            List<Vector3> currentDirections;
            int startVertexIndex = _vertices.Count;

            CreateCrossRings(position, finalRotation, out currentPoints, out currentDirections);

            if (_connectionPoints.Count == currentPoints.Count)
            {
                for (int i = 0; i < _connectionPoints.Count; i++)
                    CreateConnectionRing(_connectionPoints[i], currentPoints[i], _connectionDirections[i]);
            }

            if (photonView.IsMine)
                CreateControlPoint(position, finalRotation, _dotCount, startVertexIndex, _vertices.Count);

            _connectionPoints = currentPoints;
            _connectionDirections = currentDirections;
            _lastRingPosition = position;
            _lastRingRotation = finalRotation;
            _dotCount++;

            UpdateGeometry();
        }
    }

    public void UpdateLastRibbonPoint(Vector3 position, Quaternion rotation) { }

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

        foreach (var cp in _controlPoints)
        {
            if (cp.gameObject != null)
                Destroy(cp.gameObject);
        }
        _controlPoints.Clear();

        if (_mesh != null)
            _mesh.Clear();
    }
    #endregion

    #region Control Point Management
    private void CreateControlPoint(Vector3 position, Quaternion rotation, int index, int startVertexIndex, int endVertexIndex)
    {
        GameObject cpObject = null;
        float scaledControlPointSize = _controlPointScale * _sizeMultiplier;

        if (_controlPointPrefab != null)
            cpObject = Instantiate(_controlPointPrefab, position, rotation, transform);
        else
        {
            cpObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cpObject.transform.position = position;
            cpObject.transform.rotation = rotation;
            cpObject.transform.parent = transform;
            cpObject.GetComponent<Renderer>().material = _controlPointMat;
        }

        cpObject.name = "ControlPoint_" + index;
        cpObject.transform.localScale = Vector3.one * scaledControlPointSize;

        ControlPoint cp = new ControlPoint
        {
            gameObject = cpObject,
            originalPosition = position,
            originalRotation = rotation,
            initialScale = cpObject.transform.localScale,
            crossRingIndex = index
        };

        for (int i = startVertexIndex; i < endVertexIndex; i++)
        {
            cp.affectedVertexIndices.Add(i);
            cp.originalVertexOffsets.Add(_vertices[i] - position);
        }
        _controlPoints.Add(cp);
    }

    private void UpdateMeshFromControlPoints()
    {
        bool meshChanged = false;
        _accumulatedScales.Clear();

        for (int cpIndex = 0; cpIndex < _controlPoints.Count; cpIndex++)
        {
            var cp = _controlPoints[cpIndex];
            if (cp.gameObject == null) continue;

            Vector3 currentScale = cp.gameObject.transform.localScale;
            float scaleFactor = currentScale.x / cp.initialScale.x;

            if (Mathf.Abs(scaleFactor - 1f) > 0.001f)
            {
                for (int offset = -_adjacentInfluenceCount; offset <= _adjacentInfluenceCount; offset++)
                {
                    int targetIndex = cpIndex + offset;
                    if (targetIndex >= 0 && targetIndex < _controlPoints.Count)
                    {
                        float influence = 1f - (Mathf.Abs(offset) / (float)(_adjacentInfluenceCount + 1));
                        float adjacentScale = 1f + (scaleFactor - 1f) * influence;

                        if (!_accumulatedScales.ContainsKey(targetIndex))
                            _accumulatedScales[targetIndex] = adjacentScale;
                        else
                            _accumulatedScales[targetIndex] = Mathf.Max(_accumulatedScales[targetIndex], adjacentScale);
                    }
                }
            }
        }

        for (int cpIndex = 0; cpIndex < _controlPoints.Count; cpIndex++)
        {
            var cp = _controlPoints[cpIndex];
            if (cp.gameObject == null) continue;

            Vector3 currentPos = cp.gameObject.transform.position;
            Quaternion currentRot = cp.gameObject.transform.rotation;

            Vector3 positionDelta = currentPos - cp.originalPosition;
            Quaternion rotationDelta = currentRot * Quaternion.Inverse(cp.originalRotation);

            float finalScaleFactor = _accumulatedScales.ContainsKey(cpIndex) ? _accumulatedScales[cpIndex] : 1f;

            if (positionDelta.magnitude > 0.0001f || Quaternion.Angle(Quaternion.identity, rotationDelta) > 0.01f || Mathf.Abs(finalScaleFactor - 1f) > 0.001f)
            {
                meshChanged = true;
                for (int i = 0; i < cp.affectedVertexIndices.Count; i++)
                {
                    int vertexIndex = cp.affectedVertexIndices[i];
                    if (vertexIndex < _vertices.Count)
                    {
                        Vector3 localVertex = cp.originalVertexOffsets[i];
                        localVertex = rotationDelta * localVertex;
                        localVertex *= finalScaleFactor;
                        _vertices[vertexIndex] = cp.originalPosition + localVertex + positionDelta;
                    }
                }
            }
        }

        if (meshChanged)
            UpdateGeometry();
    }
    #endregion

    #region Mesh Primitives
    private void CreateCrossRings(Vector3 center, Quaternion rotation, out List<Vector3> connectionPoints, out List<Vector3> connectionDirections)
    {
        connectionPoints = new List<Vector3>();
        connectionDirections = new List<Vector3>();
        float scaledRingRadius = _ringRadius * _sizeMultiplier;

        Quaternion rot0 = rotation * Quaternion.Euler(0, 0, 0);
        CreateRing(center, rot0, scaledRingRadius, _ringSegments, _tubeSegments);
        connectionPoints.Add(center + rot0 * new Vector3(0, scaledRingRadius, 0));
        connectionPoints.Add(center + rot0 * new Vector3(0, -scaledRingRadius, 0));
        connectionPoints.Add(center + rot0 * new Vector3(-scaledRingRadius, 0, 0));
        connectionPoints.Add(center + rot0 * new Vector3(scaledRingRadius, 0, 0));
        connectionDirections.Add(rot0 * new Vector3(0, 1, 0));
        connectionDirections.Add(rot0 * new Vector3(0, -1, 0));
        connectionDirections.Add(rot0 * new Vector3(-1, 0, 0));
        connectionDirections.Add(rot0 * new Vector3(1, 0, 0));

        Quaternion rot1 = rotation * Quaternion.Euler(0, 0, _zigzagAngle);
        CreateRing(center, rot1, scaledRingRadius, _ringSegments, _tubeSegments);
        connectionPoints.Add(center + rot1 * new Vector3(0, scaledRingRadius, 0));
        connectionPoints.Add(center + rot1 * new Vector3(0, -scaledRingRadius, 0));
        connectionPoints.Add(center + rot1 * new Vector3(-scaledRingRadius, 0, 0));
        connectionPoints.Add(center + rot1 * new Vector3(scaledRingRadius, 0, 0));
        connectionDirections.Add(rot1 * new Vector3(0, 1, 0));
        connectionDirections.Add(rot1 * new Vector3(0, -1, 0));
        connectionDirections.Add(rot1 * new Vector3(-1, 0, 0));
        connectionDirections.Add(rot1 * new Vector3(1, 0, 0));
    }

    private void CreateRing(Vector3 center, Quaternion rotation, float ringRadius, int ringSegs, int tubeSegs)
    {
        int startVertexIndex = _vertices.Count;
        float scaledTubeRadius = _tubeRadius * _sizeMultiplier;

        for (int i = 0; i <= ringSegs; i++)
        {
            float ringAngle = 2 * Mathf.PI * i / ringSegs;
            float ringX = ringRadius * Mathf.Cos(ringAngle);
            float ringY = ringRadius * Mathf.Sin(ringAngle);
            Vector3 ringCenter = new Vector3(ringX, ringY, 0);

            for (int j = 0; j <= tubeSegs; j++)
            {
                float tubeAngle = 2 * Mathf.PI * j / tubeSegs;
                Vector3 tubeDirection = new Vector3(Mathf.Cos(ringAngle) * Mathf.Cos(tubeAngle), Mathf.Sin(ringAngle) * Mathf.Cos(tubeAngle), Mathf.Sin(tubeAngle));
                Vector3 localVertex = ringCenter + tubeDirection * scaledTubeRadius;
                _vertices.Add(center + rotation * localVertex);
                _normals.Add(rotation * tubeDirection);
            }
        }

        for (int i = 0; i < ringSegs; i++)
        {
            for (int j = 0; j < tubeSegs; j++)
            {
                int current = startVertexIndex + i * (tubeSegs + 1) + j;
                int next = current + (tubeSegs + 1);
                int currentNext = current + 1;
                int nextNext = next + 1;

                _triangles.Add(current);
                _triangles.Add(next);
                _triangles.Add(currentNext);

                _triangles.Add(currentNext);
                _triangles.Add(next);
                _triangles.Add(nextNext);
            }
        }
    }

    private void CreateConnectionRing(Vector3 start, Vector3 end, Vector3 perpendicularDirection)
    {
        Vector3 center = (start + end) / 2f;
        Vector3 connectionDirection = (end - start).normalized;
        Vector3 up = Vector3.Cross(perpendicularDirection, connectionDirection).normalized;
        Quaternion rotation = Quaternion.LookRotation(perpendicularDirection, up);
        float scaledConnectionRingRadius = _connectionRingRadius * _sizeMultiplier;
        CreateRing(center, rotation, scaledConnectionRingRadius, _connectionRingSegments, _connectionTubeSegments);
    }

    private void UpdateGeometry()
    {
        _mesh.Clear();
        _mesh.SetVertices(_vertices);
        _mesh.SetNormals(_normals);
        _mesh.SetTriangles(_triangles, 0);
        _mesh.RecalculateBounds();
    }
    #endregion
}
