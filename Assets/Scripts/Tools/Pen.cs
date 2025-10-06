/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using System;
using UnityEngine.PlayerLoop;
using System.Reflection;

public class Pen : Tool, IPunObservable, IDeletable, IColorChangable
{
    public InputActionProperty DrawButton;
    public InputActionProperty changeColor;
    [SerializeField] GameObject ink;

    [Header("Pen Properties")]
    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.001f, 0.1f)]
    public float penWidth = 0.001f;
    public Color[] penColors;


    LineRenderer currentDrawing;
    int index;
    int currentColorIndex;
    bool isHolding;
    PhotonView photonView;
    bool isOverRiding;

    [SerializeField] LayerMask layerMask;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];
        DrawButton.action.performed += (S) =>
        {
            isHolding = true;
        };

        DrawButton.action.canceled += (S) =>
        {
            isHolding = false;
        };

        changeColor.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);

                Debug.Log("Change color");
            }
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

    public void SetLineRenderer(LineRenderer line)
    {
        currentDrawing = line;
    }
    private void Update()
    {

        bool isDrawing = isGrabbed && isHolding;


        if (isDrawing)
        {
            // Draw();
            photonView.RPC(nameof(Draw), RpcTarget.All);
        }
        else if (currentDrawing != null)
        {
            photonView.RPC(nameof(AddCollider), RpcTarget.All);
            currentDrawing = null;
        }
    }


    [PunRPC]
    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = Instantiate(ink).GetComponent<LineRenderer>();
            currentDrawing.useWorldSpace = false;
            if (SelectionManager.Instance.GetSelectedModel())
                currentDrawing.transform.parent = SelectionManager.Instance.GetSelectedModel().transform;
            currentDrawing.material = drawingMaterial;
            currentDrawing.startColor = penColors[currentColorIndex];
            currentDrawing.endColor = penColors[currentColorIndex];
            currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);
        }
        else
        {

            currentDrawing.positionCount = index + 1;
            var currentPos = currentDrawing.GetPosition(index);

            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, tip.position);

            }
        }
    }

    [PunRPC]
    void AddCollider()
    {
        if (currentDrawing == null) return;
        currentDrawing.gameObject.AddComponent<Deletable>();
        currentDrawing.gameObject.layer = layerMask;
        currentDrawing.gameObject.AddComponent<BoxCollider>().isTrigger = true;
        currentDrawing.gameObject.GetComponent<BoxCollider>().center = currentDrawing.bounds.center;
        currentDrawing.gameObject.GetComponent<BoxCollider>().size = currentDrawing.bounds.size;
    }

    [PunRPC]
    private void SwitchColor()
    {
        if (currentColorIndex == penColors.Length - 1)
        {
            currentColorIndex = 0;
        }
        else
        {
            currentColorIndex++;
        }
        tipMaterial.color = penColors[currentColorIndex];
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isHolding);
            stream.SendNext(isGrabbed);
        }
        else
        {
            isHolding = (bool)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }

    public Color GetCurrentColor()
    {
        return penColors[currentColorIndex];
    }
    public void OnChangeColor()
    {

    }

}*/

///////////////////////////////////////down is working using recent
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using System;

public class Pen : Tool, IPunObservable, IDeletable, IColorChangable
{
    public static float globalPenWidth = 0.002f; // Default width for all pens

    public InputActionProperty DrawButton;
    public InputActionProperty changeColor;
    [SerializeField] GameObject ink;

