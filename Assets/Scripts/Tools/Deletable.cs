/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deletable : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Eraser"))
        {
            if (!other.gameObject.GetComponent<Eraser>().isGrabbed) return;
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Eraser"))
        {
            if (!other.gameObject.GetComponent<Eraser>().isGrabbed) return;
            Destroy(gameObject);
        }
    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deletable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Eraser"))
        {
            if (!other.gameObject.GetComponent<Eraser>().isGrabbed) return;
            DeleteObject();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Eraser"))
        {
            if (!other.gameObject.GetComponent<Eraser>().isGrabbed) return;
            DeleteObject();
        }
    }

    private void DeleteObject()
    {
        // Check if this is a measurement object
        MeasurmentInk measurementInk = GetComponent<MeasurmentInk>();
        if (measurementInk != null)
        {
            // Use permanent deletion for measurements
            measurementInk.DeleteMeasurement();
        }
        else
        {
            // Default deletion behavior for other objects
            Destroy(gameObject);
        }
    }

   
}






