using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportScreen : MonoBehaviour
{
    [SerializeField] Material skullMat;
    [SerializeField] Material brainMat;
    [SerializeField] Material ribCageMat;
    [SerializeField] Material Heart_i;
    [SerializeField] Material Heart_s;
    [SerializeField] Material Heart_t;
    [SerializeField] Material Heart_v;

    GameObject model;


    void DisSelectAll()
    {
        skullMat.color = Color.white;
        brainMat.color = Color.white;
        ribCageMat.color = Color.white;
        Heart_i.color = Color.white;
        Heart_s.color = Color.white;
        Heart_t.color = Color.white;
        Heart_v.color = Color.white;
        HapticManager.Instance.ActivateHapticRight(.25f, .2f);
    }

    public void OnSkullButtonPress()
    {
        DisSelectAll();
        skullMat.color = Color.blue;
      //  DestroyNotNull();

        model = PhotonNetwork.Instantiate("Models/Skull", new Vector3(0,1,0.5f), Quaternion.Euler(0f, -180f, 0f));  // change

        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());


    }

    public void OnRibCageButtonPress()
    {
        DisSelectAll();

        ribCageMat.color = Color.blue;
      //  DestroyNotNull();

        model = PhotonNetwork.Instantiate("Models/Cmf", new Vector3(0, 1, 0), Quaternion.identity);

        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnBrainButtonPress()
    {
        DisSelectAll();

        brainMat.color = Color.blue;
      //  DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Brain", new Vector3(0, 1, 0.5f), Quaternion.Euler(0f, -90f, 0f));
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());

    }
    public void OnHeartButtonPress()
    {
        DisSelectAll();

       
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart", new Vector3(0, 1, 0.5f), Quaternion.Euler(0f, -180f, 0f));
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());

    }
    public void OnHeart_1ButtonPress()
    {
        DisSelectAll();


        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart-i", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());

    }
    public void OnHeart_cardiumButtonPress()
    {
        DisSelectAll();


        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Cardium", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());

    }
    public void OnHeart_sButtonPress()
    {
        DisSelectAll();


      //  DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart-normal", new Vector3(0, 1, 0.5f), Quaternion.Euler(0f, -180f, 0f));
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());

    }
    public void OnHeart_tButtonPress()
    {
        DisSelectAll();


        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Arrow", new Vector3(0, 1, 0), Quaternion.identity);
        //SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
       

    }
    public void OnHeart_vButtonPress()
    {
        DisSelectAll();


        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart-v", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());

    }

     void DestroyNotNull()
     {
         if (model != null)
         {
             //NetworkManager.Destroy(model);
             model.GetComponent<Model>().DestroyOverNetwork();
         }
     }


}


/*using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportScreen : MonoBehaviour
{
    [SerializeField] Material skullMat;
    [SerializeField] Material brainMat;
    [SerializeField] Material ribCageMat;
    [SerializeField] Material Heart_i;
    [SerializeField] Material Heart_s;
    [SerializeField] Material Heart_t;
    [SerializeField] Material Heart_v;


    [SerializeField] AudioSource audioSource;         // Drag AudioSource here or let it auto-assign

    [SerializeField] AudioClip clickSound;

    GameObject model;




    void Start()

    {

        if (audioSource == null)

        {

            audioSource = gameObject.AddComponent<AudioSource>();

        }



        audioSource.clip = clickSound;

    }



    void PlayClickSound()

    {

        if (audioSource != null && clickSound != null)

        {

            audioSource.PlayOneShot(clickSound);

        }

    }


    void DisSelectAll()
    {
        skullMat.color = Color.white;
        brainMat.color = Color.white;
        ribCageMat.color = Color.white;
        Heart_i.color = Color.white;
        Heart_s.color = Color.white;
        Heart_t.color = Color.white;
        Heart_v.color = Color.white;
        HapticManager.Instance.ActivateHapticRight(.25f, .2f);
    }

    public void OnSkullButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        skullMat.color = Color.blue;
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Skull", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnRibCageButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        ribCageMat.color = Color.blue;
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/RibCage", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnBrainButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        brainMat.color = Color.blue;
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Brain", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnHeartButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnHeart_1ButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart-i", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnHeart_cardiumButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Cardium", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnHeart_sButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart-s", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnHeart_tButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart-t", new Vector3(0, 1, 0), Quaternion.identity);
        //SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    public void OnHeart_vButtonPress()
    {
        if (!PhotonNetwork.IsMasterClient) return;  // Only master client can proceed
        PlayClickSound();
        DisSelectAll();
        DestroyNotNull();
        model = PhotonNetwork.Instantiate("Models/Heart-v", new Vector3(0, 1, 0), Quaternion.identity);
        SelectionManager.Instance.SelectModel(model.GetComponent<ModelObject>());
    }

    void DestroyNotNull()
    {
        if (model != null)
        {
            model.GetComponent<Model>().DestroyOverNetwork();
        }
    }
}*/