    [Header("Pen Properties")]
    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.001f, 0.1f)]
    public float minPenWidth = 0.002f;
    public float maxPenWidth = 0.1f;
    public Color[] penColors;

    private LineRenderer currentDrawing;
    private int index;
    private int currentColorIndex;
    private bool isHolding;
    private PhotonView photonView;
    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();
    [SerializeField] LayerMask layerMask;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))

        {

            Destroy(gameObject);

            return;

        }
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];

        DrawButton.action.performed += (S) => { isHolding = true; };
        DrawButton.action.canceled += (S) => { isHolding = false; };

        changeColor.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };

        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                destroyedObjectIDs.Add(photonView.ViewID.ToString());
                // photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.All);
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

    private void Update()
    {
        bool isDrawing = isGrabbed && isHolding;

        if (isDrawing)
        {
            photonView.RPC(nameof(Draw), RpcTarget.All);
        }
        else if (currentDrawing != null)
        {
            photonView.RPC(nameof(AddCollider), RpcTarget.All);
            currentDrawing = null;
        }
    }

    [PunRPC]
    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = Instantiate(ink).GetComponent<LineRenderer>();
            currentDrawing.useWorldSpace = false;

            if (SelectionManager.Instance.GetSelectedModel())
                currentDrawing.transform.parent = SelectionManager.Instance.GetSelectedModel().transform;

            currentDrawing.material = drawingMaterial;
            currentDrawing.startColor = penColors[currentColorIndex];
            currentDrawing.endColor = penColors[currentColorIndex];

            // Apply global pen width (synced across all players)
            currentDrawing.startWidth = globalPenWidth;
            currentDrawing.endWidth = globalPenWidth;

            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);
        }
        else
        {
            currentDrawing.startWidth = globalPenWidth;
            currentDrawing.endWidth = globalPenWidth;

            currentDrawing.positionCount = index + 1;
            var currentPos = currentDrawing.GetPosition(index);

            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, tip.position);
            }
        }
    }

    [PunRPC]
    void AddCollider()
    {
        if (currentDrawing == null) return;

        currentDrawing.gameObject.AddComponent<Deletable>();
        currentDrawing.gameObject.layer = layerMask;
        currentDrawing.gameObject.AddComponent<BoxCollider>().isTrigger = true;
        currentDrawing.gameObject.GetComponent<BoxCollider>().center = currentDrawing.bounds.center;
        currentDrawing.gameObject.GetComponent<BoxCollider>().size = currentDrawing.bounds.size;
    }

    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        tipMaterial.color = penColors[currentColorIndex];
    }

    [PunRPC]
    public void SyncPenWidth(float newWidth)
    {
        globalPenWidth = newWidth;
    }

    public void AdjustPenWidth(float value)
    {
        float newWidth = Mathf.Lerp(minPenWidth, maxPenWidth, value);
        photonView.RPC(nameof(SyncPenWidth), RpcTarget.AllBuffered, newWidth);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isHolding);
            stream.SendNext(isGrabbed);
            stream.SendNext(globalPenWidth);
        }
        else
        {
            isHolding = (bool)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();
            globalPenWidth = (float)stream.ReceiveNext();
        }
    }

    public Color GetCurrentColor()
    {
        return penColors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        // Implementation if needed
    }

    private void OnApplicationQuit()

    {

        destroyedObjectIDs.Clear();

    }
}*/




