using UnityEngine;

public class LockHandsRotation : MonoBehaviour
{
    public Transform cameraTransform;

    void LateUpdate()
    {
        transform.position = cameraTransform.position;

        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
    }
}
