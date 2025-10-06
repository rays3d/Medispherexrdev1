/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
{
    [SerializeField] ModelObject model;
    public event Action OnSelectedObject;

    public bool isGrabbing;
    public void SelectModel(ModelObject _model)
    {
        model = _model;
        OnSelectedObject?.Invoke();

        if (_model != null)
        {
            XRRightHandController.Instance?.SetRotateIndicatorActive();
            XRRightHandController.Instance?.SetResetIndicatorActive();
        }

    }

    public ModelObject GetSelectedModel()
    {
        return model;
    }


    public virtual List<Material> GetMaterialOfObject()
    {
        return model.GetMaterial();

    }
}
*/ /// thiss is normal
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
{
    [SerializeField] private ModelObject model;
    public event Action OnSelectedObject;

    public bool isGrabbing;

    public void SelectModel(ModelObject _model)
    {
        if (_model == null)
        {
            Debug.LogWarning("Attempted to select a null model.");
            return;
        }

        model = _model;
        OnSelectedObject?.Invoke();

        if (XRRightHandController.Instance != null)
        {
            XRRightHandController.Instance.SetRotateIndicatorActive();
            XRRightHandController.Instance.SetResetIndicatorActive();
        }
        else
        {
            Debug.LogWarning("XRRightHandController.Instance is null.");
        }
    }

    public ModelObject GetSelectedModel()
    {
        return model;
    }

    public virtual List<Material> GetMaterialOfObject()
    {
        if (model == null)
        {
            Debug.LogWarning("Model is null. Cannot get materials.");
            return new List<Material>();
        }
        return model.GetMaterial();
    }
}*/

/*using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
{
    [SerializeField] private ModelObject model;
    public event Action OnSelectedObject;

    public bool isGrabbing;

    // Method to select a model
    public void SelectModel(ModelObject _model)
    {
        if (_model == null)
        {
            Debug.LogWarning("Attempted to select a null model.");
            return;
        }

        model = _model; // Set the model as the current selection
        OnSelectedObject?.Invoke(); // Notify listeners that a model has been selected

        if (XRRightHandController.Instance != null)
        {
            XRRightHandController.Instance.SetRotateIndicatorActive();
            XRRightHandController.Instance.SetResetIndicatorActive();
        }
        else
        {
            Debug.LogWarning("XRRightHandController.Instance is null.");
        }
    }

    // Method to get the currently selected model
    public ModelObject GetSelectedModel()
    {
        return model;
    }

    // Get materials from the selected model
    public virtual List<Material> GetMaterialOfObject()
    {
        if (model == null)
        {
            Debug.LogWarning("Model is null. Cannot get materials.");
            return new List<Material>();
        }
        return model.GetMaterial();
    }
}
*/

