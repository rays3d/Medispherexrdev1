
using UnityEngine;

public class TravelWithCamera : MonoBehaviour
{
    Transform m_Camera => Camera.main.transform;
    [SerializeField] float distance = 30;
    [SerializeField] float speed;


    private void Start()
    {
      //  m_Camera = Camera.main.transform;

    }
    void Update()
    {

     
        transform.position = Vector3.Lerp(transform.position, m_Camera.transform.position + m_Camera.transform.forward * distance, speed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0.0f, m_Camera.transform.rotation.y, 0.0f, m_Camera.transform.rotation.w), speed * Time.deltaTime);
    }
}
