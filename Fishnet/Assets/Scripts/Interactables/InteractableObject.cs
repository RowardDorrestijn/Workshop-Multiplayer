using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : NetworkBehaviour, IInteractable
{
    private Outline outline;
    private bool isLookedAt;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void HighLight()
    {
        isLookedAt = true;
    }

    void Update()
    {
        outline.enabled = isLookedAt;
    }

    void LateUpdate()
    {
        isLookedAt = false;
    }

    public virtual void Interact(GameObject player) { }
}