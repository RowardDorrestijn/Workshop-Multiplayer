using UnityEngine;

public class PlayerLooker : MonoBehaviour
{
    public float interactionDistance = 3f;
    public float interactionRadius = 0.5f;
    private Camera cam;

    void Awake() => cam = Camera.main;

    private void Update()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * interactionDistance, Color.green);
    }

    public bool TryGetLookedAt(out RaycastHit hit)
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        return Physics.Raycast(ray, out hit, interactionDistance);
    }
}