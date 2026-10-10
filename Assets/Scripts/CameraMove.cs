using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;
     void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(targetTransform == null)
        {
            return;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position + offset, ref velocity, smoothTime);
        }
    }
}
