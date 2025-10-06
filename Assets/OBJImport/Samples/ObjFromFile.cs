using Dummiesman;
using System.IO;
using UnityEngine;

public class ObjFromFile : MonoBehaviour
{
    string objPath = string.Empty;
    string error = string.Empty;
    GameObject loadedObject;

    void OnGUI() {
        objPath = GUI.TextField(new Rect(0, 0, 256, 32), objPath);

        GUI.Label(new Rect(0, 0, 256, 32), "Obj Path:");
        if(GUI.Button(new Rect(256, 32, 64, 32), "Load File"))
        {
            //file path
            if (!File.Exists(objPath))
            {
                error = "File doesn't exist.";
            }else{
                if(loadedObject != null)            
                    Destroy(loadedObject);
                loadedObject = new OBJLoader().Load(objPath);
                error = string.Empty;
            }
        }

        if(!string.IsNullOrWhiteSpace(error))
        {
            GUI.color = Color.red;
            GUI.Box(new Rect(0, 64, 256 + 64, 32), error);
            GUI.color = Color.white;
        }
    }
}

/*using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;

public class ObjFromFile : MonoBehaviour
{
    string objPath = string.Empty;
    string error = string.Empty;
    GameObject loadedObject;

    void OnGUI()
    {
        objPath = GUI.TextField(new Rect(0, 0, 256, 32), objPath);

        GUI.Label(new Rect(0, 0, 256, 32), "Obj Path:");
        if (GUI.Button(new Rect(256, 32, 64, 32), "Load File"))
        {
            // File path
            if (!File.Exists(objPath))
            {
                error = "File doesn't exist.";
            }
            else
            {
                if (loadedObject != null)
                {
                    Destroy(loadedObject);
                }

                try
                {
                    // Create a FileStream from the OBJ file
                    using (FileStream fileStream = new FileStream(objPath, FileMode.Open, FileAccess.Read))
                    {
                        // Ensure the OBJLoader is properly instantiated and used
                        OBJLoader loader = new OBJLoader();

                        // Load the model using the OBJLoader from the stream
                        GameObject model = loader.Load(fileStream, Vector3.zero, Quaternion.identity);

                        if (model != null)
                        {
                            // Optionally adjust the position or settings of the model
                            model.transform.position = Vector3.zero; // Adjust as needed

                            // Set the loaded model to the loadedObject reference
                            loadedObject = model;
                            error = string.Empty;
                        }
                        else
                        {
                            error = "Failed to load the model from the OBJ file.";
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    error = "Error loading OBJ file: " + ex.Message;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            GUI.color = Color.red;
            GUI.Box(new Rect(0, 64, 256 + 64, 32), error);
            GUI.color = Color.white;
        }
    }
}
*/