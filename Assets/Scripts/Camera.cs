using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject car;

    void Start()
    {

    }

    void Update()
    {
        Vector3 carWorldPosition = car.transform.TransformPoint(0, 4f, -4f);
        transform.position = Vector3.Lerp(transform.position, carWorldPosition, Time.deltaTime * 2);
        transform.LookAt(car.transform);
    }
}
