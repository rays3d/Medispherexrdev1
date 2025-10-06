using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ObjLoader : MonoBehaviour
{
    public static GameObject LoadObjFromFile(string filePath)
    {
        GameObject obj = new GameObject("LoadedObj");
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("v "))
                {
                    string[] tokens = line.Split(' ');
                    Vector3 vertex = new Vector3(
                        float.Parse(tokens[1]),
                        float.Parse(tokens[2]),
                        float.Parse(tokens[3]));
                    vertices.Add(vertex);
                }
                else if (line.StartsWith("vn "))
                {
                    string[] tokens = line.Split(' ');
                    Vector3 normal = new Vector3(
                        float.Parse(tokens[1]),
                        float.Parse(tokens[2]),
                        float.Parse(tokens[3]));
                    normals.Add(normal);
                }
                else if (line.StartsWith("vt "))
                {
                    string[] tokens = line.Split(' ');
                    Vector2 texCoord = new Vector2(
                        float.Parse(tokens[1]),
                        float.Parse(tokens[2]));
                    uv.Add(texCoord);
                }
                else if (line.StartsWith("f "))
                {
                    string[] tokens = line.Split(' ');
                    foreach (string token in tokens[1..])
                    {
                        string[] indices = token.Split('/');
                        int vertexIndex = int.Parse(indices[0]) - 1;
                        int uvIndex = indices.Length > 1 && !string.IsNullOrEmpty(indices[1]) ? int.Parse(indices[1]) - 1 : -1;
                        int normalIndex = indices.Length > 2 ? int.Parse(indices[2]) - 1 : -1;

                        triangles.Add(vertexIndex);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        meshFilter.mesh = mesh;

        return obj;
    }
}
