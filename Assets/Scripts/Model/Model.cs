/*using Photon.Pun;
using System.Collections.Generic;

public class Model : ModelObject
{
    public List<ModelPart> parts = new List<ModelPart>();


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        SelectionManager.Instance.SelectModel(this);
    }
    public void ResetAllParts()
    {
        foreach (ModelPart part in parts)
        {
            photonView.RequestOwnership();
            part.Reset();
        }
    }
}
*/
using Photon.Pun;
using System.Collections.Generic;

public class Model : ModelObject
{
    public List<ModelPart> parts = new List<ModelPart>();

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        SelectionManager.Instance.SelectModel(this);
    }

    public void ResetAllParts()
    {
        foreach (ModelPart part in parts)
        {
            // Only the master client should reset parts (optional)
            if (photonView.IsMine)
            {
                photonView.RequestOwnership();
                part.Reset();
            }
        }
    }
    [PunRPC]
    public void DestroyInAll()
    {
        Destroy(gameObject);
    }
}


