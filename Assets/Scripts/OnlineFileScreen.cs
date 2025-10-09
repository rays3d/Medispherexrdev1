using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineFileScreen : MonoBehaviour
{
    [SerializeField] OnlineFileButton fileButton;
    [SerializeField] Transform content;
    public void InitializeScreen()
    {
        DeleteAllChild();
        gameObject.SetActive(true);
     // foreach (Data modelData in FirebaseManager.Instance.modelDatabase)
        {
            OnlineFileButton button = Instantiate(fileButton, content);
        //    button.SetData(modelData);
        }
    }

    void DeleteAllChild()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}
