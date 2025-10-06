using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteItemIndicator : Indicator
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
