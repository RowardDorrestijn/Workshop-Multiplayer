using FishNet.Object;
using UnityEngine;

public abstract class AnimationNetworkBehaviour : NetworkBehaviour
{
    [ServerRpc]
    protected void BroadcastAnimationTrigger(string trigger)
    {
        BroadcastAnimationTriggerObserver(trigger);
    }

    [ObserversRpc]
    private void BroadcastAnimationTriggerObserver(string trigger)
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger(trigger);
    }

    [ServerRpc]
    protected void BroadcastAnimationVariable(string variable, float value)
    {
        BroadcastAnimationVariableObserver(variable, value);
    }

    [ObserversRpc]
    private void BroadcastAnimationVariableObserver(string variable, float value)
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetFloat(variable, value);
    }
}