////////////////////////////////////above is correct


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class Pen : Tool, IPunObservable, IDeletable, IColorChangable
{
    public static float globalPenWidth = 0.002f;

    public InputActionProperty DrawButton;
    public InputActionProperty changeColor;
    [SerializeField] GameObject ink;

    [Header("Pen Properties")]
    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.001f, 0.1f)]
    public float minPenWidth = 0.002f;
    public float maxPenWidth = 0.1f;
    public Color[] penColors;

    private LineRenderer currentDrawing;
    private int index;
    private int currentColorIndex;
    private bool isHolding;
    private PhotonView photonView;

    private static HashSet<string> destroyedObjectIDs = new HashSet<string>();
    [SerializeField] LayerMask layerMask;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        // ? Check if this object was already destroyed in previous session
        if (destroyedObjectIDs.Contains(photonView.ViewID.ToString()))
        {
            Destroy(gameObject);
            return;
        }

        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];

        DrawButton.action.performed += (S) => { isHolding = true; };
        DrawButton.action.canceled += (S) => { isHolding = false; };

        changeColor.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };

        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                DeleteTool();
                destroyedObjectIDs.Add(photonView.ViewID.ToString());
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

    private void Update()
    {
        bool isDrawing = isGrabbed && isHolding;

        if (isDrawing)
        {
            photonView.RPC(nameof(Draw), RpcTarget.All);
        }
        else if (currentDrawing != null)
        {
            photonView.RPC(nameof(AddCollider), RpcTarget.All);
            currentDrawing = null;
        }
    }

    [PunRPC]
    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = Instantiate(ink).GetComponent<LineRenderer>();
            currentDrawing.useWorldSpace = false;

            if (SelectionManager.Instance.GetSelectedModel())
                currentDrawing.transform.parent = SelectionManager.Instance.GetSelectedModel().transform;

            currentDrawing.material = drawingMaterial;
            currentDrawing.startColor = penColors[currentColorIndex];
            currentDrawing.endColor = penColors[currentColorIndex];
            currentDrawing.startWidth = globalPenWidth;
            currentDrawing.endWidth = globalPenWidth;
            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);
        }
        else
        {
            currentDrawing.startWidth = globalPenWidth;
            currentDrawing.endWidth = globalPenWidth;
            currentDrawing.positionCount = index + 1;

            var currentPos = currentDrawing.GetPosition(index);

            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, tip.position);
            }
        }
    }

    [PunRPC]
    void AddCollider()
    {
        if (currentDrawing == null) return;

        currentDrawing.gameObject.AddComponent<Deletable>();
        currentDrawing.gameObject.layer = layerMask;
        var collider = currentDrawing.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = currentDrawing.bounds.center;
        collider.size = currentDrawing.bounds.size;
    }

    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        tipMaterial.color = penColors[currentColorIndex];
    }

    [PunRPC]
    public void SyncPenWidth(float newWidth)
    {
        globalPenWidth = newWidth;
    }

    public void AdjustPenWidth(float value)
    {
        float newWidth = Mathf.Lerp(minPenWidth, maxPenWidth, value);
        photonView.RPC(nameof(SyncPenWidth), RpcTarget.AllBuffered, newWidth);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isHolding);
            stream.SendNext(isGrabbed);
            stream.SendNext(globalPenWidth);
        }
        else
        {
            isHolding = (bool)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();
            globalPenWidth = (float)stream.ReceiveNext();
        }
    }

    public Color GetCurrentColor()
    {
        return penColors[currentColorIndex];
    }

    public void OnChangeColor()
    {
        // Optional
    }

    private void OnApplicationQuit()
    {
        destroyedObjectIDs.Clear();
    }
}
*/


///overall delete/////////////





