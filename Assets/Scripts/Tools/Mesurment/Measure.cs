/*using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;

public class Measure : Tool, IPunObservable, IDeletable
{
    public InputActionProperty DrawButton;

    [Header("Pen Properties")]
    [SerializeField] Transform tip;
    [Range(0.001f, 0.1f)]
    [SerializeField] float penWidth = 0.01f;

    LineRenderer currentDrawing;
    int index;
    bool isHolding;
    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        DrawButton.action.performed += (S) =>
        {
            isHolding = true;
        };

        DrawButton.action.canceled += (S) =>
        {
            isHolding = false;
        };


        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
            }
        };
    }

    [PunRPC]

    void DestroyOverNetwork()
    {
        Destroy(gameObject);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGrabbed);
        }
        else
        {
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }
    private void Update()
    {


        bool isRightHandDrawing = isGrabbed && isHolding;
        bool isLeftHandDrawing = isGrabbed && isHolding;
        if (isRightHandDrawing || isLeftHandDrawing)
        {
            Draw();
        }
        else if (currentDrawing != null)
        {
            Measurment();

            currentDrawing = null;
        }
    }
    private void Measurment()
    {
        Vector3 startPos = currentDrawing.GetPosition(0);
        Vector3 endPos = currentDrawing.GetPosition(currentDrawing.positionCount - 1);
        if (currentDrawing.GetComponent<MeasurmentInk>())
            currentDrawing.GetComponent<MeasurmentInk>().SetLineAndMesurments(startPos, endPos);

    }
    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = PhotonNetwork.Instantiate("MesurmentInk", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();

            currentDrawing.useWorldSpace = false;


            currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);

            if (currentDrawing.GetComponent<MeasurmentInk>())
                currentDrawing.GetComponent<MeasurmentInk>().SetPointOne(tip.position);

            if (SelectionManager.Instance.GetSelectedModel() != null)
                currentDrawing.transform.parent = SelectionManager.Instance.GetSelectedModel().transform;
        }
        else
        {
            var currentPos = currentDrawing.GetPosition(index);

            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                currentDrawing.positionCount = 2;
                currentDrawing.SetPosition(1, tip.position);
            }
        }
    }

}*/
////////////////////////////////////////////////////////////Above is working now//////////////////////////////////////////




// Measure.cs
// Measure.cs
/*using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;

public class Measure : Tool, IPunObservable, IDeletable
{
    public InputActionProperty DrawButton;

    [Header("Pen Properties")]
    [SerializeField] Transform tip;
    [Range(0.001f, 0.1f)]
    [SerializeField] float penWidth = 0.01f;

    private LineRenderer currentDrawing;
    private int index;
    private bool isHolding;
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        DrawButton.action.performed += (S) =>
        {
            isHolding = true;
        };

        DrawButton.action.canceled += (S) =>
        {
            isHolding = false;
        };

        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
            }
        };
    }

    [PunRPC]
    void DestroyOverNetwork()
    {
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGrabbed);
        }
        else
        {
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        bool isRightHandDrawing = isGrabbed && isHolding;
        bool isLeftHandDrawing = isGrabbed && isHolding;

        if (isRightHandDrawing || isLeftHandDrawing)
        {
            Draw();
        }
        else if (currentDrawing != null)
        {
            Measurement();
            currentDrawing = null;
        }
    }

    private void Measurement()
    {
        if (currentDrawing != null && currentDrawing.positionCount >= 2)
        {
            Vector3 startPos = currentDrawing.GetPosition(0);
            Vector3 endPos = currentDrawing.GetPosition(1);

            if (currentDrawing.GetComponent<MeasurmentInk>())
            {
                currentDrawing.GetComponent<MeasurmentInk>().SetLineAndMesurments(startPos, endPos);
            }
        }
    }

    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = PhotonNetwork.Instantiate("MesurmentInk", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
            currentDrawing.useWorldSpace = true;
            currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);

            if (currentDrawing.GetComponent<MeasurmentInk>())
                currentDrawing.GetComponent<MeasurmentInk>().SetPointOne(tip.position);

            if (SelectionManager.Instance.GetSelectedModel() != null)
                currentDrawing.transform.parent = SelectionManager.Instance.GetSelectedModel().transform;
        }
        else
        {
            var currentPos = currentDrawing.GetPosition(index);
            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                currentDrawing.positionCount = 2;
                currentDrawing.SetPosition(1, tip.position);
            }
        }
    }
}*/

