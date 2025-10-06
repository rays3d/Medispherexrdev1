using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IColorChangable
{
    Color GetCurrentColor();

    void OnChangeColor();
}