/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Pen : Tool, IPunObservable, IDeletable, IColorChangable
{
    public static float globalPenWidth = 0.002f;

    public InputActionProperty DrawButton;
    public InputActionProperty changeColor;
    [SerializeField] GameObject ink;

    [Header("Pen Properties")]
    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.001f, 0.1f)]
    public float minPenWidth = 0.002f;
    public float maxPenWidth = 0.1f;
    public Color[] penColors;

    private LineRenderer currentDrawing;
    private int index;
    private int currentColorIndex;
    private bool isHolding;
    private PhotonView photonView;
    private string modelID;

    [SerializeField] LayerMask layerMask;

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
            Debug.LogWarning("Pen missing modelID in InstantiationData.");
            modelID = "UNKNOWN_MODEL_ID_" + photonView.ViewID;
        }
    }

    private void Start()
    {
        if (IsModelDeleted(modelID))
        {
            Destroy(gameObject);
            return;
        }

        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];

        DrawButton.action.performed += (S) => { isHolding = true; };
        DrawButton.action.canceled += (S) => { isHolding = false; };

        changeColor.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };

        deleteButton.action.performed += (S) =>
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

    private void Update()
    {
        if (isGrabbed && isHolding)
        {
            photonView.RPC(nameof(Draw), RpcTarget.All);
        }
        else if (currentDrawing != null)
        {
            photonView.RPC(nameof(AddCollider), RpcTarget.All);
            currentDrawing = null;
        }
    }

    [PunRPC]
    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = Instantiate(ink).GetComponent<LineRenderer>();
            currentDrawing.useWorldSpace = false;

            if (SelectionManager.Instance.GetSelectedModel())
                currentDrawing.transform.parent = SelectionManager.Instance.GetSelectedModel().transform;

            currentDrawing.material = drawingMaterial;
            currentDrawing.startColor = penColors[currentColorIndex];
            currentDrawing.endColor = penColors[currentColorIndex];
            currentDrawing.startWidth = globalPenWidth;
            currentDrawing.endWidth = globalPenWidth;
            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);
        }
        else
        {
            currentDrawing.startWidth = globalPenWidth;
            currentDrawing.endWidth = globalPenWidth;

            currentDrawing.positionCount = index + 1;
            Vector3 currentPos = currentDrawing.GetPosition(index);

            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, tip.position);
            }
        }
    }

    [PunRPC]
    void AddCollider()
    {
        if (currentDrawing == null) return;

        currentDrawing.gameObject.AddComponent<Deletable>();
        currentDrawing.gameObject.layer = layerMask;
        var collider = currentDrawing.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = currentDrawing.bounds.center;
        collider.size = currentDrawing.bounds.size;
    }

    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        tipMaterial.color = penColors[currentColorIndex];
    }

    [PunRPC]
    public void SyncPenWidth(float newWidth)
    {
        globalPenWidth = newWidth;
    }

    public void AdjustPenWidth(float value)
    {
        float newWidth = Mathf.Lerp(minPenWidth, maxPenWidth, value);
        photonView.RPC(nameof(SyncPenWidth), RpcTarget.AllBuffered, newWidth);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isHolding);
            stream.SendNext(isGrabbed);
            stream.SendNext(globalPenWidth);
        }
        else
        {
            isHolding = (bool)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();
            globalPenWidth = (float)stream.ReceiveNext();
        }
    }

    public Color GetCurrentColor() => penColors[currentColorIndex];
    public void OnChangeColor() { }

    // --- Persistent Deletion Management ---
    private void MarkModelDeleted(string id)
    {
        HashSet<string> deleted = GetDeletedIDs();
        if (!deleted.Contains(id))
        {
            deleted.Add(id);
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                [DeletedObjectsKey] = string.Join(",", deleted)
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
            string rawStr = raw as string;
            if (!string.IsNullOrEmpty(rawStr))
                return new HashSet<string>(rawStr.Split(','));
        }
        return new HashSet<string>();
    }

    private void OnApplicationQuit() { }//* nothing needed, handled by room properties *//* }
}

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using System.Linq;

[System.Serializable]
public class DrawingData
{
    public Vector3[] points;
    public int colorIndex;
    public float width;
    public string parentModelName;
    public string drawingId;
    public bool useWorldSpace; // Add this to track coordinate space
    public Vector3 parentPosition; // Store parent position for reference
    public Quaternion parentRotation; // Store parent rotation for reference
}

public class Pen : Tool, IPunObservable, IDeletable, IColorChangable
{
    public static float globalPenWidth = 0.002f;

    public InputActionProperty DrawButton;
    public InputActionProperty changeColor;
    [SerializeField] GameObject ink;

