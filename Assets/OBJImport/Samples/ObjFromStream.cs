using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;

public class ObjFromStream : MonoBehaviour {
	void Start () {
        //make www
        var www = new WWW("https://people.sc.fsu.edu/~jburkardt/data/obj/lamp.obj");
        while (!www.isDone)
            System.Threading.Thread.Sleep(1);
        
        //create stream and load
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.text));
        var loadedObj = new OBJLoader().Load(textStream);
	}
}

/*using Dummiesman;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ObjFromStream : MonoBehaviour
{
    public string objUrl = "https://people.sc.fsu.edu/~jburkardt/data/obj/lamp.obj"; // URL of the OBJ file
    public OBJLoader objLoader; // Reference to the OBJLoader component

    private void Start()
    {
        if (objLoader == null)
        {
            Debug.LogError("OBJLoader component is not assigned.");
            return;
        }

        StartCoroutine(DownloadAndLoadOBJ(objUrl));
    }

    private IEnumerator DownloadAndLoadOBJ(string url)
    {
        // Start downloading the OBJ file
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Convert the downloaded data to a MemoryStream
                byte[] data = webRequest.downloadHandler.data;
                using (var memoryStream = new MemoryStream(data))
                {
                    // Load the OBJ file using OBJLoader
                    GameObject loadedObj = objLoader.Load(memoryStream, Vector3.zero, Quaternion.identity);

                    if (loadedObj != null)
                    {
                        Debug.Log("Model loaded successfully.");
                    }
                    else
                    {
                        Debug.LogError("Failed to load the model.");
                    }
                }
            }
            else
            {
                Debug.LogError("Error downloading the OBJ model: " + webRequest.error);
            }
        }
    }
}
*/