/////////////////////////////////////////////////for both curve anbd straight line///////////////////////////////

/*using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;

public class Measure : Tool, IPunObservable, IDeletable
{
    public InputActionProperty DrawButton;
    [Header("Pen Properties")]
    [SerializeField] Transform tip;
    [Range(0.001f, 0.1f)]
    [SerializeField] float penWidth = 0.01f;
    LineRenderer currentDrawing;
    int index;
    bool isHolding;
    PhotonView photonView;
    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();

    [SerializeField] LayerMask surfaceLayer; // Layer mask for the 3D model surface
    private bool isStraightMode = false; // Toggle for straight vs. curved mode
    private float lastTapTime; // To detect double-tap
    private const float doubleTapThreshold = 0.3f; // Time window for double-tap (seconds)

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))

        {

            Destroy(gameObject);

            return;

        }


        DrawButton.action.performed += (S) =>
        {
            // Detect double-tap for mode switch
            if (Time.time - lastTapTime < doubleTapThreshold)
            {
                isStraightMode = !isStraightMode;
                Debug.Log("Mode switched to: " + (isStraightMode ? "Straight" : "Curved"));
            }
            lastTapTime = Time.time;

            isHolding = true;
        };
        DrawButton.action.canceled += (S) =>
        {
            isHolding = false;
            if (currentDrawing != null)
            {
                Measurment();
                currentDrawing = null;
            }
        };
        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                destroyedObjectIDs.Add(photonView.ViewID.ToString());
                //photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };
    }

    [PunRPC]
    void DestroyOverNetwork()
    {
        destroyedObjectIDs.Add(photonView.ViewID.ToString());
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGrabbed);
        }
        else
        {
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        bool isRightHandDrawing = isGrabbed && isHolding;
        bool isLeftHandDrawing = isGrabbed && isHolding;
        if (isRightHandDrawing || isLeftHandDrawing)
        {
            Draw();
        }
    }

    private Vector3 SnapToSurface(Vector3 position)
    {
        if (isStraightMode) return position; // Skip snapping for straight mode
        Ray ray = new Ray(position + Vector3.up * 10f, Vector3.down); // Cast ray downward
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, surfaceLayer))
        {
            return hit.point;
        }
        return position; // Fallback if no hit
    }

    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = PhotonNetwork.Instantiate("MesurmentInk", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
            currentDrawing.useWorldSpace = false;
            currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
            currentDrawing.positionCount = 1;
            Vector3 snappedPos = SnapToSurface(tip.position);
            currentDrawing.SetPosition(0, snappedPos);
            if (currentDrawing.GetComponent<MeasurmentInk>())
                currentDrawing.GetComponent<MeasurmentInk>().SetPointOne(snappedPos);
            if (SelectionManager.Instance.GetSelectedModel() != null)
                currentDrawing.transform.parent = SelectionManager.Instance.GetSelectedModel().transform;
        }
        else
        {
            Vector3 currentPos = currentDrawing.GetPosition(index);
            Vector3 snappedPos = SnapToSurface(tip.position);
            if (Vector3.Distance(currentPos, snappedPos) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, snappedPos);
            }
        }
    }

    private void Measurment()
    {
        if (currentDrawing != null && currentDrawing.positionCount > 1)
        {
            Vector3[] points = new Vector3[currentDrawing.positionCount];
            currentDrawing.GetPositions(points);

            if (currentDrawing.GetComponent<MeasurmentInk>())
            {
                currentDrawing.GetComponent<MeasurmentInk>().SetLineAndMesurments(points, isStraightMode);
            }
        }
    }
    private void OnApplicationQuit()

    {

        destroyedObjectIDs.Clear();

    }
}
*/

///////////////////////////////////////////delete all///////////////////////////




