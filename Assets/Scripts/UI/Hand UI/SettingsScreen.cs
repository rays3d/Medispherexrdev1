using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] Slider lightIntensitySlider;
    [SerializeField] Slider shadowStrenghtSlider;

    private void Start()
    {
        lightIntensitySlider.onValueChanged.AddListener(OnLIghtIntensityChange);
        shadowStrenghtSlider.onValueChanged.AddListener(OnShadowValueChange);
    }

    private void OnShadowValueChange(float _value)
    {
       // EnvironmentManager.Instance.SetShadowStrength(_value);
    }

    private void OnLIghtIntensityChange(float _value)
    {
      //  EnvironmentManager.Instance.SetLightIntensity(_value);

    }

    public void SetSkyboxofindex(int _value)
    {
       // EnvironmentManager.Instance.SetSkybox(_value);
    }
}
