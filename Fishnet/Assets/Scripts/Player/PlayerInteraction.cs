using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerLooker looker;

    void Update()
    {
        if (looker.TryGetLookedAt(out RaycastHit hit))
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log(hit.collider.name);
            }
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.HighLight();

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    interactable.Interact(gameObject);
                }
            }
        }

        
    }

    void Awake()
    {
        looker = GetComponent<PlayerLooker>();
    }
}
