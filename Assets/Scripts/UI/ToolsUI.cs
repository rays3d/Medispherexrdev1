/*using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsUI : MonoBehaviour
{
    [SerializeField] GameObject penModel;
    [SerializeField] GameObject measureModel;
    [SerializeField] GameObject dusterModel;

    [SerializeField] GameObject slicerModel;


    Renderer penModelRenderer;
    Renderer measureModelRenderer;
    Renderer dusterModelRenderer;
    Renderer slicerModelRenderer;

    XRRigMapper mapper;

    private void Start()
    {
        penModelRenderer = penModel.GetComponentInChildren<Renderer>();
        measureModelRenderer = measureModel.GetComponentInChildren<Renderer>();
        dusterModelRenderer = dusterModel.GetComponentInChildren<Renderer>();
        slicerModelRenderer = slicerModel.GetComponentInChildren<Renderer>();

        mapper = FindObjectOfType<XRRigMapper>();
    }


    void DisSelectAll()
    {
        foreach (var item in penModelRenderer.materials)
        {
            item.color = Color.white;
        }

        foreach (var item in measureModelRenderer.materials)
        {
            item.color = Color.white;
        }

        foreach (var item in dusterModelRenderer.materials)
        {
            item.color = Color.white;
        }

        foreach (var item in slicerModelRenderer.materials)
        {
            item.color = Color.white;
        }
        HapticManager.Instance.ActivateHapticRight(.25f, .2f);

    }


    public void OnPenButtonPress()
    {
        DisSelectAll();
        foreach (var item in penModelRenderer.materials)
        {
            item.color = Color.blue;
        }

        PhotonNetwork.Instantiate("Tools/Pen", mapper.rightHandTarget.position, Quaternion.identity);
    }

    public void OnMeasureButtonPress()
    {
        DisSelectAll();
        foreach (var item in measureModelRenderer.materials)
        {
            item.color = Color.blue;
        }
        PhotonNetwork.Instantiate("Tools/Measure", mapper.rightHandTarget.position, Quaternion.identity);

    }

    public void OnDusterButtonPress()
    {
        DisSelectAll();
        foreach (var item in dusterModelRenderer.materials)
        {
            item.color = Color.blue;
        }
        PhotonNetwork.Instantiate("Tools/Duster", mapper.rightHandTarget.position, Quaternion.identity);

    }

    public void OnSliceButtonPress()
    {
        DisSelectAll();
        foreach (var item in slicerModelRenderer.materials)
        {
            item.color = Color.blue;
        }
        PhotonNetwork.Instantiate("Tools/Slice", mapper.rightHandTarget.position, Quaternion.identity);
    }
}
*/

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsUI : MonoBehaviour
{
    [SerializeField] GameObject penModel;
    [SerializeField] GameObject measureModel;
    [SerializeField] GameObject dusterModel;
    [SerializeField] GameObject slicerModel;

    Renderer penModelRenderer;
    Renderer measureModelRenderer;
    Renderer dusterModelRenderer;
    Renderer slicerModelRenderer;

    XRRigMapper mapper;

    private void Start()
    {
        penModelRenderer = penModel.GetComponentInChildren<Renderer>();
        measureModelRenderer = measureModel.GetComponentInChildren<Renderer>();
        dusterModelRenderer = dusterModel.GetComponentInChildren<Renderer>();
        slicerModelRenderer = slicerModel.GetComponentInChildren<Renderer>();

        mapper = FindObjectOfType<XRRigMapper>();
    }

    void DisSelectAll()
    {
        foreach (var item in penModelRenderer.materials)
        {
            item.color = Color.white;
        }

        foreach (var item in measureModelRenderer.materials)
        {
            item.color = Color.white;
        }

        foreach (var item in dusterModelRenderer.materials)
        {
            item.color = Color.white;
        }

        foreach (var item in slicerModelRenderer.materials)
        {
            item.color = Color.white;
        }

        HapticManager.Instance.ActivateHapticRight(.25f, .2f);
    }

    public void OnPenButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can instantiate tools.");
            return;
        }

        DisSelectAll();
        foreach (var item in penModelRenderer.materials)
        {
            item.color = Color.blue;
        }

        PhotonNetwork.Instantiate("Tools/Pen", mapper.rightHandTarget.position, Quaternion.identity);
    }

    public void OnMeasureButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can instantiate tools.");
            return;
        }

        DisSelectAll();
        foreach (var item in measureModelRenderer.materials)
        {
            item.color = Color.blue;
        }

        PhotonNetwork.Instantiate("Tools/Measure", mapper.rightHandTarget.position, Quaternion.identity);
    }

    public void OnDusterButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can instantiate tools.");
            return;
        }

        DisSelectAll();
        foreach (var item in dusterModelRenderer.materials)
        {
            item.color = Color.blue;
        }

        PhotonNetwork.Instantiate("Tools/Duster", mapper.rightHandTarget.position, Quaternion.identity);
    }

    public void OnSliceButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can instantiate tools.");
            return;
        }

        DisSelectAll();
        foreach (var item in slicerModelRenderer.materials)
        {
            item.color = Color.blue;
        }

        PhotonNetwork.Instantiate("Tools/Slice", mapper.rightHandTarget.position, Quaternion.identity);
    }
}
