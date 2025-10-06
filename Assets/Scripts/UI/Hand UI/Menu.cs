using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField] GameObject selectionVisual;

    [Header("Colors")]
    [SerializeField] Color normalColor;
    [SerializeField] Color selectionColor;

    [Header("3D Icons")]
    [SerializeField] GameObject multiplayerIcon;
    [SerializeField] GameObject importIcon;
    [SerializeField] GameObject server_importIcon;
    [SerializeField] GameObject toolsIcon;
    [SerializeField] GameObject settingsIcon;
    [SerializeField] GameObject micIcon;
    [SerializeField] GameObject homeIcon;

    [Header("Screens")]
    [SerializeField] GameObject roomDetailsScreen;
    [SerializeField] GameObject importModelScreen;
    [SerializeField] GameObject importModelScreen1;
    [SerializeField] GameObject importModelserver;
    [SerializeField] GameObject toolsScreen;
    [SerializeField] GameObject settingsScreen;
    [SerializeField] GameObject textlibrary;
    // [SerializeField] OnlineFileScreen onlineImportScreen;
    [SerializeField] GameObject[] tools;
    [SerializeField] GameObject[] samples;


    Renderer multiplayerIconRenderer;
    Renderer importIconRenderer;
    Renderer server_importIconRenderer;
    Renderer toolsIconRenderer;
    Renderer settingsIconRenderer;
    Renderer micIconRenderer;
    Renderer homeIconRenderer;


    private void Start()
    {
        multiplayerIconRenderer = multiplayerIcon.GetComponent<Renderer>();
        importIconRenderer = importIcon.GetComponent<Renderer>();
        server_importIconRenderer = server_importIcon.GetComponent<Renderer>();
        toolsIconRenderer = toolsIcon.GetComponent<Renderer>();
        settingsIconRenderer = settingsIcon.GetComponent<Renderer>();
        micIconRenderer = micIcon.GetComponent<Renderer>();
        homeIconRenderer = homeIcon.GetComponent<Renderer>();

    }

    void SetAllScreensOff()
    {
        roomDetailsScreen.SetActive(false);
        importModelScreen.SetActive(false);
        importModelScreen1.SetActive(false);
        importModelserver.SetActive(false);
        toolsScreen.SetActive(false);
        settingsScreen.SetActive(false);
        textlibrary.SetActive(false);
        selectionVisual.SetActive(false);

        foreach (var tool in tools)
        {
            tool.gameObject.SetActive(false);
        }
        foreach (var sample in samples)
        {
            sample.gameObject.SetActive(false);
        }

        multiplayerIconRenderer.material.color = normalColor;
        importIconRenderer.material.color = normalColor;
        server_importIconRenderer.material.color = normalColor;
        toolsIconRenderer.material.color = normalColor;
        settingsIconRenderer.material.color = normalColor;
        micIconRenderer.material.color = normalColor;
        homeIconRenderer.material.color = normalColor;

        HapticManager.Instance.ActivateHapticRight(.25f, .2f);

    }

    public void OnRoomDetailsButtonPress()
    {
        SetAllScreensOff();
        roomDetailsScreen.SetActive(true);

        selectionVisual.transform.position = multiplayerIcon.transform.position;
        selectionVisual.SetActive(true);
        multiplayerIconRenderer.material.color = selectionColor;


    }
    public void OnSettingsButtonPress()
    {
        SetAllScreensOff();
        settingsScreen.SetActive(true);

        selectionVisual.transform.position = settingsIcon.transform.position;
        selectionVisual.SetActive(true);
        settingsIconRenderer.material.color = selectionColor;

    }

    public void OnImportModelsButtonPress()
    {
        SetAllScreensOff();
        importModelScreen.SetActive(true);

        selectionVisual.transform.position = importIcon.transform.position;
        selectionVisual.SetActive(true);
        importIconRenderer.material.color = selectionColor;

        foreach (var sample in samples)
        {
            sample.gameObject.SetActive(true);
        }
       // onlineImportScreen.InitializeScreen();
    }
    public void OnImportModelsserverButtonPress()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the master client can access this feature.");
            return; // Prevent non-master clients from accessing the server import model
        }
        SetAllScreensOff();
        importModelserver.SetActive(true);

        selectionVisual.transform.position = server_importIcon.transform.position;
        selectionVisual.SetActive(true);
        server_importIconRenderer.material.color = selectionColor;

        /*foreach (var sample in samples)
        {
            sample.gameObject.SetActive(true);
        }*/
        // onlineImportScreen.InitializeScreen();
    }

    public void OnToolsButtonPress()
    {
        SetAllScreensOff();
        toolsScreen.SetActive(true);

        selectionVisual.transform.position = toolsIcon.transform.position;
        selectionVisual.SetActive(true);
        toolsIconRenderer.material.color = selectionColor;


        foreach (var tool in tools)
        {
            tool.gameObject.SetActive(true);           
        }
    }


    public void OnMicButtonPress()
    {
        // mute and Un mute
        SetAllScreensOff();
        micIconRenderer.material.color = selectionColor;
    }

    public void OnHomeButtonPress()
    {
        SetAllScreensOff();
        textlibrary.SetActive(true);
        selectionVisual.transform.position = homeIcon.transform.position;
        selectionVisual.SetActive(true);
        homeIconRenderer.material.color = selectionColor;
    }
}
