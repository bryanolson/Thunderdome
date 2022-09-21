using UnityEngine;

public class PointAtCamera : MonoBehaviour
{
    Transform cam;

    public void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
        transform.eulerAngles.Set(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
}