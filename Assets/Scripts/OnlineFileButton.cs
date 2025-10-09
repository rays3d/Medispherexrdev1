using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OnlineFileButton : MonoBehaviour
{
    [SerializeField] TMP_Text buttonText;
    [SerializeField] Data modelData;
    [SerializeField] Image fillImage;
    Button UIbutton;
    public void SetData(Data _data)
    {
        modelData = _data;
        buttonText.text = _data.fileName;
    }

    private void Start()
    {
        UIbutton = GetComponent<Button>();

        UIbutton.onClick.AddListener(() =>
        {
            UIbutton.interactable = false;

            LoadModelFromURL.Instance.LoadModel(modelData.downloadURL, (x, progress) =>
            {
                fillImage.fillAmount = progress;

                if (progress >= 1)
                {
                    fillImage.fillAmount = 0;
                    UIbutton.interactable = true;

                }
            });
            NetworkManager.instance.GetNetworkPlayer().LoadModelInNetwork(modelData.downloadURL);

        });
    }
}


