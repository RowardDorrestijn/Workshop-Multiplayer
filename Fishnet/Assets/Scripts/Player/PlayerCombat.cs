using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private bool isSlashing;
    private PlayerLooker playerLooker;

    void Awake()
    {
        animator = GetComponent<Animator>();
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
