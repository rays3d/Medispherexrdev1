using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateIndicator : Indicator
{
    public override void hide()
    {
       gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);

    }
}
