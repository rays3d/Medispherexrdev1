using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviourSingleton<EnvironmentManager>
{
    [SerializeField] Light mainLight;

    [SerializeField] Material[] materials;


    public void SetSkybox(int index)
    {
        RenderSettings.skybox = materials[index];
    }
    public float GetLightIntensity()
    {
        return mainLight.intensity;
    }

    public float GetShadowStrength()
    {
        return mainLight.shadowStrength;
    }

    public void SetShadowStrength(float _value)
    {
        mainLight.shadowStrength = _value;
    }

    public void SetLightIntensity(float _value)
    {
        mainLight.intensity = _value;
    }
}
