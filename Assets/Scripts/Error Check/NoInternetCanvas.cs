using UnityEngine.SceneManagement;
using UnityEngine;

public class NoInternetCanvas : MonoBehaviour
{
    public void OnExitButtonPress()
    {
        Application.Quit();
    }

    public void OnRetryButtonPress()
    {
        SceneManager.LoadScene(0);
    }
}
