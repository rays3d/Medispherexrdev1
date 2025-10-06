using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{
    public List<Material> mats => SelectionManager.Instance.GetMaterialOfObject();
    void Update()
    {
        if (mats.Count <= 0) return;
        Plane plane = new Plane(transform.up, transform.position);
        Vector4 planeRepresentation = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
        foreach (Material mat in mats)
        {
            mat.SetVector("_Plane", planeRepresentation);

        }
    }
    public void SetColor(Color color)
    {
        foreach (Material mat in mats)
        {
            mat.SetColor("_CrossSectionColor", color);
        }
    }

    public void Reset()
    {
        Vector4 planeRepresentation = new Vector4(0, 0, 0, 10000);
        foreach (Material mat in mats)
        {
            mat.SetVector("_Plane", planeRepresentation);
        }
    }

    private void OnDestroy()
    {
        Reset();
    }
}



/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{
    // Property to get materials from SelectionManager
    private List<Material> mats
    {
        get
        {
            if (SelectionManager.Instance == null)
            {
                Debug.LogWarning("SelectionManager.Instance is null.");
                return new List<Material>();
            }
            return SelectionManager.Instance.GetMaterialOfObject();
        }
    }

    void Update()
    {
        List<Material> materials = mats;
        if (materials.Count <= 0) return;

        Plane plane = new Plane(transform.up, transform.position);
        Vector4 planeRepresentation = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

        foreach (Material mat in materials)
        {
            if (mat != null)
            {
                if (mat.HasProperty("_Plane"))
                {
                    mat.SetVector("_Plane", planeRepresentation);
                }
                else
                {
                    Debug.LogWarning("Material does not have _Plane property.");
                }
            }
            else
            {
                Debug.LogWarning("Material is null.");
            }
        }
    }

    public void SetColor(Color color)
    {
        List<Material> materials = mats;
        foreach (Material mat in materials)
        {
            if (mat != null)
            {
                if (mat.HasProperty("_CrossSectionColor"))
                {
                    mat.SetColor("_CrossSectionColor", color);
                }
                else
                {
                    Debug.LogWarning("Material does not have _CrossSectionColor property.");
                }
            }
            else
            {
                Debug.LogWarning("Material is null.");
            }
        }
    }

    public void Reset()
    {
        Vector4 planeRepresentation = new Vector4(0, 0, 0,0);
        List<Material> materials = mats;
        foreach (Material mat in materials)
        {
            if (mat != null)
            {
                if (mat.HasProperty("_Plane"))
                {
                    mat.SetVector("_Plane", planeRepresentation);
                }
                else
                {
                    Debug.LogWarning("Material does not have _Plane property.");
                }
            }
            else
            {
                Debug.LogWarning("Material is null.");
            }
        }
    }

    private void OnDestroy()
    {
        Reset();
    }
}

*/


/*using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{
    private SelectionManager selectionManager;
    private static readonly Vector4 ResetPlane = new Vector4(0, 0, 0, 10000);

    private List<Material> mats => selectionManager?.GetMaterialOfObject();

    void Start()
    {
        // Cache the SelectionManager instance.
        selectionManager = SelectionManager.Instance;
    }

    void Update()
    {
        // Early exit if mats is null or empty.
        if (mats == null || mats.Count == 0) return;

        Plane plane = new Plane(transform.up, transform.position);
        Vector4 planeRepresentation = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

        foreach (Material mat in mats)
        {
            mat.SetVector("_Plane", planeRepresentation);
        }
    }

    public void SetColor(Color color)
    {
        if (mats == null || mats.Count == 0) return;

        foreach (Material mat in mats)
        {
            mat.SetColor("_CrossSectionColor", color);
        }
    }

    public void Reset()
    {
        if (mats == null || mats.Count == 0) return;

        foreach (Material mat in mats)
        {
            mat.SetVector("_Plane", ResetPlane);
        }
    }

    private void OnDestroy()
    {
        // Call Reset to ensure cleanup when the object is destroyed.
        Reset();
    }
}
*/
