using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PenWidthController : MonoBehaviour
{
    public Slider widthSlider;

    void Start()
    {
        widthSlider.onValueChanged.AddListener(UpdatePenWidth);

        // Set the slider value to match the current global width
        widthSlider.value = Mathf.InverseLerp(0.001f, 0.1f, Pen.globalPenWidth);
    }

    public void UpdatePenWidth(float value)
    {
        float newWidth = Mathf.Lerp(0.001f, 0.1f, value);

        // Update the global pen width locally
        Pen.globalPenWidth = newWidth;

        // Sync width across all players in multiplayer
        PhotonView photonView = FindObjectOfType<Pen>()?.GetComponent<PhotonView>();
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC(nameof(Pen.SyncPenWidth), RpcTarget.AllBuffered, newWidth);
        }

        Debug.Log("Global Pen Width Set To: " + Pen.globalPenWidth);
    }
}
