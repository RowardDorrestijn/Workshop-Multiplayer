using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.Networking;

public class PlayerCombat : NetworkBehaviour
{
    private NetworkAnimator animator;
    private bool isSlashing;
    private PlayerLooker playerLooker;

    void Awake()
    {
        animator = GetComponent<NetworkAnimator>();
        playerLooker = GetComponent<PlayerLooker>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed && !isSlashing)
        {
            isSlashing = true;
            animator.SetTrigger("Swing");
        }
    }

    public override void OnStartClient()
    {
        if (!IsOwner)
            enabled = false;
    }

    public void CheckSlash()
    {
        if (playerLooker.TryGetLookedAt(out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent<TreeController>(out TreeController tree))
            {
                tree.Hit(transform);
            }
        }
    }

    public void EndSlash()
    {
        isSlashing = false;
    }
}