    [Header("Pen Properties")]
    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.001f, 0.1f)]
    public float minPenWidth = 0.002f;
    public float maxPenWidth = 0.1f;
    public Color[] penColors;

    private LineRenderer currentDrawing;
    private int index;
    private int currentColorIndex;
    private bool isHolding;
    private PhotonView photonView;
    private string modelID;
    private string currentDrawingId;
    private Transform currentParentModel; // Track current parent for the drawing being created

    [SerializeField] LayerMask layerMask;

    private const string DeletedObjectsKey = "DeletedObjects";
    private const string DrawingsKey = "PersistentDrawings";
    private const string DeletedDrawingsKey = "DeletedDrawings";

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
            Debug.LogWarning("Pen missing modelID in InstantiationData.");
            modelID = "UNKNOWN_MODEL_ID_" + photonView.ViewID;
        }
    }

    private void Start()
    {
        if (IsModelDeleted(modelID))
        {
            Destroy(gameObject);
            return;
        }

        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];

        DrawButton.action.performed += (S) => { isHolding = true; };
        DrawButton.action.canceled += (S) => { isHolding = false; };

        changeColor.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                photonView.RPC(nameof(SwitchColor), RpcTarget.AllBuffered);
            }
        };

        deleteButton.action.performed += (S) =>
        {
            if (isGrabbed)
            {
                // Get the model name before deletion for marking associated drawings
                string modelName = gameObject.name;

                DeleteTool();
                MarkModelDeleted(modelID);

                // Mark all drawings attached to this model as deleted
                MarkDrawingsOfDeletedModel(modelName);

                photonView.RPC(nameof(DestroyOverNetwork), RpcTarget.AllBuffered);
            }
        };

        // Load existing drawings for late joiners
        StartCoroutine(LoadExistingDrawings());
    }

    private IEnumerator LoadExistingDrawings()
    {
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(DrawingsKey))
        {
            string drawingsData = PhotonNetwork.CurrentRoom.CustomProperties[DrawingsKey] as string;
            if (!string.IsNullOrEmpty(drawingsData))
            {
                LoadDrawingsFromData(drawingsData);
            }
        }
    }

    [PunRPC]
    void DestroyOverNetwork()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (isGrabbed && isHolding)
        {
            // Only the person holding the pen calls the Draw RPC
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(Draw), RpcTarget.All);
            }
        }
        else if (currentDrawing != null)
        {
            // Only the person who was drawing finalizes it
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(AddCollider), RpcTarget.All);
                SaveCurrentDrawing();
            }
            currentDrawing = null;
            currentDrawingId = null;
            currentParentModel = null;
        }
    }

    [PunRPC]
    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawingId = photonView.ViewID + "_" + System.DateTime.Now.Ticks;

            currentDrawing = Instantiate(ink).GetComponent<LineRenderer>();
            currentDrawing.name = "Drawing_" + currentDrawingId;

            // Set parent and coordinate space
            currentParentModel = SelectionManager.Instance.GetSelectedModel()?.transform;
            if (currentParentModel != null)
            {
                currentDrawing.transform.SetParent(currentParentModel, false);
                currentDrawing.useWorldSpace = false; // Use local space when parented
            }
            else
            {
                currentDrawing.useWorldSpace = true; // Use world space when not parented
            }

            currentDrawing.material = drawingMaterial;
            currentDrawing.startColor = penColors[currentColorIndex];
            currentDrawing.endColor = penColors[currentColorIndex];
            currentDrawing.startWidth = globalPenWidth;
            currentDrawing.endWidth = globalPenWidth;
            currentDrawing.positionCount = 1;

            // Set position based on coordinate space
            Vector3 drawPosition = currentParentModel != null ?
                currentParentModel.InverseTransformPoint(tip.position) : tip.position;
            currentDrawing.SetPosition(0, drawPosition);
        }
        else
        {
            currentDrawing.startWidth = globalPenWidth;
            currentDrawing.endWidth = globalPenWidth;

            currentDrawing.positionCount = index + 1;
            Vector3 currentPos = currentDrawing.GetPosition(index);

            // Calculate new position based on coordinate space
            Vector3 newPosition = currentParentModel != null ?
                currentParentModel.InverseTransformPoint(tip.position) : tip.position;

            if (Vector3.Distance(currentPos, newPosition) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, newPosition);
            }
        }
    }
    /*
        [PunRPC]
        void AddCollider()
        {
            if (currentDrawing == null) return;

            // Add Deletable component with callback for when drawing is deleted
            var deletable = currentDrawing.gameObject.AddComponent<Deletable>();

            // Store the drawing ID in the deletable component so we can track it
            var drawingTracker = currentDrawing.gameObject.AddComponent<DrawingTracker>();
            drawingTracker.drawingId = currentDrawingId;

            currentDrawing.gameObject.layer = layerMask;
            var collider = currentDrawing.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = currentDrawing.bounds.center;
            collider.size = currentDrawing.bounds.size;
        }
    */


    [PunRPC]
    void AddCollider()
    {
        if (currentDrawing == null) return;

        // Add Deletable component with callback for when drawing is deleted
        var deletable = currentDrawing.gameObject.AddComponent<Deletable>();

        // Store the drawing ID in the deletable component so we can track it
        var drawingTracker = currentDrawing.gameObject.AddComponent<DrawingTracker>();
        drawingTracker.drawingId = currentDrawingId;

        currentDrawing.gameObject.layer = layerMask;
        var collider = currentDrawing.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        // Fix for parented drawings - calculate bounds properly
        if (currentDrawing.useWorldSpace)
        {
            collider.center = currentDrawing.bounds.center;
            collider.size = currentDrawing.bounds.size;
        }
        else
        {
            // For local space drawings, calculate bounds manually
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            for (int i = 0; i < currentDrawing.positionCount; i++)
            {
                Vector3 point = currentDrawing.GetPosition(i);
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;

            // Ensure minimum size for small drawings
            size.x = Mathf.Max(size.x, 0.01f);
            size.y = Mathf.Max(size.y, 0.01f);
            size.z = Mathf.Max(size.z, 0.01f);

            collider.center = center;
            collider.size = size;
        }
    }

    private void SaveCurrentDrawing()
    {
        if (currentDrawing == null || !photonView.IsMine) return;

        // Create drawing data
        DrawingData data = new DrawingData
        {
            drawingId = currentDrawingId,
            points = new Vector3[currentDrawing.positionCount],
            colorIndex = currentColorIndex,
            width = globalPenWidth,
            parentModelName = currentParentModel?.name ?? "",
            useWorldSpace = currentDrawing.useWorldSpace
        };

        // Store parent transform info for reference
        if (currentParentModel != null)
        {
            data.parentPosition = currentParentModel.position;
            data.parentRotation = currentParentModel.rotation;
        }

        // Copy all points (they're already in the correct coordinate space)
        for (int i = 0; i < currentDrawing.positionCount; i++)
        {
            data.points[i] = currentDrawing.GetPosition(i);
        }

        // Save to room properties for late joiners
        SaveDrawingToRoomProperties(data);
    }

    private void SaveDrawingToRoomProperties(DrawingData data)
    {
        // Get existing drawings
        List<DrawingData> allDrawings = GetAllDrawingsFromRoom();

        // Add new drawing
        allDrawings.Add(data);

        // Keep only last 50 drawings to avoid room property limits
        if (allDrawings.Count > 50)
        {
            allDrawings.RemoveRange(0, allDrawings.Count - 50);
        }

        // Serialize and save
        string serializedData = SerializeDrawings(allDrawings);

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            [DrawingsKey] = serializedData
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private List<DrawingData> GetAllDrawingsFromRoom()
    {
        List<DrawingData> drawings = new List<DrawingData>();

        if (PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(DrawingsKey))
        {
            string data = PhotonNetwork.CurrentRoom.CustomProperties[DrawingsKey] as string;
            if (!string.IsNullOrEmpty(data))
            {
                drawings = DeserializeDrawings(data);
            }
        }

        return drawings;
    }

    private void LoadDrawingsFromData(string data)
    {
        List<DrawingData> drawings = DeserializeDrawings(data);
        HashSet<string> deletedDrawings = GetDeletedDrawingIds();
        HashSet<string> deletedModels = GetDeletedIDs(); // Get deleted models

        foreach (DrawingData drawing in drawings)
        {
            // Check if drawing is deleted OR if its parent model is deleted
            bool isDrawingDeleted = deletedDrawings.Contains(drawing.drawingId);
            bool isParentModelDeleted = !string.IsNullOrEmpty(drawing.parentModelName) &&
                                       IsModelDeletedByName(drawing.parentModelName, deletedModels);

            // Only load drawings that haven't been deleted and whose parent models haven't been deleted
            if (!isDrawingDeleted && !isParentModelDeleted)
            {
                CreatePersistedDrawing(drawing);
            }
        }
    }

    private void CreatePersistedDrawing(DrawingData data)
    {
        GameObject drawingObj = Instantiate(ink);
        LineRenderer lineRenderer = drawingObj.GetComponent<LineRenderer>();
        lineRenderer.name = "PersistedDrawing_" + data.drawingId;

        // Set coordinate space first
        lineRenderer.useWorldSpace = data.useWorldSpace;

        // Find parent model if it exists
        Transform parentTransform = null;
        if (!string.IsNullOrEmpty(data.parentModelName))
        {
            GameObject parentModel = GameObject.Find(data.parentModelName);
            if (parentModel != null)
            {
                parentTransform = parentModel.transform;
                lineRenderer.transform.SetParent(parentTransform, false);
            }
        }

        // If we couldn't find the parent but the drawing was supposed to be parented,
        // we need to convert from local to world coordinates using stored parent transform
        bool needsCoordinateConversion = !string.IsNullOrEmpty(data.parentModelName) &&
                                        parentTransform == null &&
                                        !data.useWorldSpace;

        lineRenderer.material = drawingMaterial;
        lineRenderer.startColor = penColors[data.colorIndex];
        lineRenderer.endColor = penColors[data.colorIndex];
        lineRenderer.startWidth = data.width;
        lineRenderer.endWidth = data.width;
        lineRenderer.positionCount = data.points.Length;

        for (int i = 0; i < data.points.Length; i++)
        {
            Vector3 pointToSet = data.points[i];

            // If we need coordinate conversion (parent is missing), convert local to world
            if (needsCoordinateConversion)
            {
                // Convert from stored local coordinates to world coordinates
                Matrix4x4 parentMatrix = Matrix4x4.TRS(data.parentPosition, data.parentRotation, Vector3.one);
                pointToSet = parentMatrix.MultiplyPoint3x4(data.points[i]);
                lineRenderer.useWorldSpace = true; // Switch to world space since parent is missing
            }

            lineRenderer.SetPosition(i, pointToSet);
        }

        // Add collider and tracking
        AddColliderToPersistedDrawing(lineRenderer, data.drawingId);
    }

    private void AddColliderToPersistedDrawing(LineRenderer drawing, string drawingId)
    {
        if (drawing == null) return;

        drawing.gameObject.AddComponent<Deletable>();

        // Add tracking component
        var drawingTracker = drawing.gameObject.AddComponent<DrawingTracker>();
        drawingTracker.drawingId = drawingId;

        drawing.gameObject.layer = layerMask;
        var collider = drawing.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        if (drawing.bounds.size.magnitude > 0)
        {
            collider.center = drawing.bounds.center;
            collider.size = drawing.bounds.size;
        }
        else
        {
            collider.size = Vector3.one * 0.1f;
        }
    }

    // Method to mark a drawing as deleted (called when a drawing is erased)
    public static void MarkDrawingAsDeleted(string drawingId)
    {
        if (PhotonNetwork.CurrentRoom == null || string.IsNullOrEmpty(drawingId)) return;

        HashSet<string> deletedDrawings = GetDeletedDrawingIds();
        if (!deletedDrawings.Contains(drawingId))
        {
            deletedDrawings.Add(drawingId);

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                [DeletedDrawingsKey] = string.Join(",", deletedDrawings)
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    private static HashSet<string> GetDeletedDrawingIds()
    {
        if (PhotonNetwork.CurrentRoom?.CustomProperties.TryGetValue(DeletedDrawingsKey, out object raw) == true)
        {
            string rawStr = raw as string;
            if (!string.IsNullOrEmpty(rawStr))
                return new HashSet<string>(rawStr.Split(','));
        }
        return new HashSet<string>();
    }

    // Simple serialization methods
    private string SerializeDrawings(List<DrawingData> drawings)
    {
        try
        {
            return JsonUtility.ToJson(new DrawingList { drawings = drawings.ToArray() });
        }
        catch
        {
            return "";
        }
    }

    private List<DrawingData> DeserializeDrawings(string data)
    {
        try
        {
            DrawingList list = JsonUtility.FromJson<DrawingList>(data);
            return new List<DrawingData>(list.drawings);
        }
        catch
        {
            return new List<DrawingData>();
        }
    }

    [System.Serializable]
    private class DrawingList
    {
        public DrawingData[] drawings;
    }

    [PunRPC]
    private void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        tipMaterial.color = penColors[currentColorIndex];
    }

    [PunRPC]
    public void SyncPenWidth(float newWidth)
    {
        globalPenWidth = newWidth;
    }

    public void AdjustPenWidth(float value)
    {
        float newWidth = Mathf.Lerp(minPenWidth, maxPenWidth, value);
        photonView.RPC(nameof(SyncPenWidth), RpcTarget.AllBuffered, newWidth);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isHolding);
            stream.SendNext(isGrabbed);
            stream.SendNext(globalPenWidth);
        }
        else
        {
            isHolding = (bool)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();
            globalPenWidth = (float)stream.ReceiveNext();
        }
    }

    public Color GetCurrentColor() => penColors[currentColorIndex];
    public void OnChangeColor() { }

    // --- Persistent Deletion Management ---
    private bool IsModelDeletedByName(string modelName, HashSet<string> deletedModelIds)
    {
        // This checks if any deleted model ID corresponds to this model name
        // You might need to adjust this logic based on how your model IDs relate to model names
        foreach (string deletedId in deletedModelIds)
        {
            // If the deleted ID contains the model name or vice versa
            if (deletedId.Contains(modelName) || modelName.Contains(deletedId))
            {
                return true;
            }
        }
        return false;
    }

    // Method to automatically mark drawings as deleted when their parent model is deleted
    public static void MarkDrawingsOfDeletedModel(string modelName)
    {
        if (PhotonNetwork.CurrentRoom == null || string.IsNullOrEmpty(modelName)) return;

        // Get all existing drawings
        List<DrawingData> allDrawings = new List<DrawingData>();
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(DrawingsKey))
        {
            string data = PhotonNetwork.CurrentRoom.CustomProperties[DrawingsKey] as string;
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    DrawingList list = JsonUtility.FromJson<DrawingList>(data);
                    allDrawings = new List<DrawingData>(list.drawings);
                }
                catch { }
            }
        }

        // Find drawings attached to the deleted model and mark them as deleted
        HashSet<string> deletedDrawings = GetDeletedDrawingIds();
        bool foundDrawingsToDelete = false;

        foreach (DrawingData drawing in allDrawings)
        {
            if (drawing.parentModelName == modelName && !deletedDrawings.Contains(drawing.drawingId))
            {
                deletedDrawings.Add(drawing.drawingId);
                foundDrawingsToDelete = true;
            }
        }

        // Update room properties if we found drawings to delete
        if (foundDrawingsToDelete)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                [DeletedDrawingsKey] = string.Join(",", deletedDrawings)
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
    private void MarkModelDeleted(string id)
    {
        HashSet<string> deleted = GetDeletedIDs();
        if (!deleted.Contains(id))
        {
            deleted.Add(id);
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                [DeletedObjectsKey] = string.Join(",", deleted)
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
        if (PhotonNetwork.CurrentRoom?.CustomProperties.TryGetValue(DeletedObjectsKey, out object raw) == true)
        {
            string rawStr = raw as string;
            if (!string.IsNullOrEmpty(rawStr))
                return new HashSet<string>(rawStr.Split(','));
        }
        return new HashSet<string>();
    }






    private void OnApplicationQuit() { }
}