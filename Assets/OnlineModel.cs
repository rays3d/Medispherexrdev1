using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineModel : MonoBehaviour
{
    bool modelLoaded = false;
    void Update()
    {
        if (transform.childCount == 0)
        {
            if (LoadModelFromURL.Instance.loadedObject != null && !modelLoaded)
            {
                LoadModelFromURL.Instance.MakeItXR(this.gameObject);
                modelLoaded = true;


            }
        }
    }
}
