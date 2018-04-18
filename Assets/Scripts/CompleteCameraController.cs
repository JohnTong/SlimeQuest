using UnityEngine;
using System.Collections;

public class CompleteCameraController : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }
    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position , targetCamPos , smoothing * Time.deltaTime);
    }
}
