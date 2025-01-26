using UnityEngine;

/* The camera follows the ball */

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 offset;
    private float positionOffset = 5f;
    private Vector3 velocity = Vector3.zero;
    private float lowestY;
    private Vector3 targetPosition;
    private float setX;

    private void Start() 
    {
        offset = transform.position - target.position;
        lowestY = target.position.y;
        setX = target.position.x;
    }

    private void LateUpdate ()
    {
        if (target.position.y < lowestY)  // only lowers the camera, not up when it bounces
        {
            targetPosition = new Vector3(setX, target.position.y, target.position.z) + offset;
            targetPosition.y = Mathf.Max(targetPosition.y, transform.position.y - positionOffset);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0);
            lowestY = target.position.y;
        }
    }
}