using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class Measure : Tool, IPunObservable, IDeletable
{
    public InputActionProperty DrawButton;
    [Header("Pen Properties")]
    [SerializeField] Transform tip;
    [Range(0.001f, 0.1f)]
    [SerializeField] float penWidth = 0.01f;

    LineRenderer currentDrawing;
    int index;
    bool isHolding;
    PhotonView photonView;

    [SerializeField] LayerMask surfaceLayer;
    private bool isStraightMode = true;
    private float lastTapTime;
    private const float doubleTapThreshold = 0.3f;

    private string modelID;
    private const string DeletedObjectsKey = "DeletedObjects";

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // Get modelID from instantiation data
        if (photonView.InstantiationData != null && photonView.InstantiationData.Length > 0)
        {
            modelID = photonView.InstantiationData[0] as string;
        }
        else
        {
            modelID = "UNKNOWN_MODEL_" + photonView.ViewID;
            Debug.LogWarning("Measure tool missing modelID in InstantiationData.");
        }
    }

    private void Start()
    {
        // Check if model was previously deleted
        if (IsModelDeleted(modelID))
        {
            Destroy(gameObject);
            return;
        }

        DrawButton.action.performed += (ctx) =>
        {
            // Handle double tap to toggle mode
            if (Time.time - lastTapTime < doubleTapThreshold)
            {
                isStraightMode = !isStraightMode;
                Debug.Log("Mode switched to: " + (isStraightMode ? "Straight" : "Curved"));
            }
            lastTapTime = Time.time;
            isHolding = true;
        };

        DrawButton.action.canceled += (ctx) =>
        {
            isHolding = false;
            if (currentDrawing != null)
            {
                FinalizeMeasurement();
                currentDrawing = null;
            }
        };

        deleteButton.action.performed += (ctx) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                MarkModelDeleted(modelID);
                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };
    }

    [PunRPC]
    void DestroyOverNetwork()
    {
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGrabbed);
        }
        else
        {
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (isGrabbed && isHolding)
        {
            Draw();
        }
    }

    private Vector3 SnapToSurface(Vector3 position)
    {
        if (isStraightMode) return position;
        Ray ray = new Ray(position + Vector3.up * 10f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, surfaceLayer))
        {
            return hit.point;
        }
        return position;
    }

    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = PhotonNetwork.Instantiate("MesurmentInk", Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
            currentDrawing.useWorldSpace = false;
            currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
            currentDrawing.positionCount = 1;
            Vector3 snappedPos = SnapToSurface(tip.position);
            currentDrawing.SetPosition(0, snappedPos);

            if (currentDrawing.TryGetComponent(out MeasurmentInk ink))
                ink.SetPointOne(snappedPos);

            if (SelectionManager.Instance.GetSelectedModel() != null)
                currentDrawing.transform.parent = SelectionManager.Instance.GetSelectedModel().transform;
        }
        else
        {
            Vector3 currentPos = currentDrawing.GetPosition(index);
            Vector3 snappedPos = SnapToSurface(tip.position);
            if (Vector3.Distance(currentPos, snappedPos) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, snappedPos);
            }
        }
    }

    private void FinalizeMeasurement()
    {
        if (currentDrawing != null && currentDrawing.positionCount > 1)
        {
            Vector3[] points = new Vector3[currentDrawing.positionCount];
            currentDrawing.GetPositions(points);

            if (currentDrawing.TryGetComponent(out MeasurmentInk ink))
            {
                ink.SetLineAndMesurments(points, isStraightMode);
            }
        }
    }

    // --- Persistent Deletion Helpers ---
    private void MarkModelDeleted(string id)
    {
        HashSet<string> deleted = GetDeletedIDs();
        if (!deleted.Contains(id))
        {
            deleted.Add(id);
            Hashtable props = new Hashtable
            {
                [DeletedObjectsKey] = SerializeIDSet(deleted)
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    private bool IsModelDeleted(string id)
    {
        return GetDeletedIDs().Contains(id);
    }

    private HashSet<string> GetDeletedIDs()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(DeletedObjectsKey, out object raw))
        {
            return DeserializeIDSet(raw as string);
        }
        return new HashSet<string>();
    }

    private string SerializeIDSet(HashSet<string> set)
    {
        return string.Join(",", set);
    }

    private HashSet<string> DeserializeIDSet(string str)
    {
        if (string.IsNullOrEmpty(str)) return new HashSet<string>();
        return new HashSet<string>(str.Split(','));
    }

    private void OnApplicationQuit()
    {
        // Nothing needed here due to network-based persistence
    }
}





