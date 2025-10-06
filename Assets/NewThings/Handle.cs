/*using UnityEngine;

public class Handle : MonoBehaviour
{
    public int vertexIndex;
    public BaffleMeshGenerator meshRef;

    void Update()
    {
        if (meshRef == null) return;

        // Convert world space to local space
        Vector3 localPosition = meshRef.transform.InverseTransformPoint(transform.position);
        localPosition.y = 0; // lock to horizontal plane
        meshRef.UpdateVertex(vertexIndex, localPosition);
    }
}
*/
/*using UnityEngine;

public class Handle : MonoBehaviour
{
    public int vertexIndex;
    public MeshDrawer meshRef;

    void Update()
    {
        if (meshRef == null) return;

        Vector3 localPos = meshRef.transform.InverseTransformPoint(transform.position);
       // localPos.y = 0;
         meshRef.UpdateVertex(vertexIndex, localPos);
        
    }
}
*/



using UnityEngine;

public class Handle : MonoBehaviour
{
    public int vertexIndex;
    public MeshDrawer meshRef;

    void Update()
    {
        if (meshRef == null) return;

        Vector3 localPos = meshRef.transform.InverseTransformPoint(transform.position);
       // localPos.y = 0;
        meshRef.UpdateVertex(vertexIndex, localPos);

    }
}
