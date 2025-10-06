using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeIndicator : Indicator
{

    [SerializeField] Material colorIndicatorMat;
    GameObject item;
    public override void hide()
    {
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        item = XRRightHandController.Instance.GetGrabbedItem();

        colorIndicatorMat.color = item.GetComponent<IColorChangable>().GetCurrentColor();

    }

    private void Update()
    {
        if(item != null)
        {
            colorIndicatorMat.color = item.GetComponent<IColorChangable>().GetCurrentColor();
        }

    }

}
