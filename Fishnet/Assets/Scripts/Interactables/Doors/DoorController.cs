using System.Collections;
using UnityEngine;

public class DoorController : InteractableObject
{
    private bool isOpen = false;
    private bool isMoving = false;

    public float openAngle = 90f;
    public float speed = 2f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public override void Interact(GameObject player)
    {
        if (!isMoving)
        {
            StartCoroutine(RotateDoor());
        }
    }

    private IEnumerator RotateDoor()
    {
        isMoving = true;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = isOpen ? closedRotation : openRotation;

        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * speed;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, time);
            yield return null;
        }

        transform.rotation = targetRot;
        isOpen = !isOpen;
        isMoving = false;
    }
}