/*using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
{
    [SerializeField] private ModelObject model;
    public event Action OnSelectedObject;

    public bool isGrabbing;

    public void SelectModel(ModelObject _model)
    {
        if (_model == null)
        {
            Debug.LogWarning("Attempted to select a null model.");
            return;
        }

        // Log which model is being selected
        Debug.Log("Selecting model: " + _model.gameObject.name);

        // Check if the same model is already selected
        if (model != _model)
        {
            model = _model;
            OnSelectedObject?.Invoke();

            // Log the selected model update
            Debug.Log("Model assigned in SelectionManager: " + model.gameObject.name);

            if (XRRightHandController.Instance != null)
            {
                XRRightHandController.Instance.SetRotateIndicatorActive();
                XRRightHandController.Instance.SetResetIndicatorActive();
            }
        }
        else
        {
            Debug.Log("The selected model is already assigned.");
        }
    }

    public ModelObject GetSelectedModel()
    {
        return model;
    }

    public virtual List<Material> GetMaterialOfObject()
    {
        if (model == null)
        {
            Debug.LogWarning("Model is null. Cannot get materials.");
            return new List<Material>();
        }
        return model.GetMaterial();
    }
}
*/
/// i  am using ...........................................................................................................................................................
/*using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
{
    [SerializeField] private ModelObject model;
    public event Action OnSelectedObject;

    public bool isGrabbing;

    public void SelectModel(ModelObject _model)
    {
        if (_model == null)
        {
            Debug.LogWarning("Attempted to select a null model.");
            return;
        }

        Debug.Log("Selecting model: " + _model.gameObject.name);

        if (model != _model)
        {
            model = _model;
            OnSelectedObject?.Invoke();
            Debug.Log("Model assigned in SelectionManager: " + model.gameObject.name);

            if (XRRightHandController.Instance != null)
            {
                XRRightHandController.Instance.SetRotateIndicatorActive();
                XRRightHandController.Instance.SetResetIndicatorActive();
            }
        }
        else
        {
            Debug.Log("The selected model is already assigned.");
        }
    }

    public ModelObject GetSelectedModel()
    {
        return model;
    }

    public virtual List<Material> GetMaterialOfObject()
    {
        if (model == null)
        {
            Debug.LogWarning("Model is null. Cannot get materials.");
            return new List<Material>();
        }
        return model.GetMaterial();
    }




    public GameObject GetModelByID(string modelID)

    {

        if (string.IsNullOrEmpty(modelID))

        {

            Debug.LogWarning("Model ID is null or empty.");

            return null;

        }



        // Find all PhotonView components in the scene

        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        foreach (var view in photonViews)

        {

            if (view.InstantiationData != null && view.InstantiationData.Length > 0)

            {

                if (view.InstantiationData[0] is string photonModelID && photonModelID == modelID)

                {

                    return view.gameObject;

                }

            }

        }



        Debug.LogWarning($"No model found with ID: {modelID}");

        return null;

    }
}

*/


//////////////this is neww//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
{
    [SerializeField] private ModelObject model;
    public event Action OnSelectedObject;
    public event Action<string> OnModelDeleted;

    public bool isGrabbing;

    /*    public void SelectModel(ModelObject _model)
        {
            if (_model == null)
            {
                Debug.LogWarning("Attempted to select a null model.");
                ClearSelection();
                return;
            }

            Debug.Log("Selecting model: " + _model.gameObject.name);

            if (model != _model)
            {
                model = _model;
                OnSelectedObject?.Invoke();
                Debug.Log("Model assigned in SelectionManager: " + model.gameObject.name);

                if (XRRightHandController.Instance != null)
                {
                    XRRightHandController.Instance.SetRotateIndicatorActive();
                    XRRightHandController.Instance.SetResetIndicatorActive();
                }
            }
            else
            {
                Debug.Log("The selected model is already assigned.");
            }
        }*/

    public void SelectModel(ModelObject _model)
    {
        if (_model == null)
        {
            Debug.LogWarning("Attempted to select a null model.");
            ClearSelection();
            return;
        }

        Debug.Log("Selecting model: " + _model.gameObject.name);

        // Always update the model reference and invoke the event
        // This ensures that even re-selecting the same model triggers the proper updates
        model = _model;
        OnSelectedObject?.Invoke();
        Debug.Log("Model assigned in SelectionManager: " + model.gameObject.name);

/*        var resetPosition = FindObjectOfType<ResetPosition>();
        if (resetPosition != null)
        {
            resetPosition.UpdateOriginalTransform(_model);
        }*/


        if (XRRightHandController.Instance != null)
        {
            XRRightHandController.Instance.SetRotateIndicatorActive();
            XRRightHandController.Instance.SetResetIndicatorActive();
        }
    }
    public void DeleteModel(ModelObject _model)
    {
        if (_model == null)
        {
            Debug.LogWarning("Attempted to delete a null model.");
            return;
        }

        string modelID = GetModelID(_model);
        if (string.IsNullOrEmpty(modelID))
        {
            Debug.LogWarning("Could not retrieve model ID for deletion.");
            return;
        }

        OnModelDeleted?.Invoke(modelID);

        if (model == _model)
        {
            ClearSelection();
        }

        if (_model.GetComponent<PhotonView>() != null && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(_model.gameObject);
        }
        else
        {
            Destroy(_model.gameObject);
        }

        Debug.Log($"Model deleted: {_model.gameObject.name} (ID: {modelID})");
    }

    public void ClearSelection()
    {
        if (model != null)
        {
            model = null;
            OnSelectedObject?.Invoke();
            Debug.Log("Selection cleared in SelectionManager.");
        }
    }

    public ModelObject GetSelectedModel()
    {
        if (model != null && model.gameObject != null && model.gameObject.activeInHierarchy)
        {
            return model;
        }
        if (model != null)
        {
            Debug.LogWarning("Selected model is invalid, clearing selection.");
            ClearSelection();
        }
        return null;
    }

    public virtual List<Material> GetMaterialOfObject()
    {
        if (model == null)
        {
            Debug.LogWarning("Model is null. Cannot get materials.");
            return new List<Material>();
        }
        return model.GetMaterial();
    }

       public GameObject GetModelByID(string modelID)
        {
            if (string.IsNullOrEmpty(modelID))
            {
                Debug.LogWarning("Model ID is null or empty.");
                return null;
            }

            PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
            foreach (var view in photonViews)
            {
                if (view.InstantiationData != null && view.InstantiationData.Length > 0)
                {
                    if (view.InstantiationData[0] is string photonModelID && photonModelID == modelID)
                    {
                        return view.gameObject;
                    }
                }
            }

            Debug.LogWarning($"No model found with ID: {modelID}");
            return null;
        }

   


    private string GetModelID(ModelObject _model)
    {
        if (_model == null) return null;

        PhotonView pv = _model.GetComponent<PhotonView>();
        if (pv != null && pv.InstantiationData != null && pv.InstantiationData.Length > 0)
        {
            return pv.InstantiationData[0] as string;
        }
        return _model.name + "_" + _model.GetInstanceID();
    }
}














