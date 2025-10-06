using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class XRCompactableManager : MonoBehaviourSingleton<XRCompactableManager>
{
    public void MakeModelXRCompactable(GameObject _model)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        foreach (var renderer in _model.GetComponentsInChildren<Renderer>())
        {
            Collider col = renderer.gameObject.AddComponent<BoxCollider>();
            renderer.transform.position = Vector3.zero;
            Vector3 pos = renderer.bounds.center;

            GameObject modelPart = PhotonNetwork.Instantiate("ModelPart", pos, Quaternion.identity);

            renderer.transform.parent = modelPart.transform;
            modelPart.transform.position = Vector3.zero;

            XRGrabNetworkInteractable xrGrab = modelPart.AddComponent<XRGrabNetworkInteractable>();
            xrGrab.colliders.Add(col);
            xrGrab.useDynamicAttach = true;

            renderer.material = new Material(Shader.Find("Shader Graphs/CrossSectionShader"));
            Vector4 planeRepresentation = new Vector4(0, 0, 0, 10000);
            foreach (Material mat in renderer.materials)
            {
                mat.SetVector("_Plane", planeRepresentation);
                mat.SetColor("_CrossSectionColor", Color.white);
            }
        }
    }

    public void MakeModelXRCompactable(GameObject _model,GameObject modelPart)
    {
        foreach (var renderer in _model.GetComponentsInChildren<Renderer>())
        {
            Collider col = renderer.gameObject.AddComponent<BoxCollider>();
            renderer.transform.position = Vector3.zero;
            Vector3 pos = renderer.bounds.center;

            renderer.transform.parent = modelPart.transform;
            modelPart.transform.position = Vector3.zero;

            XRGrabNetworkInteractable xrGrab = modelPart.AddComponent<XRGrabNetworkInteractable>();
            xrGrab.colliders.Add(col);
            xrGrab.useDynamicAttach = true;

            renderer.material = new Material(Shader.Find("Shader Graphs/CrossSectionShader"));
            Vector4 planeRepresentation = new Vector4(0, 0, 0, 10000);
            foreach (Material mat in renderer.materials)
            {
                mat.SetVector("_Plane", planeRepresentation);
                mat.SetColor("_CrossSectionColor", Color.white);
            }
        }
    }
}