//////////////////////////down is original from old

/*using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
{
    [SerializeField] private ModelObject model;
    public event Action OnSelectedObject;

    public bool isGrabbing;

    public void SelectModel(ModelObject _model)
    {
        if (_model == null)
        {
            Debug.LogWarning("Attempted to select a null model.");
            return;
        }

        // Log which model is being selected
        Debug.Log("Selecting model: " + _model.gameObject.name);

        // Check if the same model is already selected
        if (model != _model)
        {
            model = _model;
            OnSelectedObject?.Invoke();

            // Log the selected model update
            Debug.Log("Model assigned in SelectionManager: " + model.gameObject.name);

            if (XRRightHandController.Instance != null)
            {
                XRRightHandController.Instance.SetRotateIndicatorActive();
                XRRightHandController.Instance.SetResetIndicatorActive();
            }
        }
        else
        {
            Debug.Log("The selected model is already assigned.");
        }
    }

    public ModelObject GetSelectedModel()
    {
        return model;
    }

    public virtual List<Material> GetMaterialOfObject()
    {
        if (model == null)
        {
            Debug.LogWarning("Model is null. Cannot get materials.");
            return new List<Material>();
        }
        return model.GetMaterial();
    }
}


*/








/*using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
{
    [SerializeField] private ModelObject model;
    public event Action OnSelectedObject;
    public bool isGrabbing;

    public void SelectModel(ModelObject _model)
    {
        // Ensure only the master client can select models
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can select models.");
            return;
        }

        if (_model == null)
        {
            Debug.LogWarning("Attempted to select a null model.");
            return;
        }

        Debug.Log("Selecting model: " + _model.gameObject.name);

        if (model != _model)
        {
            model = _model;
            OnSelectedObject?.Invoke();
            Debug.Log("Model assigned in SelectionManager: " + model.gameObject.name);

            if (XRRightHandController.Instance != null)
            {
                XRRightHandController.Instance.SetRotateIndicatorActive();
                XRRightHandController.Instance.SetResetIndicatorActive();
            }
        }
        else
        {
            Debug.Log("The selected model is already assigned.");
        }
    }

    public ModelObject GetSelectedModel()
    {
        return model;
    }

    public virtual List<Material> GetMaterialOfObject()
    {
        if (model == null)
        {
            Debug.LogWarning("Model is null. Cannot get materials.");
            return new List<Material>();
        }
        return model.GetMaterial();
    }
}
*/